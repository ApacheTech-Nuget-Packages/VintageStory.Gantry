using Vintagestory.API.Server;

namespace Gantry.Core.GameContent.Extensions;

/// <summary>
///     Extension methods for when working with players and player entities.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class PlayerExtensions
{
    /// <summary>
    ///     Sends a generic notification message to a given player, from the server.
    /// </summary>
    /// <param name="player">The player to send the message to.</param>
    /// <param name="groupId">The chat group to send the message to.</param>
    /// <param name="message">The message to send.</param>
    /// <param name="chatType">The type of message to send.</param>
    public static void SendMessage(this IPlayer player, int groupId, string message, EnumChatType chatType = EnumChatType.Notification)
    {
        if (player is IClientPlayer clientPlayer)
        {
            clientPlayer.ShowChatNotification(message);
            return;
        }
        ((IServerPlayer)player).SendMessage(groupId, message, chatType);
    }

    /// <summary>
    ///     Retrieves the light level at the player's current position in the world.
    /// </summary>
    /// <param name="player">The player whose position will be used to retrieve the light level.</param>
    /// <returns>The light level at the player's current position.</returns>
    public static float GetLightLevelAtPlayerPosition(this EntityPlayer player)
    {
        var blockAccessor = player.World.BlockAccessor;
        var pos = player.Pos;
        return blockAccessor.GetLightLevel(pos?.AsBlockPos, EnumLightLevelType.MaxTimeOfDayLight);
    }

    /// <summary>
    ///     Sends a message to the player, giving feedback about an invalid syntax message.
    /// </summary>
    /// <param name="player">The player to send the message to.</param>
    /// <param name="groupId">The chat group to send the message to.</param>
    public static void SendInvalidSyntaxMessage(this IPlayer player, int groupId)
    {
        var invalidSyntax = LangEx.ConfirmationString("invalid-syntax");
        var tryAgain = LangEx.ConfirmationString("try-again");
        player.SendMessage(groupId, $"{invalidSyntax} {tryAgain}", EnumChatType.CommandError);
    }
}