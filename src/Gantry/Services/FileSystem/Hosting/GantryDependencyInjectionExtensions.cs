﻿using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using ApacheTech.Common.Extensions.System;
using Gantry.Services.FileSystem.Abstractions.Contracts;
using Gantry.Services.FileSystem.Configuration;
using Gantry.Services.FileSystem.Enums;
using JetBrains.Annotations;
using Vintagestory.API.Common;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Gantry.Services.FileSystem.Hosting;

/// <summary>
///     Extension methods to aid the registration of the File System service, into a Gantry MDK IOC Container.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class GantryDependencyInjectionExtensions
{
    /// <summary>
    ///     Attempts to add the file system service to the service collection, as service type <see cref="IFileSystemService"/>.
    /// </summary>
    /// <param name="services">The services collection to add the service to.</param>
    /// <param name="api">The universal Core API.</param>
    /// <param name="options">The options to pass to the service.</param>
    /// <returns>A reference to this instance, after this operation has completed.</returns>
    public static IServiceCollection AddFileSystemService(
        this IServiceCollection services, 
        ICoreAPI api,
        Action<FileSystemServiceOptions> options = null)
    {
        var fileSystemServiceOptions = FileSystemServiceOptions.Default.With(options);
        services.TryAddSingleton<IFileSystemService>(new FileSystemService(api, fileSystemServiceOptions));

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
    public static IServiceCollection AddFeatureSettings<TSettings>(this IServiceCollection services, FileScope scope, string featureName = null) where TSettings : class, new()
    {
        if (string.IsNullOrWhiteSpace(featureName)) featureName = typeof(TSettings).Name.Replace("Settings", "");
        services.AddSingleton(_ => ModSettings.For(scope).Feature<TSettings>(featureName));
        return services;
    }

    /// <summary>
    ///     Adds a per-world settings service for a specific feature.
    /// </summary>
    /// <typeparam name="TSettings">The type of the settings.</typeparam>
    /// <param name="services">The services collection to add the service to.</param>
    /// <param name="featureName">The name of the feature.</param>
    /// <returns>A reference to this instance, after this operation has completed.</returns>
    public static IServiceCollection AddFeatureWorldSettings<TSettings>(this IServiceCollection services, string featureName = null) where TSettings : class, new()
    {
        if (string.IsNullOrWhiteSpace(featureName)) featureName = typeof(TSettings).Name.Replace("Settings", "");
        services.AddSingleton(_ => ModSettings.World.Feature<TSettings>(featureName));
        return services;
    }

    /// <summary>
    ///     Adds a global settings service for a specific feature.
    /// </summary>
    /// <typeparam name="TSettings">The type of the settings.</typeparam>
    /// <param name="services">The services collection to add the service to.</param>
    /// <param name="featureName">The name of the feature.</param>
    /// <returns>A reference to this instance, after this operation has completed.</returns>
    public static IServiceCollection AddFeatureGlobalSettings<TSettings>(this IServiceCollection services, string featureName = null) where TSettings : class, new()
    {
        if (string.IsNullOrWhiteSpace(featureName)) featureName = typeof(TSettings).Name.Replace("Settings", "");
        services.AddSingleton(_ => ModSettings.Global.Feature<TSettings>(featureName));
        return services;
    }
}