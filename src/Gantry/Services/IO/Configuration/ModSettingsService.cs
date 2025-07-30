using Gantry.Core.Abstractions;
using Gantry.Core.Helpers;
using Gantry.Services.IO.Configuration.Abstractions;
using Gantry.Services.IO.DataStructures;

namespace Gantry.Services.IO.Configuration;

/// <summary>
///     A service for managing mod settings files, providing access to global, world, and gantry settings.
/// </summary>
public class ModSettingsService(ICoreGantryAPI core) : IModSettingsService
{
    private readonly AsyncLocal<IJsonSettingsFile> _globalSettings = new();
    private readonly AsyncLocal<IJsonSettingsFile> _worldSettings = new();
    private readonly AsyncLocal<IJsonSettingsFile> _gantrySettings = new();
    private readonly ICoreGantryAPI _core = core;

    /// <inheritdoc />
    public IJsonSettingsFile Global => _globalSettings.Value!;

    /// <inheritdoc />
    public IJsonSettingsFile World => _worldSettings.Value!;

    /// <inheritdoc />
    public IJsonSettingsFile Gantry =>  _gantrySettings.Value!;

    /// <inheritdoc />
    public void CopySettings(ModFileScope fromScope, ModFileScope toScope)
    {
        var fromSettings = For(fromScope);
        var toSettings = For(toScope);
        if (fromSettings is null)         {
            _core.Logger.Error($"Cannot copy settings from {fromScope} to {toScope} because the source settings are null.");
            return;
        }
        if (toSettings is null) {
            _core.Logger.Error($"Cannot copy settings from {fromScope} to {toScope} because the destination settings are null.");
            return;
        }
        toSettings.File.SaveFrom(fromSettings.File);
    }

    /// <inheritdoc />
    public void Dispose()
    {  
        _worldSettings.Value?.Dispose();
        _worldSettings.Value = null!;

        _globalSettings.Value?.Dispose();
        _globalSettings.Value = null!;

        _gantrySettings.Value?.Dispose();
        _gantrySettings.Value = null!;
    }

    /// <inheritdoc />
    public IJsonSettingsFile For(ModFileScope scope) => scope switch
    {
        ModFileScope.Global => Global,
        ModFileScope.World => World,
        ModFileScope.Gantry => Gantry,
        _ => throw new UnreachableException()
    };

    void IModSettingsService.Set(EnumAppSide side, ModFileScope scope, IJsonSettingsFile settings)
    {
        switch (scope)
        {
            case ModFileScope.Global:
                _globalSettings.Value = settings;
                break;
            case ModFileScope.World:
                _worldSettings.Value = settings;
                break;
            case ModFileScope.Gantry:
                _gantrySettings.Value = settings;
                break;
            default:
                throw new UnreachableException();
        }
    }
}