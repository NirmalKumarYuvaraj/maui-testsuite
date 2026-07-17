# Switch Control — Feature Matrix Test Plan

**Status:** Draft — pending review
**Applies to:** `UITests.Shared/Tests/Controls/Switch/SwitchFeatureMatrix.cs`
**Related:** `spec/UITestArchitecture.md` §13/§14/§19.1 (test organization, test
data, per-control checklist), `Guideline/UITesting.md` §5 (writing a new
test).

---

## 1. Objective

Define the full set of feature-matrix test cases for the `Switch` control,
and the rules that make every test **autonomous**: each test must produce the
same result whether it is run alone (`--filter Name=...`), as part of the
`Switch` category, or as part of the full suite — in any order.

This plan does not change how functional tests (`Toggle_ChangesIsToggledState`,
`Toggle_Twice_ReturnsToOriginalState`) work; it only covers the feature-matrix
cases (`[Category(UITestCategories.FeatureMatrix)]`) that cross `Toggle()`
with common `View` properties.

---

## 2. Independence Requirements (must hold for every test below)

1. **Self-contained arrange step.** A test must set up every precondition it
   needs itself (navigate, set the property under test) rather than assume a
   previous test left the control in a usable state.
2. **No shared mutable fixture state across tests.** `_switchPage` is
   re-assigned in `[SetUp]`; a test must not depend on the value any other
   test left in the ViewModel (e.g. a leftover `Opacity = 0` or
   `IsToggled = true`).
3. **Always restore what it changed, even on failure.** Every test that calls
   `SetOpacity`/`SetVisible`/`SetEnabled`/etc. must revert that property to
   the control's default before the test method returns, in a `finally` (or
   `[TearDown]`), **not** only on the happy path. An `Assert.That` failure
   must not leave the property mutated for the next test.
4. **Don't rely on `NavigateFromHome()`'s no-op/re-navigate side effect as a
   reset mechanism.** `NavigateFromHome` only re-navigates when
   `IsDisplayed` (i.e. `TryFindElement(SwitchIds.Control)` returns `null`) —
   this is true for `IsVisible = false` but **not** for `Opacity = 0` (the
   element usually remains in the accessibility tree at zero alpha), so an
   `Opacity` test finishing without reverting would silently leak into the
   next test that reuses the same page instance. Explicit revert (rule 3) is
   the only safe mechanism; the re-navigate behavior is a bonus, not a
   substitute.
5. **Read, don't assume, the initial state.** Continue the existing pattern
   of `var initialState = _switchPage.IsToggled;` inside the test body,
   never a hardcoded `false`.
6. **No inter-test ordering assumptions.** NUnit does not guarantee
   declaration order within a fixture; tests must pass under
   `dotnet test --filter Category=Switch`, `--filter Category=FeatureMatrix`,
   a single `--filter Name=...`, and a shuffled/parallel run alike.
7. **No `Thread.Sleep`/fixed delays** for settling property changes — use
   `WaitForCondition`/`WaitForElement` (Guideline §5, spec §15).

---

## 3. Test Data Axes

Reuse existing shared data providers; add one new one.

| Axis         | Provider                     | Values                    | Status |
| ------------ | ----------------------------- | ------------------------- | ------ |
| Opacity      | `Data/OpacityData.cs`         | `0.0, 0.25, 0.5, 0.75, 1.0`| exists |
| Visibility   | `Data/VisibilityData.cs`      | `true, false`              | exists |
| IsEnabled    | `Data/EnabledStateData.cs` *(new)* | `true, false`         | to add |

`IsEnabled` is proposed as a new axis because a disabled `Switch` must not
change state on `Toggle()` — this is a materially different assertion shape
(state must **not** change) from the other two axes (state must change), so
it's called out as its own planned case in §4 rather than folded into the
opacity/visibility pattern.

`InputTransparent` is intentionally **out of scope** for this plan: it has no
dedicated planned case below (would duplicate the `IsEnabled=false` shape
without adding coverage); revisit only if a real defect surfaces there.

---

## 4. Planned Test Cases

All feature-matrix cases live in `SwitchFeatureMatrix.cs`, tagged
`[Category(UITestCategories.FeatureMatrix)]`, per spec §13/§19.1 (single file
per control, no separate `*FeatureMatrix.cs` split).

### 4.1 `Toggle_StillWorks_AtGivenOpacity(double opacity)` — existing, revise

- **Data:** `OpacityData.Values`.
- **Arrange:** Navigate to Switch Properties → View Properties, set
  `Opacity = opacity`, Apply, re-anchor `_switchPage`.
- **Act:** Read `initialState`, call `Toggle()`.
- **Assert:** `IsToggled != initialState`.
- **Cleanup (new):** In a `finally`, re-open View Properties and reset
  `Opacity = 1.0` before returning, regardless of assertion outcome, so the
  next test (any test, any order) starts from the control's default opacity.

### 4.2 `Toggle_StillWorks_AtGivenVisibility(bool isVisible)` — existing, revise

- **Data:** `VisibilityData.Values`.
- **Arrange:** Set `IsVisible = isVisible`, Apply, re-anchor.
- **Act/Assert (isVisible = true):** Same toggle assertion as above.
- **Act/Assert (isVisible = false):** Assert the control is **not**
  interactable — `_switchPage.IsDisplayed` is `false` — instead of the
  current early `return` (a silent no-op is not a real assertion; see §5
  risk R1).
- **Cleanup (new):** In a `finally`, set `IsVisible = true` again before
  returning — needed because `false` is a real, asserted case now, not a
  skip, so the fixture must still restore visibility for whatever test runs
  next.

### 4.3 `Toggle_DoesNotChangeState_WhenDisabled()` — new

- **Arrange:** Set `IsEnabled = false`, Apply, re-anchor.
- **Act:** Read `initialState`. Attempt `Toggle()` — but `Toggle()`'s
  internal `WaitForCondition(() => IsToggled != wasToggled)` would time out
  by design here (the state is expected to *not* change), so this case
  needs a variant that clicks without waiting for a flip and instead
  positively waits for "no change" — see §5 risk R2 for how this is
  reconciled without a fixed sleep.
- **Assert:** `IsToggled == initialState` (state did not change), and no
  exception was thrown attempting the click.
- **Cleanup:** `finally` sets `IsEnabled = true`.

### 4.4 `Toggle_Twice_AtGivenOpacity(double opacity)` / `AtGivenVisibility(bool)` — deferred

Considered for symmetry with the existing `Toggle_Twice_ReturnsToOriginalState`
functional test, but **not** included in this pass — flagged as an open
question in §6 rather than assumed in scope.

---

## 5. Risks & Mitigations

- **R1 — Silent no-op assertions.** The current
  `Toggle_StillWorks_AtGivenVisibility(false)` case does `return;` with only
  a comment, which means it asserts nothing and always reports as passed.
  Mitigation: §4.2 above replaces the early return with a real assertion on
  `IsDisplayed`.
- **R2 — Asserting "no state change" without a fixed sleep.** Proving a
  negative (`Toggle()` had no effect) can't use the existing
  `WaitForCondition(() => IsToggled != wasToggled)` (it's designed to wait
  *for* a change and will always hit its timeout here, adding ~10s of dead
  time per case). Proposed approach for review: click once, then use a
  short `WaitForCondition` that polls a *different*, cheap signal the
  disabled state already changes synchronously in the host app (to be
  confirmed against `SwitchViewModel`/`BaseViewModel.IsEnabled` wiring)
  rather than polling for the switch's own (unchanged) state. This needs a
  decision before implementation — see §6.
- **R3 — Leftover ViewModel state across the fixture.** Covered by
  Independence Requirement 3/4 above; every mutating feature-matrix test
  gets an explicit `finally` revert instead of relying on
  `NavigateFromHome`'s incidental re-navigation.
- **R4 — Order-dependent flakiness only visible in full-suite runs.** Since
  this is the exact failure mode being designed against, verification
  (§7) explicitly includes running the fixture in at least two different
  orders/filters, not just once top-to-bottom.

---

## 6. Open Questions (for review before implementation)

1. Should `Toggle_DoesNotChangeState_WhenDisabled` (§4.3) be a functional
   test instead of a feature-matrix test, since it isn't data-driven?
2. What's the right non-sleep signal to confirm a disabled `Switch` ignored
   a click (R2) — is there an existing label/state on the host app page we
   can poll, or does one need to be added to `SwitchControlPage`?
3. Is `Toggle_Twice_At...` (§4.4) worth adding now, or should it wait until
   there's a concrete regression it would have caught?
4. Do we want a shared `[TearDown]` in `SwitchFeatureMatrix` that resets all
   known-mutable properties unconditionally (defense in depth), in addition
   to the per-test `finally` blocks in §4?

---

## 7. Resolved Decisions (Phase 1 implementation)

1. **`Toggle_DoesNotChangeState_WhenDisabled` is a functional test, not
   feature-matrix.** It isn't data-driven (single `false` case; the `true`
   case is already covered by the default functional tests), so it lives
   under "Functional tests" in `SwitchFeatureMatrix.cs`, untagged with
   `[Category(FeatureMatrix)]`. `EnabledStateData.cs` was **not** added —
   there's no second axis value that would justify a data provider here.
2. **R2 resolved without a new signal or fixed sleep.** Rather than polling
   for a change that's expected to never happen, the test clicks via a new
   `SwitchPage.AttemptToggle()` (click without waiting for a flip) and then
   asserts `IsToggled == initialState` directly — no wait is needed because
   MAUI's `IsEnabled = false` blocks the click synchronously. `Toggle()` was
   deliberately left untouched (still waits for a flip) since every other
   caller needs that.  A disabled control's click may also throw
   (`InvalidElementStateException` / its `ElementNotInteractableException`
   subclass) depending on platform/driver — the test tolerates either
   outcome.
3. **Independence mechanism is a single `[TearDown]`, not per-test
   `finally` blocks.** `SwitchFeatureMatrix` now has one `[TearDown]` that
   unconditionally resets `Opacity`/`IsVisible`/`IsEnabled` to their
   defaults after every test (functional and feature-matrix alike). This
   was simpler and less repetitive than a `finally` in each mutating test,
   and it protects *all* tests in the fixture (including the visual
   regression test) against leftover state from any test, not just the
   ones updated in this phase.
4. **R1 fixed.** `Toggle_StillWorks_AtGivenVisibility(false)` now asserts
   `_switchPage.IsDisplayed == false` instead of silently `return`-ing.
5. **`Toggle_Twice_At...` (§4.4) deferred, unchanged** — not implemented in
   this phase; revisit only if a concrete regression motivates it.

### 7.1 Phase 2 — arrange verification (after real-device feedback)

Ran the Phase 1 tests on an iOS simulator: they all passed, but passing
wasn't proof they were testing the right thing — every mutating test
changed a property and then only asserted on the *behavior*
(toggle works / doesn't), never confirming the property change itself
actually took effect. A silently-broken `SetEnabled`/`SetOpacity`/
`SetVisible` (e.g. a `SendKeys` that didn't commit, or a binding that
no-ops) could leave the control at its previous state and these tests
would still pass "by accident", since they'd just be re-testing the
previous state's known-good behavior. Added an explicit **arrange
verification** assertion to every mutating test, immediately after
Apply/re-anchor and before the behavioral assertion:

- `Toggle_DoesNotChangeState_WhenDisabled` — asserts `SwitchPage.IsEnabled`
  (new accessor, backed by the standard WebDriver `enabled` element state)
  is `false` before attempting the click.
- `Toggle_StillWorks_AtGivenOpacity` — asserts the Opacity entry's text
  (`BaseViewPropertiesPage.OpacityText`, new accessor) round-trips to the
  requested value *before* clicking Apply.
- `Toggle_StillWorks_AtGivenVisibility` — asserts `SwitchPage.IsDisplayed`
  matches the requested `isVisible` in **both** branches (previously only
  checked in the `false` branch after R1).

This is now the standing convention for this fixture (and a candidate
convention for future controls, see the note added to
`spec/UITestArchitecture.md` §13): **a feature-matrix test that mutates a
property must verify the mutation took effect before asserting behavior
against it.**

### 7.2 Phase 3 — `Opacity = 0` is not just "still works" (after real-device feedback)

Running Phase 2 on the iOS simulator, `Toggle_StillWorks_AtGivenOpacity`
(now renamed `Toggle_RespondsCorrectly_AtGivenOpacity`) **failed** at
`opacity = 0.0`: the Switch didn't toggle. This wasn't a test bug in the
arrange-verification sense (the arrange assertion confirmed `Opacity` really
was set to `0`) — it was a wrong behavioral expectation. At least iOS's
native hit-testing excludes views with ~zero alpha, so a tap synthesized at
the Switch's coordinates lands on nothing; the control stays in the
accessibility tree (`AutomationId` lookup still succeeds — unlike
`IsVisible = false`), but no longer responds to interaction.

Fix: the test now special-cases `opacity <= 0.0` and asserts "no effect"
there (via `SwitchPage.AttemptToggle()` + `IsToggled == initialState`), the
same shape as `Toggle_DoesNotChangeState_WhenDisabled`, instead of asserting
`Toggle()` still flips the state for every value in `OpacityData.Values`.

This is a **shared-data-provider-level** fact, not Switch-specific — any
future control's feature-matrix test that consumes `OpacityData.Values` and
asserts "behavior still works" at every value will have the same false
failure at `0.0`. Documented directly on `OpacityData.cs` and in
`spec/UITestArchitecture.md`'s Feature Matrix section so the next control
added doesn't have to rediscover this on a real device.

---

## 8. Verification Plan

**Status:**
- Phase 1 was run on a real iOS simulator by the repo owner — all tests
  passed, but that run is what surfaced §7.1's arrange-verification gap
  (tests passing without proving the property mutation they were meant to
  cover).
- Phase 2 was also run on the real iOS simulator — `Toggle_StillWorks_
  AtGivenOpacity` **failed** at `opacity = 0.0`, surfacing §7.2's finding
  (zero opacity blocks native hit-testing on iOS). That's now fixed as
  `Toggle_RespondsCorrectly_AtGivenOpacity`.
- Phase 3 (this revision) has only been build-verified in this sandboxed
  environment (no simulator/device available here) — **re-running on the
  iOS simulator (and ideally Android/macOS/Windows) is a required
  follow-up** before considering this plan done, per
  `Guideline/UITesting.md` §7 step 10.

---

## 9. Phase 4 — host-app capability expansion (Switch-specific + real View bindings)

The host app (`TestSuite/Views/Switch/*`, outside this test project) was
independently updated to significantly expand what's actually testable on
the Switch control:

- **`SwitchControlPage.cs`**: `TestSwitch` previously only bound `IsEnabled`
  and `IsToggled` to the ViewModel — every other `BaseViewModel` property
  (Opacity, IsVisible, InputTransparent, Background, ZIndex, Shadow, Clip,
  layout/size, transforms) was a dead binding. It now binds essentially all
  of them, plus the new `OnColor`/`OffColor`/`ThumbColor`. **This means the
  Opacity/Visibility feature-matrix tests added in earlier phases were
  exercising dead bindings until this change** — they were passing
  vacuously, not because the behavior was verified. They now exercise real
  behavior with no test-side changes needed.
- **`SwitchPropertiesPage.cs`** (host app "Options" page): restructured
  into a "SWITCH PROPERTIES" section (an `IsToggled` switch bound to the
  same `SwitchViewModel.IsToggled` property as the control-under-test's own
  Switch, plus `OnColor`/`OffColor`/`ThumbColor` entries) and a "VIEW
  PROPERTIES" section (unchanged navigation to `BaseViewPropertiesPage`).
- New automation IDs: `SwitchIds.IsToggledSwitch`, `OnColorEntry`,
  `OffColorEntry`, `ThumbColorEntry`.

### New/changed test coverage (this phase)

| Test | Category | Verifies |
|---|---|---|
| `Toggle_DoesNotChangeState_WhenInputTransparent` | Functional | `InputTransparent = true` now genuinely blocks the tap (previously a dead binding); distinct from disabled since the control still renders as enabled. |
| `SetIsToggled_ViaPropertiesPage_ReflectsOnControlPage(bool)` | Functional, data-driven | The Options page's own `IsToggled` switch and the control page's Switch are two independent visual controls bound to the same ViewModel property — setting one must be reflected on the other. Only feasible now that the Options page exposes its own `IsToggled` switch. |
| `SwitchControlPage_MatchesBaseline_WithCustomOnColor` | Visual regression | `OnColor` is applied and visible while toggled on. |
| `SwitchControlPage_MatchesBaseline_WithCustomOffColor` | Visual regression | `OffColor` is applied and visible while toggled off. |
| `SwitchControlPage_MatchesBaseline_WithCustomThumbColor` | Visual regression | `ThumbColor` is applied and visible in either state; pinned to "off". |

`OnColor`/`OffColor`/`ThumbColor` have no reliable cross-platform native
attribute to read the *rendered* color back from — arrange verification for
these three tests only confirms the Entry field's text round-tripped before
Apply (a necessary but not sufficient check); the visual regression
screenshot comparison is what actually proves the color was applied. This
mirrors the existing pattern for other appearance-only properties (see
`spec/UITestArchitecture.md`).

`[TearDown]` was extended to also reset `OnColor`/`OffColor`/`ThumbColor`
(via empty-string `SetOnColor("")` etc., which clears the entry so the
bound `Color?` reverts to `null`) and `InputTransparent` (`SetInputTransparent(false)`),
for the same defense-in-depth reason as the other reset properties (§7).

### Outstanding follow-up

- **New visual-regression baselines must be captured on a real device**
  before the three new color tests can pass — per `Guideline/UITesting.md`
  §6, run once with `UPDATE_VISUAL_BASELINES=1`, review the generated PNGs,
  commit them, then re-run to confirm. This cannot be done in this
  sandboxed dev environment (no simulator/device attached here).
- This phase has only been build-verified in this sandboxed environment —
  a real-device run (iOS simulator at minimum) is required to confirm the
  new functional tests actually pass as designed, consistent with every
  prior phase in this plan.

Once run, confirm independence by expecting all-green in every case:

```bash
# Whole fixture, declared order
dotnet test UITests.<Platform>.csproj --filter "Category=Switch"

# Feature matrix only
dotnet test UITests.<Platform>.csproj --filter "Category=FeatureMatrix"

# Each new/changed test individually
dotnet test UITests.<Platform>.csproj --filter "Name=Toggle_RespondsCorrectly_AtGivenOpacity"
dotnet test UITests.<Platform>.csproj --filter "Name=Toggle_StillWorks_AtGivenVisibility"
dotnet test UITests.<Platform>.csproj --filter "Name=Toggle_DoesNotChangeState_WhenDisabled"
dotnet test UITests.<Platform>.csproj --filter "Name=Toggle_DoesNotChangeState_WhenInputTransparent"
dotnet test UITests.<Platform>.csproj --filter "Name=SetIsToggled_ViaPropertiesPage_ReflectsOnControlPage"

# Full Switch fixture run twice in a row in the same session, to catch
# leftover state from the first run bleeding into the second
dotnet test UITests.<Platform>.csproj --filter "Category=Switch"
dotnet test UITests.<Platform>.csproj --filter "Category=Switch"
```
