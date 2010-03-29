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

namespace System451.Communication.Dashboard
{
    [ToolboxBitmap(typeof(icofinds), "System451.Communication.Dashboard.TBB.OnOff.png")]
    public partial class OnOffControl : UserControl, IDashboardControl
    {
        bool speedval = false;
        string paramName = "digi1";
        delegate void UpdaterDelegate();

        public OnOffControl()
        {
            InitializeComponent();

        }
        [DefaultValue("0"), Category("ZomB"), Description("The Value of the bool")]
        public bool Value
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
                if (value == "0" || value == "1")
                    Value = int.Parse(value) == 0 ? false : true;
                else
                    Value = bool.Parse(value);
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

        private void OnOffControl_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.Clear(this.BackColor);
            e.Graphics.ScaleTransform((float)this.Width / 25f, (float)this.Height / 25f);
            if (Value)
            {
                e.Graphics.FillEllipse(Brushes.DarkGreen, 1, 1, 23, 23);
                e.Graphics.FillEllipse(Brushes.ForestGreen, 3, 3, 19, 19);
                e.Graphics.FillEllipse(Brushes.LawnGreen, 6, 6, 13, 13);
            }
            else
            {
                e.Graphics.FillEllipse(Brushes.DarkRed, 1, 1, 23, 23);
                e.Graphics.FillEllipse(Brushes.Crimson, 3, 3, 19, 19);
                e.Graphics.FillEllipse(Brushes.Red, 6, 6, 13, 13);
            }
        }
    }
    [ToolboxBitmap(typeof(icofinds), "System451.Communication.Dashboard.TBB.OnOff.png")]
    public partial class AlertControl : UserControl, IDashboardControl
    {
        bool speedval = false;
        string paramName = "digi1";
        delegate void UpdaterDelegate();

        public AlertControl()
        {
            InitializeComponent();

        }
        [DefaultValue("0"), Category("ZomB"), Description("The Value of the bool")]
        public bool Value
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
                if (value == "0" || value == "1")
                    Value = int.Parse(value) == 0 ? false : true;
                else
                    Value = bool.Parse(value);
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

        private void OnOffControl_Paint(object sender, PaintEventArgs e)
        {
            if (Value)
            {
                e.Graphics.Clear(this.ForeColor);
            }
            else
            {
                e.Graphics.Clear(this.BackColor);
            }
        }
    }
    public enum SpikePositions
    {
        Forward = 1,
        Off = 0,
        Reverse = -1
    };
    [ToolboxBitmap(typeof(icofinds), "System451.Communication.Dashboard.TBB.Spike.png")]
    public partial class SpikeControl : UserControl, IDashboardControl
    {
        SpikePositions speedval = SpikePositions.Off;
        string paramName = "spk1";
        delegate void UpdaterDelegate();

        public SpikeControl()
        {
            InitializeComponent();

        }
        [DefaultValue("Off"), Category("ZomB"), Description("The Value of the Spike")]
        public SpikePositions Value
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
        [DefaultValue("spk1"), Category("ZomB"), Description("What this control will get the value of from the packet Data")]
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
                Value = (SpikePositions)int.Parse(value);
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
                return ((int)Value).ToString();
            }
        }

        #endregion

        private void SpikeControl_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.Clear(this.BackColor);
            e.Graphics.ScaleTransform((float)this.Width / 25f, (float)this.Height / 25f);
            if (Value == SpikePositions.Forward)
            {
                e.Graphics.FillRectangle(Brushes.DarkGreen, 1 - .5f, 1 - .5f, 23, 23);
                e.Graphics.FillRectangle(Brushes.ForestGreen, 3 - .5f, 3 - .5f, 19, 19);
                e.Graphics.FillRectangle(Brushes.LawnGreen, 6 - .5f, 6 - .5f, 13, 13);
            }
            else if (Value == SpikePositions.Reverse)
            {
                e.Graphics.FillRectangle(Brushes.DarkRed, 1 - .5f, 1 - .5f, 23, 23);
                e.Graphics.FillRectangle(Brushes.Crimson, 3 - .5f, 3 - .5f, 19, 19);
                e.Graphics.FillRectangle(Brushes.Red, 6 - .5f, 6 - .5f, 13, 13);
            }
            else
            {
                e.Graphics.FillRectangle(Brushes.Black, 1 - .5f, 1 - .5f, 23, 23);
            }
        }
    }
}
