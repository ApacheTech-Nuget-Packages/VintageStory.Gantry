using System.Diagnostics;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

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
    public static void OpenUrl(string url)
    {
        try
        {
            Process.Start(url);
        }
        catch
        {
            // https://github.com/dotnet/corefx/issues/10361
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw;
            }
        }
    }
}