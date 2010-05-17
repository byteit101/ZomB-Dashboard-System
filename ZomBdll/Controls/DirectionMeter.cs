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

        public override void UpdateControl(string value)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdaterDelegate(UpdateControl),value);
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