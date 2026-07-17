using System;
using TestSuite.ViewModels.Base;

namespace TestSuite.ViewModels;

public class SwitchViewModel : BaseViewModel
{
    public SwitchViewModel(View testView) : base(testView)
    {
    }

    bool isToggled = false;
    public bool IsToggled
    {
        get => isToggled;
        set { if (isToggled != value) { isToggled = value; OnPropertyChanged(); } }
    }

    Color? onColor;
    public Color? OnColor
    {
        get => onColor;
        set { if (!ReferenceEquals(onColor, value)) { onColor = value; OnPropertyChanged(); } }
    }

    Color? offColor;
    public Color? OffColor
    {
        get => offColor;
        set { if (!ReferenceEquals(offColor, value)) { offColor = value; OnPropertyChanged(); } }
    }

    Color? thumbColor;
    public Color? ThumbColor
    {
        get => thumbColor;
        set { if (!ReferenceEquals(thumbColor, value)) { thumbColor = value; OnPropertyChanged(); } }
    }
}
