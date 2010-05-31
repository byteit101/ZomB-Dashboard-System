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

    /// <summary>
    /// Allows the ZomB URL parser to automaticaly find and instance a DataSource
    /// </summary>
    [global::System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class DataSourceAttribute : Attribute
    {
        /// <summary>
        /// Allows the ZomB URL parser to automaticaly find and instance a DataSource
        /// </summary>
        /// <param name="sourceName">The name of this source (ie DBPacket)</param>
        public DataSourceAttribute(string sourceName)
        {
            this.SourceName = sourceName;
            this.ConstructorFormat = "";
        }

        /// <summary>
        /// Allows the ZomB URL parser to automaticaly find and instance a DataSource
        /// </summary>
        /// <param name="sourceName">The name of this source (ie DBPacket)</param>
        /// <param name="constructorFormat">The format of the constructor, leave blank if it accepts empty constructor. Otherwise, use the format "_name,_required,[_optional,[o_ptional" for the constructor. All values are case-insensitive, shorthand is specified with the underscore.</param>
        public DataSourceAttribute(string sourceName, string constructorFormat)
        {
            this.SourceName = sourceName;
            this.ConstructorFormat = constructorFormat;
        }

        /// <summary>
        /// The name of this source (ie DBPacket)
        /// </summary>
        public string SourceName { get; private set; }

        /// <summary>
        /// The format of the constructor, leave blank if it accepts empty constructor.
        /// Otherwise, use the format "_name,_required,[_optional,[o_ptional" for the constructor.
        /// All values are case-insensitive, shorthand is specified with the underscore.
        /// </summary>
        /// <remarks>
        /// There are a few special arguments:
        /// _port is the URL's port
        /// _team is the URL's team
        /// & denotes a IZomBController must be passed in
        /// $ denotes the service must be passed in (path on server as string)
        /// </remarks>
        public string ConstructorFormat { get; private set; }
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
