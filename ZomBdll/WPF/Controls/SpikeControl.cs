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
    [TemplatePart(Name = "PART_Rect", Type = typeof(Rectangle))]
    public class SpikeControl : ZomBGLControl
    {
        Rectangle PART_Rect;
        static SpikeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SpikeControl),
            new FrameworkPropertyMetadata(typeof(SpikeControl)));
        }

        public SpikeControl()
        {
            this.Background = Brushes.Red;
            this.Foreground = Brushes.Green;
            this.BorderBrush = Brushes.Black;
            this.SnapsToDevicePixels = true;
            this.Width = 50;
            this.Height = 50;
        }

        private void boolchange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (PART_Rect != null)
            {
                PART_Rect.Fill = Value== SpikePositions.Forward ? Foreground : Value== SpikePositions.Reverse?Background:BorderBrush;
            }
        }

        private static void boolchanges(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((SpikeControl)sender).boolchange(sender, e);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_Rect = base.GetTemplateChild("PART_Rect") as Rectangle;
            boolchange(null, new DependencyPropertyChangedEventArgs());
        }


        public override void UpdateControl(string value)
        {
            base.UpdateControl(value);
            Value = (SpikePositions)Enum.Parse(typeof(SpikePositions), value);
        }

        public SpikePositions Value
        {
            get { return (SpikePositions)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(SpikePositions), typeof(SpikeControl),
            new FrameworkPropertyMetadata(SpikePositions.Off, boolchanges));

       
    }
}
