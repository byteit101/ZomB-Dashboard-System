using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Diagnostics;
//using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System451.Communication.Dashboard.Utils;
using System.Windows.Input;

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
