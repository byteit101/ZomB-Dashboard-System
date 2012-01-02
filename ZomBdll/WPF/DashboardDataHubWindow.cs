/*
 * ZomB Dashboard System <http://firstforge.wpi.edu/sf/projects/zombdashboard>
 * Copyright (C) 2012, Patrick Plenefisch and FIRST Robotics Team 451 "The Cat Attack"
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
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace System451.Communication.Dashboard.WPF.Controls
{
    public class DashboardDataHubWindow : Window, IZomBDashboardDataHubConsumer
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        static Mutex mutex;

        DashboardDataHub dashboardDataHub1;

        public DashboardDataHubWindow()
        {
            Init(true);
        }

        protected DashboardDataHubWindow(bool mutexonly)
        {
            Init(!mutexonly);
        }

        protected void Init(bool newddh)
        {
            //Check Singleton
            bool createdNew = true;
            if ((int)Registry.LocalMachine.OpenSubKey(@"Software\ZomB").GetValue("Singleton", 0) == 1)
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

            if (newddh)
            {
                this.dashboardDataHub1 = new DashboardDataHub();
                dashboardDataHub1.StartSources = "zomb://0.0.0.0/DBPkt";//default, will always work
                dashboardDataHub1.InvalidPacketAction = InvalidPacketActions.Ignore;
            }

            this.Title = "ZomB Dashboard";
            this.Icon = BitmapFrame.Create(new Uri("pack://application:,,,/ZomB;component/Resources/ZomB.ico", UriKind.RelativeOrAbsolute));
            this.SizeToContent = SizeToContent.WidthAndHeight;

            this.Loaded += delegate
            {
                if ((!newddh) && Content is IZomBDashboardDataHubConsumer)
                    dashboardDataHub1 = (Content as IZomBDashboardDataHubConsumer).DashboardDataHub;
                else
                {
                    try
                    {
                        this.InvalidPacketAction = (InvalidPacketActions)(Content as DependencyObject).GetValue(InvalidPacketActionProperty);
                        if (!DesignerProperties.GetIsInDesignMode(this) && ZDesigner.IsRunMode)
                            ReloadControls();
                    }
                    catch { }
                }
            };

            bool driveroveride = false;
            try
            {
                driveroveride = (int)Registry.LocalMachine.OpenSubKey(@"Software\ZomB").GetValue("DriverDisable", 0) == 1;
            }
            catch { }

            if (Environment.UserName == "Driver" && !driveroveride)
            {
                this.WindowStyle = WindowStyle.None;
                this.WindowStartupLocation = WindowStartupLocation.Manual;
                this.Left = 0;
                this.Top = 0;
            }
            GC.KeepAlive(mutex);
            this.Closing += delegate
            {
                try
                {
                    GC.KeepAlive(mutex);
                    mutex.Close();
                }
                catch { }
                try
                {
                    dashboardDataHub1.Stop();
                    GC.Collect();
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
            try
            {
                dashboardDataHub1.Stop();
            }
            catch { }
        }

        /// <summary>
        /// Gets the internal DashboardDataHub
        /// </summary>
        public DashboardDataHub DashboardDataHub
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
            AddControls(this);
            if (AutoStart)
                Start();
        }

        /// <summary>
        /// Start the DashboardDataHub
        /// </summary>
        public void Start()
        {
            if ((!DesignerProperties.GetIsInDesignMode(this)) && (!Running) && dashboardDataHub1 != null)
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

        public static InvalidPacketActions GetInvalidPacketAction(DependencyObject obj)
        {
            return (InvalidPacketActions)obj.GetValue(InvalidPacketActionProperty);
        }

        public static void SetInvalidPacketAction(DependencyObject obj, InvalidPacketActions value)
        {
            obj.SetValue(InvalidPacketActionProperty, value);
        }

        // Using a DependencyProperty as the backing store for InvalidPacketAction.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InvalidPacketActionProperty =
            DependencyProperty.RegisterAttached("InvalidPacketAction", typeof(InvalidPacketActions), typeof(DashboardDataHubWindow), new UIPropertyMetadata(InvalidPacketActionChanged));

        //Ultimate hack!
        private static void InvalidPacketActionChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            dpo.SetValue(InvalidPacketActionProperty, e.NewValue);
        }

        private void AddControls(DependencyObject controlCollection)
        {
            foreach (var item in LogicalTreeHelper.GetChildren(controlCollection))
            {
                if (item is IZomBControl)
                {
                    dashboardDataHub1.Add((IZomBControl)item);
                }

                if (item is IZomBMonitor)
                {
                    dashboardDataHub1.Add((IZomBMonitor)item);
                }

                if (item is IZomBControlGroup)
                {
                    dashboardDataHub1.Add((IZomBControlGroup)item);
                    continue; //assume that the group manages all children
                }

                //If panel or has other controls, find those
                try
                {
                    foreach (var sub in LogicalTreeHelper.GetChildren((DependencyObject)item))
                    {
                        AddControls((DependencyObject)item);
                        break;
                    }
                }
                catch { /*No children or not a DepObj*/ }
            }
        }

        public void StopAll()
        {
            try
            {
                StopControls(this);
            }
            catch { }
        }

        private void StopControls(DependencyObject controlCollection)
        {
            foreach (var item in LogicalTreeHelper.GetChildren(controlCollection))
            {
                //If panel or has other controls, find those
                try
                {
                    foreach (var sub in LogicalTreeHelper.GetChildren((DependencyObject)item))
                    {
                        StopControls((DependencyObject)item);
                        break;
                    }
                }
                catch { /*No children or not a DepObj*/ }

                try
                {
                    (item as IDisposable).Dispose();
                }
                catch { }
                try
                {
                    (item).GetType().GetMethod("Close").Invoke(item, null);
                }
                catch { }
            }
        }
    }

    public interface IZomBDashboardDataHubConsumer
    {
        DashboardDataHub DashboardDataHub { get; }
    }
}
