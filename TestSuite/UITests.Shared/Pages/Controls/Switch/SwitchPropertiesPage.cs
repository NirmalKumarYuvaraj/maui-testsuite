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
}
