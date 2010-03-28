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
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;
using System.IO;
using System.Collections.ObjectModel;

namespace System451.Communication.Dashboard
{
    public class DashboardDataHub : Component
    {
        public delegate void DashboardDataRecievedDelegate(string getValue);
        public event DashboardDataRecievedDelegate DashboardDataRecieved;
        UdpClient cRIOConnection;
        //delegate void SetTextCallback(string text);
        Thread mt;
        Stack<IDashboardControl> controls = new Stack<IDashboardControl>();
         
        public DashboardDataHub()
        {
            mt = new Thread(new ThreadStart(this.moniter));
            mt.IsBackground = true;
           
            
            DashboardDataRecieved += new DashboardDataRecievedDelegate(DashboardReciever_DashboardDataRecieved);
        }

        ~DashboardDataHub()
        {
            try
            {

                cRIOConnection.Client.Disconnect(false);
                cRIOConnection.Close();
            }
            catch
            {
            }
        }
        public Stack<IDashboardControl> GetControls()
        {
            return controls;
        }

        void DashboardReciever_DashboardDataRecieved(string getValue)
        {
            UpdateControls(getValue);
        }

        /// <summary>
        /// Add a control to the Dashboard
        /// </summary>
        /// <param name="control">the control inplementing IDashboardControl</param>
        public void AddDashboardControl(IDashboardControl control)
        {
            controls.Push(control);
        }

        /// <summary>
        /// Add a bunch of controls to the Dashboard
        /// </summary>
        /// <param name="controls">the controls inplementing IDashboardControl</param>
        public void AddDashboardControl(Collection<IDashboardControl> controls)
        {
            foreach (IDashboardControl control in controls)
            {
                AddDashboardControl(control);
            }
        }

        private void UpdateControls(string getValue)
        {
            foreach (IDashboardControl cont in controls)
            {
                cont.Value = GetParam(cont.ParamName[0], getValue, cont.DefalutValue);
                cont.Update();
            }
        }

        /// <summary>
        /// Start Monitering the Dashboard port
        /// </summary>
        public void StartRecieving()
        {
            if (cRIOConnection == null)
            {
                try
                {
                    cRIOConnection = new UdpClient(1165);
                    mt.Start();
                }
                catch
                {
                    MessageBox.Show("1165 not open");
                }
            }
           
        }

        /// <summary>
        /// Stop monitering the dashboard port
        /// </summary>
        public void StopRecieving()
        {
            try
            {
                mt.Abort();

            }
            catch
            {
            }

        }

        private void moniter()
        {
            try
            {
                while (true)
                {
                    IPEndPoint RIPend = null;
                    byte[] buffer = cRIOConnection.Receive(ref RIPend);
                    string Output = "";

                    for (int cnr = 0; cnr < buffer.Length; cnr++)
                    {
                        Output += ((buffer[cnr] != 0) ? ((char)buffer[cnr]).ToString() : "");
                    }

                    //SetText2(Output);

                    if (Output.Contains("@@@451:|") && Output.Contains("|:451@@@"))
                    {
                        Output = Output.Substring(Output.IndexOf("@@@451:|")+8, (Output.IndexOf("|:451@@@") - (Output.IndexOf("@@@451:|")+8)));
                        if (Output != "")
                        {
                            
                            DashboardDataRecieved(Output);
                        }
                    }
                    //Output = Output.Replace("|", "\n|");
                    //SetText(Output);

                }
                //DashboardDataRecieved();

            }
            catch(Exception ex)
            {
                MessageBox.Show("Error! moniter failed: \n\n"+ex.ToString());
                return;
            }
        }

        private string GetParam(string ParamName, string ParamString, string DefaultValue)
        {
            foreach (string ValString in ParamString.Split(new char[] { '|' }))
            {
                if (ValString.ToUpper().StartsWith(ParamName.ToUpper()))
                {
                    if (ValString.Split(new char[] { '=' })[1] == "NaN")
                    {
                        return "0";
                    }
                    return ValString.Split(new char[] { '=' })[1];
                }
            }
            return DefaultValue;
        }
        //private void SetText(string ntext)
        //{
        //    if (this.TextBox1.InvokeRequired)
        //    {
        //        SetTextCallback method = new SetTextCallback(this.SetText);
        //        base.Invoke(method, new object[] { ntext });
        //    }
        //    else
        //    {
        //        this.TextBox1.Text = ntext;
        //    }
        //}

        //private void SetText2(string ntext)
        //{
        //    if (this.TextBox1.InvokeRequired)
        //    {
        //        SetTextCallback method = new SetTextCallback(this.SetText2);
        //        base.Invoke(method, new object[] { ntext });
        //    }
        //    else
        //    {
        //        this.TextBox1.Tag = ntext;
        //    }
        //}

    }
}