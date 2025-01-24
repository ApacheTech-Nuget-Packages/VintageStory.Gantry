using ProtoBuf;

namespace Gantry.Core.GameContent.ChatCommands.DataStructures;

/// <summary>
///     Represents a player that's been added to a whitelist, or blacklist.
/// </summary>
[JsonObject]
[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public record Player(string Id, string Name);