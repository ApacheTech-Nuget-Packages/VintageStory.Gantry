using System.Text.Json.Serialization;

namespace Gantry.Tools.ModInfoFileGenerator.DataStructures;

/// <summary>
///     Represents the ModInfo for the mod.
///     This record is serialised to and from modinfo.json files.
/// </summary>
public record ModInfoJsonObject
{
    /// <summary>
    ///     The URL of the JSON schema to follow.
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("$schema")]
    public required string Schema { get; init; }

    /// <summary>
    ///     The type of the mod (e.g., "Code", "Content", "Source", or "Theme").
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required ModType Type { get; init; }

    /// <summary>
    ///     The side(s) the mod runs on (e.g., "Client", "Server", "Universal").
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("side")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required EnumAppSide Side { get; init; }

    /// <summary>
    ///     The display name of the mod.
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    ///     The unique identifier for the mod.
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("modId")]
    public required string ModId { get; init; }

    /// <summary>
    ///     The version of the mod.
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("version")]
    public required string Version { get; init; }

    /// <summary>
    ///     A description of the mod's functionality or purpose.
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("description")]
    public required string Description { get; init; }

    /// <summary>
    ///     The list of primary authors of the mod.
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("authors")]
    public required IReadOnlyList<string> Authors { get; init; }

    /// <summary>
    ///     The list of contributors to the mod.
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("contributors")]
    public required IReadOnlyList<string> Contributors { get; init; }

    /// <summary>
    ///     The website URL for the mod or its documentation.
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("website")]
    public required string Website { get; init; }

    /// <summary>
    ///     Indicates if the mod is required on the client side.
    /// </summary>
    [JsonPropertyName("requiredOnClient")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? RequiredOnClient { get; init; }

    /// <summary>
    ///     Indicates if the mod is required on the server side.
    /// </summary>
    [JsonPropertyName("requiredOnServer")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? RequiredOnServer { get; init; }

    /// <summary>
    ///     The network version required for compatibility.
    /// </summary>
    [JsonPropertyName("networkVersion")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? NetworkVersion { get; init; }

    /// <summary>
    ///     The list of dependencies required by the mod.
    /// </summary>
    [JsonPropertyName("dependencies")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyDictionary<string, string>? Dependencies { get; init; }
}