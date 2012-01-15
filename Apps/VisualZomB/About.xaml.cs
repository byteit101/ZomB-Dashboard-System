/*
 * ZomB Dashboard System <http://firstforge.wpi.edu/sf/projects/zombdashboard>
 * Copyright (C) 2012, Patrick Plenefisch and FIRST Robotics Team 451 "The Cat Attack"
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace System451.Communication.Dashboard.ViZ
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            ReleaseBlock.Text = ZVersionMgr.ReleaseDateString;
            VersionBlock.Text = "Version " + ZVersionMgr.FullNumber + " - " + ZVersionMgr.CodeName;
            //#ifdef Holloween
            if (DateTime.Today.Day == 31 && DateTime.Today.Month == 10)
            {
                GradientBlue.Color = Colors.Red;
                ViZFullLogoDrawing.Brush = Brushes.Red;
                ViZLogoDrawing.Brush = Brushes.Red;
            }
            //#endif /*Holloween*/
        }

        private void Credits_Click(object sender, RoutedEventArgs e)
        {
            new Credits().ShowDialog();
        }
    }
}
