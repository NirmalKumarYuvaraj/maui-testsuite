using System;
using TestSuite.ViewModels;

namespace TestSuite.Views.ActivityIndicator;

public class ActivityIndicatorNavPage : NavigationPage
{
    public ActivityIndicatorNavPage()
    {
        var activityIndicatorPage = new ActivityIndicatorControlPage();
        PushAsync(activityIndicatorPage);
    }
}


public class ActivityIndicatorControlPage : ContentPage
{
    readonly ActivityIndicatorViewModel? _viewModel;

    public ActivityIndicatorControlPage()
    {
        _viewModel = new ActivityIndicatorViewModel();
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

        layout.Children.Add(new Label
        {
            Text = "Activity Indicator Control",
            FontSize = 24,
            HorizontalOptions = LayoutOptions.Center
        });


        var CustomSwitch = new Switch();
        CustomSwitch.SetBinding(Switch.IsEnabledProperty, nameof(ActivityIndicatorViewModel.IsEnabled), BindingMode.TwoWay);
        layout.Children.Add(CustomSwitch);

        Content = layout;
    }

    void SetUpOptions()
    {
        ToolbarItems.Add(new ToolbarItem("Options", null, async () =>
        {
            await Navigation.PushAsync(new ActivityIndicatorPropertiesPage(_viewModel));
        }));
    }
}
