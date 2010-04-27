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
    [ToolboxBitmap(typeof(icofinds), "System451.Communication.Dashboard.TBB.OnOff.png")]
    public partial class OnOffControl : ZomBControl
    {
        bool speedval = false;
        delegate void UpdaterDelegate(string value);

        public OnOffControl()
        {
            InitializeComponent();

            ControlName = "digital1";
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
                if (value == "0" || value == "1")
                    Value = int.Parse(value) == 0 ? false : true;
                else
                    Value = bool.Parse(value);
            }
        }

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
    public partial class AlertControl : ZomBControl
    {
        bool speedval = false;
        delegate void UpdaterDelegate(string value);

        public AlertControl()
        {
            InitializeComponent();
            ControlName = "alert1";
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
                if (value == "0" || value == "1")
                    Value = int.Parse(value) == 0 ? false : true;
                else
                    Value = bool.Parse(value);
            }
        }
             
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
    public partial class SpikeControl : ZomBControl
    {
        SpikePositions speedval = SpikePositions.Off;
        delegate void UpdaterDelegate(string value);

        public SpikeControl()
        {
            InitializeComponent();
            ControlName = "spike1";
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
                Value = (SpikePositions)int.Parse(value);
            }
        }

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
