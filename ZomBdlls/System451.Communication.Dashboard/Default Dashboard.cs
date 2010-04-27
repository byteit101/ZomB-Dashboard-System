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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace System451.Communication.Dashboard
{
    public partial class Default_Dashboard : Form
    {
        public Default_Dashboard()
        {
            InitializeComponent();
            dashboardDataHub1.Add(distanceMeter1);
            dashboardDataHub1.Add(messageDisp1);
            dashboardDataHub1.Add(roundSpeedMeter1);
            dashboardDataHub1.Add(roundSpeedMeter10);
            dashboardDataHub1.Add(roundSpeedMeter2);
            dashboardDataHub1.Add(roundSpeedMeter3);
            dashboardDataHub1.Add(roundSpeedMeter4);
            dashboardDataHub1.Add(roundSpeedMeter5);
            dashboardDataHub1.Add(roundSpeedMeter6);
            dashboardDataHub1.Add(roundSpeedMeter7);
            dashboardDataHub1.Add(roundSpeedMeter8);
            dashboardDataHub1.Add(roundSpeedMeter9);
            dashboardDataHub1.Add(onOffControl1);
            dashboardDataHub1.Add(onOffControl10);
            dashboardDataHub1.Add(onOffControl11);
            dashboardDataHub1.Add(onOffControl12);
            dashboardDataHub1.Add(onOffControl13);
            dashboardDataHub1.Add(onOffControl14);
            dashboardDataHub1.Add(onOffControl2);
            dashboardDataHub1.Add(onOffControl3);
            dashboardDataHub1.Add(onOffControl4);
            dashboardDataHub1.Add(onOffControl5);
            dashboardDataHub1.Add(onOffControl6);
            dashboardDataHub1.Add(onOffControl8);
            dashboardDataHub1.Add(onOffControl9);
            dashboardDataHub1.Add(onOffControl7);
            dashboardDataHub1.Add(analogMeter1);
            dashboardDataHub1.Add(analogMeter2);
            dashboardDataHub1.Add(analogMeter3);
            dashboardDataHub1.Add(analogMeter4);
            dashboardDataHub1.Add(analogMeter5);
            dashboardDataHub1.Add(analogMeter6);
            dashboardDataHub1.Add(analogMeter7);
            dashboardDataHub1.Add(analogMeter8);

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            dashboardDataHub1.Start();
            button1.Enabled = false;
            button1.Text = "Recieving Dashboard Data on Port 1165...";
        }
    }
}
