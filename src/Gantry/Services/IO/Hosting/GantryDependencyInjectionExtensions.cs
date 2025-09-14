using Gantry.Services.EasyX;
using Gantry.Services.IO.Abstractions.Contracts;
using Gantry.Services.IO.Configuration;
using Gantry.Services.IO.Configuration.Abstractions;
using Gantry.Services.IO.DataStructures;
using Gantry.Services.IO.Helpers;

namespace Gantry.Services.IO.Hosting;

/// <summary>
///     Extension methods to aid the registration of the File System service, into a Gantry MDK IOC Container.
/// </summary>
public static class GantryDependencyInjectionExtensions
{
    /// <summary>
    ///     Attempts to add the file system service to the service collection, as service type <see cref="IFileSystemService"/>.
    /// </summary>
    /// <param name="services">The services collection to add the service to.</param>
    /// <param name="options">The options to pass to the service.</param>
    /// <returns>A reference to this instance, after this operation has completed.</returns>
    public static IServiceCollection AddFileSystemService(
        this IServiceCollection services,
        Action<FileSystemServiceOptions>? options = null)
    {
        services.TryAddSingleton<GantryPaths>();
        services.TryAddSingleton(FileSystemServiceOptions.Default.With(options));
        services.TryAddSingleton<IModSettingsService, ModSettingsService>();
        services.TryAddSingleton<IFileSystemService, FileSystemService>();

        return services;
    }

    /// <summary>
    ///     Adds a per-world settings service for a specific feature.
    /// </summary>
    /// <typeparam name="TSettings">The type of the settings.</typeparam>
    /// <param name="services">The services collection to add the service to.</param>
    /// <param name="scope">The settings file to add the feature to.</param>
    /// <param name="featureName">The name of the feature.</param>
    /// <returns>A reference to this instance, after this operation has completed.</returns>
    public static IServiceCollection AddFeatureSettings<TSettings>(this IServiceCollection services, ModFileScope scope, string? featureName = null) where TSettings : FeatureSettings<TSettings>, new()
    {
        if (string.IsNullOrWhiteSpace(featureName)) featureName = typeof(TSettings).Name.Replace("Settings", "");
        services.AddSingleton(sp =>
        {
            var settingsService = sp.GetRequiredService<IModSettingsService>();
            return settingsService.For(scope).Feature<TSettings>(featureName);
        });
        return services;
    }

    /// <summary>
    ///     Adds a per-world settings service for a specific feature.
    /// </summary>
    /// <typeparam name="TSettings">The type of the settings.</typeparam>
    /// <param name="services">The services collection to add the service to.</param>
    /// <param name="featureName">The name of the feature.</param>
    /// <returns>A reference to this instance, after this operation has completed.</returns>
    public static IServiceCollection AddScopedFeatureSettings<TSettings>(this IServiceCollection services, string? featureName = null) where TSettings : FeatureSettings<TSettings>, new()
    {
        if (string.IsNullOrWhiteSpace(featureName)) featureName = typeof(TSettings).Name.Replace("Settings", "");
        services.AddTransient(sp =>
        {
            var settingsService = sp.GetRequiredService<IModSettingsService>();
            var configuration = settingsService.Global.Feature<ConfigurationSettings>();
            var scope = configuration.Scope;
            return settingsService.For(scope).Feature<TSettings>(featureName);
        });
        return services;
    }


    /// <summary>
    ///     Adds a per-world settings service for a specific feature.
    /// </summary>
    /// <typeparam name="TSettings">The type of the settings.</typeparam>
    /// <param name="services">The services collection to add the service to.</param>
    /// <param name="featureName">The name of the feature.</param>
    /// <returns>A reference to this instance, after this operation has completed.</returns>
    public static IServiceCollection AddFeatureWorldSettings<TSettings>(this IServiceCollection services, string? featureName = null) where TSettings : FeatureSettings<TSettings>, new()
        => services.AddFeatureSettings<TSettings>(ModFileScope.World, featureName);

    /// <summary>
    ///     Adds a global settings service for a specific feature.
    /// </summary>
    /// <typeparam name="TSettings">The type of the settings.</typeparam>
    /// <param name="services">The services collection to add the service to.</param>
    /// <param name="featureName">The name of the feature.</param>
    /// <returns>A reference to this instance, after this operation has completed.</returns>
    public static IServiceCollection AddFeatureGlobalSettings<TSettings>(this IServiceCollection services, string? featureName = null) where TSettings : FeatureSettings<TSettings>, new()
        => services.AddFeatureSettings<TSettings>(ModFileScope.Global, featureName);

    /// <summary>
    ///     Adds a gantry settings service for a specific feature.
    /// </summary>
    /// <typeparam name="TSettings">The type of the settings.</typeparam>
    /// <param name="services">The services collection to add the service to.</param>
    /// <param name="featureName">The name of the feature.</param>
    /// <returns>A reference to this instance, after this operation has completed.</returns>
    internal static IServiceCollection AddGantrySettings<TSettings>(this IServiceCollection services, string? featureName = null) where TSettings : FeatureSettings<TSettings>, new()
        => services.AddFeatureSettings<TSettings>(ModFileScope.Gantry, featureName);
}