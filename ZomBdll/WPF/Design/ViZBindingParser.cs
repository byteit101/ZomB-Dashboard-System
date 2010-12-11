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
            try
            {
                switch (parameter.ToString()[0])
                {
                    case 's':
                        return value.ToString();
                    case 'n':
                        return targetType.GetMethod("Parse", new Type[] { typeof(string) }).Invoke(null, new object[] { value.ToString() });
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
                    case 'p':
                        {
                            switch (parameter.ToString()[1])
                            {
                                case 'n':
                                    {
                                        //Format: {Fromstart:fromend}-{tostart:toend}
                                        //ex: {-1.0:1.0}-{0:360}
                                        var rx = new Regex("\\{([\\-\\.0-9]*)\\:([\\-\\.0-9]*)\\}\\-\\{([\\-\\.0-9]*)\\:([\\-\\.0-9]*)\\}");
                                        var res = rx.Match(parameter.ToString().Substring(2));
                                        var fs = res.Groups[1].Value;
                                        var fe = res.Groups[2].Value;
                                        var ts = res.Groups[3].Value;
                                        var te = res.Groups[4].Value;
                                        try
                                        {
                                            var fsd = double.Parse(fs);
                                            var fed = double.Parse(fe);
                                            var tsd = double.Parse(ts);
                                            var ted = double.Parse(te);

                                            //massive converter, fun
                                            double end = (((((double)value) - fsd) / (fed - fsd)) * (ted - tsd)) + tsd;
                                            //return, using its own parse
                                            if (targetType == typeof(double))
                                                return end;
                                            try
                                            {
                                                return targetType.GetMethod("Parse", new Type[] { typeof(string) }).Invoke(null, new object[] { end.ToString() });
                                            }
                                            catch { }
                                            try
                                            {
                                                return targetType.GetMethod("Parse", new Type[] { typeof(string) }).Invoke(null, new object[] { Math.Round(end).ToString() });
                                            }
                                            catch { }
                                            return end;//hope for the best
                                        }
                                        catch { }
                                        break;
                                    }
                                default:
                                    break;
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
            catch { }
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
