namespace UITests.Data;

/// <summary>
/// Reusable <c>IsEnabled</c> values consumed by feature-matrix tests (spec
/// /UITestArchitecture.md §14). Unlike <see cref="OpacityData"/> and
/// <see cref="VisibilityData"/>, both values here assert a genuinely
/// different behavior shape (enabled: state changes on interaction;
/// disabled: state must not change) rather than the same assertion at
/// different points on an axis — see
/// <c>Tests/Controls/Switch/SwitchFeatureMatrix.cs</c>
/// <c>Toggle_RespondsCorrectly_AtGivenEnabledState</c> for how both shapes
/// are reconciled in a single data-driven test.
/// </summary>
public static class EnabledStateData
{
    public static readonly bool[] Values = { true, false };
}
