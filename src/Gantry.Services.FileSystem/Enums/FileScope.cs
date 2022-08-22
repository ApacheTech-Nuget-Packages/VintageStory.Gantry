using System.ComponentModel;

namespace Gantry.Services.FileSystem.Enums
{
    /// <summary>
    ///     Specifies the scope of a file saved to the user's game folder.
    /// </summary>
    public enum FileScope
    {
        /// <summary>
        ///     Denotes that a file is held in global scope, for all multi-player and single-player worlds.
        /// </summary>
        [Description("Global File")] Global,

        /// <summary>
        ///     Denotes that a file is created for each world a player enters.
        /// </summary>
        [Description("Per World File")] World,

        /// <summary>
        ///     Denotes that a file is local to the mod, and not copied to the host filesystem.
        /// </summary>
        [Description("Local  File")] Local
    }
}
