using System;

namespace TestSuite.Test.Base;

public class ClipPropertiesPage : ContentPage
{
    Entry? ClipToBoundsEntry;
    Entry? ClipShapeEntry;

    public ClipPropertiesPage()
    {
        Title = "Clip Properties";

        var layout = new VerticalStackLayout
        {
            Spacing = 0,
            Padding = new Thickness(0, 0, 0, 24)
        };

        ClipToBoundsEntry = new Entry();
        layout.Add(CreateEntryRow("Clip To Bounds", ClipToBoundsEntry));

        ClipShapeEntry = new Entry();
        layout.Add(CreateEntryRow("Clip Shape", ClipShapeEntry));

        Content = new ScrollView
        {
            Content = layout
        };
    }

    Grid CreateEntryRow(string text, View view)
    {
        var grid = new Grid
        {
            Padding = new Thickness(8, 4),
            ColumnSpacing = 8,
            ColumnDefinitions =
   {
    new ColumnDefinition { Width = 160 },
    new ColumnDefinition { Width = GridLength.Star }
   }
        };

        grid.Add(new Label
        {
            Text = text,
            FontSize = 13,
            VerticalOptions = LayoutOptions.Center,
            TextColor = Color.FromArgb("#333")
        });

        Grid.SetColumn(view, 1);
        grid.Add(view);

        return grid;
    }
}
