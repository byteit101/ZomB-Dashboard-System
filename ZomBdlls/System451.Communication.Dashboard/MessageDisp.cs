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
