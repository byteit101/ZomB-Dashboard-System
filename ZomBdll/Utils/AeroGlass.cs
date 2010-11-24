using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows;

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
