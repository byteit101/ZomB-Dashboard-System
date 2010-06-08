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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System451.Communication.Dashboard.Libs.AviFile;

namespace System451.Communication.Dashboard.Controls
{
    [ToolboxBitmap(typeof(icofinds), "System451.Communication.Dashboard.TBB.ZomBeye.png")]
    public partial class ZomBeye : UserControl
    {
        string folder = BTZomBFingerFactory.DefaultLoadLocation;
        AviPlayer playa = null;
        string[] files;
        int index;
        public ZomBeye()
        {
            InitializeComponent();
            if (!DesignMode)
            ReloadAll();
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(640, 500);
            }
        }
        public void LoadFolder(string folder)
        {
            this.folder = folder;
            ReloadAll();
            Play();
        }

        private void ReloadAll()
        {
            try
            {
                playa = null;
                Playing = false;
                files = Directory.GetFiles(folder, "*.avi");
                index = 0;
                Reload();
            }
            catch
            {
                //MessageBox.Show("Path Not found");
            }
        }

        private void Reload()
        {
            //playa.Stop();
            playa = new AviPlayer(new AviManager(files[index], true).GetVideoStream(), pictureBox1, null);
            playa.Stopped += new EventHandler(playa_Stopped);
        }

        void playa_Stopped(object sender, EventArgs e)
        {
            if (Playing)
            {
                Next();
                Play();
                return;
            }
            Playing = playa.IsRunning;

        }

        public void Next()
        {
            index++;
            if (index >= files.Length)
                index = 0;
            Reload();
        }

        public void Previous()
        {
            index--;
            if (index < 0)
                index = files.Length - 1;
            Reload();
        }

        public void Play()
        {
            if (playa != null)
            {
                playa.Start();
                Playing = true;
            }
        }

        public void PlayPause()
        {
            if (!Playing)
                Play();
            else
                Pause();
        }

        public void Pause()
        {
            if (playa != null)
            {
                Playing = false;
                playa.Stop();
                Playing = false;
            }
        }
        public bool Playing { get; private set; }

        private void button1_Click(object sender, EventArgs e)
        {
            PlayPause();
        }
    }
}
