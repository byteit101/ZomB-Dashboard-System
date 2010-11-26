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
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace System451.Communication.Dashboard.Utils
{
    public class AeroGlass
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct MARGINS
        {
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;
        }
 
        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern bool DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);
 
        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern bool DwmIsCompositionEnabled();

        public static bool GlassifyHwnd(IntPtr ptr)
        {
            if (Environment.OSVersion.Version.Major >= 6 && DwmIsCompositionEnabled())
            {
                MARGINS margins = new MARGINS();
                margins.Left = -1;
                margins.Right = -1;
                margins.Top = -1;
                margins.Bottom = -1;
                return !DwmExtendFrameIntoClientArea(ptr, ref margins);
            }
            return false;
        }

        public static bool GlassifyWindow(Window win)
        {
            if (Environment.OSVersion.Version.Major >= 6 && DwmIsCompositionEnabled())
            {
                IntPtr mainWindowPtr = new WindowInteropHelper(win).Handle;
                HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
                mainWindowSrc.CompositionTarget.BackgroundColor = Colors.Transparent;
                win.Background = Brushes.Transparent;
                return GlassifyHwnd(mainWindowSrc.Handle);
            }
            return false;
        }
    }
}
