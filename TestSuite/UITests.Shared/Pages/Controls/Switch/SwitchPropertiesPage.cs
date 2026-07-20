using TestSuite.AutomationIds;
using UITests.Core;
using UITests.Pages.Base;

namespace UITests.Pages.Controls.Switch;

/// <summary>
/// Page Object for <c>TestSuite/Views/Switch/SwitchPropertiesPage.cs</c>
/// (the Switch control's "Options" page).
/// </summary>
public class SwitchPropertiesPage : BasePage
{
    // ── Switch-specific properties ──────────────────────────────────────

    /// <summary>
    /// State of the "Is Toggled" switch on this Options page. It's bound to
    /// the same <c>SwitchViewModel.IsToggled</c> property as the control
    /// under test's own Switch, so it's a second, independent way to read/
    /// set that state (distinct from clicking the control's own Switch via
    /// <see cref="UITests.Pages.Controls.SwitchPage.Toggle"/>).
    /// </summary>
    public bool IsToggled => IsToggleOn(SwitchIds.IsToggledSwitch);

    public void SetIsToggled(bool desiredState)
    {
        if (IsToggled != desiredState)
            FindElement(SwitchIds.IsToggledSwitch).Click();
    }

    public void SetOnColor(string color) => SetEntry(SwitchIds.OnColorEntry, color);

    public void SetOffColor(string color) => SetEntry(SwitchIds.OffColorEntry, color);

    public void SetThumbColor(string color) => SetEntry(SwitchIds.ThumbColorEntry, color);

    /// <summary>
    /// Sets the parameter that will be passed to <c>SwitchViewModel.ToggledCommand</c>
    /// the next time the Toggled event fires (see <see cref="UITests.Pages.Controls.SwitchPage.CommandExecutionCount"/>
    /// and <see cref="UITests.Pages.Controls.SwitchPage.LastCommandParameterText"/>).
    /// </summary>
    public void SetCommandParameter(string value) => SetEntry(SwitchIds.CommandParameterEntry, value);

    public string CommandParameterText => WaitForElement(SwitchIds.CommandParameterEntry).Text;

    /// <summary>
    /// Current text in the On/Off/Thumb color entries. Exposed for arrange
    /// verification (spec/TestPlan.md §7.1) — confirm the field accepted the
    /// typed value before Apply, since there's no reliable cross-platform
    /// native attribute to read a Switch's actual rendered color back from;
    /// real verification of the applied color still requires a visual
    /// regression comparison (see <c>SwitchFeatureMatrix.cs</c>).
    /// </summary>
    public string OnColorText => WaitForElement(SwitchIds.OnColorEntry).Text;

    public string OffColorText => WaitForElement(SwitchIds.OffColorEntry).Text;

    public string ThumbColorText => WaitForElement(SwitchIds.ThumbColorEntry).Text;

    /// <summary>Navigates to the shared View Properties editor for this Switch instance.</summary>
    public BaseViewPropertiesPage OpenViewProperties()
    {
        FindElement(SwitchIds.NavigateToViewPropertiesButton).Click();
        return new BaseViewPropertiesPage();
    }

    /// <summary>
    /// Applies changes and returns to the Switch control page. The host app's
    /// "Apply" toolbar item calls <c>Navigation.PopToRootAsync()</c>, which
    /// pops all the way back to <c>SwitchControlPage</c> (the root of this
    /// control's own <c>NavigationPage</c>), not just one level up.
    /// </summary>
    public SwitchPage Apply()
    {
        FindElement(SwitchIds.ApplyToolbarItem).Click();
        return new SwitchPage();
    }

    void SetEntry(string automationId, string value)
    {
        var entry = WaitForElement(automationId);
        entry.Clear();
        if (!string.IsNullOrEmpty(value))
            entry.SendKeys(value);
    }
}
