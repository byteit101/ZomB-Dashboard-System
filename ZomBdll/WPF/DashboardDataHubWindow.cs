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
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace System451.Communication.Dashboard.WPF
{
    public class DashboardDataHubWindow : Window
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        static Mutex mutex;

        DashboardDataHub dashboardDataHub1 = new DashboardDataHub();

        public DashboardDataHubWindow()
        {
            //Check Singleton
            bool createdNew = true;
            mutex = new Mutex(true, "ZomBGLSingletonMutex", out createdNew);

            if ((!createdNew))
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

            this.Title = "ZomB Dashboard";
            //this.WindowStyle = WindowStyle.None;
            this.SizeToContent = SizeToContent.WidthAndHeight;
            //this.Width = 1024;
            //this.Height = 400;

            this.Loaded += delegate
            {
                if (!DesignerProperties.GetIsInDesignMode(this))
                    ReloadControls();
            };

            /*if (Environment.UserName == "Driver" || DesignMode)
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
            }*/
            dashboardDataHub1.StartSource = StartSources.DashboardPacket;
            dashboardDataHub1.InvalidPacketAction = InvalidPacketActions.Ignore;
            GC.KeepAlive(mutex);
            this.Closing += delegate
            {
                try
                {
                    GC.KeepAlive(mutex);
                    mutex.Close();
                }
                catch { }
            };
        }

        ~DashboardDataHubWindow()
        {
            try
            {
                mutex.Close();
            }
            catch { }
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
        //public bool AutoStart { get; set; }

        /// <summary>
        /// Re-iterate and find all controls
        /// </summary>
        public void ReloadControls()
        {
            AddControls(this);
            Start();
        }

        /// <summary>
        /// Start the DashboardDataHub
        /// </summary>
        public void Start()
        {
            if ((!DesignerProperties.GetIsInDesignMode(this)) && (!Running))
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
        public bool Running { get; private set; }

        /// <summary>
        /// Enable resizing when not in Driver mode
        /// </summary>
        //public bool EnableResize { get; set; }

        /// <summary>
        /// What the DDH will load as sources when it start()'s
        /// </summary>
        public StartSources DefaultSources
        {
            get
            {
                return dashboardDataHub1.StartSource;
            }
            set
            {
                dashboardDataHub1.StartSource = value;
            }
        }

        /// <summary>
        /// What to do when an invalid packet is recieved
        /// </summary>
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
            /*if (Environment.UserName == "Driver" || DesignMode)
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
            }*/
            ReloadControls();
            //if (AutoStart && (!DesignMode))
            //{
            //    Start();
            //}
        }

        private void AddControls(DependencyObject controlCollection)
        {
            foreach (var item in LogicalTreeHelper.GetChildren(controlCollection))
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
                foreach (var sub in LogicalTreeHelper.GetChildren((DependencyObject)item))
                {
                    AddControls((DependencyObject)item);
                    break;
                }
            }
        }
    }
}
