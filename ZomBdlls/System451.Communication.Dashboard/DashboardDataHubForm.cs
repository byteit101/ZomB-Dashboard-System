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
                            break;
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
                this.Size = DefaultSize;
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.StartPosition = FormStartPosition.WindowsDefaultLocation;
                this.ControlBox = true;
            }
            this.ResumeLayout(false);
            GC.KeepAlive(mutex);
        }
        ~DashboardDataHubForm()
        {
            GC.KeepAlive(mutex);
            mutex.Close();
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

            DashboardDataHubForm_Load(null, null);
        }
        /// <summary>
        /// Start the DashboardDataHub
        /// </summary>
        public void Start()
        {
            if (!DesignMode)
                dashboardDataHub1.StartRecieving();
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
            dashboardDataHub1.StopRecieving();
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
                this.Size = DefaultSize;
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.StartPosition = FormStartPosition.WindowsDefaultLocation;
                this.ControlBox = true;
            }
            dashboardDataHub1.GetControls().Clear();
            AddControls(this.Controls);
            if (AutoStart && (!DesignMode))
            {
                dashboardDataHub1.StartRecieving();
            }
        }

        private void AddControls(Control.ControlCollection controlCollection)
        {
            foreach (Control item in controlCollection)
            {
                if (item is IDashboardControl)
                {
                    dashboardDataHub1.AddDashboardControl((IDashboardControl)item);
                }
                if (item.Controls.Count > 0)
                {
                    AddControls(item.Controls);
                }
            }
        }

        private void DashboardDataHubForm_SizeChanged(object sender, EventArgs e)
        {
            if (this.Size != DefaultSize && DesignMode)
                this.Size = DefaultSize;
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