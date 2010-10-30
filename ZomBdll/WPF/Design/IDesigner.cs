using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Reflection;

namespace System451.Communication.Dashboard.WPF.Design
{
    public interface IDesigner
    {
        FrameworkElement GetProperyField(object obj, PropertyInfo property);
    }
}
