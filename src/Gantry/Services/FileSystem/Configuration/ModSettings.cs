using Gantry.Services.FileSystem.Abstractions.Contracts;
using Gantry.Services.FileSystem.Enums;

#pragma warning disable IDE0060 // Remove unused parameter

namespace Gantry.Services.FileSystem.Configuration;

/// <summary>
///     Globally accessible settings files for the mod. Populated via the <see cref="IFileSystemService"/>.
/// </summary>
public static class ModSettings
{
    /// <summary>
    ///     The global mod settings; these settings will persist through each gameworld.
    /// </summary>
    /// <value>The global settings.</value>
    internal static IJsonSettingsFile? ClientGlobal { get; set; }

    /// <summary>
    ///     The per-world mod settings; these settings can change within each gameworld.
    /// </summary>
    /// <value>The per-world settings.</value>
    internal static IJsonSettingsFile? ClientWorld { get; set; }

    /// <summary>
    ///     The global mod settings; these settings will persist through each gameworld.
    /// </summary>
    /// <value>The global settings.</value>
    internal static IJsonSettingsFile? GantryServerGlobal { get; set; }

    /// <summary>
    ///     The per-world mod settings; these settings can change within each gameworld.
    /// </summary>
    /// <value>The per-world settings.</value>
    internal static IJsonSettingsFile? GantryServerWorld { get; set; }

    /// <summary>
    ///     The global mod settings; these settings will persist through each gameworld.
    /// </summary>
    /// <value>The global settings.</value>
    internal static IJsonSettingsFile? GantryClientGlobal { get; set; }

    /// <summary>
    ///     The per-world mod settings; these settings can change within each gameworld.
    /// </summary>
    /// <value>The per-world settings.</value>
    internal static IJsonSettingsFile? GantryClientWorld { get; set; }

    /// <summary>
    ///     The global mod settings; these settings will persist through each gameworld.
    /// </summary>
    /// <value>The global settings.</value>
    internal static IJsonSettingsFile? ServerGlobal { get; set; }

    /// <summary>
    ///     The per-world mod settings; these settings can change within each gameworld.
    /// </summary>
    /// <value>The per-world settings.</value>
    internal static IJsonSettingsFile? ServerWorld { get; set; }

    /// <summary>
    ///     The per-world mod settings; these settings can change within each gameworld.
    /// </summary>
    /// <value>The per-world settings.</value>
    internal static IJsonSettingsFile? GantryWorld => ApiEx.OneOf(GantryClientWorld, GantryServerWorld);

    /// <summary>
    ///     The global mod settings; these settings will persist through each gameworld.
    /// </summary>
    /// <value>The global settings.</value>
    internal static IJsonSettingsFile? GantryGlobal => ApiEx.OneOf(GantryClientGlobal, GantryServerGlobal);

    /// <summary>
    ///     The global mod settings; these settings will persist through each gameworld.
    /// </summary>
    /// <value>The global settings.</value>
    public static IJsonSettingsFile Global => ApiEx.OneOf(ClientGlobal, ServerGlobal)!;

    /// <summary>
    ///     The per-world mod settings; these settings can change within each gameworld.
    /// </summary>
    /// <value>The per-world settings.</value>
    public static IJsonSettingsFile World => ApiEx.OneOf(ClientWorld, ServerWorld)!;

    /// <summary>
    ///     The mod settings for a specific <see cref="FileScope"/>.
    /// </summary>
    /// <value>The global settings.</value>
    public static IJsonSettingsFile For(FileScope scope, bool gantrySettings = false)
    {
        return scope switch
        {
            FileScope.Global => Global,
            FileScope.World => World,
            _ => throw new ArgumentOutOfRangeException(nameof(scope))
        };
    }

    /// <summary>
    ///     Copies the settings from one scope to another.
    /// </summary>
    /// <param name="scope">The scope to copy the settings to.</param>
    /// <param name="gantrySettings"></param>
    public static void CopyTo(FileScope scope, bool gantrySettings = false)
    {
        For(scope, gantrySettings).File.SaveFrom(For((FileScope)((int)scope ^ 1), gantrySettings).File);
    }

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public static void Dispose()
    {
        FeaturePatcher.UnpatchAll();
        ApiEx.Run(ClientDispose, ServerDispose);
    }

    private static void ClientDispose()
    {
        ClientWorld?.Dispose();
        ClientGlobal = null;

        ClientGlobal?.Dispose();
        ClientWorld = null;

        GantryClientWorld?.Dispose();
        GantryClientWorld = null;

        GantryClientGlobal?.Dispose();
        GantryClientGlobal = null;
    }

    private static void ServerDispose()
    {
        ServerGlobal?.Dispose();
        ServerGlobal = null;

        ServerWorld?.Dispose();
        ServerWorld = null;

        GantryServerWorld?.Dispose();
        GantryServerWorld = null;

        GantryServerGlobal?.Dispose();
        GantryServerGlobal = null;
    }

    internal static Harmony FeaturePatcher { get; } = new($"{ModEx.ModInfo.ModID}_ObservableFeatures");
}