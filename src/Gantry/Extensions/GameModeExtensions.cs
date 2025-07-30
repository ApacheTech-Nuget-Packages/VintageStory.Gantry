namespace Gantry.Extensions;

/// <summary>
///     Extension methods to aid working with the current player's game mode.
/// </summary>
public static class GameModeExtensions
{
    /// <summary>
    ///     Determines whether the current game mode is set to Survival.
    /// </summary>
    /// <param name="mode">The current game mode.</param>
    /// <returns><c>true</c> if the current game mode is set to Survival; otherwise, <c>false</c>.</returns>
    public static bool IsSurvival(this EnumGameMode mode) => mode is EnumGameMode.Survival;

    /// <summary>
    ///     Determines whether the current game mode is set to Creative.
    /// </summary>
    /// <param name="mode">The current game mode.</param>
    /// <returns><c>true</c> if the current game mode is set to Creative; otherwise, <c>false</c>.</returns>
    public static bool IsCreative(this EnumGameMode mode) => mode is EnumGameMode.Creative;

    /// <summary>
    ///     Determines whether the current game mode is set to Spectator.
    /// </summary>
    /// <param name="mode">The current game mode.</param>
    /// <returns><c>true</c> if the current game mode is set to Spectator; otherwise, <c>false</c>.</returns>
    public static bool IsSpectator(this EnumGameMode mode) => mode is EnumGameMode.Spectator;

    /// <summary>
    ///     Determines whether the current game mode is set to Guest.
    /// </summary>
    /// <param name="mode">The current game mode.</param>
    /// <returns><c>true</c> if the current game mode is set to Guest; otherwise, <c>false</c>.</returns>
    public static bool IsGuest(this EnumGameMode mode) => mode is EnumGameMode.Guest;
}