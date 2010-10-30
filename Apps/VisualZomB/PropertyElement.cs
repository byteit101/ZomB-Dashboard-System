using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System451.Communication.Dashboard.WPF.Design;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Globalization;
using System.Windows.Media;

namespace System451.Communication.Dashboard.ViZ
{
    /// <summary>
    /// Represents a designable property
    /// </summary>
    public class PropertyElement : DependencyObject, IComparable<PropertyElement>
    {
        /// <summary>
        /// Creates a new PropertyElement
        /// </summary>
        /// <param name="obj">The object</param>
        /// <param name="propName">The propery name</param>
        public PropertyElement(object obj, string propName)
        {
            Object = obj;
            Property = obj.GetType().GetProperty(propName);
        }

        /// <summary>
        /// Creates a new PropertyElement
        /// </summary>
        /// <param name="obj">The object</param>
        /// <param name="prop">The propery</param>
        public PropertyElement(object obj, PropertyInfo prop)
        {
            Object = obj;
            Property = prop;
        }

        /// <summary>
        /// Gets the name of the Property
        /// </summary>
        public string Name
        {
            get
            {
                string r = Property.Name;
                foreach (var at in Property.GetCustomAttributes(typeof(ZomBDesignableAttribute), true))
                    return (at as ZomBDesignableAttribute).DisplayName ?? r;
                foreach (var at in Object.GetType().GetCustomAttributes(typeof(ZomBDesignablePropertyAttribute), true))
                    if ((at as ZomBDesignablePropertyAttribute).PropertyName == Property.Name)
                    return (at as ZomBDesignablePropertyAttribute).DisplayName ?? r;
                return r;
            }
        }

        /// <summary>
        /// Gets the category of the Property
        /// </summary>
        public string Category
        {
            get
            {
                string r = "Misc";
                foreach (var at in Property.GetCustomAttributes(typeof(CategoryAttribute), true))
                    return (at as CategoryAttribute).Category ?? r;
                foreach (var at in Object.GetType().GetCustomAttributes(typeof(ZomBDesignablePropertyAttribute), true))
                    if ((at as ZomBDesignablePropertyAttribute).PropertyName == Property.Name)
                        return (at as ZomBDesignablePropertyAttribute).Category ?? r;
                return r;
            }
        }

        /// <summary>
        /// Gets the description of the Property
        /// </summary>
        public string Description
        {
            get
            {
                string r = Name;
                foreach (var at in Property.GetCustomAttributes(typeof(DescriptionAttribute), true))
                    return (at as DescriptionAttribute).Description ?? r;
                foreach (var at in Object.GetType().GetCustomAttributes(typeof(ZomBDesignablePropertyAttribute), true))
                    if ((at as ZomBDesignablePropertyAttribute).PropertyName == Property.Name) 
                        return (at as ZomBDesignablePropertyAttribute).Description ?? r;
                return r;
            }
        }

        /// <summary>
        /// Gets the Index of the Property
        /// </summary>
        public uint Index
        {
            get
            {
                uint r = 0;
                foreach (var at in Property.GetCustomAttributes(typeof(ZomBDesignableAttribute), true))
                    r = (at as ZomBDesignableAttribute).Index;
                foreach (var at in Object.GetType().GetCustomAttributes(typeof(ZomBDesignablePropertyAttribute), true))
                    if ((at as ZomBDesignablePropertyAttribute).PropertyName == Property.Name) 
                        r = (at as ZomBDesignablePropertyAttribute).Index;
                if (r == 0)
                    r = uint.MaxValue / 2;
                return r;
            }
        }

        /// <summary>
        /// Gets the dynamics of the Property
        /// </summary>
        public bool Dynamic
        {
            get
            {
                foreach (var at in Property.GetCustomAttributes(typeof(ZomBDesignableAttribute), true))
                    return (at as ZomBDesignableAttribute).Dynamic;
                foreach (var at in Object.GetType().GetCustomAttributes(typeof(ZomBDesignablePropertyAttribute), true))
                    if ((at as ZomBDesignablePropertyAttribute).PropertyName == Property.Name)
                        return (at as ZomBDesignablePropertyAttribute).Dynamic;
                return false;
            }
        }

        /// <summary>
        /// Gets the type of the Property
        /// </summary>
        public Type Type
        {
            get
            {
                return Property.PropertyType;
            }
        }

        /// <summary>
        /// Gets or sets the value of the Property
        /// </summary>
        public object Value
        {
            get
            {
                return Property.GetValue(Object, null);
            }
            set
            {
                var tc = Property.PropertyType.GetCustomAttributes(typeof(TypeConverterAttribute), true);
                if (tc.Length > 0)
                {
                    string tcname=(tc[0] as TypeConverterAttribute).ConverterTypeName;
                    TypeConverter tcv = System.Type.GetType(tcname).GetConstructor(Type.EmptyTypes).Invoke(null) as TypeConverter;
                    try
                    {
                        value = tcv.ConvertFrom(null, System.Globalization.CultureInfo.CurrentCulture, value);
                    }
                    catch
                    {
                        return;
                    }
                }
                if (Type == typeof(double))
                    value = double.Parse(value.ToString());
                if (Type == typeof(int))
                    value = int.Parse(value.ToString());
                if (Type.IsEnum)
                    value = Enum.Parse(Type, value.ToString());

                Property.SetValue(Object, value, null);
            }
        }

        /// <summary>
        /// Gets or sets the Object we are dealing with
        /// </summary>
        public object Object
        {
            get { return (object)GetValue(ObjectProperty); }
            set { SetValue(ObjectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Object.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ObjectProperty =
            DependencyProperty.Register("Object", typeof(object), typeof(PropertyElement), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the Property
        /// </summary>
        public PropertyInfo Property
        {
            get { return (PropertyInfo)GetValue(PropertyProperty); }
            set { SetValue(PropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Property.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertyProperty =
            DependencyProperty.Register("Property", typeof(PropertyInfo), typeof(PropertyElement), new UIPropertyMetadata(null));

        public int CompareTo(PropertyElement other)
        {
            var ix = Index.CompareTo(other.Index);
            if (ix == 0)
                return Name.CompareTo(other.Name);
            return ix;
        }

        public FrameworkElement GetEntry()
        {
            if (Object != null && Property != null)
            {
                var itm = new StackPanel();
                itm.Orientation = Orientation.Horizontal;
                itm.Children.Add(new TextBlock());
                (itm.Children[0] as TextBlock).Text = Name + ": ";
                if (Type == typeof(bool))
                {
                    itm.Children.Add(new CheckBox());
                    (itm.Children[1] as CheckBox).IsChecked = (bool)Value;
                    (itm.Children[1] as CheckBox).Checked += delegate { Value = true; };
                    (itm.Children[1] as CheckBox).Unchecked += delegate { Value = false; };
                    (itm.Children[1] as CheckBox).Focusable = false;
                }
                else if (Type == typeof(int) || Type == typeof(double))
                {
                    //TODO: better support
                    itm.Children.Add(new TextBox());
                    (itm.Children[1] as TextBox).Width = 50.0;
                    if (Dynamic)
                    {
                        Binding bind = new Binding();
                        bind.Mode = BindingMode.TwoWay;
                        bind.Source = Object;
                        bind.Path = new PropertyPath(GetRealProperty());
                        bind.Converter = new StringValueConverter();
                        (itm.Children[1] as TextBox).SetBinding(TextBox.TextProperty, bind);
                    }
                    else
                    {
                        (itm.Children[1] as TextBox).Text = Value.ToString();
                        (itm.Children[1] as TextBox).TextChanged += delegate(object sender, TextChangedEventArgs e) { Value = (sender as TextBox).Text; };

                    }
                }
                else if (Type.IsEnum)
                {
                    itm.Children.Add(new ComboBox());
                    (itm.Children[1] as ComboBox).Width = 90;
                    foreach (var item in Enum.GetNames(Type))
                    {
                        (itm.Children[1] as ComboBox).Items.Add(item);
                        if (item == Value.ToString())
                        {
                            (itm.Children[1] as ComboBox).SelectedValue = item;
                        }
                    }
                    (itm.Children[1] as ComboBox).SelectionChanged+=delegate(object sender, SelectionChangedEventArgs e) { try { Value = (sender as ComboBox).SelectedValue; } catch { } };
            
                }
                else if (Type == typeof(Brush))
                {
                    itm.Children.Add(new BrushDesigner().GetProperyField(Object, Property));
                }
                else if (Type == typeof(Color))
                {
                    itm.Children.Add(new ColorDesigner().GetProperyField(Object, Property));
                }
                else
                {
                    itm.Children.Add(new TextBox());
                    (itm.Children[1] as TextBox).Width = 100.0;
                    try
                    {
                        (itm.Children[1] as TextBox).Text = Value.ToString();
                    }
                    catch { }//Null value
                    (itm.Children[1] as TextBox).TextChanged += delegate(object sender, TextChangedEventArgs e) { Value = (sender as TextBox).Text; };

                }
                itm.Tag = this;
                itm.Margin = new Thickness(1);
                return itm;
            }
            else
                throw new NullReferenceException("Object and Property must not be null");
        }

        private object GetRealProperty()
        {
            FieldInfo fieldInfo = Object.GetType().GetField(Property.Name + "Property", BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public);
            if (fieldInfo == null)
                return Property.Name;
            DependencyProperty dp = (DependencyProperty)fieldInfo.GetValue(null);
            if (dp == null)
                return Property.Name;
            return dp;
        }
    }
}
