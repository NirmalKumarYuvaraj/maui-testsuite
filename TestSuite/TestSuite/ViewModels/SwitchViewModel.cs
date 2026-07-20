using System;
using System.Windows.Input;
using TestSuite.ViewModels.Base;

namespace TestSuite.ViewModels;

public class SwitchViewModel : BaseViewModel
{
    readonly bool defaultIsToggled;
    readonly Color? defaultOnColor;
    readonly Color? defaultOffColor;
    readonly Color? defaultThumbColor;

    public SwitchViewModel(View testView) : base(testView)
    {
        ToggledCommand = new Command<object?>(OnToggledCommandExecuted);
        defaultIsToggled = isToggled = ((Switch)testView).IsToggled;
        defaultOnColor = onColor = ((Switch)testView).OnColor;
        defaultOffColor = offColor = ((Switch)testView).OffColor;
        defaultThumbColor = thumbColor = ((Switch)testView).ThumbColor;
    }

    bool isToggled = false;
    public bool IsToggled
    {
        get => isToggled;
        set { if (isToggled != value) { isToggled = value; OnPropertyChanged(); } }
    }

    Color? onColor;
    public Color? OnColor
    {
        get => onColor;
        set { if (!ReferenceEquals(onColor, value)) { onColor = value; OnPropertyChanged(); } }
    }

    Color? offColor;
    public Color? OffColor
    {
        get => offColor;
        set { if (!ReferenceEquals(offColor, value)) { offColor = value; OnPropertyChanged(); } }
    }

    Color? thumbColor;
    public Color? ThumbColor
    {
        get => thumbColor;
        set { if (!ReferenceEquals(thumbColor, value)) { thumbColor = value; OnPropertyChanged(); } }
    }

    // ── Toggled event tracking ──────────────────────────────────────────
    //
    // Microsoft.Maui.Controls.Switch only exposes a Toggled event - it has
    // no Command/CommandParameter of its own (unlike Button), so the only
    // way to verify "does a Command fire when the user interacts with this
    // control" is to wire the event to a Command manually in the host page's
    // code-behind (see SwitchControlPage.OnTestSwitchToggled) and record
    // observable proof here that both the event and the command actually
    // ran. Every property below is surfaced as a read-only Label on the
    // control page (via AutomationId) so UI tests can assert on it without
    // needing any custom WebDriver capability.

    int toggledEventCount;
    public int ToggledEventCount
    {
        get => toggledEventCount;
        set { if (toggledEventCount != value) { toggledEventCount = value; OnPropertyChanged(); } }
    }

    bool? lastToggledEventValue;
    public bool? LastToggledEventValue
    {
        get => lastToggledEventValue;
        set { if (lastToggledEventValue != value) { lastToggledEventValue = value; OnPropertyChanged(); } }
    }

    string? commandParameter;
    /// <summary>
    /// Value passed to <see cref="ToggledCommand"/> when it executes -
    /// settable from the Options page's "Command Parameter" entry so tests
    /// can assert the exact parameter that comes back on
    /// <see cref="LastCommandParameter"/>.
    /// </summary>
    public string? CommandParameter
    {
        get => commandParameter;
        set { if (commandParameter != value) { commandParameter = value; OnPropertyChanged(); } }
    }

    /// <summary>
    /// Manually invoked from <c>SwitchControlPage</c>'s Toggled event
    /// handler with <see cref="CommandParameter"/> - there is no native
    /// Switch.Command/CommandParameter binding to hang this off of.
    /// </summary>
    public ICommand ToggledCommand { get; }

    int commandExecutionCount;
    public int CommandExecutionCount
    {
        get => commandExecutionCount;
        set { if (commandExecutionCount != value) { commandExecutionCount = value; OnPropertyChanged(); } }
    }

    object? lastCommandParameter;
    public object? LastCommandParameter
    {
        get => lastCommandParameter;
        set { if (!Equals(lastCommandParameter, value)) { lastCommandParameter = value; OnPropertyChanged(); } }
    }

    void OnToggledCommandExecuted(object? parameter)
    {
        CommandExecutionCount++;
        LastCommandParameter = parameter;
    }

    /// <summary>
    /// Single reset mechanism for every mutable state this ViewModel
    /// exposes - both <c>BaseViewModel</c>'s View properties (via
    /// <c>base.ResetToDefaults()</c>) and the Switch-specific/event-tracking
    /// state added here (<c>OnColor</c>/<c>OffColor</c>/<c>ThumbColor</c>,
    /// <c>CommandParameter</c>, and the cumulative
    /// <c>ToggledEventCount</c>/<c>CommandExecutionCount</c> counters).
    /// Invoked from a single "Reset" control directly on
    /// <c>SwitchControlPage</c> (no page navigation required) - see
    /// spec/TestPlan.md's reset-architecture phase for why this replaced the
    /// previous per-property <c>[TearDown]</c> that opened Options/View
    /// Properties and set each field back individually.
    /// </summary>
    public override void ResetToDefaults()
    {
        base.ResetToDefaults();

        // IsToggled = defaultIsToggled;
        // OnColor = defaultOnColor;
        // OffColor = defaultOffColor;
        // ThumbColor = defaultThumbColor;
        CommandParameter = null;

        ToggledEventCount = 0;
        LastToggledEventValue = null;
        CommandExecutionCount = 0;
        LastCommandParameter = null;
    }
}

