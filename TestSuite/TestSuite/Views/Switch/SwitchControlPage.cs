using System;
using TestSuite.AutomationIds;
using TestSuite.ViewModels;

namespace TestSuite.Views;


public class SwitchNavPage : NavigationPage
{
    public SwitchNavPage()
    {
        var switchPage = new SwitchControlPage();
        PushAsync(switchPage);
    }
}

public class SwitchControlPage : ContentPage
{
    readonly SwitchViewModel? _viewModel;
    readonly Switch TestSwitch = new Switch { AutomationId = SwitchIds.Control };

    public SwitchControlPage()
    {
        _viewModel = new SwitchViewModel(TestSwitch);
        BindingContext = _viewModel;
        SetupUI();
    }

    void SetupUI()
    {
        SetUpOptions();
        var layout = new StackLayout
        {
            Padding = new Thickness(20),
            Spacing = 10
        };

        Title = "Switch Control";

        //Switch Properties
        TestSwitch.SetBinding(Switch.IsToggledProperty, nameof(SwitchViewModel.IsToggled));
        TestSwitch.SetBinding(Switch.OnColorProperty, nameof(SwitchViewModel.OnColor));
        TestSwitch.SetBinding(Switch.OffColorProperty, nameof(SwitchViewModel.OffColor));
        TestSwitch.SetBinding(Switch.ThumbColorProperty, nameof(SwitchViewModel.ThumbColor));

        //BaseView Properties - Alignment
        TestSwitch.SetBinding(Switch.HorizontalOptionsProperty, nameof(SwitchViewModel.HorizontalOptions));
        TestSwitch.SetBinding(Switch.VerticalOptionsProperty, nameof(SwitchViewModel.VerticalOptions));
        TestSwitch.SetBinding(Switch.FlowDirectionProperty, nameof(SwitchViewModel.FlowDirection));

        //BaseView Properties - Appearance
        TestSwitch.SetBinding(Switch.OpacityProperty, nameof(SwitchViewModel.Opacity));
        TestSwitch.SetBinding(Switch.IsVisibleProperty, nameof(SwitchViewModel.IsVisible));
        TestSwitch.SetBinding(Switch.BackgroundProperty, nameof(SwitchViewModel.Background));

        //BaseView Properties - Behavior
        TestSwitch.SetBinding(Switch.IsEnabledProperty, nameof(SwitchViewModel.IsEnabled));
        TestSwitch.SetBinding(Switch.InputTransparentProperty, nameof(SwitchViewModel.InputTransparent));

        //BaseView Properties - Advanced
        TestSwitch.SetBinding(Switch.ZIndexProperty, nameof(SwitchViewModel.ZIndex));
        TestSwitch.SetBinding(Switch.ShadowProperty, nameof(SwitchViewModel.Shadow));
        TestSwitch.SetBinding(Switch.ClipProperty, nameof(SwitchViewModel.Clip));

        //BaseView Properties - Layout & Size
        TestSwitch.SetBinding(Switch.WidthRequestProperty, nameof(SwitchViewModel.WidthRequest));
        TestSwitch.SetBinding(Switch.HeightRequestProperty, nameof(SwitchViewModel.HeightRequest));
        TestSwitch.SetBinding(Switch.MinimumWidthRequestProperty, nameof(SwitchViewModel.MinimumWidthRequest));
        TestSwitch.SetBinding(Switch.MinimumHeightRequestProperty, nameof(SwitchViewModel.MinimumHeightRequest));
        TestSwitch.SetBinding(Switch.MaximumWidthRequestProperty, nameof(SwitchViewModel.MaximumWidthRequest));
        TestSwitch.SetBinding(Switch.MaximumHeightRequestProperty, nameof(SwitchViewModel.MaximumHeightRequest));
        TestSwitch.SetBinding(Switch.MarginProperty, nameof(SwitchViewModel.Margin));

        //BaseView Properties - Transforms
        TestSwitch.SetBinding(Switch.TranslationXProperty, nameof(SwitchViewModel.TranslationX));
        TestSwitch.SetBinding(Switch.TranslationYProperty, nameof(SwitchViewModel.TranslationY));
        TestSwitch.SetBinding(Switch.RotationProperty, nameof(SwitchViewModel.Rotation));
        TestSwitch.SetBinding(Switch.RotationXProperty, nameof(SwitchViewModel.RotationX));
        TestSwitch.SetBinding(Switch.RotationYProperty, nameof(SwitchViewModel.RotationY));
        TestSwitch.SetBinding(Switch.ScaleProperty, nameof(SwitchViewModel.Scale));
        TestSwitch.SetBinding(Switch.ScaleXProperty, nameof(SwitchViewModel.ScaleX));
        TestSwitch.SetBinding(Switch.ScaleYProperty, nameof(SwitchViewModel.ScaleY));
        TestSwitch.SetBinding(Switch.AnchorXProperty, nameof(SwitchViewModel.AnchorX));
        TestSwitch.SetBinding(Switch.AnchorYProperty, nameof(SwitchViewModel.AnchorY));

        layout.Children.Add(TestSwitch);
        layout.Children.Add(BuildDescriptionLabel());
        layout.Children.Add(BuildEventAndCommandDiagnostics());

        // Microsoft.Maui.Controls.Switch has no Command/CommandParameter of
        // its own (unlike Button) - the only native hook is the Toggled
        // event, so a Command is invoked manually from here to make
        // "does a Command fire on interaction" testable at all. See
        // SwitchViewModel's "Toggled event tracking" region for what gets
        // recorded and why.
        TestSwitch.Toggled += OnTestSwitchToggled;

        Content = layout;
    }

    static Label BuildDescriptionLabel() => new()
    {
        AutomationId = SwitchIds.DescriptionLabel,
        Text = "Toggle the Switch above to verify IsToggled binding, the Toggled event, " +
               "and ToggledCommand/CommandParameter execution (see the values below).",
        FontAttributes = FontAttributes.Italic,
    };

    VerticalStackLayout BuildEventAndCommandDiagnostics()
    {
        var stack = new VerticalStackLayout { Spacing = 4 };

        var toggledEventCountLabel = new Label { AutomationId = SwitchIds.ToggledEventCountLabel };
        toggledEventCountLabel.SetBinding(Label.TextProperty, new Binding(nameof(SwitchViewModel.ToggledEventCount), stringFormat: "{0}"));
        stack.Add(toggledEventCountLabel);

        var lastToggledValueLabel = new Label { AutomationId = SwitchIds.LastToggledValueLabel };
        lastToggledValueLabel.SetBinding(Label.TextProperty, new Binding(nameof(SwitchViewModel.LastToggledEventValue), stringFormat: "{0}"));
        stack.Add(lastToggledValueLabel);

        var commandExecutionCountLabel = new Label { AutomationId = SwitchIds.CommandExecutionCountLabel };
        commandExecutionCountLabel.SetBinding(Label.TextProperty, new Binding(nameof(SwitchViewModel.CommandExecutionCount), stringFormat: "{0}"));
        stack.Add(commandExecutionCountLabel);

        var lastCommandParameterLabel = new Label { AutomationId = SwitchIds.LastCommandParameterLabel };
        lastCommandParameterLabel.SetBinding(Label.TextProperty, new Binding(nameof(SwitchViewModel.LastCommandParameter), stringFormat: "{0}"));
        stack.Add(lastCommandParameterLabel);

        return stack;
    }

    void OnTestSwitchToggled(object? sender, ToggledEventArgs e)
    {
        if (_viewModel is null)
            return;

        _viewModel.ToggledEventCount++;
        _viewModel.LastToggledEventValue = e.Value;

        if (_viewModel.ToggledCommand.CanExecute(_viewModel.CommandParameter))
            _viewModel.ToggledCommand.Execute(_viewModel.CommandParameter);
    }

    void SetUpOptions()
    {
        ToolbarItems.Add(new ToolbarItem("Options", null, async () =>
        {
            await Navigation.PushAsync(new SwitchPropertiesPage(_viewModel));
        })
        {
            AutomationId = SwitchIds.OptionsToolbarItem
        });

        // Single-click reset (no navigation) - restores every property and
        // event/command counter to the control's defaults directly on this
        // page. Exists specifically so UI tests can establish a clean
        // baseline in [SetUp] without the cost/fragility of navigating
        // through Options/View Properties and setting each field back
        // individually (spec/TestPlan.md's reset-architecture phase).
        ToolbarItems.Add(new ToolbarItem("Reset", null, () =>
        {
            _viewModel?.ResetToDefaults();
        })
        {
            AutomationId = SwitchIds.ResetToolbarItem
        });
    }
}
