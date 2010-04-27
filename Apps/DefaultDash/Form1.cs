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
using System.Windows.Forms;
using DefaultDash.Properties;
using System.Collections.ObjectModel;
using System451.Communication.Dashboard;
using System.IO;

namespace DefaultDash
{
    public partial class Form1 : DashboardDataHubForm
    {
        //Create a new Video Saving Object
        VideoStreamSaver vss;
        public Form1()
        {
            //Init stuff
            InitializeComponent();
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox2.Image = System451.Communication.Dashboard.Properties.Resources.ZomB_text;
            //Get ready to save images
            Directory.CreateDirectory(BTZomBFingerFactory.DefaultSaveLocation);
            vss = new VideoStreamSaver(this.cameraView1);
            vss.FPS = 15;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Help button
            pictureBox1.Visible = !pictureBox1.Visible;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Restart Camera
            cameraView1.Restart();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            //Help button
            pictureBox1.Visible = false;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            //Change IP, then reset
            cameraView1.IPAddress = "10." + numericUpDown1.Value.ToString().PadLeft(4, '0').Substring(0, 2) + "." + numericUpDown1.Value.ToString().PadLeft(4, '0').Substring(2) + ".2";
            cameraView1.Restart();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Finish saving
            vss.EndSave();
        }

        private void startBothToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Start Saving
            vss.StartSave(BTZomBFingerFactory.DefaultSaveLocation + "\\Capture" + (DateTime.Now.Ticks.ToString("x")) + ".avi");
        }
    }
}
