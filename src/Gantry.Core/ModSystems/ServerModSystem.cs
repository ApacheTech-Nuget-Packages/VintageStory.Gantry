using Gantry.Core.ModSystems.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Gantry.Core.ModSystems
{
    /// <summary>
    ///     Acts as a base class for Server-Side Only ModSystems. Derived classes will only be loaded on the Server.
    /// </summary>
    /// <seealso cref="ModSystemBase" />
    public abstract class ServerModSystem : ModSystemBase
    {
        /// <summary>
        ///     The core API implemented by the server. The main interface for accessing the server. Contains all sub-components, and some miscellaneous methods.
        /// </summary>
        protected ICoreServerAPI Sapi => UApi as ICoreServerAPI;

        /// <summary>
        ///     Returns if this mod should be loaded for the given app side.
        /// </summary>
        /// <param name="forSide">For side.</param>
        /// <returns><c>true</c> if the mod should be loaded on the specified side, <c>false</c> otherwise.</returns>
        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return forSide.IsServer();
        }
    }
}