using Gantry.Tools.Common.ModLoader;
using Gantry.Tools.ModInfoFileGenerator.DataStructures;
using Gantry.Tools.ModInfoFileGenerator.Extensions;
using Json.Schema;

namespace Gantry.Tools.ModInfoFileGenerator.Builders;

public class ModInfoFileBuilder
{
    private CommandLineArgs? _args;
    private FileInfo? _assemblyFile;
    private ModDetails? _modDetails;
    private ModInfoJsonObject? _modInfo;
    private EvaluationResults? _validationResults;
    private string? _outputDirectory;

    public static ModInfoFileBuilder WithArguments(CommandLineArgs args)
    {
        Log.Information("Step: WithArguments - Setting command line arguments.");
        return new ModInfoFileBuilder { _args = args };
    }

    public ModInfoFileBuilder ResolveAssemblyFile()
    {
        Log.Information("Step: ResolveAssemblyFile - Resolving assembly file from arguments.");
        if (_args is null) throw new InvalidOperationException("Arguments must be set before resolving assembly file.");
        if (!Path.IsPathRooted(_args.TargetPath))
            _args.TargetPath = Path.Combine(Environment.CurrentDirectory, _args.TargetPath);
        var assemblyFile = new FileInfo(_args.TargetPath);
        if (!assemblyFile.Exists)
            throw new FileNotFoundException("No file was found at the given location.", _args.TargetPath);
        if (!assemblyFile.Extension.Equals(".dll", StringComparison.OrdinalIgnoreCase))
            throw new DllNotFoundException("The selected file is not a .dll file.");
        _assemblyFile = assemblyFile;
        return this;
    }

    public ModInfoFileBuilder ExtractModDetails(out ModDetails modDetails)
    {
        Log.Information("Step: ExtractModDetails - Extracting mod details from assembly file.");
        if (_assemblyFile is null || _args is null)
            throw new InvalidOperationException("Assembly file and arguments must be set before extracting mod details.");
        _modDetails = _assemblyFile.WithAssemblyContext(_args.DependenciesDir, assembly => assembly.GetModDetailsFromAssembly(_args));
        if (_modDetails is null)
            throw new InvalidOperationException("Failed to retrieve mod details from the assembly.");
        modDetails = _modDetails;
        return this;
    }

    public ModInfoFileBuilder ConvertToModInfoJsonObject()
    {
        Log.Information("Step: ConvertToModInfoJsonObject - Converting mod details to ModInfoJsonObject.");
        if (_modDetails is null)
            throw new InvalidOperationException("Mod details must be extracted before converting to ModInfoJsonObject.");
       
        Log.Information("Starting conversion of ModDetails to ModInfoJsonObject.");
        var atribute = _modDetails.ModInfo;

        Log.Debug("Parsing mod side: {Side}", atribute.Side);
        if (!Enum.TryParse(atribute.Side, true, out EnumAppSide side))
        {
            Log.Error("Failed to parse EnumAppSide from string: {SideString}", atribute.Side);
            throw new ArgumentException($"Cannot parse '{atribute.Side}', must be either 'Client', 'Server' or 'Universal'.");
        }
        Log.Debug("Parsed EnumAppSide: {Side}", side);

        Log.Debug("Dependencies: {Dependencies}", _modDetails.Dependencies);

        Log.Debug("Creating ModInfoJsonObject with metadata: Name={Name}, ModId={ModId}, Version={Version}", atribute.Name, atribute.ModID, _modDetails.Version);
        _modInfo = new ModInfoJsonObject
        {
            Schema = "https://mods.vintagestory.at/web/schema/modinfo.v2.rc3.json",
            Type = ModType.Code,
            Side = side,
            Name = atribute.Name,
            ModId = atribute.ModID,
            Version = _modDetails.Version,
            Description = atribute.Description,
            Authors = [.. atribute.Authors],
            Contributors = [.. atribute.Contributors],
            Website = atribute.Website,
            RequiredOnClient = atribute.RequiredOnClient,
            RequiredOnServer = atribute.RequiredOnServer,
            NetworkVersion = atribute.NetworkVersion,
            Dependencies = _modDetails.Dependencies.Any() ? _modDetails.Dependencies : null,
        };
        Log.Debug("ModInfoJsonObject mapped successfully.");
        return this;
    }

    public async Task<ModInfoFileBuilder> ValidateSchemaAsync()
    {
        Log.Information("Step: ValidateSchemaAsync - Validating ModInfoJsonObject against schema.");
        if (_modInfo is null)
            throw new InvalidOperationException("ModInfoJsonObject must be created before validation.");
        _validationResults = await _modInfo.ValidateSchemaAsync();
        if (!_validationResults.IsValid)
        {
            Log.Fatal("ModInfoJsonObject failed schema validation. Errors:");
            foreach (var detail in _validationResults.Details)
                Log.Fatal(" - {Error}", detail.ToString());
            throw new JsonSchemaException("ModInfoJsonObject failed schema validation.");
        }
        Log.Information("ModInfoJsonObject is valid against the schema: {SchemaUrl}", _modInfo.Schema);
        return this;
    }

    public ModInfoFileBuilder WithOutputDirectory(string? outputDirectory)
    {
        Log.Information("Step: WithOutputDirectory - Setting output directory.");
        _outputDirectory = outputDirectory ?? _assemblyFile?.DirectoryName;
        return this;
    }

    public async Task WriteModInfoFileAsync()
    {
        Log.Information("Step: WriteModInfoFileAsync - Writing modinfo.json to output directory.");
        if (_modInfo is null || _outputDirectory is null)
            throw new InvalidOperationException("ModInfoJsonObject and output directory must be set before writing file.");
        var json = _modInfo.ToJson();
        var outputFilePath = Path.Combine(_outputDirectory, "modinfo.json");
        if (!Directory.Exists(_outputDirectory))
        {
            Log.Information("Creating output directory: {OutputDirectory}", _outputDirectory);
            Directory.CreateDirectory(_outputDirectory);
        }
        Log.Debug("Writing mod info JSON to file: {OutputFile}", outputFilePath);
        await File.WriteAllTextAsync(outputFilePath, json);
        Log.Information("Mod info file generation completed successfully. Output file: {OutputFile}", outputFilePath);
    }

    // Properties for inspection/testing
    public FileInfo? AssemblyFile => _assemblyFile;
    public ModDetails? ModDetails => _modDetails;
    public ModInfoJsonObject? ModInfo => _modInfo;
    public EvaluationResults? ValidationResults => _validationResults;
    public string? OutputDirectory => _outputDirectory;
}
