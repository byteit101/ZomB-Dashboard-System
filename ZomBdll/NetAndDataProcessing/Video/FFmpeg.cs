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
using System.Diagnostics;
using System.IO;

namespace System451.Communication.Dashboard.Net.Video
{
    public static class FFmpeg
    {
        //Note: to update ffmpeg, find a build at http://ffmpeg.arrozcru.org/

        public static Stream GetEncoderStream(string fileName, int fps)
        {
            return exec("-f image2pipe -vcodec mjpeg -r " + fps + " -i - \"" + fileName + "\"").StandardInput.BaseStream;
        }

        public static Process exec(string args)
        {
            //hopefully we have ffmpeg in the current directory
            ProcessStartInfo psi = new ProcessStartInfo("ffmpeg.exe", args);
            psi.UseShellExecute = false;
            psi.RedirectStandardInput = true;
            //psi.RedirectStandardError = true;
            try
            {
                Process p = Process.Start(psi);
                return p;
            }
            catch { }
            return null;
        }
    }
}
