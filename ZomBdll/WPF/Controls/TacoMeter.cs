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
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace System451.Communication.Dashboard.WPF.Controls
{
    /// <summary>
    /// Interaction logic for TacoMeter.xaml
    /// </summary>
    [Design.ZomBControl("Taco Meter", Description = "This shows -1 to 1, useful for eating", IconName = "TacoMeterIcon")]
    [Design.ZomBDesignableProperty("DoubleValue", DisplayName = "Value")]
    public class TacoMeter : ZomBGLControl, IMultiValueConverter
    {
        static TacoMeter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TacoMeter),
            new FrameworkPropertyMetadata(typeof(TacoMeter)));
        }

        public TacoMeter()
        {
            this.Width = 300;
            this.Height = 150;
        }

        [Design.ZomBDesignable(), Description("The maximum value we are going to get."), Category("Behavior")]
        public double Max
        {
            get { return (double)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Max.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(double), typeof(TacoMeter), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));

        [Design.ZomBDesignable(), Description("The minimum value we are going to get."), Category("Behavior")]
        public double Min
        {
            get { return (double)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Min.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(double), typeof(TacoMeter), new FrameworkPropertyMetadata(-1.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public object Convert(object[] value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var ths = value[0] as TacoMeter;
            double v = (double)value[1];
            v = (v - ths.Min) / (ths.Max - ths.Min);
            v = (v - 0.5) * 180.0;
            if (parameter.ToString() == "2")
            {
                if (v < -25)
                    return v - 91;
                return v - 150;
            }
            return v - 90;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        [Design.ZomBDesignerVerb("Set range to analog")]
        public void SetRangeToAnalog()
        {
            Min = 0;
            Max = 1024;
            DoubleValue = 0;
        }

        [Design.ZomBDesignerVerb("Set range to degrees")]
        public void SetRangeToDegrees()
        {
            Min = -90;
            Max = 90;
            DoubleValue = 0;
        }

        [Design.ZomBDesignerVerb("Set range to normalized")]
        public void SetRangeToNormalized()
        {
            Min = -1;
            Max = 1;
            DoubleValue = 0;
        }
    }
}
