using JetBrains.Annotations;

namespace Gantry.Services.FileSystem.Extensions;

/// <summary>
///     Extension methods to extend the functionality of <see cref="DirectoryInfo"/> instances.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class DirectoryInfoExtensions
{
    /// <summary>
    ///     Copy the directory to another location.
    /// </summary>
    /// <param name="sourceDir">The source dir.</param>
    /// <param name="destinationDir">The destination dir.</param>
    /// <param name="recursive">if set to <c>true</c>, sub-directories will recursively be copied as well. Default: True.</param>
    public static void CopyTo(this DirectoryInfo sourceDir, string destinationDir, bool recursive = true)
    {
        if (!sourceDir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {sourceDir.FullName}");

        var dirs = sourceDir.GetDirectories();
        Directory.CreateDirectory(destinationDir);

        foreach (var file in sourceDir.GetFiles())
        {
            var targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath);
        }

        if (!recursive) return;
        foreach (var subDir in dirs)
        {
            var newDestinationDir = Path.Combine(destinationDir, subDir.Name);
            var subDirectoryInfo = new DirectoryInfo(subDir.FullName);
            subDirectoryInfo.CopyTo(newDestinationDir);
        }
    }

    /// <summary>
    ///     Determines whether the specified directory is empty.
    /// </summary>
    /// <param name="sourceDir">The source dir.</param>
    /// <returns><c>true</c> if the specified source dir is empty; otherwise, <c>false</c>.</returns>
    public static bool IsEmpty(this DirectoryInfo sourceDir) 
        => sourceDir.EnumerateFileSystemInfos().Any();
}