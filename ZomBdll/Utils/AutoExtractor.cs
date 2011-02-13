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
using System.IO;
using System.Reflection;
using System451.Communication.Dashboard.Properties;

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
                [Obsolete("slimDX is not in this dll, download it fom the interwebs")]
                SlimDX = 0x4,
                VLC = 0x8
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
                    {
                        if (CPU.Is64BitOperatingSystem())
                            File.WriteAllBytes("32feetWidcomm.dll", Resources._32feetWidcommx64);
                        else
                            File.WriteAllBytes("32feetWidcomm.dll", Resources._32feetWidcommx86);
                    }
                }
                if ((files & Files.VLC) == Files.VLC)
                {
                    if (!File.Exists("libvlc.dll"))
                        File.WriteAllBytes("libvlc.dll", Resources.libvlc);
                    if (!File.Exists("libvlccore.dll"))
                        File.WriteAllBytes("libvlccore.dll", Resources.libvlccore);
                    if (!File.Exists("Vlc.DotNet.Core.dll"))
                        File.WriteAllBytes("Vlc.DotNet.Core.dll", Resources.Vlc_DotNet_Core);
                    if (!File.Exists("Vlc.DotNet.Forms.dll"))
                        File.WriteAllBytes("Vlc.DotNet.Forms.dll", Resources.Vlc_DotNet_Forms);
                }
            }

            public static Assembly AssemblyResolve(object sender, ResolveEventArgs e)
            {
                var dll = e.Name.Replace("\\\\", "\\");

                if (dll.ToLower().Contains("inthehand.net.personal"))
                        Extract(Files.InTheHandManaged | Files.InTheHandNative);
                    else if (dll.ToLower().Contains("vlc"))
                        Extract(Files.VLC);
                else
                    return null;
                    try
                    {
                        var asm = Assembly.Load(dll);
                        return asm;
                    }
                    catch { }
                    return null;
            }
        }
    }
}