using Gantry.Core.ModSystems.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Gantry.Core.ModSystems
{
    /// <summary>
    ///     Acts as a base class for Client-Side Only ModSystems. Derived classes will only be loaded on the Client.
    /// </summary>
    /// <seealso cref="ModSystemBase" />
    public abstract class ClientModSystem : ModSystemBase
    {
        /// <summary>
        ///     The core API implemented by the client. The main interface for accessing the client. Contains all sub-components, and some miscellaneous methods.
        /// </summary>
        protected ICoreClientAPI Capi => UApi as ICoreClientAPI;

        /// <summary>
        ///     Returns if this mod should be loaded for the given app side.
        /// </summary>
        /// <param name="forSide">For side.</param>
        /// <returns><c>true</c> if the mod should be loaded on the specified side, <c>false</c> otherwise.</returns>
        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return forSide.IsClient();
        }

        /// <inheritdoc />
        public override void StartServerSide(ICoreServerAPI api) { }

        /// <inheritdoc />
        protected sealed override void StartPreServerSide(ICoreServerAPI api) { }

        /// <inheritdoc />
        protected sealed override void StartPreUniversal(ICoreAPI api) { }
    }
}