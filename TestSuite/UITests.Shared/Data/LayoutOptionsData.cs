namespace UITests.Data;

/// <summary>
/// Reusable <c>LayoutOptions</c> values consumed by feature-matrix tests
/// (spec/UITestArchitecture.md §14). Values match the strings accepted by
/// <c>TestSuite.Helper.PropertyTypeResolver.ToLayoutOptions</c> (bound via
/// <c>Views/Base/BaseViewPropertiesPage.cs</c>'s Horizontal/Vertical Options
/// entries), not the <see cref="Microsoft.Maui.LayoutOptions"/> struct
/// directly. Shared by both the <c>HorizontalOptions</c> and
/// <c>VerticalOptions</c> axes since both entries accept the same set of
/// strings.
/// </summary>
public static class LayoutOptionsData
{
    public static readonly string[] Values = { "Start", "Center", "End", "Fill" };
}
