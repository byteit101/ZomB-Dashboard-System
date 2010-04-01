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
using System.ComponentModel;
using System.Drawing;
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
