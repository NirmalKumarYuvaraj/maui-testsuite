namespace TestSuite.AutomationIds;

/// <summary>
/// Single source of truth for the <c>AutomationId</c>s used on
/// <c>TestSuite.Core.CorePage</c> — the app's landing/home page listing every
/// control, layout, page, and shell sample. Linked into the UI test projects
/// the same way as <see cref="SwitchIds"/> (see spec/UITestArchitecture.md
/// §4.3/§18).
/// </summary>
public static class CoreIds
{
    public const string SearchEntry = "Home.SearchEntry";
    public const string SearchButton = "Home.SearchButton";
    public const string ControlsList = "Home.ControlsList";
}
