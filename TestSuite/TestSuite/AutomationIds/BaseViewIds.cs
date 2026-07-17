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
    // Layout & size
    public const string LayoutAndSizeButton = "BaseView.LayoutAndSize";

    // Alignment
    public const string HorizontalOptionsEntry = "BaseView.HorizontalOptions";
    public const string VerticalOptionsEntry = "BaseView.VerticalOptions";
    public const string FlowDirectionEntry = "BaseView.FlowDirection";

    // Appearance
    public const string OpacityEntry = "BaseView.Opacity";
    public const string BackgroundEntry = "BaseView.Background";

    // Behavior
    public const string IsEnabledSwitch = "BaseView.IsEnabled";
    public const string InputTransparentSwitch = "BaseView.InputTransparent";
    public const string IsVisibleSwitch = "BaseView.IsVisible";

    // Advanced
    public const string ZIndexEntry = "BaseView.ZIndex";
    public const string ShadowOptionsButton = "BaseView.ShadowOptions";
    public const string ClipOptionsButton = "BaseView.ClipOptions";
    public const string MoreOptionsButton = "BaseView.MoreOptions";

    // Read-only info
    public const string IsFocusedLabel = "BaseView.IsFocused";
    public const string DesiredSizeLabel = "BaseView.DesiredSize";
    public const string FrameLabel = "BaseView.Frame";

    public const string ApplyToolbarItem = "BaseView.Apply";
}
