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
using System.Drawing.Drawing2D;

namespace System451.Communication.Dashboard
{
    [ToolboxBitmap(typeof(icofinds), "System451.Communication.Dashboard.TBB.Graph.png")]
    public partial class DataGraph  : UserControl, IDashboardControl
    {
        float speedval = 0;
        Queue<float> values = new Queue<float>();
        string paramName = "graph1";
        delegate void UpdaterDelegate();
        
        public DataGraph()
        {
            InitializeComponent();

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
                while (values.Count >=200)
                {
                    values.Dequeue();
                }
                this.Invalidate();
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
        [DefaultValue("graph1"), Category("ZomB"), Description("What this control will get the value of from the packet Data")]
        public string BindToInput
        {
            get { return paramName; }
            set { paramName = value; }
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

        #endregion

        private void DataGraph_Paint(object sender, PaintEventArgs e)
        {

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.ScaleTransform((float)this.Width / 200f, (float)this.Height / 200f);
            float[] vals = values.ToArray();
            Array.Reverse(vals);
            //Draw background
            using (Brush mhb = new HatchBrush(HatchStyle.Cross,Color.Green, BackColor))
            e.Graphics.FillRectangle(mhb, 0,0,200,200);
            if (vals.Length>2)
            for (int i = 1; i < vals.Length; i++)
            {
                e.Graphics.DrawLine(Pens.LightGreen,
                    200 - (i - 1), 200-(((vals[i-1]-Min)/(Max-Min)) * 200),
                    200 - (i), 200-(((vals[i]-Min)/(Max-Min)) * 200));
            }
        }
    }
}
