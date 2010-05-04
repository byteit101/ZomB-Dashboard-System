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
using System.Text;
using System.IO;

namespace System451.Communication.Dashboard.Net
{
    /// <summary>
    /// Represents a source of Dashboard data
    /// </summary>
    public interface IDashboardDataSource
    {
        void Start();
        void Stop();
        bool HasStatus { get; }
        bool HasData { get; }
        IDashboardStatusDataSource GetStatusSource();
        IDashboardDataDataSource GetDataSource();
        event EventHandler DataRecieved;
        event InvalidPacketRecievedEventHandler InvalidPacketRecieved;
        event ErrorEventHandler OnError;
    }

    /// <summary>
    /// Represents the Status portion of a IDashboardDataSource
    /// </summary>
    public interface IDashboardStatusDataSource : IDashboardDataSource
    {
        FRCDSStatus GetStatus();
        IDashboardDataSource ParentDataSource { get; }
        event NewStatusRecievedEventHandler NewStatusRecieved;
    }

    /// <summary>
    /// Represents the Data portion of a IDashboardDataSource
    /// </summary>
    public interface IDashboardDataDataSource : IDashboardDataSource
    {
        Dictionary<string, string> GetData();
        IDashboardDataSource ParentDataSource { get; }
        event NewDataRecievedEventHandler NewDataRecieved;
    }

    public delegate void InvalidPacketRecievedEventHandler(object sender, InvalidPacketRecievedEventArgs e);
    public delegate void NewDataRecievedEventHandler(object sender, NewDataRecievedEventArgs e);
    public delegate void NewStatusRecievedEventHandler(object sender, NewStatusRecievedEventArgs e);

    public class NewStatusRecievedEventArgs : EventArgs
    {
        public NewStatusRecievedEventArgs(FRCDSStatus status)
        {
            NewStatus = status;
        }
        public FRCDSStatus NewStatus { get; set; }
    }

    public class NewDataRecievedEventArgs : EventArgs
    {
        public NewDataRecievedEventArgs(Dictionary<string, string> data)
        {
            NewData = data;
        }
        public Dictionary<string, string> NewData { get; set; }
    }
}
