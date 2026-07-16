namespace UITests.Data;

/// <summary>
/// Reusable opacity values consumed by feature-matrix tests (spec
/// /UITestArchitecture.md §14). Add axes here as more controls need them,
/// rather than duplicating value sets per control.
/// </summary>
public static class OpacityData
{
    public static readonly double[] Values = { 0.0, 0.25, 0.5, 0.75, 1.0 };
}
