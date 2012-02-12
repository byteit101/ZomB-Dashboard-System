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
using System.Linq;

namespace System451.Communication.Dashboard.Net
{
    [DataSource("NetTable", "2012 SmartDashboard NetworkTable support (Expands SmartGroups)", IgnoreClones=false)]
    [DataSource("SmartNG", "2012 SmartDashboard NetworkTable support", IgnoreClones = false)]
    public class NetTableSource : IDashboardDataSource, IDashboardDataDataSource, IDashboardPeekableDataSource, IDataSender
    {
        public const int NTPort = 1735;
        TcpClient cRIOConnection;
        bool isrunning;
        Thread backThread;
        DashboardDataHub ddh;
        uint remoteTable = 0;
        byte[] remoteTableBytes = null;
        int port = NTPort;
        Semaphore Qlock = new Semaphore(1, 1);
        NetworkStream nstream = null;
        bool hideSmartTypes = false;

        ZomBDataLookup kys = new ZomBDataLookup();
        Dictionary<uint, string> nametable = new Dictionary<uint, string>();
        Dictionary<uint, ZomBDataTypeHint> typetable = new Dictionary<uint, ZomBDataTypeHint>();
        Dictionary<uint, ZomBDataLookup> tables = new Dictionary<uint, ZomBDataLookup>();
        Dictionary<uint, ZomBDataLookup> tablesbyfield = new Dictionary<uint, ZomBDataLookup>();
        Dictionary<ZomBDataLookup, NameTableGrouping> tablesbyparent = new Dictionary<ZomBDataLookup, NameTableGrouping>();
        Queue<byte[]> sendQ = new Queue<byte[]>();

        private class NameTableGrouping
        {
            public ZomBDataLookup Table { get; set; }
            public string Name { get; set; }
        }

        const int STRING = 0;
        const int BEGIN_STRING = 0xFF;
        const int END_STRING = 0x00;
        const int INT = 0x01;
        const int DOUBLE = 0x02;
        const int TABLE = 0x03;
        const int TABLE_ASSIGNMENT = TABLE;
        const int BOOLEAN_FALSE = 0x04;
        const int BOOLEAN_TRUE = 0x05;
        const int ASSIGNMENT = 0x06;
        const int EMPTY = 0x07;
        const int DATA = 0x08;
        const int OLD_DATA = 0x09;
        const int TRANSACTION = 0x0A;
        const int REMOVAL = 0x0B;
        const int TABLE_REQUEST = 0x0C;
        const int DENIAL = 0x10;
        const int CONFIRMATION = 0x20;
        const int CONFIRMATION_MAX = 0x1F;
        const int PING = CONFIRMATION;
        const int TABLE_ID = 0x40;
        const int FIELD_ID = 0x80;
        const byte TABLE_ID_MAGIC_NUMBER = 0x7C;
        const byte FIELD_ID_MAGIC_NUMBER = 0xFC;

        static readonly byte[] SMART_REQUEST = { 0x0C /*TABLE_REQUEST*/, 0x0E /*string length*/, 0x53, 0x6d, 0x61, 0x72, 0x74, 0x44, 0x61, 0x73, 0x68, 0x62, 0x6f, 0x61, 0x72, 0x64 /* ASCII: SmartDashboard */, 0x40 /* Table 0 */ };

        public NetTableSource(IZomBController ddh)
        {
            this.ddh = ddh.GetDashboardDataHub();
            tables[0] = kys;
            tablesbyparent[kys] = null;
        }

        public NetTableSource(ZomBUrl info, IZomBController ddh)
        {
            if (ddh != null)
                this.ddh = ddh.GetDashboardDataHub();
            this.Hostname = info.IPAddress.ToString();
            this.port = info.Port;
            this.hideSmartTypes = (info.SourceName == "SmartNG" && !info.Path.Contains("CollapseTypes=false")) || (info.SourceName == "NetTable" && info.Path.Contains("CollapseTypes=true"));
            tables[0] = kys;
            tablesbyparent[kys] = null;
        }

        ~NetTableSource()
        {
            Stop();
        }

        private string hostname;

        public string Hostname
        {
            get { return hostname; }
            set
            {
                hostname = value;
                if (isrunning)
                {
                    Stop();
                    Start();
                }
            }
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
                    cRIOConnection.Close();
                }
                catch
                {
                    try
                    {
                        cRIOConnection.Close();
                    }
                    catch { }
                }
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
#pragma warning disable 67
        public event InvalidPacketRecievedEventHandler InvalidPacketRecieved;
#pragma warning restore 67
        public event ErrorEventHandler OnError;

        #endregion

        #region IDashboardDataDataSource Members

        public ZomBDataLookup GetData()
        {
            return kys;
        }

        public event NewDataRecievedEventHandler NewDataRecieved;

        public IDashboardDataSource ParentDataSource
        {
            get { return this; }
        }

        #endregion

        #region IDataSender Members

        Dictionary<uint, string> idcache = new Dictionary<uint, string>();
        Dictionary<string, uint> idnamecache = new Dictionary<string, uint>();

        public void Send(string name, string value)
        {
            uint fid = 0;
            if (idnamecache.ContainsKey(name))
                fid = idnamecache[name];
            else
            {
                var elms = ZomBDataLookup.GetNameSegments(name);
                ZomBDataLookup obj = kys;
                foreach (var item in elms)
                {
                    var tmp = obj[item];
                    if (tmp.Value is ZomBDataLookup && item != elms[elms.Length - 1])
                    {
                        obj = tmp.Value as ZomBDataLookup;
                        continue;
                    }
                    var ids = (from key in nametable
                         where key.Value == elms[elms.Length - 1]
                        select key.Key);
                    foreach (var iitem in ids)
                    {
                        if (tablesbyfield[iitem] == obj)
                        {
                            fid = iitem;
                            break;
                        }
                    }
                    break;
                }
                idnamecache.AddOrUpdate(name, fid);
                idcache.AddOrUpdate(fid, name);
            }
            ZomBDataTypeHint type = typetable[fid];
            CreateUpdate(fid, type, value);
        }

        #endregion

        #region conversions

        private string FieldIDToDottedName(uint fid)
        {
            if (idcache.ContainsKey(fid))
                return idcache[fid];
            string name = FieldIDToDottedNameNoCache(fid);
            idcache.AddOrUpdate(fid, name);
            idnamecache.AddOrUpdate(name, fid);
            return name;
        }

        private string FieldIDToDottedNameNoCache(uint fid)
        {
            string name = nametable[fid];
            ZomBDataLookup lup = tablesbyfield[fid];
            if (hideSmartTypes && ((name == "Data" && lup.ContainsKey("~TYPE~") && tablesbyparent.ContainsKey(lup))
                    || (name == "~TYPE~" && tablesbyparent.ContainsKey(lup))))
                return null;
            while (lup != kys)
            {
                var tmp = tablesbyparent[lup];
                if (hideSmartTypes && ((tmp.Name == "Data" && tmp.Table.ContainsKey("~TYPE~") && tablesbyparent.ContainsKey(tmp.Table))
                    || (tmp.Name == "~TYPE~" && tablesbyparent.ContainsKey(tmp.Table))))
                    return null;

                name = tmp.Name + "." + name;
                lup = tmp.Table;
            }
            return name;
        }

        private byte[] FieldIDToBytes(uint id)
        {//0xFC is with field bit, 7C w/o
            if (id < 124)
                return new byte[] { (byte)(id | FIELD_ID) };
            else if (id <= 0xFF)
                return new byte[] { 0xFC, (byte)id };
            else if (id <= 0xFFFF)
                return new byte[] { 0xFD, (byte)(id << 0x08), (byte)id };
            else if (id <= 0xFFFFFF)
                return new byte[] { 0xFE, (byte)(id << 0x10), (byte)(id << 0x08), (byte)id };
            else //if (id <= 0xFFFFFFFF)
                return new byte[] { 0xFF, (byte)(id << 0x18), (byte)(id << 0x10), (byte)(id << 0x08), (byte)id };
        }

        private byte[] TableIDToBytes(uint id)
        {//0x7C is with field bit, 3C w/o
            if (id < 60)
                return new byte[] { (byte)(id | TABLE_ID) };
            else if (id <= 0xFF)
                return new byte[] { 0x7C, (byte)id };
            else if (id <= 0xFFFF)
                return new byte[] { 0x7D, (byte)(id >> 0x08), (byte)id };
            else if (id <= 0xFFFFFF)
                return new byte[] { 0x7E, (byte)(id >> 0x10), (byte)(id >> 0x08), (byte)id };
            else //if (id <= 0xFFFFFFFF)
                return new byte[] { 0x7F, (byte)(id >> 0x18), (byte)(id >> 0x10), (byte)(id >> 0x08), (byte)id };
        }

        private uint BytesToTableID(byte[] bytes)
        {//0x7C is with field bit, 3C w/o
            if ((bytes[0] & 0x7C) != 0x7C)
                return (uint)(bytes[0] & ~TABLE_ID);
            else if ((bytes[0] & 0x03) == 0)
                return (uint)bytes[1];
            else if ((bytes[0] & 0x03) == 1)
                return (uint)((bytes[1] << 0x08) + bytes[2]);
            else if ((bytes[0] & 0x03) == 2)
                return (uint)(((uint)bytes[1] << 0x10) + (bytes[2] << 0x08) + bytes[3]);
            else //if ((bytes[0] & 0x03) == 3)
                return (uint)(((uint)bytes[1] << 0x18) + ((uint)bytes[2] << 0x10) + (bytes[3] << 0x08) + bytes[4]);
        }

        private uint BytesToFieldID(byte[] bytes)
        {//0xFC is with field bit, 7C w/o
            if ((bytes[0] & 0xFC) != 0xFC)
                return (uint)(bytes[0] & ~FIELD_ID);
            else if ((bytes[0] & 0x03) == 0)
                return (uint)bytes[1];
            else if ((bytes[0] & 0x03) == 1)
                return (uint)((bytes[1] << 0x08) + bytes[2]);
            else if ((bytes[0] & 0x03) == 2)
                return (uint)(((uint)bytes[1] << 0x10) + (bytes[2] << 0x08) + bytes[3]);
            else //if ((bytes[0] & 0x03) == 3)
                return (uint)(((uint)bytes[1] << 0x18) + ((uint)bytes[2] << 0x10) + (bytes[3] << 0x08) + bytes[4]);
        }

        private byte[] ReadID(NetworkStream stream, byte fullmarker)
        {
            byte[] buffer = new byte[5];
            buffer[0] = (byte)stream.ReadByte();
            return ReadID(buffer, stream, fullmarker);
        }

        private byte[] ReadID(byte[] buffer, NetworkStream stream, byte fullmarker)
        {
            //Read convolouted byte length marker
            if ((buffer[0] & 0xFC) == fullmarker)
            {
                int len = ((buffer[0] & 0x03) + 1);
                for (int i = 0; i < len; i++)
                {
                    buffer[i + 1] = (byte)stream.ReadByte();
                }
            }
            return buffer;
        }

        private string ReadNetString(NetworkStream net)
        {
            int buf = net.ReadByte();
            if (buf == -1)
                return null;
            if (buf == BEGIN_STRING)
            {
                StringBuilder sb = new StringBuilder();
                buf = net.ReadByte();
                while (buf != END_STRING && buf != -1)
                {
                    sb.Append((char)buf);
                    buf = net.ReadByte();
                }
                return sb.ToString();
            }
            else
            {
                byte[] buffer = new byte[buf];
                net.Read(buffer, 0, buf);
                return ASCIIEncoding.ASCII.GetString(buffer);
            }
        }

        private byte[] CreateNetString(string value)
        {
            byte[] buffer = new byte[value.Length + ((value.Length < 255) ? 1 : 2)];
            if (value.Length < 255)
            {

                buffer[0] = (byte)value.Length;
                Buffer.BlockCopy(ASCIIEncoding.ASCII.GetBytes(value), 0, buffer, 1, value.Length);
            }
            else
            {
                buffer[0] = BEGIN_STRING;
                Buffer.BlockCopy(ASCIIEncoding.ASCII.GetBytes(value), 0, buffer, 1, value.Length);
                buffer[buffer.Length - 1] = END_STRING;
            }
            return buffer;
        }

        Queue<uint> Pekingnames = new Queue<uint>();
        private void UpdateValue(uint id, ZomBDataObject value, bool fire)
        {
            string name = nametable[id];
            var tbl = tablesbyfield[id];
            if (tbl == null)
            {
            }
            tbl.AddOrUpdate(name, value);


            //If we are in AutoListen mode
            if (peeking)
            {
                AlertPeek(id, value.Value is ZomBDataLookup);
            }

            if (fire)
            {
                //Fire events
                if (DataRecieved != null)
                    DataRecieved(this, new EventArgs());
                if (NewDataRecieved != null)
                    NewDataRecieved(this, new NewDataRecievedEventArgs(kys));
            }
        }

        private void AlertPeek(uint id, bool requeue)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Alerting ID: " + id);
                requeue = true;
                if (requeue)//table added, check queue
                {
                    var tque = new Queue<uint>();
                    uint pid = 0;
                    while (Pekingnames.Count > 0)
                    {
                        pid = Pekingnames.Dequeue();
                        try
                        {
                            var name = FieldIDToDottedName(pid);
                            if (name == null)//SmartDashboard Hidden type
                                continue;//skip it
                            dp.Invoke(cb, name);
                        }
                        catch //Nope, still not a table yet
                        {
                            tque.Enqueue(pid);
                        }
                    }
                    if (tque.Count > 0)
                    {
                        foreach (var item in tque)
                        {
                            Pekingnames.Enqueue(item);
                            if (item == id)
                                requeue = false;
                        }
                    }
                }
                if (requeue)
                {
                    var name = FieldIDToDottedName(id);
                    dp.Invoke(cb, name);
                }
            }
            catch //Nope, no tables yet
            {
                Pekingnames.Enqueue(id);
            }
        }

        private void ReadUpdate(int firstByte, NetworkStream stream)
        {
            this.ReadUpdate(firstByte, stream, true);
        }

        private void ReadUpdate(int firstByte, NetworkStream stream, bool fire)
        {
            //FieldID(), type, type(value)
            byte[] buffer = new byte[5];
            buffer[0] = (byte)firstByte;
            uint fid = BytesToFieldID(ReadID(buffer, stream, 0xFC));
            int type = stream.ReadByte();
            switch (type)
            {
                case STRING:
                    {
                        typetable[fid] = ZomBDataTypeHint.String;
                        string value = ReadNetString(stream);
                        this.UpdateValue(fid, value, fire);
                        break;
                    }
                case INT:
                    {
                        typetable[fid] = ZomBDataTypeHint.Integer;
                        int value = (stream.ReadByte() << 0x18) + (stream.ReadByte() << 0x10) + (stream.ReadByte() << 0x08) + (stream.ReadByte());
                        this.UpdateValue(fid, value, fire);
                        break;
                    }
                case BOOLEAN_TRUE:
                case BOOLEAN_FALSE:
                    {
                        typetable[fid] = ZomBDataTypeHint.Boolean;
                        bool value = (type & 1) == 1;
                        this.UpdateValue(fid, value, fire);
                        break;
                    }
                case DOUBLE:
                    {
                        typetable[fid] = ZomBDataTypeHint.Double;
                        long part = 0;
                        part += (long)(((long)stream.ReadByte()) << 56);
                        part += (long)(((long)stream.ReadByte()) << 48);
                        part += (long)(((long)stream.ReadByte()) << 40);
                        part += (long)(((long)stream.ReadByte()) << 32);
                        part += (long)(stream.ReadByte() << 24);
                        part += (long)(stream.ReadByte() << 16);
                        part += (long)(stream.ReadByte() << 8);
                        part += (long)(stream.ReadByte());
                        double value = BitConverter.Int64BitsToDouble(part);
                        this.UpdateValue(fid, value, fire);
                        break;
                    }
                default:
                    if ((type & TABLE_ID) == TABLE_ID) //We have a subtable
                    {
                        typetable[fid] = ZomBDataTypeHint.Lookup;
                        byte[] tableidbuff = new byte[5];
                        buffer[0] = (byte)type;
                        uint tableid = BytesToTableID(ReadID(buffer, stream, TABLE_ID_MAGIC_NUMBER));
                        ZomBDataLookup dict;
                        if (!tables.ContainsKey(tableid))//gotta create it
                        {
                            throw new InvalidOperationException("Throw er in the debugger! Something odd happened!");
                            //dict = new ZomBDataLookup();
                            //tables.AddOrUpdate(tableid, dict);
                            ////tablesbyfield.AddOrUpdate(fid, dict);//TODO: fix this
                            //this.SillySendTableLink(tableid);
                        }
                        else //only gotta assign the fid
                        {
                            dict = tables[tableid];
                        }
                        tablesbyparent.AddOrUpdate(dict, new NameTableGrouping { Table = tablesbyfield[fid], Name = nametable[fid].Replace("\\", "\\\\").Replace(".", "\\.") });
                        this.UpdateValue(fid, dict, fire);
                        break;
                    }
                    else
                    {
                        typetable[fid] = ZomBDataTypeHint.Unknown;
                        throw new Exception("WHOA! Unknown type, buddy!");
                    }
            }
        }

        private void CreateUpdate(uint fid, ZomBDataTypeHint type, string value)
        {
            //FieldID(), type, type(value)
            byte[] bufferid = FieldIDToBytes(fid);
            byte[] buffer= new byte[1];
            switch (type)
            {
                case ZomBDataTypeHint.String:
                    {
                        byte[] netstring = CreateNetString(value);
                        buffer = new byte[netstring.Length +1 + bufferid.Length];
                        Buffer.BlockCopy(bufferid, 0, buffer, 0, bufferid.Length);
                        buffer[bufferid.Length] = STRING;
                        Buffer.BlockCopy(netstring, 0, buffer, bufferid.Length + 1, netstring.Length);
                        break;
                    }
                case ZomBDataTypeHint.Integer:
                    {
                        double tvalue;
                        double.TryParse(value, out tvalue);
                        int ivalue = (int)tvalue;

                        buffer = new byte[5 + bufferid.Length];
                        Buffer.BlockCopy(bufferid, 0, buffer, 0, bufferid.Length);
                        buffer[bufferid.Length] = INT;
                        buffer[bufferid.Length + 1] = (byte)(ivalue >> 0x18);
                        buffer[bufferid.Length + 2] = (byte)(ivalue >> 0x10);
                        buffer[bufferid.Length + 3] = (byte)(ivalue >> 0x08);
                        buffer[bufferid.Length + 4] = (byte)(ivalue);
                        break;
                    }
                case ZomBDataTypeHint.Double:
                    {
                        double tvalue;
                        double.TryParse(value, out tvalue);
                        long lvalue = BitConverter.DoubleToInt64Bits(tvalue);

                        buffer = new byte[9 + bufferid.Length];
                        Buffer.BlockCopy(bufferid, 0, buffer, 0, bufferid.Length);
                        buffer[bufferid.Length] = DOUBLE;
                        for (int i = 1; i < 9; i++)
			            {
                            buffer[bufferid.Length + i] = (byte)(lvalue >> (64 - (i * 8)));
			            }
                        break;
                    }
                case ZomBDataTypeHint.Boolean:
                    {
                        bool bvalue = true;
                        if (value.ToLower() == "false" || value == "0")
                            bvalue = false;
                        else
                        {
                            if (!bool.TryParse(value, out bvalue))
                                bvalue = true;
                        }
                        buffer = new byte[1 + bufferid.Length];
                        Buffer.BlockCopy(bufferid, 0, buffer, 0, bufferid.Length);
                        buffer[bufferid.Length] = (byte)(BOOLEAN_FALSE + (bvalue ? 1 : 0));
                        break;
                    }
                case ZomBDataTypeHint.Lookup:
                    return;
                case ZomBDataTypeHint.Unknown:
                default:
                    return;
            }
            sendQ.Enqueue(buffer);
            if (Qlock.WaitOne(0))
            {
                try
                {
                    while (sendQ.Count > 0)
                    {
                        byte[] sendPop = sendQ.Dequeue();
                        this.nstream.Write(sendPop, 0, sendPop.Length);
                    }
                }
                finally
                {
                    Qlock.Release();
                }
            }
            
        }

        private void SillySendTableLink(uint tableid)
        {
            byte[] bufferid = TableIDToBytes(tableid);
            byte[] buffer = new byte[1 + (bufferid.Length * 2)];
            buffer[0] = TABLE_ASSIGNMENT;
            Buffer.BlockCopy(bufferid, 0, buffer, 1, bufferid.Length);
            Buffer.BlockCopy(bufferid, 0, buffer, 1 + bufferid.Length, bufferid.Length);

            
            this.nstream.Write(buffer, 0, buffer.Length);
        }

        #endregion

        /// <summary>
        /// The background worker. Will exit after 10 consectutive errors
        /// </summary>
        private void DoWork()
        {
            Qlock.WaitOne();
            //number of errors
            int nume = 0;
            while (isrunning)
            {
                try
                {
                    cRIOConnection = new TcpClient(Hostname, port);
                    NetworkStream stream = cRIOConnection.GetStream();
                    this.nstream = stream;
                    stream.WriteByte(PING);
                    cRIOConnection.NoDelay = true;

                    //Request Tables
                    stream.Write(SMART_REQUEST, 0, SMART_REQUEST.Length);

                    while (isrunning)
                    {
                        stream.WriteByte(PING);
                        Qlock.Release();
                        int b = stream.ReadByte();
                        Qlock.WaitOne();
                        switch (b)
                        {
                            case -1://closed
                                isrunning = false;
                                return;
                            case PING:
                                stream.WriteByte(PING);
                                while (sendQ.Count > 0)
                                {
                                    byte[] sendPop = sendQ.Dequeue();
                                    stream.Write(sendPop, 0, sendPop.Length);
                                }
                                break;
                            case TABLE_ASSIGNMENT:
                                {
                                    //table(local), table(remote-table-number)
                                    byte[] buffer = ReadID(stream, TABLE_ID_MAGIC_NUMBER);
                                    uint tableid = BytesToTableID(buffer);

                                    //if we have the first table, link with the old
                                    if (tableid == 0)
                                    {
                                        buffer = ReadID(stream, TABLE_ID_MAGIC_NUMBER);
                                        remoteTable = BytesToTableID(buffer);
                                        remoteTableBytes = TableIDToBytes(remoteTable);//do it fresh to avoid extra empty bytes
                                        tables[remoteTable] = kys;
                                    }
                                    else
                                    {
                                        //this.SillySendTableLink(tableid);
                                        buffer = ReadID(stream, TABLE_ID_MAGIC_NUMBER);
                                        throw new NotImplementedException("Argh! Throw 'er in the debugger, laddie!");
                                    }
                                    break;
                                }
                            case ASSIGNMENT:
                                {
                                    //tableid(remote), string(name), fieldid(number)
                                    byte[] buffer = ReadID(stream, TABLE_ID_MAGIC_NUMBER);
                                    uint tableid = BytesToTableID(buffer);
                                    bool master = (tableid == remoteTable);

                                    string fname = ReadNetString(stream);

                                    buffer = ReadID(stream, FIELD_ID_MAGIC_NUMBER);
                                    uint fid = BytesToFieldID(buffer);

                                    if (fname == null || fname == "")
                                    {
                                        throw new Exception("INVALID NAME");
                                    }

                                    nametable.AddOrUpdate(fid, fname);
                                    typetable.AddOrUpdate(fid, ZomBDataTypeHint.Unknown);
                                    if (!tables.ContainsKey(tableid))//gotta create it
                                    {
                                        ZomBDataLookup dict = new ZomBDataLookup();
                                        tables.AddOrUpdate(tableid, dict);
                                        tablesbyfield.AddOrUpdate(fid, dict);
                                        this.SillySendTableLink(tableid);
                                    }
                                    else //only gotta assign the fid
                                    {
                                        tablesbyfield.AddOrUpdate(fid, tables[tableid]);
                                    }

                                    //Send it back so sending works...
                                    buffer = TableIDToBytes(master ? 0 : tableid);
                                    byte[] netstring = CreateNetString(fname);
                                    byte[] idbuff = FieldIDToBytes(fid);
                                    byte[] outbuffer = new byte[netstring.Length + idbuff.Length + 1 + buffer.Length];
                                    outbuffer[0] = ASSIGNMENT;
                                    Buffer.BlockCopy(buffer, 0, outbuffer, 1, buffer.Length);
                                    Buffer.BlockCopy(netstring, 0, outbuffer, buffer.Length + 1, netstring.Length);
                                    Buffer.BlockCopy(idbuff, 0, outbuffer, netstring.Length + buffer.Length + 1, idbuff.Length);
                                    stream.Write(outbuffer, 0, outbuffer.Length);
                                    AlertPeek(fid, true);
                                    break;
                                }
                            case TRANSACTION:
                                {
                                    do
                                    {
                                        this.ReadUpdate(b, stream, false);
                                        b = stream.ReadByte();
                                    } while (b != TRANSACTION);
                                    stream.WriteByte((byte)CONFIRMATION + 1);
                                    //Fire events
                                    if (DataRecieved != null)
                                        DataRecieved(this, new EventArgs());
                                    if (NewDataRecieved != null)
                                        NewDataRecieved(this, new NewDataRecievedEventArgs(kys));
                                }
                                break;
                            case OLD_DATA:
                                //What? we are not a server. If you feel like implementing this, please do.
                                break;
                            default:
                                {
                                    if ((b & FIELD_ID) == FIELD_ID)//update things
                                    {
                                        this.ReadUpdate(b, stream);
                                        stream.WriteByte((byte)CONFIRMATION + 1);
                                    }
                                    else if ((b & CONFIRMATION) == CONFIRMATION)//Great!
                                    {
                                        //TODO: confirm
                                    }
                                    else if ((b & DENIAL) == DENIAL)//UMM...
                                    {
                                        //TODO: Confirm something...
                                    }
                                    break;
                                }
                        }
                    }
                    if (nume > 0)
                        nume--;
                }
                catch (ThreadAbortException)
                {
                    isrunning = false;
                    cRIOConnection.Close();
                    return;
                }
                catch (Exception ex)
                {
                    try
                    {
                        Qlock.Release();
                    }
                    catch
                    {}
                    Qlock.WaitOne(1);
                    nume++;
                    try
                    {
                        cRIOConnection.Close();
                    }
                    catch { }
                    try
                    {
                        cRIOConnection = null;
                    }
                    catch { }
                    if (nume > 1)
                    {
                        isrunning = false;
                        DoError(new Exception(ex.Message + "\r\n2 consecutive errors were encountered, stopping NetTable", ex));
                        isrunning = false;
                        return;
                    }
                    else
                        DoError(ex);
                }
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
                if (ddh == null)
                    System.Windows.Forms.MessageBox.Show(ex.ToString());
                else
                    ddh.DoError(ex);
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
            return new ZomBUrlInfo { DefaultPort = NTPort };
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

    public static class NetTableDictionaryExtendos
    {
        public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
        }
    }
}
