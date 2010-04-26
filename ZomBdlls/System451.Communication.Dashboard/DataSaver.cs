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
