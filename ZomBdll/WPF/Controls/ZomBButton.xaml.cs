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
using System.Windows.Controls;

namespace System451.Communication.Dashboard.WPF.Controls
{
    /// <summary>
    /// Interaction logic for ZomBButton.xaml
    /// </summary>
    [Design.ZomBDesignableProperty("Width", Dynamic = true, Category = "Layout")]
    [Design.ZomBDesignableProperty("Height", Dynamic = true, Category = "Layout")]
    [Design.ZomBDesignableProperty("Content", Category = "Appearance")]
    [Design.ZomBControl("ZomBButton", Description = "Useful Button", IconName = "ZomBButtonIcon")]
    public partial class ZomBButton : Button
    {
        public ZomBButton()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.ContextMenu.PlacementTarget = this;
            this.ContextMenu.IsOpen = true;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //Restart
            DashboardDataHub.RestartZomB();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            //Exit
            DashboardDataHub.ExitZomB();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            //DS restart
            DashboardDataHub.RestartDS();
        }
    }
}
