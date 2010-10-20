﻿/*
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
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Data;
using System;

namespace System451.Communication.Dashboard.WPF.Controls
{
    /// <summary>
    /// Interaction logic for ValueMeter.xaml
    /// </summary>
    [Design.ZomBControl("Value Meter", Description = "This shows a -1 to 1 value and is useful for total parts things (like battery)")]
    public class ValueMeter : ZomBGLControl, IValueConverter
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

        #region IValueConverter Members

        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Math.Min(Math.Max((((double)value) + 1) / 2.0, 0), 1);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
