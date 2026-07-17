using OpenQA.Selenium;
using TestSuite.AutomationIds;
using UITests.Core;
using HomePage = UITests.Pages.Home.CorePage;

namespace UITests.Pages.Controls.Switch;

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
    public bool IsToggled => WaitForElement(SwitchIds.Control).Selected;

    /// <summary>Toggles the Switch control and waits for the new state to be reflected.</summary>
    public void Toggle()
    {
        var wasToggled = IsToggled;

        FindElement(SwitchIds.Control).Click();
        WaitForCondition(() => IsToggled != wasToggled);
    }

    /// <summary>Navigates to the Switch's "Options" (properties) page.</summary>
    public SwitchPropertiesPage OpenOptions()
    {
        FindElement(SwitchIds.OptionsToolbarItem).Click();
        return new SwitchPropertiesPage();
    }
}
