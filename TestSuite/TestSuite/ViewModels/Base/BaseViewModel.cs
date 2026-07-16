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

    // ── Constructor ──────────────────────────────────────────────────────────
    public BaseViewModel(View testView)
    {
        horizontalOptions = testView.HorizontalOptions;
        verticalOptions = testView.VerticalOptions;
        flowDirection = testView.FlowDirection;

        opacity = testView.Opacity;
        isVisible = testView.IsVisible;
        background = testView.Background;

        isEnabled = testView.IsEnabled;
        inputTransparent = testView.InputTransparent;

        zIndex = testView.ZIndex;
        shadow = testView.Shadow;
        clip = testView.Clip;

        widthRequest = testView.WidthRequest;
        heightRequest = testView.HeightRequest;
        minimumWidthRequest = testView.MinimumWidthRequest;
        minimumHeightRequest = testView.MinimumHeightRequest;
        maximumWidthRequest = testView.MaximumWidthRequest;
        maximumHeightRequest = testView.MaximumHeightRequest;
        margin = testView.Margin;

        translationX = testView.TranslationX;
        translationY = testView.TranslationY;
        rotation = testView.Rotation;
        rotationX = testView.RotationX;
        rotationY = testView.RotationY;
        scale = testView.Scale;
        scaleX = testView.ScaleX;
        scaleY = testView.ScaleY;
        anchorX = testView.AnchorX;
        anchorY = testView.AnchorY;
        isFocused = testView.IsFocused;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
