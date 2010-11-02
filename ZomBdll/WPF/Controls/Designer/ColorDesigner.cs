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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System451.Communication.Dashboard.WPF.Design;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Globalization;
using System.ComponentModel;
using System451.Communication.Dashboard.WPF.Controls.Designers;
using System451.Communication.Dashboard.WPF.Controls.Designer;

namespace System451.Communication.Dashboard.WPF.Design
{
    public class BrushDesigner : DesignerBase
    {
        private ComboBox comb;
        public override FrameworkElement GetProperyField()
        {
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            ComboBox cb = new ComboBox();
            cb.Width = 90;
            cb.IsTextSearchEnabled = true;
            cb.IsEditable = true;
            comb = cb;
            UpdateCombo();
            comb.AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(delegate { try { Property.SetValue(Object, new BrushConverter().ConvertFrom(null, CultureInfo.CurrentCulture, cb.Text), null); } catch { } }));
            sp.Children.Add(cb);
            Button btn = new Button();
            btn.Content = "...";
            btn.Click += new RoutedEventHandler(OpenBrushDialog);
            sp.Children.Add(btn);
            return sp;
        }

        private void UpdateCombo()
        {
            object cvalue = Property.GetValue(Object, null);
            foreach (var item in typeof(Brushes).GetProperties())
            {
                comb.Items.Add(item.Name);
                if (cvalue != null && cvalue == item.GetValue(null, null))
                {
                    comb.SelectedValue = item.Name;
                    cvalue = null;
                }
            }
            if (cvalue != null)
            {
                comb.Text = cvalue.ToString();
            }
        }

        void OpenBrushDialog(object sender, RoutedEventArgs e)
        {
            new BrushDesignerWindow(Object, Property).ShowDialog();
            UpdateCombo();
        }

        public override bool IsExpanded()
        {
            return !(Property.GetValue(Object, null) is SolidColorBrush);
        }

        public override string GetValue()
        {
            if (!IsExpanded())
                return comb.Text;
            else
            {
                //TODO: add others
                //must be linear gradient
                LinearGradientBrush lb = (Property.GetValue(Object, null) as LinearGradientBrush);
                StringBuilder s = new StringBuilder("<LinearGradientBrush StartPoint=\"0,0\" EndPoint=\"1,0\">");
                foreach (var item in lb.GradientStops)
                {
                    s.Append("<GradientStop Offset=\"");
                    s.Append(item.Offset);
                    s.Append("\" Color=\"");
                    s.Append(item.Color.ToString());
                    s.Append("\" />");
                }
                s.Append("</LinearGradientBrush>");
                return s.ToString();
            }
        }
    }
    public class ColorDesigner : DesignerBase
    {
        ComboBox comb;
        public override FrameworkElement GetProperyField()
        {
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            ComboBox cb = new ComboBox();
            cb.Width = 90;
            cb.IsTextSearchEnabled = true;
            cb.IsEditable = true;
            object cvalue = Property.GetValue(Object, null);
            foreach (var item in typeof(Colors).GetProperties())
            {
                cb.Items.Add(item.Name);
                if (cvalue != null && cvalue == item.GetValue(null, null))
                {
                    cb.SelectedValue = item.Name;
                    cvalue = null;
                }
            }
            if (cvalue != null)
            {
                cb.Text = cvalue.ToString();
            }
            cb.AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(delegate { try { Property.SetValue(Object, new ColorConverter().ConvertFrom(null, CultureInfo.CurrentCulture, cb.Text), null); } catch { } }));
            comb = cb;
            sp.Children.Add(cb);
            return sp;
        }

        public override string GetValue()
        {
            return comb.Text;
        }
    }
}
