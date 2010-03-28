using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using WebCam_Capture;
using System.Drawing;

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

        #region IDashboardControl Members

        public string[] ParamName
        {
            get
            {
                return null;
            }
            set
            {
                
            }
        }

        public string Value
        {
            get;
            set;
        }

        public string DefalutValue
        {
            get { return ""; }
        }

        public void Update()
        {
            
        }

        #endregion
    }
}
