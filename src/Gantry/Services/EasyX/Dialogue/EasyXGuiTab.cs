using Gantry.Core.Abstractions;
using Gantry.Core.Network.Extensions;
using Gantry.GameContent.ChatCommands.DataStructures;
using Gantry.GameContent.Extensions.Gui;
using Gantry.GameContent.GUI.Abstractions;
using Gantry.Services.EasyX.Abstractions;
using Gantry.Services.IO.Configuration.Abstractions;
using System.Linq.Expressions;
using System.Numerics;
using Vintagestory.API.Util;

namespace Gantry.Services.EasyX.Dialogue;

/// <summary>
///     Base class for EasyX feature GUI tabs which can be composed into a <see cref="GenericDialogue"/>.
///     Provides common composition logic for access control, player whitelisting/blacklisting and
///     helper methods for adding feature settings controls.
/// </summary>
public abstract class EasyXGuiTab<TSettings> : ComposableGuiTab
    where TSettings : FeatureSettings<TSettings>, IEasyXServerSettings, new()
{
    /// <summary>
    ///     The identifier of the currently-selected player from the "whitelistable" drop-down.
    /// </summary>
    private string _selectedWhitelistablePlayerId = string.Empty;

    /// <summary>
    ///     The identifier of the currently-selected player from the "whitelisted" drop-down.
    /// </summary>
    private string _selectedWhitelistedPlayerId = string.Empty;

    /// <summary>
    ///     The identifier of the currently-selected player from the "blacklistable" drop-down.
    /// </summary>
    private string _selectedBlacklistablePlayerId = string.Empty;

    /// <summary>
    ///     The identifier of the currently-selected player from the "blacklisted" drop-down.
    /// </summary>
    private string _selectedBlacklistedPlayerId = string.Empty;

    private readonly ICoreGantryAPI _core;

    /// <summary>
    ///     The bounds used by this composable tab when composing into the parent dialogue.
    /// </summary>
    public override ElementBounds Bounds { get; set; } = ElementBounds.Fixed(0, 25, 600, 30);

    /// <summary>
    ///     The localised feature name key used for translation lookups.
    /// </summary>
    protected string FeatureName { get; }

    /// <summary>
    ///     The feature settings instance this GUI tab edits.
    /// </summary>
    protected TSettings Settings { get; }

    /// <summary>
    ///     The client network channel used to send settings to the server.
    /// </summary>
    protected IClientNetworkChannel? ClientChannel { get; } = null!;

    /// <summary>
    ///     The parent dialogue instance this tab is composed into.
    /// </summary>
    protected GenericDialogue? Parent { get; set; }

    /// <summary>
    ///     The active composer used while this tab is composed.
    /// </summary>
    protected GuiComposer Composer { get; private set; } = null!;

    /// <summary>
    ///     Initialises a new instance of the <see cref="EasyXGuiTab{TSettings}"/> class.
    /// </summary>
    /// <param name="core">The core Gantry API.</param>
    /// <param name="featureName">The feature name used for localisation keys.</param>
    /// <param name="settings">The settings instance for the feature.</param>
    protected EasyXGuiTab(ICoreGantryAPI core, string featureName, TSettings settings)
    {
        _core = core;
        FeatureName = featureName;
        Settings = settings;
        ClientChannel = _core.Capi.Network.GetDefaultChannel(_core);
        Name = F("TabName");
    }

    /// <summary>
    ///     Helper to translate a feature-scoped translation code using the Gantry localisation API.
    /// </summary>
    /// <param name="code">The translation code relative to the feature's ModMenu domain.</param>
    /// <param name="args">Optional format arguments.</param>
    /// <returns>The translated string.</returns>
    protected string F(string code, params object[] args)
        => _core.Lang.Translate($"{FeatureName}.ModMenu", code, args);

    /// <summary>
    ///     Helper to translate a generic ModMenu translation code using the Gantry localisation API.
    /// </summary>
    /// <param name="code">The translation code under the common ModMenu domain.</param>
    /// <param name="args">Optional format arguments.</param>
    /// <returns>The translated string.</returns>
    protected string L(string code, params object[] args)
        => _core.Lang.Translate("ModMenu", code, args);

    /// <summary>
    ///     Helper for pluralised translations scoped to this feature.
    /// </summary>
    /// <typeparam name="T">A numeric type which indicates the amount for pluralisation.</typeparam>
    /// <param name="code">The translation code relative to the feature.</param>
    /// <param name="amount">The numeric amount to use for pluralisation rules.</param>
    /// <returns>The pluralised translation string.</returns>
    protected string P<T>(string code, T amount) where T : struct, INumber<T>
        => _core.Lang.Pluralise(_core.Lang.Code(FeatureName, $"ModMenu.{code}"), amount);

    /// <inheritdoc />
    public override GuiComposer ComposePart(GenericDialogue parent, GuiComposer composer)
    {
        Parent = parent;
        Composer = composer;

        // Title
        composer.AddStaticText(F("lblTitle.Text"), CairoFont.WhiteSmallishText().WithWeight(Cairo.FontWeight.Bold), Bounds, "lblTitle");

        // Description
        SetRowBounds(Bounds, Bounds, out var left, out var right);
        composer.AddStaticText(F("lblDescription.Text"), LabelFont, EnumTextOrientation.Justify, left.WithFixedWidth(570), "lblDescription");

        // Access Mode
        SetRowBounds(left, right, out left, out right);
        ComposeAccessMode(composer, ref left, ref right);

        // Feature Settings
        SetRowBounds(left, right, out left, out right);
        ComposeFeatureSettings(composer, left, right);

        // Save Button
        var controlRowBoundsRightFixed = ElementBounds.FixedSize(150, 30).WithFixedOffset(0, 25f).WithAlignment(EnumDialogArea.RightTop);
        composer
            .AddSmallButton(L("btnSaveChanges.Text"), OnSaveButtonPressed, controlRowBoundsRightFixed, EnumButtonStyle.Small, "btnSaveChanges");

        return composer;
    }

    /// <inheritdoc />
    public override void RefreshValues(GuiComposer composer)
    {
        composer.GetDropDown("cbxAccessMode")?.SetSelectedValue(Settings.Mode.ToString());
        var cbxWhitelistablePlayers = composer.GetDropDown("cbxWhitelistablePlayers");
        cbxWhitelistablePlayers?.SetSelectedIndex(0);
        _selectedWhitelistablePlayerId = cbxWhitelistablePlayers?.SelectedValue ?? string.Empty;

        var cbxWhitelistedPlayers = composer.GetDropDown("cbxWhitelistedPlayers");
        cbxWhitelistedPlayers?.SetSelectedIndex(0);
        _selectedWhitelistedPlayerId = cbxWhitelistedPlayers?.SelectedValue ?? string.Empty;

        var cbxBlacklistablePlayers = composer.GetDropDown("cbxBlacklistablePlayers");
        cbxBlacklistablePlayers?.SetSelectedIndex(0);
        _selectedBlacklistablePlayerId = cbxBlacklistablePlayers?.SelectedValue ?? string.Empty;

        var cbxBlacklistedPlayers = composer.GetDropDown("cbxBlacklistedPlayers");
        cbxBlacklistedPlayers?.SetSelectedIndex(0);
        _selectedBlacklistedPlayerId = cbxBlacklistedPlayers?.SelectedValue ?? string.Empty;
    }

    /// <summary>
    ///     Override point for composing feature-specific settings rows.
    ///     Derived classes should add their setting controls using the supplied bounds.
    /// </summary>
    /// <param name="composer">The composer to add controls to.</param>
    /// <param name="left">The left bounds for labels.</param>
    /// <param name="right">The right bounds for controls.</param>
    protected virtual void ComposeFeatureSettings(GuiComposer composer, ElementBounds left, ElementBounds right)
    {
        // INTENTIONALLY BLANK
    }

    /// <summary>
    ///     Composes the access mode selection UI and conditional player lists for whitelist/blacklist modes.
    /// </summary>
    /// <param name="composer">The composer to use.</param>
    /// <param name="left">The left bounds which may be updated as rows are added.</param>
    /// <param name="right">The right bounds which may be updated as rows are added.</param>
    private void ComposeAccessMode(GuiComposer composer, ref ElementBounds left, ref ElementBounds right)
    {
        var accessModes = Enum.GetNames<AccessMode>();
        var accessModeText = accessModes.Select(p => L($"cbxAccessMode.{p}")).ToArray();
        var selectedIndex = accessModes.IndexOf(Settings.Mode.ToString());

        composer
            .AddStaticText(L("lblAccessMode.Text"), LabelFont, EnumTextOrientation.Right, left, "lblAccessMode")
            .AddAutoSizeHoverText(L("lblAccessMode.HoverText"), HoverTextFont, HOVER_TEXT_WIDTH, left)
            .AddDropDown(accessModes, accessModeText, selectedIndex, OnAccessModeChanged, right, "cbxAccessMode");

        if (Settings.Mode is AccessMode.Whitelist)
        {
            var whitelistedPlayers = Settings.Whitelist;
            var whitelistablePlayers = _core.Capi.World.AllOnlinePlayers
                .Select(p => new Player(p.PlayerUID, p.PlayerName))
                .Except(whitelistedPlayers)
                .ToList();

            if (whitelistablePlayers.Count != 0)
            {
                SetRowBounds(left, right, out left, out right);
                var cbxWhitelistablePlayersBounds = right.FlatCopy().WithFixedWidth(260);
                var btnWhitelistablePlayerBounds = right.FlatCopy().WithFixedWidth(100).FixedRightOf(cbxWhitelistablePlayersBounds, 10);

                composer
                    .AddStaticText(L("lblWhitelistable.Text"), LabelFont, EnumTextOrientation.Right, left, "lblWhitelistable")
                    .AddAutoSizeHoverText(L("lblWhitelistable.HoverText"), HoverTextFont, HOVER_TEXT_WIDTH, left)
                    .AddDropDown([.. whitelistablePlayers.Select(p => p.Id)], [.. whitelistablePlayers.Select(p => p.Name)], 0, OnSelectedWhitelistablePlayerChanged, cbxWhitelistablePlayersBounds, HoverTextFont, "cbxWhitelistablePlayers")
                    .AddSmallButton(L("btnAddWhitelistPlayer.Text"), OnAddWhitelistPlayer, btnWhitelistablePlayerBounds, EnumButtonStyle.Small, "btnAddWhitelistPlayer");
            }

            if (whitelistedPlayers.Count != 0)
            {
                SetRowBounds(left, right, out left, out right);
                var cbxWhitelistedPlayersBounds = right.FlatCopy().WithFixedWidth(260);
                var btnWhitelistedPlayerBounds = right.FlatCopy().WithFixedWidth(100).FixedRightOf(cbxWhitelistedPlayersBounds, 10);

                composer
                    .AddStaticText(L("lblWhitelisted.Text"), LabelFont, EnumTextOrientation.Right, left, "lblWhitelisted")
                    .AddAutoSizeHoverText(L("lblWhitelisted.HoverText"), HoverTextFont, HOVER_TEXT_WIDTH, left)
                    .AddDropDown([.. whitelistedPlayers.Select(p => p.Id)], [.. whitelistedPlayers.Select(p => p.Name)], 0, OnSelectedWhitelistedPlayerChanged, cbxWhitelistedPlayersBounds, HoverTextFont, "cbxWhitelistedPlayers")
                    .AddSmallButton(L("btnRemoveWhitelistedPlayer.Text"), OnRemoveWhitelistedPlayer, btnWhitelistedPlayerBounds, EnumButtonStyle.Small, "btnRemoveWhitelistedPlayer");
            }
        }

        if (Settings.Mode is AccessMode.Blacklist)
        {
            var blacklistedPlayers = Settings.Blacklist;
            var blacklistablePlayers = _core.Capi.World.AllOnlinePlayers
                .Select(p => new Player(p.PlayerUID, p.PlayerName))
                .Except(blacklistedPlayers)
                .ToList();

            if (blacklistablePlayers.Count != 0)
            {
                SetRowBounds(left, right, out left, out right);
                var cbxBlacklistablePlayersBounds = right.FlatCopy().WithFixedWidth(260);
                var btnBlacklistablePlayerBounds = right.FlatCopy().WithFixedWidth(100).FixedRightOf(cbxBlacklistablePlayersBounds, 10);

                composer
                    .AddStaticText(L("lblBlacklistable.Text"), LabelFont, EnumTextOrientation.Right, left, "lblBlacklistable")
                    .AddAutoSizeHoverText(L("lblBlacklistable.HoverText"), HoverTextFont, HOVER_TEXT_WIDTH, left)
                    .AddDropDown([.. blacklistablePlayers.Select(p => p.Id)], [.. blacklistablePlayers.Select(p => p.Name)], 0, OnSelectedBlacklistablePlayerChanged, cbxBlacklistablePlayersBounds, HoverTextFont, "cbxBlacklistablePlayers")
                    .AddSmallButton(L("btnAddBlacklistPlayer.Text"), OnAddBlacklistPlayer, btnBlacklistablePlayerBounds, EnumButtonStyle.Small, "btnAddBlacklistPlayer");
            }

            if (blacklistedPlayers.Count != 0)
            {
                SetRowBounds(left, right, out left, out right);
                var cbxBlacklistedPlayersBounds = right.FlatCopy().WithFixedWidth(260);
                var btnBlacklistedPlayerBounds = right.FlatCopy().WithFixedWidth(100).FixedRightOf(cbxBlacklistedPlayersBounds, 10);

                composer
                    .AddStaticText(L("lblBlacklisted.Text"), LabelFont, EnumTextOrientation.Right, left, "lblBlacklisted")
                    .AddAutoSizeHoverText(L("lblBlacklisted.HoverText"), HoverTextFont, HOVER_TEXT_WIDTH, left)
                    .AddDropDown([.. blacklistedPlayers.Select(p => p.Id)], [.. blacklistedPlayers.Select(p => p.Name)], 0, OnSelectedBlacklistedPlayerChanged, cbxBlacklistedPlayersBounds, HoverTextFont, "cbxBlacklistedPlayers")
                    .AddSmallButton(L("btnRemoveBlacklistedPlayer.Text"), OnRemoveBlacklistPlayer, btnBlacklistedPlayerBounds, EnumButtonStyle.Small, "btnRemoveBlacklistedPlayer");
            }
        }
    }

    /// <summary>
    ///     Handles changes to the access mode drop-down and requests a recompose of the parent dialogue.
    /// </summary>
    /// <param name="code">The selected access mode code.</param>
    /// <param name="selected">Indicates whether the item was selected.</param>
    private void OnAccessModeChanged(string code, bool selected)
    {
        Settings.Mode = Enum.Parse<AccessMode>(code);
        Parent?.Recompose();
    }

    /// <summary>
    ///     Handler when a whitelistable player is selected from the drop-down.
    /// </summary>
    /// <param name="playerId">The selected player identifier.</param>
    /// <param name="selected">Indicates whether the item was selected.</param>
    private void OnSelectedWhitelistablePlayerChanged(string playerId, bool selected)
    {
        _selectedWhitelistablePlayerId = playerId;
    }

    /// <summary>
    ///     Adds the currently-selected whitelistable player to the settings whitelist and recomposes the parent.
    /// </summary>
    /// <returns>True if the player was added; otherwise false.</returns>
    private bool OnAddWhitelistPlayer()
    {
        if (string.IsNullOrEmpty(_selectedWhitelistablePlayerId)) return false;
        var player = _core.Capi.World.AllOnlinePlayers
            .Select(p => new Player(p.PlayerUID, p.PlayerName))
            .FirstOrDefault(p => p.Id == _selectedWhitelistablePlayerId);
        if (player is null) return false;
        Settings.Whitelist.Add(player);
        Parent?.Recompose();
        return true;
    }

    /// <summary>
    ///     Handler when a whitelisted player is selected from the drop-down.
    /// </summary>
    /// <param name="playerId">The selected player identifier.</param>
    /// <param name="selected">Indicates whether the item was selected.</param>
    private void OnSelectedWhitelistedPlayerChanged(string playerId, bool selected)
    {
        _selectedWhitelistedPlayerId = playerId;
    }

    /// <summary>
    ///     Removes the currently-selected whitelisted player from the settings and recomposes the parent.
    /// </summary>
    /// <returns>True if the player was removed; otherwise true (operation is idempotent).</returns>
    private bool OnRemoveWhitelistedPlayer()
    {
        Settings.Whitelist.RemoveAll(p => p.Id == _selectedWhitelistedPlayerId);
        Parent?.Recompose();
        return true;
    }

    /// <summary>
    ///     Handler when a blacklistable player is selected from the drop-down.
    /// </summary>
    /// <param name="playerId">The selected player identifier.</param>
    /// <param name="selected">Indicates whether the item was selected.</param>
    private void OnSelectedBlacklistablePlayerChanged(string playerId, bool selected)
    {
        _selectedBlacklistablePlayerId = playerId;
    }

    /// <summary>
    ///     Adds the currently-selected blacklistable player to the settings blacklist and recomposes the parent.
    /// </summary>
    /// <returns>True if the player was added; otherwise false.</returns>
    private bool OnAddBlacklistPlayer()
    {
        if (string.IsNullOrEmpty(_selectedBlacklistablePlayerId)) return false;
        var player = _core.Capi.World.AllOnlinePlayers
            .Select(p => new Player(p.PlayerUID, p.PlayerName))
            .FirstOrDefault(p => p.Id == _selectedBlacklistablePlayerId);
        if (player is null) return false;
        Settings.Blacklist.Add(player);
        Parent?.Recompose();
        return true;
    }

    /// <summary>
    ///     Handler when a blacklisted player is selected from the drop-down.
    /// </summary>
    /// <param name="playerId">The selected player identifier.</param>
    /// <param name="selected">Indicates whether the item was selected.</param>
    private void OnSelectedBlacklistedPlayerChanged(string playerId, bool selected)
    {
        _selectedBlacklistedPlayerId = playerId;
    }

    /// <summary>
    ///     Removes the currently-selected blacklisted player from the settings and recomposes the parent.
    /// </summary>
    /// <returns>True if the player was removed; otherwise true (operation is idempotent).</returns>
    private bool OnRemoveBlacklistPlayer()
    {
        Settings.Blacklist.RemoveAll(p => p.Id == _selectedBlacklistedPlayerId);
        Parent?.Recompose();
        return true;
    }

    /// <summary>
    ///     Sends the current settings to the server over the client network channel.
    /// </summary>
    /// <returns>True to indicate the button action was handled.</returns>
    private bool OnSaveButtonPressed()
    {
        ClientChannel?.SendPacket(Settings);
        return true;
    }

    /// <summary>
    ///     Adds a row for a strongly-typed setting property by inspecting the provided expression.
    ///     Supported property types are <see cref="bool"/>, <see cref="int"/> and <see cref="float"/>.
    /// </summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="expression">An expression that selects the property from the settings object.</param>
    /// <param name="left">The left bounds for the label.</param>
    /// <param name="right">The right bounds for the control.</param>
    protected void AddSettingRow<TProperty>(Expression<System.Func<TSettings, TProperty>> expression, ElementBounds left, ElementBounds right)
    {
        var member = (MemberExpression)expression.Body;
        var propertyInfo = (PropertyInfo)member.Member;
        var propertyName = propertyInfo.Name;
        var propertyType = propertyInfo.PropertyType;

        Composer
            .AddStaticText(F($"lbl{propertyName}.Text"), LabelFont, EnumTextOrientation.Right, left, $"lbl{propertyName}")
            .AddAutoSizeHoverText(F($"lbl{propertyName}.HoverText"), HoverTextFont, HOVER_TEXT_WIDTH, left);

        switch (propertyType)
        {
            case var t when t == typeof(bool):
                Composer.AddSwitch(state =>
                {
                    propertyInfo.SetValue(Settings, state);
                    RefreshValues(Composer);
                }, right, $"btn{propertyName}");
                break;
            case var t when t == typeof(int):
                Composer.AddSliderNew(value =>
                {
                    propertyInfo.SetValue(Settings, value);
                    RefreshValues(Composer);
                    return true;
                }, right, $"int{propertyName}");
                break;
            case var t when t == typeof(float):
                Composer.AddSlider<float>(value =>
                {
                    propertyInfo.SetValue(Settings, value);
                    RefreshValues(Composer);
                    return true;
                }, right, $"flt{propertyName}");
                break;
            default:
                throw new NotSupportedException($"Property type '{propertyType}' is not supported.");
        }
    }
}