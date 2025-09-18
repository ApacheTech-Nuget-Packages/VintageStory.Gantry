namespace Gantry.Analysers.CSharp.Abstractions;

/// <summary>
///     Base class for Gantry diagnostic analysers, providing common diagnostic descriptor
///     construction, and initialisation behaviour.
/// </summary>
public abstract class GantryDiagnosticAnalyserBase : DiagnosticAnalyzer
{
    private readonly DiagnosticDescriptor _rule;

    /// <summary>
    ///     Initialises a new instance of the <see cref="GantryDiagnosticAnalyserBase"/>,
    ///     creating the underlying <see cref="DiagnosticDescriptor"/> from the analyser's
    ///     identifier, title, message, category, severity, and default enabled state.
    /// </summary>
    protected GantryDiagnosticAnalyserBase() => _rule = new(
        id: DiagnosticId,
        title: Title,
        messageFormat: Message,
        category: Category,
        defaultSeverity: Severity,
        isEnabledByDefault: IsEnabledByDefault);

    /// <summary>
    ///     The diagnostic identifier for the analyser.
    /// </summary>
    protected abstract string DiagnosticId { get; }

    /// <summary>
    ///     The localised message displayed when the diagnostic is reported.
    /// </summary>
    protected abstract LocalizableString Message { get; }

    /// <summary>
    ///     The diagnostic title, derived from <see cref="Message"/> by default.
    /// </summary>
    protected virtual string Title => Message.ToString();

    /// <summary>
    ///     The diagnostic category, by default <c>"Usage"</c>.
    /// </summary>
    protected virtual string Category => "Usage";

    /// <summary>
    ///     The default diagnostic severity, which is <see cref="DiagnosticSeverity.Error"/> by default.
    /// </summary>
    protected virtual DiagnosticSeverity Severity => DiagnosticSeverity.Error;

    /// <summary>
    ///     Whether the diagnostic is enabled by default.
    /// </summary>
    protected virtual bool IsEnabledByDefault => true;

    /// <summary>
    ///     The collection of supported diagnostics for the analyser.
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [_rule];

    /// <summary>
    ///     Initialises analysis by enabling concurrent execution, configuring generated code analysis,
    ///     and invoking <see cref="RegisterActions"/> so derived analysers can register their actions.
    /// </summary>
    /// <param name="context">The analysis context to configure.</param>
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(
            GeneratedCodeAnalysisFlags.Analyze |
            GeneratedCodeAnalysisFlags.ReportDiagnostics);

        RegisterActions(context);
    }

    /// <summary>
    ///     Registers the analysis actions with the provided <see cref="AnalysisContext"/>.
    ///     Implementations should register only the actions they require.
    /// </summary>
    /// <param name="context">The analysis context to register actions on.</param>
    public abstract void RegisterActions(AnalysisContext context);
}