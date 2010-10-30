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
        void Initialize(object obj, PropertyInfo property);
        FrameworkElement GetProperyField();
        bool IsDefaultValue();
        bool IsExpanded();
        string GetValue();
    }

    [global::System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    sealed class DesignerAttribute : Attribute
    {
        public DesignerAttribute(Type DesignerType)
        {
            this.DesignerType = DesignerType;
        }

        public Type DesignerType { get; private set; }
    }
}
