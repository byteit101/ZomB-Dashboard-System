using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System451.Communication.Dashboard.Properties;
using System.Drawing;

namespace System451.Communication.Dashboard
{
    [ToolboxBitmap(typeof(Panel))]
    public class DashboardDataHubPanel: Panel
    {
        DashboardDataHub ddh;
        public DashboardDataHubPanel()
        {
            ddh = new DashboardDataHub();
            ddh.StartSource = StartSources.DashboardPacket;
            this.DoubleBuffered = true;
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            
            base.OnPaintBackground(e);
            if (DesignMode)
                e.Graphics.DrawImage(Resources.ZomBZ, 3, 3);
        }
        
        /// <summary>
        /// Gets the internal DashboardDataHub
        /// </summary>
        protected DashboardDataHub DashboardDataHub
        {
            get
            {
                return ddh;
            }
        }

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
                ddh.Start();
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
                ddh.Stop();
                Running = false;
            }
        }

        /// <summary>
        /// Are we running the dashboard task?
        /// </summary>
        public bool Running { get; private set; }

        /// <summary>
        /// What the DDH will load as sources when it start()'s
        /// </summary>
        [DefaultValue(typeof(StartSources), "DashboardPacket"), Category("ZomB"), Description("What the DDH will load as sources when it start()'s")]
        public StartSources DefaultSources
        {
            get
            {
                return ddh.StartSource;
            }
            set
            {
                ddh.StartSource = value;
            }
        }

        private void AddControls(Control.ControlCollection controlCollection)
        {
            foreach (Control item in controlCollection)
            {
                if (item is IZomBControl)
                {
                    ddh.Add((IZomBControl)item);
                }

                if (item is IZomBControlGroup)
                {
                    ddh.Add((IZomBControlGroup)item);
                }
                if (item is IZomBMonitor)
                {
                    ddh.Add((IZomBMonitor)item);
                }

                //If panel or has other controls, find those
                if (item.Controls.Count > 0)
                {
                    AddControls(item.Controls);
                }
            }
        }
    }
}
