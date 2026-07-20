namespace UITests.Data;

/// <summary>
/// Reusable hex color values consumed by feature-matrix tests (spec
/// /UITestArchitecture.md §14) for the <c>Background</c> axis. Values match
/// the <c>#RRGGBB</c> format accepted by
/// <c>TestSuite.Helper.PropertyTypeResolver.ToColor</c> (bound via
/// <c>Views/Base/BaseViewPropertiesPage.cs</c>'s Background entry).
/// </summary>
public static class BackgroundColorData
{
    public static readonly string[] Values = { "#FF0000", "#00FF00", "#0000FF", "linear", "radial" };
}
