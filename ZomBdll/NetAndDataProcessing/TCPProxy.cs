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
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace System451.Communication.Dashboard.Net
{
    public class TCPProxy
    {
        Socket localsoc, remotesoc, remotefrom;
        Thread thrds;
        byte[] frombuf = new byte[10000];
        byte[] tobuf = new byte[10000];
        int frombytes, tobyes;

        public TCPProxy(IPEndPoint localEP, IPEndPoint remoteEP)
        {
            LocalFromEP = localEP;
            RemoteToEP = remoteEP;
        }

        ~TCPProxy()
        {
            Stop();
        }

        public IPEndPoint LocalFromEP { get; private set; }

        public IPEndPoint RemoteToEP { get; private set; }

        public IPEndPoint RemoteFromEP { get; private set; }

        public void Stop()
        {

        }

        public void Start()
        {
            localsoc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            localsoc.Bind(LocalFromEP);
            localsoc.Listen(10);
            localsoc.BeginAccept(new AsyncCallback(localaccept), this);
        }

        private void localaccept(IAsyncResult ar)
        {
            try
            {
                remotefrom = localsoc.EndAccept(ar);
                RemoteFromEP = (IPEndPoint)remotefrom.RemoteEndPoint;
                remotesoc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                remotesoc.BeginConnect(RemoteToEP, remoteconect, this);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Yowza!");
            }
        }

        private void remoteconect(IAsyncResult ar)
        {
            try
            {
                remotesoc.EndConnect(ar);
                //thrds = new Thread(RunDataExchange);
                RunDataExchange();
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Yowza 2!");
            }
        }

        private void RunDataExchange()
        {
            remotesoc.BeginReceive(tobuf, 0, tobuf.Length, SocketFlags.None, toRead, this);
            remotefrom.BeginReceive(frombuf, 0, frombuf.Length, SocketFlags.None, fromRead, this);
        }

        private void toRead(IAsyncResult ar)
        {
            try
            {
                tobyes = remotesoc.EndReceive(ar);
                remotefrom.BeginSend(tobuf, 0, tobyes, SocketFlags.None, tosend, this);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Yowza 2r!");
            }
        }

        private void tosend(IAsyncResult ar)
        {
            try
            {
                int bytes = remotefrom.EndSend(ar);
                if (bytes != tobyes)
                {
                    System.Windows.Forms.MessageBox.Show("Yowza 3!");
                }
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Yowza 23!");
            }
        }

        private void fromRead(IAsyncResult ar)
        {
            try
            {
                frombytes = remotefrom.EndReceive(ar);
                remotesoc.BeginSend(frombuf, 0, frombytes, SocketFlags.None, fromsend, this);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Yowza 2r3!");
            }
        }

        private void fromsend(IAsyncResult ar)
        {
            try
            {
                int bytes = remotesoc.EndSend(ar);
                if (bytes != frombytes)
                {
                    System.Windows.Forms.MessageBox.Show("Yowza 3.5!");
                }
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Yowza 23.5!");
            }
        }
    }
}
