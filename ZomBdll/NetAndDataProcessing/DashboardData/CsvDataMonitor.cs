/*
 * ZomB Dashboard System <http://firstforge.wpi.edu/sf/projects/zombdashboard>
 * Copyright (C) 2012, Patrick Plenefisch and FIRST Robotics Team 451 "The Cat Attack"
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
using System451.Communication.Dashboard.Net;
using System451.Communication.Dashboard.Utils;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace System451.Communication.Dashboard
{
    /// <summary>
    /// DataSaver saves data from the DDH to a file that can be read by DataPlayerSource
    /// </summary>
    [DataSource("CsvSaver", "Saves all data as a csv. Usage: zomb://.0/CsvSaver/[C:\\Path.to.file]")]
    public class CsvSaver : IZomBMonitor
    {
        StringCollection header = new StringCollection();
        Collection<StringCollection> dataq = new Collection<StringCollection>();
        int qindx = 0;
        bool saving = false;
        StreamWriter outs;
        uint lasttime, savetime;
        DateTime starttime;
        string fpath;

        /// <summary>
        /// Creates a new DataSaver
        /// </summary>
        /// <param name="file">A file to save to</param>
        public CsvSaver(string file)
        {
            fpath = file;
        }

        internal CsvSaver(ZomBUrl url)
        {
            var urlPath = url.Path.Substring(1);//remove first char
            if (urlPath == "")
                fpath = BTZomBFingerFactory.DefaultSaveLocation + "\\ZCapture" + (DateTime.Now.Ticks.ToString("x")) + ".csv";
            else if (!urlPath.Contains(":\\"))//not absoloute
                fpath = BTZomBFingerFactory.DefaultSaveLocation + "\\" + urlPath;
            else
                fpath = urlPath;
        }

        ~CsvSaver()
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


        private void AddLookupData(string prefix, ZomBDataLookup datal)
        {
            if (saving)
            {
                lock (dataq)
                {
                    var scl = new StringCollection();
                    foreach (var item in datal)
                    {
                        if (item.Value.Value is ZomBDataLookup)
                        {
                            AddLookupData(prefix + item.Key + ".", item.Value.Value as ZomBDataLookup);
                            continue;
                        }
                        if (!header.Contains(prefix + item.Key))
                        {
                            header.Add(prefix + item.Key);
                        }
                        int hidx = header.IndexOf(prefix + item.Key);
                        while (scl.Count <= hidx)
                        {
                            scl.Add(null);
                        }
                        scl[hidx] = item.Value.ToString();
                    }
                    if (dataq.Count > 0 && GetTime() - lasttime < 50)//less than 50ms
                    {
                        var mmine = new string[scl.Count];
                        scl.CopyTo(mmine, 0);
                        if (Mergable(ref scl))
                        {
                            dataq[dataq.Count - 1] = scl;
                        }
                        else
                        {
                            scl = new StringCollection();
                            scl.AddRange(mmine);
                            dataq.Add(scl);
                        }
                    }
                    else
                    {
                        dataq.Add(scl);
                        lasttime = GetTime();
                    }
                }
            }
        }

        private bool Mergable(ref StringCollection scl)
        {
            var dq = dataq[dataq.Count - 1];
            while (scl.Count < dq.Count)
            {
                scl.Add(dq[scl.Count]);
            }
            for (int i = 0; i < dq.Count; i++)
            {
                if (scl[i] == null && dq[i] != null)
                {
                    scl[i] = dq[i];
                }
                if (scl[i] != dq[i] && dq[i] != null)
                    return false;
            }
            return true;
        }

        private uint GetTime()
        {
            TimeSpan dif = DateTime.Now.Subtract(starttime);
            return (uint)(dif.TotalMinutes * 60000);//ms
        }

        private void WriteBuffer()
        {
            lock (dataq)
            {
                try
                {
                    outs = new StreamWriter(FilePath, true);
                    for (; qindx < header.Count; qindx++)
                    {
                        outs.Write(header[qindx] + ",");
                    }
                    System.Diagnostics.Debug.WriteLine(qindx);
                    for (int i = 0; i < dataq.Count - 1; i++)
                    {
                        outs.WriteLine();
                        foreach (var item in dataq[i])
                        {
                            outs.Write((item ?? "") + ",");
                        }
                    }

                    //save last for next merge
                    var last = dataq[dataq.Count - 1];
                    dataq.Clear();
                    dataq.Add(last);
                    savetime = GetTime();
                    outs.Flush();
                    outs.Close();
                    outs = null;
                }
                catch { }
            }
        }

        /// <summary>
        /// Start saving
        /// </summary>
        public void Start()
        {
            if (!Running)
            {
                saving = true;
                starttime = DateTime.Now;
                lasttime = savetime = 0;
                qindx = 0;
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
                qindx = 0;
            }
        }

        private void WriteOff()
        {
            if (outs != null)
            {
                lock (outs)
                {
                    try
                    {
                        outs.Flush();
                        outs.Close();
                        outs = null;
                    }
                    catch { }
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
            //ignore status
        }

        /// <summary>
        /// IZomBMonitor: Updates the data
        /// </summary>
        /// <param name="data">The new data</param>
        public void UpdateData(ZomBDataLookup data)
        {
            if (!saving)
            {
                Start();
            }
            try
            {
                AddLookupData("", data);
                if (dataq.Count > 100 || lasttime - savetime > 10000)
                    WriteBuffer();
            }
            catch { }

        }

        #endregion

        /// <summary>
        /// Magic method for zomb:// urls
        /// </summary>
        /// <returns></returns>
        private static ZomBUrlInfo GetZomBUrlInfo()
        {
            return new ZomBUrlInfo { DefaultPort = 0 };
        }
    }
}
