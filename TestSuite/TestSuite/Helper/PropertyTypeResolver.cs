using System;

namespace TestSuite.Helper;

public static class PropertyTypeResolver
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
            _ => "LeftToRight"
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

}
