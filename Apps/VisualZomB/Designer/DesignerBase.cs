using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System451.Communication.Dashboard.WPF.Design;
using System.Windows;
using System.Reflection;
using System.ComponentModel;

namespace System451.Communication.Dashboard.ViZ.Design
{
    public abstract class DesignerBase : IDesigner
    {
        protected object Object { get; private set; }
        protected PropertyInfo Property { get; private set; }

        #region IDesigner Members

        public void Initialize(object obj, PropertyInfo property)
        {
            Object = obj;
            Property = property;
        }

        public abstract FrameworkElement GetProperyField();

        public virtual bool IsDefaultValue()
        {
            try
            {
                var o = GetRealProperty();
                if (o is DependencyProperty)
                {
                    return (o as DependencyProperty).GetMetadata(Object.GetType()).DefaultValue == Property.GetValue(Object, null);
                }
                object[] dv = Property.GetCustomAttributes(typeof(DefaultValueAttribute), true);
                if (dv.Length > 0)
                {
                    return ((dv[0] as DefaultValueAttribute).Value == Property.GetValue(Object, null));
                }
                return false;
            }
            catch { }//No DP, I have no idea now
            return false;
        }

        public abstract bool IsExpanded();

        public virtual string GetValue()
        {
            return Property.GetValue(Object, null).ToString();
        }

        #endregion

        protected object GetRealProperty()
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
