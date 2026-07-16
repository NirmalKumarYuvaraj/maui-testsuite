using System;
using TestSuite.AutomationIds;
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
    readonly Switch TestSwitch = new Switch { AutomationId = SwitchIds.Control };

    public SwitchControlPage()
    {
        _viewModel = new SwitchViewModel(TestSwitch);
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


        TestSwitch.SetBinding(Switch.IsEnabledProperty, nameof(SwitchViewModel.IsEnabled));
        TestSwitch.SetBinding(Switch.IsToggledProperty, nameof(SwitchViewModel.IsToggled));
        layout.Children.Add(TestSwitch);

        Content = layout;
    }

    void SetUpOptions()
    {
        ToolbarItems.Add(new ToolbarItem("Options", null, async () =>
        {
            await Navigation.PushAsync(new SwitchPropertiesPage(_viewModel));
        })
        {
            AutomationId = SwitchIds.OptionsToolbarItem
        });
    }
}
