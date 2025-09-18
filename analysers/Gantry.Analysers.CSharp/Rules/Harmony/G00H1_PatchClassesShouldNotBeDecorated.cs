using Gantry.Analysers.CSharp.Abstractions;

namespace Gantry.Analysers.CSharp.Rules.Harmony;

/// <summary>
///     Analyses named type symbols to find Gantry patch classes that are incorrectly decorated
///     with <c>HarmonySidedPatchAttribute</c>, and reports a diagnostic when such usage is found.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class G00H1_PatchClassesShouldNotBeDecorated : GantryDiagnosticAnalyserBase
{
    /// <summary>
    ///     The diagnostic identifier used when a Gantry patch class is decorated with Harmony patch attributes.
    /// </summary>
    protected override string DiagnosticId => "G00H1";
 
    /// <summary>
    ///     The message shown to the user when the diagnostic is reported.
    /// </summary>
    protected override LocalizableString Message => "Do not decorate GantryPatch classes with HarmonyPatch attributes";

    /// <summary>
    ///     Registers analysis actions with the provided <see cref="AnalysisContext"/>.
    ///     The analyser registers a symbol action that targets named type symbols.
    /// </summary>
    /// <param name="context">The analysis context to register actions on.</param>
    public override void RegisterActions(AnalysisContext context)
    {
        // Register the named method as the symbol action handler
        context.RegisterSymbolAction(AnalyseNamedType, SymbolKind.NamedType);
    }

    /// <summary>
    ///     Analyses a named type symbol to determine whether it implements <c>IGantryPatchClass</c>,
    ///     and whether it has been annotated with <c>HarmonySidedPatchAttribute</c>. If both conditions are met,
    ///     a diagnostic is reported on the type declaration location.
    /// </summary>
    /// <param name="symbolContext">The symbol analysis context provided by Roslyn.</param>
    private void AnalyseNamedType(SymbolAnalysisContext symbolContext)
    {
        if (symbolContext.Symbol is not INamedTypeSymbol namedType)
            return;

        // Only target classes
        if (namedType.TypeKind != TypeKind.Class)
            return;

        var compilation = symbolContext.Compilation;

        // Resolve the IGantryPatchClass symbol from the compilation. This allows detection
        // when the interface is implemented directly or by a base class in the inheritance chain.
        var gantryPatchInterface = compilation.GetTypeByMetadataName("Gantry.Services.HarmonyPatches.Abstractions.IGantryPatchClass");
        if (gantryPatchInterface is null)
        {
            // Could not resolve the interface symbol - abort.
            return;
        }

        // Check whether the type implements the interface (directly or via base types)
        var implementsGantryPatch = namedType.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, gantryPatchInterface));
        if (!implementsGantryPatch)
            return;

        // This is a Gantry patch class - check for the prohibited HarmonySidedPatchAttribute, or any attribute derived from it.
        var harmonySidedAttr = compilation.GetTypeByMetadataName("Gantry.Services.HarmonyPatches.Annotations.HarmonySidedPatchAttribute");
        var hasSidedPatchAttribute = false;
        if (harmonySidedAttr is not null)
        {
            foreach (var attr in namedType.GetAttributes())
            {
                var attrType = attr.AttributeClass;
                while (attrType is not null)
                {
                    if (SymbolEqualityComparer.Default.Equals(attrType, harmonySidedAttr))
                    {
                        hasSidedPatchAttribute = true;
                        break;
                    }
                    attrType = attrType.BaseType;
                }

                if (hasSidedPatchAttribute)
                    break;
            }
        }

        if (hasSidedPatchAttribute)
        {
            // Report diagnostic on the type declaration location
            var location = namedType.Locations.FirstOrDefault();
            var descriptor = new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, IsEnabledByDefault);
            symbolContext.ReportDiagnostic(Diagnostic.Create(descriptor, location));
        }
    }
}