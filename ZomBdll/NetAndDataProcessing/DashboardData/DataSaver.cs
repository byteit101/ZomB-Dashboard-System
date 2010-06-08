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
using System.IO;
using System.Text;

namespace System451.Communication.Dashboard
{
    /// <summary>
    /// DataSaver saves data from the DDH to a file that can be read by DataPlayerSource
    /// </summary>
    public class DataSaver : IZomBMonitor
    {
        Queue<byte[]> buffer = new Queue<byte[]>();
        bool saving = false;
        BinaryWriter outs;
        DateTime lasttime;
        string fpath;

        /// <summary>
        /// Creates a new DataSaver
        /// </summary>
        /// <param name="file">A file to save to</param>
        public DataSaver(string file)
        {
            fpath = file;
        }

        ~DataSaver()
        {
            Stop();
            WriteOff();
        }

        /// <summary>
        /// Are we saving
        /// </summary>
        public bool Running
        {
            get
            {
                return saving;
            }
        }


        private void AddValue(byte[] value)
        {
            if (saving)
            {
                if (value != null)
                {
                    lock (buffer)
                    {
                        buffer.Enqueue(GetTime());
                        buffer.Enqueue(value);
                    }
                    if (buffer.Count >= 30)
                        WriteBuffer();
                }
            }
        }

        private byte[] GetTime()
        {
            byte[] bits = new byte[3];
            bits[0] = 84;//T
            TimeSpan dif = DateTime.Now.Subtract(lasttime);
            lasttime = DateTime.Now;
            ushort msd = (ushort)(dif.TotalMinutes * 60000);//ms
            bits[1] = (byte)(msd >> 8);
            bits[2] = (byte)msd;
            return bits;
        }

        private void WriteBuffer()
        {
            lock (buffer)
            {
                lock (outs)
                {
                    while (buffer.Count > 1)
                    {
                        outs.Write(buffer.Dequeue());
                    }
                }
            }
        }

        /// <summary>
        /// Start saving
        /// </summary>
        public void Start()
        {
            if (!Running)
            {
                outs = new BinaryWriter(File.Open(FilePath, FileMode.Append));
                saving = true;
                lasttime = DateTime.Now;
            }
        }

        /// <summary>
        /// Stop Saving
        /// </summary>
        public void Stop()
        {
            if (Running)
            {
                saving = false;
                WriteBuffer();
                WriteOff();
            }
        }

        private void WriteOff()
        {
            if (outs != null)
            {
                lock (outs)
                {
                    outs.Write((byte)81);//Q
                    outs.Flush();
                    outs.Close();
                    outs = null;
                }
            }
        }

        /// <summary>
        /// Where to save the file
        /// </summary>
        [Category("ZomB"), Description("Where to save the files")]
        public string FilePath
        {
            get { return fpath; }
            set { fpath = value; }
        }

        #region IZomBMonitor Members

        /// <summary>
        /// IZomBMonitor: Updates the status
        /// </summary>
        /// <param name="status">The new status</param>
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

        /// <summary>
        /// IZomBMonitor: Updates the data
        /// </summary>
        /// <param name="data">The new data</param>
        public void UpdateData(Dictionary<string, string> data)
        {
            //TODO: Test
            if (saving)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("D00");//hog space for the size
                foreach (var item in data)
                {
                    sb.Append(item.Key);
                    sb.Append("=");
                    sb.Append(item.Value);
                    sb.Append("|");
                }
                byte[] bit = UTF8Encoding.UTF8.GetBytes(sb.ToString());
                bit[1] = (byte)(sb.Length >> 8);
                bit[2] = (byte)(sb.Length);
                AddValue(bit);
            }
            //TODO: Fix this
        }

        #endregion
    }
    //Format: T(ushort)[timespan]S[data]D(ushort)[length][data]
}
