#nullable enable
using Gantry.Core.Diagnostics;
using Vintagestory.API.Util;

// ReSharper disable RedundantSuppressNullableWarningExpression
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
// ReSharper disable StringLiteralTypo
// ReSharper disable InconsistentNaming
// ReSharper disable CommentTypo

#pragma warning disable CS8603 // Possible null reference return.

namespace Gantry.Core;

/// <summary>
///     Globally accessible utilities for Gantry.
/// </summary>
[DoNotPruneType]
public static class G
{
    #region Logging

    private static readonly AsyncLocal<GantryLogger?> _serverLogger = new();
    private static readonly AsyncLocal<GantryLogger?> _clientLogger = new();

    /// <summary>
    ///     The directory that the log files are stored in.
    /// </summary>
    internal static DirectoryInfo LogDirectory { get; private set; } = default!;

    /// <summary>
    ///     Interface to the client's and server's event, debug and error logging utilty.
    /// </summary>
    public static ILogger Logger => _serverLogger.Value ?? _clientLogger.Value;

    /// <summary>
    ///     Traces a message to the <see cref="ILogger.VerboseDebug(string, object[])"/> log file.
    /// </summary>
    public static void Log(string template, params object[] args) => Logger.VerboseDebug(template, args);

    internal static void CreateLogger(ICoreAPI api, Mod mod)
    {
        api.Logger.Debug($"[Gantry] Initialising Gantry {api.Side} logger.");
        LogDirectory = new DirectoryInfo(Path.Combine(GamePaths.Logs, "gantry", mod.Info.ModID));
        if (!LogDirectory.Exists) LogDirectory.Create();
        LogDirectory.EnumerateFiles("*.txt").Foreach(p => p.Delete());
        api.Logger.Debug($" - Directory: {LogDirectory}");

        switch (api.Side)
        {
            case EnumAppSide.Server:
                _serverLogger.Value = new GantryLogger.Server(api, mod.Info);
                break;
            case EnumAppSide.Client:
                _clientLogger.Value = new GantryLogger.Client(api, mod.Info);
                break;
            case EnumAppSide.Universal:
            default:
                throw new ArgumentOutOfRangeException(nameof(api), api, "App-side cannot be determined.");
        }
        Logger.VerboseDebug($"Hello, World!");
    }

    internal static void DisposeLogger(ICoreAPI api)
    {
        Logger.VerboseDebug($"Gantry {api.Side} logger shut down. Goodbye!");
        switch (api.Side)
        {
            case EnumAppSide.Server:
                _serverLogger.Value?.Dispose();
                _serverLogger.Value = null;
                break;
            case EnumAppSide.Client:
                _clientLogger.Value?.Dispose();
                _clientLogger.Value = null;
                break;
            case EnumAppSide.Universal:
            default:
                throw new ArgumentOutOfRangeException(nameof(api), api, "App-side cannot be determined.");
        }
    }

    #endregion
}