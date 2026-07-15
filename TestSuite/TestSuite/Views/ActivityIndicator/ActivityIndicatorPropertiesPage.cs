using System;
using TestSuite.ViewModels;
using TestSuite.Views.Base;

namespace TestSuite.Views.ActivityIndicator;

public class ActivityIndicatorPropertiesPage : ContentPage
{
    readonly ActivityIndicatorViewModel? _viewModel;
    public ActivityIndicatorPropertiesPage(ActivityIndicatorViewModel? viewModel)
    {
        _viewModel = viewModel;
        BindingContext = _viewModel;

        var layout = new StackLayout
        {
            Padding = new Thickness(20),
            Spacing = 10
        };

        layout.Children.Add(new Label
        {
            Text = "Activity Indicator Properties",
            FontSize = 24,
            HorizontalOptions = LayoutOptions.Center
        });
        Button basePropertiesButton = new Button
        {
            Text = "Base Properties",
            HorizontalOptions = LayoutOptions.Center
        };
        basePropertiesButton.Clicked += async (sender, e) =>
        {
            await Navigation.PushAsync(new BaseViewPropertiesPage(_viewModel!));
        };

        layout.Children.Add(basePropertiesButton);
        Content = layout;

        SetUpOptions();
    }

    void SetUpOptions()
    {
        ToolbarItems.Add(new ToolbarItem("Apply", null, async () =>
        {
            await Navigation.PopToRootAsync();
        }));
    }
}
