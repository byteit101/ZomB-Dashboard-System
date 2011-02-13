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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System451.Communication.Dashboard.Utils;

namespace System451.Communication.Dashboard.Net.DriverStation
{
    class KeySafety
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc llkpr = HookCallback;
        private static IntPtr hook = IntPtr.Zero;
        private static VoidFunction enable, disable, estop;

        public static void Start(VoidFunction Enable, VoidFunction Disable, VoidFunction EStop)
        {
            enable = Enable;
            disable = Disable;
            estop = EStop;
            hook = SetHook(llkpr);
        }

        public static void Stop()
        {
            UnhookWindowsHookEx(hook);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc llkproc)
        {
            using (Process curproc = Process.GetCurrentProcess())
            {
                using (ProcessModule mod = curproc.MainModule)
                {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, llkproc, GetModuleHandle(mod.ModuleName), 0);
                }
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                Key vkCode = (Key)Marshal.ReadInt32(lParam);
                if (vkCode == Key.Delete)//Space, go figure
                {
                    disable();
                }
                else if (vkCode == Key.F23)//F1, Go figure
                {
                    enable();
                }
                else if (vkCode == Key.Escape && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
                {
                    estop();
                }
            }
            return CallNextHookEx(hook, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
