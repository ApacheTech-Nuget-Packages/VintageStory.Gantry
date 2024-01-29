using Gantry.Services.FileSystem.Enums;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Gantry.Services.FileSystem.Extensions;

/// <summary>
///     Extension methods to aid the use of <see cref="FileScope"/> enums.
/// </summary>
public static class FileScopeExtensions
{
    /// <summary>
    ///     Converts the value of this FileScope enum to its equivalent string representation.
    /// </summary>
    /// <param name="scope">The FileScope enum value to convert.</param>
    public static string FastToString(this FileScope scope)
    {
        return scope switch
        {
            FileScope.Global => nameof(FileScope.Global),
            FileScope.World => nameof(FileScope.World),
            _ => throw new ArgumentOutOfRangeException(nameof(scope), scope, null)
        };
    }
}