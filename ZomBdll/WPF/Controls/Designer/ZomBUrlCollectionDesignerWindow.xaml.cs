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
using System.Windows.Controls.Primitives;
using System451.Communication.Dashboard.Net;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace System451.Communication.Dashboard.WPF.Controls.Designer
{
    public partial class ZomBUrlCollectionDesignerWindow : Window
    {
        Collection<string> Object { get; set; }
        int Team { get; set; }
        int lastid;
        bool haltevents = false;

        public ZomBUrlCollectionDesignerWindow(Collection<string> obj, int team)
        {
            Object = obj;
            Team=team;
            lastid = Object.Count;
            InitializeComponent();
            if (Object.Count > 0)
            {
                PopulateList();
            }
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            Object.Add("zomb://." + Team + "/DBPkt");
            PopulateList();
            ListItems.SelectedIndex = Object.Count - 1;
        }

        private void PopulateList()
        {
            ListItems.Items.Clear();
            foreach (var item in Object)
            {
                ListItems.Items.Add(item);
            }
        }

        private void removeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ListItems.SelectedItem != null)
            {
                Object.Remove(ListItems.SelectedItem as string);
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
                nameBox.Text = ListItems.SelectedItem as string;
                if (nameBox.Text.EndsWith("/TCP"))
                {
                    TCPBtn.IsChecked = true;
                    nameBox.IsReadOnly = true;
                }
                else if (nameBox.Text.EndsWith("/TCP2"))
                {
                    TCP2Btn.IsChecked = true;
                    nameBox.IsReadOnly = true;
                }
                else if (nameBox.Text.EndsWith("/DBPkt"))
                {
                    DBPacketBtn.IsChecked = true;
                    nameBox.IsReadOnly = true;
                }
                else if (nameBox.Text.EndsWith("/DBPacket"))
                {
                    DBPacketBtn.IsChecked = true;
                    nameBox.IsReadOnly = true;
                }
                else if (nameBox.Text.EndsWith("/Smart"))
                {
                    SmartBtn.IsChecked = true;
                    nameBox.IsReadOnly = true;
                }
                else
                {
                    GeneralCustardBtn.IsChecked = true;
                    nameBox.IsReadOnly = false;
                }
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
                Object[Object.IndexOf(ListItems.SelectedItem as string)] = nameBox.Text;
                PopulateList();
                ListItems.SelectedIndex = si;
                haltevents = false;
            }
        }

        private void GeneralCustardBtn_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var item in toglepanel.Children)
            {
                if (item != sender)
                    (item as ToggleButton).IsChecked = false;
            }
            if (sender == GeneralCustardBtn)
            {
                nameBox.IsReadOnly = false;
            }
            else if (sender == TCPBtn)
            {
                nameBox.IsReadOnly = true;
                nameBox.Text = "zomb://." + Team + "/TCP";
            }
            else if (sender == TCP2Btn)
            {
                nameBox.IsReadOnly = true;
                nameBox.Text = "zomb://." + Team + "/TCP2";
            }
            else if (sender == DBPacketBtn)
            {
                nameBox.IsReadOnly = true;
                nameBox.Text = "zomb://." + Team + "/DBPkt";
            }
            else if (sender == SmartBtn)
            {
                nameBox.IsReadOnly = true;
                nameBox.Text = "zomb://." + Team + "/Smart";
            }
        }

        private void nameBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (nameBox.IsReadOnly)
            {
                GeneralCustardBtn.IsChecked = true;
                GeneralCustardBtn_Checked(GeneralCustardBtn, null);
            }
        }
    }
}
