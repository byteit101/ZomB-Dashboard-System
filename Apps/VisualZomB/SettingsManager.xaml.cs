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
using System451.Communication.Dashboard.ViZ.Properties;
using Microsoft.Win32;

namespace System451.Communication.Dashboard.ViZ
{
    /// <summary>
    /// Interaction logic for SettingsManager.xaml
    /// </summary>
    public partial class SettingsManager : Window
    {
        public SettingsManager()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            VizGenChkToolbox.IsChecked = Settings.Default.EmbeddedTbx;
            VizGenTxtTeamNumber.Text = Settings.Default.LastTeamNumber;

            VizBldChkCopyDll.IsChecked = Settings.Default.CopyDLLs;
            VizBldTxtSaveLocation.Text = Settings.Default.LastBuildLocation;
            VizBldTxtIconLocation.Text = Settings.Default.LastIconLocation;

            ZbGenChkTitlebar.IsChecked = (int)Registry.LocalMachine.OpenSubKey(@"Software\ZomB").GetValue("DriverDisable", 0) == 0;
            ZbGenChkSingleton.IsChecked = (int)Registry.LocalMachine.OpenSubKey(@"Software\ZomB").GetValue("Singleton", 0) == 1;
        }

        private void SaveSettings()
        {
            Settings.Default.EmbeddedTbx = VizGenChkToolbox.IsChecked == true;
            Settings.Default.LastTeamNumber = VizGenTxtTeamNumber.Text;

            Settings.Default.CopyDLLs = VizBldChkCopyDll.IsChecked == true;
            Settings.Default.LastBuildLocation = VizBldTxtSaveLocation.Text;
            Settings.Default.LastIconLocation = VizBldTxtIconLocation.Text;
            Settings.Default.Save();

            Registry.LocalMachine.OpenSubKey(@"Software\ZomB",true).SetValue("DriverDisable", ZbGenChkTitlebar.IsChecked == true ? 0 : 1);
            Registry.LocalMachine.OpenSubKey(@"Software\ZomB",true).SetValue("Singleton", ZbGenChkSingleton.IsChecked == true ? 1 : 0);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
        }
    }
}
