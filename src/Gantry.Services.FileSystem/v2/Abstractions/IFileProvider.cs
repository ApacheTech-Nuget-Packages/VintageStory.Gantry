using System.Collections.Generic;
using Gantry.Services.FileSystem.v2.DataStructures;
using System.IO;
using Gantry.Core.Diagnostics;

namespace Gantry.Services.FileSystem.v2.Abstractions
{
    /// <summary>
    ///     Defines a mechanism for retrieving a file, from the file system.
    /// </summary>
    public interface IFileProvider
    {
        /// <summary>
        ///     Gets the file, with the specified name, and scope.
        /// </summary>
        /// <param name="fileName">The name of the file, including the extension.</param>
        /// <param name="scope">Determines where the file is stored on the file system.</param>
        /// <returns>
        ///     A <see cref="FileInfo"/> representation of a file on the file system.
        /// </returns>
        FileInfo GetFile(string fileName, FileScope scope);

        /// <summary>
        ///     Gets the service object of the specified type.
        /// </summary>
        /// <param name="fileName">The name of the file to get.</param>
        /// <param name="scope">The scope of the file to get.</param>
        /// <exception cref="KeyNotFoundException">No file named `{fileName}`, of scope `{scope}`, has been registered.</exception>
        /// <exception cref="GantryException">No wrapper found for file extension, `{file.Extension}`</exception>
        internal T Wrap<T>(string fileName, FileScope scope);
    }
}