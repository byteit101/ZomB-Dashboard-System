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
using System.Windows;
using System.Windows.Media;

namespace System451.Communication.Dashboard.WPF.Controls
{
    /// <summary>
    /// Interaction logic for StatusLabel.xaml
    /// </summary>
    [Design.ZomBControl("Status Label", Description = "This shows a raw value for a control, and is a nice label", IconName = "StatusLabelIcon")]
    [Design.ZomBDesignableProperty("Foreground")]
    [Design.ZomBDesignableProperty("StringValue", DisplayName = "Text")]
    [Design.ZomBDesignableProperty("FontSize")]
    public class StatusLabel : ZomBGLControl
    {
        static StatusLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StatusLabel),
            new FrameworkPropertyMetadata(typeof(StatusLabel)));
            StringValueProperty.OverrideMetadata(typeof(StatusLabel), new FrameworkPropertyMetadata("Label"));
        }

        public StatusLabel()
        {
            this.Foreground = Brushes.Black;
            this.StringValue = "Label";
        }

        public override void UpdateControl(ZomBDataObject value)
        {
            StringValue = value;
        }
    }
}
