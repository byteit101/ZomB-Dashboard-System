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
using System451.Communication.Dashboard.WPF.Design;
using System.Reflection;
using System.Collections;
using System.ComponentModel;

namespace System451.Communication.Dashboard.ViZ
{
    /// <summary>
    /// Interaction logic for BindingDesigner.xaml
    /// </summary>
    public partial class BindingDesigner : Window
    {
        object obj;
        PropertyInfo prop;
        public BindingDesigner(object o, PropertyInfo p, IEnumerable itms)
        {
            InitializeComponent();
            obj = o;
            prop = p;
            elmbox.ItemsSource = itms;
            InfoBlock.Text = "Element: " + (o as FrameworkElement).Name + " - {" + o.GetType().Name + "}\r\nProperty: "+p.Name+"\r\nType: "+p.PropertyType.Name;
        }

        private void elmbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            propnamebox.ItemsSource = from p in (elmbox.SelectedItem as SurfaceControl).Control.GetType().GetProperties()
                                      orderby p.Name
                                      where IsBindable(p)
                                      select new object[] {p, GetIsPropertyMain(p)};
        }

        private bool IsBindable(PropertyInfo p)
        {
            foreach (var at in p.GetCustomAttributes(typeof(BindableAttribute), true))
            {
                return (at as BindableAttribute).Bindable;
            }
            return true;
        }

        private bool GetIsPropertyMain(PropertyInfo p)
        {
            foreach (var at in p.GetCustomAttributes(typeof(ZomBDesignableAttribute), true))
            {
                return true;
            }
            foreach (var at in (elmbox.SelectedItem as SurfaceControl).Control.GetType().GetCustomAttributes(typeof(ZomBDesignablePropertyAttribute), true))
            {
                if ((at as ZomBDesignablePropertyAttribute).PropertyName == p.Name)
                    return true;
            }
            return p.Name.EndsWith("Value");
        }

        private void propnamebox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            proptypelbl.Content = (((object[])propnamebox.SelectedItem)[0] as PropertyInfo).PropertyType.Name;
            proptypelbl.Foreground = ((((object[])propnamebox.SelectedItem)[0] as PropertyInfo).PropertyType == prop.PropertyType) ? Brushes.Black : Brushes.Red;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dpd = DependencyPropertyDescriptor.FromName(prop.Name, prop.DeclaringType, obj.GetType());
            var b = new Binding((((object[])propnamebox.SelectedItem)[0] as PropertyInfo).Name);
            b.Source = elmbox.SelectedItem;
            (obj as FrameworkElement).SetBinding(dpd.DependencyProperty, b);
        }
    }

    internal class BindNameConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value as SurfaceControl).Control.Name + " - {" + (value as SurfaceControl).Control.GetType().Name + "}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    internal class BindPropConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (((object[])value)[0] as PropertyInfo).Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    internal class BindPropBConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((bool)(((object[])value)[1])) ? FontWeights.Bold : FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
