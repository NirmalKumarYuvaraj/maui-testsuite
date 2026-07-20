using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls.Shapes;
using TestSuite.Helper;

namespace TestSuite.ViewModels.Base;

public class BaseViewModel : INotifyPropertyChanged
{
    // ── Alignment ────────────────────────────────────────────────────────────
    LayoutOptions horizontalOptions;
    LayoutOptions verticalOptions;
    FlowDirection flowDirection;

    public LayoutOptions HorizontalOptions
    {
        get => horizontalOptions;
        set { if (horizontalOptions != value) { horizontalOptions = value; OnPropertyChanged(); } }
    }

    public LayoutOptions VerticalOptions
    {
        get => verticalOptions;
        set { if (verticalOptions != value) { verticalOptions = value; OnPropertyChanged(); } }
    }

    public FlowDirection FlowDirection
    {
        get => flowDirection;
        set { if (flowDirection != value) { flowDirection = value; OnPropertyChanged(); } }
    }


    // ── Appearance ───────────────────────────────────────────────────────────
    double opacity;
    bool isVisible;
    Brush? background;

    public double Opacity
    {
        get => opacity;
        set { if (opacity != value) { opacity = value; OnPropertyChanged(); } }
    }

    public bool IsVisible
    {
        get => isVisible;
        set { if (isVisible != value) { isVisible = value; OnPropertyChanged(); } }
    }

    public Brush? Background
    {
        get => background;
        set { if (background != value) { background = value; OnPropertyChanged(); } }
    }

    // ── Behavior ─────────────────────────────────────────────────────────────
    bool isEnabled;
    bool inputTransparent;

    public bool IsEnabled
    {
        get => isEnabled;
        set { if (isEnabled != value) { isEnabled = value; OnPropertyChanged(); } }
    }

    public bool InputTransparent
    {
        get => inputTransparent;
        set { if (inputTransparent != value) { inputTransparent = value; OnPropertyChanged(); } }
    }

    // ── Advanced ─────────────────────────────────────────────────────────────
    int zIndex;
    Shadow? shadow;
    Geometry? clip;

    public int ZIndex
    {
        get => zIndex;
        set { if (zIndex != value) { zIndex = value; OnPropertyChanged(); } }
    }

    public Shadow? Shadow
    {
        get => shadow;
        set { if (shadow != value) { shadow = value; OnPropertyChanged(); } }
    }

    public Geometry? Clip
    {
        get => clip;
        set { if (!ReferenceEquals(clip, value)) { clip = value; OnPropertyChanged(); } }
    }

    // ── Layout & Size ────────────────────────────────────────────────────────
    double widthRequest;
    double heightRequest;
    double minimumWidthRequest;
    double minimumHeightRequest;
    double maximumWidthRequest;
    double maximumHeightRequest;
    Thickness margin;

    public double WidthRequest
    {
        get => widthRequest;
        set { if (widthRequest != value) { widthRequest = value; OnPropertyChanged(); } }
    }

    public double HeightRequest
    {
        get => heightRequest;
        set { if (heightRequest != value) { heightRequest = value; OnPropertyChanged(); } }
    }

    public double MinimumWidthRequest
    {
        get => minimumWidthRequest;
        set { if (minimumWidthRequest != value) { minimumWidthRequest = value; OnPropertyChanged(); } }
    }

    public double MinimumHeightRequest
    {
        get => minimumHeightRequest;
        set { if (minimumHeightRequest != value) { minimumHeightRequest = value; OnPropertyChanged(); } }
    }

    public double MaximumWidthRequest
    {
        get => maximumWidthRequest;
        set { if (maximumWidthRequest != value) { maximumWidthRequest = value; OnPropertyChanged(); } }
    }

    public double MaximumHeightRequest
    {
        get => maximumHeightRequest;
        set { if (maximumHeightRequest != value) { maximumHeightRequest = value; OnPropertyChanged(); } }
    }

    public Thickness Margin
    {
        get => margin;
        set { if (margin != value) { margin = value; OnPropertyChanged(); } }
    }

    // ── Transforms ───────────────────────────────────────────────────────────
    double translationX;
    double translationY;
    double rotation;
    double rotationX;
    double rotationY;
    double scale;
    double scaleX;
    double scaleY;
    double anchorX;
    double anchorY;

    public double TranslationX
    {
        get => translationX;
        set { if (translationX != value) { translationX = value; OnPropertyChanged(); } }
    }

    public double TranslationY
    {
        get => translationY;
        set { if (translationY != value) { translationY = value; OnPropertyChanged(); } }
    }

    public double Rotation
    {
        get => rotation;
        set { if (rotation != value) { rotation = value; OnPropertyChanged(); } }
    }

    public double RotationX
    {
        get => rotationX;
        set { if (rotationX != value) { rotationX = value; OnPropertyChanged(); } }
    }

    public double RotationY
    {
        get => rotationY;
        set { if (rotationY != value) { rotationY = value; OnPropertyChanged(); } }
    }

    public double Scale
    {
        get => scale;
        set { if (scale != value) { scale = value; OnPropertyChanged(); } }
    }

    public double ScaleX
    {
        get => scaleX;
        set { if (scaleX != value) { scaleX = value; OnPropertyChanged(); } }
    }

    public double ScaleY
    {
        get => scaleY;
        set { if (scaleY != value) { scaleY = value; OnPropertyChanged(); } }
    }

    public double AnchorX
    {
        get => anchorX;
        set { if (anchorX != value) { anchorX = value; OnPropertyChanged(); } }
    }

    public double AnchorY
    {
        get => anchorY;
        set { if (anchorY != value) { anchorY = value; OnPropertyChanged(); } }
    }

    // ── Read-only info ───────────────────────────────────────────────────────
    bool isFocused;

    public bool IsFocused
    {
        get => isFocused;
        set { if (isFocused != value) { isFocused = value; OnPropertyChanged(); } }
    }

    // ── Reset-to-defaults snapshot ────────────────────────────────────────────
    //
    // Captured once, at construction, from the actual View instance under
    // test - not hardcoded literals (e.g. "LeftToRight") - so ResetToDefaults
    // can never drift from whatever MAUI's real default for a given property
    // actually is. This backs a single, reusable "reset everything" operation
    // for feature-matrix tests (spec/TestPlan.md's reset-architecture phase)
    // instead of every test/TearDown having to know and set each property's
    // default individually.
    readonly LayoutOptions defaultHorizontalOptions;
    readonly LayoutOptions defaultVerticalOptions;
    readonly FlowDirection defaultFlowDirection;
    readonly double defaultOpacity;
    readonly bool defaultIsVisible;
    readonly Brush? defaultBackground;
    readonly bool defaultIsEnabled;
    readonly bool defaultInputTransparent;
    readonly int defaultZIndex;
    readonly Shadow? defaultShadow;
    readonly Geometry? defaultClip;
    readonly double defaultWidthRequest;
    readonly double defaultHeightRequest;
    readonly double defaultMinimumWidthRequest;
    readonly double defaultMinimumHeightRequest;
    readonly double defaultMaximumWidthRequest;
    readonly double defaultMaximumHeightRequest;
    readonly Thickness defaultMargin;
    readonly double defaultTranslationX;
    readonly double defaultTranslationY;
    readonly double defaultRotation;
    readonly double defaultRotationX;
    readonly double defaultRotationY;
    readonly double defaultScale;
    readonly double defaultScaleX;
    readonly double defaultScaleY;
    readonly double defaultAnchorX;
    readonly double defaultAnchorY;

    // ── Constructor ──────────────────────────────────────────────────────────
    public BaseViewModel(View testView)
    {
        horizontalOptions = defaultHorizontalOptions = testView.HorizontalOptions;
        verticalOptions = defaultVerticalOptions = testView.VerticalOptions;
        flowDirection = defaultFlowDirection = testView.FlowDirection;

        opacity = defaultOpacity = testView.Opacity;
        isVisible = defaultIsVisible = testView.IsVisible;
        background = defaultBackground = testView.Background;

        isEnabled = defaultIsEnabled = testView.IsEnabled;
        inputTransparent = defaultInputTransparent = testView.InputTransparent;

        zIndex = defaultZIndex = testView.ZIndex;
        shadow = defaultShadow = testView.Shadow;
        clip = defaultClip = testView.Clip;

        widthRequest = defaultWidthRequest = testView.WidthRequest;
        heightRequest = defaultHeightRequest = testView.HeightRequest;
        minimumWidthRequest = defaultMinimumWidthRequest = testView.MinimumWidthRequest;
        minimumHeightRequest = defaultMinimumHeightRequest = testView.MinimumHeightRequest;
        maximumWidthRequest = defaultMaximumWidthRequest = testView.MaximumWidthRequest;
        maximumHeightRequest = defaultMaximumHeightRequest = testView.MaximumHeightRequest;
        margin = defaultMargin = testView.Margin;

        translationX = defaultTranslationX = testView.TranslationX;
        translationY = defaultTranslationY = testView.TranslationY;
        rotation = defaultRotation = testView.Rotation;
        rotationX = defaultRotationX = testView.RotationX;
        rotationY = defaultRotationY = testView.RotationY;
        scale = defaultScale = testView.Scale;
        scaleX = defaultScaleX = testView.ScaleX;
        scaleY = defaultScaleY = testView.ScaleY;
        anchorX = defaultAnchorX = testView.AnchorX;
        anchorY = defaultAnchorY = testView.AnchorY;
        isFocused = testView.IsFocused;
    }

    /// <summary>
    /// Restores every <c>BaseViewModel</c> property to the value captured
    /// from the actual View at construction time - the single reset
    /// mechanism feature-matrix tests should call (via a host-app Reset
    /// command, see <c>SwitchViewModel.ResetToDefaults</c>/<c>SwitchControlPage</c>)
    /// instead of setting each property back individually. Overridden by
    /// derived ViewModels to additionally reset their own control-specific
    /// properties/counters.
    /// </summary>
    public virtual void ResetToDefaults()
    {
        HorizontalOptions = defaultHorizontalOptions;
        VerticalOptions = defaultVerticalOptions;
        FlowDirection = defaultFlowDirection;

        Opacity = defaultOpacity;
        IsVisible = defaultIsVisible;
        Background = defaultBackground;

        IsEnabled = defaultIsEnabled;
        InputTransparent = defaultInputTransparent;

        ZIndex = defaultZIndex;
        Shadow = defaultShadow;
        Clip = defaultClip;

        WidthRequest = defaultWidthRequest;
        HeightRequest = defaultHeightRequest;
        MinimumWidthRequest = defaultMinimumWidthRequest;
        MinimumHeightRequest = defaultMinimumHeightRequest;
        MaximumWidthRequest = defaultMaximumWidthRequest;
        MaximumHeightRequest = defaultMaximumHeightRequest;
        Margin = defaultMargin;

        TranslationX = defaultTranslationX;
        TranslationY = defaultTranslationY;
        Rotation = defaultRotation;
        RotationX = defaultRotationX;
        RotationY = defaultRotationY;
        Scale = defaultScale;
        ScaleX = defaultScaleX;
        ScaleY = defaultScaleY;
        AnchorX = defaultAnchorX;
        AnchorY = defaultAnchorY;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
