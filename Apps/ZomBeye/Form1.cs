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
using System451.Communication.Dashboard;
using ZomBeye.Properties;

namespace ZomBeye
{
    public partial class Form1 : Form
    {
        BTFinger bf;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            numericUpDown1.Value = Settings.Default.TeamNumber;
            zomBeye1.LoadFolder(BTZomBFingerFactory.DefaultSaveLocation);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            numericUpDown1.Visible = true;
            numericUpDown1.Focus();
            numericUpDown1.Select(0, 4);
        }

        private void numericUpDown1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                e.SuppressKeyPress = true;
                numericUpDown1.Visible = false;
                try
                {
                    bf = (new BTZomBFingerFactory((int)numericUpDown1.Value, BTZomBFingerFactory.DefaultLoadLocation, BTZomBFingerFactory.DefaultSaveLocation)).GetFinger();
                    bf.Start();
                    button1.Enabled = false;
                    button1.Text = "BlueFinger Running";
                    Settings.Default.TeamNumber = numericUpDown1.Value;
                    Settings.Default.Save();
                }
                catch
                {
                    button1.Enabled = true;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
