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
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System451.Communication.Dashboard.Controls
{
    [Designer(typeof(Design.ValueMeterDesigner))]
    public class ValueMeter : ZomBControl
    {
        float speedval, aval;

        public ValueMeter()
        {
            Max = 100;
            this.DoubleBuffered = true;
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(125, 200);
            }
        }

        [DefaultValue(0f), Category("ZomB"), Description("The value of the meter")]
        public float Value
        {
            get
            {
                return aval;// (float)Math.Round((speedval / 100.0) * (Max - Min) + Min, 5);
            }
            set
            {
                speedval = ((value - Min) / (Max - Min)) * 100f;
                aval = value;
                this.Invalidate();
            }
        }

        [DefaultValue(100f), Category("ZomB"), Description("The Max value of the meter")]
        public float Max
        {
            get
            {
                return mx;
            }
            set
            {
                mx = value;
                highthresh = ((ahighthresh - Min) / (Max - Min)) * 100f;
                lowthresh = ((alowthresh - Min) / (Max - Min)) * 100f;
                speedval = ((aval - Min) / (Max - Min)) * 100f;
                this.Invalidate();
            }
        }
        [DefaultValue(0f), Category("ZomB"), Description("The Min value of the meter")]
        public float Min
        {
            get
            {
                return mn;
            }
            set
            {
                mn = value;
                highthresh = ((ahighthresh - Min) / (Max - Min)) * 100f;
                lowthresh = ((alowthresh - Min) / (Max - Min)) * 100f;
                speedval = ((aval - Min) / (Max - Min)) * 100f;
                this.Invalidate();
            }
        }
        float mx, mn;
        [DefaultValue(6f), Category("ZomB"), Description("The Width of the Bar")]
        public float BarWidth
        {
            get
            {
                return bwidth;
            }
            set
            {
                bwidth = value;
                Invalidate();
            }
        }
        private float bwidth = 6f;

        [DefaultValue(typeof(Orientation), "Vertical"), Category("ZomB"), Description("The orientation of the meter")]
        public Orientation Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                orientation = value;
                Invalidate();
            }
        }
        private Orientation orientation = Orientation.Vertical;


        [DefaultValue(typeof(BorderStyle), "FixedSingle"), Category("ZomB"), Description("The orientation of the meter")]
        public BorderStyle BorderStyle
        {
            get
            {
                return borderstyle;
            }
            set
            {
                borderstyle = value;
                Invalidate();
            }
        }
        private BorderStyle borderstyle = BorderStyle.FixedSingle;

        [DefaultValue(typeof(Color), "Black"), Category("ZomB"), Description("The color of the bar")]
        public Color BorderColor
        {
            get
            {
                return bordercolor;
            }
            set
            {
                bordercolor = value;
                Invalidate();
            }
        }
        private Color bordercolor = Color.Black;


        [DefaultValue(typeof(Color), "RoyalBlue"), Category("ZomB"), Description("The color of the bar")]
        public Color BarColor
        {
            get
            {
                return barcolor;
            }
            set
            {
                barcolor = value;
                Invalidate();
            }
        }
        private Color barcolor = Color.RoyalBlue;

        [DefaultValue(typeof(Color), "Red"), Category("Appearance"), Description("The color of the meter when high is hit")]
        public Color HighColor
        {
            get
            {
                return highcolor;
            }
            set
            {
                highcolor = value;
                Invalidate();
            }
        }
        private Color highcolor = Color.Red;

        [DefaultValue(10f), Category("ZomB"), Description("The value that triggers a low value")]
        public float LowThreshold
        {
            get
            {
                return alowthresh;// (float)Math.Round((lowthresh / 100.0) * (Max - Min) + Min, 5);
            }
            set
            {
                lowthresh = ((value - Min) / (Max - Min)) * 100f;
                alowthresh = value;
                Invalidate();
            }
        }
        float alowthresh = 10f, lowthresh = 10f;
        [DefaultValue(90f), Category("ZomB"), Description("The value that triggers a high value")]
        public float HighThreshold
        {
            get
            {
                return ahighthresh;// (float)Math.Round((highthresh / 100.0) * (Max - Min) + Min, 5);
            }
            set
            {
                highthresh = ((value - Min) / (Max - Min)) * 100f;
                ahighthresh = value;
                Invalidate();
            }
        }
        float highthresh = 90f, ahighthresh = 90f;

        [DefaultValue(typeof(Color), "Green"), Category("Appearance"), Description("The color of the meter when low is hit")]
        public Color LowColor
        {
            get
            {
                return lowcolor;
            }
            set
            {
                lowcolor = value;
                Invalidate();
            }
        }
        private Color lowcolor = Color.Green;

        [DefaultValue(false), Category("Appearance"), Description("Show the value of the meter?")]
        public bool Label
        {
            get
            {
                return label;
            }
            set
            {
                label = value;
                Invalidate();
            }
        }
        private bool label = false;
        [DefaultValue(true), Category("ZomB"), Description("Show the decimal place?")]
        public bool Decimal
        {
            get
            {
                return deci;
            }
            set
            {
                deci = value;
                Invalidate();
            }
        }
        private bool deci = true;

        [DefaultValue(false), Category("ZomB"), Description("Create a bar instead of a traditional progress bar")]
        public bool UseBar
        {
            get { return useBar; }
            set { useBar = value; Invalidate(); }
        }
        private bool useBar = false;

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            float height = this.Height;
            float width = this.Width;
            if (Orientation == Orientation.Vertical)
            {
                e.Graphics.TranslateTransform(.5f, (float)this.Height - .5f);
                e.Graphics.RotateTransform(-90);
                width = this.Height;
                height = this.Width;
            }

            if (speedval <= lowthresh)
                e.Graphics.Clear(this.LowColor);
            else
                e.Graphics.Clear(this.BackColor);

            Color c = this.ForeColor;
            if (speedval >= highthresh)
                c = this.HighColor;

            using (Brush b = new SolidBrush(c))
            {
                //Note: value is already normalized
                e.Graphics.FillRectangle(b, -0.5f, -0.5f, ((speedval) / 100.0f) * width, height + 1f);

            }
            if (UseBar)
            {
                using (Brush b = new SolidBrush(this.BarColor))
                {
                    //Note: value is already normalized
                    e.Graphics.FillRectangle(b, (((speedval) / 100.0f) * width) - (BarWidth / 2f + .5f), -0.5f, BarWidth, height + 1f);
                }
            }
            if (Label)
            {
                using (Brush p = new SolidBrush(this.BorderColor))
                {
                    string v = this.Value.ToString("0.00");
                    if (!Decimal)
                        v = this.Value.ToString("0");
                    SizeF x = e.Graphics.MeasureString(v, this.Font);
                    x.Height = (this.Height - x.Height) / 2.0f;
                    x.Width = (this.Width - x.Width) / 2.0f;
                    if (Orientation == Orientation.Vertical)
                    {
                        e.Graphics.ResetTransform();
                        e.Graphics.DrawString(v, this.Font, p, x.ToPointF());
                        e.Graphics.TranslateTransform(.5f, (float)this.Height - .5f);
                        e.Graphics.RotateTransform(-90);
                    }
                    else
                        e.Graphics.DrawString(v, this.Font, p, x.ToPointF());
                }
            }

            if (BorderStyle != BorderStyle.None)
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                using (Pen p = new Pen(this.BorderColor))
                {
                    e.Graphics.DrawRectangle(p, 0, 0, width - 1, height - 1);
                }
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
    }

    namespace Design
    {
        internal class ValueMeterDesigner : ControlDesigner
        {
            ValueMeter vm;
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

            public ValueMeterDesigner()
            {

            }
            public override void Initialize(IComponent component)
            {
                base.Initialize(component);
                vm = (ValueMeter)component;

            }

            void vm_MouseMove(object sender, MouseEventArgs e)
            {
                vm.Value += .01f;
            }
            protected override void OnSetCursor()
            {
                if (!inadorn && !dragin)
                    base.OnSetCursor();
            }

            public Rectangle GetValueRec()
            {
                Rectangle r = new Rectangle();
                float value = (vm.Value - vm.Min) / (vm.Max - vm.Min);
                float width = vm.Orientation == Orientation.Vertical ? vm.Width : vm.Height;
                float height = vm.Orientation == Orientation.Horizontal ? vm.Width : vm.Height;
                float barv = (vm.BarWidth) / (height);
                if (vm.UseBar == false)
                    barv = (2) / (height);
                if (vm.Orientation == Orientation.Vertical)
                {
                    r.Location = new Point(0, (int)(height - (value * height) - (barv * height / 2)));
                    r.Width = (int)width;
                    r.Height = (int)(barv * height);
                }
                else
                {
                    value = 1 - value;
                    r.Location = new Point((int)(height - (value * height) - (barv * height / 2)), 0);
                    r.Width = (int)(barv * height);
                    r.Height = (int)width;
                }
                return r;
            }
            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case WM_LButtonDown:
                        if (GetValueRec().Contains(new Point(m.LParam.ToInt32())))
                        {
                            dragin = true;
                            Cursor.Current = Cursors.Hand;
                        }
                        break;
                    case WM_MouseMove:
                        if (dragin)
                        {
                            Point p = new Point(m.LParam.ToInt32());

                            float value = 1 - ((vm.Value - vm.Min) / (vm.Max - vm.Min));
                            float height = vm.Height;
                            float nval = 1 - p.Y / height;
                            if (vm.Orientation == Orientation.Horizontal)
                            {
                                height = vm.Width;
                                nval = p.X / height;
                            }
                            nval = Math.Max(0, Math.Min(1, nval));
                            if (value != nval)
                                vm.Value = (nval * (vm.Max - vm.Min)) + vm.Min;
                            Cursor.Current = Cursors.Hand;
                            return;
                        }
                        else if (GetValueRec().Contains(new Point(m.LParam.ToInt32())))
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
                        vbs.Add(new DesignerVerb("Show/Hide Label", new EventHandler(showHideLbl)));
                        vbs.Add(new DesignerVerb("Set range to normalized", new EventHandler(Reset1_1)));
                        vbs.Add(new DesignerVerb("Set range to battery", new EventHandler(Resetbat)));
                    }
                    return vbs;
                }
            }
            public void showHideLbl(object sender, EventArgs e)
            {
                vm.Label = !vm.Label;
            }
            public void Reset1_1(object sender, EventArgs e)
            {
                vm.Min = -1f;
                vm.Max = 1f;
                vm.Value = 0f;
            }
            public void Resetbat(object sender, EventArgs e)
            {
                vm.Min = 7f;
                vm.Max = 14.5f;
                vm.HighThreshold = 13.25f;
                vm.LowThreshold = 9.5f;
                vm.Value = 12f;
            }
        }
    }
}