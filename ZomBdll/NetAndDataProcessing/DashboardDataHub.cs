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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System451.Communication.Dashboard.Net;

namespace System451.Communication.Dashboard
{

    /// <summary>
    /// This is the main controller of the dashboard packets data
    /// </summary>
    public class DashboardDataHub : Component, IZomBController
    {
        public event InvalidPacketRecievedEventHandler InvalidPacketRecieved;
        public event ErrorEventHandler OnError;

        bool havestatus = false;
        Collection<IDashboardDataSource> DataSrcs = new Collection<IDashboardDataSource>();

        Collection<IZomBControl> zomBcontrols = new Collection<IZomBControl>();
        Collection<IZomBControlGroup> zomBgroups = new Collection<IZomBControlGroup>();
        Collection<IZomBMonitor> zomBmonitors = new Collection<IZomBMonitor>();

        FRCDSStatus currentstatus;

        /// <summary>
        /// Creates a new DashboardDataHub
        /// </summary>
        public DashboardDataHub()
        {
            ClearSources();
            RegisterDashboardPacketSource();//Default
        }

        void src_OnError(object sender, ErrorEventArgs e)
        {
            if (this.OnError != null)
                OnError(sender, e);
        }

        void src_InvalidPacketRecieved(object sender, InvalidPacketRecievedEventArgs e)
        {
            if (this.InvalidPacketRecieved != null)
                InvalidPacketRecieved(sender, e);
        }

        /// <summary>
        /// Annihilates the DashboardDataHub
        /// </summary>
        ~DashboardDataHub()
        {
            try
            {
                this.Stop();
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
                foreach (IZomBControl item in controlgroup.GetControls())
                {
                    item.ControlAdded(this, new ZomBControlAddedEventArgs(this));
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

        /// <summary>
        /// Start Monitering the Dashboard port
        /// </summary>
        public void Start()
        {
            if (!Running)
            {
                int i = 0;
                try
                {
                    if (StartSource != StartSources.Manual && DataSrcs.Count==0) //TODO: re-evaluate the best way to do this
                    {
                        //Auto setup the sources
                        ClearSources();
                        RegisterSource(StartSource);
                    }
                    for (; i < DataSrcs.Count; i++)
                    {
                        StartSrc(DataSrcs[i]);
                    }
                    Running = true;
                }
                catch
                {
                    for (; i <= 0; i--)
                    {
                        StopSrc(DataSrcs[i]);
                    }
                    Running = false;
                }
            }
        }

        /// <summary>
        /// stop the SCR and any associated srcs
        /// </summary>
        /// <param name="item">The src</param>
        protected void StopSrc(IDashboardDataSource item)
        {
            item.Stop();
            //TODO: Test, this shouldnt be nescesecary
            if (item.HasData && item != (item.GetDataSource() as IDashboardDataSource))
            {
                item.GetDataSource().Stop();
            }
            if (item.HasStatus && item != (item.GetStatusSource() as IDashboardDataSource))
            {
                item.GetStatusSource().Stop();
            }
        }

        /// <summary>
        /// Start the SCR and any associated srcs
        /// </summary>
        /// <param name="item">The src</param>
        protected void StartSrc(IDashboardDataSource item)
        {
            item.Start();
            //TODO: Test
            if (item.HasData && item != (item.GetDataSource() as IDashboardDataSource))
            {
                item.GetDataSource().Start();
            }
            if (item.HasStatus && item != (item.GetStatusSource() as IDashboardDataSource))
            {
                item.GetStatusSource().Start();
            }
        }

        /// <summary>
        /// Stop monitering the dashboard port
        /// </summary>
        public void Stop()
        {
            if (Running)
            {
                try
                {
                    for (int i = 0; i < DataSrcs.Count; i++)
                    {
                        StopSrc(DataSrcs[i]);
                    }
                    Running = false;
                }
                catch
                {
                    Running = true;
                }
            }

        }

        /// <summary>
        /// Are we running?
        /// </summary>
        public bool Running { get; private set; }

        /// <summary>
        /// What the DDH will load as sources when it start()'s
        /// </summary>
        public StartSources StartSource { get; set; }

        /// <summary>
        /// Register a new IDashboardDataSource. This adds it to the collection and uses it
        /// You must not be running the DDH to add successfully
        /// </summary>
        /// <param name="src">The source to add</param>
        /// <returns>True on success, false otherwise</returns>
        public bool RegisterSource(IDashboardDataSource src)
        {
            if (!Running && src != null)
            {
                if (!DataSrcs.Contains(src))
                {
                    src.InvalidPacketRecieved += new InvalidPacketRecievedEventHandler(src_InvalidPacketRecieved);
                    src.OnError += new ErrorEventHandler(src_OnError);
                    src.DataRecieved += new EventHandler(src_DataRecieved);
                    if (src.HasData)
                    {
                        src.GetDataSource().NewDataRecieved += new NewDataRecievedEventHandler(src_NewDataRecieved);
                    }
                    if (src.HasStatus && !havestatus)
                    {
                        src.GetStatusSource().NewStatusRecieved += new NewStatusRecievedEventHandler(src_NewStatusRecieved);
                    }
                    try
                    {
                        DataSrcs.Add(src);
                        return true;
                    }
                    catch { }
                }
            }
            return false;
        }

        /// <summary>
        /// Clears the current sources
        /// You must not be running the DDH to clear successfully
        /// </summary>
        /// <returns>true on success, false otherwise</returns>
        public bool ClearSources()
        {
            if (!Running)
            {
                DataSrcs.Clear();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Register the predefined sources
        /// Will not clear the previous sources
        /// You must not be running the DDH to add successfully
        /// </summary>
        /// <param name="sources">the sources</param>
        /// <returns>true or false</returns>
        protected bool RegisterSource(StartSources sources)
        {
            if (!Running)
            {
                switch (sources)
                {
                    case StartSources.DashboardPacket:
                        return (RegisterDashboardPacketSource() == null ? false : true);
                    case StartSources.Manual:
                        //I can't set it up!
                        break;
                    default:
                        //TODO: make sure this never happens
                        throw new NotImplementedException();
                }
            }
            return false;
        }

        /// <summary>
        /// Registers a DB Packet Data source and returns it if successfull
        /// You must not be running the DDH to add successfully
        /// </summary>
        /// <returns>If successfull, the registerd DBPDS</returns>
        public DashboardPacketDataSource RegisterDashboardPacketSource()
        {
            if (!Running)
            {
                DashboardPacketDataSource src = new DashboardPacketDataSource(this);
                return (RegisterSource(src) ? src : null);
            }
            return null;
        }

        /// <summary>
        /// Registers a TCP Data source and returns it if successfull
        /// You must not be running the DDH to add successfully
        /// </summary>
        /// <param name="team">The team number</param>
        /// <returns>If successfull, the registerd TCPDS</returns>
        public TCPDataSource RegisterTCPSource(int team)
        {
            if (!Running)
            {
                TCPDataSource src = new TCPDataSource(team);
                return (RegisterSource(src) ? src : null);
            }
            return null;
        }

        void src_NewStatusRecieved(object sender, NewStatusRecievedEventArgs e)
        {
            //Process the Monitors
            foreach (IZomBMonitor monitor in zomBmonitors)
            {
                monitor.UpdateStatus(e.NewStatus);
            }
            currentstatus = e.NewStatus;
        }

        void src_NewDataRecieved(object sender, NewDataRecievedEventArgs e)
        {
            //Process the Monitors
            foreach (IZomBMonitor monitor in zomBmonitors)
            {

                monitor.UpdateData(e.NewData);
            }

            //Process the normal controls
            foreach (IZomBControl cont in zomBcontrols)
            {
                ProcessControl(cont, e.NewData);
            }

            //Process the GroupControls
            foreach (IZomBControlGroup group in zomBgroups)
            {
                foreach (IZomBControl item in group.GetControls())
                {
                    ProcessControl(item, e.NewData);
                }
            }
        }

        /// <summary>
        /// Processes the control and gives it what it wants (aka. appesement)
        /// </summary>
        /// <param name="control">Le ZomB control</param>
        /// <param name="vals">El dictionario</param>
        /// <remarks>I must be tired</remarks>
        private static void ProcessControl(IZomBControl control, Dictionary<string, string> vals)
        {
            try
            {
                //TODO: Support dbg style multiple values
                string val = "";
                //if we are watching multiple values
                if (control.IsMultiWatch)
                {
                    if (control.ControlName == "*")
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var item in vals)
                        {
                            sb.Append("|");
                            sb.Append(item.Key);
                            sb.Append("=");
                            sb.Append(item.Value);
                        }
                        val = sb.ToString();
                    }
                    else
                    {
                        foreach (var item in control.ControlName.Split(';'))
                        {
                            val += "|" + vals[item.Trim()];
                        }
                    }
                    val = val.Substring(1);//remove first |
                }
                else
                    val = vals[control.ControlName];//get the value it wants

                control.UpdateControl(val);
            }
            //It should only get here if the key does not exist
            catch
            {
            }
        }

        void src_DataRecieved(object sender, EventArgs e)
        {
            //Don't Care at this point
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
        /// Processes any errors that may have been encountered, and either fires the OnError, or Alerts the user
        /// </summary>
        /// <param name="ex">The error</param>
        internal void DoError(Exception ex)
        {
            if (OnError == null)
                MessageBox.Show(ex.Message);
            else
                OnError(this, new ErrorEventArgs(ex));
        }

        /// <summary>
        /// What to do when an invalid packet is recieved
        /// </summary>
        public InvalidPacketActions InvalidPacketAction { get; set; }

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
            Environment.Exit(0);//Kill regardless of race (WPF or WinForms)
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

    /// <summary>
    /// What the DDH will load as sources when it loads
    /// </summary>
    public enum StartSources //TODO: Add TCP
    {
        /// <summary>
        /// Use the DB packet
        /// </summary>
        DashboardPacket,

        /// <summary>
        /// Manual configuration
        /// </summary>
        Manual
    }

    /// <summary>
    /// Actions to take when a packet is corroupted
    /// </summary>
    public enum InvalidPacketActions
    {
        /// <summary>
        /// Skip this packet
        /// </summary>
        Ignore = 1,
        /// <summary>
        /// Pretend the packet is fine
        /// </summary>
        Continue,
        /// <summary>
        /// Ignore, regardless of what the InvalidPacketRecievedEventArgs.ContinueAnyway is set to 
        /// </summary>
        AlwaysIgnore,
        /// <summary>
        /// Continue, regardless of what the InvalidPacketRecievedEventArgs.ContinueAnyway is set to 
        /// </summary>
        AlwaysContinue
    }

    /// <summary>
    /// EventArgs for the InvalidPacketRecieved event in DashboardDataHub
    /// </summary>
    public class InvalidPacketRecievedEventArgs : EventArgs
    {
        /// <summary>
        /// Create a new InvalidPacketRecievedEventArgs
        /// </summary>
        /// <param name="packetData">The invalid packet data</param>
        public InvalidPacketRecievedEventArgs(byte[] packetData)
        {
            PacketData = packetData;
        }

        /// <summary>
        /// Create a new InvalidPacketRecievedEventArgs
        /// </summary>
        /// <param name="packetData">The invalid packet data</param>
        /// <param name="continueAnyway">Continue anyway</param>
        public InvalidPacketRecievedEventArgs(byte[] packetData, bool continueAnyway)
        {
            PacketData = packetData;
            ContinueAnyway = continueAnyway;
        }

        /// <summary>
        /// The invalid packet
        /// </summary>
        public byte[] PacketData { get; private set; }

        /// <summary>
        /// Should we ignore this error?
        /// This can be overridden by the IgnorePacketAction field's AlwaysContinue and AlwaysIgnore
        /// </summary>
        public bool ContinueAnyway { get; set; }
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