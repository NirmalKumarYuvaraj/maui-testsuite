using OpenQA.Selenium.Appium;

namespace UITests.Infrastructure;

/// <summary>
/// Centralizes screenshot capture so individual tests/page objects don't each
/// call <c>App.GetScreenshot().SaveAsFile(...)</c> (see
/// spec/UITestArchitecture.md §17), and drives baseline visual-regression
/// comparisons via <see cref="ScreenshotComparer"/> (see spec §22).
/// </summary>
public static class ScreenshotManager
{
    /// <summary>
    /// Set the <c>UPDATE_VISUAL_BASELINES</c> environment variable to
    /// <c>1</c>/<c>true</c> to (re)generate baseline images instead of
    /// comparing against them — e.g. after an intentional UI change.
    /// </summary>
    const string UpdateBaselinesEnvVar = "UPDATE_VISUAL_BASELINES";

    static readonly string BaselineDirectory = Path.Combine("Screenshots", "Baseline");
    static readonly string ActualDirectory = Path.Combine("Screenshots", "Actual");
    static readonly string DiffDirectory = Path.Combine("Screenshots", "Diff");

    static bool ShouldUpdateBaselines
    {
        get
        {
            var value = Environment.GetEnvironmentVariable(UpdateBaselinesEnvVar);
            return value is "1" or "true" or "True";
        }
    }

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

    /// <summary>
    /// Captures a screenshot named <paramref name="name"/> and compares it
    /// against <c>Screenshots/Baseline/{name}.png</c> using
    /// <see cref="ScreenshotComparer"/>.
    /// </summary>
    /// <remarks>
    /// If no baseline exists yet, this throws unless
    /// <c>UPDATE_VISUAL_BASELINES=1</c> is set, in which case the captured
    /// screenshot becomes the new baseline and the comparison is reported as
    /// a match. Diff images (when pixels differ) are written to
    /// <c>Screenshots/Diff/{name}.png</c>.
    /// </remarks>
    public static ImageComparisonResult CompareToBaseline(
        AppiumDriver driver,
        string name,
        double threshold = ScreenshotComparer.DefaultMatchThreshold)
    {
        Directory.CreateDirectory(ActualDirectory);
        var actualPath = Path.Combine(ActualDirectory, $"{name}.png");
        driver.GetScreenshot().SaveAsFile(actualPath);

        Directory.CreateDirectory(BaselineDirectory);
        var baselinePath = Path.Combine(BaselineDirectory, $"{name}.png");

        if (!File.Exists(baselinePath))
        {
            if (!ShouldUpdateBaselines)
            {
                throw new InvalidOperationException(
                    $"No baseline image found at '{baselinePath}'. Re-run with the " +
                    $"{UpdateBaselinesEnvVar}=1 environment variable set to create it, " +
                    "then review the generated image before committing it.");
            }

            File.Copy(actualPath, baselinePath, overwrite: true);
            return new ImageComparisonResult(Matches: true, DiffPercentage: 0, DiffImagePath: null);
        }

        Directory.CreateDirectory(DiffDirectory);
        var diffPath = Path.Combine(DiffDirectory, $"{name}.png");
        var result = ScreenshotComparer.Compare(baselinePath, actualPath, diffPath, threshold);

        if (ShouldUpdateBaselines)
            File.Copy(actualPath, baselinePath, overwrite: true);

        return result;
    }
}
