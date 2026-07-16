using NUnit.Framework;
using UITests.Core;
using UITests.Infrastructure;

// You will have to make sure that all the namespaces match
// between the different platform specific projects and the shared
// code files. This has to do with how we initialize the AppiumDriver
// through the AppiumSetup.cs files and NUnit SetUpFixture attributes.
// Also see: https://docs.nunit.org/articles/nunit/writing-tests/attributes/setupfixture.html
namespace UITests.Tests.Smoke;

// This is an example of tests that do not need anything platform specific
[TestFixture]
[Category(UITestCategories.Smoke)]
public class MainPageTests : BaseTest
{
	[Test]
	public void AppLaunches()
	{
		TakeScreenshot(nameof(AppLaunches));
	}

	[Test]
	public void ClickCounterTest()
	{
		// Arrange
		// Find elements with the value of the AutomationId property
		var element = FindUIElement("CounterBtn");

		// Act
		element.Click();

		// Wait for the click to register instead of a fixed delay, so the test
		// doesn't hardcode how long the UI update should take.
		WaitForCondition(() => element.Text == "Clicked 1 time");

		// Assert
		TakeScreenshot(nameof(ClickCounterTest));
		Assert.That(element.Text, Is.EqualTo("Clicked 1 time"));
	}
}
