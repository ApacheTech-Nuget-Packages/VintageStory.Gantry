using Gantry.Analysers.CSharp.Abstractions;
using Gantry.Analysers.CSharp.Extensions;

namespace Gantry.Analysers.CSharp.Rules.Harmony;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class G00H2_PatchesMustSpecifySide : GantryDiagnosticAnalyserBase
{
    protected override string DiagnosticId => "G00H2";
 
    /// <summary>
    ///     The message shown to the user when the diagnostic is reported.
    /// </summary>
    protected override LocalizableString Message => "All patches must specify an app side";

    public override void RegisterActions(AnalysisContext context)
    {
        context.RegisterSymbolAction(AnalyseMethod, SymbolKind.Method);
    }

    private void AnalyseMethod(SymbolAnalysisContext symbolContext)
    {
        if (symbolContext.Symbol is not IMethodSymbol method) return;
        if (!method.IsHarmonyPatch()) return;

        var compilation = symbolContext.Compilation;
        var harmonySidedAttr = compilation.GetTypeByMetadataName("Gantry.Services.HarmonyPatches.Annotations.HarmonySidedPatchAttribute");
        var runsOnAttr = compilation.GetTypeByMetadataName("Gantry.Services.HarmonyPatches.Annotations.RunsOnAttribute");
        if (method.IsDecoratredWithAny(harmonySidedAttr, runsOnAttr)) return;

        ReportDiagnostic(symbolContext, method);
    }

    private void ReportDiagnostic(SymbolAnalysisContext symbolContext, IMethodSymbol method)
    {
        var location = method.Locations.First();
        var descriptor = SupportedDiagnostics.First();
        symbolContext.ReportDiagnostic(Diagnostic.Create(descriptor, location));
    }
}