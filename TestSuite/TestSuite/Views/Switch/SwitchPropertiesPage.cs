using System;
using TestSuite.ViewModels;

namespace TestSuite.Views;

public class SwitchPropertiesPage : ContentPage
{
    readonly SwitchViewModel? _viewModel;
    public SwitchPropertiesPage(SwitchViewModel? viewModel)
    {
        _viewModel = viewModel;
        BindingContext = _viewModel;
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
