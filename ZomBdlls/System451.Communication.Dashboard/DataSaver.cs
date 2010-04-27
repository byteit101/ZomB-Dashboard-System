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
                if (value != null)
                {
                    buffer.Enqueue(value);
                    if (buffer.Count >= 30)
                        WriteBuffer();
                }
            }
        }

        private void WriteBuffer()
        {
            while (buffer.Count>1)
            {
                outs.Write(buffer.Dequeue());
                outs.Write((byte)' ');
                outs.Write((byte)' ');
                outs.Write((byte)' ');
                outs.Write((byte)'&');
                outs.Write((byte)' ');
            }
        }

        public void Start()
        {
            outs = new BinaryWriter(File.Open(FilePath, FileMode.Append));
            saving = true;
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
                outs.Flush();
                outs.Close();
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
            
        }

        public void UpdateData(Dictionary<string, string> data, byte[] packetData)
        {
            AddValue(packetData);
        }

        #endregion
    }
}
