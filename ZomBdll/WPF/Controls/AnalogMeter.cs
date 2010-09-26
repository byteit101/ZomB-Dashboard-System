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
    /// Interaction logic for AnalogMeter.xaml
    /// </summary>
    public class AnalogMeter : ZomBGLControl, IValueConverter
    {
        static AnalogMeter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnalogMeter),
            new FrameworkPropertyMetadata(typeof(AnalogMeter)));
        }

        public AnalogMeter()
        {
            this.Background = Brushes.PaleGreen;
            this.Foreground = Brushes.DarkSlateGray;
            this.SnapsToDevicePixels = true;
            this.BorderBrush = Brushes.Black;
            this.Width = 100;
            this.Height = 50;
        }

        public override void UpdateControl(string value)
        {
            StringValue = value;
            int o;
            int.TryParse(value, out o);
            IntValue = o;
            double d;
            double.TryParse(value, out d);
            DoubleValue = d;
        }

        static void MaxUpdated(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            AnalogMeter am = (o as AnalogMeter);
            am.DoubleValue = Math.Max(am.Min, Math.Min(am.DoubleValue, (double)e.OldValue));
            am.IntValue = (int)am.DoubleValue;
            am.StringValue = am.DoubleValue.ToString();
        }
        static void MinUpdated(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            AnalogMeter am = (o as AnalogMeter);
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
            DependencyProperty.Register("Max", typeof(double), typeof(AnalogMeter), new UIPropertyMetadata(1024.0, new PropertyChangedCallback(AnalogMeter.MaxUpdated)));



        public double Min
        {
            get { return (double)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Min.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(double), typeof(AnalogMeter), new UIPropertyMetadata(0.0, new PropertyChangedCallback(AnalogMeter.MinUpdated)));
       
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Math.Max(-90, Math.Min(((Math.Max(this.Min, Math.Min((double)value, this.Max)) - this.Min )/(this.Max-this.Min)*180) - 90, 90));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


    }
}
