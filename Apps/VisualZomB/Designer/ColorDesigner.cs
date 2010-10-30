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

namespace System451.Communication.Dashboard.ViZ
{
    public class BrushDesigner : IDesigner
    {
        public FrameworkElement GetProperyField(object obj, PropertyInfo property)
        {
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            ComboBox cb = new ComboBox();
            cb.Width = 90;
            cb.IsTextSearchEnabled = true;
            cb.IsEditable = true;
            object cvalue = property.GetValue(obj, null);
            foreach (var item in typeof(Brushes).GetProperties())
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
            cb.AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(delegate { try { property.SetValue(obj, new BrushConverter().ConvertFrom(null, CultureInfo.CurrentCulture, cb.Text), null); } catch { } }));
            sp.Children.Add(cb);
            return sp;
        }
    }
    public class ColorDesigner : IDesigner
    {
        public FrameworkElement GetProperyField(object obj, PropertyInfo property)
        {
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            ComboBox cb = new ComboBox();
            cb.Width = 90;
            cb.IsTextSearchEnabled = true;
            cb.IsEditable = true;
            object cvalue = property.GetValue(obj, null);
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
            cb.AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(delegate { try { property.SetValue(obj, new ColorConverter().ConvertFrom(null, CultureInfo.CurrentCulture, cb.Text), null); } catch { } }));
            sp.Children.Add(cb);
            return sp;
        }
    }
}
