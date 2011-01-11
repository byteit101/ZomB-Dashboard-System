/*
 * ZomB Dashboard System <http://firstforge.wpi.edu/sf/projects/zombdashboard>
 * Copyright (C) 2011, Patrick Plenefisch and FIRST Robotics Team 451 "The Cat Attack"
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
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System451.Communication.Dashboard.Utils;

namespace System451.Communication.Dashboard.Net.DriverStation
{
    [WPF.Design.ZomBControl("Driver Station", Description = "A small driver station for driving the robot without runnning the ds software", IconName = "DSIcon")]
    [TemplatePart(Name = "PART_endis", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Status", Type = typeof(Label))]
    [WPF.Design.ZomBDesignableProperty("Width", Dynamic = true, Category = "Layout")]
    [WPF.Design.ZomBDesignableProperty("Height", Dynamic = true, Category = "Layout")]
    [WPF.Design.ZomBDesignableProperty("Background")]
    [WPF.Design.ZomBDesignableProperty("BorderBrush")]
    [WPF.Design.ZomBDesignableProperty("BorderThickness")]
    public class DriverStation : Control, IZomBControl
    {
        Button PART_endis;
        Label statuslbl;
        DashboardDataHub ddh;
        UdpClient uc;
        bool running;
        System.Timers.Timer tmr;
        short loops;
        public const int MaxConnectionTimeout = 10;
        int connected = MaxConnectionTimeout + 1;


        public DriverStation()
        {
            this.Width = 150;
            this.Height = 100;
            this.Background = Brushes.LightGray;
            Enabled = false;
            Joystick1 = new Joystick();
            Joystick2 = new Joystick();
            Joystick3 = new Joystick();
            Joystick4 = new Joystick();
            KeySafety.Start(Enable, Disable, Disable);
        }

        ~DriverStation()
        {
            try
            {
                Disable();
                Stop();
            }
            catch { }
            try
            {
                uc.Close();
                uc = null;
            }
            catch { }
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
            statuslbl = base.GetTemplateChild("PART_Status") as Label;
            statuslbl.Content = "Not Connected";
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
            Enabled = false;
            try
            {
                PART_endis.Background = Brushes.Green;
                PART_endis.Content = "Enable";
            }
            catch { }
        }

        private void Start()
        {
            running = true;
            uc = new UdpClient(1150, AddressFamily.InterNetwork);
            tmr = new System.Timers.Timer(20);
            tmr.AutoReset = true;
            tmr.Elapsed += new ElapsedEventHandler(hz);
            tmr.Start();
            dolisten();
        }

        private void dolisten()
        {
            uc.BeginReceive(delegate(IAsyncResult ar)
            {
                try
                {
                    IPEndPoint iep = null;
                    byte[] dgram = uc.EndReceive(ar, ref iep);
                    connected = 0;
                    stat(dgram[1].ToString("x") + "." + dgram[2].ToString("x"));
                    dolisten();
                }
                catch { }
            }, this);
        }

        private void stat(string stat)
        {
            statuslbl.Dispatcher.Invoke(new VoidFunction(() => statuslbl.Content = stat), null);
        }

        void hz(object sender, ElapsedEventArgs e)
        {
            try
            {
                int start = ((int)(Team / 100.0)) * 100;

                uc.Send(GetStatus(), 1024, "10." + (start / 100) + "." + (Team - start) + ".2", 1110);

                if (connected > MaxConnectionTimeout)
                {
                    stat("Not Connected");
                }
                else
                    connected++;
            }
            catch
            {
                if (tmr != null)
                    tmr.Stop();
                try
                {
                    uc.Close();
                }
                catch { }
                //throw;
            }
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
            //Joystick data
            try
            {
                Dispatcher.Invoke(new BytesFunction(sendJoystics), r);
            }
            catch (ThreadAbortException) { throw; }
            catch { }


            //version
            r[72] = 0x31;
            r[73] = 0x30;
            r[74] = 0x30;
            r[75] = 0x32;
            r[76] = 0x30;
            r[77] = 0x38;
            r[78] = 0x30;
            r[79] = 0x30;

            //CRC
            uint cr = Libs.Crc32.Compute(r);
            r[1020] = (byte)(cr >> 24);
            r[1021] = (byte)(cr >> 16);
            r[1022] = (byte)(cr >> 8);
            r[1023] = (byte)(cr);
            return r;
        }


        private void sendJoystics(byte[] r)
        {
            Joystick1.SaveDataTo(r, 8);
            Joystick2.SaveDataTo(r, 16);
            Joystick3.SaveDataTo(r, 24);
            Joystick4.SaveDataTo(r, 32);
        }

        private void Stop()
        {
            if (tmr != null)
                tmr.Stop();
            Disable();
            hz(this, null);
            running = false;
        }

        static void joychanged(object o, DependencyPropertyChangedEventArgs e)
        {
            ((Joystick)(o as DriverStation).GetValue(e.Property)).SetFindName((n) => (o as DriverStation).FindName(n));
        }

        [WPF.Design.ZomBDesignable(), Category("IO")]
        public Joystick Joystick1
        {
            get { return (Joystick)GetValue(Joystick1Property); }
            set { SetValue(Joystick1Property, value); }
        }

        // Using a DependencyProperty as the backing store for Joystick1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Joystick1Property =
            DependencyProperty.Register("Joystick1", typeof(Joystick), typeof(DriverStation), new UIPropertyMetadata(null, joychanged));

        [WPF.Design.ZomBDesignable(), Category("IO")]
        public Joystick Joystick2
        {
            get { return (Joystick)GetValue(Joystick2Property); }
            set { SetValue(Joystick2Property, value); }
        }

        // Using a DependencyProperty as the backing store for Joystick1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Joystick2Property =
            DependencyProperty.Register("Joystick2", typeof(Joystick), typeof(DriverStation), new UIPropertyMetadata(null, joychanged));

        [WPF.Design.ZomBDesignable(), Category("IO")]
        public Joystick Joystick3
        {
            get { return (Joystick)GetValue(Joystick3Property); }
            set { SetValue(Joystick3Property, value); }
        }

        // Using a DependencyProperty as the backing store for Joystick1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Joystick3Property =
            DependencyProperty.Register("Joystick3", typeof(Joystick), typeof(DriverStation), new UIPropertyMetadata(null, joychanged));

        [WPF.Design.ZomBDesignable(), Category("IO")]
        public Joystick Joystick4
        {
            get { return (Joystick)GetValue(Joystick4Property); }
            set { SetValue(Joystick4Property, value); }
        }

        // Using a DependencyProperty as the backing store for Joystick1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Joystick4Property =
            DependencyProperty.Register("Joystick4", typeof(Joystick), typeof(DriverStation), new UIPropertyMetadata(null, joychanged));

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
     * Values in bytes
Offset - Size - Description - extra info

First, is the Driver Station -> cRio packet

0-2-Packet Index
2-1-Control Byte (more about this one later)
3-1-Driver Station Digital Input
4-2-Team ID
6-1-Driver Station Alliance
7-1-Driver Station Position
8-(1x6)-Joystick 0, 6 bytes, each is one axis (signed byte, -128 to 127)
14-2-Joystick 0 Buttons
16-(1x6)-Joystick 1, 6 bytes, same as Joystick 0
22-2-Joystick 1 Buttons
24-(1x6)-Joystick 2, 6 bytes, same as Joystick 1
30-2-Joystick 2 Buttons
32-(1x6)-Joystick 3, 6 bytes, same as Joystick 2
38-2-Joystick 3 Buttons

40-2-Analog 0-Only uses 10 of the 16 bits, so max value of 1024
42-2-Analog 1-Same as 0
44-2-Analog 2-Same as 0
46-2-Analog 3-Same as 0

48-8-cRio checksum
56-4-FPGA Checksum 1
60-4-FPGA Checksum 2
64-4-FPGA Checksum 3
68-4-FPGA Checksum 4

72-8-Version Data

80-938-High end data, this is where current dashboard sends/recieves data

1020-4-Packet CRC (More on this one later)


Now for cRio -> Driver Station (more simple)

0-1-Control Byte
1-2-Battery Voltage, slightly weird, explained later
3-1-Robot Digital Output
4-4-Unknown/Not Needed
8-2-Team Number
10-6-Robot Mac
16-8-Robot Version
24-6-Unknown
30-2-Counter
32-988-High End Data
1020-4-CRC checksum


Now, for further explanation

Control Byte
the bits of the control byte specify robot state, such as enabled/disabled, auton/teleop, etc
0-FPGA Checksum
1-cRio Checksum
2-Resynch
3-FMS Attached
4-Auton
5-Enabled
6-Not E-Stopped
7-Reset
If you would like help on how to set bits of a number, i can help you with that as well

Packet CRC
make sure that you are generating all 1024 bytes, and that 1020-1024 are empty when you generate this.

Battery voltage
this value was weird, but the value is sent over in the following fashion
if the first byte has a hex value of 12 and the second has a hex value of 45, then the battery is 12.45 volts, so if you were gonna set a label, you would do label.Text = Convert.toString(firstByte,16) + "." + Convert.toString(secondByte,16) + " volts"


     * 
     * 
     **************************************/
}
