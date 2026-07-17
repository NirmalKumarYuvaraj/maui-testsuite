using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Appium.Mac;
using OpenQA.Selenium.Appium.Windows;
using UITests.Infrastructure;

// AppiumSetup is defined per-platform in the root UITests namespace ([SetUpFixture]
// scoping relies on all shared test code living under the UITests.* namespace tree
// — see the note in BaseTest.cs / tasks.md Phase 2).
using UITests;

namespace UITests.Core;

/// <summary>
/// Base class for every Page Object (see spec/UITestArchitecture.md §10).
/// Page Objects derive from this instead of talking to Appium directly, so
/// element lookup, waiting, and screenshot capture stay in one place.
/// </summary>
public abstract class BasePage
{
    protected AppiumDriver App => AppiumSetup.App;

    /// <summary>
    /// Resolves an element by its <c>AutomationId</c>. AutomationId is the only
    /// supported locator strategy for this project (spec §4.4/§18) — never
    /// locate by text, XPath, or index.
    /// </summary>
    protected AppiumElement FindElement(string automationId)
    {
        return App is WindowsDriver
            ? App.FindElement(MobileBy.AccessibilityId(automationId))
            : App.FindElement(MobileBy.Id(automationId));
    }

    /// <summary>Same as <see cref="FindElement"/> but returns <see langword="null"/> instead of throwing.</summary>
    protected AppiumElement? TryFindElement(string automationId)
    {
        try
        {
            return FindElement(automationId);
        }
        catch (NoSuchElementException)
        {
            return null;
        }
    }

    /// <summary>
    /// Waits for an element to exist, polling until <paramref name="timeout"/>
    /// elapses (default <see cref="WaitHelper.DefaultTimeout"/>).
    /// </summary>
    protected AppiumElement WaitForElement(string automationId, TimeSpan? timeout = null)
    {
        AppiumElement? found = null;

        WaitHelper.WaitUntil(() => (found = TryFindElement(automationId)) is not null, timeout);

        return found ?? throw new NoSuchElementException(
            $"Element with AutomationId '{automationId}' was not found within the timeout.");
    }

    /// <summary>Waits for an arbitrary condition (e.g. a bound label's text updating).</summary>
    protected bool WaitForCondition(Func<bool> condition, TimeSpan? timeout = null)
        => WaitHelper.WaitUntil(condition, timeout);

    /// <summary>
    /// Reads the on/off state of a MAUI <c>Switch</c> (or other checkable
    /// toggle control) identified by <paramref name="automationId"/>.
    /// <para>
    /// Don't use <see cref="AppiumElement.Selected"/> for this: despite the
    /// W3C WebDriver spec saying "selected" applies to toggle buttons, in
    /// practice UIAutomator2 maps it to the native <c>View.isSelected()</c>
    /// state (unrelated to a Switch's checked/toggle state) and it never
    /// changes when the Switch is toggled, while XCUITest only exposes the
    /// toggle's actual on/off state via the element's "value" attribute.
    /// Read the platform-appropriate attribute instead.
    /// </para>
    /// </summary>
    protected bool IsToggleOn(string automationId)
    {
        var element = WaitForElement(automationId);

        return App switch
        {
            IOSDriver or MacDriver => element.GetAttribute("value") is "1" or "true",
            WindowsDriver => element.Selected,
            _ => element.GetAttribute("checked") is "true",
        };
    }

    protected void TakeScreenshot(string name)
        => ScreenshotManager.Capture(App, name);

    /// <summary>
    /// Captures a screenshot and compares it against its stored baseline
    /// image (see spec/UITestArchitecture.md §22). Crop params default to
    /// the platform's status bar/nav bar/title bar insets; pass 0 explicitly
    /// for an edge to disable cropping there instead.
    /// </summary>
    protected ImageComparisonResult CompareToBaseline(
        string name,
        double threshold = ScreenshotComparer.DefaultMatchThreshold,
        int? cropLeft = null,
        int? cropTop = null,
        int? cropRight = null,
        int? cropBottom = null)
        => ScreenshotManager.CompareToBaseline(App, name, threshold, cropLeft, cropTop, cropRight, cropBottom);
}
