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
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace System451.Communication.Dashboard.WPF.Controls
{
    /// <summary>
    /// Interaction logic for ValueMeter.xaml
    /// </summary>
    [Design.ZomBControl("Value Meter", Description = "This shows a -1 to 1 value and is useful for total parts things (like battery)", IconName = "ValueMeterIcon")]
    [Design.ZomBDesignableProperty("Foreground")]
    [Design.ZomBDesignableProperty("Background")]
    [Design.ZomBDesignableProperty("BorderBrush")]
    [Design.ZomBDesignableProperty("BorderThickness")]
    [Design.ZomBDesignableProperty("DoubleValue", DisplayName = "Value")]
    public class ValueMeter : ZomBGLControl, IMultiValueConverter, IZomBDataControl
    {
        static ValueMeter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ValueMeter),
            new FrameworkPropertyMetadata(typeof(ValueMeter)));
        }

        public ValueMeter()
        {
            this.Width = 50;
            this.Height = 100;
            this.Background = Brushes.White;
            BorderBrush = Brushes.Black;
            Foreground = Brushes.Peru;
            BorderThickness = new Thickness(1);
        }

        [Design.ZomBDesignable(), Description("The maximum value we are going to get."), Category("Behavior")]
        public double Max
        {
            get { return (double)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Max.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(double), typeof(ValueMeter), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));


        [Design.ZomBDesignable(), Description("The minimum value we are going to get."), Category("Behavior")]
        public double Min
        {
            get { return (double)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Min.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(double), typeof(ValueMeter), new FrameworkPropertyMetadata(-1.0, FrameworkPropertyMetadataOptions.AffectsRender));


        #region IValueConverter Members

        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            var ths = value[0] as ValueMeter;
            return Math.Min(Math.Max((((double)value[1] - ths.Min) / (ths.Max - ths.Min)), 0), 1);
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region IZomBDataControl Members

        public event ZomBDataControlUpdatedEventHandler DataUpdated;

        bool dce = false;
        public bool DataControlEnabled
        {
            get
            {
                return dce;
            }
            set
            {
                if (dce != value)
                {
                    dce = value;
                    if (dce)
                    {
                        this.MouseLeftButtonUp += AlertControl_MouseLeftButtonUp;
                        this.MouseMove += ValueMeter_MouseMove;
                        this.MouseLeftButtonDown += ValueMeter_MouseLeftButtonDown;
                    }
                    else
                    {
                        this.MouseLeftButtonUp -= AlertControl_MouseLeftButtonUp;
                        this.MouseMove -= ValueMeter_MouseMove;
                        this.MouseLeftButtonDown -= ValueMeter_MouseLeftButtonDown;
                    }
                }
            }
        }

        bool draggin = false;

        void ValueMeter_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            draggin = true;
            this.CaptureMouse();
        }

        void ValueMeter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (draggin)
            {
                DoubleValue = Math.Max(Min, Math.Min(Max, (1 - e.GetPosition(this).Y / this.ActualHeight) * (this.Max - this.Min) + this.Min));
                if (DataUpdated != null)
                    DataUpdated(this, new ZomBDataControlUpdatedEventArgs(ControlName, DoubleValue.ToString()));
            }
        }

        void AlertControl_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            draggin = false;
            ReleaseMouseCapture();
        }

        #endregion
    }
}
