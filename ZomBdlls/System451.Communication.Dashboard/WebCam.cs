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
}
