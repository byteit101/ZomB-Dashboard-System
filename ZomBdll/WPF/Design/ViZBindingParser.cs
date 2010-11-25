using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.ComponentModel;

namespace System451.Communication.Dashboard.WPF.Controls
{
    public class ViZBindingParser: IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch (parameter.ToString()[0])
            {
                case 's':
                    return value.ToString();
                case 'n':
                    return targetType.GetMethod("Parse", new Type[]{typeof(string)}).Invoke(null, new object[] { value.ToString() });
                case 'C':
                    {
                        var tc = targetType.GetCustomAttributes(typeof(TypeConverterAttribute), true);
                        if (tc.Length > 0)
                        {
                            string tcname = (tc[0] as TypeConverterAttribute).ConverterTypeName;
                            TypeConverter tcv = Type.GetType(tcname).GetConstructor(Type.EmptyTypes).Invoke(null) as TypeConverter;
                            try
                            {
                                return tcv.ConvertFrom(null, culture, value);
                            }
                            catch
                            {
                                return value;
                            }
                        }
                        return value;
                    }
                default:
                    break;
            }

            /*Dictionary<string, string> dic = new Dictionary<string, string>();
            var parms = parameter.ToString().Split(';');
            var spliter = new Regex(@"([A-Za-z]*)=\(([^\)]*)\)");
            foreach (var item in parms)
            {
                var res = spliter.Match(item);
                dic.Add(res.Groups[1].Value, res.Groups[2].Value);
            }
            */
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}
