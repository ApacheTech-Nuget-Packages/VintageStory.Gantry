using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.Common.Extensions.System;
using Gantry.Core;
using Gantry.Core.Diagnostics;
using Gantry.Core.Extensions.DotNet;
using Gantry.Services.FileSystem.Abstractions;
using Gantry.Services.FileSystem.Abstractions.Contracts;
using Gantry.Services.FileSystem.Configuration;
using Gantry.Services.FileSystem.Configuration.Extensions;
using Gantry.Services.FileSystem.Enums;
using Gantry.Services.FileSystem.Extensions;
using Newtonsoft.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

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
    [InjectableConstructor]
    public FileSystemService(ICoreAPI api, FileSystemServiceOptions options)
    {
        _registeredFiles = new Dictionary<string, ModFileBase>();

        var modId = ModEx.ModInfo.ModID;

        var worldIdentifier = api.World.SavegameIdentifier;
        WorldGuid = Ensure.PopulatedWith(WorldGuid, worldIdentifier);
        api.Logger.VerboseDebug($"[Gantry] {modId} WorldGuid: {WorldGuid}");

        var rootPath = ModInfo.ToModID(ModEx.ModInfo.Authors[0].IfNullOrWhitespace("Gantry"));
        VintageModsRootPath = CreateDirectory(Path.Combine(GamePaths.DataPath, "ModData", rootPath));
        api.Logger.VerboseDebug($"[Gantry] {modId} VintageModsRootPath: {VintageModsRootPath}");

        var rootFolderName = options.RootFolderName.IfNullOrWhitespace(modId);
        ModDataRootPath = CreateDirectory(Path.Combine(VintageModsRootPath, rootFolderName));
        api.Logger.VerboseDebug($"[Gantry] {modId} ModDataRootPath: {ModDataRootPath}");

        ModDataGlobalPath = CreateDirectory(Path.Combine(ModDataRootPath, "Global"));
        api.Logger.VerboseDebug($"[Gantry] {modId} ModDataGlobalPath: {ModDataGlobalPath}");

        ModDataWorldPath = CreateDirectory(Path.Combine(ModDataRootPath, worldIdentifier));
        api.Logger.VerboseDebug($"[Gantry] {modId} ModDataWorldPath: {ModDataWorldPath}");

        ModRootPath = Path.GetDirectoryName(ModEx.ModAssembly.Location)!;
        api.Logger.VerboseDebug($"[Gantry] {modId} ModRootPath: {ModRootPath}");

        ModAssetsPath = Path.Combine(ModRootPath, "assets");
        api.Logger.VerboseDebug($"[Gantry] {modId} ModAssetsPath: {ModAssetsPath}");

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
    public IFileSystemService RegisterFile(string fileName, FileScope scope)
    {
        var file = new FileInfo(GetScopedPath(fileName, scope));
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

    /// <summary>
    ///     Retrieves a file that has previously been registered with the FileSystem Service.
    /// </summary>
    /// <typeparam name="TFileType">The type of the file type to return as.</typeparam>
    /// <param name="fileName">The name of the file, including file extension.</param>
    /// <returns>Return a <typeparamref name="TFileType" /> representation of the file, on disk.</returns>
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
    /// <param name="fileName">The name of the file, including file extension.</param>
    /// <returns>An instance of type <typeparamref name="TModel" />, populated with data from this file.</returns>
    public TModel ParseEmbeddedJsonFile<TModel>(string fileName)
    {
        try
        {
            var json = ModEx.ModAssembly.GetResourceContent(fileName);
            return JsonConvert.DeserializeObject<TModel>(json);
        }
        catch (Exception)
        {
            return default;
        }
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
        try
        {
            var json = ModEx.ModAssembly.GetResourceContent(fileName);
            return JsonConvert.DeserializeObject<IEnumerable<TModel>>(json);
        }
        catch (Exception)
        {
            return default;
        }
    }

    /// <summary>
    /// Retrieves a file that has previously been registered with the FileSystem Service.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>Return a <see cref="ITextModFile" /> representation of the file, on disk.</returns>
    public ITextModFile GetTextFile(string fileName)
    {
        return GetRegisteredFile<ITextModFile>(fileName);
    }

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
            await writer.WriteLineAsync("[]");
        });
    }

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        ModDataRootPath = null;
        ModDataGlobalPath = null;
        ModDataWorldPath = null;
        ModRootPath = null;
        ModAssetsPath = null;
        WorldGuid = null;

        ModSettings.Dispose();
        _registeredFiles.Clear();
    }
}