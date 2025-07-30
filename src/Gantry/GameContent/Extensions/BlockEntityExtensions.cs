using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Gantry.GameContent.Extensions;

/// <summary>
///     Provides extension methods for <see cref="BlockEntity"/> to simplify attribute encoding, decoding, and comparison.
/// </summary>
public static class BlockEntityExtensions
{
    /// <summary>
    ///     Decodes a string of ASCII85-encoded data into a <see cref="TreeAttribute"/> and applies it to the specified <see cref="BlockEntity"/>.
    /// </summary>
    /// <param name="blockEntity">
    ///     The <see cref="BlockEntity"/> to update with the decoded attributes.
    /// </param>
    /// <param name="data">
    ///     The ASCII85-encoded string representing the block entity's attributes.
    /// </param>
    /// <param name="worldAccessor">
    ///     The <see cref="IWorldAccessor"/> used for context when applying the attributes.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="blockEntity"/> or <paramref name="data"/> is <c>null</c>.
    /// </exception>
    public static void FromEncodedTreeAttributes(this BlockEntity blockEntity, string data, IWorldAccessor worldAccessor)
    {
        var buffer = Ascii85.Decode(data);
        var tree = new TreeAttribute();
        using var ms = new MemoryStream(buffer);
        using var reader = new BinaryReader(ms);
        tree.FromBytes(reader);
        blockEntity.FromTreeAttributes(tree, worldAccessor);
    }

    /// <summary>
    ///     Extracts the current attributes of the specified <see cref="BlockEntity"/> as a <see cref="TreeAttribute"/>.
    /// </summary>
    /// <param name="blockEntity">
    ///     The <see cref="BlockEntity"/> whose attributes are to be retrieved.
    /// </param>
    /// <returns>
    ///     A <see cref="TreeAttribute"/> containing the block entity's attributes.
    /// </returns>
    public static TreeAttribute Attributes(this BlockEntity blockEntity)
    {
        var tree = new TreeAttribute();
        blockEntity.ToTreeAttributes(tree);
        return tree;
    }

    /// <summary>
    ///     Compares the attributes of two <see cref="BlockEntity"/> instances, ignoring positional data.
    /// </summary>
    /// <param name="this">
    ///     The first <see cref="BlockEntity"/> to compare.
    /// </param>
    /// <param name="other">
    ///     The second <see cref="BlockEntity"/> to compare.
    /// </param>
    /// <returns>
    ///     <c>true</c> if the two block entities have the same attributes, excluding positional data; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    ///     Positional attributes ("posx", "posy", "posz") are ignored during comparison.
    /// </remarks>
    public static bool IsSameAs(this BlockEntity @this, BlockEntity other)
    {
        var ignoredPaths = new[] { "posx", "posy", "posz" };
        var thisAttributes = @this.Attributes();
        var otherAttributes = other.Attributes();
        return thisAttributes.Equals(@this.Api.World, otherAttributes, ignoredPaths);
    }
}