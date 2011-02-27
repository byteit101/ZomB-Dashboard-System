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
//#define UseFile //uncomment this to debug a little easier, put ni-rt.ini in C:\Program files\ZomB\Bindings or edit the paths below
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace OutStaller
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                try
                {
                    Application.Run(new Form1());
                    return 0;
                }
                catch { return -100; }
            }

            string file = Path.GetFullPath(args[1]);
            if (!File.Exists(file))
                return -2;
            IPAddress iad;
            if (!IPAddress.TryParse(args[0], out iad))
            {
                return -3;
            }
            DoInstall(file, "ftp://" + args[0] + "/");
            return 0;
        }

        internal static int DoInstall(string file, string iad)
        {
#if !UseFile
            int r = ZomBDownload(file, iad + "ni-rt/system/ZomB.out");
#else
            int r = 0;
#endif
            if (r == 0)
                return IniConfig(iad + "ni-rt.ini");
            return r;
        }

        private static int ZomBDownload(string file, string iad)
        {
            try
            {
                FtpWebRequest request = FtpWebRequest.Create(iad) as FtpWebRequest;
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = false;
                request.Method = WebRequestMethods.Ftp.UploadFile;


                using (Stream responseStream = request.GetRequestStream())
                {
                    byte[] buffer = File.ReadAllBytes(file);
                    responseStream.Write(buffer, 0, buffer.Length);
                }
            }
            catch
            {
                return -10;
            }
            return 0;
        }

        private static int IniConfig(string iad)
        {
            try
            {
                MemoryStream ms = new MemoryStream(4096);
                MemoryStream rms = new MemoryStream(4096);
                byte[] bytes;
#if !UseFile
                FtpWebRequest request = FtpWebRequest.Create(iad) as FtpWebRequest;
                request.UsePassive = true;
                request.UseBinary = false;
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                using (FtpWebResponse response = request.GetResponse() as FtpWebResponse)
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
#else
                {
                    using (Stream responseStream = File.OpenRead("C:\\Program Files\\ZomB\\Bindings\\ni-rt.ini"))
                    {
#endif
                        while (true)
                        {
                            Thread.Sleep(100);
                            byte[] buf = new byte[1024];
                            int read = responseStream.Read(buf, 0, buf.Length);
                            if (read < 1)
                                break;
                            else
                                rms.Write(buf, 0, read);
                        }
                    }
                }
                rms.Seek(0, SeekOrigin.Begin);
                int len, rlen = (int)rms.Length;
                using (StreamReader sr = new StreamReader(rms))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (line.StartsWith("StartupDlls") && !line.Contains("ZomB.out"))
                        {
                            if (line.Contains("FRC_UserProgram.out"))
                                line = line.Replace("FRC_UserProgram.out", "ZomB.out;FRC_UserProgram.out");//C++
                            else if (line.Contains("FRC_JavaVM.out"))
                                line = line.Replace("FRC_JavaVM.out", "ZomB.out;FRC_JavaVM.out");//Java
                            else //LabVIEW
                                line += "ZomB.out;";
                        }
                        WriteLine(ms, line);
                    }
                    bytes = ms.GetBuffer();
                    len = (int)ms.Length;
                }
                if (len < rlen)
                    return -50;
#if !UseFile
                FtpWebRequest upload = FtpWebRequest.Create(iad) as FtpWebRequest;
                upload.UsePassive = true;
                upload.KeepAlive = false;
                upload.Method = WebRequestMethods.Ftp.Rename;
                upload.RenameTo = "ni-rt.backup-" + DateTime.Now.ToFileTimeUtc() + ".ini";
                try
                {
                    upload.GetResponse().Close();
                }
                catch { }

                upload = FtpWebRequest.Create(iad) as FtpWebRequest;
                upload.UsePassive = true;
                upload.UseBinary = false;
                upload.KeepAlive = false;
                upload.Method = WebRequestMethods.Ftp.UploadFile;
                using (Stream responseStream = upload.GetRequestStream())
#else
                using (Stream responseStream = File.Create("C:\\Program Files\\ZomB\\Bindings\\ni-rt2.ini"))
#endif
                {
                    responseStream.Write(bytes, 0, len);
                }
            }
            catch
            {
                return -11;
            }
            return 0;
        }
        public static void WriteLine(MemoryStream ms, string line)
        {
            byte[] ar = ASCIIEncoding.ASCII.GetBytes(line + "\r\n");
            ms.Write(ar, 0, ar.Length);
        }
    }
}
