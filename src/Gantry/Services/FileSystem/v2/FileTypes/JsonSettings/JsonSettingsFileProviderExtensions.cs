using Gantry.Services.FileSystem.Enums;
using Gantry.Services.FileSystem.v2.Abstractions;
using Gantry.Services.FileSystem.v2.DataStructures;

namespace Gantry.Services.FileSystem.v2.FileTypes.JsonSettings;

/// <summary>
///     Extension methods to aid providing JSON settings files to the user.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class JsonSettingsFileProviderExtensions
{
    /// <summary>
    ///     Gets a JSON file, previously registered with the file provider.
    /// </summary>
    /// <param name="provider">The provider.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="scope">The scope of the file.</param>
    /// <returns>A <see cref="IJsonSettingsFile"/> representation of the file, on the file system.</returns>
    public static IJsonSettingsFile? GetJsonSettingsFile(this IFileProvider provider, string fileName, FileScope scope)
        => provider.Wrap<IJsonSettingsFile>(fileName, scope);

    /// <summary>
    ///     Gets the settings file for the specific scope, on the Client.
    /// </summary>
    /// <param name="provider">The provider.</param>
    /// <param name="scope">The scope of the file.</param>
    /// <returns>A <see cref="IJsonSettingsFile"/> representation of the file, on the file system.</returns>
    public static IJsonSettingsFile? GetClientSettings(this IFileProvider provider, FileScope scope)
        => provider.Wrap<IJsonSettingsFile>("Client.settings.json", scope);

    /// <summary>
    ///     Gets the settings file for the specific scope, on the Server.
    /// </summary>
    /// <param name="provider">The provider.</param>
    /// <param name="scope">The scope of the file.</param>
    /// <returns>A <see cref="IJsonSettingsFile"/> representation of the file, on the file system.</returns>
    public static IJsonSettingsFile? GetServerSettings(this IFileProvider provider, FileScope scope)
        => provider.Wrap<IJsonSettingsFile>("Server.settings.json", scope);

    /// <summary>
    ///     Gets the settings file for the specific scope, for the current app side.
    /// </summary>
    /// <param name="provider">The provider.</param>
    /// <param name="scope">The scope of the file.</param>
    /// <param name="side">Which side to get the settings for.</param>
    /// <returns>A <see cref="IJsonSettingsFile"/> representation of the file, on the file system.</returns>
    public static IJsonSettingsFile? GetSettings(this IFileProvider provider, FileScope scope, EnumAppSide side)
        => provider.Wrap<IJsonSettingsFile>($"{side}.settings.json", scope);

    /// <summary>
    ///     Gets the world settings file for the current app side.
    /// </summary>
    /// <param name="provider">The provider.</param>
    /// <param name="side">Which side to get the settings for.</param>
    /// <returns>A <see cref="IJsonSettingsFile"/> representation of the file, on the file system.</returns>
    public static IJsonSettingsFile? GetWorldSettings(this IFileProvider provider, EnumAppSide side)
        => provider.Wrap<IJsonSettingsFile>($"{side}.settings.json", FileScope.World);

    /// <summary>
    ///     Gets the global settings file for the current app side.
    /// </summary>
    /// <param name="provider">The provider.</param>
    /// <param name="side">Which side to get the settings for.</param>
    /// <returns>A <see cref="IJsonSettingsFile"/> representation of the file, on the file system.</returns>
    public static IJsonSettingsFile? GetGlobalSettings(this IFileProvider provider, EnumAppSide side)
        => provider.Wrap<IJsonSettingsFile>($"{side}.settings.json", FileScope.Global);
}