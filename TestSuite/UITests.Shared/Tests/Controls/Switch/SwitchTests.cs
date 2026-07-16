using NUnit.Framework;
using UITests.Infrastructure;
using UITests.Pages.Controls.Switch;

namespace UITests.Tests.Controls.Switch;

/// <summary>
/// Functional tests for the Switch control itself (see
/// spec/UITestArchitecture.md §13 "Functional Tests"). Combinatorial
/// property-based tests live in <see cref="SwitchFeatureMatrix"/> instead.
/// </summary>
[TestFixture]
[Category(UITestCategories.Switch)]
public class SwitchTests
{
    SwitchPage _switchPage = null!;

    [SetUp]
    public void SetUp()
    {
        _switchPage = new SwitchPage();
    }

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
}
