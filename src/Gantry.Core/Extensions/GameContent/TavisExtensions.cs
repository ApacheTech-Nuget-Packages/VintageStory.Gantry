using System.Collections.Generic;
using JetBrains.Annotations;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.ServerMods.NoObf;

namespace Gantry.Core.Extensions.GameContent
{
    /// <summary>
    ///     Extension methods for Tavis JSON Patching Engine.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class TavisExtensions
    {
        private static int _dummyValue;

        /// <summary>
        /// 	Initialises static members of the <see cref="TavisExtensions"/> class.
        /// </summary>
        static TavisExtensions()
        {
            _dummyValue = 0;
        }

        /// <summary>
        ///     Applies a single patch to a JSON file.
        /// </summary>
        /// <param name="api">The core API used by the game, on both the client, and the server.</param>
        /// <param name="patch">The patch to apply.</param>
        public static void ApplyJsonPatch(this ICoreAPI api, JsonPatch patch)
        {
            // Still using these awkward pass by reference dummy values.
            // Ideally, the part of the method that actually adds the patch should be extracted.
            var jsonPatcher = api.ModLoader.GetModSystem<ModJsonPatchLoader>();
            jsonPatcher.ApplyPatch(0, patch.File, patch,
                ref _dummyValue, ref _dummyValue, ref _dummyValue);
        }

        /// <summary>
        ///     Applies a number of patches to the JSON assets of the game.
        /// </summary>
        /// <param name="api">The core API used by the game, on both the client, and the server.</param>
        /// <param name="patches">The patches to apply.</param>
        public static void ApplyJsonPatches(this ICoreAPI api, List<JsonPatch> patches)
        {
            var jsonPatcher = api.ModLoader.GetModSystem<ModJsonPatchLoader>();
            foreach (var patch in patches)
            {
                jsonPatcher.ApplyPatch(patch);
            }
        }

        /// <summary>
        ///     Applies a single patch to a JSON file.
        /// </summary>
        /// <param name="jsonPatcher">The <see cref="ModJsonPatchLoader"/> ModSystem used to patch JSON files in the game.</param>
        /// <param name="patch">The patch to apply.</param>
        public static void ApplyPatch(this ModJsonPatchLoader jsonPatcher, JsonPatch patch)
        {
            // Still using these awkward pass by reference dummy values.
            // Ideally, the part of the method that actually adds the patch should be extracted.
            jsonPatcher.ApplyPatch(0, patch.File, patch, ref _dummyValue, ref _dummyValue, ref _dummyValue);
        }

        /// <summary>
        ///     Applies a number of patches to the JSON assets of the game.
        /// </summary>
        /// <param name="jsonPatcher">The <see cref="ModJsonPatchLoader"/> ModSystem used to patch JSON files in the game.</param>
        /// <param name="patches">The patches to apply.</param>
        public static void ApplyPatches(this ModJsonPatchLoader jsonPatcher, List<JsonPatch> patches)
        {
            foreach (var patch in patches)
            {
                jsonPatcher.ApplyPatch(patch);
            }
        }

        /// <summary>
        ///     Registers a BlockBehaviour with the API, and patches the JSON file to add the behaviour to the block.
        /// </summary>
        /// <typeparam name="TBlockBehaviour">The type of <see cref="BlockBehavior"/> to register.</typeparam>
        /// <param name="api">The API to register the <see cref="BlockBehavior"/> with.</param>
        /// <param name="fileAsset">The file to patch.</param>
        public static void PatchBlockBehaviour<TBlockBehaviour>(this ICoreAPI api, AssetLocation fileAsset)
            where TBlockBehaviour : BlockBehavior
        {
            api.RegisterBlockBehaviour<TBlockBehaviour>();
            api.ApplyJsonPatch(new JsonPatch
            {
                Op = EnumJsonPatchOp.AddEach,
                File = fileAsset,
                Path = "/behaviors/-",
                Value = JsonObject.FromJson($"[{{ \"name\": \"{typeof(TBlockBehaviour).Name}\" }}]")
            });
        }

        /// <summary>
        ///     Registers a BlockEntityBehaviour with the API, and patches the JSON file to add the behaviour to the block.
        /// </summary>
        /// <typeparam name="TBehaviour">The type of <see cref="BlockEntityBehavior"/> to register.</typeparam>
        /// <param name="api">The API to register the <see cref="BlockEntityBehavior"/> with.</param>
        /// <param name="fileAsset">The file to patch.</param>
        public static void PatchBlockEntityBehaviour<TBehaviour>(this ICoreAPI api, AssetLocation fileAsset)
            where TBehaviour : BlockEntityBehavior
        {
            api.RegisterBlockEntityBehaviour<TBehaviour>();
            api.ApplyJsonPatch(new JsonPatch
            {
                Op = EnumJsonPatchOp.AddEach,
                File = fileAsset,
                Path = "/entityBehaviors",
                Value = JsonObject.FromJson($"[{{ \"name\": \"{typeof(TBehaviour).Name}\" }}]")
            });
        }
    }
}