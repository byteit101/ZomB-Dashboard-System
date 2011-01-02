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
using System.Diagnostics.CodeAnalysis;

namespace System451.Communication.Dashboard.Net
{
    [DataSource("TCP")]
    public class TCPDataSource : IDashboardDataSource, IDashboardDataDataSource
    {
        public const int DefaultPort = 9066;//"ZB"

        TcpClient cRIOConnection;
        bool isrunning;
        Thread backThread;

        Dictionary<string, string> kys = new Dictionary<string, string>();

        public TCPDataSource(int team)
        {
            IPAddress = IPAddress.Parse("10." + ((int)(team / 100)) + "." + ((int)(team % 100)) + ".2");
            Port = DefaultPort;
        }
        public TCPDataSource(int team, int port)
        {
            IPAddress = IPAddress.Parse("10." + ((int)(team / 100)) + "." + ((int)(team % 100)) + ".2");
            Port = port;
        }
        public TCPDataSource(IPAddress addr)
        {
            IPAddress = addr;
            Port = DefaultPort;
        }
        public TCPDataSource(IPAddress addr, int port)
        {
            IPAddress = addr;
            Port = port;
        }
        public TCPDataSource(ZomBUrl url)
        {
            IPAddress = url.IPAddress;
            Port = url.Port;
        }
        ~TCPDataSource()
        {
            Stop();
        }

        public int Port { get; private set; }
        private int teamNumber;
        [Obsolete]
        public int TeamNumber
        {
            get
            {
                return teamNumber;
            }
            set
            {
                teamNumber = value;
                IPAddress = IPAddress.Parse("10." + ((int)(teamNumber / 100)) + "." + ((int)(teamNumber % 100)) + ".2");
            }
        }
        public IPAddress IPAddress
        {
            get;
            set;
        }

        #region IDashboardDataSource Members

        public void Start()
        {
            if (!isrunning)
            {
                if (cRIOConnection == null)
                {
                    try
                    {
                        cRIOConnection = new TcpClient();
                    }
                    catch (Exception ex)
                    {
                        DoError(ex);
                    }
                }
                try
                {
                    backThread = new Thread(new ThreadStart(this.TickleClientWorker));
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
                try
                {
                    cRIOConnection.Close();
                    cRIOConnection = null;
                }
                catch { }
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

        event InvalidPacketRecievedEventHandler IDashboardDataSource.InvalidPacketRecieved
        {
            add { }
            remove { }
        }

        public event ErrorEventHandler OnError;

        #endregion


        #region IDashboardDataDataSource Members

        public Dictionary<string, string> GetData()
        {
            return kys;
        }

        public IDashboardDataSource ParentDataSource
        {
            get { return this; }
        }

        public event NewDataRecievedEventHandler NewDataRecieved;

        #endregion

        /// <summary>
        /// The background worker. Will exit after 10 consectutive errors
        /// </summary>
        private void TickleClientWorker()
        {
            //TCP==tickle
            //Person[] ticklishPeople = People.GetTicklish();
            //foreach (Person poorSoul in ticklishPeople)
            //{
            //    poorSoul.Apply(Objects.Get("Feather"), Person.Location.Foot);
            //}

            //number of errors
            int nume = 0;
            //config
            bool implicitSend = false;
            bool longNames = false;
            NetworkStream zb = null;
            while (isrunning)
            {
                try
                {
                    cRIOConnection.Connect(IPAddress, Port);
                    zb = cRIOConnection.GetStream();
                    break;
                }
                catch (SocketException)
                {
                    //Nohbdy is around
                }
                catch (ThreadAbortException)
                {
                    isrunning = false;
                    return;
                }
                catch (Exception ex)
                {
                    nume++;
                    DoError(ex);
                    if (nume > 10)
                    {
                        isrunning = false;
                        DoError(new Exception("10 consecutive errors were encountered, stopping DashboardDataHub"));
                        isrunning = false;
                        return;
                    }
                }
            }
            try
            {
                //config
                int last;
                do
                {
                    last = zb.ReadByte();
                    switch (last)
                    {

                        case 0x45:
                            implicitSend = false;
                            break;
                        case 0x49:
                            implicitSend = true;
                            break;
                        case 0x4C:
                            longNames = true;
                            break;
                        case -1:
                            return;
                        default:
                            break;
                    }
                }
                while (last != 0x00);
                //end config
                int namel, valuel;
                while (isrunning)
                {
                    try
                    {
                        last = zb.ReadByte();
                        //Fire! on 0
                        if (last == 0x00 || implicitSend)
                        {
                            if (kys.Count > 0)
                            {
                                //fire events
                                if (DataRecieved != null)
                                    DataRecieved(this, new EventArgs());
                                if (NewDataRecieved != null)
                                    NewDataRecieved(this, new NewDataRecievedEventArgs(kys));
                                kys = new Dictionary<string, string>();
                            }
                            if (last == 0x00)
                                continue;
                        }
                        if (last == -1)
                            return;//stream closed

                        //last byte was name lenght
                        namel = last;

                        //Read the value
                        if (longNames)
                        {
                            valuel = (zb.ReadByte() << 8) + zb.ReadByte();//TODO: test
                        }
                        else
                        {
                            valuel = zb.ReadByte();
                        }
                        if (valuel < 0)
                            return;//stream closed

                        //Make the buffers
                        byte[] buf = new byte[namel], vbuf = new byte[valuel];

                        //Read the name
                        while (cRIOConnection.Available < namel)
                            Thread.Sleep(1);
                        zb.Read(buf, 0, namel);

                        //and the value
                        while (cRIOConnection.Available < valuel)
                            Thread.Sleep(1);
                        zb.Read(vbuf, 0, valuel);

                        //Add the value
                        string nom = Encoding.UTF8.GetString(buf), val = Encoding.UTF8.GetString(vbuf);
                        if (kys.ContainsKey(nom))
                            kys[nom] = val;
                        else
                            kys.Add(nom, val);

                        //Decrease error
                        if (nume > 0)
                            nume--;
                    }
                    catch (ThreadAbortException)
                    {
                        isrunning = false;
                        return;
                    }
                    catch (Exception ex)
                    {
                        nume++;
                        DoError(ex);
                        if (nume > 10)
                        {
                            isrunning = false;
                            DoError(new Exception("10 consecutive errors were encountered, stopping DashboardDataHub"));
                            isrunning = false;
                            return;
                        }
                    }
                }
            }
            catch { }//Don't let em leak!
            finally
            {
                if (zb != null)
                    zb.Dispose();
                try
                {
                    cRIOConnection.Close();
                }
                catch { }
                cRIOConnection = null;
            }
        }

        /// <summary>
        /// Processes any errors that may have been encountered, and either fires the OnError, or Alerts the user
        /// </summary>
        /// <param name="ex">The error</param>
        internal void DoError(Exception ex)
        {
            if (OnError == null)
            {
                //TODO: I want to throw the exception, but I need to make the running catch do calls safe
            }
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
            return new ZomBUrlInfo { DefaultPort = 9066 };
        }
    }
}
