using NUnit.Framework;
using UITests.Data;
using UITests.Infrastructure;
using UITests.Pages.Controls.Switch;

namespace UITests.Tests.Controls.Switch;

/// <summary>
/// Combinatorial tests crossing the shared <c>BaseViewModel</c> surface
/// (opacity, visibility) with the Switch's own toggle behavior (see
/// spec/UITestArchitecture.md §13 "Feature Matrix"). Value sets come from
/// <c>Data/*</c> so they can be reused by future controls instead of being
/// redeclared per control.
/// </summary>
[TestFixture]
[Category(UITestCategories.Switch)]
[Category(UITestCategories.FeatureMatrix)]
public class SwitchFeatureMatrix
{
    SwitchPage _switchPage = null!;

    [SetUp]
    public void SetUp()
    {
        _switchPage = new SwitchPage();
    }

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
}

