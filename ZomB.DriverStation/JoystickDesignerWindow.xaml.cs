/*
 * ZomB Dashboard System <http://firstforge.wpi.edu/sf/projects/zombdashboard>
 * Copyright (C) 2011, Patrick Plenefisch and FIRST Robotics Team 451 "The Cat Attack"
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
using System.Windows.Controls;

namespace System451.Communication.Dashboard.Net.DriverStation
{
    /// <summary>
    /// Interaction logic for JoystickDesignerWindow.xaml
    /// </summary>
    public partial class JoystickDesignerWindow : Window
    {
        public Joystick joy { get; set; }
        public JoystickDesignerWindow()
        {
            InitializeComponent();
        }

        public JoystickDesignerWindow InitConfig()
        {
            XBox.SetConfig(joy.XSource);
            YBox.SetConfig(joy.YSource);
            ZBox.SetConfig(joy.ZSource);
            return this;
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            joy.XSource = XBox.GetConfigString();
            joy.YSource = YBox.GetConfigString();
            joy.ZSource = ZBox.GetConfigString();
            //if (xsourcebox.SelectedItem.ToString().Contains("Virtual"))
            //    joy.XSource = xsourcebox.SelectedItem.ToString().Replace(" ", "") + "@" + xdetailbox.SelectedItem.ToString();
            //else
            //    joy.XSource = xsourcebox.SelectedItem.ToString().Replace(" ", "") + xdetailbox.SelectedItem.ToString();
            this.DialogResult = true;
        }

        private void detail_changed(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
