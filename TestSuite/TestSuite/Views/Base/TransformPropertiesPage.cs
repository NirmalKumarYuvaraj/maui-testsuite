namespace TestSuite.Views.Base;

public class TransformPropertiesPage : ContentPage
{
    Entry? TranslationXEntry;
    Entry? TranslationYEntry;
    Entry? RotationEntry;
    Entry? RotationXEntry;
    Entry? RotationYEntry;
    Entry? ScaleEntry;
    Entry? ScaleXEntry;
    Entry? ScaleYEntry;
    Entry? AnchorXEntry;
    Entry? AnchorYEntry;

    Style? sectionHeaderStyle;

    void CreateStyles()
    {
        sectionHeaderStyle = new Style(typeof(Label))
        {
            Setters =
            {
                new Setter { Property = Label.FontSizeProperty, Value = 13d },
                new Setter { Property = Label.FontAttributesProperty, Value = FontAttributes.Bold },
                new Setter { Property = Label.TextColorProperty, Value = Colors.White },
                new Setter { Property = Label.BackgroundColorProperty, Value = Color.FromArgb("#444") },
            }
        };
    }

    public TransformPropertiesPage()
    {
        Title = "Transform Properties";

        CreateStyles();

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

        layout.Add(new Label
        {
            Text = "TRANSLATION",
            Style = sectionHeaderStyle,
            Margin = new Thickness(0, 8, 0, 0)
        });

        TranslationXEntry = new Entry { Keyboard = Keyboard.Numeric };
        TranslationXEntry.TextChanged += OnTranslationXChanged;
        layout.Add(PropertyPageHelpers.CreateEntryRow("Translation X", TranslationXEntry, new Thickness(8, 4)));

        TranslationYEntry = new Entry { Keyboard = Keyboard.Numeric };
        TranslationYEntry.TextChanged += OnTranslationYChanged;
        layout.Add(PropertyPageHelpers.CreateEntryRow("Translation Y", TranslationYEntry, new Thickness(8, 4)));

        layout.Add(new Label
        {
            Text = "ROTATION",
            Style = sectionHeaderStyle,
            Margin = new Thickness(0, 8, 0, 0)
        });

        RotationEntry = new Entry { Keyboard = Keyboard.Numeric };
        RotationEntry.TextChanged += OnRotationChanged;
        layout.Add(PropertyPageHelpers.CreateEntryRow("Rotation (Z)", RotationEntry, new Thickness(8, 4)));

        RotationXEntry = new Entry { Keyboard = Keyboard.Numeric };
        RotationXEntry.TextChanged += OnRotationXChanged;
        layout.Add(PropertyPageHelpers.CreateEntryRow("Rotation X", RotationXEntry, new Thickness(8, 4)));

        RotationYEntry = new Entry { Keyboard = Keyboard.Numeric };
        RotationYEntry.TextChanged += OnRotationYChanged;
        layout.Add(PropertyPageHelpers.CreateEntryRow("Rotation Y", RotationYEntry, new Thickness(8, 4)));

        layout.Add(new Label
        {
            Text = "SCALE",
            Style = sectionHeaderStyle,
            Margin = new Thickness(0, 8, 0, 0)
        });

        ScaleEntry = new Entry { Keyboard = Keyboard.Numeric };
        ScaleEntry.TextChanged += OnScaleChanged;
        layout.Add(PropertyPageHelpers.CreateEntryRow("Scale", ScaleEntry, new Thickness(8, 4)));

        ScaleXEntry = new Entry { Keyboard = Keyboard.Numeric };
        ScaleXEntry.TextChanged += OnScaleXChanged;
        layout.Add(PropertyPageHelpers.CreateEntryRow("Scale X", ScaleXEntry, new Thickness(8, 4)));

        ScaleYEntry = new Entry { Keyboard = Keyboard.Numeric };
        ScaleYEntry.TextChanged += OnScaleYChanged;
        layout.Add(PropertyPageHelpers.CreateEntryRow("Scale Y", ScaleYEntry, new Thickness(8, 4)));

        layout.Add(new Label
        {
            Text = "ANCHOR (pivot point, 0–1)",
            Style = sectionHeaderStyle,
            Margin = new Thickness(0, 8, 0, 0)
        });

        AnchorXEntry = new Entry { Keyboard = Keyboard.Numeric };
        AnchorXEntry.TextChanged += OnAnchorXChanged;
        layout.Add(PropertyPageHelpers.CreateEntryRow("Anchor X", AnchorXEntry, new Thickness(8, 4)));

        AnchorYEntry = new Entry { Keyboard = Keyboard.Numeric };
        AnchorYEntry.TextChanged += OnAnchorYChanged;
        layout.Add(PropertyPageHelpers.CreateEntryRow("Anchor Y", AnchorYEntry, new Thickness(8, 4)));

        Content = new ScrollView
        {
            Content = layout
        };
    }

    void OnTranslationXChanged(object? sender, TextChangedEventArgs e)
    {
        // Needs to implement
    }

    void OnTranslationYChanged(object? sender, TextChangedEventArgs e)
    {
        // Needs to implement
    }

    void OnRotationChanged(object? sender, TextChangedEventArgs e)
    {
        // Needs to implement
    }

    void OnRotationXChanged(object? sender, TextChangedEventArgs e)
    {
        // Needs to implement
    }

    void OnRotationYChanged(object? sender, TextChangedEventArgs e)
    {
        // Needs to implement
    }

    void OnScaleChanged(object? sender, TextChangedEventArgs e)
    {
        // Needs to implement
    }

    void OnScaleXChanged(object? sender, TextChangedEventArgs e)
    {
        // Needs to implement
    }

    void OnScaleYChanged(object? sender, TextChangedEventArgs e)
    {
        // Needs to implement
    }

    void OnAnchorXChanged(object? sender, TextChangedEventArgs e)
    {
        // Needs to implement
    }

    void OnAnchorYChanged(object? sender, TextChangedEventArgs e)
    {
        // Needs to implement
    }
}
