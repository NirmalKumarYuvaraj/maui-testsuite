using System;

namespace TestSuite.Views.Base;

public class ShadowPropertiesPage : ContentPage
{
    Entry? ShadowBrushEntry;
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

        ShadowBrushEntry = new Entry();
        layout.Add(PropertyPageHelpers.CreateEntryRow("Shadow Brush", ShadowBrushEntry, new Thickness(8, 4)));

        ShadowOffsetXEntry = new Entry();
        layout.Add(PropertyPageHelpers.CreateEntryRow("Shadow Offset X", ShadowOffsetXEntry, new Thickness(8, 4)));

        ShadowOffsetYEntry = new Entry();
        layout.Add(PropertyPageHelpers.CreateEntryRow("Shadow Offset Y", ShadowOffsetYEntry, new Thickness(8, 4)));

        ShadowOpacityEntry = new Entry();
        layout.Add(PropertyPageHelpers.CreateEntryRow("Shadow Opacity", ShadowOpacityEntry, new Thickness(8, 4)));

        ShadowRadiusEntry = new Entry();
        layout.Add(PropertyPageHelpers.CreateEntryRow("Shadow Radius", ShadowRadiusEntry, new Thickness(8, 4)));

        Content = new ScrollView
        {
            Content = layout
        };
    }
}
