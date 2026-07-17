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

    /// <summary>
    /// The current platform's snapshot folder name, matching the convention
    /// used by dotnet/maui's own visual regression tests (see
    /// <c>UITest.cs</c>'s <c>environmentName</c>/<c>VisualRegressionTester</c>):
    /// each platform gets its own subfolder under Baseline/Actual/Diff since
    /// screenshots naturally differ by platform (fonts, chrome, DPI, etc.).
    /// Resolved at compile time via the <c>UITEST_*</c> constants defined in
    /// each platform test project (UITests.iOS/.Android/.macOS/.Windows).
    /// </summary>
    static readonly string PlatformDirectoryName =
#if UITEST_IOS
        "iOS";
#elif UITEST_ANDROID
        "Android";
#elif UITEST_MACOS
        "macOS";
#elif UITEST_WINDOWS
        "Windows";
#else
        "Unknown";
#endif

    static readonly string BaselineDirectory = Path.Combine("Screenshots", "Baseline", PlatformDirectoryName);
    static readonly string ActualDirectory = Path.Combine("Screenshots", "Actual", PlatformDirectoryName);
    static readonly string DiffDirectory = Path.Combine("Screenshots", "Diff", PlatformDirectoryName);

    /// <summary>
    /// Default pixels to crop off each edge before comparing, matching
    /// dotnet/maui's own <c>UITest.cs</c> crop defaults: OS chrome (status
    /// bar, 3-button nav, title bar) isn't part of the app under test and
    /// changes between runs (clock, ripple/nav-button flash, theme), so it's
    /// cropped out rather than causing false-positive diffs. These pixel
    /// values assume the same reference simulator/emulator sizes MAUI's UI
    /// tests use — override via <see cref="CompareToBaseline"/>'s crop
    /// parameters if this suite targets different devices.
    /// </summary>
    static readonly (int Left, int Top, int Right, int Bottom) DefaultCropInsets =
#if UITEST_ANDROID
        (0, 60, 0, 125);
#elif UITEST_IOS
        (0, 110, 0, 40);
#elif UITEST_WINDOWS
        (0, 32, 0, 0);
#elif UITEST_MACOS
        (0, 29, 0, 0);
#else
        (0, 0, 0, 0);
#endif

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
    /// against <c>Screenshots/Baseline/{Platform}/{name}.png</c> using
    /// <see cref="ScreenshotComparer"/>.
    /// </summary>
    /// <remarks>
    /// If no baseline exists yet, this throws unless
    /// <c>UPDATE_VISUAL_BASELINES=1</c> is set, in which case the captured
    /// screenshot becomes the new baseline and the comparison is reported as
    /// a match. Diff images (when pixels differ) are written to
    /// <c>Screenshots/Diff/{Platform}/{name}.png</c>.
    /// Crop parameters default to <see cref="DefaultCropInsets"/> (per-platform
    /// status bar/nav bar/title bar insets) when left <see langword="null"/>;
    /// pass 0 explicitly for an edge to disable cropping there instead of
    /// using the default.
    /// </remarks>
    /// <param name="cropLeft">Pixels to crop from the left edge before comparing.</param>
    /// <param name="cropTop">Pixels to crop from the top edge before comparing.</param>
    /// <param name="cropRight">Pixels to crop from the right edge before comparing.</param>
    /// <param name="cropBottom">Pixels to crop from the bottom edge before comparing.</param>
    public static ImageComparisonResult CompareToBaseline(
        AppiumDriver driver,
        string name,
        double threshold = ScreenshotComparer.DefaultMatchThreshold,
        int? cropLeft = null,
        int? cropTop = null,
        int? cropRight = null,
        int? cropBottom = null)
    {
        Directory.CreateDirectory(ActualDirectory);
        var actualPath = Path.Combine(ActualDirectory, $"{name}.png");

        var screenshotBytes = ScreenshotComparer.Crop(
            driver.GetScreenshot().AsByteArray,
            cropLeft ?? DefaultCropInsets.Left,
            cropTop ?? DefaultCropInsets.Top,
            cropRight ?? DefaultCropInsets.Right,
            cropBottom ?? DefaultCropInsets.Bottom);
        File.WriteAllBytes(actualPath, screenshotBytes);

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
