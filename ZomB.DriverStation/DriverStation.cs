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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Net.Sockets;
using System.Timers;
using System451.Communication.Dashboard.Utils;

namespace System451.Communication.Dashboard.Net.DriverStation
{
    [WPF.Design.ZomBControl("Driver Station", Description = "A small driver station for driving the robot without runnning the ds software")]
    [TemplatePart(Name = "PART_endis", Type = typeof(Button))]
    [WPF.Design.ZomBDesignableProperty("Width", Dynamic = true, Category = "Layout")]
    [WPF.Design.ZomBDesignableProperty("Height", Dynamic = true, Category = "Layout")]
    public class DriverStation : Control, IZomBControl
    {
        Button PART_endis;
        DashboardDataHub ddh;
        UdpClient uc;
        bool running;
        Timer tmr;
        short loops;

        public DriverStation()
        {
            this.Width = 100;
            this.Height = 50;
            this.Background = Brushes.LightGray;
            Enabled = false;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_endis = base.GetTemplateChild("PART_endis") as Button;
            PART_endis.Click += delegate
            {
                if (Enabled)
                    Disable();
                else
                    Enable();
            };
            Disable();
        }

        public bool Enabled { get; private set; }

        public int Team
        {
            get
            {
                return ddh.Team;
            }
        }

        public void Enable()
        {
            PART_endis.Background = Brushes.Red;
            PART_endis.Content = "Disable";
            Enabled = true;
            if (!running)
                Start();
        }

        public void Disable()
        {
            PART_endis.Background = Brushes.Green;
            PART_endis.Content = "Enable";
            Enabled = false;
        }

        private void Start()
        {
            running = true;
            uc = new UdpClient(1150, AddressFamily.InterNetwork);
            tmr = new Timer(20);
            tmr.AutoReset = true;
            tmr.Elapsed += new ElapsedEventHandler(hz);
            tmr.Start();
        }

        void hz(object sender, ElapsedEventArgs e)
        {
            int start = ((int)(Team/100.0))*100;

            uc.Send(GetStatus(), 1024, "10." + (start / 100) + "." + (Team - start) + ".2", 1110);
        }

        private byte[] GetStatus()
        {
            byte[] r = new byte[1024];
            r[0] = (byte)(loops >> 8);
            r[1] = (byte)(loops);
            loops++;
            var status = new BitField();
            /*
             * 0-FPGA Checksum
             * 1-cRio Checksum
             * 2-Resynch
             * 3-FMS Attached
             * 4-Auton
             * 5-Enabled
             * 6-Not E-Stopped
             * 7-Reset
             */
            status[5] = Enabled;
            status[6] = true;
            r[2] = status.Byte;
            r[3] = 0xff;//digitalInput
            r[4] = (byte)(Team >> 8);
            r[5] = (byte)(Team);
            //alliance, R/B, ascii 1,2,3
            r[6] = 0x52;
            r[7] = 0x31;

            //version
            r[72] = 0x31;
            r[73] = 0x30;
            r[74] = 0x30;
            r[75] = 0x32;
            r[76] = 0x30;
            r[77] = 0x38;
            r[78] = 0x30;
            r[79] = 0x30;
            uint cr = Libs.Crc32.Compute(r);
            r[1020] = (byte)(cr >> 24);
            r[1021] = (byte)(cr >> 16);
            r[1022] = (byte)(cr >> 8);
            r[1023] = (byte)(cr);
            return r;
        }

        private void Stop()
        {
            tmr.Stop();
            Disable();
            hz(this, null);
            running = false;
        }

        #region IZomBControl Members

        bool IZomBControl.IsMultiWatch { get { return false; } }

        string IZomBControl.ControlName { get { return null; } }

        void IZomBControl.UpdateControl(string value) { }

        void IZomBControl.ControlAdded(object sender, ZomBControlAddedEventArgs e)
        {
            ddh = e.Controller.GetDashboardDataHub();
            //we only want the controller, now remove ourselves
            ddh.Remove(this);
        }

        #endregion
    }
    /**************************************
     **** Driver Station Protocol 2010 ****
     **************************************
     * The UDP server needs to be set up on
     * port 1150 and set to recieve from 
     * only the robot's IP. The UDP client
     * should send to 1110 on the robot at
     * 50 hertz.
     * 
     * 
     * 
     * 
     * 
     **************************************/
}
