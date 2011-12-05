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
using System.Collections.Generic;

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
            Team = team;
            lastid = Object.Count;
            InitializeComponent();
            if (Object.Count > 0)
            {
                PopulateList();
            }
            FindUrls();
            if (ListItems.Items.Count >0)
                ListItems.SelectedIndex = 0;
        }

        private void FindUrls()
        {
            var typers = new Collection<Type>();
            var isrc = new System.Collections.Generic.SortedList<TypedDataSourceAttributeComparer, DataSourceAttribute>();
            KnownTypes.ItemsSource = isrc;
            foreach (var item in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var itype in item.GetTypes())
                {
                    foreach (var cat in itype.GetCustomAttributes(typeof(DataSourceAttribute), false))
                    {
                        var xcat = (cat as DataSourceAttribute);
                        if (xcat.IgnoreClones)
                        {
                            foreach (var cloneposs in typers)
                            {
                                if (cloneposs == itype)
                                {
                                    goto noadd;
                                }
                            }
                        }
                        isrc.Add(new TypedDataSourceAttributeComparer { DataSourceAttribute = xcat, Type = itype }, xcat);
                        typers.Add(itype);
                    noadd:
                        continue;
                    }
                }
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
                string nbtxt = nameBox.Text = ListItems.SelectedItem as string;
                nbtxt = nbtxt.Substring(7);//zomb://
                nbtxt = nbtxt.Substring(nbtxt.IndexOf("/"));
                KnownTypes.UnselectAll();
                foreach (var item in KnownTypes.ItemsSource)
                {
                    var kvpair = (KeyValuePair<TypedDataSourceAttributeComparer, DataSourceAttribute>)item;
                    if (nbtxt == ("/" + kvpair.Value.SourceName) || nbtxt.StartsWith("/" + kvpair.Value.SourceName + "?") || nbtxt.StartsWith("/" + kvpair.Value.SourceName + "/"))
                    {
                        KnownTypes.SelectedItem = item;
                        break;
                    }
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
                Object[si] = nameBox.Text;
                PopulateList();
                ListItems.SelectedIndex = si;
                haltevents = false;
            }
        }

        private void GeneralCustardBtn_Checked(object sender, SelectionChangedEventArgs e)
        {
            if (haltevents)
                return;
            try
            {
                haltevents = true;
                string nbtxt = nameBox.Text;
                if (string.IsNullOrEmpty(nbtxt))
                    return;
                nbtxt = nbtxt.Substring(7);//zomb://
                string nbtxtstart = "zomb://" + nbtxt.Substring(0, nbtxt.IndexOf("/") + 1);
                nbtxt = nbtxt.Substring(nbtxt.IndexOf("/") + 1);
                var newtxt = ((KeyValuePair<TypedDataSourceAttributeComparer, DataSourceAttribute>)KnownTypes.SelectedItem).Value.SourceName;
                if (nbtxt.Contains('/'))
                {
                    nbtxt = newtxt + nbtxt.Substring(nbtxt.IndexOf("/"));
                }
                else if (nbtxt.Contains('?'))
                {
                    nbtxt = newtxt + nbtxt.Substring(nbtxt.IndexOf("?"));
                }
                else
                {
                    nbtxt = newtxt;
                }
                nameBox.Text = nbtxtstart + nbtxt;
                haltevents = false;
                nameBox_TextChanged(sender, null);
            }
            catch { haltevents = false; }
        }

        private void nameBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (haltevents)
                return;
            try
            {
                haltevents = true;
                string nbtxt = nameBox.Text;
                nbtxt = nbtxt.Substring(7);//zomb://
                nbtxt = nbtxt.Substring(nbtxt.IndexOf("/"));
                KnownTypes.UnselectAll();
                foreach (var item in KnownTypes.ItemsSource)
                {
                    var kvpair = (KeyValuePair<TypedDataSourceAttributeComparer, DataSourceAttribute>)item;
                    if (nbtxt == ("/" + kvpair.Value.SourceName) || nbtxt.StartsWith("/" + kvpair.Value.SourceName + "?") || nbtxt.StartsWith("/" + kvpair.Value.SourceName + "/"))
                    {
                        KnownTypes.SelectedItem = item;
                        break;
                    }
                }
                haltevents = false;
            }
            catch { haltevents = false; }
        }
    }
}
