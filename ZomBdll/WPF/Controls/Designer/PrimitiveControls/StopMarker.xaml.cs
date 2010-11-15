using System;
using System.Collections.Generic;
using System.Linq;
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

namespace System451.Communication.Dashboard.WPF.Controls.Designer.PrimitiveControls
{
    /// <summary>
    /// Interaction logic for StopMarker.xaml
    /// </summary>
    public partial class StopMarker : Canvas
    {
        public Color Forecolor
        {
            get
            {
                return (Color)GetValue(ForecolorProperty);
            }
            set
            {
                SetValue(ForecolorProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Foreground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForecolorProperty =
            DependencyProperty.Register("Forecolor", typeof(Color), typeof(StopMarker), new UIPropertyMetadata(Colors.Black));
        
        public StopMarker()
        {
            InitializeComponent();
            (this.Children[0] as Slider).ValueChanged += new RoutedPropertyChangedEventHandler<double>(StopMarker_ValueChanged);
        }

        void StopMarker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ValueChanged != null)
                ValueChanged(this, e);
        }

        public event RoutedPropertyChangedEventHandler<double> ValueChanged;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

        }

        public double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            Slider.ValueProperty.AddOwner(typeof(StopMarker));


    }
}
