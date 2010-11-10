/*
 * ZomB Dashboard System <http://firstforge.wpi.edu/sf/projects/zombdashboard>
 * Copyright (C) 2009-2010, Patrick Plenefisch and FIRST Robotics Team 451 "The Cat Attack"
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
using System.Windows.Media;

namespace System451.Communication.Dashboard.WPF
{
    public static class Arrow
    {
        private static readonly string Arrowup = "M2,0.232 4,3.692 0,3.692 2,0.232z";
        private const double degreeToRadian = 0.017453292519943295769236907684886;//pi/180

        /// <summary>
        /// Create an arrow in the center of the rectangle at degrees off
        /// </summary>
        /// <param name="location">Rectangle to contain the circle that the arrow starts at the center of</param>
        /// <param name="degree">The degree that arrow is at (where 0 degrees is up)</param>
        static public Drawing Get(Rect location, double degree, double thickness, Brush color)
        {
            return Init(location, degree, thickness, color);
        }

        /// <summary>
        /// Create an arrow in the center of the rectangle at degrees off
        /// </summary>
        /// <param name="x">Rectangle x origin</param>
        /// <param name="y">Rectangle y origin</param>
        /// <param name="width">Rectangle width</param>
        /// <param name="height">Rectangle height</param>
        /// <param name="degree">The degree that arrow is at (where 0 degrees is up)</param>
        public static Drawing Get(double x, double y, double width, double height, double degree, double thickness, Brush color)
        {
            return Init(new Rect(x, y, width, height), degree, thickness, color);
        }

#warning Need to do other overloads for constructor

        private static Drawing Init(Rect location, double degree, double width, Brush color)
        {
            Point t = (location.TopLeft + (Vector)location.BottomRight);
            Point m = new Point(t.X / 2.0, t.Y / 2.0);

            Point z = new Point(t.X / 2.0 + (Math.Sin(degree * degreeToRadian) * ((location.Right - location.Left) / 2)),
                                 t.Y / 2.0 - (Math.Cos(degree * degreeToRadian) * ((location.Bottom - location.Top) / 2)));

            StreamGeometry g = new StreamGeometry();
            using (StreamGeometryContext dc = g.Open())
            {
                dc.BeginFigure(m, false, false);
                dc.LineTo(z, true, false);
                Geometry pg = PathGeometry.CreateFromGeometry(PathGeometry.Parse(Arrowup));
                pg.Transform = new RotateTransform(degree);
                PathGeometry p = PathGeometry.CreateFromGeometry(pg);

                dc.BeginFigure(p.Figures[0].StartPoint, true, true);
                dc.LineTo((p.Figures[0].Segments[0] as PolyLineSegment).Points[0], true, true);
                dc.LineTo((p.Figures[0].Segments[0] as PolyLineSegment).Points[1], true, true);
                dc.LineTo((p.Figures[0].Segments[0] as PolyLineSegment).Points[2], true, true);
            }
            g.Freeze();
            return new GeometryDrawing(color, new Pen(color, width), g);
        }
    }
}
