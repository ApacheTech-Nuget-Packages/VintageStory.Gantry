using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace Gantry.Analysers.CSharp.CodeFixes;

/// <summary>
///     Provides a code fix that removes the <see cref="Gantry.Services.HarmonyPatches.Annotations.HarmonySidedPatchAttribute"/>
///     from Gantry patch classes reported by the analyser.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveHarmonySidedPatchAttributeCodeFixProvider)), Shared]
public class RemoveHarmonySidedPatchAttributeCodeFixProvider : CodeFixProvider
{
    /// <summary>
    ///     The title shown to the user for the code fix.
    /// </summary>
    private const string TITLE = "Remove attribute";

    /// <summary>
    ///     The diagnostics that this code fix can address.
    /// </summary>
    public sealed override ImmutableArray<string> FixableDiagnosticIds => ["G00H1"];

    /// <summary>
    ///     Returns the <see cref="FixAllProvider"/> used for batch fixes.
    /// </summary>
    /// <returns>
    ///     A <see cref="FixAllProvider"/> instance.
    /// </returns>
    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <summary>
    ///     Registers available code fixes for diagnostics reported in the specified <see cref="CodeFixContext"/>.
    ///     The method registers a single fix that removes the offending attribute from the containing type declaration.
    /// </summary>
    /// <param name="context">The <see cref="CodeFixContext"/> containing the diagnostics to fix.</param>
    public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics.FirstOrDefault();
        if (diagnostic is null) return Task.CompletedTask;

        var document = context.Document;

        context.RegisterCodeFix(
            CodeAction.Create(
                TITLE,
                ct => RemoveAttributeAsync(document, diagnostic, ct),
                equivalenceKey: TITLE),
            diagnostic);

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Removes any occurrences of <c>HarmonySidedPatchAttribute</c> from the type declaration that contains the
    ///     diagnostic location, preserving other attributes, and returning an updated <see cref="Document"/>.
    ///     The method resolves the attribute type via metadata, finds matching attributes on the type, and removes
    ///     either the whole attribute list if it contains only the matched attribute, or the individual attribute
    ///     otherwise.
    /// </summary>
    /// <param name="document">The document to modify.</param>
    /// <param name="diagnostic">The diagnostic that triggered the code fix.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The modified <see cref="Document"/>, or the original document if no changes were made.</returns>
    private static async Task<Document> RemoveAttributeAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null) return document;

        var node = root.FindNode(diagnostic.Location.SourceSpan);

        // Find the containing type declaration
        var typeDecl = node.FirstAncestorOrSelf<TypeDeclarationSyntax>();
        if (typeDecl is null) return document;

        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        if (semanticModel is null) return document;

        var compilation = semanticModel.Compilation;
        var harmonySidedAttrSymbol = compilation.GetTypeByMetadataName("Gantry.Services.HarmonyPatches.Annotations.HarmonySidedPatchAttribute");
        if (harmonySidedAttrSymbol is null)
        {
            // Nothing to do if we can't resolve the attribute type
            return document;
        }

        var attributesToRemove = typeDecl.AttributeLists
            .SelectMany(al => al.Attributes.Select(a => (list: al, attr: a)))
            .Where(pair =>
            {
                var attr = pair.attr;
                var symbolInfo = semanticModel.GetSymbolInfo(attr, cancellationToken);
                var ctor = symbolInfo.Symbol as IMethodSymbol ?? symbolInfo.CandidateSymbols.OfType<IMethodSymbol>().FirstOrDefault();
                var attrType = ctor?.ContainingType;
                return attrType is not null && SymbolEqualityComparer.Default.Equals(attrType, harmonySidedAttrSymbol);
            })
            .ToList();

        if (!attributesToRemove.Any())
            return document;

        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        foreach (var (list, attr) in attributesToRemove)
        {
            // If the attribute list contains only this attribute, remove the whole list, otherwise remove the single attribute.
            if (list.Attributes.Count == 1)
            {
                editor.RemoveNode(list);
            }
            else
            {
                editor.RemoveNode(attr, SyntaxRemoveOptions.KeepNoTrivia);
            }
        }

        var newDoc = editor.GetChangedDocument();
        return newDoc;
    }
}