# .NET MAUI TestSuite — UI Test Architecture Specification (v2)

**Status:** Draft  
**Owner:** TestSuite maintainers  
**Applies to:** `TestSuite/` (`TestSuite.slnx`)

---

# 1. Purpose

`TestSuite` is a .NET MAUI control gallery application used to perform local
end-to-end UI verification through Appium across Android, iOS, macOS
(Catalyst), and Windows.

The project consists of:

- A MAUI host application containing the control gallery.
- A shared Appium/NUnit test project.
- Four platform-specific test bootstrap projects.

The current project structure works for a small number of controls, but as more
controls are added there is no defined convention for:

- page objects
- test organization
- automation IDs
- feature matrix tests
- reusable infrastructure

This document defines a repeatable architecture so that adding a new control is
primarily a mechanical task rather than a design exercise.

This specification intentionally focuses on **local development and
verification only**.

Continuous Integration, pipeline design, parallelization, and test sharding are
considered future work.

---

# 2. Goals

The architecture should provide:

- Consistent host application structure
- Consistent Page Object Model (POM)
- Reusable automation infrastructure
- Minimal duplicated code
- Easy addition of new controls
- Easy maintenance as the gallery grows
- Clear separation of responsibilities

---

# 3. Non Goals

This specification does **not** define:

- CI pipelines
- Parallel execution
- Test sharding
- Visual regression testing
- Screenshot diffing
- Cloud device execution

These may be introduced in future specifications without affecting the overall
architecture defined here.

---

# 4. Design Principles

The architecture follows several core principles.

## 4.1 Layered Responsibilities

Each layer owns exactly one responsibility.

| Layer          | Responsibility                           |
| -------------- | ---------------------------------------- |
| Host App       | Render controls and expose AutomationIds |
| Page Objects   | Interact with UI                         |
| Tests          | Verify behavior                          |
| Feature Matrix | Execute property combinations            |
| Infrastructure | Driver, waits, screenshots, logging      |
| Platform Heads | Platform-specific driver creation        |

No layer should perform work belonging to another layer.

Examples:

- Tests should not locate UI elements directly.
- Page Objects should not contain assertions.
- Infrastructure should not know about controls.

---

## 4.2 Behavior over Locators

Tests interact with **behavior**, never implementation details.

Good:

```csharp
switchPage.Toggle();

Assert.That(switchPage.IsToggled);
```

Bad:

```csharp
FindElement("Switch");
FindElement("Switch").Click();
```

Automation IDs remain private implementation details of page objects.

---

## 4.3 Single Source of Truth

Every AutomationId exists in one location.

The same constant should be referenced by:

- MAUI host page
- Page Object

Avoid duplicated string literals.

Example:

```csharp
public static class SwitchIds
{
    public const string Control = "Switch.Control";
    public const string Toggle = "Switch.Toggle";
}
```

---

## 4.4 Composition over Duplication

Common behaviors should exist only once.

Examples:

- Wait helpers
- Screenshot capture
- Navigation
- Shared property pages
- Base feature matrix

New controls should reuse existing components whenever possible.

---

# 5. Solution Architecture

```
Platform Test Heads

    Android
    iOS
    macOS
    Windows

            │

            ▼

UITests.Shared

    Core
    Infrastructure
    Pages
    Tests
    FeatureMatrix
    Data

            │

            ▼

TestSuite

    Core
    Views
    ViewModels
```

---

# 6. Host Application

The host application remains responsible for presenting controls.

Recommended structure:

```
Views/

    Button/

    Entry/

    Switch/

        SwitchNavPage

        SwitchControlPage

        SwitchPropertiesPage
```

Each control owns:

- Navigation page
- Control page
- Properties page

Every interactive element must expose an `AutomationId`.

---

## ViewModels

Each control has one ViewModel.

```
SwitchViewModel

    : BaseViewModel
```

Only properties unique to the control belong here.

Common View properties remain inside `BaseViewModel`.

---

## Shared Property Editors

Shared property editors remain centralized.

```
Views/Base

    BaseViewPropertiesPage

    LayoutPropertiesPage

    TransformPropertiesPage

    ShadowPropertiesPage

    ClipPropertiesPage
```

Control-specific property editors are placed alongside the control.

---

# 7. UITests.Shared

Recommended structure:

```
UITests.Shared

    Core

    Infrastructure

    Pages

        Base

        Controls

            Switch

            Button

            Entry

    Tests

        Smoke

        Controls

            Switch

            Button

    FeatureMatrix

    Data
```

---

# 8. Core Layer

The Core layer contains reusable testing functionality.

Suggested classes:

```
BaseTest

BasePage

WaitHelper

RetryHelper

ScreenshotManager
```

Responsibilities:

- Driver access
- Explicit waits
- Retry logic
- Screenshot capture
- Shared utilities

This layer must not contain control-specific logic.

---

# 9. Infrastructure Layer

Infrastructure contains testing support.

Example:

```
Infrastructure

    AppiumServerHelper

    AppiumSetupBase

    UITestCategories

    TestConfiguration
```

Responsibilities:

- Appium lifecycle
- Shared driver configuration
- Categories
- Logging
- Test configuration

---

# 10. Page Object Model

Every MAUI page has one matching Page Object.

```
Views/Switch/SwitchControlPage

↓

Pages/Controls/Switch/SwitchPage
```

The folder hierarchy mirrors the host application.

---

## Page Object Rules

Page Objects expose behavior.

Good:

```csharp
Toggle()

OpenOptions()

IsToggled
```

Bad:

```csharp
FindElement()

AppiumElement

Click(By.XPath(...))
```

Tests should never manipulate UI elements directly.

---

## BasePage

Every page derives from `BasePage`.

Responsibilities include:

- FindElement
- WaitForElement
- WaitForCondition
- Screenshot capture
- Navigation helpers

Shared functionality belongs here rather than being duplicated.

---

# 11. Shared Components

Not every reusable UI needs a full page object.

Shared UI may become reusable components.

Examples:

```
SearchBarComponent

ToolbarComponent

NavigationComponent
```

Page Objects compose these components instead of reimplementing them.

---

# 12. Navigation

Navigation should be abstracted.

Instead of:

```csharp
Search

Click

Wait
```

tests become:

```csharp
var page = CorePage.OpenControl<SwitchPage>();
```

The navigation implementation remains inside the page object.

---

# 13. Test Organization

Tests are grouped by control.

```
Tests

    Controls

        Switch

            SwitchTests

            SwitchFeatureMatrix

        Button

        Entry
```

One functional test file per control.

One feature matrix per control.

---

## Functional Tests

Functional tests verify the control's own behavior.

Example:

```
Toggle changes state

Toggle updates label

Toggle disabled

Toggle enabled
```

Each test should verify one behavior.

---

## Feature Matrix

Feature matrix tests verify combinations of common View properties.

Instead of duplicating property combinations in every control, common property
sets should be reusable.

Example shared matrices:

```
Opacity

Visibility

Rotation

Scale

Translation

Shadow

Clip
```

Each control consumes these shared property sets.

---

# 14. Test Data

Avoid hardcoded values.

Provide reusable data providers.

Example:

```
OpacityData

VisibilityData

RotationData

ScaleData
```

Feature matrices consume these datasets.

---

# 15. Waiting Strategy

Avoid:

```csharp
Thread.Sleep()

Task.Delay()
```

Use explicit waits.

Examples:

```
WaitUntilLoaded()

WaitForVisible()

WaitForEnabled()

WaitForText()

WaitForValue()
```

Explicit waits produce more reliable local tests and easier debugging.

---

# 16. Logging

Infrastructure should log significant actions automatically.

Examples:

```
Navigate -> Switch

Toggle Switch

Open Properties

Set Opacity

Return Home
```

Tests should remain readable without explicit logging statements.

---

# 17. Screenshots

Screenshots should be managed centrally.

Suggested helper:

```
ScreenshotManager
```

Capabilities:

- Capture
- CaptureOnFailure
- SaveToArtifacts

Individual tests should not implement screenshot logic.

---

# 18. Automation ID Convention

Automation IDs should follow a consistent naming pattern.

Recommended:

```
<Control>.<Element>

Switch.Control

Switch.Toggle

Switch.Label

Entry.Text

Button.Click
```

Do not locate elements by:

- visible text
- XPath
- hierarchy
- index

AutomationId is the only supported locator strategy.

---

# 19. Adding a New Control

## Host Application

- Create ViewModel
- Create Navigation Page
- Create Control Page
- Create Properties Page
- Register in CorePage navigation
- Assign AutomationIds

---

## UITests

- Create Page Object
- Create Properties Page Object
- Add test category
- Create functional tests
- Create feature matrix

---

## Verify

- Build successfully
- Run tests locally
- Verify all new tests pass

---

## 19.1 Reference Implementation (Switch)

The Switch control (Phase 4 of the migration) is the copyable, working
example for every item above. When adding `<Control>`, create the same set
of files, substituting `<Control>` for `Switch`:

**Host application** (`TestSuite/TestSuite/`)

- `AutomationIds/<Control>Ids.cs` — constants only, no logic. Naming:
  `<Control>.<Element>` (e.g. `Switch.Control`, `Switch.Options`,
  `Switch.Apply`, `Switch.NavigateToViewProperties`). Reuse
  `AutomationIds/BaseViewIds.cs` for the shared property editors instead of
  redefining `BaseView.*` constants per control.
  - Example: `AutomationIds/SwitchIds.cs`, `AutomationIds/BaseViewIds.cs`.
- Set `AutomationId` on every element a test needs to find, using the
  constants above — never inline string locators.
  - Example: `Views/Switch/SwitchControlPage.cs`,
    `Views/Switch/SwitchPropertiesPage.cs`,
    `Views/Base/BaseViewPropertiesPage.cs`.

**UITests.Shared**

- `Pages/Controls/<Control>/<Control>Page.cs` — page object for the control's
  gallery page (behavior methods like `Toggle()`, not raw `FindElement`
  calls in tests). Derives from `Core/BasePage.cs`.
  - Example: `Pages/Controls/Switch/SwitchPage.cs`.
- `Pages/Controls/<Control>/<Control>PropertiesPage.cs` — page object for the
  control's Options/Properties page; composes `Pages/Base/*` page objects for
  shared editors rather than duplicating them.
  - Example: `Pages/Controls/Switch/SwitchPropertiesPage.cs`,
    `Pages/Base/BaseViewPropertiesPage.cs`.
- `Data/<Property>Data.cs` — static value providers reused across controls
  (only add one if it doesn't already exist for that property).
  - Example: `Data/OpacityData.cs`, `Data/VisibilityData.cs`.
- `Infrastructure/UITestCategories.cs` — add a `<Control>` category constant.
- `Tests/Controls/<Control>/<Control>FeatureMatrix.cs` — a **single** test
  fixture holding all of the control's test coverage: functional tests
  (plain `[Test]`, tagged only with the fixture-level
  `[Category(UITestCategories.<Control>)]`), feature matrix tests
  (`[TestCaseSource]` driven, crossing `Data/*` providers with the
  control's behavior, additionally tagged `[Category(UITestCategories.FeatureMatrix)]`),
  and a visual regression test (additionally tagged
  `[Category(UITestCategories.VisualRegression)]`, using
  `CompareToBaseline` — see §22). Derive the fixture from `Core/BaseTest`
  to get `CompareToBaseline`. Don't split these into separate
  `*Tests.cs`/`*FeatureMatrix.cs`/`*VisualRegressionTests.cs` files — a
  control's test surface is small enough that doing so only adds
  navigation overhead, and per-test `[Category]` attributes already give
  independent `dotnet test --filter` scoping without a file split.
  - Example: `Tests/Controls/Switch/SwitchFeatureMatrix.cs`.

**Wiring, not code changes** — link the host app's `AutomationIds/` folder
into each platform test project the same way `UITests.Shared` is linked
(no `ProjectReference` between an app and a test project):

```xml
<Compile Include="..\TestSuite\AutomationIds\**\*.cs" LinkBase="AutomationIds" Visible="false" />
```

Already present in all 4 platform `.csproj` files — do **not** duplicate it
per control; it links the whole `AutomationIds/` folder once.

**Known limitation to check before reusing `Pages/Base/*`:** only wire up
AutomationIds/page objects for shared property editor fields that are
actually bound to a working ViewModel property in the host app. As of
Phase 4, `TransformPropertiesPage`, `LayoutAndSizePropertiesPage`,
`ClipPropertiesPage`, and `ShadowPropertiesPage` have unimplemented
(`// Needs to implement`) change handlers for most fields — adding
automation for those fields would test nothing. Extend `BaseViewIds.cs` and
`Pages/Base/BaseViewPropertiesPage.cs` (or add sibling page objects) only as
each field is actually wired up in the host app.

**Navigation gotcha:** if a control's Properties/Apply flow uses
`Navigation.PopToRootAsync()` (as Switch's does), it returns to the root of
the _current_ `NavigationPage`, not one level up. Shared page object
`Apply()` methods should return `void` rather than a strongly-typed "next
page", and calling test code should re-anchor via `new <Control>Page()`
afterward — see `SwitchFeatureMatrix.cs` for the pattern.

---

# 20. Migration Strategy

Migration should be incremental.

## Phase 1

Introduce the new folder structure.

No behavior changes.

---

## Phase 2

Move shared infrastructure.

- BaseTest
- Appium helpers
- Categories

---

## Phase 3

Introduce BasePage.

Move common element lookup and wait logic.

---

## Phase 4

Implement Switch as the reference control.

Complete:

- Page Objects
- Functional Tests
- Feature Matrix

---

## Phase 5

Apply the same architecture to every new control.

No further architectural changes should be required.

---

# 21. Future Work

The following topics are intentionally excluded from this specification and may
be introduced later:

- CI pipelines
- Parallel execution
- Test sharding
- Cloud device execution
- Performance benchmarking

Visual regression / screenshot comparison was in this list originally; it has
since been implemented — see §22.

The architecture defined here should support these additions without requiring
major restructuring.

---

# 22. Visual Regression (Screenshot Diffing)

Pixel-by-pixel screenshot comparison, layered on top of `ScreenshotManager`
(§17) rather than replacing it — `Capture`/`CaptureOnFailure` are unchanged
and remain the choice for ad hoc/failure screenshots that aren't compared
against anything.

## Components

- `Infrastructure/ScreenshotComparer.cs` — pure image comparison, no Appium
  dependency. `Compare(baselinePath, actualPath, diffImagePath?, threshold,
channelTolerance)` returns an `ImageComparisonResult` (`Matches`,
  `DiffPercentage`, `DiffImagePath`). Uses `SkiaSharp` for
  decoding/pixel access — MIT licensed (free for any use, no revenue-based
  fee), cross-platform, and the same rendering engine .NET MAUI itself is
  built on. **Not** `SixLabors.ImageSharp`, which requires a paid commercial
  license as a direct dependency in closed-source software once an
  organization exceeds $1M annual revenue.
  - Images of different dimensions are always a 100% mismatch (pixels can't
    be compared 1:1) — no diff image is produced in that case.
  - A small per-pixel `channelTolerance` (default 12/255 per RGBA channel)
    and a small overall `threshold` (default 0.1% of pixels) absorb harmless
    anti-aliasing/rendering noise without masking real differences.
  - When pixels differ, a diff image is written with mismatches in solid red
    and matching pixels dimmed, so differences are easy to spot visually.
  - `Crop(pngBytes, left, top, right, bottom)` — pixel-perfect crop (via
    `SKBitmap.ExtractSubset`, no resampling) used to strip OS chrome before
    comparison; a no-op when all insets are zero/negative.
- `Infrastructure/ScreenshotManager.CompareToBaseline(driver, name, threshold)`
  — captures a screenshot, then compares it to its baseline. Images are
  namespaced per platform (mirroring dotnet/maui's own `UITest.cs`
  `environmentName`/`VisualRegressionTester` convention), since screenshots
  naturally differ across platforms (fonts, chrome, DPI, etc.):
  `PlatformDirectoryName` is resolved at compile time from the `UITEST_IOS`
  / `UITEST_ANDROID` / `UITEST_MACOS` / `UITEST_WINDOWS` constants defined
  in each platform test project's `.csproj`.
  - `Screenshots/Baseline/{Platform}/{name}.png` — the accepted reference
    image for that platform (checked into source control).
  - `Screenshots/Actual/{Platform}/{name}.png` — what the current run
    captured.
  - `Screenshots/Diff/{Platform}/{name}.png` — only written when pixels
    differ.
  - If no baseline exists yet, throws instructing the caller to set
    `UPDATE_VISUAL_BASELINES=1` and re-run, rather than silently creating an
    unreviewed baseline.
  - With `UPDATE_VISUAL_BASELINES=1` set, the captured screenshot always
    becomes/overwrites the baseline and the comparison reports a match —
    use this once, after manually reviewing the new image, whenever a UI
    change is intentional.
- `BaseTest.CompareToBaseline` / `BasePage.CompareToBaseline` — thin
  convenience wrappers so tests/page objects don't reach into
  `ScreenshotManager` directly.

## Cropping OS chrome

Mirrors dotnet/maui's own `UITest.cs` crop defaults: before comparing (or
saving a new baseline), `CompareToBaseline` crops pixels off each edge to
remove OS chrome that isn't part of the app and changes between runs (clock,
Android ripple/nav-button flash, theme-dependent title bars):

| Platform | Top (status/title bar) | Bottom (nav/home indicator) |
| -------- | ----------------------- | ---------------------------- |
| Android  | 60px                     | 125px                        |
| iOS      | 110px                    | 40px                         |
| Windows  | 32px                     | 0px                          |
| macOS    | 29px                     | 0px                          |

These per-platform defaults are compiled in via the same `UITEST_*`
constants used for `PlatformDirectoryName` (see `DefaultCropInsets`) and
assume the same reference simulator/emulator sizes MAUI's UI tests target
— resize them (or override per-call) if this suite runs on different
devices. `CompareToBaseline(name, threshold, cropLeft, cropTop, cropRight,
cropBottom)` accepts nullable overrides: `null` (the default) uses the
platform default for that edge; pass `0` explicitly to disable cropping on
an edge instead. Cropping happens once, before the (possibly cropped)
screenshot is written to `Screenshots/Actual/...` — so a newly-created
baseline is already cropped consistently with future comparisons.

## Usage pattern

```csharp
[Test]
public void SwitchControlPage_MatchesBaseline_WhenToggledOff()
{
    var result = CompareToBaseline(nameof(SwitchControlPage_MatchesBaseline_WhenToggledOff));

    Assert.That(result.Matches, Is.True,
        $"Screenshot differs from baseline by {result.DiffPercentage:P2}. " +
        $"Diff image: {result.DiffImagePath}. " +
        "If intentional, re-run with UPDATE_VISUAL_BASELINES=1 to accept it.");
}
```

See `Tests/Controls/Switch/SwitchFeatureMatrix.cs` for the full
reference example — tag visual regression tests with both the owning
control's category and `UITestCategories.VisualRegression` so they can be
run/excluded as a group (e.g. `--filter "TestCategory=VisualRegression"`),
since they're inherently more prone to environment-specific flakiness
(fonts, DPI, theme) than functional tests.

## Known limitations

- No baseline images have been captured/committed in this environment (no
  Appium/emulator available to generate them) — the first real run against
  a device must be done with `UPDATE_VISUAL_BASELINES=1` to seed them, then
  the generated baseline reviewed and committed.
- Comparisons assume the baseline and actual screenshots come from the same
  platform/device/theme — no cross-platform or cross-resolution
  normalization is attempted. Baselines are kept per platform (e.g.
  `Screenshots/Baseline/Android/`, `Screenshots/Baseline/iOS/`) so each
  platform's reference images never collide with another's.

## Reference visual regression testing

# Visual Regression Testing in .NET MAUI UI Tests

## Overview

The visual regression testing system compares screenshots taken during UI tests against committed baseline images. Any pixel-level difference above a configurable threshold causes the test to fail and produces diff artifacts for inspection.

---

## Directory Structure

```
{testRootDirectory}/
  snapshots/                        ← committed baselines (source of truth)
    android/
    ios/
    ios-26/
    ios-iphonex/
    windows/
    mac/
  snapshots-diff/                   ← created at runtime, never committed
    android/
      MyTest.png                    ← failing actual screenshot
      MyTest-diff.png               ← red-highlighted pixel diff
```

In CI, `snapshots-diff/` is placed under:

```
$BUILD_ARTIFACTSTAGINGDIRECTORY/Controls.TestCases.Shared.Tests/snapshots-diff/
```

so it is available as a downloadable build artifact.

---

## Key Components

| Class                          | Location                                       | Responsibility                                           |
| ------------------------------ | ---------------------------------------------- | -------------------------------------------------------- |
| `VisualRegressionTester`       | `src/TestUtils/src/VisualTestUtils/`           | Orchestrates comparison, diff creation, and test failure |
| `MagickNetVisualComparer`      | `src/TestUtils/src/VisualTestUtils.MagickNet/` | Pixel-level image comparison via ImageMagick RMS metric  |
| `MagickNetVisualDiffGenerator` | `src/TestUtils/src/VisualTestUtils.MagickNet/` | Generates red-highlighted diff PNG using ImageMagick     |
| `ImageSnapshot`                | `src/TestUtils/src/VisualTestUtils/`           | Wraps image bytes + format; handles load/save            |
| `UITest`                       | `src/Controls/tests/TestCases.Shared.Tests/`   | Test base class; wires everything together               |

---

## Initialization

`UITest` creates a `VisualRegressionTester` in its constructor with:

```csharp
_visualRegressionTester = new VisualRegressionTester(
    testRootDirectory: projectRootDirectory,
    visualComparer: new MagickNetVisualComparer(),
    visualDiffGenerator: new MagickNetVisualDiffGenerator(),
    ciArtifactsDirectory: ciArtifactsDirectory);
```

- `MagickNetVisualComparer` uses the **RMS error metric** with a default threshold of **0.5%**.
- `ciArtifactsDirectory` is read from the `BUILD_ARTIFACTSTAGINGDIRECTORY` environment variable; `null` when running locally.

---

## Baseline Image Generation

There is no dedicated "generate" mode. The workflow is:

1. Run the test when no baseline exists.
2. The test **fails** and saves the actual screenshot to `snapshots-diff/` with a message like:
   ```
   Baseline snapshot not yet created: snapshots/android/MyTest.png
   cp snapshots-diff/android/MyTest.png snapshots/android/
   ```
3. Inspect the saved image and confirm it looks correct.
4. Copy it into the `snapshots/{environmentName}/` directory and commit it.

---

## Comparison Flow

```
actualImage (screenshot bytes)
    │
    ▼
baselineImagePath = snapshots/{env}/{name}.png
    │
    ├─ [NOT FOUND] ──► save actual to snapshots-diff/ ──► Fail (baseline missing)
    │
    └─ [FOUND] ──► MagickNetVisualComparer.Compare(baseline, actual)
                        │
                        ├─ Size mismatch? (width/height differ)
                        │       ──► save actual + generate diff ──► Fail
                        │
                        ├─ RMS pixel error > 0.5%?
                        │       ──► save actual + generate diff ──► Fail
                        │
                        └─ null (match) ──► delete stale diff files ──► Pass
```

### What `MagickNetVisualComparer.Compare` does

1. Loads both images as `MagickImage` objects.
2. Checks for size differences first (`ImageSizeDifference.Compare`).
3. Calculates RMS distortion across the Red channel:
   ```csharp
   double distortionDifference = magickBaselineImage.Compare(magickActualImage, ErrorMetric.RootMeanSquared, Channels.Red);
   if (distortionDifference > _differenceThreshold)
       return new ImagePercentageDifference(distortionDifference);
   return null;
   ```
4. Returns `null` on match, an `ImageDifference` subclass on failure.

### What `MagickNetVisualDiffGenerator.GenerateDiff` does

Uses ImageMagick's compare output to produce a PNG with differing pixels highlighted:

```csharp
using var magickDiffImage = (MagickImage)magickBaselineImage.Compare(magickActualImage, _errorMetric, Channels.Red, out _);
magickDiffImage.Format = MagickFormat.Png;
return new ImageSnapshot(magickDiffImage.ToByteArray(), ImageSnapshotFormat.PNG);
```

---

## Diff Folder Creation

The `snapshots-diff/` directory is **created on demand** only when a test fails — never pre-created. Each failure writes two files:

| File              | Content                                         |
| ----------------- | ----------------------------------------------- |
| `{name}.png`      | The actual screenshot that failed               |
| `{name}-diff.png` | Red-highlighted pixel diff against the baseline |

On a **passing** test, any leftover diff files from a previous run are actively deleted:

```csharp
this.DeleteFileIfExists(diffDirectoryImagePath);
this.DeleteFileIfExists(diffDirectoryDiffImagePath);
```

---

## Environment Name → Subdirectory Mapping

The environment name is determined at test time by reading device capabilities and is used as the subdirectory name under `snapshots/` and `snapshots-diff/`.

| Platform     | Default env name   | Condition                     |
| ------------ | ------------------ | ----------------------------- |
| Android      | `android`          | API 30, 1080×1920, 420 dpi    |
| Android      | `android-notch-36` | API 36, 1440×2960, 560 dpi    |
| iOS          | `ios`              | iPhone Xs, iOS 18.5 (default) |
| iOS          | `ios-26`           | iOS 26.x platform version     |
| iOS          | `ios-iphonex`      | iPhone X, iOS 16.4            |
| Windows      | `windows`          | —                             |
| Mac Catalyst | `mac`              | —                             |

Tests run on devices that don't match any known configuration do not fail by default; the environment name is left as an empty string and images land directly in `snapshots/`.

---

## Tolerance Override

Tests can pass a `tolerance` percentage to `VerifyScreenshot()` to allow small rendering differences:

```csharp
VerifyScreenshot("AnimatedElement", tolerance: 2.0); // allow up to 2% difference
```

- **Hard cap**: `tolerance > 15` throws `ArgumentException`.
- When within tolerance, the test passes and logs a warning:
  ```
  Visual difference 1.8% within tolerance 2% for 'AnimatedElement' on android
  ```

### Retry Support

`VerifyScreenshot` also supports retry for animations:

```csharp
VerifyScreenshot("AnimatedElement", retryTimeout: TimeSpan.FromSeconds(2));
```

This keeps retrying until the screenshot matches or the timeout expires.

---

## Local Workflow Commands

After a test failure, the following commands are printed to guide the developer:

```sh
# View the new snapshot
vview snapshots-diff/android/MyTest.png

# Accept it as the new baseline
cp snapshots-diff/android/MyTest.png snapshots/android/

# Diff all changed images at once
vdiff snapshots/ snapshots-diff/
```

More info: https://aka.ms/visual-test-workflow
