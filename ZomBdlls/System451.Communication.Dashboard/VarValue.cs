﻿/*
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
using System.Collections.Specialized;
using System.Reflection;

namespace System451.Communication.Dashboard
{
    public partial class VarValue : UserControl, IZomBControl
    {
        System.Collections.Generic.Dictionary<string, string> vrs = new Dictionary<string, string>();

        delegate void UpdaterDelegate(string value);

        public VarValue()
        {
            InitializeComponent();
            this.DoubleBuffered = true;


        }

        [Browsable(false), Obsolete("Use Control Name"), Category("ZomB"), Description("[OBSOLETE] What this control will get the value of from the packet Data")]
        public string BindToInput
        {
            get { return ControlName; }
        }
        public string ControlName
        {
            get
            {
                return "dbg";
            }
            set
            {

            }
        }

        public void UpdateControl(string value)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdaterDelegate(UpdateControl));
            }
            else
            {
                if (value != "")
                {
                    if (value.Contains(": "))
                        vrs[value.Substring(0, value.IndexOf(": "))] = value.Substring(value.IndexOf(": ") + 2);
                }
                label1.Text = label2.Text = "";
                foreach (KeyValuePair<string, string> kv in vrs)
                {
                    label1.Text += kv.Key + "\r\n";
                    label2.Text += kv.Value + "\r\n";
                }
                label1.Text += " \r\n";
                label2.Text += " \r\n";
                this.Update();
            }
        }
        private void panel1_Scroll(object sender, ScrollEventArgs e)
        {
            this.Invalidate();
        }



        #region IZomBControl Members

        public bool RequiresAllData
        {
            get { return true; }
        }

        public bool IsMultiWatch
        {
            get { return false; }
        }

        public void UpdateControl(string value, byte[] packetData)
        {
            ///FROM DDH///
            //this needs to be tested, but should work
            string Output = UTF7Encoding.UTF7.GetString(packetData);

            //Find segment of data
            if (Output.Contains("@@ZomB:|") && Output.Contains("|:ZomB@@"))
            {
                Output = Output.Substring(Output.IndexOf("@@ZomB:|") + 8, (Output.IndexOf("|:ZomB@@") - (Output.IndexOf("@@ZomB:|") + 8)));
                if (Output != "")
                {
                    string[] vars = Output.Split('|');
                    foreach (string item in vars)
                    {
                        if (item.StartsWith("dbg="))
                            UpdateControl(item.Substring(4));
                    }
                }
            }
        }

        public new void ControlAdded(object sender, ZomBControlAddedEventArgs e)
        {

        }

        #endregion
    }


}
