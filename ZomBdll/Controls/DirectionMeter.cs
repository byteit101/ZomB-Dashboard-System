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
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System451.Communication.Dashboard.Utils;

internal class icofinds
{

}

namespace System451.Communication.Dashboard.Controls
{
    [ToolboxBitmap(typeof(icofinds), "System451.Communication.Dashboard.TBB.DashboardDirection.bmp")]
    [Designer(typeof(Design.DirectionMeterDesigner))]
    public partial class DirectionMeter : ZomBControl
    {
        RangeAndValue rv;

        public DirectionMeter()
        {
            rv = new RangeAndValue(0, 360, 0, true, true);
            rv.Invalidate += this.Invalidate;
            InitializeComponent();
            ControlName = "gyro1";
        }

        [DefaultValue("0"), Category("ZomB"), Description("The value of the Direction Meter")]
        public float Value
        {
            get
            {
                return rv.Value;
            }
            set
            {
                rv.Value = value;
            }
        }
        [DefaultValue("360"), Category("ZomB"), Description("The maximum value of the Direction Meter")]
        public float Max
        {
            get
            {
                return rv.Max;
            }
            set
            {
                rv.Max = value;
            }
        }
        [DefaultValue("0"), Category("ZomB"), Description("The minimum value of the Direction Meter")]
        public float Min
        {
            get
            {
                return rv.Min;
            }
            set
            {
                rv.Min = value;
            }
        }

        public override void UpdateControl(ZomBDataObject value)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Utils.ZomBDataFunction(UpdateControl), value);
            }
            else
            {
                this.Value = float.Parse(value);
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

        /// <summary>
        /// Converts a Point with center 50,50 to an angle
        /// </summary>
        /// <param name="p"></param>
        /// <returns>The angle off of the center 50,50</returns>
        public static double PointToAngle(PointF p)
        {
            double r = Math.Atan((p.X - 50.0) / (50.0 - p.Y)) / Math.PI * 90;
            if (p.Y > 50)
                return r + 90;
            return r;
        }


        private void DirectionMeter_Paint(object sender, PaintEventArgs e)
        {
            float value = rv.Scale(0, 360);

            //Set settings
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.Clear(this.BackColor);
            e.Graphics.ScaleTransform((float)this.Width / 100f, (float)this.Height / 100f);

            //Draw outer circle
            using (Pen roundPen = new Pen(CircleColor, CircleWidth))
            {
                e.Graphics.DrawEllipse(roundPen, 5, 5, 90, 90);
            }

            //Draw Guides
            using (Pen guidepen = new Pen(GuidesColor, GuidesWidth))
            {
                e.Graphics.DrawLine(guidepen, (6f - (CircleWidth / 2f)), 50f, (94f + (CircleWidth / 2f)), 50f);
                e.Graphics.DrawLine(guidepen, 50f, (6f - (CircleWidth / 2f)), 50f, (94f + (CircleWidth / 2f)));
            }

            //Draw arrow
            using (Pen ArrPen = new Pen(ArrowColor, ArrowWidth))
            {
                ArrPen.EndCap = LineCap.ArrowAnchor;
                ArrPen.StartCap = System.Drawing.Drawing2D.LineCap.RoundAnchor;
                PointF tp = new Point();

                //Its a circle, use trig
                double radians = value * Math.PI / 180;
                tp.Y = (float)(50f - (Math.Cos(radians) * 50f));
                tp.X = (float)(50f + (Math.Sin(radians) * 50f));
                e.Graphics.DrawLine(ArrPen, new PointF(50, 50), tp);
            }
        }

    }

    namespace Design
    {
        internal class DirectionMeterDesigner : ControlDesigner
        {
            DirectionMeter vm;
            bool dragin = false;
            bool inadorn = false;

            private const int WM_MouseMove = 0x0200;
            private const int WM_LButtonDown = 0x0201;
            private const int WM_LButtonUp = 0x0202;
            private const int WM_LButtonDblClick = 0x0203;
            private const int WM_RButtonDown = 0x0204;
            private const int WM_RButtonUp = 0x0205;
            private const int WM_RButtonDblClick = 0x0206;

            DesignerVerbCollection vbs;

            public DirectionMeterDesigner()
            {

            }
            public override void Initialize(IComponent component)
            {
                base.Initialize(component);
                vm = (DirectionMeter)component;

            }
            protected override void OnSetCursor()
            {
                if (!inadorn && !dragin)
                    base.OnSetCursor();
            }

            public Region GetValueRec()
            {
                Region r;
                GraphicsPath gp = new GraphicsPath();
                gp.AddEllipse((float)((.05 - vm.CircleWidth / 200f) * vm.Width), (float)((.05 - vm.CircleWidth / 200f) * vm.Height), (float)(.90 + vm.CircleWidth / 100f) * vm.Width, (float)(.90 + vm.CircleWidth / 100f) * vm.Height);
                r = new Region(gp);
                gp = new GraphicsPath();
                gp.AddEllipse((float)((.05 + vm.CircleWidth / 200f) * vm.Width), (float)((.05 + vm.CircleWidth / 200f) * vm.Height), (float)(.90 - vm.CircleWidth / 100f) * vm.Width, (float)(.90 - vm.CircleWidth / 100f) * vm.Height);
                r.Xor(gp);
                return r;
            }
            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case WM_LButtonDown:
                        if (GetValueRec().IsVisible(new Point(m.LParam.ToInt32())))
                        {
                            dragin = true;
                            Cursor.Current = Cursors.Hand;
                        }
                        break;
                    case WM_MouseMove:
                        if (dragin)
                        {
                            Point p = new Point(m.LParam.ToInt32());

                            vm.Value = (float)(DirectionMeter.PointToAngle(p) / 180 * (vm.Max - vm.Min) + vm.Min);
                            Cursor.Current = Cursors.Hand;
                            return;
                        }
                        else if (GetValueRec().IsVisible(new Point(m.LParam.ToInt32())))
                        {
                            inadorn = true;
                            Cursor.Current = Cursors.Hand;
                        }
                        else
                            inadorn = false;

                        break;
                    case WM_LButtonUp:
                        if (dragin)
                            dragin = false;
                        break;
                }
                base.WndProc(ref m);
            }
            public override DesignerVerbCollection Verbs
            {
                get
                {
                    if (vbs == null)
                    {
                        vbs = new DesignerVerbCollection();
                        vbs.Add(new DesignerVerb("Set range to normalized", new EventHandler(Reset1_1)));
                        vbs.Add(new DesignerVerb("Set range to degrees", new EventHandler(Resetdeg)));
                        vbs.Add(new DesignerVerb("Set range to radians", new EventHandler(Resetpi)));
                    }
                    return vbs;
                }
            }
            public void Reset1_1(object sender, EventArgs e)
            {
                vm.Min = -1f;
                vm.Max = 1f;
                vm.Value = 0f;
            }
            public void Resetdeg(object sender, EventArgs e)
            {
                vm.Min = 0;
                vm.Max = 360;
                vm.Value = 0;
            }
            public void Resetpi(object sender, EventArgs e)
            {
                vm.Min = 0;
                vm.Max = (float)(2 * Math.PI);
                vm.Value = 0;
            }
        }
    }
}