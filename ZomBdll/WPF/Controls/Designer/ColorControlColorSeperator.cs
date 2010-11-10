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
using System.Windows.Data;
using System.Windows.Media;

namespace System451.Communication.Dashboard.WPF.Controls.Designer
{
    public class ColorControlColorSeperator : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush(Color.FromArgb(System.Convert.ToByte(value[3]), System.Convert.ToByte(value[0]), System.Convert.ToByte(value[1]), System.Convert.ToByte(value[2])));
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            var cr = (value as SolidColorBrush).Color;
            return new object[] { cr.R, cr.G, cr.B, cr.A };
        }
    }
}
