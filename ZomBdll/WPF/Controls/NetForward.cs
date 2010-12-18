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
using System.Windows;
using System.Windows.Media;
using System451.Communication.Dashboard.Libs.Xbox360Controller;
using System451.Communication.Dashboard.Utils;

namespace System451.Communication.Dashboard.WPF.Controls
{
    /// <summary>
    /// Interaction logic for AnalogMeter.xaml
    /// </summary>
    //[Design.ZomBControl("Network Forwarder", Description = "This will forward stuff to a different computer", IconName = "NetForwardIcon")]
    [Design.ZomBDesignableProperty("Background")]
    public class NetForward : ZomBGLControl
    {
        GamepadState pad = new GamepadState(0);
        static NetForward()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NetForward),
            new FrameworkPropertyMetadata(typeof(NetForward)));
        }

        public NetForward()
        {
            this.Background = Brushes.Wheat;
            this.Width = 20;
            this.Height = 20;
        }

    }
}
