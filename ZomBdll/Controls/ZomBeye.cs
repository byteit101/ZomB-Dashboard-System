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
using System451.Communication.Dashboard.Utils;
using Vlc.DotNet.Forms;
using System.Linq;
using Vlc.DotNet.Core.Medias;
using Vlc.DotNet.Core;
using System.Windows.Threading;
using Microsoft.Win32;

namespace System451.Communication.Dashboard.Controls
{
    [ToolboxBitmap(typeof(icofinds), "System451.Communication.Dashboard.TBB.ZomBeye.png")]
    public partial class ZomBeye : UserControl
    {
        string folder = BTZomBFingerFactory.DefaultLoadLocation;
        string[] files;
        int index;
        static ZomBeye()
        {
            AutoExtractor.Extract(AutoExtractor.Files.VLC);
        }
        public ZomBeye()
        {
            InitializeComponent();
            vlcBox.Manager = new VlcManager();
            this.vlcBox.Manager.PluginsPath = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\VideoLAN\VLC").GetValue("InstallDir", @"C:\Program Files\VideoLan\VLC\").ToString();
            this.vlcBox.EndReached += new Vlc.DotNet.Core.VlcEventHandler<EventArgs>(vlcBox_EndReached);
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
        }

        private void ReloadAll()
        {
            try
            {
                Playing = false;
                files = Directory.GetFiles(folder, "*.webm");
                index = 0;
                Reload();
            }
            catch { }
        }

        private void Reload()
        {
            var mm = new FileMedia();
            mm.Path = files[index];
            vlcBox.Play(mm);
            Playing = true;
        }

        void vlcBox_EndReached(object sender, VlcEventArgs<EventArgs> e)
        {
            setTimeout(delegate
            {
                Next();
            }, 2);
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
            var mm = new FileMedia();
            mm.Path = files[index];
            vlcBox.Play(mm);
            Playing = true;
        }

        public void PlayPause()
        {
            vlcBox.Pause();
        }

        public void Pause()
        {
            Playing = false;
            vlcBox.Pause();
            Playing = false;
        }
        public bool Playing { get; private set; }

        private void button1_Click(object sender, EventArgs e)
        {
            PlayPause();
        }

        void setTimeout(VoidFunction eh, int ms)
        {
            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = new TimeSpan(0, 0, 0, 0, ms);
            dt.Tick += delegate
            {
                dt.Stop();
                eh();
            };
            dt.Start();
        }
    }
}
