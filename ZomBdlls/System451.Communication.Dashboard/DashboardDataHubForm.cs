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
        public bool AutoStart { get; set; }
        public void ReloadControls()
        {

            DashboardDataHubForm_Load(null, null);
        }
        public void Start()
        {
            if (!DesignMode)
            dashboardDataHub1.StartRecieving();
        }
        public void Restart()
        {
            Stop();
            Start();
        }
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
