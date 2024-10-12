using Vintagestory.API.Client;

namespace Gantry.Core.GameContent.Extensions.Gui;

/// <summary>
///     Extends the functionality of GUI dialogues.
/// </summary>
public static class GuiExtensions
{
    /// <summary>
    ///     Gets a specific element within the GUI.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dialogue"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T GetElement<T>(this GuiDialog dialogue, string name) where T : GuiElement
    {
        foreach (var composer in dialogue.Composers.Values)
        {
            try
            {
                var element = composer.GetElement(name);
                if (element is null) continue;
                return (T)element;
            }
            catch
            {
                // ignored
            }
        }
        return null;
    }
}