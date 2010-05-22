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
using System.ComponentModel;
using System.Drawing;
using WebCam_Capture;
using System451.Communication.Dashboard.Net;

namespace System451.Communication.Dashboard
{
    public class WebCam : ISavableZomBData
    {
        WebCamCapture wcc;
        Image latest;
        public WebCam()
        {
            wcc = new WebCamCapture();
            wcc.TimeToCapture_milliseconds = 30;
            wcc.CaptureHeight = 240;
            wcc.CaptureWidth = 320;
            wcc.ImageCaptured += new WebCamCapture.WebCamEventHandler(wcc_ImageCaptured);
            wcc.Start(0);
        }

        ~WebCam()
        {
            wcc.Stop();
        }

        void wcc_ImageCaptured(object source, WebcamEventArgs e)
        {
            latest = e.WebCamImage;
            if (dataUpdatedEvent != null)
                dataUpdatedEvent(this, e);
        }

        #region ISavableZomBData Members

        TypeConverter ISavableZomBData.GetTypeConverter()
        {
            return new BitmapConverter();
        }

        string ISavableZomBData.DataValue
        {
            get { return (new BitmapConverter()).ConvertToString(latest); }
        }
        private EventHandler dataUpdatedEvent;

        event EventHandler ISavableZomBData.DataUpdated
        {
            add { dataUpdatedEvent += value; }
            remove { dataUpdatedEvent -= value; }
        }

        #endregion


    }
    public class WebCamVideoSource : IDashboardVideoDataSource
    {
        WebCamCapture wcc;
        Image latest;

        public WebCamVideoSource()
        {
            wcc = new WebCamCapture();
            wcc.TimeToCapture_milliseconds = 20;//50hz
            wcc.CaptureHeight = 240;
            wcc.CaptureWidth = 320;
            wcc.ImageCaptured += new WebCamCapture.WebCamEventHandler(wcc_ImageCaptured);
        }
        public WebCamVideoSource(float fps, int width, int height)
        {
            wcc = new WebCamCapture();
            wcc.TimeToCapture_milliseconds = (int)((1f / fps) * 1000);//50hz
            wcc.CaptureHeight = height;
            wcc.CaptureWidth = width;
            wcc.ImageCaptured += new WebCamCapture.WebCamEventHandler(wcc_ImageCaptured);
        }


        ~WebCamVideoSource()
        {
            wcc.Stop();
        }

        void wcc_ImageCaptured(object source, WebcamEventArgs e)
        {
            latest = e.WebCamImage;
            if (NewImageRecieved != null)
                NewImageRecieved(this, new NewImageDataRecievedEventArgs(e.WebCamImage));
        }
        #region IDashboardVideoDataSource Members

        public Image GetImage()
        {
            return latest;
        }

        public int Height
        {
            get { return wcc.CaptureHeight; }
            set { wcc.CaptureHeight = value; }
        }

        public int Width
        {
            get { return wcc.CaptureWidth; }
            set { wcc.CaptureWidth = value; }
        }

        public float FPS
        {
            get { return 1f / wcc.TimeToCapture_milliseconds * 1000; }
            set
            {
                wcc.TimeToCapture_milliseconds = (int)(1f / value * 1000);
            }
        }

        public void Start()
        {
            if (wcc.TimeToCapture_milliseconds <= 0)
                FPS = 50;
            wcc.Start(0);
            Running = true;
        }

        public void Stop()
        {
            wcc.Stop();
            Running = false;
        }

        public bool Running
        {
            get;
            protected set;
        }

        public event NewImageDataRecievedEventHandler NewImageRecieved;

        #endregion
    }
}
