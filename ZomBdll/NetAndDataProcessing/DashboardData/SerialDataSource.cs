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
    [DataSource("Serial", "Serial ZomB source for the old IFI systems. Query string: baud, parity, data, stop")]
    public class SerialDataSource : IDashboardDataSource, IDashboardDataDataSource, IDashboardPeekableDataSource
    {
        SerialPort killer;
        bool isrunning;
        Thread backThread;
        DashboardDataHub ddh;

        FRCDSStatus cStat = new FRCDSStatus();
        ZomBDataLookup kys = new ZomBDataLookup();

        public SerialDataSource(ZomBUrl info)
        {
            int baud = 115200;
            int data = 8;
            Parity parity = Parity.None;
            StopBits stopbits = StopBits.One;
            if (info.Path.Length > 5 && info.Path[0] == '?')
            {
                var nv = HttpUtility.ParseQueryString(info.Path);
                if (nv["baud"] != null)
                {
                    if (!int.TryParse(nv["baud"], out baud))
                        baud = 115200;
                }
                if (nv["data"] != null)
                {
                    if (!int.TryParse(nv["data"], out data))
                        data = 8;
                }
                if (nv["parity"] != null)
                {
                    try
                    {
                        parity = (Parity)Enum.Parse(typeof(Parity), nv["parity"], true);
                    }
                    catch
                    {
                        parity = Parity.None;
                    }
                }
                if (nv["stop"] != null)
                {
                    try
                    {
                        stopbits = (StopBits)Enum.Parse(typeof(StopBits), nv["stop"], true);
                    }
                    catch
                    {
                        stopbits = StopBits.One;
                    }
                }
            }
            killer = new SerialPort("COM" + info.Port.ToString(), baud, parity, data, stopbits);
            killer.NewLine = "\n";
        }

        ~SerialDataSource()
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
                catch (Exception ex)
                {
                    DoError(ex);
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
                if (killer.IsOpen)
                    killer.Close();
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

        public event InvalidPacketRecievedEventHandler InvalidPacketRecieved;

        public event ErrorEventHandler OnError;

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
            //number of errors
            int nume = 0;
            killer.Open();
            while (isrunning)
            {
                try
                {
                    string line = killer.ReadLine();
                    if (line[0] != ' ' && line[0] != ';' && line[0] != '/' && line[0] != '#')
                    {
                        //Get the items in a dictionary
                        try
                        {
                            ZomBDataLookup vals = SplitParams(line);
                            if (peeking)
                            {
                                foreach (var keys in vals)
                                {
                                    dp.Invoke(cb, keys.Key);
                                }
                            }
                            else
                            {
                                kys = vals;

                                //Fire events
                                if (DataRecieved != null)
                                    DataRecieved(this, new EventArgs());
                                if (NewDataRecieved != null)
                                    NewDataRecieved(this, new NewDataRecievedEventArgs(vals));
                            }
                        }
                        catch { }
                    }
                    if (nume > 0)
                        nume--;
                }
                catch (ThreadAbortException)
                {
                    isrunning = false;
                    killer.Close();
                    return;
                }
                catch (Exception ex)
                {
                    nume++;
                    DoError(ex);
                    if (nume > 10)
                    {
                        isrunning = false;
                        killer.Close();
                        DoError(new Exception("10 consecutive errors were encountered, stopping DashboardDataHub"));
                        isrunning = false;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Convert the name=value|n=v form to a Dictionary of name and values
        /// </summary>
        /// <param name="Output"></param>
        /// <returns></returns>
        private ZomBDataLookup SplitParams(string Output)
        {
            //Split the main string
            string[] s = Output.Split(':');
            ZomBDataLookup k = new ZomBDataLookup(1);
            string ky, val;
            ky = s[0];
            val = s[1].Split('\r','\n','\b','#')[0];
            k[ky] = new ZomBDataObject(val, ZomBDataTypeHint.Unknown);
            return k;
        }

        /// <summary>
        /// Processes any errors that may have been encountered, and either fires the OnError, or Alerts the user
        /// </summary>
        /// <param name="ex">The error</param>
        internal void DoError(Exception ex)
        {
            if (OnError == null)
                ddh.DoError(ex);
            else
                OnError(this, new ErrorEventArgs(ex));
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
