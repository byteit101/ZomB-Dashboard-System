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
            object cvalue = Property.GetValue(Object, null);
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
            cb.AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(delegate { try { Property.SetValue(Object, new BrushConverter().ConvertFrom(null, CultureInfo.CurrentCulture, cb.Text), null); } catch { } }));
            comb = cb;
            sp.Children.Add(cb);
            return sp;
        }

        public override string GetValue()
        {
            return comb.Text;
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
