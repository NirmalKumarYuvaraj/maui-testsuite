namespace UITests.Data;

/// <summary>
/// Reusable <c>ZIndex</c> values consumed by feature-matrix tests (spec
/// /UITestArchitecture.md §14). <c>ZIndex</c> only affects paint order among
/// sibling views, not hit-testing/interactivity for a control that's the
/// sole child of its layout (as every control-under-test page in this suite
/// is), so these values are chosen to exercise negative/zero/positive
/// without implying any behavior change is expected.
/// </summary>
public static class ZIndexData
{
    public static readonly int[] Values = { -1, 0, 1, 100 };
}
