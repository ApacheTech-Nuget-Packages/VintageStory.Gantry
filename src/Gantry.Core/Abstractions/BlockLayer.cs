using System;
using JetBrains.Annotations;

namespace Gantry.Core.Abstractions
{
    /// <summary>
    ///     Constants used for GetBlock or GetBlockId calls throughout the engine, to guide whether the block should be read from the solid blocks layer, the fluids layer, or perhaps both.
    ///     <br />The game engine supports different block "layers" in 1.17+.  Currently there is a solid blocks layer (e.g. terrain, loose stones, plants, reeds) and a fluids layer (e.g. still water, flowing water, lava, lake ice).  Both layers can contain a block at the same position.
    ///     <br />The .Default access is usually fine for getting blocks in the general case, but if checking for the presence of water, lake ice or lava, the .Fluids access should be used
    /// </summary>
    [Flags]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public enum BlockLayer
    {
        /// <summary>
        ///     Returns the contents of the 'solid blocks' layer; but for backwards compatibility, if it is empty (air) and a fluid is present, returns the fluid instead
        ///     <br />Useful when checking for air blocks, i.e. if 0 / air is returned then there is no block in *any* of the layers at this position.   Also maintains compability with 1.16.x code for 1.16.x worlds (Where there can never be both a solid block and a fluid in the same position)
        ///     <br />In other situations mods coded for 1.17+ should ideally not use this but should instead specify SolidBlocks or Fluids explicitly
        /// </summary>
        Default = 0x00,

        /// <summary>
        ///     Returns only the contents of the blocks layer, even if a liquid or ice is present
        /// </summary>
        SolidBlocks = 0x01,

        /// <summary>
        ///     Returns only the contents of the fluids layer, even if a non-air block is present in the blocks layer<br />
        ///     The fluids layer can include also Lake Ice - so not necessarily a liquid!!!  Will never return null.  For no fluid present you'll get a block instance with block code "air" and id 0
        /// </summary>
        Fluids = 0x10,

        /// <summary>
        ///     Returns the contents of the fluids layer, unless it is empty in which case returns the solid blocks layer - useful for generating the RainHeightMap for example
        /// </summary>
        FluidOrElseSolid = Fluids | SolidBlocks
    }
}