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
using System.Net.Sockets;
using System.Collections.ObjectModel;
using System.Net;

namespace System451.Communication.Dashboard.Net.Socket
{
    public class TsTcpClient
    {
        static Dictionary<int, TsTcpClient> sockets = new Dictionary<int, TsTcpClient>();

        /// <summary>
        /// Gets a shared TcpClient. Throws if none exist.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">You need to create a TsTcpClient previous to this invocation</exception>
        /// <returns>Thread Safe UdpClient with specifed port</returns>
        public static TsTcpClient GetTcpClient()
        {
            if (sockets.Count > 0)
                return sockets.First().Value;
            else
                throw new InvalidOperationException("No unnamed Thread Safe TcpClient objects avalible. Add one or add a port number to the call");
        }

        /// <summary>
        /// Gets or creates the shared TcpClient on the specified port
        /// </summary>
        /// <param name="port">What TCP port it should default to</param>
        /// <returns>Thread Safe TcpClient with specifed port</returns>
        public static TsTcpClient GetTcpClient(int port)
        {
            if (sockets[port] == null)
            {
                var tmp = new TsUdpClient(port);
                sockets[port] = tmp;
                return tmp;
            }
            else
                return sockets[port];
        }

        TcpClient sock;

        private TsTcpClient(IPEndPoint port)
        {
            sock = new TcpClient(port);
        }

        //FIXME!
    }
}
