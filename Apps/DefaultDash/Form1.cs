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
