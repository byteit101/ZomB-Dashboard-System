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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace System451.Communication.Dashboard
{
    public partial class DashboardDataHubForm : Form
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        static Mutex mutex;

        public DashboardDataHubForm()
        {
            //Check Singleton
            bool createdNew = true;
            mutex = new Mutex(true, "ZomBSingletonMutex", out createdNew);

            if ((!createdNew) && (!DesignMode))
            {
                Process current = Process.GetCurrentProcess();
                //Don't kill designer
                if ((!current.MainModule.FileName.Contains("Microsoft Visual Studio")) && (!current.MainModule.FileName.Contains("devenv")) && (!current.MainModule.FileName.Contains("MonoDevelop")) && (!current.MainModule.FileName.Contains("VCSExpress.exe")))
                {
                    foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                    {
                        if (process.Id != current.Id)
                        {
                            SetForegroundWindow(process.MainWindowHandle);
                            current.Kill();
                            return;
                        }
                    }
                }
            }

            AutoStart = !DesignMode;
            InitializeComponent();
            this.SuspendLayout();
            if (Environment.UserName == "Driver" || DesignMode)
            {
                this.FormBorderStyle = FormBorderStyle.None;
                if (!DesignMode)
                {
                    this.StartPosition = FormStartPosition.Manual;
                }
                else
                {
                    this.StartPosition = FormStartPosition.WindowsDefaultLocation;
                }
                this.ControlBox = false;
                this.ClientSize = DefaultSize;
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                this.StartPosition = FormStartPosition.WindowsDefaultLocation;
                this.ControlBox = true;
            }
            dashboardDataHub1.InvalidPacketAction = InvalidPacketActions.Ignore;
            this.ResumeLayout(false);
            GC.KeepAlive(mutex);
            this.FormClosing += new FormClosingEventHandler(DashboardDataHubForm_FormClosing);
        }

        void DashboardDataHubForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                GC.KeepAlive(mutex);
                mutex.Close();
            }
            catch { }
        }

        ~DashboardDataHubForm()
        {
            try
            {
                mutex.Close();
            }
            catch { }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(1024, 400);
            }
        }

        [DefaultValue(typeof(Size), "1024, 400")]
        public new Size Size
        {
            get { return base.Size; }
            set { base.Size = value; }
        }

        /// <summary>
        /// Gets the internal DashboardDataHub
        /// </summary>
        protected DashboardDataHub DashboardDataHub
        {
            get
            {
                return dashboardDataHub1;
            }
        }

        /// <summary>
        /// Start the DashboardDataHub when we load the form?
        /// </summary>
        public bool AutoStart { get; set; }

        /// <summary>
        /// Re-iterate and find all controls
        /// </summary>
        public void ReloadControls()
        {
            AddControls(this.Controls);
        }

        /// <summary>
        /// Start the DashboardDataHub
        /// </summary>
        public void Start()
        {
            if ((!DesignMode) && (!Running))
            {
                dashboardDataHub1.Start();
                Running = true;
            }
        }

        /// <summary>
        /// Restart the DashboardDataHub
        /// </summary>
        public void Restart()
        {
            Stop();
            Start();
        }

        /// <summary>
        /// Stop the DashboardDataHub
        /// </summary>
        public void Stop()
        {
            if (Running)
            {
                dashboardDataHub1.Stop();
                Running = false;
            }
        }

        /// <summary>
        /// Are we running the dashboard task?
        /// </summary>
        [Browsable(false)]
        public bool Running { get; private set; }

        /// <summary>
        /// Enable resizing when not in Driver mode
        /// </summary>
        [DefaultValue(false), Category("ZomB"), Description("Enable resizing when not in driver mode")]
        public bool EnableResize { get; set; }

        /// <summary>
        /// What the DDH will load as sources when it start()'s
        /// </summary>
        [DefaultValue("zomb://0.0.0.0/DBPkt"), Category("ZomB"), Description("What the DDH will load as sources when it start()'s")]
        public Net.ZomBUrlCollection DefaultSources
        {
            get
            {
                return dashboardDataHub1.StartSources;
            }
            set
            {
                dashboardDataHub1.StartSources = value;
            }
        }

        /// <summary>
        /// What to do when an invalid packet is recieved
        /// </summary>
        [DefaultValue(typeof(InvalidPacketActions), "Ignore"), Category("ZomB"), Description("What the DDH will do when an invalid packet is recieved")]
        public InvalidPacketActions InvalidPacketAction
        {
            get
            {
                return dashboardDataHub1.InvalidPacketAction;
            }
            set
            {
                dashboardDataHub1.InvalidPacketAction = value;
            }
        }

        private void DashboardDataHubForm_Load(object sender, EventArgs e)
        {
            if (Environment.UserName == "Driver" || DesignMode)
            {
                this.FormBorderStyle = FormBorderStyle.None;
                if (!DesignMode)
                {
                    this.StartPosition = FormStartPosition.Manual;
                }
                else
                {
                    this.StartPosition = FormStartPosition.WindowsDefaultLocation;
                }
                this.ControlBox = false;
                this.ClientSize = DefaultSize;
            }
            else
            {
                if (EnableResize)
                    this.FormBorderStyle = FormBorderStyle.Sizable;
                else
                    this.FormBorderStyle = FormBorderStyle.FixedSingle;
                this.StartPosition = FormStartPosition.WindowsDefaultLocation;
                this.ControlBox = true;
            }
            ReloadControls();
            if (AutoStart && (!DesignMode))
            {
                Start();
            }
        }

        private void AddControls(Control.ControlCollection controlCollection)
        {
            foreach (Control item in controlCollection)
            {
                if (item is IZomBControl)
                {
                    dashboardDataHub1.Add((IZomBControl)item);
                }

                if (item is IZomBControlGroup)
                {
                    dashboardDataHub1.Add((IZomBControlGroup)item);
                }
                if (item is IZomBMonitor)
                {
                    dashboardDataHub1.Add((IZomBMonitor)item);
                }

                //If panel or has other controls, find those
                if (item.Controls.Count > 0)
                {
                    AddControls(item.Controls);
                }
            }
        }

        private void DashboardDataHubForm_SizeChanged(object sender, EventArgs e)
        {
            if (this.ClientSize != DefaultSize && DesignMode && (!EnableResize))
                this.ClientSize = DefaultSize;
        }

    }
}
/* using mutex lock style from http://www.iridescence.no/post/CreatingaSingleInstanceApplicationinC.aspx
[DllImport("user32.dll")]
[return: MarshalAs(UnmanagedType.Bool)]
static extern bool SetForegroundWindow(IntPtr hWnd);
/// <summary>
/// The main entry point for the application.
/// </summary>
[STAThread]
static void Main()
{
bool createdNew = true;
using (Mutex mutex = new Mutex(true, "MyApplicationName", out createdNew))
{
if (createdNew)
{
Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);
Application.Run(new MainForm());
}
else
        {
Process current = Process.GetCurrentProcess();
foreach (Process process in Process.GetProcessesByName(current.ProcessName))
{
if (process.Id != current.Id)
{
SetForegroundWindow(process.MainWindowHandle);
break;
}
}
}
}
}
*/