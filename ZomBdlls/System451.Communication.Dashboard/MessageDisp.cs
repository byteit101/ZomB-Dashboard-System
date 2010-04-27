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
    public partial class MessageDisp : ZomBControl
    {
        public MessageDisp()
        {
            InitializeComponent();
            this.ControlName = "printf";
        }
string text = "";
delegate void UpdaterDelegate(string value);

        [DefaultValue("0"), Category("ZomB"), Description("The Value of the control")]
        public string Value
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                textBox1.Text = value;
            }
        }
        bool append;
        [DefaultValue(true), Category("ZomB"), Description("Should we append like the console, or overwrite")]
        public bool Append
        {
            get
            {
                return append;
            }
            set
            {
                append = value;
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
                if (Append)
                    Value += value;
                else
                    Value=value;
            }
        }
    }
}
