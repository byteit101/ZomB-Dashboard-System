/*
 * ZomB Dashboard System <http://firstforge.wpi.edu/sf/projects/zombdashboard>
 * Copyright (C) 2011, Patrick Plenefisch and FIRST Robotics Team 451 "The Cat Attack"
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
using Microsoft.Win32;
using System.EnterpriseServices.Internal;

namespace NullGEN
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "-uninstall")
            {
                Uninstall();
                return;
            }
            if (args.Length < 1)
                Console.WriteLine("Press Y to ngen this exe");
            if (args.Length > 0 || Console.ReadKey().Key == ConsoleKey.Y)
            {
                Process.Start(Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\.NETFramework")
                                .GetValue("InstallRoot", @"C:\WINDOWS\Microsoft.NET\Framework\").ToString() + "v2.0.50727\\ngen.exe",
                                "install \"" + Process.GetCurrentProcess().MainModule.FileName + "\"").WaitForExit();
            }
        }
        static void AppeaseNgen()
        {
            //wpf stuff
            //PreCore
            if (System.Windows.Media.Brushes.Aqua.CanFreeze)
            {
                System.Windows.FrameworkElement fe = null;
                if (fe == null)
                    fe = new System.Windows.Controls.Button();
            }
            System.Windows.Automation.Provider.NavigateDirection nd = System.Windows.Automation.Provider.NavigateDirection.FirstChild;
            if (nd != System.Windows.Automation.Provider.NavigateDirection.Parent)
                nd = System.Windows.Automation.Provider.NavigateDirection.LastChild;
            Type t = typeof(System.Configuration.Install.AssemblyInstaller);

        }

        public static Process NGenu()
        {
            return NGenu(Process.GetCurrentProcess().MainModule.FileName);
        }

        public static Process NGenu(string AssemblyPath)
        {
            var pi = new ProcessStartInfo(Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\.NETFramework")
                                .GetValue("InstallRoot", @"C:\WINDOWS\Microsoft.NET\Framework\").ToString() + "v2.0.50727\\ngen.exe",
                                "uninstall \"" + AssemblyPath + "\"");
            pi.UseShellExecute = false;
            pi.CreateNoWindow = true;
            return Process.Start(pi);
        }


        public static void Uninstall()
        {
            var pub = new Publish();
            pub.GacRemoveC("InTheHand.Net.Personal");
            pub.GacRemoveC("Vlc.DotNet.Core");
            pub.GacRemoveC("Vlc.DotNet.Forms");
            pub.GacRemoveC("ZomB");
            var p = NGenu("ViZ, Version=0.9.1.0, Culture=neutral, PublicKeyToken=c7d9dbcb0b13713a");
            p.WaitForExit();
            var q = NGenu("ZomB, Version=0.9.1.0, Culture=neutral, PublicKeyToken=5880636763ded5de");
            q.WaitForExit();
        }
    }
    static class PublishExtents
    {
        static internal void GacRemoveC(this Publish pub, string name)
        {
            string toout = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\ZomB").GetValue("Path", @"C:\Program Files\ZomB").ToString();
            pub.GacRemove(toout+"\\"+name+".dll");
        }
    }
}
