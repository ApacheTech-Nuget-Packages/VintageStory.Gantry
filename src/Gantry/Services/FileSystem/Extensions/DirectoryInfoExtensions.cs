

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable IdentifierTypo

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
    /// <param name="recursive">if set to <c>true</c>, subdirectories will recursively be copied as well. Default: True.</param>
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
    
    /// <summary>
    ///     Renames a directory, keeping its contents intact. The new name is case-sensitive.
    /// </summary>
    /// <param name="directoryInfo">The DirectoryInfo object representing the directory to rename.</param>
    /// <param name="newName">The new name for the directory.</param>
    public static void Rename(this DirectoryInfo directoryInfo, string newName)
    {
        if (directoryInfo == null) return;
        if (string.IsNullOrWhiteSpace(newName)) return;
        if (string.Equals(directoryInfo.Name, newName, StringComparison.Ordinal)) return; // No change in name

        var parentDirectory = directoryInfo.Parent?.FullName;
        if (parentDirectory is null) return;

        var newDirectoryPath = Path.Combine(parentDirectory, newName);
        var tempDir = new DirectoryInfo($"{newDirectoryPath}_linux_sucks");

        directoryInfo.MoveToWithOverwrite(tempDir.FullName);
        tempDir.MoveToWithOverwrite(newDirectoryPath);
    }

    /// <summary>
    ///     Moves a directory and its contents to another location, overwriting existing files and directories.
    /// </summary>
    /// <param name="sourceDir">The source directory to move.</param>
    /// <param name="destDir">The destination directory.</param>
    public static void MoveToWithOverwrite(this DirectoryInfo sourceDir, string destDir)
    {
        if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir!);

        foreach (var file in sourceDir.GetFiles())
        {
            var destFilePath = Path.Combine(destDir, file.Name);
            file.CopyTo(destFilePath, true);
        }

        foreach (var subDir in sourceDir.GetDirectories())
        {
            var destSubDirPath = Path.Combine(destDir, subDir.Name);
            subDir.MoveToWithOverwrite(destSubDirPath);
        }

        sourceDir.Delete(true);
    }
}