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

namespace System451.Communication.Dashboard
{
    [ToolboxBitmap(typeof(icofinds), "System451.Communication.Dashboard.TBB.Distance.png")]
    public partial class DistanceMeter  : ZomBControl
    {
        float speedval = 0;
        delegate void UpdaterDelegate(string value);
        
        public DistanceMeter()
        {
            InitializeComponent();
            speedval = 0;
            ControlName = "distance";
        }
        [DefaultValue("0"), Category("ZomB"), Description("The Value of the Distance Meter")]
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
                this.Invalidate();
            }
        }

        private Color col = Color.Peru;
        [DefaultValue(typeof(Color), "Peru"), Category("ZomB"), Description("Color of the Meter")]
        public Color DistanceColor
        {
            get
            {
                return col;
            }

            set
            {
                if (col != value)
                    col = value;
                this.Invalidate();
            }

        }

        private void DistanceMeter_Paint(object sender, PaintEventArgs e)
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
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.Clear(this.BackColor);
            e.Graphics.ScaleTransform((float)this.Width / 100f, (float)this.Height / 150f);
            
            using (Brush colorBrush = new SolidBrush(DistanceColor))
            {
                e.Graphics.FillRectangle(colorBrush, 0, 150-(Value*150), 100, (Value*150));
            }
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            e.Graphics.ResetTransform();
            e.Graphics.DrawRectangle(Pens.Black, 0, 0, this.Width-1, this.Height-1);
        }
    }
}
