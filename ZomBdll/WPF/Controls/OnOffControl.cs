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
    [TemplatePart(Name = "PART_Rect", Type = typeof(Ellipse))]
    public class OnOffControl : ZomBGLControl
    {
        Ellipse PART_Rect;
        static OnOffControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OnOffControl),
            new FrameworkPropertyMetadata(typeof(OnOffControl)));
        }

        public OnOffControl()
        {
            this.Background = Brushes.Red;
            this.Foreground = Brushes.Green;
            this.SnapsToDevicePixels = true;
            this.Width = 50;
            this.Height = 50;
            BoolValueProperty.OverrideMetadata(typeof(OnOffControl), new FrameworkPropertyMetadata(false, boolchange));
        }

        private void boolchange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (PART_Rect != null)
            {
                PART_Rect.Fill = BoolValue ? Foreground : Background;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_Rect = base.GetTemplateChild("PART_Rect") as Ellipse;
            boolchange(null, new DependencyPropertyChangedEventArgs());
        }
    }
}
