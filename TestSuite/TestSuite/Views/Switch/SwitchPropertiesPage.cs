using System;
using TestSuite.AutomationIds;
using TestSuite.ViewModels;
using TestSuite.ViewModels.Base;
using TestSuite.Views.Base;

namespace TestSuite.Views;

public class SwitchPropertiesPage : ContentPage
{
    readonly SwitchViewModel? _viewModel;
    public SwitchPropertiesPage(SwitchViewModel? viewModel)
    {
        _viewModel = viewModel;
        BindingContext = _viewModel;
        SetUpUI();
        SetUpOptions();
    }

    void SetUpUI()
    {
        Button navigateToViewPropertiesButton = new Button
        {
            Text = "Navigate to Switch View Properties Page",
            AutomationId = SwitchIds.NavigateToViewPropertiesButton,
            Command = new Command(async () =>
            {
                await Navigation.PushAsync(new BaseViewPropertiesPage(_viewModel!));
            })
        };

        Content = new StackLayout
        {
            Children =
            {
                new Label { Text = "Switch Properties Page" },
                navigateToViewPropertiesButton
            }
        };
    }


    void SetUpOptions()
    {
        ToolbarItems.Add(new ToolbarItem("Apply", null, async () =>
        {
            await Navigation.PopToRootAsync();
        })
        {
            AutomationId = SwitchIds.ApplyToolbarItem
        });
    }

}
