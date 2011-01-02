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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System451.Communication.Dashboard.Properties;

namespace System451.Communication.Dashboard
{
    [ToolboxBitmap(typeof(Panel))]
    public class DashboardDataHubPanel : Panel
    {
        DashboardDataHub ddh;
        bool loaded = false;
        public DashboardDataHubPanel()
        {
            ddh = new DashboardDataHub();
            ddh.StartSources = "zomb://0.0.0.0/DBPkt";
            ddh.InvalidPacketAction = InvalidPacketActions.Ignore;
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
            loaded = true;
            AddControls(this.Controls);
        }

        /// <summary>
        /// Start the DashboardDataHub
        /// </summary>
        public void Start()
        {
            if ((!DesignMode) && (!Running))
            {
                if (!loaded)
                    ReloadControls();
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
        [Browsable(false)]
        public bool Running { get; private set; }

        /// <summary>
        /// What the DDH will load as sources when it start()'s
        /// </summary>
        [DefaultValue("zomb://0.0.0.0/DBPkt"), Category("ZomB"), Description("What the DDH will load as sources when it start()'s")]
        public Net.ZomBUrlCollection DefaultSources
        {
            get
            {
                return ddh.StartSources;
            }
            set
            {
                ddh.StartSources = value;
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
                return ddh.InvalidPacketAction;
            }
            set
            {
                ddh.InvalidPacketAction = value;
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
