using OpenQA.Selenium.Appium;

namespace UITests.Infrastructure;

/// <summary>
/// Centralizes screenshot capture so individual tests/page objects don't each
/// call <c>App.GetScreenshot().SaveAsFile(...)</c> (see
/// spec/UITestArchitecture.md §17).
/// </summary>
public static class ScreenshotManager
{
    /// <summary>Captures a screenshot named "{name}.png" in the test run's working directory.</summary>
    public static string Capture(AppiumDriver driver, string name)
    {
        var fileName = $"{name}.png";
        driver.GetScreenshot().SaveAsFile(fileName);
        return fileName;
    }

    /// <summary>Captures a screenshot tagged as a failure for the given test name.</summary>
    public static string CaptureOnFailure(AppiumDriver driver, string testName)
        => Capture(driver, $"{testName}_FAILURE");
}
