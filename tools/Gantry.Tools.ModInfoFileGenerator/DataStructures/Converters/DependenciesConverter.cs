using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gantry.Tools.ModInfoFileGenerator.DataStructures.Converters;

/// <summary>
///     Serialises and deserialises mod dependencies for modinfo.json files using System.Text.Json.
///     Used for converting between JSON and <see cref="ModDependency"/> objects in mod info files.
/// </summary>
public class DependenciesConverter : JsonConverter<IReadOnlyList<ModDependency>>
{
    public override IReadOnlyList<ModDependency> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject token for dependencies.");

        var dependencies = new List<ModDependency>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected PropertyName token in dependencies object.");
            var modId = reader.GetString();
            reader.Read();
            var version = reader.GetString();
            dependencies.Add(new ModDependency(modId!, version!));
        }
        return dependencies.AsReadOnly();
    }

    public override void Write(Utf8JsonWriter writer, IReadOnlyList<ModDependency> value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }
        writer.WriteStartObject();
        foreach (var modDependency in value)
        {
            writer.WriteString(modDependency.ModID, modDependency.Version);
        }
        writer.WriteEndObject();
    }
}