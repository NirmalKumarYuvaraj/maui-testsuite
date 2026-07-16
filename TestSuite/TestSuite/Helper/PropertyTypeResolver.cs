using System;

namespace TestSuite.Helper;

public static class PropertyHelperExtensions
{
    public static string FromLayoutOptions(this LayoutOptions layoutOptions)
    {
        return layoutOptions.Alignment switch
        {
            LayoutAlignment.Start => "Start",
            LayoutAlignment.Center => "Center",
            LayoutAlignment.End => "End",
            LayoutAlignment.Fill => "Fill",
            _ => "Start"
        };
    }

    public static LayoutOptions ToLayoutOptions(string layoutOptionsString)
    {
        return layoutOptionsString switch
        {
            "Start" => LayoutOptions.Start,
            "Center" => LayoutOptions.Center,
            "End" => LayoutOptions.End,
            "Fill" => LayoutOptions.Fill,
            _ => default
        };
    }

    public static string FromFlowDirection(this FlowDirection flowDirection)
    {
        return flowDirection switch
        {
            FlowDirection.MatchParent => "MatchParent",
            FlowDirection.LeftToRight => "LeftToRight",
            FlowDirection.RightToLeft => "RightToLeft",
            _ => "MatchParent"
        };
    }

    public static FlowDirection ToFlowDirection(string flowDirectionString)
    {
        return flowDirectionString switch
        {
            "MatchParent" => FlowDirection.MatchParent,
            "LeftToRight" => FlowDirection.LeftToRight,
            "RightToLeft" => FlowDirection.RightToLeft,
            _ => default
        };
    }

    public static Brush ToColor(string colorString)
    {
        if (string.IsNullOrWhiteSpace(colorString))
            return Brush.Default;

        if (colorString.StartsWith('#'))
        {
            return new SolidColorBrush(Color.FromArgb(colorString));
        }
        else if (colorString.Contains("linear", StringComparison.OrdinalIgnoreCase))
        {
            // create a linear gradient brush 
            return new LinearGradientBrush()
            {
                EndPoint = new Point(1, 0),
                GradientStops =
                [
                    new GradientStop(){Color = Colors.Yellow, Offset = 0.1f},
                    new GradientStop(){Color = Colors.Green, Offset = 1.0f}
                ]
            };
        }
        else if (colorString.Contains("radial", StringComparison.OrdinalIgnoreCase))
        {
            // create a radial gradient brush
            return new RadialGradientBrush()
            {
                GradientStops =
                [
                    new GradientStop(){Color = Colors.Yellow, Offset = 0.1f},
                    new GradientStop(){Color = Colors.Green, Offset = 1.0f}
                ]
            };

        }
        else
        {
            return new SolidColorBrush(Colors.Transparent);
        }
    }

}
