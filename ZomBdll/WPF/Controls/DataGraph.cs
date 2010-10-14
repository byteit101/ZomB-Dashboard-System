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
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace System451.Communication.Dashboard.WPF.Controls
{
    /// <summary>
    /// Interaction logic for DataGraph.xaml
    /// </summary>
    [TemplatePart(Name = "PART_PathGeo", Type = typeof(GeometryDrawing)), Design.ZomBDesignable()]
    public class DataGraph : ZomBGLControl, IValueConverter, Design.IZomBDesignableControl
    {
        GeometryDrawing PathGeo;
        Queue<double> vals = new Queue<double>(300);
        static DataGraph()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DataGraph),
            new FrameworkPropertyMetadata(typeof(DataGraph)));
            DoubleValueProperty.OverrideMetadata(typeof(DataGraph), new FrameworkPropertyMetadata(new PropertyChangedCallback(newDdb)));
        }

        public DataGraph()
        {
            this.Background = Brushes.Black;
            this.Foreground = Brushes.Lime;
            this.SnapsToDevicePixels = true;
            this.BorderBrush = Brushes.Green;
            this.Width = 300;
            this.Height = 50;
            for (int i = 0; i < 300; i++) vals.Enqueue(0);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PathGeo = base.GetTemplateChild("PART_PathGeo") as GeometryDrawing;
        }

        static void newDdb(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataGraph am = (o as DataGraph);
            am.vals.Dequeue();
            am.vals.Enqueue((double)e.NewValue);
            am.Regenerate();
        }

        private void Regenerate()
        {
            var pf = new PathFigure();
            int x = 0;
            foreach (var y in vals)
            {
                if (x == 0)
                {
                    pf.StartPoint = new Point(++x/2.0, y*-10.0+10.0);
                }
                else
                {
                    pf.Segments.Add(new LineSegment(new Point(++x/2.0, y*-10.0+10.0), true));
                }
            }
            var pgo = new PathGeometry();
            pgo.Figures.Add(pf);
            PathGeo.Geometry=pgo;
        }


        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (25.0 / (double)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IZomBDesignableControl Members

        public Design.ZomBDesignableControlInfo GetDesignInfo()
        {
            return new Design.ZomBDesignableControlInfo { Name = "Data Graph", Description = "This shows -1 to 1 over time, useful for almost everything", Type = typeof(DataGraph) };
        }

        #endregion
    }
}
