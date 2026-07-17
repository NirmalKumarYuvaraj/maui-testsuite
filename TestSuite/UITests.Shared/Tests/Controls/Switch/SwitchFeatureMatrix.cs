using NUnit.Framework;
using OpenQA.Selenium;
using System.Globalization;
using UITests.Core;
using UITests.Data;
using UITests.Infrastructure;
using UITests.Pages.Controls;
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

    [TearDown]
    public void TearDown()
    {
        // Defense-in-depth for test independence (spec/TestPlan.md §2): every
        // feature-matrix test below mutates a View or Switch-specific
        // property on the shared SwitchViewModel. Unconditionally restoring
        // all of them here - regardless of which test ran or whether it
        // passed - guarantees no test can leak state into whichever test
        // NUnit runs next, in any order or filter. This also protects the
        // visual regression test from leftover Opacity/colors changing what
        // the baseline comparison sees.
        //
        // Deliberately NOT relying on SwitchPage.NavigateFromHome's
        // incidental re-navigation for this: it only re-navigates when the
        // Switch control leaves the accessibility tree (true for
        // IsVisible = false), not for Opacity = 0, where the element
        // typically remains present just fully transparent.
        var propertiesPage = _switchPage.OpenOptions();
        propertiesPage.SetOnColor("");
        propertiesPage.SetOffColor("");
        propertiesPage.SetThumbColor("");

        var viewPropertiesPage = propertiesPage.OpenViewProperties();
        viewPropertiesPage.SetOpacity(1.0);
        viewPropertiesPage.SetVisible(true);
        viewPropertiesPage.SetEnabled(true);
        viewPropertiesPage.SetInputTransparent(false);
        viewPropertiesPage.Apply();
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

    [Test]
    public void Toggle_DoesNotChangeState_WhenDisabled()
    {
        var propertiesPage = _switchPage.OpenOptions().OpenViewProperties();
        propertiesPage.SetEnabled(false);
        propertiesPage.Apply();

        // "Apply" pops back to the Switch control page — re-anchor the page object.
        _switchPage = new SwitchPage();

        // Arrange verification: confirm IsEnabled actually took effect before
        // asserting anything about the click behavior. Without this, a
        // broken SetEnabled(false) (e.g. a binding that silently no-ops)
        // would still make the assertion below pass — the test would prove
        // nothing about disabling and the bug would go undetected.
        Assert.That(_switchPage.IsEnabled, Is.False,
            "Arrange failed: the Switch should report IsEnabled = false before attempting to toggle it.");

        var initialState = _switchPage.IsToggled;

        // A disabled Switch may either ignore the click outright or have the
        // driver refuse to dispatch it (varies by platform/driver), so both
        // outcomes are accepted as proof the control didn't respond. Use
        // AttemptToggle (not Toggle) - Toggle waits for a flip that's
        // expected to never happen here and would just burn its full
        // timeout every run.
        try
        {
            _switchPage.AttemptToggle();
        }
        catch (InvalidElementStateException)
        {
            // Covers ElementNotInteractableException, which derives from this.
        }

        Assert.That(_switchPage.IsToggled, Is.EqualTo(initialState),
            "A disabled Switch must not change IsToggled in response to a click.");
    }

    [Test]
    public void Toggle_DoesNotChangeState_WhenInputTransparent()
    {
        var propertiesPage = _switchPage.OpenOptions().OpenViewProperties();
        propertiesPage.SetInputTransparent(true);
        propertiesPage.Apply();

        // "Apply" pops back to the Switch control page — re-anchor the page object.
        _switchPage = new SwitchPage();

        var initialState = _switchPage.IsToggled;

        // InputTransparent = true is a distinct case from IsEnabled = false:
        // the control still renders as enabled but ignores touch input, so a
        // click may still land without throwing (unlike disabled, which some
        // drivers refuse to dispatch to at all) - the only reliable check is
        // that the state genuinely didn't change.
        try
        {
            _switchPage.AttemptToggle();
        }
        catch (InvalidElementStateException)
        {
            // Covers ElementNotInteractableException, which derives from this.
        }

        Assert.That(_switchPage.IsToggled, Is.EqualTo(initialState),
            "An InputTransparent Switch must not change IsToggled in response to a click.");
    }

    [TestCase(true)]
    [TestCase(false)]
    public void SetIsToggled_ViaPropertiesPage_ReflectsOnControlPage(bool desiredState)
    {
        var propertiesPage = _switchPage.OpenOptions();
        propertiesPage.SetIsToggled(desiredState);

        // Arrange verification: confirm the Options page's own "Is Toggled"
        // switch actually reports the requested state before asserting it
        // propagated anywhere - otherwise a broken SetIsToggled would still
        // let this test pass if the control page's Switch happened to
        // already be in the desired state.
        Assert.That(propertiesPage.IsToggled, Is.EqualTo(desiredState),
            "Arrange failed: the Options page's Is Toggled switch did not accept the requested state.");

        _switchPage = propertiesPage.Apply();

        // Both switches are bound to the same SwitchViewModel.IsToggled
        // property, so setting it via the Options page's switch must be
        // reflected on the control-under-test's own Switch too.
        Assert.That(_switchPage.IsToggled, Is.EqualTo(desiredState),
            "Setting IsToggled via the Options page should be reflected on the Switch control page.");
    }

    // ── Feature matrix (combinatorial, data-driven) ─────────────────────

    [Category(UITestCategories.FeatureMatrix)]
    [TestCaseSource(typeof(OpacityData), nameof(OpacityData.Values))]
    public void Toggle_RespondsCorrectly_AtGivenOpacity(double opacity)
    {
        var propertiesPage = _switchPage.OpenOptions().OpenViewProperties();
        propertiesPage.SetOpacity(opacity);

        // Arrange verification: confirm the Opacity field actually accepted
        // the typed value before Apply - otherwise a silently-failed
        // SetOpacity (e.g. SendKeys not committing, or the value getting
        // parsed back to a default) would leave the control at its previous
        // opacity, and the toggle assertion below would still pass for the
        // wrong reason (it never really exercised `opacity`).
        Assert.That(double.Parse(propertiesPage.OpacityText, CultureInfo.InvariantCulture), Is.EqualTo(opacity).Within(0.0001),
            $"Arrange failed: Opacity entry did not accept {opacity} before Apply.");

        propertiesPage.Apply();

        // "Apply" pops back to the Switch control page — re-anchor the page object.
        _switchPage = new SwitchPage();

        var initialState = _switchPage.IsToggled;

        if (opacity <= 0.0)
        {
            // A fully transparent (Opacity = 0) control is not the same as a
            // hidden one: it stays in the accessibility tree (AutomationId
            // lookup still succeeds, unlike IsVisible = false), but at least
            // iOS's native hit-testing excludes views with ~zero alpha, so a
            // synthesized tap lands on nothing and the Switch doesn't toggle.
            // Verified against a real iOS simulator run - "still works" was
            // the wrong expectation for this value; assert "no effect"
            // instead, the same way as the disabled-Switch case.
            _switchPage.AttemptToggle();

            Assert.That(_switchPage.IsToggled, Is.EqualTo(initialState),
                "A fully transparent (Opacity = 0) Switch should not respond to a tap.");
            return;
        }

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
        {
            // A hidden control isn't interactable and can't be toggled -
            // assert that directly instead of a silent no-op return, so
            // this case actually verifies something (spec/TestPlan.md §5 R1).
            // This doubles as the arrange verification for this branch: if
            // SetVisible(false) had silently failed, the Switch would still
            // be displayed and this assertion would correctly fail.
            Assert.That(_switchPage.IsDisplayed, Is.False,
                "A Switch with IsVisible = false should not be present/interactable via AutomationId.");
            return;
        }

        // Arrange verification for the true case: confirm the Switch is
        // actually displayed (i.e. IsVisible = true really took effect)
        // before asserting Toggle() still works - otherwise this case would
        // be indistinguishable from a no-op SetVisible(true) that just left
        // whatever visibility state a previous run/step left behind.
        Assert.That(_switchPage.IsDisplayed, Is.True,
            "Arrange failed: the Switch should be displayed when IsVisible = true.");

        var initialState = _switchPage.IsToggled;
        _switchPage.Toggle();

        Assert.That(_switchPage.IsToggled, Is.Not.EqualTo(initialState));
    }

    // ── Switch-specific properties (visual regression) ──────────────────
    //
    // OnColor/OffColor/ThumbColor are purely visual - there's no reliable
    // cross-platform native attribute to read a Switch's actually-rendered
    // color back from, so a screenshot comparison against a baseline is the
    // only credible verification (spec/UITestArchitecture.md §22). Each test
    // still does an arrange verification on the entry text first (spec/
    // TestPlan.md §7.1), but that only proves the field accepted the typed
    // value, not that the color was applied - the baseline comparison is
    // what actually proves that.

    [Category(UITestCategories.VisualRegression)]
    [Test]
    public void SwitchControlPage_MatchesBaseline_WithCustomOnColor()
    {
        const string onColor = "#FF0000";

        var propertiesPage = _switchPage.OpenOptions();
        propertiesPage.SetOnColor(onColor);

        Assert.That(propertiesPage.OnColorText, Is.EqualTo(onColor),
            "Arrange failed: On Color entry did not accept the typed value before Apply.");

        _switchPage = propertiesPage.Apply();

        // OnColor is only visible while the Switch is toggled on.
        if (!_switchPage.IsToggled)
            _switchPage.Toggle();

        var result = CompareToBaseline(nameof(SwitchControlPage_MatchesBaseline_WithCustomOnColor));

        Assert.That(result.Matches, Is.True,
            $"Screenshot differs from baseline by {result.DiffPercentage:P2}. " +
            $"Diff image: {result.DiffImagePath ?? "(none, likely a dimension mismatch)"}. " +
            $"If this change is intentional, re-run with UPDATE_VISUAL_BASELINES=1 to accept it.");
    }

    [Category(UITestCategories.VisualRegression)]
    [Test]
    public void SwitchControlPage_MatchesBaseline_WithCustomOffColor()
    {
        const string offColor = "#0000FF";

        var propertiesPage = _switchPage.OpenOptions();
        propertiesPage.SetOffColor(offColor);

        Assert.That(propertiesPage.OffColorText, Is.EqualTo(offColor),
            "Arrange failed: Off Color entry did not accept the typed value before Apply.");

        _switchPage = propertiesPage.Apply();

        // OffColor is only visible while the Switch is toggled off.
        if (_switchPage.IsToggled)
            _switchPage.Toggle();

        var result = CompareToBaseline(nameof(SwitchControlPage_MatchesBaseline_WithCustomOffColor));

        Assert.That(result.Matches, Is.True,
            $"Screenshot differs from baseline by {result.DiffPercentage:P2}. " +
            $"Diff image: {result.DiffImagePath ?? "(none, likely a dimension mismatch)"}. " +
            $"If this change is intentional, re-run with UPDATE_VISUAL_BASELINES=1 to accept it.");
    }

    [Category(UITestCategories.VisualRegression)]
    [Test]
    public void SwitchControlPage_MatchesBaseline_WithCustomThumbColor()
    {
        const string thumbColor = "#00FF00";

        var propertiesPage = _switchPage.OpenOptions();
        propertiesPage.SetThumbColor(thumbColor);

        Assert.That(propertiesPage.ThumbColorText, Is.EqualTo(thumbColor),
            "Arrange failed: Thumb Color entry did not accept the typed value before Apply.");

        _switchPage = propertiesPage.Apply();

        // The thumb renders in both toggle states; pin to "off" so this
        // baseline isn't sensitive to whatever state a previous test left.
        if (_switchPage.IsToggled)
            _switchPage.Toggle();

        var result = CompareToBaseline(nameof(SwitchControlPage_MatchesBaseline_WithCustomThumbColor));

        Assert.That(result.Matches, Is.True,
            $"Screenshot differs from baseline by {result.DiffPercentage:P2}. " +
            $"Diff image: {result.DiffImagePath ?? "(none, likely a dimension mismatch)"}. " +
            $"If this change is intentional, re-run with UPDATE_VISUAL_BASELINES=1 to accept it.");
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
