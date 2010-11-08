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
    /// Interaction logic for TacoMeter.xaml
    /// </summary>
    [Design.ZomBControl("Taco Meter", Description = "This shows -1 to 1, useful for eating", IconName="TacoMeterIcon")]
    public class TacoMeter : ZomBGLControl, IValueConverter
    {
        static TacoMeter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TacoMeter),
            new FrameworkPropertyMetadata(typeof(TacoMeter)));
        }

        public TacoMeter()
        {
            this.Width = 150;
            this.Height = 100;
        }


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter.ToString() == "2")
            {
                if (((double)value) * 90<-25)
                    return ((double)value) * 90 - 91;
                return ((double)value) * 90 - 150;
            }
            return ((double)value) * 90 - 90;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
