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

        // Single-click reset (see SwitchViewModel.ResetToDefaults /
        // SwitchControlPage's "Reset" toolbar item) restores every View
        // property, Switch-specific property, and event/command counter to
        // its default before every test - regardless of what the previous
        // test mutated or whether it passed or failed. Deliberately placed
        // in [SetUp] rather than [TearDown]:
        //   - It's the Arrange step for every test in this fixture, so it
        //     belongs there conventionally.
        //   - It guarantees a clean baseline even if a previous run's
        //     [TearDown]-equivalent didn't execute (e.g. a hard crash),
        //     rather than relying on every prior test's cleanup succeeding.
        //   - It leaves a failed test's on-screen state intact until the
        //     next test starts, instead of a [TearDown] immediately
        //     overwriting it - useful when inspecting a failure.
        //
        // This replaced a previous [TearDown] that opened the Options and
        // View Properties pages and set ~13 fields back individually across
        // two navigations - inconsistent with the rest of this fixture's
        // "one thing, one Page Object call" style, slower (two page
        // transitions plus typing into every field on every single test),
        // and it didn't reset the cumulative ToggledEventCount/
        // CommandExecutionCount counters at all, which made any test
        // asserting an absolute (not delta) count fragile when run alone vs.
        // as part of the full fixture. See spec/TestPlan.md's
        // reset-architecture phase for the full rationale.
        _switchPage.Reset();
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

    // ── Toggled event / Command tests ───────────────────────────────────
    //
    // Microsoft.Maui.Controls.Switch has no Command/CommandParameter of its
    // own (unlike Button) - SwitchControlPage wires the native Toggled event
    // to SwitchViewModel.ToggledCommand manually (see spec/TestPlan.md's
    // Phase 7 entry). These tests verify all three observable pieces: the
    // event fired, the command actually executed (not just the event), and
    // the command received the expected parameter.

    [Test]
    public void Toggle_FiresToggledEvent_AndIncrementsEventCount()
    {
        var initialCount = _switchPage.ToggledEventCount;

        _switchPage.Toggle();

        Assert.That(_switchPage.ToggledEventCount, Is.EqualTo(initialCount + 1),
            "Toggling the Switch should fire exactly one Toggled event.");
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Toggle_ToggledEvent_ReportsTheNewValue(bool desiredState)
    {
        var propertiesPage = _switchPage.OpenOptions();
        propertiesPage.SetIsToggled(desiredState);
        _switchPage = propertiesPage.Apply();

        // Arrange verification: confirm the control actually landed in
        // desiredState before reading the event's reported value below -
        // otherwise a broken SetIsToggled could still make this assertion
        // pass by accident if the control happened to already be there.
        Assert.That(_switchPage.IsToggled, Is.EqualTo(desiredState),
            "Arrange failed: the Switch did not reach the requested state before this test's own toggle.");

        // Toggling from a known state means the resulting value is
        // deterministic, so the event's reported value can be asserted
        // directly instead of just checking it changed.
        _switchPage.Toggle();

        Assert.That(_switchPage.LastToggledValueText, Is.EqualTo((!desiredState).ToString()),
            $"The Toggled event's reported value should match the Switch's new IsToggled state ({!desiredState}).");
    }

    [Test]
    public void Toggle_ExecutesToggledCommand_AndIncrementsExecutionCount()
    {
        var initialEventCount = _switchPage.ToggledEventCount;
        var initialCommandCount = _switchPage.CommandExecutionCount;

        _switchPage.Toggle();

        // Asserting both counts together is what actually proves the
        // Command is wired up, not just the event: a broken/unwired Command
        // would still leave ToggledEventCount incrementing (the native event
        // always fires) while CommandExecutionCount stayed flat.
        Assert.That(_switchPage.ToggledEventCount, Is.EqualTo(initialEventCount + 1),
            "Arrange/control check: the Toggled event itself should still fire once.");
        Assert.That(_switchPage.CommandExecutionCount, Is.EqualTo(initialCommandCount + 1),
            "ToggledCommand should execute exactly once per Toggled event.");
    }

    [TestCase("SwitchTestParameter")]
    [TestCase("")]
    public void Toggle_ExecutesToggledCommand_WithExpectedParameter(string parameter)
    {
        var propertiesPage = _switchPage.OpenOptions();
        propertiesPage.SetCommandParameter(parameter);

        // Arrange verification: confirm the Command Parameter entry actually
        // accepted the typed value before Apply - otherwise a silently
        // failed SetCommandParameter would leave the previous parameter in
        // place and the assertion below could pass for the wrong reason.
        //
        // Skipped for an empty parameter: Android's accessibility tree
        // reports an Entry's Placeholder as its "text" whenever the actual
        // Text is empty (that's what a screen reader would announce), so
        // CommandParameterText can't be distinguished from the placeholder
        // via this property in that case - there's nothing meaningful left
        // to verify beyond what SetCommandParameter's Clear() already
        // guarantees.
        if (!string.IsNullOrEmpty(parameter))
        {
            Assert.That(propertiesPage.CommandParameterText, Is.EqualTo(parameter),
                $"Arrange failed: Command Parameter entry did not accept \"{parameter}\" before Apply.");
        }

        _switchPage = propertiesPage.Apply();

        _switchPage.Toggle();

        Assert.That(_switchPage.LastCommandParameterText, Is.EqualTo(parameter),
            $"ToggledCommand should have been invoked with the Command Parameter (\"{parameter}\") set on the Options page.");
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

    [Category(UITestCategories.FeatureMatrix)]
    [TestCaseSource(typeof(EnabledStateData), nameof(EnabledStateData.Values))]
    public void Toggle_RespondsCorrectly_AtGivenEnabledState(bool isEnabled)
    {
        var propertiesPage = _switchPage.OpenOptions().OpenViewProperties();
        propertiesPage.SetEnabled(isEnabled);
        propertiesPage.Apply();

        // "Apply" pops back to the Switch control page — re-anchor the page object.
        _switchPage = new SwitchPage();

        // Arrange verification: confirm IsEnabled actually took effect before
        // asserting anything about the click behavior (same rationale as
        // Toggle_DoesNotChangeState_WhenDisabled above).
        Assert.That(_switchPage.IsEnabled, Is.EqualTo(isEnabled),
            $"Arrange failed: the Switch should report IsEnabled = {isEnabled} before attempting to toggle it.");

        var initialState = _switchPage.IsToggled;

        if (!isEnabled)
        {
            // Same "no effect" shape as Toggle_DoesNotChangeState_WhenDisabled -
            // kept here too (data-driven) so the IsEnabled axis is covered by
            // the feature matrix like Opacity/Visibility, not only by the
            // single-case functional test.
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
            return;
        }

        _switchPage.Toggle();

        Assert.That(_switchPage.IsToggled, Is.Not.EqualTo(initialState));
    }

    [Category(UITestCategories.FeatureMatrix)]
    [TestCaseSource(typeof(FlowDirectionData), nameof(FlowDirectionData.Values))]
    public void Toggle_StillWorks_AtGivenFlowDirection(string flowDirection)
    {
        var propertiesPage = _switchPage.OpenOptions().OpenViewProperties();
        propertiesPage.SetFlowDirection(flowDirection);

        // Arrange verification: confirm the FlowDirection field actually
        // accepted the typed value before Apply - FlowDirection is purely
        // layout-affecting (LTR/RTL mirroring), so a broken SetFlowDirection
        // would still let the toggle assertion below pass "by accident"
        // (Toggle() doesn't care which direction the control is laid out in).
        Assert.That(propertiesPage.FlowDirectionText, Is.EqualTo(flowDirection),
            $"Arrange failed: FlowDirection entry did not accept {flowDirection} before Apply.");

        propertiesPage.Apply();

        // "Apply" pops back to the Switch control page — re-anchor the page object.
        _switchPage = new SwitchPage();

        var initialState = _switchPage.IsToggled;
        _switchPage.Toggle();

        Assert.That(_switchPage.IsToggled, Is.Not.EqualTo(initialState));
    }

    [Category(UITestCategories.FeatureMatrix)]
    [TestCaseSource(typeof(LayoutOptionsData), nameof(LayoutOptionsData.Values))]
    public void Toggle_StillWorks_AtGivenHorizontalOptions(string horizontalOptions)
    {
        var propertiesPage = _switchPage.OpenOptions().OpenViewProperties();
        propertiesPage.SetHorizontalOptions(horizontalOptions);

        // Arrange verification: HorizontalOptions is purely a layout/alignment
        // concern - Toggle() doesn't care where the control sits within its
        // parent, so a broken SetHorizontalOptions would still let the
        // behavioral assertion below pass "by accident" without this check.
        Assert.That(propertiesPage.HorizontalOptionsText, Is.EqualTo(horizontalOptions),
            $"Arrange failed: HorizontalOptions entry did not accept {horizontalOptions} before Apply.");

        propertiesPage.Apply();

        // "Apply" pops back to the Switch control page — re-anchor the page object.
        _switchPage = new SwitchPage();

        var initialState = _switchPage.IsToggled;
        _switchPage.Toggle();

        Assert.That(_switchPage.IsToggled, Is.Not.EqualTo(initialState));
    }

    [Category(UITestCategories.FeatureMatrix)]
    [TestCaseSource(typeof(LayoutOptionsData), nameof(LayoutOptionsData.Values))]
    public void Toggle_StillWorks_AtGivenVerticalOptions(string verticalOptions)
    {
        var propertiesPage = _switchPage.OpenOptions().OpenViewProperties();
        propertiesPage.SetVerticalOptions(verticalOptions);

        Assert.That(propertiesPage.VerticalOptionsText, Is.EqualTo(verticalOptions),
            $"Arrange failed: VerticalOptions entry did not accept {verticalOptions} before Apply.");

        propertiesPage.Apply();
        _switchPage = new SwitchPage();

        var initialState = _switchPage.IsToggled;
        _switchPage.Toggle();

        Assert.That(_switchPage.IsToggled, Is.Not.EqualTo(initialState));
    }

    [Category(UITestCategories.FeatureMatrix)]
    [TestCaseSource(typeof(BackgroundColorData), nameof(BackgroundColorData.Values))]
    public void Toggle_StillWorks_AtGivenBackground(string background)
    {
        var propertiesPage = _switchPage.OpenOptions().OpenViewProperties();
        propertiesPage.SetBackground(background);

        Assert.That(propertiesPage.BackgroundText, Is.EqualTo(background),
            $"Arrange failed: Background entry did not accept {background} before Apply.");

        propertiesPage.Apply();
        _switchPage = new SwitchPage();

        // Background is an appearance-only property behind the Switch - it
        // shouldn't affect interaction. Unlike Opacity, there's no "zero
        // alpha blocks hit-testing" analogue here, so a single behavioral
        // shape covers every value in the axis.
        var initialState = _switchPage.IsToggled;
        _switchPage.Toggle();

        Assert.That(_switchPage.IsToggled, Is.Not.EqualTo(initialState));
    }

    [Category(UITestCategories.FeatureMatrix)]
    [TestCaseSource(typeof(ZIndexData), nameof(ZIndexData.Values))]
    public void Toggle_StillWorks_AtGivenZIndex(int zIndex)
    {
        var propertiesPage = _switchPage.OpenOptions().OpenViewProperties();
        propertiesPage.SetZIndex(zIndex);

        Assert.That(int.Parse(propertiesPage.ZIndexText, CultureInfo.InvariantCulture), Is.EqualTo(zIndex),
            $"Arrange failed: ZIndex entry did not accept {zIndex} before Apply.");

        propertiesPage.Apply();
        _switchPage = new SwitchPage();

        // ZIndex only affects paint order among siblings; the Switch is the
        // sole child of its layout on this page, so it shouldn't affect
        // interactivity at any value in the axis (negative, zero, or
        // positive).
        var initialState = _switchPage.IsToggled;
        _switchPage.Toggle();

        Assert.That(_switchPage.IsToggled, Is.Not.EqualTo(initialState));
    }

    // §4.4 of spec/TestPlan.md: symmetry with the existing
    // Toggle_Twice_ReturnsToOriginalState functional test, crossed with the
    // Opacity/Visibility axes. Opacity = 0.0 is excluded (via .Skip(1) on
    // OpacityData.Values) since a fully transparent Switch doesn't respond
    // to a tap at all (see Toggle_RespondsCorrectly_AtGivenOpacity) - toggling
    // it "twice" would just be two no-ops, which isn't what this case is
    // meant to verify.
    [Category(UITestCategories.FeatureMatrix)]
    [TestCaseSource(typeof(OpacityData), nameof(OpacityData.Values))]
    public void Toggle_Twice_ReturnsToOriginalState_AtGivenOpacity(double opacity)
    {
        if (opacity <= 0.0)
            Assert.Ignore("Opacity = 0 blocks native hit-testing; covered separately by Toggle_RespondsCorrectly_AtGivenOpacity.");

        var propertiesPage = _switchPage.OpenOptions().OpenViewProperties();
        propertiesPage.SetOpacity(opacity);

        Assert.That(double.Parse(propertiesPage.OpacityText, CultureInfo.InvariantCulture), Is.EqualTo(opacity).Within(0.0001),
            $"Arrange failed: Opacity entry did not accept {opacity} before Apply.");

        propertiesPage.Apply();
        _switchPage = new SwitchPage();

        var initialState = _switchPage.IsToggled;

        _switchPage.Toggle();
        _switchPage.Toggle();

        Assert.That(_switchPage.IsToggled, Is.EqualTo(initialState));
    }

    [Category(UITestCategories.FeatureMatrix)]
    [Test]
    public void Toggle_Twice_ReturnsToOriginalState_WhenVisible()
    {
        // Only the isVisible = true case is meaningful here - a hidden
        // Switch can't be toggled at all (see Toggle_StillWorks_AtGivenVisibility),
        // so there's no "twice" variant to add for isVisible = false beyond
        // what that test already covers.
        var propertiesPage = _switchPage.OpenOptions().OpenViewProperties();
        propertiesPage.SetVisible(true);
        propertiesPage.Apply();
        _switchPage = new SwitchPage();

        Assert.That(_switchPage.IsDisplayed, Is.True,
            "Arrange failed: the Switch should be displayed when IsVisible = true.");

        var initialState = _switchPage.IsToggled;

        _switchPage.Toggle();
        _switchPage.Toggle();

        Assert.That(_switchPage.IsToggled, Is.EqualTo(initialState));
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
