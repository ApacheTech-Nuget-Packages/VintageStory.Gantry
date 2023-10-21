using ApacheTech.Common.Extensions.Harmony;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.Client.NoObf;

namespace Gantry.Core.Extensions.Api;

/// <summary>
///     Extensions methods for the <see cref="ClientMain"/> class.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ClientMainExtensions
{
    /// <summary>
    ///     Stops all currently playing sounds.
    /// </summary>
    public static void StopAllSounds(this ClientMain game)
    {
        var activeSounds = game.GetField<Queue<ILoadedSound>>("ActiveSounds");
        foreach (var sound in activeSounds)
        {
            sound.Stop();
        }
    }
}