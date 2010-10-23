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
    /// Interaction logic for SpeedMeter.xaml
    /// </summary>
    [Design.ZomBControl("Speed Meter", Description = "This shows -1 to 1, useful for any thing, but helpful for motors and joystick inputs", IconName="SpeedMeterIcon")]
    public class SpeedMeter : ZomBGLControl, IValueConverter
    {
        static SpeedMeter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SpeedMeter),
            new FrameworkPropertyMetadata(typeof(SpeedMeter)));
        }

        public SpeedMeter()
        {
            this.Background = Brushes.Tan;
            this.Foreground = new LinearGradientBrush(Colors.Red, Colors.Green, 0.0);
            this.SnapsToDevicePixels = true;
            this.Width = 100;
            this.Height = 50;
        }

        static void MaxUpdated(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SpeedMeter am = (o as SpeedMeter);
            am.DoubleValue = Math.Max(am.Min, Math.Min(am.DoubleValue, (double)e.OldValue));
            am.IntValue = (int)am.DoubleValue;
            am.StringValue = am.DoubleValue.ToString();
        }
        static void MinUpdated(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SpeedMeter am = (o as SpeedMeter);
            am.DoubleValue = Math.Max((double)e.OldValue, Math.Min(am.DoubleValue, am.Max));
            am.IntValue = (int)am.DoubleValue;
            am.StringValue = am.DoubleValue.ToString();
        }

        public double Max
        {
            get { return (double)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Max.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(double), typeof(SpeedMeter), new UIPropertyMetadata(1.0, new PropertyChangedCallback(SpeedMeter.MaxUpdated)));



        public double Min
        {
            get { return (double)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Min.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(double), typeof(SpeedMeter), new UIPropertyMetadata(-1.0, new PropertyChangedCallback(SpeedMeter.MinUpdated)));

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var r= Math.Max(-90, Math.Min(((Math.Max(this.Min, Math.Min((double)value, this.Max)) - this.Min) / (this.Max - this.Min) * 180) - 90, 90));
            if (parameter.ToString() == "l")
            {
                if (r <= 0)
                {
                    return -90;
                }
                else
                {
                    return r / 2.5;
                }
            }
            else if (parameter.ToString() == "l2")
            {
                if (r <= 0)
                {
                    return -90;
                }
                else
                {
                    return r / 6;
                }
            }
            else if (parameter.ToString() == "r")
            {
                if (r >= 0)
                {
                    return 90;
                }
                else
                {
                    return r / 2.5;
                }
            }
            else if (parameter.ToString() == "r2")
            {
                if (r >= 0)
                {
                    return 90;
                }
                else
                {
                    return r / 6;
                }
            }
            else if (parameter.ToString() == "sh")
            {
                if (r > 0)
                {
                    return -50;
                }
                else
                {
                    return 0;
                }
            }
            else
                return r;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
