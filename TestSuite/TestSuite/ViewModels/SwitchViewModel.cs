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
        set
        {
            if (isToggled != value)
            {
                isToggled = value;
                OnPropertyChanged(nameof(IsToggled));
            }
        }
    }
}
