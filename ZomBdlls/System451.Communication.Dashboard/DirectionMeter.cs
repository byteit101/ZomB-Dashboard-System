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
internal class icofinds
{

}
namespace System451.Communication.Dashboard
{
    [ToolboxBitmap(typeof(icofinds), "System451.Communication.Dashboard.TBB.DashboardDirection.bmp")]
    //[ToolboxBitmap(typeof(Button))]
    public partial class DirectionMeter : ZomBControl
    {
        float speedval = 0;
        delegate void UpdaterDelegate(string value);
        
        
        public DirectionMeter()
        {
            InitializeComponent();
            ControlName = "gyro1";
        }
        [DefaultValue("0"), Category("ZomB"), Description("The Value of the Direction Meter")]
        public float Value
        {
            get
            {
                return speedval;
            }
            set
            {
                speedval = value;
                this.Invalidate();
            }
        }


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

        private Color circleColor = Color.IndianRed;
        [DefaultValue(typeof(Color), "IndianRed"), Category("ZomB"), Description("Color of the Circle")]
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
        private float circlewidth = 5;
        [DefaultValue("5"), Category("ZomB"), Description("Width of the Circle")]
        public float CircleWidth
        {
            get
            {
                return circlewidth;
            }

            set
            {
                if (circlewidth != value)
                    circlewidth = value;
                this.Invalidate();
            }

        }
        private Color arrowColor = Color.Navy;
        [DefaultValue(typeof(Color), "Navy"), Category("ZomB"), Description("Color of the Arrow")]
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
        private Color guidesColor = Color.DeepSkyBlue;
        [DefaultValue(typeof(Color), "DeepSkyBlue"), Category("ZomB"), Description("Color of the Guides")]
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


        private void DirectionMeter_Paint(object sender, PaintEventArgs e)
        {
            if (Value > 360 || Value < 0)
            {
                if (Value == -999.99)
                {
                }
                else
                {
                    while (Value < 0)
                        Value += 360;
                    if (Value > 360)
                        Value = Value % 360;
                    //Value = (Value < 0) ? 0 : 0;
                }
            }
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.Clear(this.BackColor);
            e.Graphics.ScaleTransform((float)this.Width / 100f, (float)this.Height / 100f);
            using (Pen roundPen = new Pen(CircleColor, CircleWidth))
            {
                e.Graphics.DrawEllipse(roundPen, 5, 5, 90, 90);
            }
            using (Pen guidepen = new Pen(GuidesColor, GuidesWidth))
            {
                e.Graphics.DrawLine(guidepen, 3, 50, 107, 50);
                e.Graphics.DrawLine(guidepen, 50, 3, 50, 107);
            }
            using (Pen ArrPen = new Pen(ArrowColor, ArrowWidth))
            {
                ArrPen.EndCap = LineCap.ArrowAnchor;
                ArrPen.StartCap = System.Drawing.Drawing2D.LineCap.RoundAnchor;
                PointF tp = new Point();
                double radians = Value * Math.PI / 180;
                tp.Y = (float)(50 - (Math.Cos(radians) * 50));
                tp.X = (float)(50 + (Math.Sin(radians) * 50));
                // }
                e.Graphics.DrawLine(ArrPen, new PointF(50, 50), tp);
            }
        }

    }
}