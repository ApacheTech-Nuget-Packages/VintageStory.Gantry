﻿using ApacheTech.Common.Extensions.Harmony;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.Client.NoObf;

namespace Gantry.Core.GameContent.Extensions.Gui;

/// <summary>
///     Extensions methods to aid the composition of GUI forms.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class GuiComposerExtensions
{
    /// <summary>
    ///     The default bounds for a menu button.
    /// </summary>
    /// <returns>The default bounds for a menu button.</returns>

    public static ElementBounds DefaultButtonBounds()
    {
        return ElementBounds.Fixed(0.0, 0.0, 0.0, 40.0).WithFixedPadding(0.0, 3.0);
    }

    /// <summary>
    ///     The default bounds for a menu button.
    /// </summary>
    /// <returns>The default bounds for a menu button.</returns>

    public static ElementBounds DefaultButtonBounds(this GuiCompositeSettings _) => DefaultButtonBounds();

    /// <summary>
    ///     Returns all elements of a specific type, from the composer.
    /// </summary>
    public static List<T> GetElements<T>(this GuiComposer composer) where T : GuiElement
    {
        return composer.GetElements().OfType<T>().ToList();
    }

    /// <summary>
    ///     Returns all elements that have been added to the composer.
    /// </summary>
    public static IEnumerable<GuiElement> GetElements(this GuiComposer composer)
    {
        var list = new List<GuiElement>();
        var interactiveElements = composer.GetField<Dictionary<string, GuiElement>>("interactiveElements");
        var staticElements = composer.GetField<Dictionary<string, GuiElement>>("staticElements");
        list.AddRange(interactiveElements.Values);
        list.AddRange(staticElements.Values);
        return list;
    }

    /// <summary>
    ///     Opens the GUI is closed, closes the GUI is open.
    /// </summary>
    /// <typeparam name="TDialogue">The type of the dialogue.</typeparam>
    /// <param name="dialogue">The dialogue.</param>
    /// <returns></returns>
    public static bool ToggleGui<TDialogue>(this TDialogue dialogue) where TDialogue : GuiDialog
    {
        return ApiEx.Client!.OpenedGuis.Contains(dialogue.ToggleKeyCombinationCode)
            ? dialogue.TryClose()
            : dialogue.TryOpen();
    }
}