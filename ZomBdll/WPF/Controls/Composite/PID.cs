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
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.IO;
using System.Text;

namespace System451.Communication.Dashboard.WPF.Controls
{
    /// <summary>
    /// Interaction logic for ValueMeter.xaml
    /// </summary>
    [Design.ZomBControl("PID Controller",
        Description = "This is a group of controls as a single PID control",
        IconName = "PIDIcon",
        TypeHints = ZomBDataTypeHint.Lookup)]
    [ZomBComposite("PIDController")]
    [ZomBComposite("PID Controller")]
    public class PIDComposite : IZomBCompositeDescriptor
    {
        private static string markup = Properties.Resources.PIDCompositeMarkup;
        public PIDComposite()
        {

        }


        #region IZomBCompositeDescriptor Members

        public string Name
        {
            get { return "PID Controller"; }
        }

        public FrameworkElement InflateControls()
        {
            return XamlReader.Load(new MemoryStream(UTF8Encoding.UTF8.GetBytes(markup))) as Grid;
        }

        #endregion
    }
}
