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
using System451.Communication.Dashboard.Utils;

namespace System451.Communication.Dashboard.WPF.Controls.Designer
{
    /// <summary>
    /// Interaction logic for TriggerListenersDesignerWindow.xaml
    /// </summary>
    public partial class TriggerListenersDesignerWindow : Window
    {
        IEnumerable<string> elmnames;
        public TriggerListenersDesignerWindow()
        {
            InitializeComponent();
        }

        public string UpdateTLs(string tls)
        {
            elmnames = from child in ZDesigner.GetChildren() where child is FrameworkElement select (child as FrameworkElement).Name;
            string[] trigs = tls.Split(';');
            foreach (var trig in trigs)
            {
                string[] nv = trig.Split(':');
                if (nv.Length == 2)
                    AddTrigger(nv[0], nv[1]);
            }
            AddTrigger("", "");
            this.ShowDialog();
            string ret = "";
            foreach (UIElement item in Stacker.Children)
            {
                ret += ((item as Grid).Children[0] as ComboBox).Text + ":";
                ret += ((item as Grid).Children[1] as ComboBox).Text + ";";
            }
            return ret.Replace(":;","");
        }

        private void AddTrigger(string name, string calls)
        {
            var sp = new Grid();
            sp.ColumnDefinitions.Add(new ColumnDefinition());
            sp.ColumnDefinitions.Add(new ColumnDefinition());
            sp.ColumnDefinitions.Add(new ColumnDefinition());
            sp.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
            sp.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
            sp.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Auto);
            var cb = new ComboBox();
            cb.ItemsSource = elmnames;
            cb.SelectedIndex = elmnames.ToList().IndexOf(name);
            cb.SelectionChanged += new SelectionChangedEventHandler(cb_SelectionChanged);
            Grid.SetColumn(cb, 0);
            sp.Children.Add(cb);
            var mcb = new ComboBox();
            Grid.SetColumn(mcb, 1);
            sp.Children.Add(mcb);
            var btn = new Button();
            btn.Content = "-";
            Grid.SetColumn(btn, 2);
            btn.Click += new RoutedEventHandler(btn_Click);
            sp.Children.Add(btn);
            Stacker.Children.Add(sp);
        }

        void btn_Click(object sender, RoutedEventArgs e)
        {
            var box = FindAnchestor<Grid>(e.Source as DependencyObject);
            if (box != null && LogicalTreeHelper.GetParent(box) == Stacker && Stacker.Children.IndexOf(box) < Stacker.Children.Count - 1)
            {
                Stacker.Children.Remove(box);
            }
        }

        void cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var box = FindAnchestor<Grid>(e.Source as DependencyObject);
            if (Stacker.Children.IndexOf(box) == Stacker.Children.Count - 1)
            {
                AddTrigger("", "");
            }
            var cbbx = sender as ComboBox;
            (box.Children[1] as ComboBox).ItemsSource = from src in ZDesigner.GetChildByName(cbbx.SelectedValue.ToString()).GetType().GetMethods()
                                                        where src.GetParameters().Count() == 0 && src.ReturnType == typeof(void)
                                                        select src.Name;
        }

        public static T FindAnchestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }
    }
}
