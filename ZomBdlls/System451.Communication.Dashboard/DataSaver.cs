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
        string paramName = "DataSave1";
        Collection<string> buffer = new Collection<string>();
        bool saving = false;
        StreamWriter outs;
        delegate void UpdaterDelegate(string value);

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

        [DefaultValue("DataSave1"), Category("ZomB"), Description("What this control will get the value of from the packet Data")]
        public string BindToInput
        {
            get { return paramName; }
            set { paramName = value; }
        }

        private void AddValue(string value)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdaterDelegate(AddValue),value);
            }
            else
            {
                if (value != null && value != "")
                {
                    buffer.Add(value);
                    if (buffer.Count >= 30)
                        WriteBuffer();
                }
            }
        }

        private void WriteBuffer()
        {
            foreach (string value in buffer)
            {
                outs.Write("\""+value + "\",");
            }
            buffer.Clear();
        }

        public void Start()
        {
            outs = new StreamWriter(FilePath);
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
#warning Implement this
            //TODO: IMplement this
        }

        #endregion
    }
}
