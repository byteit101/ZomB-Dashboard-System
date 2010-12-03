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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System451.Communication.Dashboard.Libs.AviFile;

namespace System451.Communication.Dashboard
{
    namespace Utils
    {
        /// <summary>
        /// Saves a video
        /// </summary>
        public class VideoStreamSaver : IZomBDataSaver
        {
            Queue<string> pubicQueue { get; set; }
            Queue<string> privateQueue { get; set; }
            Queue<Bitmap> imageQueue { get; set; }

            ISavableZomBData source;

            Thread saber;
            bool saving;

            public VideoStreamSaver(ISavableZomBData DataSource)
            {
                this.Add(DataSource);
                pubicQueue = new Queue<string>();
                privateQueue = new Queue<string>();
                imageQueue = new Queue<Bitmap>();
                PrefixBindings = false;
                saber = new Thread(aviSaverbg);
                saber.IsBackground = true;
                FPS = 15;
            }

            #region IZomBDataSaver Members

            /// <summary>
            /// Ignore this
            /// </summary>
            public bool PrefixBindings
            {
                get;
                set;
            }

            public double FPS { get; set; }

            public void Add(ISavableZomBData DataSource)
            {
                if (source == null)
                {
                    source = DataSource;
                    source.DataUpdated += new EventHandler(source_DataUpdated);
                }
            }

            void source_DataUpdated(object sender, EventArgs e)
            {
                if (saving)
                    lock (pubicQueue)
                    {
                        pubicQueue.Enqueue(source.DataValue);
                    }
            }

            public void StartSave(string file)
            {
                if (!saving)
                {
                    saber = new Thread(aviSaverbg);
                    saber.IsBackground = true;
                    saber.Start(file);
                }
            }

            public void StartSave()
            {

            }

            public void EndSave()
            {
                saving = false;
            }

            private void aviSaverbg(object file)
            {
                string filename = file.ToString();
                pubicQueue.Clear();
                privateQueue.Clear();
                imageQueue.Clear();
                saving = true;
                Net.Video.AviStreamer srm = null;
                while (saving)
                {
                    Thread.Sleep(750);
                    while (pubicQueue.Count < 1 && saving)
                    {
                        Thread.Sleep(50);
                    }
                    if (!saving)
                    {
                        srm.Close();
                        return;
                    }
                    while (pubicQueue.Count > 0)
                    {
                        lock (pubicQueue)
                        {
                            privateQueue.Enqueue(pubicQueue.Dequeue());
                        }
                    }
                    Thread.Sleep(50);
                    while (privateQueue.Count > 0)
                    {
                        //We should honor them, but lets not, as we should have a imageconverter
                        //imageQueue.Enqueue((Bitmap)source.GetTypeConverter().ConvertFrom(null, System.Globalization.CultureInfo.CurrentCulture, privateQueue.Dequeue(), typeof(Image)));
                        imageQueue.Enqueue((Bitmap)((new Net.Video.BitmapConverter()).ConvertFromString(privateQueue.Dequeue())));
                    }
                    if (srm == null)
                        srm = new Net.Video.AviStreamer(filename, FPS, imageQueue.Dequeue());
                    Thread.Sleep(50);
                    while (imageQueue.Count > 0)
                    {
                        srm.Add(imageQueue.Dequeue());
                    }
                    Thread.Sleep(500);
                }
                srm.Close();
            }

            #endregion
        }
    }

    namespace Net.Video
    {
        /// <summary>
        /// Here for the Video Saver
        /// </summary>
        public class BitmapConverter : TypeConverter
        {
            public BitmapConverter() { }
            public string ConvertToString(Image value)
            {
                MemoryStream imageStream = new MemoryStream();
                value.Save(imageStream, ImageFormat.Jpeg);
                return Convert.ToBase64String(imageStream.ToArray());
            }
            new public Image ConvertFromString(string value)
            {
                byte[] imgs = Convert.FromBase64String(value);
                return Image.FromStream(new MemoryStream(imgs));
            }
        }

        /// <summary>
        /// Creates an easy to use class that saves mp4 files (TO BE RENAMED SOON)
        /// </summary>
        public class AviStreamer : IDisposable
        {
            Stream vs;
            StreamReader bs;

            /// <summary>
            /// Creates a new AVI Streamer
            /// </summary>
            /// <param name="filename">The file to save the avi to</param>
            /// <param name="fps">The framerate</param>
            /// <param name="first">The first frame</param>
            public AviStreamer(string filename, double fps, Bitmap first)
            {
                vs = FFmpeg.GetEncoderStream(filename, (int)fps);
                Add(first);
            }

            ~AviStreamer()
            {
                Dispose();
            }

            /// <summary>
            /// Dispose of the streamer
            /// </summary>
            public void Dispose()
            {
                try
                {
                    vs.Close();
                }
                catch
                {

                }
            }

            /// <summary>
            /// Add another frame
            /// </summary>
            /// <param name="nextFrame">The next frame</param>
            public void Add(Bitmap nextFrame)
            {
                System.Diagnostics.Debug.Print("Adding...");
                MemoryStream ms = new MemoryStream();
                nextFrame.Save(ms, ImageFormat.Jpeg);
                ms.WriteTo(vs);
                if (bs != null)
                {
                    //if (!bs.EndOfStream)
                    //{
                        string line = bs.ReadLine();
                    //}
                }
            }

            /// <summary>
            /// Save and close the AVI file
            /// </summary>
            public void Close()
            {
                Dispose();
            }
        }
    }
}