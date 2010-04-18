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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace System451.Communication.Dashboard
{
    /// <summary>
    /// This is the main controller of the dashboard packets data
    /// </summary>
    public class DashboardDataHub : Component, IZomBController
    {
        [Obsolete("This will be replaced with IZomBControl, and removed in v0.7 and later")]
        public delegate void DashboardDataRecievedDelegate(string getValue);
        [Obsolete("This will be replaced with IZomBControl, and removed in v0.7 and later")]
        public event DashboardDataRecievedDelegate DashboardDataRecieved;

        public event ErrorEventHandler OnError;

        UdpClient cRIOConnection;
        bool isrunning;
        Thread mt;
        Stack<IDashboardControl> controls = new Stack<IDashboardControl>();

        Collection<IZomBControl> zomBcontrols = new Collection<IZomBControl>();
        Collection<IZomBControlGroup> zomBgroups = new Collection<IZomBControlGroup>();
        Collection<IZomBMonitor> zomBmonitors = new Collection<IZomBMonitor>();

        FRCDSStatus currentstatus;

        /// <summary>
        /// Creates a new DashboardDataHub
        /// </summary>
        public DashboardDataHub()
        {
            DashboardDataRecieved += new DashboardDataRecievedDelegate(DashboardReciever_DashboardDataRecieved);
        }

        /// <summary>
        /// Annihilates the DashboardDataHub
        /// </summary>
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

        /// <summary>
        /// Adds a new ZomB control.
        /// </summary>
        /// <param name="control">The control to add. If this is already in the data hub, it will be ignored</param>
        public void Add(IZomBControl control)
        {
            if (!zomBcontrols.Contains(control))
            {
                zomBcontrols.Add(control);
                control.ControlAdded(this, new ZomBControlAddedEventArgs(this));
            }
        }

        /// <summary>
        /// Adds a new ZomB control group.
        /// </summary>
        /// <param name="control">The control group to add. If this is already in the data hub, it will be ignored</param>
        public void Add(IZomBControlGroup controlgroup)
        {
            if (!zomBgroups.Contains(controlgroup))
            {
                zomBgroups.Add(controlgroup);
                foreach (KeyValuePair<string, IZomBControl> item in controlgroup.GetControls())
                {
                    item.Value.ControlAdded(this, new ZomBControlAddedEventArgs(this));
                }
            }
        }

        /// <summary>
        /// Adds a new ZomB monitor.
        /// </summary>
        /// <param name="control">The monitor to add. If this is already in the data hub, it will be ignored</param>
        public void Add(IZomBMonitor monitor)
        {
            if (!zomBmonitors.Contains(monitor))
            {
                zomBmonitors.Add(monitor);
            }
        }

        /// <summary>
        /// Removes a ZomB control.
        /// </summary>
        /// <param name="control">The control to remove.</param>
        public void Remove(IZomBControl control)
        {
            if (zomBcontrols.Contains(control))
            {
                zomBcontrols.Remove(control);
            }
        }

        /// <summary>
        /// Removes a ZomB control group.
        /// </summary>
        /// <param name="control">The control group to remove.</param>
        public void Remove(IZomBControlGroup controlgroup)
        {
            if (zomBgroups.Contains(controlgroup))
            {
                zomBgroups.Remove(controlgroup);
            }
        }

        /// <summary>
        /// Removes a ZomB monitor.
        /// </summary>
        /// <param name="control">The monitor to remove.</param>
        public void Remove(IZomBMonitor monitor)
        {
            if (zomBmonitors.Contains(monitor))
            {
                zomBmonitors.Remove(monitor);
            }
        }

        [Obsolete("This will be replaced with IZomBControl, and removed in v0.7 and later")]
        public Stack<IDashboardControl> GetControls()
        {
            return controls;
        }

        void DashboardReciever_DashboardDataRecieved(string getValue)
        {
            UpdateControls(getValue);
        }

        /// <summary>
        /// Add a control to the Dashboard (Obsolete)
        /// </summary>
        /// <param name="control">the control inplementing IDashboardControl</param>
        [Obsolete("This will be replaced with IZomBControl, and removed in v0.7 and later")]
        public void AddDashboardControl(IDashboardControl control)
        {
            controls.Push(control);
        }

        /// <summary>
        /// Add a bunch of controls to the Dashboard (Obsolete)
        /// </summary>
        /// <param name="controls">the controls inplementing IDashboardControl</param>
        [Obsolete("This will be replaced with IZomBControl, and removed in v0.7 and later")]
        public void AddDashboardControl(Collection<IDashboardControl> controls)
        {
            foreach (IDashboardControl control in controls)
            {
                AddDashboardControl(control);
            }
        }

        [Obsolete("This will be replaced with IZomBControl, and removed in v0.7 and later")]
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
        [Obsolete("This will be replaced with Start, and removed in v0.7 and later")]
        public void StartRecieving()
        {
            Start();
        }

        /// <summary>
        /// Start Monitering the Dashboard port
        /// </summary>
        public void Start()
        {
            if (cRIOConnection == null)
            {
                try
                {
                    cRIOConnection = new UdpClient(1165);
                }
                catch (Exception ex)
                {
                    DoError(ex);
                }
            }
            try
            {
                mt = new Thread(new ThreadStart(this.run));
                mt.IsBackground = true;
                isrunning = true;
                mt.Start();
            }
            catch (Exception ex)
            {
                DoError(ex);
            }
        }

        /// <summary>
        /// Stop monitering the dashboard port
        /// </summary>
        [Obsolete("This will be replaced with Stop, and removed in v0.7 and later")]
        public void StopRecieving()
        {
            Stop();
        }

        /// <summary>
        /// Stop monitering the dashboard port
        /// </summary>
        public void Stop()
        {
            try
            {
                isrunning = false;
                Thread.Sleep(500);
                if (mt.IsAlive)
                    mt.Abort();
            }
            catch
            {
            }
        }

        /// <summary>
        /// The background worker. will exit after 5 consectutive errors
        /// </summary>
        private void run()
        {
            int nume = 0;
            while (isrunning)
            {
                try
                {
                    IPEndPoint RIPend = null;
                    //Recieve the data
                    byte[] buffer = cRIOConnection.Receive(ref RIPend);
                    string Output;

                    //Convert
                    //this works, and is proven
                    //Output="";
                    //for (int cnr = 0; cnr < buffer.Length; cnr++)
                    //{
                    //    Output += ((buffer[cnr] != 0) ? ((char)buffer[cnr]).ToString() : "");
                    //}
                    //this needs to be tested, but should work
                    Output = UTF7Encoding.UTF7.GetString(buffer);

                    //Find segment of data
                    if (Output.Contains("@@ZomB:|") && Output.Contains("|:ZomB@@"))
                    {
                        Output = Output.Substring(Output.IndexOf("@@ZomB:|") + 8, (Output.IndexOf("|:ZomB@@") - (Output.IndexOf("@@ZomB:|") + 8)));
                        if (Output != "")
                        {
                            ProcessControls(Output, buffer);
                        }
                    }
                    if (nume > 0)
                        nume--;
                }
                catch (ThreadAbortException)
                {
                    isrunning = false;
                    return;
                }
                catch (Exception ex)
                {
                    nume++;
                    DoError(ex);
                    if (nume > 5)
                    {
                        isrunning = false;
                        DoError(new Exception("5 consecutive errors were encountered, stopping DashboardDataHub"));
                        isrunning = false;
                        return;
                    }
                }
            }
        }

        private void ProcessControls(string Output, byte[] buffer)
        {
            //Get the items in a dictionary
            Dictionary<string, string> vals = SplitParams(Output);
            FRCDSStatus status = ParseDSBytes(buffer);
            currentstatus = status;

            //Process the Monitors
            foreach (IZomBMonitor monitor in zomBmonitors)
            {
                monitor.UpdateStatus(status);
                monitor.UpdateData(vals, buffer);
            }

            //Process the normal controls
            foreach (IZomBControl cont in zomBcontrols)
            {
                ProcessControl(cont, vals, buffer);
            }

            //TODO: Remove Obsolete methoods
            DashboardDataRecieved(Output);

            //Process the GroupControls
            foreach (IZomBControlGroup group in zomBgroups)
            {
                foreach (KeyValuePair<string, IZomBControl> item in group.GetControls())
                {
                    ProcessControl(item.Value, vals, buffer);
                }
            }
        }

        private static void ProcessControl(IZomBControl control, Dictionary<string, string> vals, byte[] buffer)
        {
            string val = "";
            //if we are watching multiple values
            if (control.IsMultiWatch)
            {
                foreach (var item in control.ControlName.Split(';'))
                {
                    val += "|" + vals[item.Trim()];
                }
                val = val.Substring(1);//remove first |
            }
            else
                val = vals[control.ControlName];//get the value it wants

            //If it does not need the data, don't pass
            if (control.RequiresAllData)
                control.UpdateControl(val, buffer);
            else
                control.UpdateControl(val, null);
        }

        /// <summary>
        /// Gets the current robot status
        /// </summary>
        /// <returns>The current robot status</returns>
        public FRCDSStatus GetDSStatus()
        {
            return currentstatus;
        }

        /// <summary>
        /// Convert the DS Bytes to a FRCDSStatus
        /// </summary>
        /// <param name="buffer">The bytes from the Robot packet</param>
        /// <returns>A FRCDSStatus containg the robot status</returns>
        static protected FRCDSStatus ParseDSBytes(byte[] buffer)
        {
            //TODO: Find and Fix errors here
            FRCDSStatus ret = new FRCDSStatus();
            ret.PacketNumber = buffer[0];
            ret.PacketNumber += (ushort)(buffer[1] >> 8);
            ret.DigitalIn = new DIOBitField(buffer[2]);
            ret.DigitalOut = new DIOBitField(buffer[3]);
            ret.Battery = float.Parse(buffer[4].ToString("x") + "." + buffer[5].ToString("x"));
            ret.Status = new StatusBitField(buffer[6]);
            ret.Error = new ErrorBitField(buffer[7]);
            ret.Team = int.Parse(buffer[8].ToString("x") + buffer[9].ToString("x"));
            //OR
            ret.Team = (int)buffer[8] + (int)(buffer[9] >> 8);
            //TODO: Add version

            return ret;
        }

        /// <summary>
        /// Convert the name=value|n=v form to a Dictionary of name and values
        /// </summary>
        /// <param name="Output"></param>
        /// <returns></returns>
        private Dictionary<string, string> SplitParams(string Output)
        {
            //Split the main string
            string[] s = Output.Split('|');
            Dictionary<string, string> k = new Dictionary<string, string>(s.Length);
            foreach (string item in s)
            {
                //split and add each item to the Dictionary
                string ky, val;
                ky = item.Split('=')[0];
                val = item.Split('=')[1];
                k[ky] = val;//Latter will overwrite
            }
            return k;
        }

        [Obsolete("This will be replaced with IZomBControl, and removed in v0.7 and later")]
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

        /// <summary>
        /// Processes any errors that may have been encountered, and either fires the OnError, or Alerts the user
        /// </summary>
        /// <param name="ex">The error</param>
        protected void DoError(Exception ex)
        {
            if (OnError == null)
                MessageBox.Show(ex.Message);
            else
                OnError(this, new ErrorEventArgs(ex));
        }

        /// <summary>
        /// Kill, and then re-animate ZomB
        /// </summary>
        public static void RestartZomB()
        {
            Application.Restart();
        }

        /// <summary>
        /// Cut off the ZomB's head
        /// </summary>
        public static void ExitZomB()
        {
            Application.Exit();
        }

        /// <summary>
        /// Restart the DriverStation, removing FMS Locked condition
        /// </summary>
        public static void RestartDS()
        {
            Process[] dgs = Process.GetProcessesByName("Driver Station");
            if (dgs.Length != 1)
            {
                MessageBox.Show("DS not running, starting");
            }
            else
            {
                dgs[0].CloseMainWindow();
                dgs[0].WaitForExit(6000);
                //If http://www.youtube.com/watch?v=dGFXGwHsD_A, then kill
                if (!dgs[0].HasExited)
                {
                    dgs[0].Kill();
                }
            }
            try
            {
                Process.Start(@"C:\Program Files\FRC Driver Station\Driver Station.exe");
            }
            catch
            {

            }
        }

        #region IZomBController Members

        DashboardDataHub IZomBController.GetDashboardDataHub()
        {
            return this;
        }

        #endregion
    }

    [ToolboxBitmap(typeof(Button))]
    public class ControlBoxMenuButton : Button
    {

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem restartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restartDSToolStripMenuItem;

        public ControlBoxMenuButton()
        {
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip();
            this.restartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restartDSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            this.contextMenuStrip1.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.restartToolStripMenuItem,
            this.exitToolStripMenuItem,
            this.restartDSToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(138, 70);
            // 
            // restartToolStripMenuItem
            // 
            this.restartToolStripMenuItem.Name = "restartToolStripMenuItem";
            this.restartToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.restartToolStripMenuItem.Text = "&Restart ZomB";
            this.restartToolStripMenuItem.Click += new EventHandler(restartToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.exitToolStripMenuItem.Text = "E&xit ZomB";
            this.exitToolStripMenuItem.Click += new EventHandler(exitToolStripMenuItem_Click);

            // 
            // restartDSToolStripMenuItem
            // 
            this.restartDSToolStripMenuItem.Name = "restartDSToolStripMenuItem";
            this.restartDSToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.restartDSToolStripMenuItem.Text = "Restart DS";
            this.restartDSToolStripMenuItem.Click += new EventHandler(restartDSToolStripMenuItem_Click);


            this.Text = "ZomB";
            this.MouseDown += new MouseEventHandler(ControlBoxMenuButton_MouseDown);

            this.contextMenuStrip1.ResumeLayout(false);
        }

        void ControlBoxMenuButton_MouseDown(object sender, MouseEventArgs e)
        {
            contextMenuStrip1.Show(Cursor.Position);
        }

        void restartDSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DashboardDataHub.RestartDS();
        }

        void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DashboardDataHub.ExitZomB();
        }

        void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DashboardDataHub.RestartZomB();
        }
    }
}