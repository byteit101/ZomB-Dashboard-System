/*
 * ZomB Dashboard System <http://firstforge.wpi.edu/sf/projects/zombdashboard>
 * Copyright (C) 2011, Patrick Plenefisch and FIRST Robotics Team 451 "The Cat Attack"
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
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace System451.Communication.Dashboard.Net
{
    [DataSource("TCP2")]
    public class TCPDataSender: IDataSender
    {
        TcpListener cRIOConnection;
        TcpClient tcpc = null;
        bool isrunning;

        public TCPDataSender(int team)
        {
            IPAddress = IPAddress.Parse("10." + ((int)(team / 100)) + "." + ((int)(team % 100)) + ".2");
            Port = TCPDataSource.DefaultPort+1;
        }

        public TCPDataSender(int team, int port)
        {
            IPAddress = IPAddress.Parse("10." + ((int)(team / 100)) + "." + ((int)(team % 100)) + ".2");
            Port = port;
        }

        public TCPDataSender(IPAddress addr)
        {
            IPAddress = addr;
            Port = TCPDataSource.DefaultPort+1;
        }

        public TCPDataSender(IPAddress addr, int port)
        {
            IPAddress = addr;
            Port = port;
        }

        public TCPDataSender(ZomBUrl zurl)
        {
            IPAddress = zurl.IPAddress;
            Port = zurl.Port;
        }

        ~TCPDataSender()
        {
            Stop();
        }

        public int Port { get; private set; }
        public IPAddress IPAddress
        {
            get;
            set;
        }

        #region IDataSender Members

        public void Start()
        {
            if (!isrunning)
            {
                if (cRIOConnection == null)
                {
                    cRIOConnection = new TcpListener(IPAddress.Any, Port);
                }
                if (tcpc != null)
                {
                    try
                    {
                        tcpc.GetStream().WriteByte(0x00);
                        tcpc.GetStream().WriteByte(0x00);
                        tcpc.GetStream().WriteByte(0x00);
                        tcpc.GetStream().WriteByte(0x00);
                        tcpc.Close();
                    }
                    catch (Exception ex)
                    {
                        DoError(ex);
                    }
                }
                try
                {
                    tcpc = null;
                    cRIOConnection.Start();
                    cRIOConnection.BeginAcceptTcpClient(new AsyncCallback(delegate (IAsyncResult ar)
                        {
                            try
                            {
                                tcpc = cRIOConnection.EndAcceptTcpClient(ar);
                                tcpc.GetStream().WriteByte(0x49);
                                tcpc.GetStream().WriteByte(0x00);
                                isrunning = true;
                            }
                            catch { }//must be disposing, bye-bye world!
                            //*Thunk* The coffin slams shut
                        }), cRIOConnection);
                }
                catch (Exception ex)
                {
                    DoError(ex);
                }
            }
        }

        public void Stop()
        {
            isrunning = false;
            try
            {
                if (tcpc != null)
                    tcpc.Close();
                cRIOConnection.Stop();
            }
            catch { }
            tcpc = null;
            cRIOConnection = null;
        }

        public void Send(string name, string value)
        {
            if (isrunning && tcpc != null)
            {
                if (name.Length > 255)
                    throw new ArgumentException("Control name is too long", "name");
                if (value.Length > 255) //TODO: longvalues
                    throw new ArgumentException("Value is too long", "value");

                tcpc.GetStream().WriteByte(Convert.ToByte(name.Length));
                tcpc.GetStream().WriteByte(Convert.ToByte(value.Length));

                byte[] buff = ASCIIEncoding.ASCII.GetBytes(name);
                tcpc.GetStream().Write(buff, 0, buff.Length);
                buff = ASCIIEncoding.ASCII.GetBytes(value);
                tcpc.GetStream().Write(buff, 0, buff.Length);

                tcpc.GetStream().WriteByte(0x00);
            }
            else
                throw new InvalidOperationException("You need to start the sender before sending data");
        }

        public event ErrorEventHandler OnError;

        #endregion

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
            get
            {
                return isrunning;
            }
        }

        /// <summary>
        /// Magic method for zomb:// urls
        /// </summary>
        /// <returns></returns>
        private static ZomBUrlInfo GetZomBUrlInfo()
        {
            return new ZomBUrlInfo { DefaultPort = 9067 };
        }
    }
}
