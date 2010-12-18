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
    public class UDPProxy
    {
        UdpClient uc;

        public UDPProxy(IPEndPoint localEP, IPEndPoint remoteEP)
        {
            LocalFromEP = localEP;
            RemoteToEP = remoteEP;
        }

        ~UDPProxy()
        {
            Stop();
        }

        public IPEndPoint LocalFromEP { get; private set; }

        public IPEndPoint RemoteToEP { get; private set; }

        public IPEndPoint RemoteFromEP { get; private set; }

        public void Stop()
        {
            uc = null;
        }

        public void Start()
        {
            uc = new UdpClient(LocalFromEP);
            uc.BeginReceive(fromRecive, this);
        }

        private void fromRecive(IAsyncResult ar)
        {
            try
            {
                IPEndPoint iep = null;
                byte[] buf = uc.EndReceive(ar, ref iep);
                RemoteFromEP = iep;
                int send = uc.Send(buf, buf.Length);
                if (send != buf.Length)
                {
                }
                uc.BeginReceive(fromRecive, this);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Yowuza!");
            }
        }
    }
}
