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
using System.Text;
using System.Threading;
using System.Windows.Media.Imaging;
using System451.Communication.Dashboard.Net.Video;

namespace System451.Communication.Dashboard
{
    namespace Utils
    {
        /// <summary>
        /// Saves a video from a data source
        /// </summary>
        public class VideoStreamSaver : IZomBDataSaver
        {
            Queue<string> pubicQueue { get; set; }
            Queue<string> privateQueue { get; set; }
            Queue<MemoryStream> imageQueue { get; set; }

            ISavableZomBData source;

            Thread saber;
            bool saving;
            VideoEncoder srm = null;

            /// <summary>
            /// Create a new VideoStreamSaver from the specified data source
            /// </summary>
            /// <param name="DataSource">The data source we monitor</param>
            public VideoStreamSaver(ISavableZomBData DataSource)
            {
                (this as IZomBDataSaver).Add(DataSource);
                pubicQueue = new Queue<string>();
                privateQueue = new Queue<string>();
                imageQueue = new Queue<MemoryStream>();
                saber = new Thread(aviSaverbg);
                saber.IsBackground = true;
                FPS = 15;
            }

            ~VideoStreamSaver()
            {
                if (srm != null)
                {
                    try
                    {
                        srm.Close();
                    }
                    catch { }
                }
            }

            #region IZomBDataSaver Members

            /// <summary>
            /// Gets or sets the FPS of the resulting file
            /// </summary>
            public float FPS { get; set; }

            void IZomBDataSaver.Add(ISavableZomBData DataSource)
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

            /// <summary>
            /// Start saving to the specified file
            /// </summary>
            /// <param name="file">File to save to. Type inferred from extension</param>
            public void StartSave(string file)
            {
                if (!saving)
                {
                    saber = new Thread(aviSaverbg);
                    saber.IsBackground = true;
                    saber.Start(file);
                }
            }

            /// <summary>
            /// Close the file and end the save
            /// </summary>
            public void EndSave()
            {
                saving = false;
            }

            private void aviSaverbg(object file)
            {
                try
                {

                    string filename = file.ToString();
                    pubicQueue.Clear();
                    privateQueue.Clear();
                    imageQueue.Clear();
                    saving = true;
                    while (saving)
                    {
                        Thread.Sleep(750);
                        while (pubicQueue.Count < 1 && saving)
                        {
                            Thread.Sleep(50);
                        }
                        if (!saving)
                        {
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
                            imageQueue.Enqueue(new MemoryStream(Convert.FromBase64String(privateQueue.Dequeue())));
                        }
                        if (srm == null)
                            srm = new VideoEncoder(filename, FPS);
                        Thread.Sleep(50);
                        while (imageQueue.Count > 0)
                        {
                            srm.Add(imageQueue.Dequeue());
                        }
                        Thread.Sleep(500);
                    }
                }
                finally
                {
                    srm.Close();
                }
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
                EncoderParameters pams = new EncoderParameters(1);
                pams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                value.Save(imageStream, VideoEncoder.GetEncoder(ImageFormat.Jpeg), pams);
                return Convert.ToBase64String(imageStream.ToArray());
            }
            new public Image ConvertFromString(string value)
            {
                byte[] imgs = Convert.FromBase64String(value);
                return Image.FromStream(new MemoryStream(imgs));
            }
        }

        /// <summary>
        /// Creates an easy to use class that encapsulates FFmpeg video encoding
        /// </summary>
        public class VideoEncoder : IDisposable
        {
            Stream vs;

            /// <summary>
            /// Creates a new fideo encoder
            /// </summary>
            /// <param name="filename">The file to save to. All FFmpeg file type supported (mp4, avi, wemb, mov, wmv, etc...)</param>
            /// <param name="fps">The framerate</param>
            public VideoEncoder(string filename, float fps)
            {
                vs = FFmpeg.GetEncoderStream(filename, fps);
            }

            /// <summary>
            /// Dispose of the encoding stream
            /// </summary>
            ~VideoEncoder()
            {
                Dispose();
            }

            /// <summary>
            /// Dispose of the encoding stream
            /// </summary>
            public void Dispose()
            {
                try
                {
                    vs.Close();
                }
                catch { }
            }

            /// <summary>
            /// Add another frame
            /// </summary>
            /// <param name="nextFrame">The next frame</param>
            public void Add(Bitmap nextFrame)
            {
                EncoderParameters pams = new EncoderParameters(1);
                pams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                nextFrame.Save(vs, GetEncoder(ImageFormat.Jpeg), pams);
            }

            /// <summary>
            /// Add another frame
            /// </summary>
            /// <param name="nextFrame">The next frame</param>
            public void Add(BitmapSource nextFrame)
            {
                var jbe = new JpegBitmapEncoder();
                jbe.QualityLevel = 100;
                jbe.Frames.Add(BitmapFrame.Create(nextFrame));
                jbe.Save(vs);
            }

            /// <summary>
            /// Add another frame
            /// </summary>
            /// <param name="nextFrame">The next frame</param>
            public void Add(MemoryStream nextFrame)
            {
                nextFrame.WriteTo(vs);
            }

            /// <summary>
            /// Save and close the file
            /// </summary>
            public void Close()
            {
                Dispose();
            }

            /// <summary>
            /// Get the Encoder for the specified ImageFormat
            /// </summary>
            /// <param name="format">ImageFormat that defines the Encoder</param>
            /// <returns>Encoder of the specified format</returns>
            public static ImageCodecInfo GetEncoder(ImageFormat format)
            {
                ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
                foreach (ImageCodecInfo codec in codecs)
                {
                    if (codec.FormatID == format.Guid)
                    {
                        return codec;
                    }
                }
                return null;
            }
        }
    }
}