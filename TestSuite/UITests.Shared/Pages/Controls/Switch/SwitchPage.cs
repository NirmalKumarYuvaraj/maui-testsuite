using OpenQA.Selenium;
using System.Globalization;
using TestSuite.AutomationIds;
using UITests.Core;
using UITests.Pages.Controls.Switch;
using HomePage = UITests.Pages.Home.CorePage;

namespace UITests.Pages.Controls;

/// <summary>
/// Page Object for <c>TestSuite/Views/Switch/SwitchControlPage.cs</c>.
/// Tests must go through this class rather than calling <c>FindElement</c>
/// directly (see spec/UITestArchitecture.md §10).
/// </summary>
public class SwitchPage : BasePage
{
    /// <summary>Whether the Switch control page is currently on screen.</summary>
    public bool IsDisplayed => TryFindElement(SwitchIds.Control) is not null;

    /// <summary>
    /// Navigates from the app's home page (<see cref="HomePage"/>) to the
    /// Switch control page, if not already there. The host app replaces the
    /// window's root page on navigation (see <c>TestSuite/Core/CorePage.cs</c>
    /// <c>PerformNavigation</c>) rather than pushing onto a shared stack, so
    /// there's no way back to home once here — this is a no-op if a previous
    /// test in the same fixture already navigated here. Call this from
    /// <c>[SetUp]</c> instead of <c>new SwitchPage()</c> directly.
    /// </summary>
    public static SwitchPage NavigateFromHome()
    {
        var switchPage = new SwitchPage();
        if (!switchPage.IsDisplayed)
        {
            new HomePage().Search("Switch");
            switchPage = new SwitchPage();
        }

        return switchPage;
    }

    /// <summary>Current toggled state of the Switch control under test.</summary>
    public bool IsToggled => IsToggleOn(SwitchIds.Control);

    /// <summary>
    /// Whether the Switch control under test is currently enabled. Backed by
    /// the standard WebDriver "enabled" element state (<see cref="OpenQA.Selenium.IWebElement.Enabled"/>),
    /// which — unlike <c>Selected</c> for toggle state (see
    /// <see cref="UITests.Core.BasePage.IsToggleOn"/>) — is consistently
    /// wired to the native enabled/disabled state across UIAutomator2,
    /// XCUITest and the Windows driver, so no per-platform branching is
    /// needed here.
    /// </summary>
    public bool IsEnabled => WaitForElement(SwitchIds.Control).Enabled;

    /// <summary>Toggles the Switch control and waits for the new state to be reflected.</summary>
    public void Toggle()
    {
        var wasToggled = IsToggled;

        FindElement(SwitchIds.Control).Click();
        WaitForCondition(() => IsToggled != wasToggled);
    }

    /// <summary>
    /// Clicks the Switch without waiting for its state to flip. Use this
    /// (instead of <see cref="Toggle"/>) only when asserting that a click
    /// should have <b>no</b> effect (e.g. a disabled Switch) — waiting via
    /// <see cref="Toggle"/> there would always run out its full timeout
    /// polling for a flip that's expected to never happen.
    /// </summary>
    public void AttemptToggle() => FindElement(SwitchIds.Control).Click();

    /// <summary>Navigates to the Switch's "Options" (properties) page.</summary>
    public SwitchPropertiesPage OpenOptions()
    {
        FindElement(SwitchIds.OptionsToolbarItem).Click();
        return new SwitchPropertiesPage();
    }

    // ── Description ──────────────────────────────────────────────────────

    /// <summary>Static text describing what this control page exercises.</summary>
    public string Description => WaitForElement(SwitchIds.DescriptionLabel).Text;

    // ── Toggled event / Command diagnostics ─────────────────────────────
    //
    // Microsoft.Maui.Controls.Switch has no Command/CommandParameter of its
    // own - these labels are populated manually by SwitchControlPage's
    // Toggled event handler (see TestSuite/Views/Switch/SwitchControlPage.cs),
    // which is the only way to make "did a Command fire in response to user
    // interaction" observable/testable for this control.

    /// <summary>
    /// Number of times the native <c>Toggled</c> event has fired on the
    /// Switch control under test since the page was created.
    /// </summary>
    public int ToggledEventCount => int.Parse(WaitForElement(SwitchIds.ToggledEventCountLabel).Text, CultureInfo.InvariantCulture);

    /// <summary>
    /// The <c>ToggledEventArgs.Value</c> from the most recent <c>Toggled</c>
    /// event, as rendered text ("True"/"False", or empty before any toggle
    /// has occurred).
    /// </summary>
    public string LastToggledValueText => WaitForElement(SwitchIds.LastToggledValueLabel).Text;

    /// <summary>
    /// Number of times <c>SwitchViewModel.ToggledCommand</c> has actually
    /// executed - distinct from <see cref="ToggledEventCount"/> because it
    /// proves the Command wiring itself works, not just that the event fired.
    /// </summary>
    public int CommandExecutionCount => int.Parse(WaitForElement(SwitchIds.CommandExecutionCountLabel).Text, CultureInfo.InvariantCulture);

    /// <summary>
    /// The parameter <c>ToggledCommand</c> was most recently invoked with -
    /// should match whatever was set via <see cref="SwitchPropertiesPage.SetCommandParameter"/>
    /// at the time of the last toggle.
    /// </summary>
    public string LastCommandParameterText => WaitForElement(SwitchIds.LastCommandParameterLabel).Text;
}
