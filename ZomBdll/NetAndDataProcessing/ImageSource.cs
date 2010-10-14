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
using System.Text;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace System451.Communication.Dashboard.Net.Video
{
    /// <summary>
    /// Access a WPILib camera
    /// </summary>
    public class WPILibTcpVideoSource : IDashboardVideoDataSource
    {
        // axis camera sends video in the following order :
        // header { 0x1, 0x0, 0x0, 0x0 } raw bytes - no endianness
        // image size { 4 bytes, variable data } - big endian
        // image data { variable size, variable data } - always big endian

        public event NewImageDataRecievedEventHandler NewImageRecieved;
        public event Utils.StringFunction OnError;

        bool shouldBeRunning = false;
        Image CurImg = new Bitmap(1, 1);

        TcpClient eyeSocket;//ZomB Humor
        Thread eyeThread;//"stretched" //ZomB Humor
        //OMG, those puns are going to kill ZomB, oh wait, it can't be killed,
        //its Un-Dead! Well, they will cause quite a bit of groaning at how bad
        //the past four lines are. Come on, don't you like humor? and tibias?
        //and fibulas? and, never mind. Sigh

        /// <summary>
        /// Create a new instance of WPILibTcpVideoImageSource
        /// </summary>
        /// <param name="team">Team number, for IP</param>
        /// <param name="instance">The camera number</param>
        public WPILibTcpVideoSource(int team, int instance)
        {
            Init(team, instance);
        }

        /// <summary>
        /// Create a new instance of WPILibTcpVideoImageSource
        /// </summary>
        /// <param name="team">Team number, for IP</param>
        public WPILibTcpVideoSource(int team)
        {
            Init(team, 1);
        }
        ~WPILibTcpVideoSource()
        {
            Stop();
        }

        public bool Running { get; protected set; }
        public int Port { get; protected set; }
        public IPAddress IP { get; protected set; }

        private void Init(int team, int instance)
        {
            Running = false;
            //10.xx.yy.2, 451=10.4.51.2
            IP = IPAddress.Parse("10." + ((int)(team / 100)) + "." + ((int)(team % 100)) + ".2");
            Port = 1179 + instance;//default: 1180
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
                    if (eyeSocket != null)
                        eyeSocket.Close();
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
                NetworkStream istream = null;
                try
                {
                    //attach limb
                    eyeSocket = new TcpClient(new IPEndPoint(IPAddress.Any, Port));
                    eyeSocket.ReceiveBufferSize = 2048576;//2mb
                    eyeSocket.Connect(IP, Port);
                    istream = eyeSocket.GetStream();

                    Running = true;
                    int image_size = 0;
                    byte[] buf;
                    while (shouldBeRunning)
                    {
                        Thread.Sleep(5);//gather data in RecieveQ, don't overload

                        //switch, but without the compare overhead, and with fall through
                        //based off IL optimizations in LR's .net Reflector

                        while (eyeSocket.Available < 4)
                        {
                            Thread.Sleep(1);
                        }

                        buf = new byte[4];
                        istream.Read(buf, 0, 4);

                        //correct header?
                        //if any of the bytes are wrong
                        if ((buf[0] != 1) || (buf[1] != 0) || (buf[2] != 0) || (buf[3] != 0))
                        {
                            //find the header
                            //this goes down the line until one of the conditions fails, then starts over
                            while ((istream.ReadByte() != 1) || (istream.ReadByte() != 0) || (istream.ReadByte() != 0) || (istream.ReadByte() != 0))
                            {
                            }
                        }
                        //Got correct header, wait for next bit
                        while (eyeSocket.Available < 4)
                        {
                            Thread.Sleep(1);
                        }

                        //buf = new byte[4];//not strictly necesarry, we just had 4 byte header
                        istream.Read(buf, 0, 4);
                        image_size = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buf, 0));
                        // image size being less than zero wouldn't make sense, nor larger than buffer
                        if (image_size > 0 && image_size <= 2048576)
                        {
                            buf = new byte[image_size];
                            while (eyeSocket.Available < image_size)
                            {
                                Thread.Sleep(1);
                            }

                            istream.Read(buf, 0, image_size);
                            image_size = 0;

                            //Save Image
                            CurImg = Bitmap.FromStream(new MemoryStream(buf));
                            if (NewImageRecieved != null)
                                NewImageRecieved(this, new NewImageDataRecievedEventArgs(CurImg, new MemoryStream(buf)));
                        }
                        else
                            er("Unknown size");
                    }
                }
                catch
                {
                    Running = false;
                }
                finally
                {
                    try
                    {
                        eyeSocket.Close();
                        if (istream != null)
                            istream.Close();
                    }
                    catch { }
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

    /// <summary>
    /// Represents an abstract data source that carries video
    /// </summary>
    public interface IDashboardVideoDataSource
    {
        /// <summary>
        /// Gets the current Image
        /// </summary>
        /// <returns>The current Image</returns>
        Image GetImage();
        /// <summary>
        /// Starts the video source
        /// </summary>
        void Start();
        /// <summary>
        /// Ends the video source
        /// </summary>
        void Stop();
        /// <summary>
        /// Is the control running?
        /// </summary>
        bool Running { get; }
        /// <summary>
        /// Fires when a new image is recieved
        /// </summary>
        event NewImageDataRecievedEventHandler NewImageRecieved;
    }

    public delegate void NewImageDataRecievedEventHandler(object sender, NewImageDataRecievedEventArgs e);
    public class NewImageDataRecievedEventArgs : EventArgs
    {
        public NewImageDataRecievedEventArgs(Image data, Stream streamdata)
        {
            NewData = data;
            NewDataStream = streamdata;
        }
        public Image NewData { get; set; }
        public Stream NewDataStream { get; set; }
    }
}
