using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace System451.Communication.Dashboard
{
    public partial class PercentBar : Control, IDashboardControl
    {
        float speedval;

        delegate void UpdaterDelegate();
        public PercentBar()
        {
            Max = 100;
            InitializeComponent();
        }
        [DefaultValue(0f), Category("ZomB"), Description("The Value of the PercentPar")]
        public float Value
        {
            get
            {
                return (float)Math.Round((speedval / 100.0) * (Max - Min) + Min,5);
            }
            set
            {
                speedval = ((value - Min) / (Max - Min)) * 100f;
                this.Invalidate();
            }
        }

        [DefaultValue(100f), Category("ZomB"), Description("The Max value of the PercentPar")]
        public float Max
        {
            get;
            set;
        }
        [DefaultValue(0f), Category("ZomB"), Description("The Min value of the PercentPar")]
        public float Min
        {
            get;
            set;
        }

        [DefaultValue(false), Category("ZomB"), Description("Create a bar instead of a traditional Progress bar")]
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
            e.Graphics.Clear(this.BackColor);

            using (Brush b = new SolidBrush(this.ForeColor))
            {
                if (UseBar)
                {
                    e.Graphics.FillRectangle(b, ((speedval) / 100.0f * this.Width) - 3.5f, -0.5f, 6.0f, this.Height + 1f);
                }
                else
                {
                    e.Graphics.FillRectangle(b, -0.5f, -0.5f, (speedval + 1.0f) / 100.0f * this.Width, this.Height + 1f);
                }
            }
        }

        #region IDashboardControl Members

        string[] IDashboardControl.ParamName
        {
            get
            {
                return new string[] { BindToInput };
            }
            set
            {
                BindToInput = value[0];
            }
        }
        [DefaultValue("digi1"), Category("ZomB"), Description("What this control will get the value of from the packet Data")]
        public string BindToInput
        {
            get;
            set;
        }

        string IDashboardControl.Value
        {
            get
            {
                return Value.ToString();
            }
            set
            {
                Value = float.Parse(value);
            }
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


        string IDashboardControl.DefalutValue
        {
            get
            {
                return Value.ToString();
            }
        }

        #endregion
    }
}
