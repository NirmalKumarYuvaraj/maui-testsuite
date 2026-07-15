using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestSuite.ViewModels.Base;

public class BaseViewModel : INotifyPropertyChanged
{
    bool isEnabled;

    public bool IsEnabled
    {
        get => isEnabled;
        set
        {
            if (isEnabled != value)
            {
                isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
