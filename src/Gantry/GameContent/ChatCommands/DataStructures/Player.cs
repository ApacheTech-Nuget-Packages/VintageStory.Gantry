using ProtoBuf;

namespace Gantry.GameContent.ChatCommands.DataStructures;

/// <summary>
///     Represents a player that's been added to a whitelist, or blacklist.
/// </summary>
[JsonObject]
[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public record Player()
{
    /// <summary>
    ///     Initialises a new instance of the <see cref="Player"/> class.
    /// </summary>
    /// <param name="id">The unique identifier of the player.</param>
    /// <param name="name">The name of the player.</param>
    public Player(string id, string name) : this()
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    ///     The unique identifier of the player.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    ///     The name of the player.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    ///     Implicitly converts an instance of <see cref="PlayerUidName"/> to a <see cref="Player"/>.
    /// </summary>
    /// <param name="playerUidName">An instance of <see cref="PlayerUidName"/> to convert.</param>
    /// <returns>
    ///     A new instance of <see cref="Player"/> with the corresponding identifier and name,
    ///     or <c>null</c> if <paramref name="playerUidName"/> is <c>null</c>.
    /// </returns>
    public static implicit operator Player?(PlayerUidName playerUidName) 
        => playerUidName is null ? null : new Player(playerUidName.Uid, playerUidName.Name);

    /// <summary>
    ///     Implicitly converts an instance of <see cref="Player"/> to a <see cref="PlayerUidName"/>.
    /// </summary>
    /// <param name="player">An instance of <see cref="Player"/> to convert.</param>
    /// <returns>
    ///     A new instance of <see cref="PlayerUidName"/> with the corresponding UID and name,
    ///     or <c>null</c> if <paramref name="player"/> is <c>null</c>.
    /// </returns>
    public static implicit operator PlayerUidName?(Player player) 
        => player is null ? null : new PlayerUidName(player.Id, player.Name);
}