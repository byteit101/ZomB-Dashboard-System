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
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace System451.Communication.Dashboard.Net.Video
{
    /// <summary>
    /// Access a Axis206, M1011, or other standard network camera
    /// </summary>
    public class MJPEGVideoSource : IDashboardVideoDataSource
    {
        public event NewImageDataRecievedEventHandler NewImageRecieved;
        public event Utils.StringFunction OnError;

        bool shouldBeRunning = false;
        Image CurImg = new Bitmap(1, 1);

        Thread eyeThread;

        /// <summary>
        /// Create a new instance of MJPEGVideoSource
        /// </summary>
        /// <param name="address">IP address of the camera</param>
        /// <param name="fps">FPS rate of the camera (0 default)</param>
        public MJPEGVideoSource(IPAddress address, int fps)
        {
            Init(address, fps);
        }

        ~MJPEGVideoSource()
        {
            Dispose();
        }

        public void Dispose()
        {
            Stop();
        }

        public bool Running { get; protected set; }
        public int Port { get; protected set; }
        public IPAddress IP { get; protected set; }
        public int FPS { get; protected set; }

        private void Init(IPAddress address, int fps)
        {
            Running = false;
            IP = address;
            Port = 80;//web!
            FPS = fps;
        }

        /// <summary>
        /// Gets a copy of the image
        /// </summary>
        /// <returns>copy of the current image</returns>
        public Image GetImage()
        {
            return (Image)CurImg.Clone();
        }

        /// <summary>
        /// Starts the WPILib Camera
        /// </summary>
        public void Start()
        {
            if (!Running)
            {
                try
                {
                    if (eyeThread != null)
                        eyeThread.Abort();

                    shouldBeRunning = true;
                    eyeThread = new Thread(GraveWorker);
                    eyeThread.IsBackground = true;
                    eyeThread.Start();
                    Running = true;
                }
                catch (Exception ex)
                {
                    er("An error has occured: " + ex.ToString());
                }
            }
        }

        /// <summary>
        /// Stops the WPILib Camera
        /// </summary>
        public void Stop()
        {
            if (Running)
            {
                try
                {
                    shouldBeRunning = false;
                }
                catch { }
            }
        }

        /// <summary>
        /// This is a grave joke
        /// </summary>
        private void GraveWorker()
        {
            //while (ZomB.IsUnDead)
            //{
            //    GravePlot gp = ZomB.GetNextGraveYard().GetNextAvaliblePlot();
            //    gp.Dig1Foot();
            //    gp.Dig1Foot();
            //    gp.Dig1Foot();
            //    gp.Dig1Foot();
            //    gp.Dig1Foot();
            //    gp.Dig1Foot();
            //    gp.GetPit().Add(ZomB.GetNextPiece())
            //    gp.FillPit();
            //    ZomB.GraveYards.MoveNext();
            //}
            while (shouldBeRunning)
            {
            tryagain:
                //http://myserver/axis-cgi/mjpg/video.cgi
                //produces:
                //Content-Type: multipart/x-mixed-replace; boundary=myboundary
                //
                //--myboundary
                //Content-Type: image/jpeg
                //Content-Length: 123
                //
                //<JPEG image data> 
                //--myboundary
                //Content-Type: image/jpeg 
                //Content-Length: 456 
                //
                //<JPEG image data> 
                //--myboundary

                //                User-Agent: HTTPStreamClient\n\
                //Connection: Keep-Alive\n\
                //Cache-Control: no-cache\n\
                //Authorization: Basic RlJDOkZSQw==\n\n";
                HttpWebRequest hrq = (HttpWebRequest)HttpWebRequest.Create("http://" + IP.ToString() + "/axis-cgi/mjpg/video.cgi?fps=" + FPS.ToString());
                hrq.UserAgent = "ZomB/" + System451.Communication.Dashboard.ZVersionMgr.ShortNumber + " (Streaming Client)";
                hrq.Credentials = new NetworkCredential("FRC", "FRC");
                Stream ns;
                StreamReader sr;
                try
                {
                    //attach limb
                    HttpWebResponse resp = (HttpWebResponse)hrq.GetResponse();
                    ns = resp.GetResponseStream();

                    if (resp.StatusCode != HttpStatusCode.OK)
                    {
                        Thread.Sleep(1000);
                        er("yaah! retrying!");
                    }
                    else
                    {
                        string boundrywaters = resp.ContentType.Substring(resp.ContentType.IndexOf("boundary=") + 9);
                        if (boundrywaters.Substring(0, 2) == "--")
                            boundrywaters = boundrywaters.Substring(2);
                        Running = true;
                        int image_size = 0;
                        char[] buf;
                        sr = new StreamReader(ns, new binencode());
                        while (shouldBeRunning)
                        {
                            Thread.Sleep(1);//let it pile up!

                            string sbuf = null;
                            int iiii = 0;
                            while (sbuf != "--" + boundrywaters)
                            {
                                sbuf = sr.ReadLine();
                                iiii += sbuf.Length;
                                if (sbuf == null || iiii > 4000000)//4MB? we fail!
                                    goto tryagain;
                            }
                            sbuf = sr.ReadLine();
                            while (!sbuf.StartsWith("Content-Type", StringComparison.CurrentCultureIgnoreCase))
                            {
                                sbuf = sr.ReadLine();
                            }
                            sbuf = sr.ReadLine();
                            image_size = int.Parse(sbuf.Substring(sbuf.IndexOf(": ") + 2));//Content-Length
                            sbuf = sr.ReadLine();//clear the next line

                            // image size being less than zero wouldn't make sense, nor larger than buffer
                            if (image_size > 0 && image_size <= 2048576)
                            {
                                buf = new char[image_size];
                                Thread.Sleep(1);
                                byte[] bbuf = new byte[image_size];
                                int red = 0;
                                while (red < image_size)
                                {
                                    int tred = sr.Read(buf, red, image_size - red);
                                    if (tred < 1)
                                        break;
                                    red += tred;
                                }

                                for (int i = 0; i < image_size; i++)
                                {
                                    bbuf[i] = (byte)buf[i];
                                }

                                //Save Image
                                CurImg = Bitmap.FromStream(new MemoryStream(bbuf));
                                if (NewImageRecieved != null)
                                    NewImageRecieved(this, new NewImageDataRecievedEventArgs(CurImg, new MemoryStream(bbuf)));
                            }
                            else
                                er("Unknown size");
                        }
                    }
                }
                catch
                {
                    Running = false;
                    return;//TODO: figure out better way to dispose of bad sockets when we are not connected to the bot
                }
            }
        }

        /// <summary>
        /// Throw/Fire an error
        /// </summary>
        /// <param name="msg">le message</param>
        protected void er(string msg)
        {
            if (OnError != null)
            {
                OnError(msg);
            }
        }
    }

    public class binencode : Encoding
    {
        public override int GetByteCount(char[] chars, int index, int count)
        {
            return count;
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            for (int i = 0; i < charCount; i++)
            {
                bytes[byteIndex + i] = (byte)chars[charIndex + i];
            }
            return charCount;
        }

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return count;
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            for (int i = 0; i < byteCount; i++)
            {
                chars[charIndex + i] = (char)bytes[byteIndex + i];
            }
            return byteCount;
        }

        public override int GetMaxByteCount(int charCount)
        {
            return charCount;
        }

        public override int GetMaxCharCount(int byteCount)
        {
            return byteCount;
        }
    }
}
