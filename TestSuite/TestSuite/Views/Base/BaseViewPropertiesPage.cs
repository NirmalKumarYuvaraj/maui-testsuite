using TestSuite.AutomationIds;
using TestSuite.Helper;
using TestSuite.ViewModels.Base;

namespace TestSuite.Views.Base;

public class BaseViewPropertiesPage : ContentPage
{
    readonly BaseViewModel? _viewModel;

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

    public BaseViewPropertiesPage(BaseViewModel viewModel)
    {
        _viewModel = viewModel;
        BindingContext = _viewModel;
        Title = "View Properties";

        InitializeToolbar();

        var layout = new VerticalStackLayout
        {
            Spacing = 5,
            Padding = new Thickness(5, 0, 5, 0)
        };

        BuildLayoutAndSizeSection(layout);
        BuildAlignmentAndAppearanceSection(layout);
        BuildBehaviorSection(layout);
        BuildAdvancedSection(layout);
        BuildReadOnlySection(layout);

        Content = new ScrollView { Content = layout };
    }

    void InitializeToolbar()
    {
        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Apply",
            AutomationId = BaseViewIds.ApplyToolbarItem,
            Command = new Command(async () => await Navigation.PopToRootAsync())
        });
    }

    void BuildLayoutAndSizeSection(VerticalStackLayout layout)
    {
        layout.Add(CreateSectionHeader("LAYOUT & SIZE"));

        var button = new Button { Text = "Layout & Size Properties" };
        button.Clicked += OnNavigateToLayoutAndSizePropertiesPageClicked;
        layout.Add(button);
    }

    void BuildAlignmentAndAppearanceSection(VerticalStackLayout layout)
    {
        var grid = new Grid
        {
            ColumnSpacing = 5,
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star }
            }
        };

        grid.Add(BuildAlignmentStack());
        grid.Add(BuildAppearanceStack(), 1, 0);

        layout.Add(grid);
    }

    VerticalStackLayout BuildAlignmentStack()
    {
        var stack = new VerticalStackLayout { Spacing = 0 };
        stack.Add(CreateSectionHeader("ALIGNMENT"));

        var horizontalOptionsEntry = new Entry();
        horizontalOptionsEntry.TextChanged += OnHorizontalOptionsChanged;
        stack.Add(PropertyPageHelpers.CreateStackedEntryRow("Horizontal Options", horizontalOptionsEntry));

        var verticalOptionsEntry = new Entry();
        verticalOptionsEntry.TextChanged += OnVerticalOptionsChanged;
        stack.Add(PropertyPageHelpers.CreateStackedEntryRow("Vertical Options", verticalOptionsEntry));

        var flowDirectionEntry = new Entry();
        flowDirectionEntry.TextChanged += OnFlowDirectionChanged;
        stack.Add(PropertyPageHelpers.CreateStackedEntryRow("Flow Direction", flowDirectionEntry));

        return stack;
    }

    VerticalStackLayout BuildAppearanceStack()
    {
        var stack = new VerticalStackLayout { Spacing = 0 };
        stack.Add(CreateSectionHeader("APPEARANCE"));

        var opacityEntry = new Entry { AutomationId = BaseViewIds.OpacityEntry };
        opacityEntry.TextChanged += OnOpacityChanged;
        stack.Add(PropertyPageHelpers.CreateStackedEntryRow("Opacity (0 to 1)", opacityEntry));

        var backgroundEntry = new Entry();
        backgroundEntry.TextChanged += OnBackgroundChanged;
        stack.Add(PropertyPageHelpers.CreateStackedEntryRow("Background", backgroundEntry));

        return stack;
    }

    void BuildBehaviorSection(VerticalStackLayout layout)
    {
        var behaviorLayout = new HorizontalStackLayout { Spacing = 5 };

        layout.Add(CreateSectionHeader("BEHAVIOR"));

        var isEnabledSwitch = new Switch { AutomationId = BaseViewIds.IsEnabledSwitch };
        isEnabledSwitch.SetBinding(Switch.IsToggledProperty, nameof(BaseViewModel.IsEnabled));
        behaviorLayout.Add(PropertyPageHelpers.CreateStackedEntryRow("Is Enabled", isEnabledSwitch));

        var inputTransparentSwitch = new Switch();
        inputTransparentSwitch.SetBinding(Switch.IsToggledProperty, nameof(BaseViewModel.InputTransparent));
        behaviorLayout.Add(PropertyPageHelpers.CreateStackedEntryRow("Input Transparent", inputTransparentSwitch));

        var isVisibleSwitch = new Switch { AutomationId = BaseViewIds.IsVisibleSwitch };
        isVisibleSwitch.SetBinding(Switch.IsToggledProperty, nameof(BaseViewModel.IsVisible));
        behaviorLayout.Add(PropertyPageHelpers.CreateStackedEntryRow("Is Visible", isVisibleSwitch));

        layout.Add(behaviorLayout);
    }

    void BuildAdvancedSection(VerticalStackLayout layout)
    {
        layout.Add(CreateSectionHeader("ADVANCED"));

        var zIndexEntry = new Entry();
        zIndexEntry.TextChanged += OnZIndexChanged;
        layout.Add(PropertyPageHelpers.CreateStackedEntryRow("ZIndex", zIndexEntry));

        var shadowButton = new Button { Text = "Shadow Options" };
        shadowButton.Clicked += OnNavigateToShadowOptionsPageClicked;
        layout.Add(shadowButton);

        var clipButton = new Button { Text = "Clip Options" };
        clipButton.Clicked += OnNavigateToClipOptionsPageClicked;
        layout.Add(clipButton);

        var moreOptionsButton = new Button { Text = "More Options" };
        moreOptionsButton.Clicked += OnMoreOptionsPageClicked;
        layout.Add(moreOptionsButton);
    }

    void BuildReadOnlySection(VerticalStackLayout layout)
    {
        layout.Add(CreateSectionHeader("READ-ONLY INFO"));

        var isFocusedLabel = new Label { Style = SharedStyles.ValueLabelStyle };
        layout.Add(PropertyPageHelpers.CreateStackedEntryRow("Is Focused", isFocusedLabel));

        var desiredSizeLabel = new Label { Style = SharedStyles.ValueLabelStyle };
        layout.Add(PropertyPageHelpers.CreateStackedEntryRow("Desired Size", desiredSizeLabel));

        var frameLabel = new Label { Style = SharedStyles.ValueLabelStyle };
        layout.Add(PropertyPageHelpers.CreateStackedEntryRow("Frame", frameLabel));
    }

    static Label CreateSectionHeader(string text) => new()
    {
        Text = text,
        Style = SectionHeaderStyle,
        Margin = new Thickness(0, 8, 0, 0)
    };

    void OnHorizontalOptionsChanged(object? sender, TextChangedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(e.NewTextValue))
            _viewModel?.HorizontalOptions = PropertyHelperExtensions.ToLayoutOptions(e.NewTextValue);
    }

    void OnVerticalOptionsChanged(object? sender, TextChangedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(e.NewTextValue))
            _viewModel?.VerticalOptions = PropertyHelperExtensions.ToLayoutOptions(e.NewTextValue);
    }

    void OnFlowDirectionChanged(object? sender, TextChangedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(e.NewTextValue))
            _viewModel?.FlowDirection = PropertyHelperExtensions.ToFlowDirection(e.NewTextValue);
    }

    void OnOpacityChanged(object? sender, TextChangedEventArgs e)
    {
        _viewModel?.Opacity = e.NewTextValue is not null && double.TryParse(e.NewTextValue, out var parsed) ? parsed : 1.0;
    }

    void OnBackgroundChanged(object? sender, TextChangedEventArgs e)
    {
        _viewModel?.Background = PropertyHelperExtensions.ToColor(e.NewTextValue);
    }

    void OnZIndexChanged(object? sender, TextChangedEventArgs e)
    {
        _viewModel?.ZIndex = e.NewTextValue is not null && int.TryParse(e.NewTextValue, out var parsed) ? parsed : 0;
    }

    void OnNavigateToLayoutAndSizePropertiesPageClicked(object? sender, EventArgs e) =>
        Navigation.PushAsync(new LayoutAndSizePropertiesPage(_viewModel));

    void OnNavigateToShadowOptionsPageClicked(object? sender, EventArgs e) =>
        Navigation.PushAsync(new ShadowPropertiesPage());

    void OnNavigateToClipOptionsPageClicked(object? sender, EventArgs e) =>
        Navigation.PushAsync(new ClipPropertiesPage());

    void OnMoreOptionsPageClicked(object? sender, EventArgs e) =>
        Navigation.PushAsync(new TransformPropertiesPage());
}
