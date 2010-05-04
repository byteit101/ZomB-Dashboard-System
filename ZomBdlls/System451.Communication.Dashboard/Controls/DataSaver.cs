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
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;

namespace System451.Communication.Dashboard
{
    class DataSaver:Control, IZomBMonitor
    {
        Queue<byte[]> buffer = new Queue<byte[]>();
        bool saving = false;
        BinaryWriter outs;
        delegate void UpdaterDelegate(byte[] value);
        DateTime  lasttime;

        public DataSaver()
        {
        }
        ~DataSaver()
        {
            Stop();
            WriteOff();
        }

        public bool Running
        {
            get
            {
                return saving;
            }
        }


        private void AddValue(byte[] value)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdaterDelegate(AddValue),value);
            }
            else
            {
                if (saving)
                {
                    if (value != null)
                    {
                        buffer.Enqueue(GetTime());
                        buffer.Enqueue(value);

                        if (buffer.Count >= 30)
                            WriteBuffer();
                    }
                }
            }
        }

        private byte[] GetTime()
        {
            byte[] bits = new byte[3];
            bits[0] = 84;
            DateTime dif = DateTime.Now.Subtract(lasttime);
            lasttime = DateTime.Now;
            ushort msd = (ushort)(dif.Minute * 60000);
            msd += (ushort)(dif.Second * 1000);
            msd += (ushort)dif.Millisecond;
            bits[1] = (byte)(msd >> 8);
            bits[2] = (byte)msd;
            return bits;
        }

        private void WriteBuffer()
        {
            while (buffer.Count>1)
            {
                outs.Write(buffer.Dequeue());
            }
        }

        public void Start()
        {
            outs = new BinaryWriter(File.Open(FilePath, FileMode.Append));
            saving = true;
            lasttime = DateTime.Now;
        }
        public void Stop()
        {
            saving = false;
            WriteBuffer();
            WriteOff();
        }

        private void WriteOff()
        {
            if (outs != null)
            {
                outs.Write((byte)81);//Q
                outs.Flush();
                outs.Close();
                outs = null;
            }
        }

        private string fpath;
        [Category("ZomB"),Description("Where to save the files")]
        public string FilePath
        {
            get { return fpath; }
            set { fpath = value; }
        }



        #region IZomBMonitor Members

        public void UpdateStatus(FRCDSStatus status)
        {
            if (saving)
            {
                byte[] bit = new byte[9];
                bit[0] = 83;//"S"
                bit[1] = status.Status.Byte;
                bit[2] = status.Error.Byte;
                bit[3] = status.DigitalIn.Byte;
                bit[4] = status.DigitalOut.Byte;
                bit[5] = (byte)(status.PacketNumber >> 8);
                bit[6] = (byte)status.PacketNumber;
                bit[7] = (byte)((int)status.Battery);
                bit[8] = (byte)((status.Battery - ((int)status.Battery)) * 100);
                AddValue(bit);
            }
        }

        public void UpdateData(Dictionary<string, string> data)
        {
#warning Make this work
            if (saving)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Dln");
                foreach (var item in data)
                {
                    sb.Append(item.Key);
                    sb.Append("=");
                    sb.Append(item.Value);
                    sb.Append("|");
                }
                byte[] bit = UTF8Encoding.UTF8.GetBytes(sb.ToString());
                bit[0] = 68;//"D"
                bit[1] = (byte)(sb.Length >> 8);
                bit[2] = (byte)(sb.Length);
                AddValue(bit);
            }
            //TODO: Fix this
            //AddValue(packetData);
        }

        #endregion
    }
    //Format: T(ushort)[timespan]S[data]D(ushort)[length][data]
}
