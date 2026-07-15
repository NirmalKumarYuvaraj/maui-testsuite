using System;

namespace TestSuite.Views.Base;

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
        layout.Add(PropertyPageHelpers.CreateEntryRow("Clip To Bounds", ClipToBoundsEntry, new Thickness(8, 4)));

        ClipShapeEntry = new Entry();
        layout.Add(PropertyPageHelpers.CreateEntryRow("Clip Shape", ClipShapeEntry, new Thickness(8, 4)));

        Content = new ScrollView
        {
            Content = layout
        };
    }
}
