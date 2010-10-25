using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;

namespace System451.Communication.Dashboard.ViZ
{
    public class SnapGridHelper
    {
        public const double SnapableDistance = 0.5;
        public static Func<Control, Control, double> SnapDistaceLeftLeft =
            (current, other) => (Canvas.GetLeft(other) - Canvas.GetLeft(current));
        public static Func<Control, Control, double> SnapDistaceLeftRight =
            (current, other) => ((Canvas.GetLeft(other) + other.Width) - Canvas.GetLeft(current));
        public static Func<Control, Control, bool> SnapableLeft =
            (current, other) => (Math.Abs(SnapDistaceLeftLeft(current, other)) < SnapableDistance || Math.Abs(SnapDistaceLeftRight(current, other)) < SnapableDistance);

        public static Func<Control, Control, double> SnapDistaceTopTop =
            (current, other) => (Canvas.GetTop(other) - Canvas.GetTop(current));
        public static Func<Control, Control, double> SnapDistaceTopBottom =
            (current, other) => ((Canvas.GetTop(other) + other.Height) - Canvas.GetTop(current));
        public static Func<Control, Control, bool> SnapableTop =
            (current, other) => (Math.Abs(SnapDistaceTopTop(current, other)) < SnapableDistance || Math.Abs(SnapDistaceTopBottom(current, other)) < SnapableDistance);
        
        public static Func<Control, Control, double> SnapDistaceBottomBottom =
            (current, other) => ((Canvas.GetTop(other) + other.Height) - (Canvas.GetTop(current)+current.Height));
        public static Func<Control, Control, double> SnapDistaceBottomTop =
            (current, other) => (Canvas.GetTop(other) - (Canvas.GetTop(current)+current.Height));
        public static Func<Control, Control, bool> SnapableBottom =
            (current, other) => (Math.Abs(SnapDistaceBottomBottom(current, other)) < SnapableDistance || Math.Abs(SnapDistaceBottomTop(current, other)) < SnapableDistance);
    }

    public class SnapLine
    {
        public SnapLine()
        {
        }
        public double x1 { get; set; }
        public double y1 { get; set; }
        public double x2 { get; set; }
        public double y2 { get; set; }
        public Color color { get; set; }
    }

    public enum SnapGridDirections
    {
        X = 0x1,
        Y = 0x2,
        Right = 0x4,
        Bottom = 0x8,
        All = 0xF
    }
}
