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
using System.Drawing.Drawing2D;

namespace System451.Communication.Dashboard
{
    [ToolboxBitmap(typeof(icofinds), "System451.Communication.Dashboard.TBB.Analog.png")]
    public partial class AnalogMeter : ZomBControl 
    {
        float speedval = 0;
        bool use1to1023 = false;
        delegate void UpdaterDelegate(string value);
        public AnalogMeter()
        {
            InitializeComponent();
            speedval = 0;
            this.ControlName = "analog1";
        }
        [DefaultValue("0"), Category("ZomB"), Description("The Value of the Analog Meter")]
        public float Value
        {
            get
            {
                return speedval;
            }
            set
            {
                speedval = (use1to1023) ? value / 1023 : value;
                this.Invalidate();
            }
        }

        [DefaultValue(false), Category("ZomB"), Description("Does the dashboard send a value of 0-1023, or 0-1")]
        public bool Use0To1023
        {
            get
            {
                return use1to1023;
            }
            set
            {
                use1to1023 = value;
            }
        }


        #region IDashboardControl Members
        
        [Obsolete("Use Control Name"), Category("ZomB"), Description("[OBSOLETE] What this control will get the value of from the packet Data")]
        public string BindToInput
        {
            get { return ControlName; }
            set { ControlName = value; }
        }
        public override void UpdateControl(string value)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdaterDelegate(UpdateControl));
            }
            else
            {
                this.Value = float.Parse(value);
                this.Invalidate();
            }
        }

        #endregion 
        private Color circleColor = Color.PaleGreen;
        [DefaultValue(typeof(Color),"PaleGreen"), Category("ZomB"), Description("Color of the Circle")]
        public Color CircleColor
        {
            get
            {
                return circleColor;
            }

            set
            {
                if (circleColor != value)
                    circleColor = value;
                this.Invalidate();
            }

        }

        private Color arrowColor = Color.SlateGray;
        [DefaultValue(typeof(Color), "SlateGray"), Category("ZomB"), Description("Color of the Arrow")]
        public Color ArrowColor
        {
            get
            {
                return arrowColor;
            }

            set
            {
                if (arrowColor != value)
                    arrowColor = value;
                this.Invalidate();
            }

        }
        private float arrowwidth = 4;
        [DefaultValue("4"), Category("ZomB"), Description("Width of the Arrow")]
        public float ArrowWidth
        {
            get
            {
                return arrowwidth;
            }

            set
            {
                if (arrowwidth != value)
                    arrowwidth = value;
                this.Invalidate();
            }

        }
        private Color guidesColor = Color.Black;
        [DefaultValue(typeof(Color), "Black"), Category("ZomB"), Description("Color of the Guides")]
        public Color GuidesColor
        {
            get
            {
                return guidesColor;
            }

            set
            {
                if (guidesColor != value)
                    guidesColor = value;
                this.Invalidate();
            }

        }
        private float guideswidth = 1;
        [DefaultValue("1"), Category("ZomB"), Description("Width of the Guides")]
        public float GuidesWidth
        {
            get
            {
                return guideswidth;
            }

            set
            {
                if (guideswidth != value)
                    guideswidth = value;
                this.Invalidate();
            }

        }

        private void AnalogMeter_Paint(object sender, PaintEventArgs e)
        {
            if (Math.Abs(Value) > 1)
            {
                if (Value == -999.99)
                {
                }
                else
                {
                    Value = (Value < 0) ? 0 : 1;
                }
            }
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.ScaleTransform((float)this.Width / 100f, (float)this.Height / 75f);
            using (Brush cp = new SolidBrush(CircleColor))
            {
                e.Graphics.FillPie(cp, new Rectangle(5, 25, 90, 90), -180f, 180f);
            }
            using (Pen mp = new Pen(GuidesColor, GuidesWidth))
            {
                e.Graphics.DrawArc(mp, new Rectangle(5, 25, 90, 90), -180f, 180f);
                e.Graphics.DrawLine(mp, new Point(5, 70), new Point(95, 70));
                Matrix RotationTransform = new Matrix(1, 0, 0, 1, 0, 0);
                Matrix TranslationTransform = new Matrix(1, 0, 0, 1, 0, 0);
                PointF TheRotationPoint = new PointF(50f, 70f);
                RotationTransform.Scale((float)this.Width / 100f, (float)this.Height / 75f);
                RotationTransform.RotateAt(-90, TheRotationPoint);
                e.Graphics.Transform = RotationTransform;
                e.Graphics.DrawLine(mp, 50, 30, 50, 40);
                for (float f = 0F; f < 180; f += 30)
                {
                    RotationTransform.RotateAt(30, TheRotationPoint);

                    e.Graphics.Transform = RotationTransform;
                    e.Graphics.DrawLine(mp, 50, 30, 50, 40);
                }
            }
            e.Graphics.ResetTransform();
            e.Graphics.ScaleTransform((float)this.Width / 100f, (float)this.Height / 75f);
            using (Pen ArrPen = new Pen(ArrowColor, ArrowWidth))
            {
                ArrPen.EndCap = LineCap.ArrowAnchor;
                ArrPen.StartCap = System.Drawing.Drawing2D.LineCap.RoundAnchor;
                PointF tp = new Point();
                double radians = (((Value-.5)*2)*90) * Math.PI / 180;
                tp.Y = (float)(70 - (Math.Cos(radians) * 50));
                tp.X = (float)(50 + (Math.Sin(radians) * 50));
                // }
                e.Graphics.DrawLine(ArrPen, new PointF(50, 70), tp);
            }
        }
    }
}
