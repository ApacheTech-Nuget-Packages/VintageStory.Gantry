using Gantry.Core;
using Gantry.Core.Extensions.DotNet;
using Gantry.Services.FileSystem.Abstractions;
using Gantry.Services.FileSystem.Abstractions.Contracts;
using Gantry.Services.FileSystem.Configuration;
using Gantry.Services.FileSystem.Configuration.Extensions;
using Gantry.Services.FileSystem.Enums;
using Gantry.Services.FileSystem.Extensions;
using Newtonsoft.Json;

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
    public FileSystemService() : this(FileSystemServiceOptions.Default)
    {
    }

    /// <summary>
    ///     Initialises a new instance of the <see cref="FileSystemService"/> class.
    /// </summary>
    public FileSystemService(FileSystemServiceOptions options)
    {
        _registeredFiles = new Dictionary<string, ModFileBase>();

        if (string.IsNullOrWhiteSpace(ModPaths.ModRootPath) ||
            ModPaths.WorldGuid != ApiEx.Current.World.SavegameIdentifier)
        {
            ModPaths.Initialise(options.RootFolderName, ApiEx.Current.World.SavegameIdentifier);
        }
        if (!options.RegisterSettingsFiles) return;
        this.RegisterSettingsFiles();
            
        ModEx.ModAssembly.InitialiseSettingsConsumers();
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
        var file = new FileInfo(ModPaths.GetScopedPath(fileName, scope));
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
    /// <returns>Return an <see cref="IModFile" /> representation of the file, on disk.</returns>
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
    /// <returns>Return an <typeparamref name="TFileType" /> representation of the file, on disk.</returns>
    public TFileType GetRegisteredFile<TFileType>(string fileName) where TFileType : IModFileBase
    {
        return (TFileType)GetRegisteredFile(fileName);
    }

    /// <summary>
    /// Retrieves a file that has previously been registered with the FileSystem Service.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>Return an <see cref="IJsonModFile" /> representation of the file, on disk.</returns>
    public IJsonModFile GetJsonFile(string fileName)
    {
        return GetRegisteredFile<IJsonModFile>(fileName);
    }

    /// <summary>
    /// Retrieves a file that has previously been registered with the FileSystem Service.
    /// </summary>
    /// <param name="fileName">The name of the file, including file extension.</param>
    /// <returns>Return an <see cref="IBinaryModFile" /> representation of the file, on disk.</returns>
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
    /// <returns>Return an <see cref="ITextModFile" /> representation of the file, on disk.</returns>
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
            Path.Combine(ModPaths.ModAssetsPath, file.Name),
            Path.Combine(ModPaths.ModRootPath, file.Name)
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
        ModSettings.Dispose();
        ModPaths.Dispose();
        _registeredFiles.Clear();
    }
}