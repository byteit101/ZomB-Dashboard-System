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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System451.Communication.Dashboard.WPF.Design.DesignUtils;

namespace System451.Communication.Dashboard.WPF.Controls.Designer
{
    /// <summary>
    /// Interaction logic for CameraDesignerWindow.xaml
    /// </summary>
    public partial class CameraDesignerWindow : Window
    {
        CameraView Object { get; set; }
        ZomBControlCollection zcc;
        int lastid;
        bool haltevents = false;

        public CameraDesignerWindow(CameraView obj)
        {
            Object = obj;
            zcc = obj.GetControls();
            lastid = zcc.Count;
            InitializeComponent();
            if (zcc.Count > 0)
            {
                PopulateList();
            }
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            zcc.Add(new CameraTarget());
            (zcc.Last() as CameraTarget).ControlName = "target" + ++lastid;
            (zcc.Last() as CameraTarget).Border = new Pen(Brushes.Lime, (1.0 / ((Object.Width + Object.Height) / 2.0)));
            (zcc.Last() as CameraTarget).Fill = new SolidColorBrush(Color.FromArgb(127, 0, 255, 0));
            PopulateList();
            ListItems.SelectedIndex = zcc.Count - 1;
        }

        private void PopulateList()
        {
            ListItems.Items.Clear();
            foreach (var item in zcc)
            {
                ListItems.Items.Add(item as CameraTarget);
            }
        }

        private void removeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ListItems.SelectedItem != null)
            {
                zcc.Remove(ListItems.SelectedItem as IZomBControl);
                PopulateList();
            }
        }

        private void ListItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (haltevents)
                return;
            try
            {
                haltevents = true;
                nameBox.Text = (ListItems.SelectedItem as CameraTarget).ControlName;
                fillsp.Children.Clear();
                fillsp.Children.Add(DesignUtils.GetDesignerField(ListItems.SelectedItem, typeof(CameraTarget).GetProperty("Fill")));
                pensp.Children.Clear();
                pensp.Children.Add(DesignUtils.GetDesignerField((ListItems.SelectedItem as CameraTarget).Border, typeof(Pen).GetProperty("Brush")));
                WidthBox.Text = ((ListItems.SelectedItem as CameraTarget).Border.Thickness * ((Object.Width + Object.Height) / 2.0)).ToString();
                haltevents = false;
            }
            catch { nameBox.Text = ""; haltevents = false; }
        }

        private void nameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (haltevents)
                return;
            if (ListItems.SelectedItem != null)
            {
                haltevents = true;
                int si = ListItems.SelectedIndex;
                (zcc[zcc.IndexOf(ListItems.SelectedItem as CameraTarget)] as CameraTarget).ControlName = nameBox.Text;
                PopulateList();
                ListItems.SelectedIndex = si;
                haltevents = false;
            }
        }

        private void WidthBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (haltevents)
                return;
            if (ListItems.SelectedItem != null)
            {
                try
                {
                    (zcc[zcc.IndexOf(ListItems.SelectedItem as CameraTarget)] as CameraTarget).Border.Thickness = (double.Parse(WidthBox.Text) / ((Object.Width + Object.Height) / 2.0));
                }
                catch { }
            }
        }
    }
}
