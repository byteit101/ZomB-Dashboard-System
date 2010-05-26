using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace System451.Communication.Dashboard
{
    public class ValueMeter : ZomBControl
    {
        float speedval,aval;
        delegate void UpdaterDelegate(string value);

        public ValueMeter()
        {
            Max = 100;
            this.DoubleBuffered = true;
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(125,200);
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
        float alowthresh = 10f,lowthresh = 10f;
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

            if (speedval<=lowthresh)
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
                        SizeF x=e.Graphics.MeasureString(v, this.Font);
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

        public override void UpdateControl(string value)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdaterDelegate(UpdateControl), value);
            }
            else
            {
                this.Value = float.Parse(value);
            }
        }
    }
}
