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
using System.Diagnostics;
using Microsoft.Win32;
using System.EnterpriseServices.Internal;

namespace System451.Communication.Dashboard.Utils
{
    public static class InstallUtils
    {
        public static Process NGen()
        {
            return NGen(Process.GetCurrentProcess().MainModule.FileName);
        }

        public static Process NGen(string AssemblyPath)
        {
            var pi = new ProcessStartInfo(Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\.NETFramework")
                                .GetValue("InstallRoot", @"C:\WINDOWS\Microsoft.NET\Framework\").ToString() + "v2.0.50727\\ngen.exe",
                                "install \"" + AssemblyPath + "\"");
            pi.UseShellExecute = false;
            pi.CreateNoWindow = true;
            return Process.Start(pi);
        }

        public static void ExtractAll()
        {
            AutoExtractor.Extract(AutoExtractor.Files.All);
        }

        public static void Install()
        {
            Install(false);
        }

        public static void Install(bool async)
        {
            ExtractAll();
            //HACK CONF: update when assemblies change
            var p = NGen("ZomB, Version=0.7.1.0, Culture=neutral, PublicKeyToken=5880636763ded5de");
            p.WaitForExit();
            var pub = new Publish();
            pub.GacInstall("InTheHand.Net.Personal.dll");
            pub.GacInstall("SlimDX.dll");
            pub.GacInstall("Vlc.DotNet.Core.dll");
            pub.GacInstall("Vlc.DotNet.Forms.dll");
            pub.GacInstall("ZomB.dll");
            p = NGen("ZomB, Version=0.7.1.0, Culture=neutral, PublicKeyToken=5880636763ded5de");
            if (!async)
                p.WaitForExit();
            var q = NGen();
            if (!async)
                q.WaitForExit();
        }
    }
}
