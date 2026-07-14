using System;

namespace TestSuite.Test.Base;

public class ShadowPropertiesPage : ContentPage
{
    Entry? ShadowColorEntry;
    Entry? ShadowOffsetXEntry;
    Entry? ShadowOffsetYEntry;
    Entry? ShadowOpacityEntry;
    Entry? ShadowRadiusEntry;

    public ShadowPropertiesPage()
    {
        Title = "Shadow Properties";

        var layout = new VerticalStackLayout
        {
            Spacing = 0,
            Padding = new Thickness(0, 0, 0, 24)
        };

        ShadowColorEntry = new Entry();
        layout.Add(CreateEntryRow("Shadow Color", ShadowColorEntry));

        ShadowOffsetXEntry = new Entry();
        layout.Add(CreateEntryRow("Shadow Offset X", ShadowOffsetXEntry));

        ShadowOffsetYEntry = new Entry();
        layout.Add(CreateEntryRow("Shadow Offset Y", ShadowOffsetYEntry));

        ShadowOpacityEntry = new Entry();
        layout.Add(CreateEntryRow("Shadow Opacity", ShadowOpacityEntry));

        ShadowRadiusEntry = new Entry();
        layout.Add(CreateEntryRow("Shadow Radius", ShadowRadiusEntry));

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
