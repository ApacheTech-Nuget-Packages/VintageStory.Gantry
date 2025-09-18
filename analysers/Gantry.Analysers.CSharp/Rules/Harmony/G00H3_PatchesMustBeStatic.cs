using Gantry.Analysers.CSharp.Abstractions;
using Gantry.Analysers.CSharp.Extensions;

namespace Gantry.Analysers.CSharp.Rules.Harmony;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class G00H3_PatchesMustBeStatic : GantryDiagnosticAnalyserBase
{
    protected override string DiagnosticId => "G00H3";
 
    /// <summary>
    ///     The message shown to the user when the diagnostic is reported.
    /// </summary>
    protected override LocalizableString Message => "Harmony patch methods must be static";

    public override void RegisterActions(AnalysisContext context)
    {
        context.RegisterSymbolAction(AnalyseMethod, SymbolKind.Method);
    }

    private void AnalyseMethod(SymbolAnalysisContext symbolContext)
    {
        if (symbolContext.Symbol is not IMethodSymbol method) return;
        if (!method.IsHarmonyPatch()) return;
        if (method.IsStatic) return;
        ReportDiagnostic(symbolContext, method);
    }

    private void ReportDiagnostic(SymbolAnalysisContext symbolContext, ISymbol symbol)
    {
        var location = symbol.Locations.First();
        var descriptor = SupportedDiagnostics.First();
        symbolContext.ReportDiagnostic(Diagnostic.Create(descriptor, location));
    }
}