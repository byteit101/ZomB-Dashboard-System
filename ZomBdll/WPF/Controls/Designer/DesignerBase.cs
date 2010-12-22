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
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System451.Communication.Dashboard.WPF.Design;

namespace System451.Communication.Dashboard.WPF.Controls.Designers
{
    public abstract class DesignerBase : IDesigner
    {
        protected object Object { get; private set; }
        protected PropertyInfo Property { get; private set; }

        #region IDesigner Members

        public virtual void Initialize(object obj, PropertyInfo property)
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

        public virtual bool IsExpanded()
        {
            return false;
        }

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

        protected T GetVaueAsType<T>() where T : class
        {
            return Property.GetValue(Object, null) as T;
        }
        protected void Set(object value)
        {
            Property.SetValue(Object, value, null);
        }
    }
}
