namespace Gantry.Extensions;

/// <summary>
///     Provides extension methods for the <see cref="bool"/> type to facilitate common operations such as toggling and ensuring specific values.
/// </summary>
public static class BoolExtensions
{
    /// <summary>
    ///     Toggles the value of a <see cref="bool"/> between <c>true</c> and <c>false</c>.
    /// </summary>
    /// <param name="value">The reference to the boolean value to toggle.</param>
    /// <returns>The toggled value of the boolean.</returns>
    public static bool Toggle(this ref bool value) => value = !value;

    /// <summary>
    ///     Ensures that a <see cref="bool"/> value is set to <c>true</c>.
    /// </summary>
    /// <param name="value">The reference to the boolean value to modify.</param>
    /// <returns>The updated boolean value, which will always be <c>true</c>.</returns>
    public static bool EnsureTrue(this ref bool value) => value = true;

    /// <summary>
    ///     Ensures that a <see cref="bool"/> value is set to <c>false</c>.
    /// </summary>
    /// <param name="value">The reference to the boolean value to modify.</param>
    /// <returns>The updated boolean value, which will always be <c>false</c>.</returns>
    public static bool EnsureFalse(this ref bool value) => value = false;
}