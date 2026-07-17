using System.Globalization;
using TestSuite.AutomationIds;
using UITests.Core;

namespace UITests.Pages.Base;

/// <summary>
/// Page Object for the shared <c>Views/Base/BaseViewPropertiesPage.cs</c>
/// editor, reused by every control's Properties flow (spec
/// /UITestArchitecture.md §4.2/§10). Every field on that page has an
/// AutomationId and a typed accessor here.
/// </summary>
public class BaseViewPropertiesPage : BasePage
{
    // ── Layout & size ────────────────────────────────────────────────────
    // No Page Object exists yet for LayoutAndSizePropertiesPage (out of
    // scope for this change) — returns void; navigate there directly if a
    // future test needs it, once that page gets its own Page Object.
    public void OpenLayoutAndSize() => FindElement(BaseViewIds.LayoutAndSizeButton).Click();

    // ── Alignment ────────────────────────────────────────────────────────
    public void SetHorizontalOptions(string value) => SetEntry(BaseViewIds.HorizontalOptionsEntry, value);

    public void SetVerticalOptions(string value) => SetEntry(BaseViewIds.VerticalOptionsEntry, value);

    public void SetFlowDirection(string value) => SetEntry(BaseViewIds.FlowDirectionEntry, value);

    // ── Appearance ───────────────────────────────────────────────────────
    public void SetOpacity(double opacity) => SetEntry(BaseViewIds.OpacityEntry, opacity.ToString(CultureInfo.InvariantCulture));

    public void SetBackground(string value) => SetEntry(BaseViewIds.BackgroundEntry, value);

    // ── Behavior ─────────────────────────────────────────────────────────
    public void SetEnabled(bool isEnabled) => SetSwitch(BaseViewIds.IsEnabledSwitch, isEnabled);

    public void SetInputTransparent(bool inputTransparent) => SetSwitch(BaseViewIds.InputTransparentSwitch, inputTransparent);

    public void SetVisible(bool isVisible) => SetSwitch(BaseViewIds.IsVisibleSwitch, isVisible);

    // ── Advanced ─────────────────────────────────────────────────────────
    public void SetZIndex(int zIndex) => SetEntry(BaseViewIds.ZIndexEntry, zIndex.ToString(CultureInfo.InvariantCulture));

    public void OpenShadowOptions() => FindElement(BaseViewIds.ShadowOptionsButton).Click();

    public void OpenClipOptions() => FindElement(BaseViewIds.ClipOptionsButton).Click();

    public void OpenMoreOptions() => FindElement(BaseViewIds.MoreOptionsButton).Click();

    // ── Read-only info ───────────────────────────────────────────────────
    public string IsFocusedText => WaitForElement(BaseViewIds.IsFocusedLabel).Text;

    public string DesiredSizeText => WaitForElement(BaseViewIds.DesiredSizeLabel).Text;

    public string FrameText => WaitForElement(BaseViewIds.FrameLabel).Text;

    /// <summary>
    /// Current text in the Opacity entry. Exposed so callers can verify a
    /// <see cref="SetOpacity"/> call actually took (i.e. the field accepted
    /// the typed value) <em>before</em> hitting Apply and asserting on
    /// downstream behavior — asserting on behavior alone can't distinguish
    /// "the property really changed" from "the arrange step silently did
    /// nothing and the behavior would've held anyway".
    /// </summary>
    public string OpacityText => WaitForElement(BaseViewIds.OpacityEntry).Text;

    /// <summary>
    /// Applies changes. The host app's "Apply" toolbar item calls
    /// <c>Navigation.PopToRootAsync()</c>, which returns to whichever
    /// control's <c>ControlPage</c> is the root of the current
    /// <c>NavigationPage</c> — this shared page object doesn't know which
    /// control that is, so callers should re-anchor via that control's own
    /// Page Object afterwards (e.g. <c>new SwitchPage()</c>).
    /// </summary>
    public void Apply()
        => FindElement(BaseViewIds.ApplyToolbarItem).Click();

    void SetEntry(string automationId, string value)
    {
        var entry = WaitForElement(automationId);
        entry.Clear();
        entry.SendKeys(value);
    }

    void SetSwitch(string automationId, bool desiredState)
    {
        if (IsToggleOn(automationId) != desiredState)
            FindElement(automationId).Click();
    }
}
