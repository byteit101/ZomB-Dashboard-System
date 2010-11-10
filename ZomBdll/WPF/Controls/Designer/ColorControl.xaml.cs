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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace System451.Communication.Dashboard.WPF.Controls.Designer
{
    /// <summary>
    /// Interaction logic for ColorControl.xaml
    /// </summary>
    public partial class ColorControl : UserControl, IValueConverter
    {
        bool swallowingEvents = false;
        public ColorControl()
        {
            InitializeComponent();

            Binding b = new Binding("Fill");
            b.Source = colorPrev;
            b.Mode = BindingMode.TwoWay;
            b.Converter = this;
            SetBinding(ColorProperty, b);
        }

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(ColorControl), new UIPropertyMetadata(new Color(), pcc));



        // Provide CLR accessors for the event
        public event RoutedEventHandler ColorChanged
        {
            add { AddHandler(ColorChangedEvent, value); }
            remove { RemoveHandler(ColorChangedEvent, value); }
        }

        // Using a RoutedEvent
        public static readonly RoutedEvent ColorChangedEvent = EventManager.RegisterRoutedEvent(
            "ColorChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ColorControl));

        static void pcc(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if ((o as ColorControl).swallowingEvents)
                return;
            if (e.NewValue != e.OldValue)
                (o as ColorControl).MoveSliders();
            (o as ColorControl).RaiseEvent(new RoutedEventArgs(ColorChangedEvent, o));
        }

        private void MoveSliders()
        {
            Color g = Color;
            swallowingEvents = true;
            slr.Value = g.R;
            slg.Value = g.G;
            slb.Value = g.B;
            sla.Value = g.A;
            swallowingEvents = false;
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            return (value as SolidColorBrush).Color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            return new SolidColorBrush((System.Windows.Media.Color)value);
        }

        #endregion
    }
}
