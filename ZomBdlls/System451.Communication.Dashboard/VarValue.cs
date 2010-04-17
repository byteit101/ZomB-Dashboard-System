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
using System.Collections.Specialized;
using System.Reflection;

namespace System451.Communication.Dashboard
{
    public partial class VarValue : UserControl,IDashboardControl
    {
        System.Collections.Generic.Dictionary<string, string> vrs = new Dictionary<string, string>();

        delegate void UpdaterDelegate();

        public VarValue()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            
            
        }
        

        #region IDashboardControl Members
        [Browsable(false)]
        public string[] ParamName
        {
            get
            {
                return new string[]{"dbg"};
            }
            set
            {
               
            }
        }

        [DefaultValue("")]
        public string Value
        {
            get
            {
                return "";
            }
            set
            {
                if (value != "")
                {
                    if (value.Contains(": "))
                        vrs[value.Substring(0, value.IndexOf(": "))] = value.Substring(value.IndexOf(": ")+2);
                }
            }
        }
        

        public string DefalutValue
        {
            get { return ""; }
        }
        new public void Update()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdaterDelegate(Update));
            }
            else
            {
                label1.Text = label2.Text = "";
                foreach (KeyValuePair<string, string> kv in vrs)
                {
                    label1.Text += kv.Key + "\r\n";
                    label2.Text += kv.Value + "\r\n";
                }
                label1.Text += " \r\n";
                label2.Text += " \r\n";
                base.Update();
            }
        }

        #endregion

        private void panel1_Scroll(object sender, ScrollEventArgs e)
        {
            this.Invalidate();
        }


    }
    

}
