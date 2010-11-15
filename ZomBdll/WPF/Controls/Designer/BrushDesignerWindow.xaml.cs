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
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using System451.Communication.Dashboard.WPF.Controls.Designer.PrimitiveControls;
using System.Windows.Input;
using System.Windows.Data;

namespace System451.Communication.Dashboard.WPF.Controls.Designer
{
    /// <summary>
    /// Interaction logic for ColorDesignerWindow.xaml
    /// </summary>
    public partial class BrushDesignerWindow : Window
    {
        Brush b;
        enum Mode
        {
            Solid, LinearGradient
        };
        Mode cmode = Mode.Solid;
        int gradIndex = 0;
        Action<Brush> setv;
        List<StopMarker> contrls = new List<StopMarker>();
        public BrushDesignerWindow(object obj, PropertyInfo prop)
        {
            InitializeComponent();
            b = prop.GetValue(obj, null) as Brush;
            setv = (x) => prop.SetValue(obj, x, null);
            if (b.IsFrozen)
            {
                b = b.Clone();
                setv(b);
            }
            if (b is SolidColorBrush)
            {
                cmode = Mode.Solid;
            }
            else if (b is LinearGradientBrush)
            {
                cmode = Mode.LinearGradient;
            }
            contrls.Add(StopM0);
            contrls.Add(StopM1);
            foreach (var item in contrls)
            {
                item.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(Stop_Click);
                item.ValueChanged += new RoutedPropertyChangedEventHandler<double>(Stop_ValueChanged);
            }
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            ColorPicker.ColorChanged += ColorControl_ColorChanged;
            if (cmode == Mode.Solid)
            {
                ColorPicker.Color = (b as SolidColorBrush).Color;
            }
            else if (cmode == Mode.LinearGradient)
            {
                GradientGrid.Background = b;
                Stop_Click(contrls[0], null);
                tc.SelectedItem = LinGradTab;
            }
        }

        private void ColorControl_ColorChanged(object sender, RoutedEventArgs e)
        {
            if (cmode == Mode.Solid)
            {
                (b as SolidColorBrush).Color = ColorPicker.Color;
            }
            else if (cmode == Mode.LinearGradient)
            {
                (b as LinearGradientBrush).GradientStops[gradIndex].Color = ColorPicker.Color;
                contrls[gradIndex].Forecolor = ColorPicker.Color;
            }
        }

        void Stop_Click(object sender, MouseButtonEventArgs e)
        {
            gradIndex = contrls.IndexOf(sender as StopMarker);
            ColorPicker.Color = (b as LinearGradientBrush).GradientStops[gradIndex].Color;
        }

        void Stop_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            (b as LinearGradientBrush).GradientStops[gradIndex].Offset = (sender as StopMarker).Value;
        }

        private void TabItem_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            if (b is LinearGradientBrush)
            {
                b = new SolidColorBrush((b as LinearGradientBrush).GradientStops[0].Color);
                setv(b);
                cmode = Mode.Solid;
            }
        }

        private void TabItemlg_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            if (b is SolidColorBrush)
            {
                b = new LinearGradientBrush((b as SolidColorBrush).Color, (b as SolidColorBrush).Color, 0);
                setv(b);
                GradientGrid.Background = b;
                cmode = Mode.LinearGradient;
            }
        }

        private void AddStopBtn_Click(object sender, RoutedEventArgs e)
        {
            AddStop(gradIndex);
        }

        private void AddStop(int indx)
        {
            int pid = 0;
            
        }
    }

    public class StopMarkerPositionConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((double)value) + 14;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
