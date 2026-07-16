# UI Testing Guideline — Writing, Deploying & Running Tests

This is a practical, step-by-step companion to `TestSuite/spec/UITestArchitecture.md`
(the source of truth for *why* the architecture looks the way it does). This
document focuses on *how* to actually write a new test and get it running
against a real app build on each platform.

If anything here conflicts with the spec, the spec wins — file/update this
doc instead of drifting.

---

## 1. Solution Layout (at a glance)

```
TestSuite/
  TestSuite/                 ← the .NET MAUI host app under test
    AutomationIds/            ← shared AutomationId constants (linked, not referenced)
    Views/ ViewModels/ Core/  ← host app pages/viewmodels
  UITests.Shared/             ← all cross-platform test code (linked into every platform project)
    Core/                     ← BaseTest, BasePage, WaitHelper, RetryHelper
    Infrastructure/           ← Appium lifecycle, ScreenshotManager/Comparer, categories
    Pages/                    ← Page Objects (Base/ + Controls/<Control>/)
    Tests/                    ← test fixtures (Smoke/ + Controls/<Control>/)
    Data/                     ← reusable data providers for feature-matrix tests
  UITests.iOS/                ← platform test head: AppiumSetup.cs + csproj only
  UITests.Android/
  UITests.macOS/
  UITests.Windows/
  Guideline/                  ← you are here
  spec/                       ← architecture spec (read this for "why")
```

Each `UITests.<Platform>` project contains **no test logic of its own** — it
only has an `AppiumSetup.cs` (driver/session bootstrap) and a `.csproj` that
links in `UITests.Shared/**/*.cs` and the host app's `AutomationIds/**/*.cs`
via `<Compile Include="..." LinkBase="..." />`, e.g.:

```xml
<Compile Include="..\UITests.Shared\**\*.cs" LinkBase="Shared" Visible="false" />
<Compile Include="..\TestSuite\AutomationIds\**\*.cs" LinkBase="AutomationIds" Visible="false" />
```

This means: **write the test once in `UITests.Shared`, it runs on all four
platforms** — you never write iOS-only or Android-only test code unless the
behavior genuinely diverges (rare; put it behind `#if UITEST_IOS` etc. if it
ever happens — see §6 below for the platform constants).

---

## 2. Prerequisites (one-time machine setup)

- .NET SDK with the `maui` workload (`dotnet workload install maui`).
- [Appium](https://appium.io/) 2.x with the platform driver installed:
  - iOS/macOS: `appium driver install xcuitest` / `mac2`
  - Android: `appium driver install uiautomator2`
  - Windows: `appium driver install windows`
- Platform SDKs/tooling as normal for MAUI development (Xcode + simulators
  for iOS/macOS, Android SDK + an emulator image, Windows App SDK for
  Windows).
- You do **not** need to start Appium manually — `AppiumSetupBase.StartServer()`
  starts a local server automatically per test run (`127.0.0.1:4723`) unless
  one is already running, in which case it's reused.

---

## 3. Deploying the app under test

The UI tests drive an **already-installed/running** app via Appium — they
don't build+deploy it for you. Deploy the host app to your target
simulator/emulator/device first, then point the platform's `AppiumSetup.cs`
at it.

All build output for every project (host app + all test projects) is
consolidated under `TestSuite/artifacts/` (see `Directory.Build.props` →
`UseArtifactsOutput`), instead of per-project `bin`/`obj` folders.

### iOS (simulator)

```bash
cd TestSuite
dotnet build TestSuite/TestSuite.csproj -f net10.0-ios -t:Run \
  -p:_DeviceName="iPhone Xs" -p:_RuntimeIdentifier=iossimulator-arm64
```
This builds, installs, and launches the app on the named simulator in one
step. `UITests.iOS/AppiumSetup.cs` already targets `DeviceName = "iPhone Xs"`,
`PlatformVersion = "18.5"`, `App = "com.companyname.testsuite"` (the bundle
id, since the app is already installed) — adjust these to match whatever
simulator you actually deployed to.

### Android (emulator)

```bash
cd TestSuite
dotnet build TestSuite/TestSuite.csproj -f net10.0-android -t:Run
```
Requires a booted emulator (or `androidOptions.AddAdditionalAppiumOption("avd", "<name>")`
in `UITests.Android/AppiumSetup.cs` to have Appium boot one for you).
`UITests.Android/AppiumSetup.cs` is pre-configured for **debug builds**
(`NoReset=true`, explicit `AppPackage`/`AppActivity`) since Appium is
incompatible with Android Fast Deployment. For release builds, build the
`.apk` and set `App = <path-to-apk>` instead (see the commented
"RELEASE BUILD SETUP" block in that file).

### macOS (Mac Catalyst)

```bash
cd TestSuite
dotnet build TestSuite/TestSuite.csproj -f net10.0-maccatalyst -t:Run
```
Then update `UITests.macOS/AppiumSetup.cs`'s `App` path to the built
`.app` bundle (default output path is commented in that file) — the bundle
ID capability is also required or Appium automates Finder instead of the app.

### Windows

```powershell
cd TestSuite
dotnet build TestSuite/TestSuite.csproj -f net10.0-windows10.0.19041.0 -t:Run
```
Then find the deployed package's App User Model ID with
`Get-AppxPackage -Name "*testsuite*"` in PowerShell and update
`UITests.Windows/AppiumSetup.cs`'s `App` capability to match
(`<PackageFamilyName>!App` format).

---

## 4. Running the tests

Once the app is deployed and `AppiumSetup.cs` capabilities match it, run the
platform's test project like any other .NET test project:

```bash
cd TestSuite
dotnet test UITests.iOS/UITests.iOS.csproj
dotnet test UITests.Android/UITests.Android.csproj
dotnet test UITests.macOS/UITests.macOS.csproj
dotnet test UITests.Windows/UITests.Windows.csproj
```

Useful filters (NUnit `--filter`, backed by `[Category]`/test name):

```bash
# Only this control's tests
dotnet test UITests.iOS/UITests.iOS.csproj --filter "Category=Switch"

# Only smoke tests
dotnet test UITests.iOS/UITests.iOS.csproj --filter "Category=Smoke"

# Skip the flakier visual-regression tests
dotnet test UITests.iOS/UITests.iOS.csproj --filter "Category!=VisualRegression"

# A single test by name
dotnet test UITests.iOS/UITests.iOS.csproj --filter "Name=Toggle_ChangesIsToggledState"
```

`AppiumSetup` is an NUnit `[SetUpFixture]`, so the driver/session is created
**once per test run** (`[OneTimeSetUp]`) and reused across every fixture,
then torn down at the end (`[OneTimeTearDown]`) — you don't need to manage
the driver lifecycle in your own tests.

---

## 5. Writing a new test for an existing control

If a Page Object already exists for the control (check
`UITests.Shared/Pages/Controls/<Control>/`), just add a test method to that
control's fixture in `UITests.Shared/Tests/Controls/<Control>/<Control>FeatureMatrix.cs`:

```csharp
[Test]
public void Toggle_Twice_ReturnsToOriginalState()
{
    var initialState = _switchPage.IsToggled;

    _switchPage.Toggle();
    _switchPage.Toggle();

    Assert.That(_switchPage.IsToggled, Is.EqualTo(initialState));
}
```

Rules of thumb (see spec §10/§13):
- **Never call `FindElement`/Appium APIs directly from a test.** Add a
  behavior method (e.g. `Toggle()`, `IsToggled`) to the Page Object instead,
  and call that from the test.
- **Never locate elements by text/XPath/index.** `AutomationId` is the only
  supported locator (spec §18) — see §7 below if the element doesn't have
  one yet.
- **Never `Thread.Sleep`/`Task.Delay`.** Use `WaitForCondition(...)` (from
  `BaseTest`/`BasePage`) or a dedicated `WaitFor*` method instead (spec §15).
- One fixture per control, one behavior per test method. Data-driven
  combinations (crossing a property like Opacity/Visibility with the
  control's behavior) go under `[Category(UITestCategories.FeatureMatrix)]`
  in the same fixture — don't create separate `*Tests.cs` files (spec §13/§19.1).

---

## 6. Writing a visual regression test

See `spec/UITestArchitecture.md` §22 for full details; short version:

```csharp
[Category(UITestCategories.VisualRegression)]
[Test]
public void SwitchControlPage_MatchesBaseline_WhenToggledOff()
{
    if (_switchPage.IsToggled)
        _switchPage.Toggle();

    var result = CompareToBaseline(nameof(SwitchControlPage_MatchesBaseline_WhenToggledOff));

    Assert.That(result.Matches, Is.True,
        $"Screenshot differs from baseline by {result.DiffPercentage:P2}. " +
        $"Diff image: {result.DiffImagePath}. " +
        "If intentional, re-run with UPDATE_VISUAL_BASELINES=1 to accept it.");
}
```

Baselines are namespaced **per platform** (compiled in via `UITEST_IOS` /
`UITEST_ANDROID` / `UITEST_MACOS` / `UITEST_WINDOWS`, one constant defined in
each platform `.csproj`) and live in source control under each platform
project, **not** under `UITests.Shared`:

```
UITests.iOS/Screenshots/Baseline/iOS/{name}.png
UITests.Android/Screenshots/Baseline/Android/{name}.png
UITests.macOS/Screenshots/Baseline/macOS/{name}.png
UITests.Windows/Screenshots/Baseline/Windows/{name}.png
```

Each platform `.csproj` copies that folder to the output directory at build
time (`<Content Include="Screenshots\Baseline\**" CopyToOutputDirectory="Always" />`),
since `ScreenshotManager` resolves paths relative to the test run's working
directory (the build output dir, under `TestSuite/artifacts/`, which is
gitignored).

### Creating/updating a baseline

1. Write the test using `CompareToBaseline(name)` as above — don't create
   the baseline file by hand.
2. Run it once with the baseline env var set:
   ```bash
   UPDATE_VISUAL_BASELINES=1 dotnet test UITests.iOS/UITests.iOS.csproj \
     --filter "Name=SwitchControlPage_MatchesBaseline_WhenToggledOff"
   ```
   This always passes and writes the captured screenshot as the new baseline
   at `artifacts/bin/UITests.iOS/debug/Screenshots/Baseline/iOS/{name}.png`.
3. **Copy the generated PNG back into source control** (build output isn't
   tracked by git):
   ```bash
   cp artifacts/bin/UITests.iOS/debug/Screenshots/Baseline/iOS/{name}.png \
      UITests.iOS/Screenshots/Baseline/iOS/
   ```
4. Visually review the image, then re-run the test **without** the env var
   to confirm it now compares cleanly, and commit the PNG.
5. Repeat per platform if the control's baseline needs to be captured on
   more than one platform.

### Cropping OS chrome

`CompareToBaseline` crops OS chrome (status bar/nav bar/title bar) before
comparing, using per-platform pixel defaults (Android: 60px top/125px
bottom, iOS: 110px top/40px bottom, Windows: 32px top, macOS: 29px top).
Override per call if needed:

```csharp
CompareToBaseline(name, cropTop: 0); // disable top cropping for this test
```

`null` (the default for each crop parameter) uses the platform default;
pass `0` explicitly to disable cropping on that edge.

---

## 7. Adding a brand-new control (end-to-end checklist)

Full reference: spec §19/§19.1. Use the Switch control's files as the
copy-paste template for every step below (substitute `<Control>`):

**Host app** (`TestSuite/TestSuite/`)
1. `AutomationIds/<Control>Ids.cs` — constants only (`<Control>.<Element>`
   naming, e.g. `Switch.Control`). Reuse `AutomationIds/BaseViewIds.cs` for
   shared property editors instead of redefining them.
2. Create the ViewModel (`<Control>ViewModel : BaseViewModel`, only
   control-specific properties).
3. Create the Navigation/Control/Properties pages under `Views/<Control>/`,
   setting `AutomationId` on every element a test needs to find.
4. Register the new control page in `CorePage` navigation.

**UITests.Shared**
5. `Pages/Controls/<Control>/<Control>Page.cs` — page object exposing
   behavior methods (derives from `Core/BasePage`).
6. `Pages/Controls/<Control>/<Control>PropertiesPage.cs` — composes
   `Pages/Base/*` shared editors rather than duplicating them.
7. Add a `<Control>` constant to `Infrastructure/UITestCategories.cs`.
8. `Tests/Controls/<Control>/<Control>FeatureMatrix.cs` — one fixture with
   functional tests, feature-matrix tests (`[Category(FeatureMatrix)]`), and
   a visual regression test (`[Category(VisualRegression)]`), tagged at the
   fixture level with `[Category(UITestCategories.<Control>)]`.

**No wiring changes needed** — the `<Compile Include="..\TestSuite\AutomationIds\**\*.cs" ...>`
link in each platform `.csproj` already picks up new `AutomationIds/*.cs`
files automatically.

**Verify**
9. Build the solution (`dotnet build TestSuite.slnx`).
10. Deploy the updated host app (§3) and run the new tests locally on at
    least one platform (§4) before committing.

---

## 8. Troubleshooting

- **`No baseline image found at 'Screenshots/Baseline/<Platform>/{name}.png'`**
  — expected the first time a visual regression test runs; follow §6
  "Creating/updating a baseline" above.
- **`Could not start REST http interface listener... EADDRINUSE 127.0.0.1:4723`**
  — a local Appium server from a previous run is already listening; harmless,
  the driver reuses it. Kill the stray process only if you need a clean
  server for some other reason.
- **iOS/Android app not found by Appium** — the app must already be
  installed/running (§3) before `dotnet test` starts; `AppiumSetup.cs`'s
  `App`/`AppPackage`/`AppActivity`/bundle-id capabilities must match exactly
  what was deployed.
- **Android tests fail to launch against a debug build** — ensure
  `NoReset=true` is set (already default in `UITests.Android/AppiumSetup.cs`)
  since Appium is incompatible with Fast Deployment; use a Release build +
  explicit `.apk` path if you must disable `NoReset`.
