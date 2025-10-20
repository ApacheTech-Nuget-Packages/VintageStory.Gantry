using Gantry.Core.Abstractions;
using Gantry.Services.IO.Configuration.Abstractions;

namespace Gantry.Services.EasyX.Abstractions;

/// <summary>
///     Base class for EasyX client patches.
/// </summary>
/// <typeparam name="TClientSystem">The client system type.</typeparam>
/// <typeparam name="TClientSettings">The client settings type.</typeparam>
/// <typeparam name="TServerSettings">The server settings type.</typeparam>
public abstract class EaxyXClientPatchClass<TClientSystem, TClientSettings, TServerSettings>
    where TClientSystem : EasyXClientSystemBase<TClientSystem, TClientSettings, TServerSettings>
    where TClientSettings : class, IEasyXClientSettings, new()
    where TServerSettings : FeatureSettings<TServerSettings>, IEasyXServerSettings, new()
{
    /// <summary>
    ///     The client system instance.
    /// </summary>
    protected static TClientSystem ClientSystem => EasyXClientSystemBase<TClientSystem, TClientSettings, TServerSettings>.Instance;

    /// <summary>
    ///     The client settings instance.
    /// </summary>
    protected static TClientSettings Settings => ClientSystem.Settings;

    /// <summary>
    ///     The core Gantry API.
    /// </summary>
    protected static ICoreGantryAPI Core => ClientSystem.Core;
}