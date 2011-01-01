/*
 * ZomB Dashboard System <http://firstforge.wpi.edu/sf/projects/zombdashboard>
 * Copyright (C) 2011, Patrick Plenefisch and FIRST Robotics Team 451 "The Cat Attack"
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace System451.Communication.Dashboard.ViZ
{
    public class SnapGridHelper
    {
        public const double SnapableWithinDistance = 0.5;
        public const double SnapDistance = 10;
        public const double SnapableForceDistance = 5.1;

        public static Func<FrameworkElement, FrameworkElement, double> SnapDistanceLeftLeft = (current, other) => (Left(other) - Left(current));
        public static Func<FrameworkElement, FrameworkElement, double> SnapDistanceLeftRight = (current, other) => (Right(other) - Left(current));
        public static Func<FrameworkElement, FrameworkElement, bool> SnapableLeft =
            (current, other) => (Math.Abs(SnapDistanceLeftLeft(current, other)) < SnapableWithinDistance || Math.Abs(SnapDistanceLeftRight(current, other)) < SnapableWithinDistance);

        public static Func<FrameworkElement, FrameworkElement, double> SnapDistanceRightRight = (current, other) => (Right(other) - Right(current));
        public static Func<FrameworkElement, FrameworkElement, double> SnapDistanceRightLeft = (current, other) => (Left(other) - Right(current));
        public static Func<FrameworkElement, FrameworkElement, bool> SnapableRight =
            (current, other) => (Math.Abs(SnapDistanceRightRight(current, other)) < SnapableWithinDistance || Math.Abs(SnapDistanceRightLeft(current, other)) < SnapableWithinDistance);

        public static Func<FrameworkElement, FrameworkElement, double> SnapDistanceTopTop = (current, other) => (Top(other) - Top(current));
        public static Func<FrameworkElement, FrameworkElement, double> SnapDistanceTopBottom = (current, other) => (Bottom(other) - Top(current));
        public static Func<FrameworkElement, FrameworkElement, bool> SnapableTop =
            (current, other) => (Math.Abs(SnapDistanceTopTop(current, other)) < SnapableWithinDistance || Math.Abs(SnapDistanceTopBottom(current, other)) < SnapableWithinDistance);

        public static Func<FrameworkElement, FrameworkElement, double> SnapDistanceBottomBottom = (current, other) => (Bottom(other) - Bottom(current));
        public static Func<FrameworkElement, FrameworkElement, double> SnapDistanceBottomTop = (current, other) => (Top(other) - Bottom(current));
        public static Func<FrameworkElement, FrameworkElement, bool> SnapableBottom =
            (current, other) => (Math.Abs(SnapDistanceBottomBottom(current, other)) < SnapableWithinDistance || Math.Abs(SnapDistanceBottomTop(current, other)) < SnapableWithinDistance);


        public static Func<FrameworkElement, FrameworkElement, bool> SnapableDistanceLeft =
            (current, other) => (Math.Abs(SnapDistanceLeftRight(current, other) + SnapDistance) < SnapableWithinDistance);
        public static Func<FrameworkElement, FrameworkElement, bool> SnapableDistanceRight =
            (current, other) => (Math.Abs(SnapDistanceRightLeft(current, other) - SnapDistance) < SnapableWithinDistance);
        public static Func<FrameworkElement, FrameworkElement, double> SnapableDistanceLeftRightY =
            (current, other) => (-SnapableDistanceEqu(new Point(Left(current), Top(current)), new Point(Right(other), Top(other)), new Point(Left(current), Bottom(current)), new Point(Right(other), Bottom(other)))) - Top(current);

        public static Func<FrameworkElement, FrameworkElement, bool> SnapableDistanceTop =
            (current, other) => (Math.Abs(SnapDistanceTopBottom(current, other) + SnapDistance) < SnapableWithinDistance);
        public static Func<FrameworkElement, FrameworkElement, bool> SnapableDistanceBottom =
            (current, other) => (Math.Abs(SnapDistanceBottomTop(current, other) - SnapDistance) < SnapableWithinDistance);
        public static Func<FrameworkElement, FrameworkElement, double> SnapableDistanceTopBottomX =
            (current, other) => (-SnapableDistanceEquX(new Point(Left(current), Top(current)), new Point(Left(other), Bottom(other)), new Point(Right(current), Top(current)), new Point(Right(other), Bottom(other)))) - Left(current);


        public static Func<FrameworkElement, FrameworkElement, double> SnapableForceLeft1 =
            (current, other) => Math.Abs(SnapDistanceLeftRight(current, other));
        public static Func<FrameworkElement, FrameworkElement, double> SnapableForceRight1 =
            (current, other) => Math.Abs(SnapDistanceRightLeft(current, other));
        public static Func<FrameworkElement, FrameworkElement, double> SnapableForceTop1 =
            (current, other) => Math.Abs(SnapDistanceTopBottom(current, other));
        public static Func<FrameworkElement, FrameworkElement, double> SnapableForceBottom1 =
            (current, other) => Math.Abs(SnapDistanceBottomTop(current, other));

        public static Func<FrameworkElement, FrameworkElement, double> SnapableForceLeft2 =
            (current, other) => Math.Abs(SnapDistanceLeftLeft(current, other));
        public static Func<FrameworkElement, FrameworkElement, double> SnapableForceRight2 =
            (current, other) => Math.Abs(SnapDistanceRightRight(current, other));
        public static Func<FrameworkElement, FrameworkElement, double> SnapableForceTop2 =
            (current, other) => Math.Abs(SnapDistanceTopTop(current, other));
        public static Func<FrameworkElement, FrameworkElement, double> SnapableForceBottom2 =
            (current, other) => Math.Abs(SnapDistanceBottomBottom(current, other));


        public static Func<FrameworkElement, FrameworkElement, double> SnapableForceDistanceLeft =
            (current, other) => (Math.Abs(SnapDistanceLeftRight(current, other) + SnapDistance));
        public static Func<FrameworkElement, FrameworkElement, double> SnapableForceDistanceRight =
            (current, other) => (Math.Abs(SnapDistanceRightLeft(current, other) - SnapDistance));
        public static Func<FrameworkElement, FrameworkElement, double> SnapableForceDistanceTop =
            (current, other) => (Math.Abs(SnapDistanceTopBottom(current, other) + SnapDistance));
        public static Func<FrameworkElement, FrameworkElement, double> SnapableForceDistanceBottom =
            (current, other) => (Math.Abs(SnapDistanceBottomTop(current, other) - SnapDistance));



        public static Func<FrameworkElement, double> Right = (ctrl) => (Canvas.GetLeft(ctrl) + ctrl.Width);
        public static Func<FrameworkElement, double> Left = (ctrl) => Canvas.GetLeft(ctrl);
        public static Func<FrameworkElement, double> Top = (ctrl) => Canvas.GetTop(ctrl);
        public static Func<FrameworkElement, double> Bottom = (ctrl) => (Canvas.GetTop(ctrl) + ctrl.Height);

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

        public static Color DistanceLineColor = Colors.LightBlue;
        public static Color EqualLineColor = Colors.Blue;
    }

    public class SnapGridDistance : IComparable<SnapGridDistance>
    {
        public SnapGridDistance() { }
        public SnapType Type { get; set; }
        public SnapGridDirections Location { get; set; }
        public double Distance { get; set; }
        public FrameworkElement other { get; set; }

        #region IComparable<SnapGridDistance> Members

        public int CompareTo(SnapGridDistance other)
        {
            return Distance.CompareTo(other.Distance);
        }

        #endregion
    }

    public enum SnapType
    {
        Equal1,
        Equal2,
        Distance
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
