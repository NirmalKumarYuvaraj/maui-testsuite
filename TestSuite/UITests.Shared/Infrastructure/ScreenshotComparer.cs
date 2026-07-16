using SkiaSharp;

namespace UITests.Infrastructure;

/// <summary>Result of comparing an "actual" screenshot against a baseline image.</summary>
/// <param name="Matches">Whether the diff percentage is within the allowed threshold.</param>
/// <param name="DiffPercentage">Fraction (0.0–1.0) of pixels that differ beyond the per-pixel tolerance.</param>
/// <param name="DiffImagePath">
/// Path to the generated diff image (mismatched pixels highlighted in red), or
/// <see langword="null"/> if there were no differing pixels at all.
/// </param>
public sealed record ImageComparisonResult(bool Matches, double DiffPercentage, string? DiffImagePath);

/// <summary>
/// Pixel-by-pixel image comparison for visual regression testing
/// (see spec/UITestArchitecture.md §22). Pure comparison logic — no Appium
/// dependency — so it can be exercised directly in isolation.
/// </summary>
/// <remarks>
/// Uses <c>SkiaSharp</c> (MIT licensed, free for any use — it's the same
/// rendering engine .NET MAUI itself is built on) rather than
/// SixLabors.ImageSharp, which requires a paid commercial license for
/// closed-source use once an organization exceeds $1M annual revenue.
/// </remarks>
public static class ScreenshotComparer
{
    /// <summary>
    /// Default fraction of pixels allowed to differ before a comparison is
    /// considered a mismatch. A small non-zero default absorbs font
    /// anti-aliasing/rendering noise that varies harmlessly between runs.
    /// </summary>
    public const double DefaultMatchThreshold = 0.001; // 0.1% of pixels

    /// <summary>
    /// Per-channel (R/G/B/A) difference, on a 0-255 scale, below which two
    /// pixels are still considered equal. Absorbs minor anti-aliasing/JPEG-ish
    /// compression noise without masking real visual differences.
    /// </summary>
    public const int DefaultChannelTolerance = 12;

    /// <summary>
    /// Compares <paramref name="actualPath"/> against <paramref name="baselinePath"/>.
    /// Images of different dimensions are always treated as a complete
    /// mismatch (100% diff) since pixels can't be meaningfully compared 1:1.
    /// </summary>
    /// <param name="diffImagePath">
    /// If provided and any pixels differ, a diff image is written here:
    /// differing pixels in opaque red, matching pixels dimmed so the
    /// mismatches stand out.
    /// </param>
    public static ImageComparisonResult Compare(
        string baselinePath,
        string actualPath,
        string? diffImagePath = null,
        double threshold = DefaultMatchThreshold,
        int channelTolerance = DefaultChannelTolerance)
    {
        using var baseline = SKBitmap.Decode(baselinePath);
        using var actual = SKBitmap.Decode(actualPath);

        if (baseline.Width != actual.Width || baseline.Height != actual.Height)
        {
            return new ImageComparisonResult(Matches: false, DiffPercentage: 1.0, DiffImagePath: null);
        }

        int width = baseline.Width;
        int height = baseline.Height;
        long diffPixelCount = 0;

        // Pixels snapshots the whole bitmap into an array once, which is
        // dramatically faster than per-pixel GetPixel calls for full
        // screenshot-sized images.
        var baselinePixels = baseline.Pixels;
        var actualPixels = actual.Pixels;
        var diffPixels = diffImagePath is not null ? new SKColor[baselinePixels.Length] : null;

        for (var i = 0; i < baselinePixels.Length; i++)
        {
            var baselinePixel = baselinePixels[i];
            var actualPixel = actualPixels[i];
            var differs = !PixelsMatch(baselinePixel, actualPixel, channelTolerance);

            if (differs)
                diffPixelCount++;

            if (diffPixels is not null)
            {
                diffPixels[i] = differs
                    ? new SKColor(255, 0, 0, 255)
                    : Dim(actualPixel);
            }
        }

        var diffPercentage = (double)diffPixelCount / (width * (long)height);
        var matches = diffPercentage <= threshold;

        string? writtenDiffPath = null;
        if (diffPixels is not null && diffPixelCount > 0)
        {
            using var diffBitmap = new SKBitmap(width, height);
            diffBitmap.Pixels = diffPixels;

            using var image = SKImage.FromBitmap(diffBitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, quality: 100);
            using var stream = File.OpenWrite(diffImagePath!);
            data.SaveTo(stream);

            writtenDiffPath = diffImagePath;
        }

        return new ImageComparisonResult(matches, diffPercentage, writtenDiffPath);
    }

    static bool PixelsMatch(SKColor a, SKColor b, int channelTolerance)
        => Math.Abs(a.Red - b.Red) <= channelTolerance
        && Math.Abs(a.Green - b.Green) <= channelTolerance
        && Math.Abs(a.Blue - b.Blue) <= channelTolerance
        && Math.Abs(a.Alpha - b.Alpha) <= channelTolerance;

    // Dims a matching pixel toward gray so the red diff highlights pop visually.
    static SKColor Dim(SKColor pixel) => new(
        (byte)(pixel.Red / 3),
        (byte)(pixel.Green / 3),
        (byte)(pixel.Blue / 3),
        pixel.Alpha);
}
