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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System451.Communication.Dashboard.WPF.Controls.Designer;
using System451.Communication.Dashboard.Utils;

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
    /// Represents a source of Peekable Dashboard data
    /// </summary>
    public interface IDashboardPeekableDataSource : IDashboardDataDataSource
    {
        bool BeginNamePeek(StringFunction callback);
        void EndNamePeek();
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
        ZomBDataLookup GetData();
        IDashboardDataSource ParentDataSource { get; }
        event NewDataRecievedEventHandler NewDataRecieved;
    }

    /// <summary>
    /// Allows the ZomB URL parser to automaticaly find and instance a DataSource
    /// </summary>
    [global::System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class DataSourceAttribute : Attribute
    {
        /// <summary>
        /// Allows the ZomB URL parser to automaticaly find and instance a DataSource
        /// </summary>
        /// <param name="sourceName">The name of this source (ie DBPacket)</param>
        public DataSourceAttribute(string sourceName)
        {
            this.SourceName = sourceName;
            this.Description = "";
            this.ConstructorFormat = "";
            this.IgnoreClones = true;
        }

        /// <summary>
        /// Allows the ZomB URL parser to automaticaly find and instance a DataSource
        /// </summary>
        /// <param name="sourceName">The name of this source (ie DBPacket)</param>
        /// <param name="description">A quick description of what this source does</param>
        public DataSourceAttribute(string sourceName, string description)
        {
            this.SourceName = sourceName;
            this.Description = description;
            this.ConstructorFormat = "";
            this.IgnoreClones = true;
        }

        /// <summary>
        /// Allows the ZomB URL parser to automaticaly find and instance a DataSource
        /// </summary>
        /// <param name="sourceName">The name of this source (ie DBPacket)</param>
        /// <param name="description">A quick description of what this source does</param>
        /// <param name="constructorFormat">The format of the constructor, leave blank if it accepts empty constructor. Otherwise, use the format "_name,_required,[_optional,[o_ptional" for the constructor. All values are case-insensitive, shorthand is specified with the underscore.</param>
        public DataSourceAttribute(string sourceName, string description, string constructorFormat)
        {
            this.SourceName = sourceName;
            this.Description = description;
            this.ConstructorFormat = constructorFormat;
            this.IgnoreClones = true;
        }

        /// <summary>
        /// The name of this source (ie DBPacket)
        /// </summary>
        public string SourceName { get; private set; }

        /// <summary>
        /// A quick description of what this source does
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Ignore double attributes to one class (default true).
        /// Useful for name-based behaviors
        /// </summary>
        public bool IgnoreClones { get; set; }

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

    public class TypedDataSourceAttributeComparer :IComparable, IComparable<TypedDataSourceAttributeComparer>
    {
        public Type Type { get; set; }
        public DataSourceAttribute DataSourceAttribute { get; set; }


        #region IComparable Members

        public int CompareTo(object obj)
        {
            return this.CompareTo((obj as TypedDataSourceAttributeComparer));
        }

        #endregion

        #region IComparable<TypedDataSourceAttributeComparer> Members

        public int CompareTo(TypedDataSourceAttributeComparer other)
        {
            int r = this.Type.Name.CompareTo(other.Type.Name);
            if (r == 0)
            {
                return this.DataSourceAttribute.SourceName.CompareTo(other.DataSourceAttribute.SourceName);
            }
            return r;
        }

        #endregion
    }

    [TypeConverter(typeof(ZomBUrlConverter))]
    public class ZomBUrl
    {
        protected ZomBUrl(string zombUrl)
        {
            if (zombUrl == null)
                throw new ArgumentNullException("zombUrl is null");
            if (!zombUrl.StartsWith("zomb://", StringComparison.CurrentCultureIgnoreCase))
                throw new ArgumentException("Url is not a ZomB url");
            var r = new Regex("^zomb://([\\.0-9a-zA-Z]+)(\\:([0-9]{3,5}))?/([a-zA-Z]+[0-9a-zA-Z]*)([/|\\?].*)?$", RegexOptions.IgnoreCase);
            var res = r.Match(zombUrl);
            //no matches? let it throw!
            string to = res.Groups[1].Value;
            string port = res.Groups[3].Value;
            SourceName = res.Groups[4].Value;
            try
            {
                Path = res.Groups[5].Value;
            }
            catch { }
            SourceType = FindSourceType();
            var getInfo = SourceType.GetMethod("GetZomBUrlInfo", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public, null, new Type[] { }, null);
            if (getInfo == null)
                throw new InvalidProgramException("Type '" + SourceType.ToString() + "' does not contain required static method GetZomBUrlInfo");
            DefaultZomBUrlInfo = getInfo.Invoke(null, null) as ZomBUrlInfo;
            if (port == "")
                Port = DefaultZomBUrlInfo.DefaultPort;
            else
                Port = int.Parse(port);
            if (to.Length < 2)
                throw new InvalidDataException("ZomB Url is to an invalid host");
            if (to[0] == '.')//team syntax
            {
                int team = int.Parse(to.Substring(1));
                IPAddress = IPAddress.Parse("10." + team / 100 + "." + team % 100 + ".2");
            }
            else if (to.Equals("localhost", StringComparison.CurrentCultureIgnoreCase))
                IPAddress = IPAddress.Loopback;
            else
                IPAddress = IPAddress.Parse(to);
        }

        private Type FindSourceType()
        {
            //this function takes a while, so lets have some predefined ones
            switch (SourceName)
            {
                case "DBPkt":
                case "DBPacket":
                    return typeof(DashboardPacketDataSource);
                case "TCP":
                    return typeof(TCPDataSource);
                case "TCP2":
                    return typeof(TCPDataSender);
                case "DataSave":
                    return typeof(DataSaver);
                case "File":
                    return typeof(DataPlayerSource);
                case "Smart":
                    return typeof(SmartDataSource);
                case "SmartNG":
                    return typeof(NetTableSource);
                case "NetTable":
                    return typeof(NetTableSource);
                case "Serial":
                    return typeof(SerialDataSource);
            }
            foreach (var item in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var itype in item.GetTypes())
                {
                    foreach (var cat in itype.GetCustomAttributes(typeof(DataSourceAttribute), false))
                    {
                        if ((cat as DataSourceAttribute).SourceName.Equals(SourceName, StringComparison.CurrentCultureIgnoreCase))
                            return itype;
                    }
                }
            }
            return null;
        }

        public static ZomBUrl Parse(string zombUrl)
        {
            return new ZomBUrl(zombUrl);
        }

        public static ZomBUrl Parse(string zombUrl, int TeamNumber)
        {
            if (zombUrl.Contains("zomb://./"))
                zombUrl = zombUrl.Replace("zomb://./", "zomb://." + TeamNumber + "/");
            return new ZomBUrl(zombUrl);
        }

        public int Port { get; private set; }
        public IPAddress IPAddress { get; private set; }
        public string SourceName { get; private set; }
        public string Path { get; private set; }
        public Type SourceType { get; private set; }
        protected ZomBUrlInfo DefaultZomBUrlInfo { get; private set; }

        /// <summary>
        /// Initializes this url and returns an instance of the proper class
        /// </summary>
        /// <param name="ctrlr">A ZomB controller, or null. Some controls may require this, others may not.</param>
        /// <returns>instance of the proper (pinkies up) class</returns>
        public object Exec(IZomBController ctrlr)
        {
            if (SourceType == null || IPAddress == null)
                throw new InvalidOperationException("ZomB URL is not valid, cannot continue");
            var ctor = SourceType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(ZomBUrl) }, null);
            if (ctor == null)
            {
                ctor = SourceType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(ZomBUrl), typeof(IZomBController) }, null);
                if (ctor == null)
                    throw new InvalidProgramException("Type '" + SourceType.ToString() + "' does not contain required constructor with arguments (ZomBUrl[, IZomBController])");
                return ctor.Invoke(new object[] { this, ctrlr });
            }
            return ctor.Invoke(new object[] { this });
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool useTeamDot)
        {
            string port = DefaultZomBUrlInfo.DefaultPort == Port ? "" : ":" + Port;
            string iadr = IPAddress.ToString();
            if (useTeamDot && iadr.StartsWith("10.") && iadr.EndsWith(".2"))
                iadr = "." + (int.Parse(iadr.Substring(3).Replace(".", "")) / 10);
            return "zomb://" + iadr + port + "/" + SourceName + (string.IsNullOrEmpty(Path) ? "" : Path);
        }
    }

    public class ZomBUrlConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
                return ZomBUrl.Parse(value.ToString());
            return base.ConvertFrom(context, culture, value);
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
                return (value as ZomBUrl).ToString();
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    [TypeConverter(typeof(ZomBUrlCollectionConverter)), WPF.Design.Designer(typeof(ZomBUrlCollectionDesigner))]
    public class ZomBUrlCollection : Collection<ZomBUrl>
    {
        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool? useTeamDot)
        {
            string res = "";
            foreach (var item in this)
            {
                if (useTeamDot == null)
                {
                    res += "/" + item.SourceName + ";";
                }
                else
                    res += item.ToString((bool)useTeamDot) + ";";
            }
            return res;
        }

        public static explicit operator string(ZomBUrlCollection col)
        {
            return col.ToString();
        }
        public static implicit operator ZomBUrlCollection(string col)
        {
            return new ZomBUrlCollectionConverter().ConvertFrom(null, null, col) as ZomBUrlCollection;
        }
    }

    public class ZomBUrlCollectionConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                var urls = value.ToString().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                ZomBUrlCollection Zuc = new ZomBUrlCollection();
                foreach (var item in urls)
                {
                    Zuc.Add(ZomBUrl.Parse(item.ToString()));
                }
                return Zuc;
            }
            return base.ConvertFrom(context, culture, value);
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
                return (value as ZomBUrlCollection).ToString();
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class ZomBUrlInfo
    {
        public int DefaultPort { get; set; }
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
        public NewDataRecievedEventArgs(ZomBDataLookup data)
        {
            NewData = data;
        }
        public ZomBDataLookup NewData { get; set; }
    }
}
