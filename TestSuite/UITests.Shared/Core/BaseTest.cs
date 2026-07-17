using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using UITests.Infrastructure;

// AppiumSetup is defined per-platform in the root UITests namespace ([SetUpFixture]
// scoping relies on all shared test code living under the UITests.* namespace tree).
using UITests;

namespace UITests.Core;

public abstract class BaseTest
{
	protected AppiumDriver App => AppiumSetup.App;

	// This could also be an extension method to AppiumDriver if you prefer
	protected AppiumElement FindUIElement(string id)
	{
		if (App is WindowsDriver)
		{
			return App.FindElement(MobileBy.AccessibilityId(id));
		}

		return App.FindElement(MobileBy.Id(id));
	}

	/// <summary>
	/// Explicit, condition-based wait for use directly in test methods that
	/// don't yet go through a Page Object. Prefer this over
	/// <c>Thread.Sleep</c>/<c>Task.Delay</c> (see spec/UITestArchitecture.md §15).
	/// </summary>
	protected bool WaitForCondition(Func<bool> condition, TimeSpan? timeout = null)
		=> WaitHelper.WaitUntil(condition, timeout);

	protected void TakeScreenshot(string name)
		=> ScreenshotManager.Capture(App, name);

	/// <summary>
	/// Captures a screenshot and compares it against its stored baseline
	/// image (see spec/UITestArchitecture.md §22). Assert on
	/// <see cref="ImageComparisonResult.Matches"/> in the test body so the
	/// failure message can include the diff percentage/path. Crop params
	/// default to the platform's status bar/nav bar/title bar insets; pass 0
	/// explicitly for an edge to disable cropping there instead.
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
