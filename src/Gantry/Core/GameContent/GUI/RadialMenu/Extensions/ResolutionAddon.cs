using System.Reflection;

namespace Gantry.Core.GameContent.GUI.RadialMenu.Extensions;

/// <summary>
///     Provides functionality for retrieving the screen resolution through reflection.
/// </summary>
public static class ResolutionAddon
{
    private static FieldInfo _fieldPlatform;
    private static FieldInfo _fieldWindow;
    private static PropertyInfo _propertyHeight;
    private static PropertyInfo _propertyWidth;
    private static bool _isInitialised = false;

    /// <summary>
    ///     Retrieves the screen resolution.
    /// </summary>
    /// <param name="capi">The client API used to access the platform and window objects.</param>
    /// <param name="x">The width of the screen resolution, passed by reference.</param>
    /// <param name="y">The height of the screen resolution, passed by reference.</param>
    /// <remarks>
    ///     This method uses reflection to access non-public fields and properties related to the
    ///     platform and window objects. It ensures these are loaded on the first call and retrieves
    ///     the current screen resolution dimensions.
    /// </remarks>
    public static void GetScreenResolution(this ICoreClientAPI capi, ref int x, ref int y)
    {
        if (!_isInitialised)
        {
            Load(capi);
        }

        var platform = _fieldPlatform.GetValue(capi.World);
        var window = _fieldWindow.GetValue(platform);
        x = (int)_propertyWidth.GetValue(window);
        y = (int)_propertyHeight.GetValue(window);
    }

    /// <summary>
    ///     Loads the necessary fields and properties through reflection.
    /// </summary>
    /// <param name="capi">The client API used to access the world object.</param>
    /// <remarks>
    ///     This method uses reflection to initialise the field and property information required
    ///     for accessing the platform and window objects.
    /// </remarks>
    private static void Load(ICoreClientAPI capi)
    {
        var world = capi.World;

        _fieldPlatform = world.GetType().GetField("Platform", BindingFlags.Instance | BindingFlags.NonPublic);
        var platform = _fieldPlatform.GetValue(world);

        _fieldWindow = platform.GetType().GetField("window", BindingFlags.Instance | BindingFlags.NonPublic);
        var window = _fieldWindow.GetValue(platform);

        _propertyHeight = window.GetType().GetProperty("Height", BindingFlags.Instance | BindingFlags.Public);
        _propertyWidth = window.GetType().GetProperty("Width", BindingFlags.Instance | BindingFlags.Public);

        _isInitialised = true;
    }
}
