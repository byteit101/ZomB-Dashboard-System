using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;

namespace System451.Communication.Dashboard.ViZ
{
    public class SnapGridHelper
    {
        public const double SnapableWithinDistance = 0.5;
        public const double SnapDistance = 10;

        public static Func<Control, Control, double> SnapDistanceLeftLeft = (current, other) => (Left(other) - Left(current));
        public static Func<Control, Control, double> SnapDistanceLeftRight = (current, other) => (Right(other) - Left(current));
        public static Func<Control, Control, bool> SnapableLeft =
            (current, other) => (Math.Abs(SnapDistanceLeftLeft(current, other)) < SnapableWithinDistance || Math.Abs(SnapDistanceLeftRight(current, other)) < SnapableWithinDistance);

        public static Func<Control, Control, double> SnapDistanceRightRight = (current, other) => (Right(other) - Right(current));
        public static Func<Control, Control, double> SnapDistanceRightLeft = (current, other) => (Left(other) - Right(current));
        public static Func<Control, Control, bool> SnapableRight =
            (current, other) => (Math.Abs(SnapDistanceRightRight(current, other)) < SnapableWithinDistance || Math.Abs(SnapDistanceRightLeft(current, other)) < SnapableWithinDistance);

        public static Func<Control, Control, double> SnapDistanceTopTop = (current, other) => (Top(other) - Top(current));
        public static Func<Control, Control, double> SnapDistanceTopBottom = (current, other) => (Bottom(other) - Top(current));
        public static Func<Control, Control, bool> SnapableTop =
            (current, other) => (Math.Abs(SnapDistanceTopTop(current, other)) < SnapableWithinDistance || Math.Abs(SnapDistanceTopBottom(current, other)) < SnapableWithinDistance);

        public static Func<Control, Control, double> SnapDistanceBottomBottom = (current, other) => (Bottom(other) - Bottom(current));
        public static Func<Control, Control, double> SnapDistanceBottomTop = (current, other) => (Top(other) - Bottom(current));
        public static Func<Control, Control, bool> SnapableBottom =
            (current, other) => (Math.Abs(SnapDistanceBottomBottom(current, other)) < SnapableWithinDistance || Math.Abs(SnapDistanceBottomTop(current, other)) < SnapableWithinDistance);


        public static Func<Control, Control, bool> SnapableDistanceLeft =
            (current, other) => (Math.Abs(SnapDistanceLeftRight(current, other) + SnapDistance) < SnapableWithinDistance);
        public static Func<Control, Control, bool> SnapableDistanceRight =
            (current, other) => (Math.Abs(SnapDistanceRightLeft(current, other) - SnapDistance) < SnapableWithinDistance);
        public static Func<Control, Control, double> SnapableDistanceLeftRightY =
            (current, other) => (-SnapableDistanceEqu(new Point(Left(current), Top(current)), new Point(Right(other), Top(other)), new Point(Left(current), Bottom(current)), new Point(Right(other), Bottom(other)))) - Top(current);

        public static Func<Control, Control, bool> SnapableDistanceTop =
            (current, other) => (Math.Abs(SnapDistanceTopBottom(current, other) + SnapDistance) < SnapableWithinDistance);
        public static Func<Control, Control, bool> SnapableDistanceBottom =
            (current, other) => (Math.Abs(SnapDistanceBottomTop(current, other) - SnapDistance) < SnapableWithinDistance);
        public static Func<Control, Control, double> SnapableDistanceTopBottomX =
            (current, other) => (-SnapableDistanceEqu(new Point(Top(current), Left(current)), new Point(Top(other), Right(other)), new Point(Bottom(current), Left(current)), new Point(Bottom(other), Right(other)))) - Left(current);


        public static Func<Control, double> Right = (ctrl) => (Canvas.GetLeft(ctrl) + ctrl.Width);
        public static Func<Control, double> Left = (ctrl) => Canvas.GetLeft(ctrl);
        public static Func<Control, double> Top = (ctrl) => Canvas.GetTop(ctrl);
        public static Func<Control, double> Bottom = (ctrl) => (Canvas.GetTop(ctrl) + ctrl.Height);

        public static Func<Point, Point, double> SnapableDistanceEquM =
            (current, other) => ((current.Y - other.Y) / (current.X - other.X));
        public static Func<Point, Point, double> SnapableDistanceEquB =
            (current, other) => (current.X * SnapableDistanceEquM(current, other) - current.Y);
        public static Func<Point, Point, Point, Point, double> SnapableDistanceEquX =
            (current1, other1, current2, other2) => ((SnapableDistanceEquB(current2, other1) - SnapableDistanceEquB(current1, other2)) / (SnapableDistanceEquM(current1, other2) - SnapableDistanceEquM(current2, other1)));
        public static Func<Point, Point, Point, Point, double> SnapableDistanceEqu =
            (current1, other1, current2, other2) => (SnapableDistanceEquM(current1, other2) * SnapableDistanceEquX(current1, other1, current2, other2) + SnapableDistanceEquB(current1, other2));
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
