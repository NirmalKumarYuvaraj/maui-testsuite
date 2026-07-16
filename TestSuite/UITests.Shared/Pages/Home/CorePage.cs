using TestSuite.AutomationIds;
using UITests.Core;

namespace UITests.Pages.Home;

/// <summary>
/// Page Object for <c>TestSuite/Core/CorePage.cs</c> — the app's landing
/// page (the window's root content, pushed by <c>CoreNavigationPage</c>).
/// Tests must go through this class rather than calling <c>FindElement</c>
/// directly (see spec/UITestArchitecture.md §10).
/// </summary>
public class CorePage : BasePage
{
    /// <summary>Whether the control-name search entry is present on screen.</summary>
    public bool IsSearchEntryDisplayed => TryFindElement(CoreIds.SearchEntry) is not null;

    /// <summary>Whether the grouped controls list is present on screen.</summary>
    public bool IsControlsListDisplayed => TryFindElement(CoreIds.ControlsList) is not null;

    /// <summary>Types <paramref name="text"/> into the search entry to filter the controls list.</summary>
    public void Search(string text)
    {
        var entry = WaitForElement(CoreIds.SearchEntry);
        entry.Clear();
        entry.SendKeys(text);
        FindElement(CoreIds.SearchButton).Click();
    }
}
