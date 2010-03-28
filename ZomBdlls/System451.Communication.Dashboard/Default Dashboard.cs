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
            dashboardDataHub1.AddDashboardControl(distanceMeter1);
            dashboardDataHub1.AddDashboardControl(messageDisp1);
            dashboardDataHub1.AddDashboardControl(roundSpeedMeter1);
            dashboardDataHub1.AddDashboardControl(roundSpeedMeter10);
            dashboardDataHub1.AddDashboardControl(roundSpeedMeter2);
            dashboardDataHub1.AddDashboardControl(roundSpeedMeter3);
            dashboardDataHub1.AddDashboardControl(roundSpeedMeter4);
            dashboardDataHub1.AddDashboardControl(roundSpeedMeter5);
            dashboardDataHub1.AddDashboardControl(roundSpeedMeter6);
            dashboardDataHub1.AddDashboardControl(roundSpeedMeter7);
            dashboardDataHub1.AddDashboardControl(roundSpeedMeter8);
            dashboardDataHub1.AddDashboardControl(roundSpeedMeter9);
            dashboardDataHub1.AddDashboardControl(onOffControl1);
            dashboardDataHub1.AddDashboardControl(onOffControl10);
            dashboardDataHub1.AddDashboardControl(onOffControl11);
            dashboardDataHub1.AddDashboardControl(onOffControl12);
            dashboardDataHub1.AddDashboardControl(onOffControl13);
            dashboardDataHub1.AddDashboardControl(onOffControl14);
            dashboardDataHub1.AddDashboardControl(onOffControl2);
            dashboardDataHub1.AddDashboardControl(onOffControl3);
            dashboardDataHub1.AddDashboardControl(onOffControl4);
            dashboardDataHub1.AddDashboardControl(onOffControl5);
            dashboardDataHub1.AddDashboardControl(onOffControl6);
            dashboardDataHub1.AddDashboardControl(onOffControl8);
            dashboardDataHub1.AddDashboardControl(onOffControl9);
            dashboardDataHub1.AddDashboardControl(onOffControl7);
            dashboardDataHub1.AddDashboardControl(analogMeter1);
            dashboardDataHub1.AddDashboardControl(analogMeter2);
            dashboardDataHub1.AddDashboardControl(analogMeter3);
            dashboardDataHub1.AddDashboardControl(analogMeter4);
            dashboardDataHub1.AddDashboardControl(analogMeter5);
            dashboardDataHub1.AddDashboardControl(analogMeter6);
            dashboardDataHub1.AddDashboardControl(analogMeter7);
            dashboardDataHub1.AddDashboardControl(analogMeter8);

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            dashboardDataHub1.StartRecieving();
            button1.Enabled = false;
            button1.Text = "Recieving Dashboard Data on Port 1165...";
        }
    }
}
