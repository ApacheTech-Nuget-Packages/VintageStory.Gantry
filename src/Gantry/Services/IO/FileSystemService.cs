using Gantry.Core.Abstractions;
using Gantry.Extensions.DotNet;
using Gantry.Services.IO.Abstractions;
using Gantry.Services.IO.Abstractions.Contracts;
using Gantry.Services.IO.Configuration;
using Gantry.Services.IO.Configuration.Abstractions;
using Gantry.Services.IO.DataStructures;
using Gantry.Services.IO.Extensions;
using Gantry.Services.IO.Helpers;

namespace Gantry.Services.IO;

/// <summary>
///     Provides a means for handling files, including embedded resources, used within a mod.
/// </summary>
public sealed class FileSystemService : IFileSystemService
{
    private readonly Dictionary<string, ModFileBase> _registeredFiles = [];
    private readonly ICoreGantryAPI _gantry;

    /// <inheritdoc />
    public GantryPaths Paths { get; }

    /// <summary>
    ///     Initialises a new instance of the <see cref="FileSystemService"/> class.
    /// </summary>
    [ActivatorUtilitiesConstructor]
    public FileSystemService(
        ICoreGantryAPI gantry,
        GantryPaths gantryPaths)
    {
        var api = gantry.Uapi;
        _gantry = gantry;
        Paths = gantryPaths;
        _gantry.Log($" - World Identifier: {Paths.WorldGuid}");
        _gantry.Log($" - World Seed: {api.World.Seed}");
        _gantry.Log($" - World Data Path: {Paths.WorldData.FullName}");
        _gantry.Log($" - World Settings Path: {Paths.WorldSettings.FullName}");
        _gantry.Log($" - Global Data Path: {Paths.GlobalData.FullName}");
        _gantry.Log($" - Global Settings Path: {Paths.GlobalSettings.FullName}");
        _gantry.Log($" - Gantry Data Path: {Paths.GantryData.FullName}");
        _gantry.Log($" - Gantry Settings Path: {Paths.GantrySettings.FullName}");
        _gantry.Log($" - Mod Assembly Path: {Paths.ModRootPath.FullName}");
        _gantry.Log($" - Mod Assets Path: {Paths.ModAssets.FullName}");
    }

    /// <summary>
    ///     Registers a file with the FileSystem Service. This will copy a default implementation of the file from:
    ///     <br/>• An embedded resource.
    ///     <br/>• The mod's unpack cache directory.
    ///     <br/>• The mod's assets folder.
    ///     <br/>If no default implementation can be found, a new file is created, at the correct location.
    /// </summary>
    /// <param name="fileName">The name of the file, including file extension.</param>
    /// <param name="type">The type of the file, such as Assets, Settings, or Data.</param>
    /// <param name="scope">The scope of the file, be it global, per-world, or gantry.</param>
    public IFileSystemService RegisterFile(string fileName, ModFileType type, ModFileScope scope)
    {
        var file = new FileInfo(Path.Combine(Paths.For(type, scope).FullName, fileName));
        if (_registeredFiles.TryAdd(fileName, file.CreateModFileWrapper(_gantry)))
        {
            if (!file.Exists) CopyFileToOutputDirectory(file);
            _gantry.Log($" - Registered new {scope} file: {fileName}");
        }
        return this;
    }

    /// <summary>
    ///     Retrieves a file that has previously been registered with the FileSystem Service.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>Return a <see cref="IModFile" /> representation of the file, on disk.</returns>
    public object GetRegisteredFile(string fileName)
    {
        if (_registeredFiles.TryGetValue(fileName, out var file)) return file;
        throw new FileNotFoundException(
            $"File `{fileName}` either does not exist, or has not yet been registered with the FileSystem Service.");
    }

    /// <inheritdoc />
    public TFileType GetRegisteredFile<TFileType>(string fileName) where TFileType : IModFileBase
    {
        return (TFileType)GetRegisteredFile(fileName);
    }

    /// <summary>
    ///     Retrieves a file that has previously been registered with the FileSystem Service.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>Return a <see cref="IJsonModFile" /> representation of the file, on disk.</returns>
    public IJsonModFile GetJsonFile(string fileName)
    {
        return GetRegisteredFile<IJsonModFile>(fileName);
    }

    /// <summary>
    ///     Retrieves a file that has previously been registered with the FileSystem Service.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>Return a <see cref="IJsonModFile" /> representation of the file, on disk.</returns>
    public IJsonSettingsFile GetJsonSettingsFile(string fileName)
    {
        return GetRegisteredFile<IJsonModFile>(fileName).To<IJsonSettingsFile>();
    }

    /// <summary>
    ///     Retrieves a file that has previously been registered with the FileSystem Service.
    /// </summary>
    /// <param name="fileName">The name of the file, including file extension.</param>
    /// <returns>Return a <see cref="IBinaryModFile" /> representation of the file, on disk.</returns>
    public IBinaryModFile GetBinaryFile(string fileName)
    {
        return GetRegisteredFile<IBinaryModFile>(fileName);
    }

    /// <summary>
    ///     Deserialises the specified file as a strongly-typed object.
    ///     The consuming type must have a paramaterless constructor.
    /// </summary>
    /// <typeparam name="TModel">The type of object to deserialise into.</typeparam>
    /// <param name="pathToFile">The name of the file, including file extension.</param>
    /// <returns>An instance of type <typeparamref name="TModel" />, populated with data from this file.</returns>
    public TModel ParseJsonFile<TModel>(string pathToFile)
    {
        var file = new FileInfo(pathToFile);
        if (!file.Exists)
        {
            _gantry.Logger.Error($"File `{file.Name}` does not exist at path `{file.FullName}`.");
            throw new FileNotFoundException($"File `{file.Name}` does not exist at path `{file.FullName}`.");
        }
        var json = File.ReadAllText(pathToFile);
        var resource = JsonConvert.DeserializeObject<TModel>(json)
            ?? throw new FileNotFoundException();
        return resource;
    }

    /// <summary>
    ///     Deserialises the specified file as a strongly-typed object.
    ///     The consuming type must have a paramaterless constructor.
    /// </summary>
    /// <typeparam name="TModel">The type of object to deserialise into.</typeparam>
    /// <param name="fileName">The name of the file, including file extension.</param>
    /// <returns>An instance of type <typeparamref name="TModel" />, populated with data from this file.</returns>
    public TModel ParseEmbeddedJsonFile<TModel>(string fileName)
    {
        var json = _gantry.ModAssembly.GetResourceContent(fileName);
        var resource = JsonConvert.DeserializeObject<TModel>(json) 
            ?? throw new FileNotFoundException();
        return resource;
    }

    /// <summary>
    ///     Deserialises the specified file as a strongly-typed object.
    ///     The consuming type must have a paramaterless constructor.
    /// </summary>
    /// <typeparam name="TModel">The type of object to deserialise into.</typeparam>
    /// <param name="fileName">The name of the file, including file extension.</param>
    /// <returns>An instance of type <typeparamref name="TModel" />, populated with data from this file.</returns>
    public IEnumerable<TModel> ParseEmbeddedJsonArrayFile<TModel>(string fileName)
    {
        var json = _gantry.ModAssembly.GetResourceContent(fileName);
        if (string.IsNullOrEmpty(json)) return [];
        return JsonConvert.DeserializeObject<IEnumerable<TModel>>(json) ?? [];
    }

    /// <summary>
    ///     Retrieves a file that has previously been registered with the FileSystem Service.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>Return a <see cref="ITextModFile" /> representation of the file, on disk.</returns>
    public ITextModFile GetTextFile(string fileName) => GetRegisteredFile<ITextModFile>(fileName);

    private void CopyFileToOutputDirectory(FileSystemInfo file)
    {
        var assembly = _gantry.ModAssembly;
        if (assembly.ResourceExists(file.Name))
        {
            assembly.DisembedResource(file.Name, file.FullName);
            return;
        }

        var locations = new List<string>
        {
            Path.Combine(Paths.ModAssets.FullName, file.Name),
            Path.Combine(Paths.ModRootPath.FullName, file.Name)
        };
        foreach (var location in locations.Where(File.Exists))
        {
            if (file.Exists) return;
            File.Copy(location, file.FullName, false);
            return;
        }

        Task.Factory.StartNew(async () =>
        {
            await using var writer = File.CreateText(file.FullName);
            if (file.ParseModFileFormat() != ModFileFormat.Json) return;
            await writer.WriteLineAsync("{}");
        });
    }

    /// <summary>
    ///     Registers a settings file with the FileSystem Service. This will copy a default implementation of the file from:
    ///      • An embedded resource.
    ///      • The mod's unpack directory.
    ///      • The mod's assets folder.
    /// 
    ///     If no default implementation can be found, a new file is created, at the correct location.
    /// </summary>
    /// <param name="fileName">The name of the file, including file extension.</param>
    /// <param name="scope">The scope of the file, be it global, or per-world.</param>
    public void RegisterSettingsFile(string fileName, ModFileScope scope)
    {
        RegisterFile(fileName, ModFileType.Settings, scope);
        var harmony = _gantry.Harmony.CreateOrUseInstance($"{_gantry.Mod.Info.ModID}_FeatureSettings");
        var file = JsonSettingsFile.FromJsonFile(GetJsonFile(fileName), _gantry, scope, harmony);
        _gantry.Settings.Set(_gantry.Side, scope, file);
    }

    /// <summary>
    ///     Registers settings files.
    /// </summary>
    void IFileSystemService.RegisterDefaultSettingsFiles()
    {
        _gantry.Logger.VerboseDebug("Registering Gantry settings files.");
        RegisterSettingsFile($"{_gantry.Mod.Info.ModID}-settings-{ModFileScope.World}-{_gantry.Side}.json", ModFileScope.World);
        RegisterSettingsFile($"{_gantry.Mod.Info.ModID}-settings-{ModFileScope.Global}-{_gantry.Side}.json", ModFileScope.Global);
        RegisterSettingsFile($"gantry-settings-{ModFileScope.Gantry}-{_gantry.Side}.json", ModFileScope.Gantry);
    }

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        _registeredFiles.Clear();
    }
}