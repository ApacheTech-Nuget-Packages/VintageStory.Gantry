using System;
using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using ApacheTech.Common.Extensions.System;
using Gantry.Services.FileSystem.Abstractions.Contracts;
using Gantry.Services.FileSystem.Configuration;
using JetBrains.Annotations;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Gantry.Services.FileSystem.DependencyInjection
{
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
        /// <param name="options">The options to pass to the service.</param>
        /// <returns>A reference to this instance, after the this operation has completed.</returns>
        public static IServiceCollection AddFileSystemService(
            this IServiceCollection services, 
            Action<FileSystemServiceOptions> options)
        {
            services.TryAddSingleton<IFileSystemService>(
                new FileSystemService(FileSystemServiceOptions.Default.With(options)));
            return services;
        }

        /// <summary>
        ///     Attempts to add the file system service to the service collection, as service type <see cref="IFileSystemService"/>.
        /// </summary>
        /// <param name="services">The services collection to add the service to.</param>
        /// <returns>A reference to this instance, after the this operation has completed.</returns>
        public static IServiceCollection AddFileSystemService(this IServiceCollection services)
        {
            return services.AddFileSystemService(_ => { });
        }

        /// <summary>
        ///     Adds a per-world settings service for a specific feature.
        /// </summary>
        /// <typeparam name="TSettings">The type of the settings.</typeparam>
        /// <param name="services">The services collection to add the service to.</param>
        /// <param name="featureName">The name of the feature.</param>
        /// <returns>A reference to this instance, after the this operation has completed.</returns>
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
        /// <returns>A reference to this instance, after the this operation has completed.</returns>
        public static IServiceCollection AddFeatureGlobalSettings<TSettings>(this IServiceCollection services, string featureName = null) where TSettings : class, new()
        {
            if (string.IsNullOrWhiteSpace(featureName)) featureName = typeof(TSettings).Name.Replace("Settings", "");
            services.AddSingleton(_ => ModSettings.Global.Feature<TSettings>(featureName));
            return services;
        }

        /// <summary>
        ///     Adds a local settings service for a specific feature.
        /// </summary>
        /// <typeparam name="TSettings">The type of the settings.</typeparam>
        /// <param name="services">The services collection to add the service to.</param>
        /// <param name="featureName">The name of the feature.</param>
        /// <returns>A reference to this instance, after the this operation has completed.</returns>
        public static IServiceCollection AddFeatureLocalSettings<TSettings>(this IServiceCollection services, string featureName = null) where TSettings : class, new()
        {
            if (string.IsNullOrWhiteSpace(featureName)) featureName = typeof(TSettings).Name.Replace("Settings", "");
            services.AddSingleton(_ => ModSettings.Local.Feature<TSettings>(featureName));
            return services;
        }
    }
}
