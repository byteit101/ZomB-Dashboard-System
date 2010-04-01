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
