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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using AviFile;

namespace System451.Communication.Dashboard
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
