using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using JetBrains.Annotations;

namespace Gantry.Core.Extensions.DotNet
{
    /// <summary>
    ///     Provides a means for interacting with embedded resources within mod assemblies.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class EmbeddedResourcesExtensions
    {
        /// <summary>
        ///     Determines whether an embedded resource exists within an assembly.
        /// </summary>
        /// <param name="assembly">The assembly to find the resource in.</param>
        /// <param name="fileName">The name of the file to find.</param>
        /// <returns><c>true</c> if the embedded resource is found, <c>false</c> otherwise.</returns>
        public static bool ResourceExists(this Assembly assembly, string fileName)
        {
            return assembly.GetManifestResourceNames().Any(p => p.EndsWith(fileName));
        }

        /// <summary>
        ///     Reads the resource, and passes back the output as a raw stream.
        /// </summary>
        /// <param name="assembly">The assembly to load the resource from.</param>
        /// <param name="fileName">Name of the file, embedded within the assembly.</param>
        /// <returns>The contents of the file, as a raw stream.</returns>
        /// <exception cref="FileNotFoundException">Embedded data file not found.</exception>
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
        ///     Reads the resource, and passes back the output as a string, as UTF8.
        /// </summary>
        /// <param name="assembly">The assembly to load the resource from.</param>
        /// <param name="fileName">Name of the file, embedded within the assembly.</param>
        /// <returns>The contents of the file, as a string.</returns>
        /// <exception cref="FileNotFoundException">Embedded data file not found.</exception>
        public static string GetResourceContent(this Assembly assembly, string fileName) => GetResourceContent(assembly, fileName, Encoding.UTF8);

        /// <summary>
        ///     Reads the resource, and passes back the output as a string
        /// </summary>
        /// <param name="assembly">The assembly to load the resource from.</param>
        /// <param name="fileName">Name of the file, embedded within the assembly.</param>
        /// <param name="encoding">The encoding to apply to the resource.</param>
        /// <returns>The contents of the file, as a string.</returns>
        /// <exception cref="FileNotFoundException">Embedded data file not found.</exception>
        public static string GetResourceContent(this Assembly assembly, string fileName, Encoding encoding)
        {
            if (!assembly.ResourceExists(fileName)) return string.Empty;
            var stream = assembly.GetResourceStream(fileName);
            using var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, false);
            var content = reader.ReadToEnd();
            reader.Close();
            return content;
        }

        /// <summary>
        ///     Disembeds an embedded resource to specified location.
        /// </summary>
        /// <param name="assembly">The assembly to load the resource from.</param>
        /// <param name="resourceName">The manifest name of the resource.</param>
        /// <param name="fileName">The full path to where the file should be copied to.</param>
        public static void DisembedResource(this Assembly assembly, string resourceName, string fileName)
        {
            if (!assembly.ResourceExists(resourceName)) return;
            var stream64 = assembly.GetResourceStream(resourceName);
            using var file = File.OpenWrite(fileName);
            stream64.CopyTo(file);
        }
    }
}