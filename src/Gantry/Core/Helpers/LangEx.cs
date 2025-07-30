namespace Gantry.Core.Helpers;

/// <inheritdoc />
public class StringTranslator(string defaultDomain) : IStringTranslator
{
    /// <inheritdoc />
    public string DefaultDomain { get; init; } = defaultDomain;
}