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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using System451.Communication.Dashboard.Net;
using System451.Communication.Dashboard.Utils;

namespace System451.Communication.Dashboard
{
    /// <summary>
    /// DataSource reads CSV data at 50hz
    /// </summary>
    [DataSource("CsvSource", "Generates data from a csv at 50hz. Usage: zomb://.0/CsvSource/C:\\Path.to.file")]
    public class CsvSource : IDashboardDataSource, IDashboardDataDataSource, IDashboardPeekableDataSource
    {
        StringCollection header = new StringCollection();
        Collection<StringCollection> dataq = new Collection<StringCollection>();
        int qindx = 0;
        bool saving = false;
        StreamReader ins;
        uint lasttime, savetime;
        DateTime starttime;
        string fpath;
        Thread backThread;
        ZomBDataLookup kys = new ZomBDataLookup();

        /// <summary>
        /// Creates a new CsvSource
        /// </summary>
        /// <param name="file">A file to save to</param>
        public CsvSource(string file)
        {
            fpath = file;
        }

        internal CsvSource(ZomBUrl url)
        {
            var urlPath = url.Path.Substring(1);//remove first char
            if (urlPath == "")
                throw new ArgumentNullException("Path required!");
            else if (!urlPath.Contains(":\\"))//not absoloute
                fpath = BTZomBFingerFactory.DefaultSaveLocation + "\\" + urlPath;
            else
                fpath = urlPath;
        }

        ~CsvSource()
        {
            Stop();
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


        private void DoWork()
        {
            while (saving)
            {
                //create a new object so we don't clobber the old data
                kys = new ZomBDataLookup();

                var str = ins.ReadLine();
                if (str == null)
                {
                    ins.BaseStream.Seek(0, SeekOrigin.Begin);
                    str = ins.ReadLine();
                    str = ins.ReadLine();
                    if (str == null)
                    {
                        saving = false;
                        return;
                    }
                }
                var strs = str.Split(',');
                for (int i = 0; i < Math.Min(header.Count, strs.Length); i++)
                {
                    kys.Add(header[i], new ZomBDataObject(strs[i], ZomBDataTypeHint.Unknown));
                }
                Thread.Sleep(20);
                if (peeking)
                {
                    foreach (var item in kys)
                    {
                        dp.Invoke(cb, item.Key);
                    }
                }

                //Fire events
                if (DataRecieved != null)
                    DataRecieved(this, new EventArgs());
                if (NewDataRecieved != null)
                    NewDataRecieved(this, new NewDataRecievedEventArgs(kys));
            }
        }
        #region new

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
                ins = new StreamReader(FilePath);
                qindx = 0;
                var str = ins.ReadLine();
                var strs = str.Split(',');
                header.Clear();
                foreach (var item in strs)
                {
                    header.Add(item);
                    qindx++;
                }
                try
                {
                    backThread = new Thread(new ThreadStart(this.DoWork));
                    backThread.IsBackground = true;
                    saving = true;
                    backThread.Start();
                }
                catch (Exception ex)
                {
                    //DoError(ex);
                }
            }
        }

        /// <summary>
        /// Stop Saving
        /// </summary>
        public void Stop()
        {
            if (Running)
            {
                try
                {
                    saving = false;
                    Thread.Sleep(500);
                    if (backThread.IsAlive)
                        backThread.Abort();
                    ins.Close();
                }
                catch
                {
                    try
                    {
                        ins.Close();
                    }
                    catch { }
                }
            }
        }

        public bool HasStatus
        {
            get { return false; }
        }

        public bool HasData
        {
            get { return true; }
        }

        public IDashboardStatusDataSource GetStatusSource()
        {
            return null;
        }

        public IDashboardDataDataSource GetDataSource()
        {
            return this;
        }

        public event EventHandler DataRecieved;
#pragma warning disable 67
        public event InvalidPacketRecievedEventHandler InvalidPacketRecieved;

        public event ErrorEventHandler OnError;
#pragma warning restore 67

        #endregion

        #region IDashboardDataDataSource Members

        public IDashboardDataSource ParentDataSource
        {
            get { return this; }
        }

        public ZomBDataLookup GetData()
        {
            return kys;
        }

        public event NewDataRecievedEventHandler NewDataRecieved;

        #endregion

        /// <summary>
        /// Where to read the file
        /// </summary>
        [Category("ZomB"), Description("Where to save the files")]
        public string FilePath
        {
            get { return fpath; }
            set { fpath = value; }
        }


        /// <summary>
        /// Are we running?
        /// </summary>
        public bool IsRunning
        {
            get { return Running; }
        }

        /// <summary>
        /// Magic method for zomb:// urls
        /// </summary>
        /// <returns></returns>
        private static ZomBUrlInfo GetZomBUrlInfo()
        {
            return new ZomBUrlInfo { DefaultPort = 1 };
        }

        #region IDashboardPeekableDataSource Members

        bool peeking = false;
        Utils.StringFunction cb;
        Dispatcher dp;

        public bool BeginNamePeek(Utils.StringFunction callback)
        {
            peeking = true;
            cb = callback;
            dp = Dispatcher.CurrentDispatcher;
            Start();
            return true;
        }

        public void EndNamePeek()
        {
            Stop();
            peeking = false;
        }

        #endregion
    }
}
