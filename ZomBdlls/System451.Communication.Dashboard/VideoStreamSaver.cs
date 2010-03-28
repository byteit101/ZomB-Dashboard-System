/*
 * Copyright (c) 2009-2010, Patrick Plenefisch and FIRST Robotics Team 451 "The Cat Attack"
 * 
 * Permission to use, copy, modify, and distribute this software, its source, and its documentation
 * for any purpose, without fee, and without a written agreement is hereby granted, 
 * provided this paragraph and the following paragraph appear in all copies, and all
 * software that uses this code is released under this license. All projects that use
 * this code MUST release their source without fee.
 * 
 * THIS SOFTWARE IS PROVIDED "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
 * AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL
 * Patrick Plenefisch OR FIRST Robotics Team 451 "The Cat Attack" BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using AviFile;

namespace System451.Communication.Dashboard
{
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
            AviStreamer srm = null;
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
                    //imageQueue.Enqueue((Bitmap)source.GetTypeConverter().ConvertFrom(null, System.Globalization.CultureInfo.CurrentCulture, privateQueue.Dequeue(), typeof(Image)));
                    imageQueue.Enqueue((Bitmap)((new BitmapConverter()).ConvertFromString(privateQueue.Dequeue())));
                }
                if (srm == null)
                    srm = new AviStreamer(filename, FPS, imageQueue.Dequeue());
                Thread.Sleep(50);
                while (imageQueue.Count>0)
                {
                    srm.Add(imageQueue.Dequeue());
                }
                Thread.Sleep(500);
            }
            srm.Close();
        }

        #endregion
    }
    public class BitmapConverter:TypeConverter
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
    public class AviStreamer : IDisposable
    {
        AviManager mgr;
        VideoStream vs;
        public AviStreamer(string filename, double fps, Bitmap first)
        {
            mgr = new AviManager(filename, false);
            vs = mgr.AddVideoStream(false, fps, first);
        }
        ~AviStreamer()
        {
            Dispose();
        }

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                //vs.Close();
                mgr.Close();
            }
            catch
            {

            }
        }
        public void Add(Bitmap nextFrame)
        {
            vs.AddFrame(nextFrame);
        }

        #endregion

        public void Close()
        {
            Dispose();
        }
    }
}
