namespace TestSuite.AutomationIds;

/// <summary>
/// Single source of truth for the <c>AutomationId</c>s on the shared
/// <c>Views/Base/*PropertiesPage</c> editors (see spec/UITestArchitecture.md
/// §4.2/§18). These pages are reused by every control's Properties flow, so
/// the IDs are not control-specific — only one instance is ever on screen at
/// a time. Linked into the UI test projects the same way as
/// <see cref="SwitchIds"/>.
/// </summary>
public static class BaseViewIds
{
    public const string OpacityEntry = "BaseView.Opacity";
    public const string IsEnabledSwitch = "BaseView.IsEnabled";
    public const string IsVisibleSwitch = "BaseView.IsVisible";
    public const string ApplyToolbarItem = "BaseView.Apply";
}
