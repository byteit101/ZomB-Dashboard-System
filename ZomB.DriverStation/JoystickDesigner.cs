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
using System451.Communication.Dashboard.WPF.Controls.Designers;
using System.Windows;
using System.Windows.Controls;

namespace System451.Communication.Dashboard.Net.DriverStation
{
    public class JoystickDesigner : DesignerBase
    {
        public override void Initialize(object obj, System.Reflection.PropertyInfo property)
        {
            base.Initialize(obj, property);
        }
        public override FrameworkElement GetProperyField()
        {
            var btn = new Button();
            btn.Content = "Set Joystick Properties";
            btn.Click += new RoutedEventHandler(btn_Click);
            return btn;
        }

        void btn_Click(object sender, RoutedEventArgs e)
        {
            new JoystickDesignerWindow { joy = GetVaueAsType<Joystick>() }.ShowDialog();
        }

        public override bool IsDefaultValue()
        {
            return false;
        }

        public override bool IsExpanded()
        {
            return true;
        }

        public override string GetValue()
        {
            var j = GetVaueAsType<Joystick>();
            var sb = new StringBuilder("<ZomBDS:Joystick XSource=\"");
            sb.Append(j.XSource);
            sb.Append("\" YSource=\"");
            sb.Append(j.YSource);
            sb.Append("\" />");
            return sb.ToString();
        }
    }
}
