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
using System.Windows.Forms;

namespace System451.Communication.Dashboard
{
    public partial class PercentBar : ZomBControl
    {
        float speedval;

        delegate void UpdaterDelegate(string value);
        public PercentBar()
        {
            Max = 100;
            BarWidth = 6f;
            InitializeComponent();
        }
        [DefaultValue(0f), Category("ZomB"), Description("The Value of the PercentPar")]
        public float Value
        {
            get
            {
                return (float)Math.Round((speedval / 100.0) * (Max - Min) + Min, 5);
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
        [DefaultValue(6f), Category("ZomB"), Description("The Width of the Bar")]
        public float BarWidth
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
                    e.Graphics.FillRectangle(b, ((speedval) / 100.0f * this.Width) - (BarWidth / 2f + .5f), -0.5f, BarWidth, this.Height + 1f);
                }
                else
                {
                    e.Graphics.FillRectangle(b, -0.5f, -0.5f, (speedval + 1.0f) / 100.0f * this.Width, this.Height + 1f);
                }
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
            }
        }
    }
}
