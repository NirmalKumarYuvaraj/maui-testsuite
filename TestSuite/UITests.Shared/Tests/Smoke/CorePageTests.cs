using NUnit.Framework;
using UITests.Core;
using UITests.Infrastructure;
using CorePage = UITests.Pages.Home.CorePage;

// You will have to make sure that all the namespaces match
// between the different platform specific projects and the shared
// code files. This has to do with how we initialize the AppiumDriver
// through the AppiumSetup.cs files and NUnit SetUpFixture attributes.
// Also see: https://docs.nunit.org/articles/nunit/writing-tests/attributes/setupfixture.html
namespace UITests.Tests.Smoke;

// Smoke test for the app's landing page (TestSuite.Core.CorePage), which
// replaced the MAUI project template's default MainPage/counter demo.
[TestFixture]
[Category(UITestCategories.Smoke)]
public class CorePageTests : BaseTest
{
	[Test]
	public void AppLaunches()
	{
		var homePage = new CorePage();

		TakeScreenshot(nameof(AppLaunches));

		Assert.Multiple(() =>
		{
			Assert.That(homePage.IsSearchEntryDisplayed, Is.True, "Search entry should be visible on launch.");
			Assert.That(homePage.IsControlsListDisplayed, Is.True, "Controls list should be visible on launch.");
		});
	}
}
