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
using System.Windows.Forms;
using System451.Communication.Dashboard.Net;
using System451.Communication.Dashboard.Net.Video;

namespace System451.Communication.Dashboard
{
    namespace Controls
    {
        public partial class CameraView : Control, ISavableZomBData, IZomBControlGroup
        {
            ZomBControlCollection targs = new ZomBControlCollection();
            Image view;
            private delegate void UpdaterDelegate(object sender, NewImageDataRecievedEventArgs e);

            IDashboardVideoDataSource videoSource;


            public CameraView()
            {
                view = new Bitmap(10, 10);
                EnableAutoReset = false;
                InitializeComponent();
            }
            private int team = 0;
            [Category("ZomB"), Description("Your team number")]
            public int TeamNumber
            {
                get { return team; }
                set
                {
                    team = value;
                    if (!DesignMode)
                    {

                        if (videoSource != null)
                        {
                            videoSource.Stop();
                        }
                        //TODO: support other sources
                        videoSource = GetDefaultSrc();
                        videoSource.NewImageRecieved += new NewImageDataRecievedEventHandler(videoSource_NewImageRecieved);
                        if (EnableAutoReset)
                            Start();
                    }
                }
            }


            [Category("ZomB"), Description("Auto Reset the Camera"), DefaultValue(false)]
            public bool EnableAutoReset { get; set; }
            public void Restart()
            {
                Stop();
                Start();
            }

            private DefaultVideoSource defsrc;
            [Category("ZomB"), Description("The default camera source"), DefaultValue(typeof(DefaultVideoSource), "WPILibTcpStream")]
            public DefaultVideoSource DefaultVideoSource
            {
                get { return defsrc; }
                set
                {
                    defsrc = value;
                    if (defsrc != DefaultVideoSource.Manual)
                        LoadDefaultSrc();
                }
            }

            private void LoadDefaultSrc()
            {
                VideoSource = GetDefaultSrc();
            }

            private IDashboardVideoDataSource GetDefaultSrc()
            {
                switch (DefaultVideoSource)
                {
                    case DefaultVideoSource.WPILibTcpStream:
                        if (TeamNumber != 0)
                            return new WPILibTcpVideoSource(TeamNumber);
                        else
                            return null;
                    case DefaultVideoSource.Webcam:
                        return new WebCamVideoSource(50, this.Width, this.Height);
                    case DefaultVideoSource.Manual:
                    default:
                        return null;
                }
            }


            [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public IDashboardVideoDataSource VideoSource
            {
                get
                {
                    return videoSource;
                }
                set
                {
                    if (videoSource != null)
                    {
                        videoSource.Stop();
                    }
                    videoSource = value;
                    if (value != null)
                        videoSource.NewImageRecieved += new NewImageDataRecievedEventHandler(videoSource_NewImageRecieved);
                }
            }

            void videoSource_NewImageRecieved(object sender, NewImageDataRecievedEventArgs e)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new UpdaterDelegate(videoSource_NewImageRecieved), sender, e);
                }
                else
                {

                    this.view = e.NewData;
                    if (this.dataUpdatedEvent != null)
                        dataUpdatedEvent(this, new EventArgs());
                    this.Invalidate();

                    //ZomBSaver takes care of this now
                    //File.WriteAllBytes(@"C:\Program Files\FRC Dashboard\img\ZomEye"+(calls++)+".jpg", Receiver.ImageData);
                }
            }


            public void Start()
            {
                if (VideoSource == null)
                    LoadDefaultSrc();
                if (!this.DesignMode && VideoSource != null && this.CanFocus)
                {
                    videoSource.Start();
                    timer1.Enabled = EnableAutoReset;
                }
            }
            public void Stop()
            {
                videoSource.Stop();
            }



            protected override Size DefaultSize
            {
                get
                {
                    return new Size(640, 480);
                }
            }
            private void UserControl2_Paint(object sender, PaintEventArgs e)
            {
                e.Graphics.DrawImage(view, 0, 0, this.Width, this.Height);
                if (this.Targets.Count > 0)
                {
                    e.Graphics.ScaleTransform((this.Width / view.Width), (this.Height / view.Height));
                    using (Pen pn = new Pen(TargetColor, TargetWidth))
                    {
                        pn.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;

                        using (Brush br = new SolidBrush(TargetFillColor))
                        {
                            foreach (IZomBControl ttarget in Targets)
                            {
                                TargetInfo target = (TargetInfo)(ttarget);
                                if (!target.Target.IsEmpty)
                                {
                                    PointF[] pts = new PointF[] { new PointF(target.Target.Top, target.Target.Left),
                                new PointF(target.Target.Top, target.Target.Right),
                                new PointF(target.Target.Bottom, target.Target.Right),
                                new PointF(target.Target.Bottom, target.Target.Left)};

                                    e.Graphics.FillPolygon(br, pts);

                                    e.Graphics.DrawPolygon(pn, pts);
                                }
                            }
                        }
                    }
                }
            }
            private Color boxColor = Color.Green;
            [DefaultValue(typeof(Color), "Green"), Category("ZomB"), Description("Target bounding box color")]
            public Color TargetColor
            {
                get { return boxColor; }
                set { boxColor = value; }
            }

            private float width = 2.5f;
            [DefaultValue(2.5f), Category("ZomB"), Description("Target bounding box width")]
            public float TargetWidth
            {
                get { return width; }
                set { width = value; }
            }

            private Color fillColor = Color.FromArgb(100, Color.Green);
            [DefaultValue(typeof(Color), "Color [A=100, R=0, G=127, B=0]"), Category("ZomB"), Description("Target bounding box color")]
            public Color TargetFillColor
            {
                get { return fillColor; }
                set { fillColor = value; }
            }

            [DefaultValue(true), Category("ZomB"), Description("Show the Auto-Reset check box?")]
            public bool ShowAutoReset
            {
                get { return checkBox1.Visible; }
                set { checkBox1.Visible = value; }
            }

            [Category("ZomB"), Description("The Targets"), Browsable(false)]
            public ZomBControlCollection Targets
            {
                get { return targs; }
            }

            // ticks every 1750ms
            // checks if the video has timeed out
            // also (re)starts the video stream
            private void timer1_Tick(object sender, EventArgs e)
            {

                try
                {
                    if (videoSource != null && !videoSource.Running)
                    {
                        videoSource.Start();
                    }
                }
                catch
                {
                }

            }


            #region ISavableZomBData Members

            TypeConverter ISavableZomBData.GetTypeConverter()
            {
                return new Net.Video.BitmapConverter();
            }

            string ISavableZomBData.DataValue
            {
                get { return (new Net.Video.BitmapConverter()).ConvertToString(this.view); }
            }
            private EventHandler dataUpdatedEvent;

            event EventHandler ISavableZomBData.DataUpdated
            {
                add { dataUpdatedEvent += value; }
                remove { dataUpdatedEvent -= value; }
            }
            #endregion

            private void checkBox1_CheckedChanged(object sender, EventArgs e)
            {
                timer1.Enabled = EnableAutoReset = checkBox1.Checked;
            }

            #region IZomBControlGroup Members

            ZomBControlCollection IZomBControlGroup.GetControls()
            {
                return this.targs;
            }

            #endregion
        }
    }

    public enum DefaultVideoSource
    {
        /// <summary>
        /// The Axis Camera from WPILib
        /// </summary>
        WPILibTcpStream,
        /// <summary>
        /// The Webcam on this computer
        /// </summary>
        Webcam,
        /// <summary>
        /// Manually configure the video source
        /// </summary>
        Manual
    }

    public class TargetInfo : IZomBControl
    {

        public TargetInfo()
        {

        }
        public TargetInfo(string parm)
        {
            ControlName = parm;
        }

        private RectangleF itarget;
        public RectangleF Target
        {
            get { return itarget; }
            set { itarget = value; }
        }




        private RectangleF RectParse(string value)
        {
            RectangleF rf = new RectangleF();
            if (value != null && value != "")
            {
                //format widthxheigth+xpos,ypos
                rf.Width = float.Parse(value.Substring(0, value.IndexOf('x')));
                rf.Height = float.Parse(value.Substring(value.IndexOf('x') + 1, (value.IndexOf('+') - (value.IndexOf('x') + 1))));
                rf.X = float.Parse(value.Substring(value.IndexOf('+') + 1, (value.IndexOf(',') - (value.IndexOf('+') + 1))));
                rf.Y = float.Parse(value.Substring(value.IndexOf(',') + 1));
            }
            return rf;
        }



        #region IZomBControl Members
        public bool IsMultiWatch
        {
            get { return false; }
        }

        public string ControlName
        {
            get;
            set;
        }

        public void UpdateControl(string value)
        {
            Target = RectParse(value);

        }

        public void ControlAdded(object sender, ZomBControlAddedEventArgs e)
        {

        }

        #endregion
    }
}
