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
using System.Windows.Input;
using System451.Communication.Dashboard.ViZ.Properties;

namespace System451.Communication.Dashboard.ViZ
{
    /// <summary>
    /// Interaction logic for Toolbox.xaml
    /// </summary>
    public partial class Toolbox : Window
    {
        public Toolbox()
        {
            InitializeComponent();
            DockCheck.IsChecked = Settings.Default.EmbeddedTbx;
        }

        private void PropScroller_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            PropScroller.ScrollToHorizontalOffset(PropScroller.HorizontalOffset - e.Delta);
            e.Handled = true;
        }

        private void CommandBinding_Deploy_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Designer.getDesigner().CommandBinding_Deploy_Executed(sender, e);
        }

        private void CommandBinding_Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Designer.getDesigner().CommandBinding_Open_Executed(sender, e);
        }

        private void CommandBinding_Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Designer.getDesigner().CommandBinding_Save_Executed(sender, e);
        }

        private void CommandBinding_Play_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Designer.getDesigner().CommandBinding_Play_Executed(sender, e);
        }

        private void DockCheck_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.EmbeddedTbx = DockCheck.IsChecked;
            Settings.Default.Save();
        }

        private void DockCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.Default.EmbeddedTbx = DockCheck.IsChecked;
            Settings.Default.Save();
        }
    }
}
