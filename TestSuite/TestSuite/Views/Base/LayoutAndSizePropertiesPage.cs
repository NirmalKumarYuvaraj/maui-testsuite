using System;
using TestSuite.ViewModels.Base;

namespace TestSuite.Views.Base;

public class LayoutAndSizePropertiesPage : ContentPage
{
    Entry? WidthEntry;
    Entry? HeightEntry;
    Entry? MinWidthEntry;
    Entry? MaxWidthEntry;
    Entry? MinHeightEntry;
    Entry? MaxHeightEntry;
    Entry? MarginEntry;

    BaseViewModel? _viewModel;

    public LayoutAndSizePropertiesPage(BaseViewModel? viewModel)
    {
        _viewModel = viewModel;
        Title = "Layout & Size Properties";

        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Apply",
            Command = new Command(async () =>
            {
                await Navigation.PopToRootAsync();
            })
        });

        var layout = new VerticalStackLayout
        {
            Spacing = 0,
            Padding = new Thickness(0, 0, 0, 24)
        };

        WidthEntry = new Entry();
        WidthEntry.TextChanged += OnWidthChanged;
        layout.Add(PropertyPageHelpers.CreateEntryRow("Width", WidthEntry));

        HeightEntry = new Entry();
        HeightEntry.TextChanged += OnHeightChanged;
        layout.Add(PropertyPageHelpers.CreateEntryRow("Height", HeightEntry));

        MinWidthEntry = new Entry();
        MinWidthEntry.TextChanged += OnMinWidthChanged;
        layout.Add(PropertyPageHelpers.CreateEntryRow("Minimum Width", MinWidthEntry));

        MaxWidthEntry = new Entry();
        MaxWidthEntry.TextChanged += OnMaxWidthChanged;
        layout.Add(PropertyPageHelpers.CreateEntryRow("Maximum Width", MaxWidthEntry));

        MinHeightEntry = new Entry();
        MinHeightEntry.TextChanged += OnMinHeightChanged;
        layout.Add(PropertyPageHelpers.CreateEntryRow("Minimum Height", MinHeightEntry));

        MaxHeightEntry = new Entry();
        MaxHeightEntry.TextChanged += OnMaxHeightChanged;
        layout.Add(PropertyPageHelpers.CreateEntryRow("Maximum Height", MaxHeightEntry));

        MarginEntry = new Entry { Text = "0" };
        MarginEntry.TextChanged += OnMarginChanged;
        layout.Add(PropertyPageHelpers.CreateEntryRow("Margin", MarginEntry));

        Content = new ScrollView
        {
            Content = layout
        };
    }

    void OnWidthChanged(object? sender, TextChangedEventArgs e) => TryParseRounded(e.NewTextValue);

    void OnHeightChanged(object? sender, TextChangedEventArgs e) => TryParseRounded(e.NewTextValue);

    void OnMinWidthChanged(object? sender, TextChangedEventArgs e) => TryParseRounded(e.NewTextValue);

    void OnMaxWidthChanged(object? sender, TextChangedEventArgs e) => TryParseRounded(e.NewTextValue);

    void OnMinHeightChanged(object? sender, TextChangedEventArgs e) => TryParseRounded(e.NewTextValue);

    void OnMaxHeightChanged(object? sender, TextChangedEventArgs e) => TryParseRounded(e.NewTextValue);

    void OnMarginChanged(object? sender, TextChangedEventArgs e)
    {
        // Needs to implement
    }

    // Shared by all size entries above. Uses TryParse instead of Parse so that
    // partial/invalid input (e.g. "-", "" while typing) doesn't crash the app.
    static double? TryParseRounded(string? text)
    {
        if (!double.TryParse(text, out var value))
            return null;

        return Math.Round(value);
    }
}
