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
using System.ComponentModel;
using System.Windows.Data;

namespace System451.Communication.Dashboard.ViZ
{
    class StringValueConverter : IValueConverter
    {
        public StringValueConverter()
        {

        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType == typeof(double))
                return double.Parse(value.ToString());
            if (targetType == typeof(int))
                return int.Parse(value.ToString());

            var tc = targetType.GetCustomAttributes(typeof(TypeConverterAttribute), true);
            if (tc.Length > 0)
            {
                string tcname = (tc[0] as TypeConverterAttribute).ConverterTypeName;
                TypeConverter tcv = System.Type.GetType(tcname).GetConstructor(Type.EmptyTypes).Invoke(null) as TypeConverter;
                try
                {
                    return tcv.ConvertFrom(null, System.Globalization.CultureInfo.CurrentCulture, value);
                }
                catch
                {
                    return null;
                }
            }
            return value;
        }

        #endregion
    }
}
