namespace TestSuite.Views.Base;

/// <summary>
/// Label styles shared across the C#-only property pages so each page doesn't
/// need to redeclare identical Style instances.
/// </summary>
public static class SharedStyles
{
    public static Style PropLabelStyle { get; } = new Style(typeof(Label))
    {
        Setters =
        {
            new Setter { Property = Label.FontSizeProperty, Value = 13d },
            new Setter { Property = Label.VerticalOptionsProperty, Value = LayoutOptions.Center },
            new Setter { Property = Label.TextColorProperty, Value = Color.FromArgb("#333") }
        }
    };

    public static Style ValueLabelStyle { get; } = new Style(typeof(Label))
    {
        Setters =
        {
            new Setter { Property = Label.FontSizeProperty, Value = 12d },
            new Setter { Property = Label.TextColorProperty, Value = Color.FromArgb("#666") },
            new Setter { Property = Label.VerticalOptionsProperty, Value = LayoutOptions.Center }
        }
    };
}

/// <summary>
/// Layout helpers shared by the property pages to avoid re-implementing the
/// same "label + input" row Grid in every page.
/// </summary>
public static class PropertyPageHelpers
{
    public static Grid CreateEntryRow(string text, View view, Thickness padding = default)
    {
        var grid = new Grid
        {
            Padding = padding,
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
            Style = SharedStyles.PropLabelStyle
        });

        Grid.SetColumn(view, 1);
        grid.Add(view);

        return grid;
    }

    /// <summary>
    /// Two-row block: label on top, input view below.
    /// Used by pages that lay fields out side-by-side in a grid (e.g. Base View Properties).
    /// </summary>
    public static Grid CreateStackedEntryRow(string text, View view)
    {
        var grid = new Grid
        {
            ColumnSpacing = 8,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Star },
                new RowDefinition { Height = GridLength.Star }
            }
        };

        grid.Add(new Label
        {
            Text = text,
            Style = SharedStyles.PropLabelStyle
        });

        Grid.SetRow(view, 1);
        grid.Add(view);

        return grid;
    }
}
