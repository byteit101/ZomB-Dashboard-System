﻿/*
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
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace System451.Communication.Dashboard.Net
{
    /// <summary>
    /// Emulates the cRIO in sending pre-determined packets to test ZomB. Sends to localhost
    /// </summary>
    public class DashboardPacketEmulator
    {
        byte[][] pkts;
        int i = 0;
        Thread t;
        UdpClient uc;

        /// <summary>
        /// Create a new V-cRIO
        /// </summary>
        public DashboardPacketEmulator()
        {
            pkts = new byte[][] { };
        }

        /// <summary>
        /// Kill the V-cRIO
        /// </summary>
        ~DashboardPacketEmulator()
        {
            Stop();
        }

        /// <summary>
        /// Gets or sets the array of byte[1018] packets to send
        /// </summary>
        public byte[][] Packets
        {
            get
            {
                return pkts;
            }
            set
            {
                if (pkts == null)
                    pkts = new byte[][] { };
                lock (pkts)
                {
                    pkts = value;
                    i = 0;
                }
            }
        }

        /// <summary>
        /// Starts the packet sending to localhost
        /// </summary>
        public void Start()
        {
            if (Packets != null)
            {
                t = new Thread(dowork);
                t.IsBackground = true;
                t.Start();
            }
        }

        void dowork()
        {
            uc = new UdpClient();
            i = 0;
            while (true)
            {
                Thread.Sleep(20);
                lock (pkts)
                {
                    if (i >= Packets.Length)
                        i = 0;
                    uc.Send(Packets[i], 1018, new IPEndPoint(IPAddress.Loopback, 1165));
                    i++;
                }
            }
        }

        /// <summary>
        /// Stop packet sending
        /// </summary>
        public void Stop()
        {
            try
            {
                t.Abort();
            }
            catch { }
        }
    }

    /// <summary>
    /// Emulates the cRIO TCP in sending pre-determined packets to test ZomB. Sends to localhost
    /// </summary>
    public class DashboardTCPEmulator
    {
        byte[][] pkts;
        int i = 0;
        Thread t;
        TcpListener listen;

        /// <summary>
        /// Create a new V-cRIO
        /// </summary>
        public DashboardTCPEmulator()
        {
            pkts = new byte[][] { };
        }

        /// <summary>
        /// Kill the V-cRIO
        /// </summary>
        ~DashboardTCPEmulator()
        {
            Stop();
        }

        /// <summary>
        /// Gets or sets the array of byte[1018] packets to send
        /// </summary>
        public byte[][] Packets
        {
            get
            {
                return pkts;
            }
            set
            {
                if (pkts == null)
                    pkts = new byte[][] { };
                lock (pkts)
                {
                    pkts = value;
                    i = 0;
                }
            }
        }

        /// <summary>
        /// Starts the packet sending to localhost
        /// </summary>
        public void Start()
        {
            if (Packets != null)
            {
                t = new Thread(dowork);
                t.IsBackground = true;
                t.Start();
            }
        }

        void dowork()
        {
            listen = new TcpListener(new IPEndPoint(IPAddress.Loopback, 9066));
            listen.Start();
            i = 0;
            while (true)
            {
                try
                {
                    var client = listen.AcceptTcpClient();
                    var stream = client.GetStream();
                    stream.Write(new byte[] { 0x45, 0x00 }, 0, 2);
                    while (client.Connected)
                    {
                        Thread.Sleep(20);
                        lock (pkts)
                        {
                            if (i >= Packets.Length)
                                i = 0;
                            stream.Write(Packets[i], 0, Packets[i].Length);
                            i++;
                        }
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Stop packet sending
        /// </summary>
        public void Stop()
        {
            try
            {
                t.Abort();
            }
            catch { }
        }
    }
}
