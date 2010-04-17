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
using System.Data;
using System.Text;
using System.Windows.Forms;
using SmashTcpDashboard;
using System.IO;
using System.Collections.ObjectModel;

namespace System451.Communication.Dashboard
{
    public partial class CameraView : UserControl, ISavableZomBData, IDashboardControl
    {
        Collection<string> paramName = new Collection<string>();
        Image view;
        Receiver Receiver;
        delegate void UpdaterDelegate();

        public CameraView()
        {
            view = new Bitmap(10, 10);
            EnableAutoReset = false;
            InitializeComponent();
            paramName.Add("target1");
            if (!this.DesignMode)
            {
                Receiver = null;
                //Receiver.OnImageUpdate += new Receiver.ImageUpdate(Receiver_OnImageUpdate);
                //Start();// we don't know the IP
                timer1.Enabled = false;
            }
            else
            {
                timer1.Enabled = false;
            }
        }
        private string ip = "10.0.0.2";
        [Category("ZomB"), Description("Target IP (10.xx.xx.2, where xxxx is your team number)")]
        public string IPAddress
        {
            get { return ip; }
            set
            {
                ip = value;
                if (!DesignMode)
                {
                    
                    if (Receiver != null)
                    {
                        Receiver.Stop();
                    }
                    Receiver = new TcpReceiver(ip);
                    Receiver.OnImageUpdate += new Receiver.ImageUpdate(Receiver_OnImageUpdate);
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


        public void Start()
        {
            if (!this.DesignMode && Receiver != null && this.CanFocus)
            {
                Receiver.Start();
                timer1.Enabled = EnableAutoReset;
            }
        }
        public void Stop()
        {
            Receiver.Stop();
        }

        void Receiver_OnImageUpdate()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdaterDelegate(Receiver_OnImageUpdate));
            }
            else
            {
                
                view = Bitmap.FromStream(new MemoryStream(Receiver.ImageData));
                if (this.dataUpdatedEvent != null)
                    dataUpdatedEvent(this, new EventArgs());
                this.Invalidate();

                //ZomBSaver takes care of this now
                //File.WriteAllBytes(@"C:\Program Files\FRC Dashboard\img\ZomEye"+(calls++)+".jpg", Receiver.ImageData);
            }
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
                        foreach (TargetInfo target in Targets)
                        {
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
        private Collection<TargetInfo> balls = new Collection<TargetInfo>();
        [Category("ZomB"), Description("Target Location")]
        public Collection<TargetInfo> Targets
        {
            get { return balls; }
            //set { balls = value; }
        }
        public Collection<IDashboardControl> GetTargets()
        {
            Collection<IDashboardControl> retu = new Collection<IDashboardControl>();
            foreach (TargetInfo targ in Targets)
            {
                retu.Add(targ);
            }
            return retu;
        }

        #region IDashboardControl Members

        string[] IDashboardControl.ParamName
        {
            get
            {
                string[] tmp = new string[BindToInput.Count];
                BindToInput.CopyTo(tmp, 0);
                return tmp;
            }
            set
            {
                //BindToInput = value;
            }
        }

        string IDashboardControl.Value
        {
            get
            {
                return Targets.ToString();
            }
            set
            {
                //Targets = RectParse(value);
            }
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

        string IDashboardControl.DefalutValue
        {
            get { return ""; }
        }

        void IDashboardControl.Update()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdaterDelegate(Update));
            }
            else
            {
                this.Invalidate();
            }
        }

        #endregion

        [DefaultValue("target1"), Category("ZomB"), Description("What this control will get the value of from the packet Data")]
        public Collection<string> BindToInput
        {
            get { return paramName; }
            //set { paramName = value; }
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

        // ticks every 1750ms
        // checks if the video has timeed out
        // also (re)starts the video stream
        private void timer1_Tick(object sender, EventArgs e)
        {

            try
            {
                if (Receiver != null)
                {
                    if (!Receiver.Running)
                    {
                        Start();
                    }
                }
                else
                {
                }
            }
            catch
            {
            }

        }


        #region ISavableZomBData Members

        TypeConverter ISavableZomBData.GetTypeConverter()
        {
             return new BitmapConverter();
        }

        string ISavableZomBData.DataValue
        {
            get { return (new BitmapConverter()).ConvertToString(this.view); }
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
    }
    public class TargetInfo : IDashboardControl
    {

        public TargetInfo()
        {

        }
        public TargetInfo(string parm)
        {
            ParamName = new string[] { parm };
        }

        private RectangleF itarget;
        public RectangleF Target
        {
            get { return itarget; }
            set { itarget = value; }
        }


        #region IDashboardControl Members

        public string[] ParamName
        {
            get;
            set;
        }

        public string Value
        {
            get
            {
                return Target.ToString();
            }
            set
            {
                Target = RectParse(value);
            }
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
