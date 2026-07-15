using System;
using TestSuite.ViewModels;

namespace TestSuite.Views;


public class SwitchNavPage : NavigationPage
{
    public SwitchNavPage()
    {
        var switchPage = new SwitchControlPage();
        PushAsync(switchPage);
    }
}


public class SwitchControlPage : ContentPage
{
    readonly SwitchViewModel? _viewModel;

    public SwitchControlPage()
    {
        _viewModel = new SwitchViewModel();
        BindingContext = _viewModel;
        SetupUI();
    }

    void SetupUI()
    {
        SetUpOptions();
        var layout = new StackLayout
        {
            Padding = new Thickness(20),
            Spacing = 10
        };

        Title = "Switch Control";


        var CustomSwitch = new Switch();
        CustomSwitch.SetBinding(Switch.IsEnabledProperty, nameof(SwitchViewModel.IsEnabled), BindingMode.TwoWay);
        CustomSwitch.SetBinding(Switch.IsToggledProperty, nameof(SwitchViewModel.IsToggled));
        layout.Children.Add(CustomSwitch);

        Content = layout;
    }

    void SetUpOptions()
    {
        ToolbarItems.Add(new ToolbarItem("Options", null, async () =>
        {
            await Navigation.PushAsync(new SwitchPropertiesPage(_viewModel));
        }));
    }
}
