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
using System451.Communication.Dashboard.WPF.Controls;

namespace System451.Communication.Dashboard.ViZ
{
    /// <summary>
    /// Interaction logic for BindingDesigner.xaml
    /// </summary>
    public partial class BindingDesigner : Window
    {
        object obj;
        PropertyInfo prop;
        bool inited = false;
        public BindingDesigner(object o, PropertyInfo p, IEnumerable itms)
        {
            InitializeComponent();
            obj = o;
            prop = p;
            elmbox.ItemsSource = from object i in itms where (i as SurfaceControl).Control.Name!="" select i;
            var name = "";
            if (o is IZomBControl && !string.IsNullOrEmpty((o as IZomBControl).ControlName))
                name = (o as IZomBControl).ControlName;
            else if (o is FrameworkElement)
                name = (o as FrameworkElement).Name;
            InfoBlock.Text = "Element: " + name + " - {" + o.GetType().Name + "}\r\nProperty: "+p.Name+"\r\nType: "+p.PropertyType.Name;
            var dp = DependencyPropertyDescriptor.FromName(p.Name, p.DeclaringType, o.GetType()).DependencyProperty;
            var dop = o as DependencyObject;
            var be = BindingOperations.GetBindingExpression(dop, dp);
            if (be != null)
            {
                var itm = be.ParentBinding.Source;
                if (itm!=null)
                    itm = SurfaceControl.GetSurfaceControlFromControl(itm);
                if (itm==null)
                    itm = (from object elm in elmbox.ItemsSource where (elm as SurfaceControl).Control.Name==be.ParentBinding.ElementName select elm).First();
                elmbox.SelectedItem = itm;
                var rs = (from object elm in propnamebox.ItemsSource
                          where (((object[])elm)[0] as PropertyInfo).Name == be.ParentBinding.Path.Path
                          select elm);
                var s = rs.First();
                int i = 0;
                foreach (var item in propnamebox.ItemsSource)
                {
                    if (((object[])item)[0] == ((object[])s)[0])
                        break;
                    i++;
                }
                propnamebox.SelectedIndex = i;

                //TODO: add converter info and reload
            }
            inited = true;
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
            if (NoConv.IsChecked==false)
            {
                b.Converter = new ViZBindingParser();
                b.ConverterParameter = getConverterType();
            }
            BindingOperations.SetBinding(obj as DependencyObject, dpd.DependencyProperty, b);
            this.DialogResult = true;
        }

        private string getConverterType()
        {
            char t = 'x';
            if (NumRangeConv.IsChecked == true) //TODO: generizise
                return "pn{" + bindconvNumCustomFomStart.Text + ":" + bindconvNumCustomFomEnd.Text + "}-{" + bindconvNumCustomToStart.Text + ":" + bindconvNumCustomToEnd.Text + "}";
            else if (NumCov.IsChecked == true)
                t = 'n';
            else if (BrushConv.IsChecked == true)
                t = 'B';
            else if (StringConv.IsChecked == true)
                t = 's';
            else if (TypeConv.IsChecked == true)
                t = 'C';
            return t.ToString();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void DefaultConvert_Changed(object sender, RoutedEventArgs e)
        {
            if (!inited)
                return;
            if (sender == NumRangeConv)
                NoConv.IsChecked = NumCov.IsChecked = BrushConv.IsChecked = StringConv.IsChecked = TypeConv.IsChecked = false;
            else
                NumRangeConv.IsChecked = false;
        }

        private void bindconvNumCustomToEnd_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = (sender as TextBox).Text;
            var ntxt = numbify(txt);
            if (txt != ntxt)
            {
                Console.Beep();
                (sender as TextBox).Text = ntxt;
            }
        }

        private string numbify(string txt)
        {
            StringBuilder sb = new StringBuilder();
            int count = txt.Length;
            for (int i = 0; i < count; i++)
            {
                switch (txt[i])
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '.':
                        sb.Append(txt[i]);
                        break;
                    case '-':
                        if (sb.Length<1)
                            sb.Append(txt[i]);
                        break;
                    default:
                        break;
                }
            }
            return sb.ToString();
        }
    }

    internal class BindNameConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var c = (value as SurfaceControl).Control;
            return ((c is IZomBControl) ? (c as IZomBControl).ControlName : (c as FrameworkElement).Name) + " - {" + (value as SurfaceControl).Control.GetType().Name + "}";
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

    /// <summary>
    /// Do not use this class except for saving values
    /// </summary>
    internal class BoundPropertyDesigner : WPF.Controls.Designers.DesignerBase
    {
        public override FrameworkElement GetProperyField() { throw new NotImplementedException("Bindings are not visual!"); }

        public override bool IsDefaultValue() { return false; }

        public override bool IsExpanded() { return true; }

        public override string GetValue()
        {
            var dp = DependencyPropertyDescriptor.FromName(Property.Name, Property.DeclaringType, Object.GetType()).DependencyProperty;
            var dop = Object as DependencyObject;
            var be = BindingOperations.GetBindingExpression(dop, dp);
            var sb = new StringBuilder();
            sb.Append("<Binding Path=\"");
            sb.Append(be.ParentBinding.Path.Path);
            sb.Append("\" ElementName=\"");
            if (be.ParentBinding.Source == null)
                sb.Append(be.ParentBinding.ElementName);
            else
                sb.Append((be.ParentBinding.Source as SurfaceControl).Control.Name);
            sb.Append("\">");
            if (be.ParentBinding.Converter != null)
            {
                sb.Append("<Binding.Converter><ZomB:ViZBindingParser /></Binding.Converter>");
                sb.Append("<Binding.ConverterParameter>");
                sb.Append(be.ParentBinding.ConverterParameter);
                sb.Append("</Binding.ConverterParameter>");
            }
            sb.Append("</Binding>");
            return sb.ToString();
        }
    }
}
