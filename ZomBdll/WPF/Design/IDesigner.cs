/*
 * ZomB Dashboard System <http://firstforge.wpi.edu/sf/projects/zombdashboard>
 * Copyright (C) 2009-2010, Patrick Plenefisch and FIRST Robotics Team 451 "The Cat Attack"
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System451.Communication.Dashboard.WPF.Controls.Designer;

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
                if (type == typeof(ImageSource))
                    return typeof(ImageSourceDesigner);
                foreach (var at in type.GetCustomAttributes(typeof(WPF.Design.DesignerAttribute), true))
                    return (at as WPF.Design.DesignerAttribute).DesignerType;
                return null;
            }
        }
    }
}
