using ApacheTech.Common.Extensions.Harmony;
using Gantry.Core.Abstractions;
using Gantry.GameContent.GUI.Abstractions;

namespace Gantry.Services.IO.Dialogue;

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
    /// <param name="gapi">The gapi.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="featureName">Name of the feature.</param>
    protected AutomaticFeatureSettingsDialogue(ICoreGantryAPI gapi, TFeatureSettings settings, string? featureName = null)
        : base(gapi, settings, featureName)
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

        composer.AddStaticText(Gantry.Lang.Translate(FeatureName, $"Dialogue.lbl{propertyName}"), font.Clone().WithOrientation(EnumTextOrientation.Right), left);
        composer.AddHoverText(Gantry.Lang.Translate(FeatureName, $"Dialogue.lbl{propertyName}.HoverText"), font, 260, left);
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