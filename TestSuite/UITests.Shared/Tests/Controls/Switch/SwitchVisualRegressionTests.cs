using NUnit.Framework;
using UITests.Core;
using UITests.Infrastructure;
using UITests.Pages.Controls.Switch;

namespace UITests.Tests.Controls.Switch;

/// <summary>
/// Visual regression (screenshot diffing) coverage for the Switch control's
/// default appearance (see spec/UITestArchitecture.md §22). This is the
/// reference example every future control's visual regression test should
/// copy: derive from <see cref="BaseTest"/> for <c>CompareToBaseline</c>,
/// still drive the UI through the control's Page Object, and assert on
/// <see cref="ImageComparisonResult.Matches"/> with a message that surfaces
/// the diff percentage/path on failure.
/// </summary>
[TestFixture]
[Category(UITestCategories.Switch)]
[Category(UITestCategories.VisualRegression)]
public class SwitchVisualRegressionTests : BaseTest
{
    SwitchPage _switchPage = null!;

    [SetUp]
    public void SetUp()
    {
        _switchPage = new SwitchPage();

        // Pin to a known state so the baseline isn't sensitive to whatever
        // toggle state a previous test left the control in.
        if (_switchPage.IsToggled)
            _switchPage.Toggle();
    }

    [Test]
    public void SwitchControlPage_MatchesBaseline_WhenToggledOff()
    {
        var result = CompareToBaseline(nameof(SwitchControlPage_MatchesBaseline_WhenToggledOff));

        Assert.That(result.Matches, Is.True,
            $"Screenshot differs from baseline by {result.DiffPercentage:P2}. " +
            $"Diff image: {result.DiffImagePath ?? "(none, likely a dimension mismatch)"}. " +
            $"If this change is intentional, re-run with UPDATE_VISUAL_BASELINES=1 to accept it.");
    }
}
