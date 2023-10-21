using System.Collections.Generic;
using System.IO;
using System.Linq;
using ApacheTech.Common.Extensions.System;
using Gantry.Core.Diagnostics;
using Gantry.Services.FileSystem.Extensions;
using Gantry.Services.FileSystem.v2.Abstractions;
using Gantry.Services.FileSystem.v2.DataStructures;
using JetBrains.Annotations;

namespace Gantry.Services.FileSystem.v2
{
    /// <summary>
    ///     Default implementation of <see cref="IFileProvider" />.
    /// </summary>
    /// <seealso cref="IFileProvider" />
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class FileProvider : IFileProvider
    {
        private readonly IEnumerable<FileDescriptor> _fileDescriptors;
        private readonly FileProviderOptions _options;
        private readonly IEnumerable<IFileTypeWrapper> _wrappers;

        /// <summary>
        ///     Initialises a new instance of the <see cref="IFileProvider"/> class.
        /// </summary>
        /// <param name="files">The <see cref="IFileProvider"/> containing service descriptors.</param>
        /// <param name="options"> Configures various service provider behaviours.</param>
        public FileProvider(IEnumerable<FileDescriptor> files, FileProviderOptions options)
        {
            _fileDescriptors = files;
            _options = options;

            _wrappers = GetType().Assembly
                .InstantiateAllTypesImplementing<IFileTypeWrapper>();
        }

        /// <summary>
        ///     Gets the service object of the specified type.
        /// </summary>
        /// <param name="fileName">The name of the file to get.</param>
        /// <param name="scope">The scope of the file to get.</param>
        public FileInfo GetFile(string fileName, FileScope scope)
        {
            var descriptor = _fileDescriptors
                .Where(p => p.Scope == scope)
                .SingleOrDefault(p => p.FileName == fileName);

            if (descriptor is null)
            {
                throw new KeyNotFoundException($"No file named `{fileName}`, of scope `{scope}`, has been registered.");
            }
            return descriptor.File;
        }

        /// <summary>
        ///     Gets the service object of the specified type.
        /// </summary>
        /// <param name="fileName">The name of the file to get.</param>
        /// <param name="scope">The scope of the file to get.</param>
        /// <exception cref="KeyNotFoundException">No file named `{fileName}`, of scope `{scope}`, has been registered.</exception>
        /// <exception cref="GantryException">No wrapper found for file extension, `{file.Extension}`</exception>
        T IFileProvider.Wrap<T>(string fileName, FileScope scope)
        {

            var descriptor = _fileDescriptors
                .Where(p => p.Scope == scope)
                .SingleOrDefault(p => p.FileName == fileName);

            if (descriptor is null)
            {
                throw new KeyNotFoundException($"No file named `{fileName}`, of scope `{scope}`, has been registered.");
            }

            var file = descriptor.File;

            foreach (var wrapper in _wrappers)
            {
                if (!wrapper.Extensions.Contains(file.Extension)) continue;
                return wrapper.Wrap(file, scope).To<T>();
            }

            throw new GantryException($"No wrapper found for file extension, `{file.Extension}`");
        }

    }
}