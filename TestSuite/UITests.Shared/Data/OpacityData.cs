namespace UITests.Data;

/// <summary>
/// Reusable opacity values consumed by feature-matrix tests (spec
/// /UITestArchitecture.md §14). Add axes here as more controls need them,
/// rather than duplicating value sets per control.
/// <para>
/// <c>0.0</c> is not just "very transparent" — verified on a real iOS
/// simulator, at least iOS's native hit-testing excludes views with ~zero
/// alpha, so a synthesized tap lands on nothing and a control at
/// <c>Opacity = 0</c> won't respond to interaction, even though it remains
/// present in the accessibility tree (unlike <c>IsVisible = false</c>,
/// which removes it from the tree entirely). Any feature-matrix test
/// consuming this axis needs a special case for <c>0.0</c> that asserts "no
/// effect" rather than "behavior still works" — see
/// <c>Tests/Controls/Switch/SwitchFeatureMatrix.cs</c>
/// <c>Toggle_RespondsCorrectly_AtGivenOpacity</c> for the pattern, and
/// <c>spec/TestPlan.md</c> §7.2 for how this was found.
/// </para>
/// </summary>
public static class OpacityData
{
    public static readonly double[] Values = { 0.0, 0.25, 0.5, 0.75, 1.0 };
}
