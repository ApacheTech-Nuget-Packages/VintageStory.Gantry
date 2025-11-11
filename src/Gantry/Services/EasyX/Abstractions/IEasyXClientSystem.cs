namespace Gantry.Services.EasyX.Abstractions;

/// <summary>
///     Represents the base system for each EasyX feature within this mod (client-side).
/// </summary>
/// <typeparam name="TClientSettings"></typeparam>
public interface IEasyXClientSystem<TClientSettings>
    where TClientSettings : class, IEasyXClientSettings, new()
{
    /// <summary>
    ///     The client-side settings for this feature.
    /// </summary>
    TClientSettings Settings { get; }
}
