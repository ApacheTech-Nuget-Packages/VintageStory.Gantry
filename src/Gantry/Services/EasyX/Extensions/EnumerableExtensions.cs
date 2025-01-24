namespace Gantry.Services.EasyX.Extensions;

/// <summary>
///     
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class EnumerableExtensions
{
    /// <summary>
    ///     
    /// </summary>
    public static bool IsOneOf<T>(this T item, params T[] options) => options.Contains(item);
}