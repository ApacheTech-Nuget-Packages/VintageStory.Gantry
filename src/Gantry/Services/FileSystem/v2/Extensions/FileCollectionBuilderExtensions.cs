using System;
using ApacheTech.Common.Extensions.System;
using Gantry.Services.FileSystem.v2.Abstractions;
using JetBrains.Annotations;

namespace Gantry.Services.FileSystem.v2.Extensions
{
    /// <summary>
    ///     Extension methods to aid the building of an <see cref="IFileProvider"/> from an <see cref="IFileCollection"/>.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class FileCollectionBuilderExtensions
    {
        /// <summary>
        ///     Creates a <see cref="FileProvider"/> containing files from the provided <see cref="IFileCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IFileCollection"/> containing file descriptors.</param>
        /// <returns>The <see cref="FileProvider"/>.</returns>

        public static FileProvider BuildServiceProvider(this IFileCollection services) 
            => services.BuildServiceProvider(FileProviderOptions.Default);

        /// <summary>
        ///     Creates a <see cref="FileProvider"/> containing files from the provided <see cref="IFileCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IFileCollection"/> containing service descriptors.</param>
        /// <param name="optionsFactory"> Configures various file provider behaviours.</param>
        /// <returns>The <see cref="FileProvider"/>.</returns>
        public static FileProvider BuildServiceProvider(this IFileCollection services, Action<FileProviderOptions> optionsFactory) 
            => services.BuildServiceProvider(FileProviderOptions.Default.With(optionsFactory));

        /// <summary>
        ///     Creates a <see cref="FileProvider"/> containing files from the provided <see cref="IFileCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IFileCollection"/> containing service descriptors.</param>
        /// <param name="options"> Configures various file provider behaviours.</param>
        /// <returns>The <see cref="FileProvider"/>.</returns>

        public static FileProvider BuildServiceProvider(this IFileCollection services, FileProviderOptions options) 
            => new(services, options);
    }
}