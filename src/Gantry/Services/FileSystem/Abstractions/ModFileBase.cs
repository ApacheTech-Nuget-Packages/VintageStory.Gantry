using System.IO;
using Gantry.Services.FileSystem.Abstractions.Contracts;
using Gantry.Services.FileSystem.Enums;

namespace Gantry.Services.FileSystem.Abstractions
{
    /// <summary>
    ///     A wrapper of a <see cref="FileInfo" /> for a specific file on on the filesystem. This class cannot be inherited.
    /// </summary>
    public abstract class ModFileBase : IModFileBase
    {
        /// <summary>
        ///     Initialises a new instance of the <see cref="ModFileInfo" /> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        protected ModFileBase(string filePath)
        {
            ModFileInfo = new FileInfo(filePath);
        }

        /// <summary>
        ///     Initialises a new instance of the <see cref="ModFileInfo" /> class.
        /// </summary>
        /// <param name="fileInfo">The file information.</param>
        protected ModFileBase(FileInfo fileInfo)
        {
            ModFileInfo = fileInfo;
        }

        /// <summary>
        ///     Gets the type of the file, be it JSON, or Binary.
        /// </summary>
        /// <value>The type of the file.</value>
        public abstract FileType FileType { get; }

        /// <summary>
        ///     Retrieves the underlying <see cref="FileInfo" /> object for the given file.
        /// </summary>
        /// <returns>
        /// A <see cref="FileInfo" /> object, instantiated with the given file's fully qualified path, as registered with the service.
        /// </returns>
        public FileInfo AsFileInfo()
        {
            return ModFileInfo;
        }

        /// <summary>
        ///     Retrieves the absolute, fully qualified path to the file.
        /// </summary>
        public string Path => ModFileInfo.FullName;

        /// <summary>
        ///     Gets the underlying mod file information.
        /// </summary>
        /// <value>
        ///     The underlying mod file information.
        /// </value>
        protected FileInfo ModFileInfo { get; }
    }
}