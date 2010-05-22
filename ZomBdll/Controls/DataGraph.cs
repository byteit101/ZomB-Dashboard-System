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
using System.Drawing.Drawing2D;

namespace System451.Communication.Dashboard
{
    [ToolboxBitmap(typeof(icofinds), "System451.Communication.Dashboard.TBB.Graph.png")]
    public partial class DataGraph : ZomBControl
    {
        float speedval = 0;
        Queue<float> values = new Queue<float>();

        delegate void UpdaterDelegate(string value);

        public DataGraph()
        {
            LineColor = Color.LimeGreen;
            InitializeComponent();
            ControlName = "graph1";
        }
        [DefaultValue("0"), Category("ZomB"), Description("The Value of the next Graph Point")]
        public float Value
        {
            get
            {
                return speedval;
            }
            set
            {
                speedval = value;
                values.Enqueue(value);
                while (values.Count >= 200)
                {
                    values.Dequeue();
                }
                this.Invalidate();
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
                this.Invalidate();
            }
        }

        private float maxgrph = 100;
        [DefaultValue(100), Category("ZomB"), Description("Maximum Value of the graph")]
        public float Max
        {
            get
            {
                return maxgrph;
            }

            set
            {
                if (maxgrph != value)
                    maxgrph = value;
                this.Invalidate();
            }

        }
        private float mingrph = 0;
        [DefaultValue(0), Category("ZomB"), Description("Minimun Value of the graph")]
        public float Min
        {
            get
            {
                return mingrph;
            }

            set
            {
                if (mingrph != value)
                    mingrph = value;
                this.Invalidate();
            }

        }


        private void DataGraph_Paint(object sender, PaintEventArgs e)
        {

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.ScaleTransform((float)this.Width / 200f, (float)this.Height / 200f);
            float[] vals = values.ToArray();
            Array.Reverse(vals);
            //Draw background
            using (Brush mhb = new HatchBrush(HatchStyle.Cross, ForeColor, BackColor))
                e.Graphics.FillRectangle(mhb, 0, 0, 200, 200);
            if (vals.Length > 2)
                using (Pen pn = new Pen(LineColor))
                {
                    for (int i = 1; i < vals.Length; i++)
                    {
                        e.Graphics.DrawLine(pn,
                            200 - (i - 1), 200 - (((vals[i - 1] - Min) / (Max - Min)) * 200),
                            200 - (i), 200 - (((vals[i] - Min) / (Max - Min)) * 200));
                    }
                    if (Arrow)
                    {
                        pn.EndCap = LineCap.ArrowAnchor;
                        pn.Width = 2.5f;
                        e.Graphics.DrawLine(pn,
                              199, 200 - (((vals[0] - Min) / (Max - Min)) * 200),
                              200, 200 - (((vals[0] - Min) / (Max - Min)) * 200));
                    }
                }
        }

        [DefaultValue(typeof(Color), "LimeGreen"), Category("ZomB"), Description("The color of the value line")]
        public Color LineColor { get; set; }
        [DefaultValue(false), Category("ZomB"), Description("Should we show an arrow at the end of the Graph")]
        public bool Arrow { get; set; }
    }
}
