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
using Microsoft.Win32;

namespace NullGEN
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
                Console.WriteLine("Press Y to ngen this exe");
            if (args.Length > 0 || Console.ReadKey().Key == ConsoleKey.Y)
            {
                Process.Start(Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\.NETFramework")
                                .GetValue("InstallRoot", @"C:\WINDOWS\Microsoft.NET\Framework\").ToString() + "v2.0.50727\\ngen.exe",
                                "install \"" + Process.GetCurrentProcess().MainModule.FileName + "\"").WaitForExit();
            }
        }
    }
}
