using System;
using TestSuite.AutomationIds;
using TestSuite.Helper;
using TestSuite.ViewModels;
using TestSuite.Views.Base;

namespace TestSuite.Views;

public class SwitchPropertiesPage : ContentPage
{
    readonly SwitchViewModel? _viewModel;

    static readonly Style SectionHeaderStyle = new(typeof(Label))
    {
        Setters =
        {
            new Setter { Property = Label.FontSizeProperty, Value = 13d },
            new Setter { Property = Label.FontAttributesProperty, Value = FontAttributes.Bold },
            new Setter { Property = Label.TextColorProperty, Value = Colors.White },
            new Setter { Property = Label.BackgroundColorProperty, Value = Color.FromArgb("#444") },
        }
    };

    public SwitchPropertiesPage(SwitchViewModel? viewModel)
    {
        _viewModel = viewModel;
        BindingContext = _viewModel;
        Title = "Switch Properties";

        InitializeToolbar();

        var layout = new VerticalStackLayout
        {
            Spacing = 5,
            Padding = new Thickness(5, 0, 5, 0)
        };

        BuildSwitchSection(layout);
        BuildViewPropertiesSection(layout);

        Content = new ScrollView { Content = layout };
    }

    void InitializeToolbar()
    {
        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Apply",
            AutomationId = SwitchIds.ApplyToolbarItem,
            Command = new Command(async () => await Navigation.PopToRootAsync())
        });
    }

    void BuildSwitchSection(VerticalStackLayout layout)
    {
        layout.Add(CreateSectionHeader("SWITCH PROPERTIES"));

        var isToggledSwitch = new Switch { AutomationId = SwitchIds.IsToggledSwitch };
        isToggledSwitch.SetBinding(Switch.IsToggledProperty, nameof(SwitchViewModel.IsToggled));
        layout.Add(PropertyPageHelpers.CreateEntryRow("Is Toggled", isToggledSwitch));

        var onColorEntry = new Entry { Placeholder = "e.g. #FF0000", AutomationId = SwitchIds.OnColorEntry };
        onColorEntry.TextChanged += OnOnColorChanged;
        layout.Add(PropertyPageHelpers.CreateEntryRow("On Color", onColorEntry));

        var offColorEntry = new Entry { Placeholder = "e.g. #CCCCCC", AutomationId = SwitchIds.OffColorEntry };
        offColorEntry.TextChanged += OnOffColorChanged;
        layout.Add(PropertyPageHelpers.CreateEntryRow("Off Color", offColorEntry));

        var thumbColorEntry = new Entry { Placeholder = "e.g. #FFFFFF", AutomationId = SwitchIds.ThumbColorEntry };
        thumbColorEntry.TextChanged += OnThumbColorChanged;
        layout.Add(PropertyPageHelpers.CreateEntryRow("Thumb Color", thumbColorEntry));
    }

    void BuildViewPropertiesSection(VerticalStackLayout layout)
    {
        layout.Add(CreateSectionHeader("VIEW PROPERTIES"));

        var button = new Button
        {
            Text = "View Properties",
            AutomationId = SwitchIds.NavigateToViewPropertiesButton
        };
        button.Clicked += OnNavigateToViewPropertiesClicked;
        layout.Add(button);
    }

    static Label CreateSectionHeader(string text) => new()
    {
        Text = text,
        Style = SectionHeaderStyle,
        Margin = new Thickness(0, 8, 0, 0)
    };

    void OnOnColorChanged(object? sender, TextChangedEventArgs e)
        => _viewModel!.OnColor = PropertyHelperExtensions.ToSwitchColor(e.NewTextValue);

    void OnOffColorChanged(object? sender, TextChangedEventArgs e)
        => _viewModel!.OffColor = PropertyHelperExtensions.ToSwitchColor(e.NewTextValue);

    void OnThumbColorChanged(object? sender, TextChangedEventArgs e)
        => _viewModel!.ThumbColor = PropertyHelperExtensions.ToSwitchColor(e.NewTextValue);

    void OnNavigateToViewPropertiesClicked(object? sender, EventArgs e)
        => Navigation.PushAsync(new BaseViewPropertiesPage(_viewModel!));
}
