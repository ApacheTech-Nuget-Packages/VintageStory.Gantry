using System.Reflection;
using System.Resources;
using System.Text;

namespace Gantry.Core.Extensions.DotNet;

/// <summary>
///     Provides extension methods for interacting with embedded resources within mod assemblies, including resource existence checks, reading, and extraction.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class EmbeddedResourcesExtensions
{
    /// <summary>
    ///     Determines whether an embedded resource exists within an assembly.
    /// </summary>
    /// <param name="assembly">The assembly to search for the resource.</param>
    /// <param name="fileName">The name of the file to find (should match the resource's file name).</param>
    /// <returns><c>true</c> if the embedded resource is found; otherwise, <c>false</c>.</returns>
    /// <remarks>
    ///     This method is useful for checking if a resource is available before attempting to read or extract it.
    /// </remarks>
    public static bool ResourceExists(this Assembly assembly, string fileName)
    {
        var resources = assembly.GetManifestResourceNames();
        return resources.Any(p => p.EndsWith(fileName));
    }

    /// <summary>
    ///     Reads the embedded resource and returns its contents as a raw stream.
    /// </summary>
    /// <param name="assembly">The assembly to load the resource from.</param>
    /// <param name="fileName">The name of the file embedded within the assembly.</param>
    /// <returns>The contents of the file as a <see cref="Stream"/>.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the embedded data file is not found.</exception>
    /// <exception cref="MissingManifestResourceException">Thrown if the resource manifest is missing or invalid.</exception>
    /// <remarks>
    ///     Use this method to obtain a stream for reading binary or text data from an embedded resource.
    /// </remarks>
    public static Stream GetResourceStream(this Assembly assembly, string fileName)
    {
        var resource = assembly.GetManifestResourceNames().SingleOrDefault(p => p.EndsWith(fileName));
        if (string.IsNullOrWhiteSpace(resource))
            throw new MissingManifestResourceException($"Embedded data file not found: {fileName}");

        var stream = assembly.GetManifestResourceStream(resource);
        if (stream is null)
            throw new FileNotFoundException($"Embedded data file not found: {fileName}");

        return stream;
    }

    /// <summary>
    ///     Reads the embedded resource and returns its contents as a string using UTF-8 encoding.
    /// </summary>
    /// <param name="assembly">The assembly to load the resource from.</param>
    /// <param name="fileName">The name of the file embedded within the assembly.</param>
    /// <returns>The contents of the file as a string.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the embedded data file is not found.</exception>
    /// <remarks>
    ///     This is a convenience overload that uses UTF-8 encoding by default.
    /// </remarks>
    public static string GetResourceContent(this Assembly assembly, string fileName) => GetResourceContent(assembly, fileName, Encoding.UTF8);

    /// <summary>
    ///     Reads the embedded resource and returns its contents as a string using the specified encoding.
    /// </summary>
    /// <param name="assembly">The assembly to load the resource from.</param>
    /// <param name="fileName">The name of the file embedded within the assembly.</param>
    /// <param name="encoding">The encoding to use when reading the resource.</param>
    /// <returns>The contents of the file as a string.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the embedded data file is not found.</exception>
    /// <remarks>
    ///     Use this method to read text resources with a specific encoding, such as UTF-8 or UTF-16.
    /// </remarks>
    public static string GetResourceContent(this Assembly assembly, string fileName, Encoding encoding)
    {
        if (!assembly.ResourceExists(fileName)) return string.Empty;
        var stream = assembly.GetResourceStream(fileName);
        using var reader = new StreamReader(stream, encoding, true, 1024, false);
        var content = reader.ReadToEnd();
        reader.Close();
        return content;
    }

    /// <summary>
    ///     Extracts (disembeds) an embedded resource to a specified file location on disk.
    /// </summary>
    /// <param name="assembly">The assembly to load the resource from.</param>
    /// <param name="resourceName">The manifest name of the resource.</param>
    /// <param name="fileName">The full path to where the file should be copied to.</param>
    /// <remarks>
    ///     This method is useful for exporting embedded resources to the file system for external use.
    ///     If the resource does not exist, the method does nothing.
    /// </remarks>
    public static void DisembedResource(this Assembly assembly, string resourceName, string fileName)
    {
        if (!assembly.ResourceExists(resourceName)) return;
        var stream64 = assembly.GetResourceStream(resourceName);
        using var file = File.OpenWrite(fileName);
        stream64.CopyTo(file);
    }
}