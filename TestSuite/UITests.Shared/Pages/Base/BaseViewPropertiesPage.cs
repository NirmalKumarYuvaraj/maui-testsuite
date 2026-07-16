using System.Globalization;
using TestSuite.AutomationIds;
using UITests.Core;

namespace UITests.Pages.Base;

/// <summary>
/// Page Object for the shared <c>Views/Base/BaseViewPropertiesPage.cs</c>
/// editor, reused by every control's Properties flow (spec
/// /UITestArchitecture.md §4.2/§10). Only the fields the host app currently
/// wires up to a bindable property (Opacity, IsEnabled, IsVisible) have
/// typed setters here — the rest of that page (alignment, background,
/// ZIndex, etc.) is not yet functional in the host app, so no automation is
/// exposed for it until it is (see tasks.md Phase 4 notes).
/// </summary>
public class BaseViewPropertiesPage : BasePage
{
    public void SetOpacity(double opacity)
    {
        var entry = WaitForElement(BaseViewIds.OpacityEntry);
        entry.Clear();
        entry.SendKeys(opacity.ToString(CultureInfo.InvariantCulture));
    }

    public void SetEnabled(bool isEnabled)
        => SetSwitch(BaseViewIds.IsEnabledSwitch, isEnabled);

    public void SetVisible(bool isVisible)
        => SetSwitch(BaseViewIds.IsVisibleSwitch, isVisible);

    /// <summary>
    /// Applies changes. The host app's "Apply" toolbar item calls
    /// <c>Navigation.PopToRootAsync()</c>, which returns to whichever
    /// control's <c>ControlPage</c> is the root of the current
    /// <c>NavigationPage</c> — this shared page object doesn't know which
    /// control that is, so callers should re-anchor via that control's own
    /// Page Object afterwards (e.g. <c>new SwitchPage()</c>).
    /// </summary>
    public void Apply()
        => FindElement(BaseViewIds.ApplyToolbarItem).Click();

    void SetSwitch(string automationId, bool desiredState)
    {
        var toggle = WaitForElement(automationId);
        if (toggle.Selected != desiredState)
            toggle.Click();
    }
}
