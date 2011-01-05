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
using System.Windows.Forms;
using System451.Communication.Dashboard.Utils;
using System.Reflection;
using System451.Communication.Dashboard.ViZ.Properties;

namespace System451.Communication.Dashboard.ViZ
{
    /// <summary>
    /// Interaction logic for BuildSettings.xaml
    /// </summary>
    public partial class BuildSettings : Window
    {
        private string xaml;
        public BuildSettings(string zaml)
        {
            xaml = zaml;
            InitializeComponent();
            outputLocation.Text = Settings.Default.LastBuildLocation??@"C:\Program Files\FRC Dashboard\Dashboard.exe";
            iconLocation.Text = Settings.Default.LastIconLocation == "" ? System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().CodeBase).Replace("file:\\", "") + "\\Dashboardexe.ico"
                : Settings.Default.LastIconLocation;
            depsCopy.IsChecked = Settings.Default.CopyDLLs;
        }

        private void outputBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.FileName = outputLocation.Text;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                outputLocation.Text = dlg.FileName;
            }
        }

        private void iconBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.FileName = iconLocation.Text;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                iconLocation.Text = dlg.FileName;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Build())
            {
                System.Windows.Forms.MessageBox.Show(@"Success!");
                Settings.Default.CopyDLLs = depsCopy.IsChecked==true;
                Settings.Default.LastBuildLocation = outputLocation.Text;
                Settings.Default.LastIconLocation = iconLocation.Text;
                Settings.Default.Save();
                this.DialogResult = true;
            }
            else
                System.Windows.Forms.MessageBox.Show("Error building exe");
        }

        public bool Build()
        {
            if (depsCopy.IsChecked==true)
                ZomBBuilder.CopyDLLs(System.IO.Path.GetDirectoryName(outputLocation.Text));
            return ZomBBuilder.BuildZomBString(xaml, outputLocation.Text, depsCopy.IsChecked == true, iconLocation.Text);
        }
    }
}
