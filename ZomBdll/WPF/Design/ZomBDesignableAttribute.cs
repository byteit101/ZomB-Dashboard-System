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

namespace System451.Communication.Dashboard.WPF.Design
{
    [global::System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class ZomBDesignableAttribute : Attribute
    {
        public ZomBDesignableAttribute() { }

        /// <summary>
        /// What to show as the field in the designer
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The 1 based index of position in the list. Do not use 1.
        /// </summary>
        public uint Index { get; set; }

        /// <summary>
        /// Is this property updated via the design surface?
        /// </summary>
        public bool Dynamic { get; set; }
    }

    [global::System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class ZomBDesignablePropertyAttribute : ZomBDesignableAttribute
    {
        /// <summary>
        /// Create a new ZomBDesignableAttribute on a property
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        public ZomBDesignablePropertyAttribute(string propertyName) { PropertyName = propertyName; }

        /// <summary>
        /// The name of the Property
        /// </summary>
        public string PropertyName { get; private set; }
    }
}
