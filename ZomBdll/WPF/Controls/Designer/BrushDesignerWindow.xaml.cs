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
                LinGradient.Background = b;
                left_gradclick(sender, null);
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
            }
        }

        private void left_gradclick(object sender, RoutedEventArgs e)
        {
            gradIndex = 0;
            ColorPicker.Color = (b as LinearGradientBrush).GradientStops[0].Color;
        }

        private void right_gradclick(object sender, RoutedEventArgs e)
        {
            gradIndex = 1;
            ColorPicker.Color = (b as LinearGradientBrush).GradientStops[1].Color;
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
                LinGradient.Background = b;
                cmode = Mode.LinearGradient;
            }
        }
    }
}
