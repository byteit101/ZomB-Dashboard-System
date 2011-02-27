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
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace OutStaller
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            if (!File.Exists(Zoutlocation.Text))
            {
                if (File.Exists(@"C:\Program Files (x86)\ZomB\Bindings\ZomB.out"))
                    Zoutlocation.Text = @"C:\Program Files (x86)\ZomB\Bindings\ZomB.out";
                else
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "ZomB.out|ZomB.out";
                    ofd.Title = "Please locate ZomB.out";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        Zoutlocation.Text = ofd.FileName;
                    }
                    else
                        throw new Exception();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IPAddress ia;
            if (!IPAddress.TryParse(ipAddressControl1.Text, out ia))
                return;
            button1.Text = "Installing... (May take a few minutes)";
            button1.Enabled = false;
            button1.Invalidate();
            int i = Program.DoInstall(Zoutlocation.Text, "ftp://" + ipAddressControl1.Text + "/");
            if (i == 0)
            {
                MessageBox.Show("Success! Please Reboot your cRIO.");
                Application.Exit();
            }
            else
            {
                MessageBox.Show("ERROR! Code: " + i);
                button1.Text = "Install!";
                button1.Enabled = true;
            }
        }

        private void ipAddressControl1_TextChanged(object sender, EventArgs e)
        {
            IPAddress ia;
            button1.Enabled = IPAddress.TryParse(ipAddressControl1.Text, out ia);
        }
    }
}
