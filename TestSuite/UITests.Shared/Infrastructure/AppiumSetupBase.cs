using OpenQA.Selenium.Appium;

namespace UITests.Infrastructure;

/// <summary>
/// Shared Appium session policy consumed by each platform's <c>AppiumSetup</c>
/// (<c>[SetUpFixture]</c>). Extracts the setup/teardown behavior that was
/// previously duplicated near-verbatim across
/// UITests.Android/iOS/macOS/Windows so timeouts and server lifecycle can't
/// silently drift between platforms (see spec/UITestArchitecture.md §4.6/§9).
/// Each platform's <c>AppiumSetup</c> keeps only what is genuinely
/// platform-specific: the concrete driver type and its capabilities.
/// </summary>
public static class AppiumSetupBase
{
    /// <summary>Implicit wait applied to the driver once it's created.</summary>
    public static readonly TimeSpan DefaultImplicitWait = TimeSpan.FromSeconds(2);

    /// <summary>Starts the shared local Appium server (a no-op if already running).</summary>
    public static void StartServer()
        => AppiumServerHelper.StartAppiumLocalServer();

    /// <summary>Applies the shared timeout policy to a freshly created driver.</summary>
    public static void ApplyDefaultTimeouts(AppiumDriver driver)
        => driver.Manage().Timeouts().ImplicitWait = DefaultImplicitWait;

    /// <summary>Quits the driver (if any) and disposes the shared local Appium server.</summary>
    public static void TearDown(AppiumDriver? driver)
    {
        driver?.Quit();
        AppiumServerHelper.DisposeAppiumLocalServer();
    }
}
