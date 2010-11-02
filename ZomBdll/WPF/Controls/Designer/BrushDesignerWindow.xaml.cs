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
using System.Reflection;

namespace System451.Communication.Dashboard.WPF.Controls.Designer
{
    /// <summary>
    /// Interaction logic for ColorDesignerWindow.xaml
    /// </summary>
    public partial class BrushDesignerWindow : Window
    {
        Brush b;
        public BrushDesignerWindow(object obj, PropertyInfo prop)
        {
            InitializeComponent();
            b = prop.GetValue(obj, null) as Brush;
            if (b.IsFrozen)
            {
                b = b.Clone();
                prop.SetValue(obj, b, null);
            }
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            ColorPicker.ColorChanged += ColorControl_ColorChanged;
            if (b is SolidColorBrush)
                ColorPicker.Color = (b as SolidColorBrush).Color;
        }

        private void ColorControl_ColorChanged(object sender, RoutedEventArgs e)
        {
            if (true)
            {
                (b as SolidColorBrush).Color = ColorPicker.Color;
            }
        }
    }
}
