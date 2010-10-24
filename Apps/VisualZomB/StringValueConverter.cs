using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;

namespace System451.Communication.Dashboard.ViZ
{
    class StringValueConverter:IValueConverter
    {
        public StringValueConverter()
        {
        }


        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType == typeof(double))
                return double.Parse(value.ToString());
            if (targetType == typeof(int))
                return int.Parse(value.ToString());

            var tc = targetType.GetCustomAttributes(typeof(TypeConverterAttribute), true);
            if (tc.Length > 0)
            {
                string tcname = (tc[0] as TypeConverterAttribute).ConverterTypeName;
                TypeConverter tcv = System.Type.GetType(tcname).GetConstructor(Type.EmptyTypes).Invoke(null) as TypeConverter;
                try
                {
                    return tcv.ConvertFrom(null, System.Globalization.CultureInfo.CurrentCulture, value);
                }
                catch
                {
                    return null;
                }
            }
            return value;
        }

        #endregion
    }
}
