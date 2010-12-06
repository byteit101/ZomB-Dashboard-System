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
using System.Windows.Controls;
using System.Windows.Shapes;

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
    [TemplatePart(Name = "PART_Background", Type = typeof(Border))]
    [TemplatePart(Name = "PART_Foreground", Type = typeof(Shape))]
    public class ValueMeter : ZomBGLControl, IMultiValueConverter, IZomBDataControl, IValueConverter
    {
        Border back = null;
        Shape fore = null;
        bool InHigh = false, InLow = false;

        static ValueMeter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ValueMeter),
            new FrameworkPropertyMetadata(typeof(ValueMeter)));
            DoubleValueProperty.OverrideMetadata(typeof(ValueMeter), new UIPropertyMetadata(ValueChanged));
        }

        public ValueMeter()
        {
            this.Width = 50;
            this.Height = 100;
            this.Background = Brushes.White;
            BorderBrush = Brushes.Black;
            Foreground = Brushes.Peru;
            BarWidth = 5.0;
            BarBrush = Brushes.RoyalBlue;
            BorderThickness = new Thickness(1);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            back = base.GetTemplateChild("PART_Background") as Border;
            fore = base.GetTemplateChild("PART_Foreground") as Shape;
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


        [Design.ZomBDesignable(), Description("The threshold before we show the high color."), Category("Behavior")]
        public double HighThreshold
        {
            get { return (double)GetValue(HighThresholdProperty); }
            set { SetValue(HighThresholdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighThreshold.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighThresholdProperty =
            DependencyProperty.Register("HighThreshold", typeof(double), typeof(ValueMeter), new UIPropertyMetadata(1.0));

        [Design.ZomBDesignable(), Description("The color we show when we are past the high threshold."), Category("Behavior")]
        public Brush HighThresholdBrush
        {
            get { return (Brush)GetValue(HighThresholdBrushProperty); }
            set { SetValue(HighThresholdBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighThresholdBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighThresholdBrushProperty =
            DependencyProperty.Register("HighThresholdBrush", typeof(Brush), typeof(ValueMeter), new UIPropertyMetadata(Brushes.Red));

        [Design.ZomBDesignable(), Description("The threshold before we show the low color."), Category("Behavior")]
        public double LowThreshold
        {
            get { return (double)GetValue(LowThresholdProperty); }
            set { SetValue(LowThresholdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighThreshold.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LowThresholdProperty =
            DependencyProperty.Register("LowThreshold", typeof(double), typeof(ValueMeter), new UIPropertyMetadata(-1.0));

        [Design.ZomBDesignable(), Description("The color we show when we are past the low threshold."), Category("Behavior")]
        public Brush LowThresholdBrush
        {
            get { return (Brush)GetValue(LowThresholdBrushProperty); }
            set { SetValue(LowThresholdBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighThresholdBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LowThresholdBrushProperty =
            DependencyProperty.Register("LowThresholdBrush", typeof(Brush), typeof(ValueMeter), new UIPropertyMetadata(Brushes.Green));
        
        public bool IsLabelVisible
        {
            get { return (bool)GetValue(IsLabelVisibleProperty); }
            set { SetValue(IsLabelVisibleProperty, value); }
        }

        [Design.ZomBDesignable(), Description("The minimum value we are going to get."), Category("Behavior")]
        public double Min
        {
            get { return (double)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLabelVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLabelVisibleProperty =
            DependencyProperty.Register("IsLabelVisible", typeof(bool), typeof(ValueMeter), new UIPropertyMetadata(true));

        
        // Using a DependencyProperty as the backing store for Min.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(double), typeof(ValueMeter), new FrameworkPropertyMetadata(-1.0, FrameworkPropertyMetadataOptions.AffectsRender));


        [Design.ZomBDesignable(), Description("The width of the bar. Default 5, use 0 for no bar"), Category("Appearance")]
        public double BarWidth
        {
            get { return (double)GetValue(BarWidthProperty); }
            set { SetValue(BarWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BarWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BarWidthProperty =
            DependencyProperty.Register("BarWidth", typeof(double), typeof(ValueMeter), new FrameworkPropertyMetadata(5.0, FrameworkPropertyMetadataOptions.AffectsRender));


        [Design.ZomBDesignable(), Description("The color of the bar."), Category("Appearance")]
        public Brush BarBrush
        {
            get { return (Brush)GetValue(BarBrushProperty); }
            set { SetValue(BarBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BarBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BarBrushProperty =
            DependencyProperty.Register("BarBrush", typeof(Brush), typeof(ValueMeter), new UIPropertyMetadata(Brushes.RoyalBlue));


        static void ValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var t = o as ValueMeter;
            if (t.back == null || t.fore == null)
                return;
            if (t.DoubleValue >= t.HighThreshold)
            {
                if (!t.InHigh)
                {
                    var b = new Binding();
                    b.Source = t;
                    b.Path = new PropertyPath(HighThresholdBrushProperty);
                    t.fore.SetBinding(Shape.FillProperty, b);
                    t.InHigh = true;
                }
            }
            else
            {
                if (t.InHigh)
                {
                    var b = new Binding();
                    b.Source = t;
                    b.Path = new PropertyPath(ForegroundProperty);
                    t.fore.SetBinding(Shape.FillProperty, b);
                    t.InHigh = false;
                }
            }
            if (t.DoubleValue <= t.LowThreshold)
            {
                if (!t.InLow)
                {
                    var b = new Binding();
                    b.Source = t;
                    b.Path = new PropertyPath(LowThresholdBrushProperty);
                    t.back.SetBinding(Border.BackgroundProperty, b);
                    t.InLow = true;
                }
            }
            else
            {
                if (t.InLow)
                {
                    var b = new Binding();
                    b.Source = t;
                    b.Path = new PropertyPath(BackgroundProperty);
                    t.back.SetBinding(Border.BackgroundProperty, b);
                    t.InLow = false;
                }
            }
        }

        [Design.ZomBDesignerVerb("Set to battery defaults")]
        public void SetRangeToBattery()
        {
            Min = 7.0;
            Max = 14.5;
            HighThreshold = 13.25;
            LowThreshold = 9.5;
            DoubleValue = 12.0;
            HighThresholdBrush = Brushes.Orange;
            LowThresholdBrush = Brushes.Red;
            BarWidth = 0;
        }

        [Design.ZomBDesignerVerb("Set to normalized")]
        public void SetRangeToNormalized()
        {
            Min = -1.0;
            Max = 1;
            HighThreshold = 1;
            LowThreshold = -1;
            DoubleValue = 0;
            HighThresholdBrush = Brushes.Red;
            LowThresholdBrush = Brushes.Green;
            BarWidth = 5;
        }


        #region IValueConverter Members

        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            var ths = value[0] as ValueMeter;
            if (parameter != null && parameter.ToString() == "Mov")
                return 1 - Math.Min(Math.Max((((double)value[1] - ths.Min) / (ths.Max - ths.Min)), 0), 1) - (ths.BarWidth / 100);
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

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(Visibility))
                return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
            return ((double)value).ToString("F");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
