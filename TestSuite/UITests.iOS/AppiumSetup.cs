using NUnit.Framework;

using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;
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

		var iOSOptions = new AppiumOptions
		{
			// Specify XCUITest as the driver, typically don't need to change this
			AutomationName = "XCUITest",
			// Always iOS for iOS
			PlatformName = "iOS",
			// iOS Version
			PlatformVersion = "18.5",
			// Don't specify if you don't want a specific device
			DeviceName = "iPhone Xs",
			// The full path to the .app file to test or the bundle id if the app is already installed on the device
			App = "com.companyname.testsuite",
		};

		// Note there are many more options that you can use to influence the app under test according to your needs

		driver = new IOSDriver(iOSOptions);
		AppiumSetupBase.ApplyDefaultTimeouts(driver);
	}

	[OneTimeTearDown]
	public void RunAfterAnyTests()
	{
		// Quits the driver and, if an Appium server was started locally above, cleans it up.
		AppiumSetupBase.TearDown(driver);
	}
}
