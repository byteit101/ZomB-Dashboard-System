using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace System451.Communication.Dashboard
{
    public partial class DashboardDataHubForm : Form
    {
        public DashboardDataHubForm()
        {
            AutoStart = !DesignMode;
            InitializeComponent();
            if (Environment.UserName == "Driver")
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.StartPosition = FormStartPosition.Manual;
                this.ControlBox = false;
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
            if (Environment.UserName == "Driver")
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.StartPosition = FormStartPosition.Manual;
                this.ControlBox = false;
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
        
    }
}
