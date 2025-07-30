using Gantry.Core.Abstractions;
using Gantry.Core.Abstractions.ModSystems;
using Gantry.Core.Helpers;
using Vintagestory.Common;

namespace Gantry.Core.Hosting;

/// <summary>
///     Only one derived instance of this base-class should be added to any single mod within
///     the VintageMods domain. This base-class will enable Dependency Injection, and add all
///     the domain services. Derived instances should only have minimal functionality, 
///     instantiating, and adding Application specific services to the IOC Container.
/// </summary>
/// <seealso cref="ModSystem" />
public abstract class ModHost<TModSystem>(Action<ICoreGantryAPI> onCoreLoaded, Action<GantryHostOptions>? options = null) 
    : UniversalModSystem<TModSystem>, IModHost where TModSystem : ModHost<TModSystem>
{
    private readonly Action<ICoreGantryAPI> _onCoreLoaded = onCoreLoaded;
    private readonly Action<GantryHostOptions>? _options = options;
    private readonly AsyncLocal<ICoreGantryAPI> _modCore = new();

    ICoreGantryAPI IModHost.Gantry => _modCore.Value!;

    /// <inheritdoc />
    protected override void OnShouldLoad(ICoreAPI api)
    {
        var core = GantryCore.Create(api, (ModContainer)Mod);
        _modCore.Value = core;
        core.BuildServiceProvider(GantryHostOptions.Default.With(_options));
        _onCoreLoaded?.Invoke(core);
    }

    /// <summary>
    ///     Ensures that the mod host is the first mod system to be executed.
    /// </summary>
    public override double ExecuteOrder() => double.NegativeInfinity;
}