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
using System451.Communication.Dashboard.Properties;
using System.Drawing.Imaging;

namespace System451.Communication.Dashboard
{
    [ToolboxBitmap(typeof(icofinds), "System451.Communication.Dashboard.TBB.DashboardSpeed.bmp")]
    //[ToolboxBitmap(typeof(Button))]
    public partial class RoundSpeedMeter : ZomBControl
    {
        float speedval = 0;
        delegate void UpdaterDelegate(string value);
        
        public RoundSpeedMeter()
        {
            InitializeComponent();
            speedval = 0;
            this.ControlName = "pwm1";
        }
        [DefaultValue("0"), Category("ZomB"), Description("The Value of the Speed Meter")]
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


        [Browsable(false), Obsolete("Use Control Name"), Category("ZomB"), Description("[OBSOLETE] What this control will get the value of from the packet Data")]
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
            }
        }

        private void RoundSpeedMeter_Paint(object sender, PaintEventArgs e)
        {
            if (Math.Abs(Value) > 1)
            {
                if (Value == -999.99)
                {
                }
                else
                {
                    Value = (Value < -1) ? -1 : 1;
                }
            }
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.Clear(this.BackColor);
            e.Graphics.TranslateTransform(0, 15);
            e.Graphics.ScaleTransform((float)this.Width / 173f, (float)this.Height / 167f);
            using (Brush flufbrush = new TextureBrush(Resources.DashboardSpeedFlufs, System.Drawing.Drawing2D.WrapMode.Clamp))
            {
                e.Graphics.FillPie(flufbrush, 0, 0, 172, 167, -90, Value * 115);
            }
            e.Graphics.ResetTransform();
            e.Graphics.ScaleTransform((float)this.Width / 173f, (float)this.Height / 167f);
            using (Brush basebrash = new TextureBrush(Resources.DashboardSpeedBase, System.Drawing.Drawing2D.WrapMode.Clamp))
            {
                e.Graphics.ResetTransform();
                e.Graphics.ScaleTransform((float)this.Width / 173f, (float)this.Height / 167f);
                e.Graphics.TranslateTransform(20, 0);
                e.Graphics.FillRectangle(basebrash, 0, 0, 132, 167);
            }
        }
    }
}
