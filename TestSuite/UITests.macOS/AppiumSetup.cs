using NUnit.Framework;

using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.Mac;
using UITests.Infrastructure;

namespace UITests;

[SetUpFixture]
public class AppiumSetup
{
	private static AppiumDriver? driver;

	public static AppiumDriver App => driver ?? throw new NullReferenceException("AppiumDriver is null");

	[OneTimeSetUp]
	public void RunBeforeAnyTests()
	{
		// If you started an Appium server manually, make sure to comment out the next line
		// This line starts a local Appium server for you as part of the test run
		AppiumSetupBase.StartServer();

		var macOptions = new AppiumOptions
		{
			// Specify mac2 as the driver, typically don't need to change this
			AutomationName = "mac2",
			// Always Mac for Mac
			PlatformName = "Mac",
			// The full path to the .app file to test
			App = "/path/to/TestSuite/bin/Debug/net10.0-maccatalyst/maccatalyst-x64/TestSuite.app",
		};

		// Setting the Bundle ID is required, else the automation will run on Finder
		macOptions.AddAdditionalAppiumOption(IOSMobileCapabilityType.BundleId, "com.companyname.testsuite");

		// Note there are many more options that you can use to influence the app under test according to your needs

		driver = new MacDriver(macOptions);
		AppiumSetupBase.ApplyDefaultTimeouts(driver);
	}

	[OneTimeTearDown]
	public void RunAfterAnyTests()
	{
		// Quits the driver and, if an Appium server was started locally above, cleans it up.
		AppiumSetupBase.TearDown(driver);
	}
}
