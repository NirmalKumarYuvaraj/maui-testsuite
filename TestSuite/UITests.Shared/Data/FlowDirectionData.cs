namespace UITests.Data;

/// <summary>
/// Reusable <c>FlowDirection</c> values consumed by feature-matrix tests
/// (spec/UITestArchitecture.md §14). Values match the strings accepted by
/// <c>TestSuite.Helper.PropertyTypeResolver.ToFlowDirection</c> (bound via
/// <c>Views/Base/BaseViewPropertiesPage.cs</c>'s Flow Direction entry), not
/// the <see cref="Microsoft.Maui.FlowDirection"/> enum members directly.
/// </summary>
public static class FlowDirectionData
{
    public static readonly string[] Values = { "LeftToRight", "RightToLeft", "MatchParent" };
}
