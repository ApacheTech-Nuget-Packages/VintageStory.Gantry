using System.Diagnostics;
using JetBrains.Annotations;
using Vintagestory.API.Config;

namespace Gantry.Core.Extensions.Helpers;

/// <summary>
///     Extension methods to aid with cross-platform operations. 
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class BrowserEx
{
    /// <summary>
    ///     Opens a URL within the user's default browser.
    /// </summary>
    /// <param name="url">The URL to browse to.</param>
    public static bool TryOpenUrl(string url)
    {
        try
        {
            Process.Start(url);
            return true;
        }
        catch
        {
            switch (RuntimeEnv.OS)
            {
                case OS.Windows:
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                    return true;
                case OS.Linux:
                    Process.Start("xdg-open", url);
                    return true;
                case OS.Mac:
                    Process.Start("open", url);
                    return true;
                default:
                    return false;
            }
        }
    }
}