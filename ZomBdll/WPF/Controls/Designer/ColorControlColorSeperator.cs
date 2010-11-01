using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;

namespace System451.Communication.Dashboard.WPF.Controls.Designer
{
    public class ColorControlColorSeperator : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush(Color.FromArgb(System.Convert.ToByte(value[3]), System.Convert.ToByte(value[0]), System.Convert.ToByte(value[1]), System.Convert.ToByte(value[2])));
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            var cr = (value as SolidColorBrush).Color;
            return new object[] { cr.R, cr.G, cr.B, cr.A };
        }
    }
}
