namespace TestSuite.AutomationIds;

/// <summary>
/// Single source of truth for the <c>AutomationId</c>s used on the Switch
/// control gallery pages. This file is compiled directly into the host app
/// (<c>TestSuite</c>) and is also linked (not project-referenced — see
/// UITests.Shared.csproj / UITests.*.csproj) into the UI test projects, so
/// the host app and its Page Objects can never drift apart on a locator
/// string (see spec/UITestArchitecture.md §4.3/§18).
/// </summary>
public static class SwitchIds
{
    public const string Control = "Switch.Control";
    public const string OptionsToolbarItem = "Switch.Options";
    public const string ApplyToolbarItem = "Switch.Apply";
    public const string NavigateToViewPropertiesButton = "Switch.NavigateToViewProperties";

    // Switch-specific properties
    public const string IsToggledSwitch = "Switch.IsToggled";
    public const string OnColorEntry = "Switch.OnColor";
    public const string OffColorEntry = "Switch.OffColor";
    public const string ThumbColorEntry = "Switch.ThumbColor";

    // Description
    public const string DescriptionLabel = "Switch.Description";

    // Event/Command diagnostics
    public const string ToggledEventCountLabel = "Switch.ToggledEventCount";
    public const string LastToggledValueLabel = "Switch.LastToggledValue";
    public const string CommandExecutionCountLabel = "Switch.CommandExecutionCount";
    public const string LastCommandParameterLabel = "Switch.LastCommandParameter";
    public const string CommandParameterEntry = "Switch.CommandParameterEntry";
}
