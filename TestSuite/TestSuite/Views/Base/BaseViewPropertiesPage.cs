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
    Button? MoreOptionsButton;

    Label? IsFocusedLabel;
    Label? DesiredSizeLabel;
    Label? FrameLabel;

    Style? sectionHeaderStyle;

    BaseViewModel? _viewModel;

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
        BindingContext = _viewModel;
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
        leftStack.Add(PropertyPageHelpers.CreateStackedEntryRow("Horizontal Options", HorizontalOptionsEntry));

        VerticalOptionsEntry = new Entry();
        VerticalOptionsEntry.TextChanged += OnVerticalOptionsChanged;
        leftStack.Add(PropertyPageHelpers.CreateStackedEntryRow("Vertical Options", VerticalOptionsEntry));

        FlowDirectionEntry = new Entry();
        FlowDirectionEntry.TextChanged += OnFlowDirectionChanged;
        leftStack.Add(PropertyPageHelpers.CreateStackedEntryRow("Flow Direction", FlowDirectionEntry));

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
        rightStack.Add(PropertyPageHelpers.CreateStackedEntryRow("Opacity", OpacityEntry));

        VisibilityEntry = new Entry();
        VisibilityEntry.TextChanged += OnVisibilityChanged;
        rightStack.Add(PropertyPageHelpers.CreateStackedEntryRow("Visibility", VisibilityEntry));

        BackgroundEntry = new Entry();
        BackgroundEntry.TextChanged += OnBackgroundChanged;
        rightStack.Add(PropertyPageHelpers.CreateStackedEntryRow("Background", BackgroundEntry));

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
        IsEnabledSwitch.SetBinding(Switch.IsToggledProperty, nameof(BaseViewModel.IsEnabled), BindingMode.TwoWay);
        behaviorLayout.Add(PropertyPageHelpers.CreateStackedEntryRow("Is Enabled", IsEnabledSwitch));

        InputTransparentSwitch = new Switch();
        InputTransparentSwitch.Toggled += OnInputTransparentToggled;
        behaviorLayout.Add(PropertyPageHelpers.CreateStackedEntryRow("Input Transparent", InputTransparentSwitch));

        layout.Add(behaviorLayout);

        layout.Add(new Label
        {
            Text = "ADVANCED",
            Style = sectionHeaderStyle,
            Margin = new Thickness(0, 8, 0, 0)
        });

        ZIndexEntry = new Entry();
        ZIndexEntry.TextChanged += OnZIndexChanged;
        layout.Add(PropertyPageHelpers.CreateStackedEntryRow("ZIndex", ZIndexEntry));

        NavigateToShadowOptionsPageButton = new Button { Text = "Shadow Options" };
        NavigateToShadowOptionsPageButton.Clicked += OnNavigateToShadowOptionsPageClicked;
        layout.Add(NavigateToShadowOptionsPageButton);

        NavigateToClipOptionsPageButton = new Button { Text = "Clip Options" };
        NavigateToClipOptionsPageButton.Clicked += OnNavigateToClipOptionsPageClicked;
        layout.Add(NavigateToClipOptionsPageButton);

        MoreOptionsButton = new Button { Text = "More Options" };
        MoreOptionsButton.Clicked += OnMoreOptionsPageClicked;

        layout.Add(MoreOptionsButton);

        layout.Add(new Label
        {
            Text = "READ-ONLY INFO",
            Style = sectionHeaderStyle,
            Margin = new Thickness(0, 8, 0, 0)
        });

        IsFocusedLabel = new Label { Style = SharedStyles.ValueLabelStyle };
        layout.Add(PropertyPageHelpers.CreateStackedEntryRow("Is Focused", IsFocusedLabel));

        DesiredSizeLabel = new Label { Style = SharedStyles.ValueLabelStyle };
        layout.Add(PropertyPageHelpers.CreateStackedEntryRow("Desired Size", DesiredSizeLabel));

        FrameLabel = new Label { Style = SharedStyles.ValueLabelStyle };
        layout.Add(PropertyPageHelpers.CreateStackedEntryRow("Frame", FrameLabel));

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

    void OnMoreOptionsPageClicked(object? sender, EventArgs e)
    {
        // future implementation for more options navigation
    }
}

// NOTE (backlog / not yet implemented):
// 1. Change all pickers to Entry.
// 2. Change all sliders to Entry.
// 3. Add const strings for enum-like values (layout options, flow direction, visibility).
// 4. Background entry should support both named colors and hex values.
// 5. Expose all Shadow/Clip properties as entry fields.
// 6. Make the page non scrollable.
// 7. Add a "reset to default" button.
// 8. Show options in a dedicated page listing all properties and current values;
//    tapping a property navigates to a page to edit that value.
// 9. Flow: main page shows the control + toolbar "Options" button -> options page
//    lists that control's properties (+ "More options" for additional/base settings).
// 10. Keep everything C# only, no XAML.
