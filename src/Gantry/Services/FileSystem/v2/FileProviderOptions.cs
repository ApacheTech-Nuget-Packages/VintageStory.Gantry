using Gantry.Services.FileSystem.v2.Abstractions;

namespace Gantry.Services.FileSystem.v2;

/// <summary>
///     Options for configuring various behaviors of the default <see cref="IFileProvider"/> implementation.
/// </summary>
public class FileProviderOptions
{
    // Avoid allocating objects in the default case
    internal static FileProviderOptions Default { get; } = new();
}