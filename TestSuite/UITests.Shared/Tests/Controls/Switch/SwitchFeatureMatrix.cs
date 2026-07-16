using NUnit.Framework;
using UITests.Core;
using UITests.Data;
using UITests.Infrastructure;
using UITests.Pages.Controls.Switch;

namespace UITests.Tests.Controls.Switch;

/// <summary>
/// All test coverage for the Switch control lives here: basic functional
/// behavior, combinatorial property-crossed behavior (data-driven via
/// <c>Data/*</c>), and visual regression — see
/// spec/UITestArchitecture.md §13 "Functional Tests" / "Feature Matrix" and
/// §22 "Visual Regression". Kept as a single file/fixture per control
/// rather than splitting into separate `*Tests.cs`/`*FeatureMatrix.cs`/
/// `*VisualRegressionTests.cs` files, since a control's test surface is
/// small enough that the split added navigation overhead without a real
/// separation-of-concerns benefit. Individual tests still carry their own
/// `[Category]` (`FeatureMatrix`/`VisualRegression`) so they can be
/// filtered/run independently.
/// </summary>
[TestFixture]
[Category(UITestCategories.Switch)]
public class SwitchFeatureMatrix : BaseTest
{
    SwitchPage _switchPage = null!;

    [SetUp]
    public void SetUp()
    {
        // Navigates from the app's home page on the first test in this
        // fixture; a no-op for subsequent tests that are already on the
        // Switch control page (see SwitchPage.NavigateFromHome).
        _switchPage = SwitchPage.NavigateFromHome();
    }

    // ── Functional tests ─────────────────────────────────────────────────

    [Test]
    public void Toggle_ChangesIsToggledState()
    {
        var initialState = _switchPage.IsToggled;

        _switchPage.Toggle();

        Assert.That(_switchPage.IsToggled, Is.Not.EqualTo(initialState));
    }

    [Test]
    public void Toggle_Twice_ReturnsToOriginalState()
    {
        var initialState = _switchPage.IsToggled;

        _switchPage.Toggle();
        _switchPage.Toggle();

        Assert.That(_switchPage.IsToggled, Is.EqualTo(initialState));
    }

    // ── Feature matrix (combinatorial, data-driven) ─────────────────────

    [Category(UITestCategories.FeatureMatrix)]
    [TestCaseSource(typeof(OpacityData), nameof(OpacityData.Values))]
    public void Toggle_StillWorks_AtGivenOpacity(double opacity)
    {
        var propertiesPage = _switchPage.OpenOptions().OpenViewProperties();
        propertiesPage.SetOpacity(opacity);
        propertiesPage.Apply();

        // "Apply" pops back to the Switch control page — re-anchor the page object.
        _switchPage = new SwitchPage();

        var initialState = _switchPage.IsToggled;
        _switchPage.Toggle();

        Assert.That(_switchPage.IsToggled, Is.Not.EqualTo(initialState));
    }

    [Category(UITestCategories.FeatureMatrix)]
    [TestCaseSource(typeof(VisibilityData), nameof(VisibilityData.Values))]
    public void Toggle_StillWorks_AtGivenVisibility(bool isVisible)
    {
        var propertiesPage = _switchPage.OpenOptions().OpenViewProperties();
        propertiesPage.SetVisible(isVisible);
        propertiesPage.Apply();
        _switchPage = new SwitchPage();

        if (!isVisible)
            return; // A hidden control can't be interacted with — covered by IsVisible = false above.

        var initialState = _switchPage.IsToggled;
        _switchPage.Toggle();

        Assert.That(_switchPage.IsToggled, Is.Not.EqualTo(initialState));
    }

    // ── Visual regression ────────────────────────────────────────────────

    [Category(UITestCategories.VisualRegression)]
    [Test]
    public void SwitchControlPage_MatchesBaseline_WhenToggledOff()
    {
        // Pin to a known state so the baseline isn't sensitive to whatever
        // toggle state a previous test left the control in.
        if (_switchPage.IsToggled)
            _switchPage.Toggle();

        var result = CompareToBaseline(nameof(SwitchControlPage_MatchesBaseline_WhenToggledOff));

        Assert.That(result.Matches, Is.True,
            $"Screenshot differs from baseline by {result.DiffPercentage:P2}. " +
            $"Diff image: {result.DiffImagePath ?? "(none, likely a dimension mismatch)"}. " +
            $"If this change is intentional, re-run with UPDATE_VISUAL_BASELINES=1 to accept it.");
    }
}
