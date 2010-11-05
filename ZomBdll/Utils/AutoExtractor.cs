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
using System.IO;
using System451.Communication.Dashboard.Properties;
using System;

namespace System451.Communication.Dashboard
{
    namespace Utils
    {
        public class AutoExtractor
        {
            public enum Files
            {
                All = 0xffff,
                InTheHandManaged = 0x1,
                InTheHandNative = 0x2,
                SlimDX = 0x4
            }
            public static void Extract(Files files)
            {
                if ((files & Files.InTheHandManaged) == Files.InTheHandManaged)
                {
                    if (!File.Exists("InTheHand.Net.Personal.dll"))
                        File.WriteAllBytes("InTheHand.Net.Personal.dll", Resources.InTheHand_Net_Personal);
                }
                if ((files & Files.InTheHandNative) == Files.InTheHandNative)
                {
                    if (!File.Exists("32feetWidcomm.dll"))
                        File.WriteAllBytes("32feetWidcomm.dll", Resources._32feetWidcomm);
                }
                if ((files & Files.SlimDX) == Files.SlimDX)
                {
                    if (CPU.Is64BitOperatingSystem() && File.Exists("SlimDX x64.dll"))
                    {
                        File.Copy("SlimDX x64.dll", "SlimDX.dll");
                        File.Delete("SlimDX x64.dll");
                        //TODO: Do I need to restart or reload?
                    }
                }
            }
        }
    }
}