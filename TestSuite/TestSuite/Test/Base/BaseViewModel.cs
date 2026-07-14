using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestSuite.Test.Base;

public class BaseViewModel : INotifyPropertyChanged
{
    LayoutOptions _horizontalOptions = LayoutOptions.Start;
    LayoutOptions _verticalOptions = LayoutOptions.Start;

    double _height = 100;
    double _width = 100;
    double _margin = 0;

    public double Height
    {
        get => _height;
        set { _height = value; OnPropertyChanged(nameof(Height)); }
    }

    public double Width
    {
        get => _width;
        set { _width = value; OnPropertyChanged(nameof(Width)); }
    }

    public double Margin
    {
        get => _margin;
        set { _margin = value; OnPropertyChanged(nameof(Margin)); }
    }

    public LayoutOptions HorizontalOptions
    {
        get => _horizontalOptions;
        set { _horizontalOptions = value; OnPropertyChanged(nameof(HorizontalOptions)); }
    }

    public LayoutOptions VerticalOptions
    {
        get => _verticalOptions;
        set { _verticalOptions = value; OnPropertyChanged(nameof(VerticalOptions)); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
