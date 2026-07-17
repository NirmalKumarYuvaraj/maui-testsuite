using NUnit.Framework;

using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
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

		var windowsOptions = new AppiumOptions
		{
			// Specify windows as the driver, typically don't need to change this
			AutomationName = "windows",
			// Always Windows for Windows
			PlatformName = "Windows",
			// The identifier of the deployed application to test
			// Update this to match your app's package family name (Get-AppxPackage in PowerShell after deploying)
			App = "com.companyname.testsuite_9zz4h110yvjzm!App",
		};

		// Note there are many more options that you can use to influence the app under test according to your needs

		driver = new WindowsDriver(windowsOptions);
		AppiumSetupBase.ApplyDefaultTimeouts(driver);
	}

	[OneTimeTearDown]
	public void RunAfterAnyTests()
	{
		// Quits the driver and, if an Appium server was started locally above, cleans it up.
		AppiumSetupBase.TearDown(driver);
	}
}
