using Gantry.Tools.ModInfoFileGenerator.DataStructures;
using Gantry.Tools.ModInfoFileGenerator.DataStructures.Converters;
using Json.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gantry.Tools.ModInfoFileGenerator.Extensions;

/// <summary>
///     Provides extension methods for converting mod details to JSON objects, serialising mod info, and validating against JSON schemas.
/// </summary>
public static class ModInfoExtensions
{
    /// <summary>
    ///     Loads a mod assembly and extracts its metadata details, including version, 
    ///     debug configuration, mod info, dependencies, location, and assembly name.
    /// </summary>
    /// <param name="assembly">The mod assembly (.dll) to inspect.</param>
    /// <param name="args">The arguments passed to the generator.</param>
    /// <returns>
    ///     A <see cref="ModDetails"/> record containing extracted metadata and dependency information for the mod.
    /// </returns>
    internal static ModDetails GetModDetailsFromAssembly(this Assembly assembly, CommandLineArgs args)
    {
        Log.Information("Extracting ModInfoAttribute from assembly: {Assembly}", assembly.FullName);
        var modInfoObject = assembly.GetCustomAttributes(false)
            .FirstOrDefault(attr => attr.GetType().FullName == typeof(ModInfoAttribute).FullName)
            ?? throw new CustomAttributeFormatException("No ModInfoAttribute found in assembly.");

        Log.Debug("Extracting ModID and Name from ModInfoAttribute.");
        var modId = modInfoObject.GetType().GetProperty("ModID")!.GetValue(modInfoObject, null)!.ToString();
        var modName = modInfoObject.GetType().GetProperty("Name")!.GetValue(modInfoObject, null)!.ToString();

        Log.Debug("Creating ModInfoAttribute instance.");
        var modInfo = new ModInfoAttribute(modName, modId);
        var properties = modInfoObject.GetType().GetProperties()
            .Where(p => p.CanWrite && p.CanRead && p.GetIndexParameters().Length != 0)
            .ToList();

        foreach (var property in modInfoObject.GetType().GetProperties().Where(p => p.CanWrite && p.CanRead))
        {
            try
            {
                var value = property.GetValue(modInfoObject, null);
                if (value is null) continue;
                Log.Debug("Setting property {Property} on ModInfoAttribute.", property.Name);
                modInfo.GetType().GetProperty(property.Name)!.SetValue(modInfo, value);
            }
            catch (ArgumentException ex)
            {
                Log.Error("Failed to set property {Property} on ModInfoAttribute: {Message}", property.Name, ex.Message);
                throw;
            }
        }

        Log.Debug("Checking for debug configuration.");
        var debugMode = assembly.GetCustomAttributes(false)
            .OfType<DebuggableAttribute>()
            .Any(da => da.IsJITTrackingEnabled);

        Log.Debug("Determining version using VersioningStyle: {VersioningStyle}", args.VersioningStyle);
        var version = args.VersioningStyle switch
        {
            VersioningStyle.ModInfo => modInfo.Version,
            VersioningStyle.Custom => args.Version!,
            VersioningStyle.Assembly => FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion!,
            _ => throw new UnreachableException()
        };
        Log.Debug("Extracted version: {Version}", version);

        Log.Debug("Scanning for ModDependencyAttribute in assembly: {Assembly}", assembly.FullName);
        var dependencies = assembly.GetCustomAttributes(false)
            .Where(attr => attr.GetType().FullName == typeof(ModDependencyAttribute).FullName)
            .ToDictionary(
                attr => (string)attr.GetType().GetProperty("ModID")!.GetValue(attr, null)!,
                attr => (string)attr.GetType().GetProperty("Version")!.GetValue(attr, null)!
            );

        Log.Debug("Found {Count} mod dependencies.", dependencies.Count);

        Log.Debug("Building ModDetails record.");
        var details = new ModDetails
        {
            Version = version,
            DebugConfiguration = debugMode,
            ModInfo = modInfo,
            Dependencies = dependencies,
            Location = args.TargetDir ?? assembly.Location,
            AssemblyName = assembly.GetName().Name!
        };
        Log.Information("ModDetails record created successfully.");
        return details;
    }

    /// <summary>
    ///     Serialises a <see cref="ModInfoJsonObject"/> to a JSON string using the configured options.
    ///     Traces the serialisation process.
    /// </summary>
    /// <param name="atribute">The mod info object to serialise.</param>
    /// <returns>The JSON string representation of the mod info object.</returns>
    public static string ToJson(this ModInfoJsonObject atribute)
    {
        Log.Debug("Serialising ModInfoJsonObject to JSON.");
        var json = JsonSerializer.Serialize(atribute, _options);
        Log.Debug("Serialisation complete. JSON length: {Length}", json.Length);
        return json;
    }

    /// <summary>
    ///     The default serialisation options for mod info JSON, including enum and dependency converters.
    /// </summary>
    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new JsonStringEnumConverter(),
            new DependenciesConverter(),
        }
    };

    /// <summary>
    ///     Validates a <see cref="ModInfoJsonObject"/> against its referenced JSON schema URL.
    ///     Downloads the schema, parses it, and evaluates the object for compliance.
    /// </summary>
    /// <param name="atribute">The mod info object to validate.</param>
    /// <returns><c>true</c> if the object is valid against the schema; otherwise, <c>false</c>.</returns>
    public static async Task<EvaluationResults> ValidateSchemaAsync(this ModInfoJsonObject atribute)
    {
        Log.Information("Starting schema validation for ModInfoJsonObject.");
        try
        {
            Log.Debug("Downloading schema from: {SchemaUrl}", atribute.Schema);
            using var httpClient = new HttpClient();
            var schemaJson = await httpClient.GetStringAsync(atribute.Schema);

            Log.Debug("Schema downloaded. Length: {Length}", schemaJson.Length);
            Log.Debug("Parsing schema.");
            var schema = JsonSchema.FromText(schemaJson);

            Log.Debug("Serialising ModInfoJsonObject for validation.");
            var json = atribute.ToJson();

            Log.Debug("Parsing JSON for validation.");
            var jsonElement = JsonDocument.Parse(json).RootElement;

            Log.Debug("Evaluating JSON against schema.");
            var result = schema.Evaluate(jsonElement, new EvaluationOptions { RequireFormatValidation = true });

            Log.Debug("Schema evaluation complete. IsValid: {IsValid}", result.IsValid);
            return result;
        }
        catch (HttpRequestException ex)
        {
            Log.Error("Failed to download schema: {Message}", ex.Message);
            throw;
        }
        catch (JsonException ex)
        {
            Log.Error("Failed to parse JSON or schema: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            Log.Error("Unexpected error during schema validation: {Message}", ex.Message);
            throw;
        }
    }
}