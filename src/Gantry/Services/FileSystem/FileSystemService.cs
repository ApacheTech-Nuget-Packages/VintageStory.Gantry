﻿using Gantry.Core.Diagnostics;
using Gantry.Core.Extensions.DotNet;
using Gantry.Services.FileSystem.Abstractions;
using Gantry.Services.FileSystem.Abstractions.Contracts;
using Gantry.Services.FileSystem.Configuration;
using Gantry.Services.FileSystem.Configuration.Extensions;
using Gantry.Services.FileSystem.Enums;
using Gantry.Services.FileSystem.Extensions;
using static Gantry.Services.FileSystem.ModPaths;

namespace Gantry.Services.FileSystem;

/// <summary>
///     Provides a means for handling files, including embedded resources, used within a mod.
/// </summary>
public sealed class FileSystemService : IFileSystemService
{
    private readonly IDictionary<string, ModFileBase> _registeredFiles;

    /// <summary>
    ///     Initialises a new instance of the <see cref="FileSystemService"/> class.
    /// </summary>
    [ActivatorUtilitiesConstructor]
    public FileSystemService(FileSystemServiceOptions options)
    {
        _registeredFiles = new Dictionary<string, ModFileBase>();

        var api = ApiEx.Current;
        var modId = ModEx.ModInfo.ModID;

        var worldIdentifier = api.World.SavegameIdentifier;
        WorldGuid = Ensure.PopulatedWith(WorldGuid, worldIdentifier);
        G.Logger.VerboseDebug($"Initialising FileSystem Service Settings");
        G.Logger.VerboseDebug($" - World Identifier: {WorldGuid}");
        G.Logger.VerboseDebug($" - World Seed: {api.World.Seed}");

        var rootPath = ModInfo.ToModID(ModEx.ModInfo.Authors[0].IfNullOrWhitespace("Gantry"));
        VintageModsRootPath = CreateDirectory(Path.Combine(GamePaths.DataPath, "ModData", rootPath));
        G.Logger.VerboseDebug($" - VintageModsRootPath: {VintageModsRootPath}");

        var rootFolderName = options.RootFolderName.IfNullOrWhitespace(modId);
        ModDataRootPath = CreateDirectory(Path.Combine(VintageModsRootPath, rootFolderName));
        G.Logger.VerboseDebug($" - ModDataRootPath: {ModDataRootPath}");

        ModDataGantryRootPath = CreateDirectory(Path.Combine(VintageModsRootPath, "gantry"));
        G.Logger.VerboseDebug($" - ModDataGantryRootPath: {ModDataGantryRootPath}");

        ModDataGantryGlobalPath = CreateDirectory(Path.Combine(ModDataGantryRootPath, "Global"));
        G.Logger.VerboseDebug($" - ModDataGantryGlobalPath: {ModDataGantryGlobalPath}");

        ModDataGantryWorldPath = CreateDirectory(Path.Combine(ModDataGantryRootPath, worldIdentifier));
        G.Logger.VerboseDebug($" - ModDataGantryWorldPath: {ModDataGantryWorldPath}");

        ModDataGlobalPath = CreateDirectory(Path.Combine(ModDataRootPath, "Global"));
        G.Logger.VerboseDebug($" - ModDataGlobalPath: {ModDataGlobalPath}");

        ModDataWorldPath = CreateDirectory(Path.Combine(ModDataRootPath, worldIdentifier));
        G.Logger.VerboseDebug($" - ModDataWorldPath: {ModDataWorldPath}");

        ModRootPath = Path.GetDirectoryName(ModEx.ModAssembly.Location)!;
        G.Logger.VerboseDebug($" - ModRootPath: {ModRootPath}");

        ModAssetsPath = Path.Combine(ModRootPath, "assets");
        G.Logger.VerboseDebug($" - ModAssetsPath: {ModAssetsPath}");

        this.RegisterGantrySettingsFiles(api);
        if (options.RegisterSettingsFiles) this.RegisterSettingsFiles(api);
    }

    /// <summary>
    ///     Registers a file with the FileSystem Service. This will copy a default implementation of the file from:
    ///     <br/>• An embedded resource.
    ///     <br/>• The mod's unpack cache directory.
    ///     <br/>• The mod's assets folder.
    ///     <br/>If no default implementation can be found, a new file is created, at the correct location.
    /// </summary>
    /// <param name="fileName">The name of the file, including file extension.</param>
    /// <param name="scope">The scope of the file, be it global, or per-world.</param>
    /// <param name="gantryFile">Inter-mod gantry settings.</param>
    public IFileSystemService RegisterFile(string fileName, FileScope scope, bool gantryFile = false)
    {
        var file = new FileInfo(GetScopedPath(fileName, scope, gantryFile));
        G.Logger.VerboseDebug($"Registering new {scope} file: {fileName}");
        _registeredFiles.Add(fileName, file.CreateModFileWrapper());
        if (!file.Exists)
        {
            CopyFileToOutputDirectory(file);
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
    /// Retrieves a file that has previously been registered with the FileSystem Service.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>Return a <see cref="IJsonModFile" /> representation of the file, on disk.</returns>
    public IJsonModFile GetJsonFile(string fileName)
    {
        return GetRegisteredFile<IJsonModFile>(fileName);
    }

    /// <summary>
    /// Retrieves a file that has previously been registered with the FileSystem Service.
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
            G.Logger.Error($"File `{file.Name}` does not exist at path `{file.FullName}`.");
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
        var json = ModEx.ModAssembly.GetResourceContent(fileName);
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
        var json = ModEx.ModAssembly.GetResourceContent(fileName);
        if (string.IsNullOrEmpty(json)) return [];
        return JsonConvert.DeserializeObject<IEnumerable<TModel>>(json) ?? [];
    }

    /// <summary>
    /// Retrieves a file that has previously been registered with the FileSystem Service.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>Return a <see cref="ITextModFile" /> representation of the file, on disk.</returns>
    public ITextModFile GetTextFile(string fileName) => GetRegisteredFile<ITextModFile>(fileName);

    private static void CopyFileToOutputDirectory(FileSystemInfo file)
    {
        var assembly = ModEx.ModAssembly;
        if (assembly.ResourceExists(file.Name))
        {
            assembly.DisembedResource(file.Name, file.FullName);
            return;
        }

        var locations = new List<string>
        {
            Path.Combine(ModAssetsPath, file.Name),
            Path.Combine(ModRootPath, file.Name)
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
            if (file.ParseFileType() != FileType.Json) return;
            await writer.WriteLineAsync("{}");
        });
    }

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        ModDataRootPath = string.Empty;
        ModDataGlobalPath = string.Empty;
        ModDataWorldPath = string.Empty;
        ModRootPath = string.Empty;
        ModAssetsPath = string.Empty;
        WorldGuid = string.Empty;

        G.Logger.VerboseDebug("Disposing Mod Settings");
        ModSettings.Dispose();

        G.Logger.VerboseDebug("Clearing Registered Files");
        _registeredFiles.Clear();
    }
}