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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System451.Communication.Dashboard.WPF.Controls.Designer.PrimitiveControls;

namespace System451.Communication.Dashboard.WPF.Controls
{
    /// <summary>
    /// Interaction logic for DataGraph.xaml
    /// </summary>
    [TemplatePart(Name = "PART_PathGeo", Type = typeof(GeometryDrawing))]
    [Design.ZomBControl("Data Graph", Description = "This shows -1 to 1 over time, useful for almost everything", IconName = "DataGraphIcon")]
    [Design.ZomBDesignableProperty("Foreground")]
    [Design.ZomBDesignableProperty("Background")]
    [Design.ZomBDesignableProperty("BorderBrush")]
    public class DataGraph : ZomBGLControl, IValueConverter
    {
        GeometryDrawing PathGeo;
        GraphScale scale;
        Queue<double> vals = new Queue<double>(300);
        static DataGraph()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DataGraph),
            new FrameworkPropertyMetadata(typeof(DataGraph)));
        }

        public DataGraph()
        {
            this.Background = Brushes.Black;
            this.Foreground = Brushes.Lime;
            this.SnapsToDevicePixels = true;
            this.BorderBrush = Brushes.Green;
            this.Width = 200;
            this.Height = 100;
            for (int i = 0; i < 300; i++) vals.Enqueue(0);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PathGeo = base.GetTemplateChild("PART_PathGeo") as GeometryDrawing;
            scale = base.GetTemplateChild("PART_scale") as GraphScale;
            this.Regenerate();//Zombie planaria!
        }

        public override void UpdateControl(ZomBDataObject value)
        {
            try
            {
                if (value.Value is int || value.Value is double || value.Value is float)
                    newDdb((double)value.Value);
                else
                    newDdb(double.Parse(value.ToString()));
            }
            catch { }
        }

        void newDdb(double nv)
        {
            DataGraph am = this;
            am.vals.Dequeue();
            am.vals.Enqueue(nv);
            this.scale.MinX++;
            this.scale.MaxX++;
            am.Regenerate();
        }

        private void Regenerate()
        {
            if (PathGeo == null)
                return;
            var pf = new PathFigure();
            int x = 0;
            if (AutoSize)
            {
                double max, min;
                max = min = vals.Peek();
                foreach (var y in vals)
                {
                    if (y > max)
                        max = y;
                    else if (y < min)
                        min = y;
                }
                if (max == min)
                {
                    ++max;
                    --min;
                }
                else
                {
                    var dist=(max - min) * (5 / 4);
                    var oldmax = max;
                    max = dist + min;
                    min = oldmax - dist;
                }
                Max = max;
                Min = min;
            }
            foreach (var y in vals)
            {
                if (x == 0)
                {
                    pf.StartPoint = new Point(++x / 2.0, Neutralize(y));
                }
                else
                {
                    pf.Segments.Add(new LineSegment(new Point(++x / 2.0, Neutralize(y)), true));
                }
            }
            var pgo = new PathGeometry();
            pgo.Figures.Add(pf);
            PathGeo.Geometry = pgo;
        }

        private double Neutralize(double y)
        {
            return 20 - (((y - Min) / (Max - Min)) * 20.0);
        }

        [Design.ZomBDesignable(), Description("Auto adjusts Max and Min of the graph to fit."), Category("Behavior")]
        public bool AutoSize
        {
            get { return (bool)GetValue(AutoSizeProperty); }
            set { SetValue(AutoSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoSizeProperty =
            DependencyProperty.Register("AutoSize", typeof(bool), typeof(DataGraph), new UIPropertyMetadata(false));

        [Design.ZomBDesignable(), Description("The maximum value we are going to get."), Category("Behavior")]
        public double Max
        {
            get { return (double)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Max.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(double), typeof(DataGraph), new UIPropertyMetadata(1.00, new PropertyChangedCallback(MaxUpdated)));

        [Design.ZomBDesignable(), Description("The minimum value we are going to get."), Category("Behavior")]
        public double Min
        {
            get { return (double)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Min.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(double), typeof(DataGraph), new UIPropertyMetadata(-1.0, new PropertyChangedCallback(MinUpdated)));

        static void MaxUpdated(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var self = (o as DataGraph);
            self.scale.MaxY = self.Max;
            self.Regenerate();
        }

        static void MinUpdated(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var self = (o as DataGraph);
            self.scale.MinY = self.Min;
            self.Regenerate();
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //thickness
            return (1 / ((double)value / 50.0));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        [Design.ZomBDesignerVerb("Set range to analog")]
        public void SetRangeToAnalog()
        {
            Min = 0;
            Max = 1024;
            DoubleValue = 0;
        }

        [Design.ZomBDesignerVerb("Set range to volts (0-5)")]
        public void SetRangeToAnalogV()
        {
            Min = 0;
            Max = 5;
        }

        [Design.ZomBDesignerVerb("Set range to degrees")]
        public void SetRangeToDegrees()
        {
            Min = 0;
            Max = 360;
        }

        [Design.ZomBDesignerVerb("Set range to radians")]
        public void SetRangeToRadians()
        {
            Min = 0;
            Max = 2 * Math.PI;
        }

        [Design.ZomBDesignerVerb("Set range to battery")]
        public void SetRangeToBattery()
        {
            Min = 7.0;
            Max = 14.5;
        }

        [Design.ZomBDesignerVerb("Set range to normalized")]
        public void SetRangeToNormalized()
        {
            Min = -1;
            Max = 1;
        }
    }
}
