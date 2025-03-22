namespace Gantry.Services.EasyX.Abstractions;

/// <summary>
///     Represents the base settings for each EasyX feature within this mod.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.All)]
public interface IEasyXClientSettings
{
    /// <summary>
    ///     Determines whether the feature should be used.
    /// </summary>
    [DoNotPrune]
    bool Enabled { get; set; }
}