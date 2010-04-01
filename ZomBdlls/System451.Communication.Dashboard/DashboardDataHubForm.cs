using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

namespace System451.Communication.Dashboard
{
    public partial class DashboardDataHubForm : Form
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        public DashboardDataHubForm()
        {
            //Check Singleton
            bool createdNew = true;
            using (Mutex mutex = new Mutex(true, "ZomBSingletonMutex", out createdNew))
            {
                if (!createdNew)
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
                    Application.Exit();
                }
            }
            AutoStart = !DesignMode;
            InitializeComponent();
            if (Environment.UserName == "Driver" || DesignMode)
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.StartPosition = FormStartPosition.Manual;
                this.ControlBox = false;
                this.Size = DefaultSize;
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.StartPosition = FormStartPosition.WindowsDefaultLocation;
                this.ControlBox = true;
            }
        }
        protected override Size DefaultSize
        {
            get
            {
                return new Size(1024, 400);
            }
        }
        [DefaultValue(typeof(Size),"1024, 400")]
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
                this.StartPosition = FormStartPosition.Manual;
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
                if (item.Controls.Count>0)
                {
                    AddControls(item.Controls);
                }
            }
        }

        private void DashboardDataHubForm_SizeChanged(object sender, EventArgs e)
        {
            if (this.Size!=DefaultSize&&DesignMode)
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