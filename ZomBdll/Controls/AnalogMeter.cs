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
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System451.Communication.Dashboard.Controls
{
    [ToolboxBitmap(typeof(icofinds), "System451.Communication.Dashboard.TBB.Analog.png")]
    [Designer(typeof(Design.AnalogMeterDesigner))]
    public partial class AnalogMeter : ZomBControl
    {
        float speedval = 0;
        bool use1to1023 = false;

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

        #endregion
        private Color circleColor = Color.PaleGreen;
        [DefaultValue(typeof(Color), "PaleGreen"), Category("ZomB"), Description("Color of the Circle")]
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
        /// <summary>
        /// Converts a Point with center 50,70 to an angle
        /// </summary>
        /// <param name="p"></param>
        /// <returns>The angle off of the center 50,70</returns>
        public static double PointToAngle(PointF p)
        {
            double r = Math.Atan((50.0 - p.X) / (p.Y - 70.0)) / Math.PI * 180;
            if (p.Y > 70)
                if (p.X > 50)
                    return 180f;
                else
                    return 0f;
            return r + 90f;
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
                double radians = (((Value - .5) * 2) * 90) * Math.PI / 180;
                tp.Y = (float)(70 - (Math.Cos(radians) * 50));
                tp.X = (float)(50 + (Math.Sin(radians) * 50));
                e.Graphics.DrawLine(ArrPen, new PointF(50, 70), tp);
            }
        }
    }

    namespace Design
    {
        internal class AnalogMeterDesigner : ControlDesigner
        {
            AnalogMeter vm;
            bool dragin = false;
            bool inadorn = false;
            long ltime = DateTime.Now.Ticks;
            int lpoint;
            Rectangle europeanswallow;
            bool cans = false;

            private const int WM_MouseMove = 0x0200;
            private const int WM_LButtonDown = 0x0201;
            private const int WM_LButtonUp = 0x0202;
            private const int WM_LButtonDblClick = 0x0203;
            private const int WM_RButtonDown = 0x0204;
            private const int WM_RButtonUp = 0x0205;
            private const int WM_RButtonDblClick = 0x0206;

            public AnalogMeterDesigner()
            {

            }

            public override void Initialize(IComponent component)
            {
                base.Initialize(component);
                vm = (AnalogMeter)component;
            }
            protected override void OnSetCursor()
            {
                if ((!inadorn && !dragin) || (cans && inadorn && !dragin))
                    base.OnSetCursor();
            }

            public GraphicsPath GetValueRec()
            {
                GraphicsPath gp = new GraphicsPath();
                gp.AddArc((float)((.05f) * vm.Width), (float)((1f / 3f) * vm.Height), (float)(.90f) * vm.Width, (float)(1.2f) * vm.Height, -180f, 180f);
                gp.AddLine((float)((.05f) * vm.Width), (float)((70f / 75f) * vm.Height), (float)(.95f) * vm.Width, (float)((70f / 75f) * vm.Height));
                return gp;
            }

            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case WM_LButtonDown:
                        if (GetValueRec().IsVisible(new Point(m.LParam.ToInt32())))
                        {
                            dragin = true;
                            lpoint = m.LParam.ToInt32();
                            Point tp = new Point(lpoint);
                            tp.Offset(-3, -3);
                            europeanswallow = new Rectangle(tp, new Size(6, 6));
                            ltime = DateTime.Now.Ticks;
                            Cursor.Current = Cursors.Hand;
                            cans = true;
                        }
                        break;
                    case WM_MouseMove:
                        if (dragin)
                        {

                            Point p = new Point(m.LParam.ToInt32());
                            if (cans)
                            {
                                if (europeanswallow.Contains(p))
                                {
                                    double f = DateTime.Now.Subtract(new DateTime(ltime)).TotalSeconds;
                                    if (f >= 1.5)
                                        cans = false;
                                    else if (f >= 0.5)
                                    {
                                        dragin = false;
                                        Cursor.Current = Cursors.SizeAll;
                                        break;
                                    }
                                }
                                else
                                    cans = false;
                            }

                            float v = (float)(((AnalogMeter.PointToAngle(p)) / 180f) * (1023));
                            vm.Value = Math.Max(0, Math.Min(1023, v));
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
        }
    }
}
