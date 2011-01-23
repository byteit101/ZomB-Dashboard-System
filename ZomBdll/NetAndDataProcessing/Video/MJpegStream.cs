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
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

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
        public MJPEGVideoSource(IPAddress address)
        {
            Init(address);
        }

        ~MJPEGVideoSource()
        {
            Stop();
        }

        public bool Running { get; protected set; }
        public int Port { get; protected set; }
        public IPAddress IP { get; protected set; }

        private void Init(IPAddress address)
        {
            Running = false;
            IP = address;
            Port = 80;//web!
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
                HttpWebRequest hrq = (HttpWebRequest)HttpWebRequest.Create("http://" + IP.ToString() + "/axis-cgi/mjpg/video.cgi");
                hrq.UserAgent = "ZomB/0.7.1.0 (Streaming Client)";
                hrq.Connection = "Keep-Alive";
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
                        Running = true;
                        int image_size = 0;
                        char[] buf;
                        while (shouldBeRunning)
                        {
                            sr = new StreamReader(ns);
                            Thread.Sleep(1);//let it pile up!

                            string sbuf = sr.ReadLine();
                            if (!sbuf.StartsWith("Content-Type", StringComparison.CurrentCultureIgnoreCase))
                            {
                                er("yaah! ctype not first!");
                            }
                            sbuf = sr.ReadLine();
                            image_size = int.Parse(sbuf.Substring(sbuf.IndexOf(": ") + 2));//Content-Length
                            sbuf = sr.ReadLine();//clear the next line

                            // image size being less than zero wouldn't make sense, nor larger than buffer
                            if (image_size > 0 && image_size <= 2048576)
                            {
                                buf = new char[image_size];
                                Thread.Sleep(1);

                                sr.Read(buf, 0, image_size);
                                image_size = 0;
                                byte[] bbuf = new byte[image_size];
                                Buffer.BlockCopy(bbuf, 0, bbuf, 0, image_size);

                                //Save Image
                                CurImg = Bitmap.FromStream(new MemoryStream(bbuf));
                                if (NewImageRecieved != null)
                                    NewImageRecieved(this, new NewImageDataRecievedEventArgs(CurImg, new MemoryStream(bbuf)));
                                while (sr.ReadLine() != ("--" + boundrywaters))
                                    ;
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
}
