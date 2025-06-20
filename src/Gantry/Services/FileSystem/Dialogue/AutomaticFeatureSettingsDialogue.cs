﻿using System.Reflection;
using ApacheTech.Common.Extensions.Harmony;
using Gantry.Core.GameContent.GUI.Abstractions;

namespace Gantry.Services.FileSystem.Dialogue;

/// <summary>
///     Acts as a base class for dialogue boxes that
/// </summary>
/// <typeparam name="TFeatureSettings">The strongly-typed settings for the feature under use.</typeparam>
/// <seealso cref="GenericDialogue" />
public abstract class AutomaticFeatureSettingsDialogue<TFeatureSettings> : FeatureSettingsDialogue<TFeatureSettings> where TFeatureSettings : class, new()
{
    /// <summary>
    ///     Initialises a new instance of the <see cref="AutomaticFeatureSettingsDialogue{TFeatureSettings}"/> class.
    /// </summary>
    /// <param name="capi">The capi.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="featureName">Name of the feature.</param>
    protected AutomaticFeatureSettingsDialogue(ICoreClientAPI capi, TFeatureSettings settings, string? featureName = null)
        : base(capi, settings, featureName)
    {
    }

    /// <summary>
    ///     Composes the header for the GUI.
    /// </summary>
    /// <param name="composer">The composer.</param>
    protected override void ComposeBody(GuiComposer composer)
    {
        var left = ElementBounds.Fixed(0, GuiStyle.TitleBarHeight + 1.0, 250, 20);
        var right = ElementBounds.Fixed(260, GuiStyle.TitleBarHeight, 20, 20);
        for (var index = 0;
             index < Settings.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Length;
             index++)
        {
            var propertyInfo = Settings.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)[index];
            AddSettingSwitch(composer, propertyInfo.Name, ref left, ref right);
        }
    }

    /// <summary>
    ///     Adds a switch control to the GUI composer for a boolean property, with label and hover text.
    /// </summary>
    /// <param name="composer">The GUI composer to add the switch to.</param>
    /// <param name="propertyName">The name of the boolean property to bind to the switch.</param>
    /// <param name="left">The bounds for the label element (updated for next row).</param>
    /// <param name="right">The bounds for the switch element (updated for next row).</param>
    private void AddSettingSwitch(GuiComposer composer, string propertyName, ref ElementBounds left, ref ElementBounds right)
    {
        const int switchSize = 20;
        const int gapBetweenRows = 20;
        var font = CairoFont.WhiteSmallText();

        composer.AddStaticText(LangEx.FeatureString(FeatureName, $"Dialogue.lbl{propertyName}"), font.Clone().WithOrientation(EnumTextOrientation.Right), left);
        composer.AddHoverText(LangEx.FeatureString(FeatureName, $"Dialogue.lbl{propertyName}.HoverText"), font, 260, left);
        composer.AddSwitch(state => { ToggleProperty(propertyName, state); },
            right.FlatCopy().WithFixedWidth(switchSize), $"btn{propertyName}");

        left = left.BelowCopy(fixedDeltaY: gapBetweenRows);
        right = right.BelowCopy(fixedDeltaY: gapBetweenRows);
    }

    /// <summary>
    ///     Toggles the value of a boolean property on the settings object and refreshes the displayed values.
    /// </summary>
    /// <param name="propertyName">The name of the property to toggle.</param>
    /// <param name="state">The new boolean value to set.</param>
    private void ToggleProperty(string propertyName, bool state)
    {
        // HACK: Be aware that this can cause the dynamic save to be called BEFORE the value is changed.
        //
        // Changes must be saved to file MANUALLY!.
        //
        // A more robust idea would be to have a FeatureSettings class that holds Global, World, and Local accessors, as well as a Save function
        // that can be used to manually save each type. However, ideally, Local settings should be read only.
        //
        // Further to this, more testing is needed, because the main issues with the dialogue were caused by ModSettings not being disposed between
        // worlds, when logging in and out of a server. With that issue now fixed, it's possible this class will work as intended, once again.
        //
        // Check again in 1.20 to see if this has been resolved with the new changes to the ModHost.

        Settings.SetProperty(propertyName, state);
        RefreshValues();
    }

    /// <summary>
    ///     Refreshes the displayed values on the form, updating all switch controls to match the current settings.
    /// </summary>
    protected override void RefreshValues()
    {
        if (!IsOpened()) return;
        foreach (var propertyInfo in Settings.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            SingleComposer
                .GetSwitch($"btn{propertyInfo.Name}")
                .SetValue((bool)propertyInfo.GetValue(Settings)!);
        }
    }
}