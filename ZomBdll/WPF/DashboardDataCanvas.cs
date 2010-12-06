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
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace System451.Communication.Dashboard.WPF.Controls
{
    public class DashboardDataCanvas : Canvas
    {
        public DashboardDataCanvas()
        {
            DashboardDataHub = new DashboardDataHub();
            this.Loaded += delegate
            {
                ReloadControls();
                if (AutoStart)
                    Start();
            };
        }

        public DashboardDataHub DashboardDataHub { get; set; }

        /// <summary>
        /// Start the DashboardDataHub when we load the form?
        /// </summary>
        public bool AutoStart { get; set; }

        /// <summary>
        /// Re-iterate and find all controls
        /// </summary>
        public void ReloadControls()
        {
            AddControls(this.Children);
        }

        /// <summary>
        /// Start the DashboardDataHub
        /// </summary>
        public void Start()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DashboardDataHub.Start();
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
            DashboardDataHub.Stop();
        }

        /// <summary>
        /// What the DDH will load as sources when it start()'s
        /// </summary>
        [Category("ZomB"), Description("What the DDH will load as sources when it start()'s")]
        public StartSources DefaultSources
        {
            get
            {
                return DashboardDataHub.StartSource;
            }
            set
            {
                DashboardDataHub.StartSource = value;
            }
        }

        /// <summary>
        /// What to do when an invalid packet is recieved
        /// </summary>
        [Category("ZomB"), Description("What the DDH will do when an invalid packet is recieved")]
        public InvalidPacketActions InvalidPacketAction
        {
            get
            {
                return DashboardDataHub.InvalidPacketAction;
            }
            set
            {
                DashboardDataHub.InvalidPacketAction = value;
            }
        }

        private void AddControls(IEnumerable controlCollection)
        {
            foreach (UIElement item in controlCollection)
            {
                if (item is IZomBControl)
                {
                    DashboardDataHub.Add((IZomBControl)item);
                }

                if (item is IZomBControlGroup)
                {
                    DashboardDataHub.Add((IZomBControlGroup)item);
                }
                if (item is IZomBMonitor)
                {
                    DashboardDataHub.Add((IZomBMonitor)item);
                }

                //If panel or has other controls, find those
                foreach (var e in LogicalTreeHelper.GetChildren(item))
                {
                    AddControls(LogicalTreeHelper.GetChildren(item));
                    break;
                }
            }
        }
    }
}
