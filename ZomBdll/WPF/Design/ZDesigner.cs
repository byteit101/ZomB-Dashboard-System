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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace System451.Communication.Dashboard
{
    public static class ZDesigner
    {
        static ISurfaceDesigner designr = null;

        public static bool IsDesignMode
        {
            get
            {
                return designr != null || (System.ComponentModel.LicenseManager.UsageMode != System.ComponentModel.LicenseUsageMode.Runtime);
            }
        }

        public static bool IsRunMode
        {
            get
            {
                return designr == null || (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Runtime);
            }
        }

        public static void SetDesigner(ISurfaceDesigner designer)
        {
            if (designr == null)
                designr = designer;
            else if (designer != designr)
                throw new InvalidOperationException("Only one designer can be active at one time");
        }

        public static ISurfaceDesigner GetDesigner()
        {
            return designr;
        }

        public static UIElement[] GetChildren()
        {
            return GetDesigner().GetChildren();
        }

        public static bool ChildrenContain(string name)
        {
            return GetDesigner().ChildrenContain(name);
        }

        public static bool ChildrenContain(UIElement element)
        {
            return GetDesigner().ChildrenContain(element);
        }

        public static UIElement GetChildByName(string name)
        {
            return GetDesigner().GetChildByName(name);
        }

        public static int TeamNumber
        {
            get
            {
                return GetDesigner().GetTeamNumber();
            }
        }
    }

    public interface ISurfaceDesigner
    {
        UIElement[] GetChildren();
        bool ChildrenContain(string name);
        bool ChildrenContain(UIElement element);
        UIElement GetChildByName(string name);
        int GetTeamNumber();
    }
}
