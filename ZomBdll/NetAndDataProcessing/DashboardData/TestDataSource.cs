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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.IO.Ports;
using System.Web;
using System.Linq;

namespace System451.Communication.Dashboard.Net
{
    [DataSource("Test", "Creates Test data")]
    public class TestDataSource : IDashboardDataSource, IDashboardDataDataSource, IDashboardPeekableDataSource
    {
        Thread backThread;
        bool isrunning = false;
        ZomBDataLookup kys = new ZomBDataLookup();

        public TestDataSource(ZomBUrl info)
        {

        }

        ~TestDataSource()
        {
            Stop();
        }

        #region IDashboardDataSource Members

        public void Start()
        {
            if (!isrunning)
            {
                try
                {
                    backThread = new Thread(new ThreadStart(this.DoWork));
                    backThread.IsBackground = true;
                    isrunning = true;
                    backThread.Start();
                }
                catch
                {
                }
            }
        }

        public void Stop()
        {
            if (isrunning)
            {
                try
                {
                    isrunning = false;
                    Thread.Sleep(500);
                    if (backThread.IsAlive)
                        backThread.Abort();
                }
                catch
                {
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
        /// The background worker. Will exit after 10 consectutive errors
        /// </summary>
        private void DoWork()
        {
            int i = 0;
            Random rand = new Random();
            kys["group.~TYPE~"] = "PIDController";
            while (isrunning)
            {
                kys["float"] = new ZomBDataObject(Math.Sin((i++) / 100.0), ZomBDataTypeHint.Double);
                kys["int"] = new ZomBDataObject(Math.Sin((i++) / 100.0) + Math.Sin((i) / 5.0)*.1, ZomBDataTypeHint.Integer);
                kys["bool"] = new ZomBDataObject(Math.Sin((i++) / 100.0) > 0, ZomBDataTypeHint.Boolean);
                kys["str"] = new ZomBDataObject(Math.Round(Math.Sin((i++) / 100.0), 3).ToString(), ZomBDataTypeHint.String);
                kys["floatrand"] = new ZomBDataObject(rand.NextDouble(), ZomBDataTypeHint.Double);
                kys["group.Data.p"] = new ZomBDataObject((Math.Sin((i) / 100.0)+1)*2.0, ZomBDataTypeHint.Double);
                kys["group.Data.i"] = new ZomBDataObject((Math.Sin((i+50) / 100.0)+1)*2.0, ZomBDataTypeHint.Double);
                kys["group.Data.d"] = new ZomBDataObject((Math.Sin((i-50) / 100.0)+1)*2.0, ZomBDataTypeHint.Double);
                Thread.Sleep(10);
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


        /// <summary>
        /// Are we running?
        /// </summary>
        public bool IsRunning
        {
            get { return isrunning; }
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
