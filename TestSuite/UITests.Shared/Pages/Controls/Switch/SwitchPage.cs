using OpenQA.Selenium;
using TestSuite.AutomationIds;
using UITests.Core;

namespace UITests.Pages.Controls.Switch;

/// <summary>
/// Page Object for <c>TestSuite/Views/Switch/SwitchControlPage.cs</c>.
/// Tests must go through this class rather than calling <c>FindElement</c>
/// directly (see spec/UITestArchitecture.md §10).
/// </summary>
public class SwitchPage : BasePage
{
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
