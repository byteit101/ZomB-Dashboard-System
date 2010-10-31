using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Reflection;
using System.Windows.Media;

namespace System451.Communication.Dashboard.WPF.Design
{
    /// <summary>
    /// Designer Interface
    /// </summary>
    public interface IDesigner
    {
        /// <summary>
        /// Initialize the Designer
        /// </summary>
        /// <param name="obj">Object we are designing on</param>
        /// <param name="property">Property on the Object that we are designing</param>
        void Initialize(object obj, PropertyInfo property);

        /// <summary>
        /// Get the field to be put in the property editor
        /// </summary>
        /// <returns>Control to be put next to the property name in the property editor</returns>
        FrameworkElement GetProperyField();

        /// <summary>
        /// Is the current value default (aka should we save this value or not)
        /// </summary>
        /// <returns>Is it default?</returns>
        bool IsDefaultValue();

        /// <summary>
        /// Should we save using the expanded syntax (&lt;class.property&gt; notation)
        /// </summary>
        /// <returns></returns>
        bool IsExpanded();

        /// <summary>
        /// Get the value (as a string) of the current property
        /// </summary>
        /// <returns>Current value of the property</returns>
        string GetValue();
    }

    [global::System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class DesignerAttribute : Attribute
    {
        public DesignerAttribute(Type DesignerType)
        {
            this.DesignerType = DesignerType;
        }

        public Type DesignerType { get; private set; }
    }

    namespace DesignUtils
    {
        public static class DesignUtils
        {
            public static FrameworkElement GetDesignerField(Type type, object Object, PropertyInfo Property)
            {
                var o = Activator.CreateInstance(type);
                if (o is IDesigner)
                {
                    IDesigner d = o as IDesigner;
                    d.Initialize(Object, Property);
                    var r = d.GetProperyField();
                    r.Tag = d;
                    return r;
                }
                return null;
            }

            public static FrameworkElement GetDesignerField(object Object, PropertyInfo Property)
            {
                return GetDesignerField(GetDesignerType(Property.PropertyType), Object, Property);
            }

            public static Type GetDesignerType(Type type)
            {
                if (type == typeof(Brush))
                    return typeof(BrushDesigner);
                if (type == typeof(Color))
                    return typeof(ColorDesigner);
                foreach (var at in type.GetCustomAttributes(typeof(WPF.Design.DesignerAttribute), true))
                    return (at as WPF.Design.DesignerAttribute).DesignerType;
                return null;
            }
        }
    }
}
