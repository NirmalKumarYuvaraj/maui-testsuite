using System;
using TestSuite.ViewModels.Base;

namespace TestSuite.Views.Base;

public class BaseViewPropertiesPage : ContentPage
{
    Entry? HorizontalOptionsEntry;
    Entry? VerticalOptionsEntry;
    Entry? FlowDirectionEntry;
    Entry? OpacityEntry;
    Entry? VisibilityEntry;
    Entry? BackgroundEntry;
    Switch? IsEnabledSwitch;
    Switch? InputTransparentSwitch;
    Entry? ZIndexEntry;

    Button? NavigateToLayoutAndSizePropertiesPageButton;
    Button? NavigateToShadowOptionsPageButton;
    Button? NavigateToClipOptionsPageButton;

    Label? IsFocusedLabel;
    Label? DesiredSizeLabel;
    Label? FrameLabel;

    Style? sectionHeaderStyle;
    Style? propLabelStyle;
    Style? valueLabelStyle;

    BaseViewModel? _viewModel;

    Grid CreateEntryRow(string text, View view)
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
            Style = propLabelStyle
        });

        Grid.SetRow(view, 1);
        grid.Add(view);

        return grid;
    }

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

        propLabelStyle = new Style(typeof(Label))
        {
            Setters =
            {
                new Setter { Property = Label.FontSizeProperty, Value = 13d },
                new Setter { Property = Label.VerticalOptionsProperty, Value = LayoutOptions.Center },
                new Setter { Property = Label.TextColorProperty, Value = Color.FromArgb("#333") }
            }
        };

        valueLabelStyle = new Style(typeof(Label))
        {
            Setters =
            {
                new Setter { Property = Label.FontSizeProperty, Value = 12d },
                new Setter { Property = Label.TextColorProperty, Value = Color.FromArgb("#666") },
                new Setter { Property = Label.VerticalOptionsProperty, Value = LayoutOptions.Center }
            }
        };
    }

    void OnHorizontalOptionsChanged(object? sender, EventArgs e)
    {
        // Needs to implement
    }

    void OnVerticalOptionsChanged(object? sender, EventArgs e)
    {
        // Needs to implement
    }

    void OnFlowDirectionChanged(object? sender, EventArgs e)
    {
        // Needs to implement
    }

    void OnOpacityChanged(object? sender, TextChangedEventArgs e)
    {
        // Needs to implement
    }

    void OnVisibilityChanged(object? sender, EventArgs e)
    {
        // Needs to implement
    }

    void OnBackgroundChanged(object? sender, EventArgs e)
    {
        // Needs to implement
    }

    void OnIsEnabledToggled(object? sender, ToggledEventArgs e)
    {
        // Needs to implement
    }

    void OnInputTransparentToggled(object? sender, ToggledEventArgs e)
    {
        // Needs to implement
    }

    void OnZIndexChanged(object? sender, TextChangedEventArgs e)
    {
        // Needs to implement
    }

    void OnRefreshInfoClicked(object? sender, EventArgs e)
    {
        // Needs to implement
    }

    public BaseViewPropertiesPage(BaseViewModel viewModel)
    {
        _viewModel = viewModel;
        Title = "View Properties";

        CreateStyles();

        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Apply",
            Command = new Command(async () =>
            {
                await Navigation.PopAsync();
            })
        });

        var layout = new VerticalStackLayout
        {
            Spacing = 5,
        };

        layout.Add(new Label
        {
            Text = "LAYOUT & SIZE",
            Style = sectionHeaderStyle,
            Margin = new Thickness(0, 8, 0, 0)
        });

        NavigateToLayoutAndSizePropertiesPageButton = new Button { Text = "Layout & Size Properties" };
        NavigateToLayoutAndSizePropertiesPageButton.Clicked += OnNavigateToLayoutAndSizePropertiesPageClicked;
        layout.Add(NavigateToLayoutAndSizePropertiesPageButton);

        Grid appearanceAndAlignmentGrid = new Grid
        {
            ColumnSpacing = 5,
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star }
            }
        };

        var leftStack = new VerticalStackLayout
        {
            Spacing = 0
        };

        leftStack.Add(new Label
        {
            Text = "ALIGNMENT",
            Style = sectionHeaderStyle,
            Margin = new Thickness(0, 8, 0, 0)
        });

        HorizontalOptionsEntry = new Entry();
        HorizontalOptionsEntry.TextChanged += OnHorizontalOptionsChanged;
        leftStack.Add(CreateEntryRow("Horizontal Options", HorizontalOptionsEntry));

        VerticalOptionsEntry = new Entry();
        VerticalOptionsEntry.TextChanged += OnVerticalOptionsChanged;
        leftStack.Add(CreateEntryRow("Vertical Options", VerticalOptionsEntry));

        FlowDirectionEntry = new Entry();
        FlowDirectionEntry.TextChanged += OnFlowDirectionChanged;
        leftStack.Add(CreateEntryRow("Flow Direction", FlowDirectionEntry));

        appearanceAndAlignmentGrid.Add(leftStack);

        var rightStack = new VerticalStackLayout
        {
            Spacing = 0
        };

        rightStack.Add(new Label
        {
            Text = "APPEARANCE",
            Style = sectionHeaderStyle,
            Margin = new Thickness(0, 8, 0, 0)
        });

        OpacityEntry = new Entry();
        OpacityEntry.TextChanged += OnOpacityChanged;
        rightStack.Add(CreateEntryRow("Opacity", OpacityEntry));

        VisibilityEntry = new Entry();
        VisibilityEntry.TextChanged += OnVisibilityChanged;
        rightStack.Add(CreateEntryRow("Visibility", VisibilityEntry));

        BackgroundEntry = new Entry();
        BackgroundEntry.TextChanged += OnBackgroundChanged;
        rightStack.Add(CreateEntryRow("Background", BackgroundEntry));

        appearanceAndAlignmentGrid.Add(rightStack, 1, 0);

        layout.Add(appearanceAndAlignmentGrid);

        HorizontalStackLayout behaviorLayout = new HorizontalStackLayout
        {
            Spacing = 5
        };
        layout.Add(new Label
        {
            Text = "BEHAVIOR",
            Style = sectionHeaderStyle,
            Margin = new Thickness(0, 8, 0, 0)
        });

        IsEnabledSwitch = new Switch();
        IsEnabledSwitch.Toggled += OnIsEnabledToggled;
        behaviorLayout.Add(CreateEntryRow("Is Enabled", IsEnabledSwitch));

        InputTransparentSwitch = new Switch();
        InputTransparentSwitch.Toggled += OnInputTransparentToggled;
        behaviorLayout.Add(CreateEntryRow("Input Transparent", InputTransparentSwitch));

        layout.Add(behaviorLayout);

        layout.Add(new Label
        {
            Text = "ADVANCED",
            Style = sectionHeaderStyle,
            Margin = new Thickness(0, 8, 0, 0)
        });

        ZIndexEntry = new Entry();
        ZIndexEntry.TextChanged += OnZIndexChanged;
        layout.Add(CreateEntryRow("ZIndex", ZIndexEntry));

        NavigateToShadowOptionsPageButton = new Button { Text = "Shadow Options" };
        NavigateToShadowOptionsPageButton.Clicked += OnNavigateToShadowOptionsPageClicked;
        layout.Add(NavigateToShadowOptionsPageButton);

        NavigateToClipOptionsPageButton = new Button { Text = "Clip Options" };
        NavigateToClipOptionsPageButton.Clicked += OnNavigateToClipOptionsPageClicked;
        layout.Add(NavigateToClipOptionsPageButton);

        layout.Add(new Label
        {
            Text = "READ-ONLY INFO",
            Style = sectionHeaderStyle,
            Margin = new Thickness(0, 8, 0, 0)
        });

        IsFocusedLabel = new Label { Style = valueLabelStyle };
        layout.Add(CreateEntryRow("Is Focused", IsFocusedLabel));

        DesiredSizeLabel = new Label { Style = valueLabelStyle };
        layout.Add(CreateEntryRow("Desired Size", DesiredSizeLabel));

        FrameLabel = new Label { Style = valueLabelStyle };
        layout.Add(CreateEntryRow("Frame", FrameLabel));

        Content = new ScrollView
        {
            Content = layout
        };
    }

    private void OnNavigateToLayoutAndSizePropertiesPageClicked(object? sender, EventArgs e)
    {
        Navigation.PushAsync(new LayoutAndSizePropertiesPage(_viewModel));
    }

    void OnNavigateToShadowOptionsPageClicked(object? sender, EventArgs e)
    {
        Navigation.PushAsync(new ShadowPropertiesPage());
    }

    void OnNavigateToClipOptionsPageClicked(object? sender, EventArgs e)
    {
        Navigation.PushAsync(new ClipPropertiesPage());
    }
}

public class LayoutAndSizePropertiesPage : ContentPage
{
    Entry? WidthEntry;
    Entry? HeightEntry;
    Entry? MinWidthEntry;
    Entry? MaxWidthEntry;
    Entry? MinHeightEntry;
    Entry? MaxHeightEntry;
    Entry? MarginEntry;

    Style? propLabelStyle;

    BaseViewModel? _viewModel;

    public LayoutAndSizePropertiesPage(BaseViewModel? viewModel)
    {
        _viewModel = viewModel;
        Title = "Layout & Size Properties";
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

        WidthEntry = new Entry();
        WidthEntry.TextChanged += OnWidthChanged;
        layout.Add(CreateEntryRow("Width", WidthEntry));

        HeightEntry = new Entry();
        HeightEntry.TextChanged += OnHeightChanged;
        layout.Add(CreateEntryRow("Height", HeightEntry));

        MinWidthEntry = new Entry();
        MinWidthEntry.TextChanged += OnMinWidthChanged;
        layout.Add(CreateEntryRow("Minimum Width", MinWidthEntry));

        MaxWidthEntry = new Entry();
        MaxWidthEntry.TextChanged += OnMaxWidthChanged;
        layout.Add(CreateEntryRow("Maximum Width", MaxWidthEntry));

        MinHeightEntry = new Entry();
        MinHeightEntry.TextChanged += OnMinHeightChanged;
        layout.Add(CreateEntryRow("Minimum Height", MinHeightEntry));

        MaxHeightEntry = new Entry();
        MaxHeightEntry.TextChanged += OnMaxHeightChanged;
        layout.Add(CreateEntryRow("Maximum Height", MaxHeightEntry));

        MarginEntry = new Entry { Text = "0" };
        MarginEntry.TextChanged += OnMarginChanged;
        layout.Add(CreateEntryRow("Margin", MarginEntry));

        Content = new ScrollView
        {
            Content = layout
        };
    }

    Grid CreateEntryRow(string text, View view)
    {
        var grid = new Grid
        {
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
            Style = propLabelStyle
        });

        Grid.SetColumn(view, 1);
        grid.Add(view);

        return grid;
    }

    void CreateStyles()
    {
        propLabelStyle = new Style(typeof(Label))
        {
            Setters =
            {
                new Setter { Property = Label.FontSizeProperty, Value = 13d },
                new Setter { Property = Label.VerticalOptionsProperty, Value = LayoutOptions.Center },
                new Setter { Property = Label.TextColorProperty, Value = Color.FromArgb("#333") }
            }
        };
    }

    void OnWidthChanged(object? sender, TextChangedEventArgs e)
    {
        double v = Math.Round(double.Parse(e.NewTextValue));
    }

    void OnHeightChanged(object? sender, TextChangedEventArgs e)
    {
        double v = Math.Round(double.Parse(e.NewTextValue));
    }

    void OnMinWidthChanged(object? sender, TextChangedEventArgs e)
    {
        double v = Math.Round(double.Parse(e.NewTextValue));
    }

    void OnMaxWidthChanged(object? sender, TextChangedEventArgs e)
    {
        double v = Math.Round(double.Parse(e.NewTextValue));
    }

    void OnMinHeightChanged(object? sender, TextChangedEventArgs e)
    {
        double v = Math.Round(double.Parse(e.NewTextValue));
    }

    void OnMaxHeightChanged(object? sender, TextChangedEventArgs e)
    {
        double v = Math.Round(double.Parse(e.NewTextValue));
    }

    void OnMarginChanged(object? sender, TextChangedEventArgs e)
    {
        // Needs to implement
    }
}

/* New Specifications for the MainPage.xaml.cs:
1.Need to change all the pickers to Entry
2. Need to change all the sliders to Entry
3. Need to add Const string for all the values (layout options, flow direction, visibility)
4. Background should be entry that supports both const colors and hex values
5. Need to create a new page for the shadow and clip properties with all the properties exposed as entry fields.
6. should make the page non scrollable.
7. Need to add a button to reset all the values to default.
8. Need to show the options in a different page with a list of all the properties and their current values.
9. so the flow is mainpage has the Test control and a toolbar item to navigate to the options page. The options page has a list of all the properties of that specific controls (example : Mainpage ->  Label , Options Page - has label specific properties, More options button at the end to navigate to additional settings) and their current values. Clicking on an interface-based naming navigates to a new page where the user can change the value of that property.
10. Make everything c# only. no xaml
*/
