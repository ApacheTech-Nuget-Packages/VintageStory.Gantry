using Newtonsoft.Json;
using ProtoBuf;

namespace Gantry.Services.EasyX.ChatCommands.DataStructures;

/// <summary>
///     Represents a player that's been added to a whitelist, or blacklist.
/// </summary>
[JsonObject]
[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public record Player(string Id, string Name);