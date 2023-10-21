using System.IO;
using Gantry.Services.FileSystem.v2.Abstractions;
using Gantry.Services.FileSystem.v2.DataStructures;
using JetBrains.Annotations;

namespace Gantry.Services.FileSystem.v2.FileTypes.Binary
{
    /// <summary>
    ///     Represents a binary file, on the file system.
    /// </summary>
    /// <seealso cref="ModFileInfo" />
    /// <seealso cref="IBinaryFile" />
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class BinaryFile : ModFileInfo, IBinaryFile
    {
        /// <summary>
        ///     Initialises a new instance of the <see cref="BinaryFile"/> class.
        /// </summary>
        /// <param name="file">A <see cref="FileInfo" /> representation of a file on the file system.</param>
        /// <param name="scope">Determines where the file is stored on the file system.</param>
        public BinaryFile(FileInfo file, FileScope scope) : base(file, scope)
        {
        }
    }
}
