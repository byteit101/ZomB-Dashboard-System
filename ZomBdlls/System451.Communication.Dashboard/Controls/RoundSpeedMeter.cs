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
