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

namespace System451.Communication.Dashboard.Net
{
    [DataSource("Smart")]
    public class SmartDataSource : IDashboardDataSource, IDashboardStatusDataSource, IDashboardDataDataSource, IDashboardPeekableDataSource
    {
        public const int DBPacketPort = 1165;
        UdpClient cRIOConnection;
        bool isrunning;
        Thread backThread;
        DashboardDataHub ddh;

        FRCDSStatus cStat = new FRCDSStatus();
        Dictionary<string, string> kys = new Dictionary<string, string>();
        Dictionary<int, SmartInfo> nametable = new Dictionary<int, SmartInfo>();

        public SmartDataSource(IZomBController ddh)
        {
            this.ddh = ddh.GetDashboardDataHub();
        }

        public SmartDataSource(ZomBUrl info, IZomBController ddh)
        {
            if (ddh != null)
                this.ddh = ddh.GetDashboardDataHub();
        }

        ~SmartDataSource()
        {
            Stop();
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
                        cRIOConnection = new UdpClient(DBPacketPort);
                    }
                    catch (Exception ex)
                    {
                        DoError(ex);
                        return;
                    }
                }
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
            get { return true; }
        }

        public bool HasData
        {
            get { return true; }
        }

        public IDashboardStatusDataSource GetStatusSource()
        {
            return this;
        }

        public IDashboardDataDataSource GetDataSource()
        {
            return this;
        }

        public event EventHandler DataRecieved;

        public event InvalidPacketRecievedEventHandler InvalidPacketRecieved;

        public event ErrorEventHandler OnError;

        #endregion

        #region IDashboardStatusDataSource Members

        public FRCDSStatus GetStatus()
        {
            return cStat;
        }

        public IDashboardDataSource ParentDataSource
        {
            get { return this; }
        }

        public event NewStatusRecievedEventHandler NewStatusRecieved;

        #endregion

        #region IDashboardDataDataSource Members

        public Dictionary<string, string> GetData()
        {
            return kys;
        }

        public event NewDataRecievedEventHandler NewDataRecieved;

        #endregion

        /// <summary>
        /// The background worker. Will exit after 10 consectutive errors
        /// </summary>
        private void DoWork()
        {
            //number of errors
            int nume = 0;
            while (isrunning)
            {
                try
                {
                    IPEndPoint RIPend = null;
                    //Recieve the data
                    while (cRIOConnection.Available < 1018 && isrunning) { Thread.Sleep(2); }
                    if (!isrunning)
                    {
                        cRIOConnection.Close();
                        return;
                    }
                    byte[] buffer = cRIOConnection.Receive(ref RIPend);

                    //Check first
                    if (!VerifyPacket(buffer))
                    {
                        if (InvalidPacketRecieved != null && !peeking)
                        {
                            //Create our e
                            InvalidPacketRecievedEventArgs e = new InvalidPacketRecievedEventArgs(buffer, ddh.InvalidPacketAction == InvalidPacketActions.AlwaysContinue || ddh.InvalidPacketAction == InvalidPacketActions.Continue);
                            InvalidPacketRecieved(this, e);
                            if ((int)ddh.InvalidPacketAction < 3)//1-4
                            {
                                if (!e.ContinueAnyway)
                                    break;
                            }
                            else if (ddh.InvalidPacketAction == InvalidPacketActions.AlwaysIgnore)
                                break;
                        }
                    }

                    //DS Status
                    cStat = ParseDSBytes(buffer);

                    //Get a stream
                    MemoryStream Output = new MemoryStream(buffer);
                    Output.Seek(27, SeekOrigin.Begin);
                    EBinaryReader binladen = new EBinaryReader(Output);

                    //get length
                    int totallength = (binladen.ReadUInt16() << 16);
                    totallength += binladen.ReadUInt16();

                    //clear last loop's controls
                    kys = new Dictionary<string, string>();

                    //loop all controls
                    while (totallength > (Output.Position - 27))
                    {
                        byte b = binladen.ReadByte();
                        switch (b)
                        {
                            case 0://Announce
                                {
                                    int id = binladen.ReadByte();
                                    SmartDataTypes type = (SmartDataTypes)binladen.ReadByte();
                                    int len = binladen.ReadUInt16();
                                    byte[] name = new byte[len];
                                    binladen.Read(name, 0, len);
                                    if (peeking)
                                    {
                                        dp.Invoke(cb, Encoding.UTF8.GetString(name));
                                        nametable[id] = new SmartInfo { Name = Encoding.UTF8.GetString(name), Type = type };
                                    }
                                    else
                                    {
                                        nametable[id] = new SmartInfo { Name = Encoding.UTF8.GetString(name), Type = type };
                                    }
                                    break;
                                }
                            case 1://Update
                                {
                                    int id = binladen.ReadByte();
                                    if (nametable.ContainsKey(id))
                                    {
                                        var info = nametable[id];
                                        string value = info.Parse(binladen);
                                        kys[info.Name] = value;
                                    }
                                    else
                                    {
                                        b = binladen.ReadByte();//get length
                                        binladen.Read(new byte[b], 0, b);//skip it
                                    }
                                    break;
                                }
                            case 2://GUI Announce, not impl
                                {
                                    System.Windows.Forms.MessageBox.Show("Dashboard selection via Protocol has not been implemented in ZomB, try WPF tabs instead");
                                    break;
                                }
                            default:
                                throw new Exception("Bin ("+b+") not a 1 or zero, exiting");

                                break;
                        }
                    }

                    if (kys.Count > 0)//Empty keys in empty packet, "Smart" Dashboard can be dumb
                        //All in favor of chaging the name to DumbDashboard say "aye!" "sqrt(-1)!"
                    {
                        //Fire events
                        if (DataRecieved != null)
                            DataRecieved(this, new EventArgs());
                        if (NewStatusRecieved != null)
                            NewStatusRecieved(this, new NewStatusRecievedEventArgs(cStat));
                        if (NewDataRecieved != null)
                            NewDataRecieved(this, new NewDataRecievedEventArgs(kys));
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
                    nume++;
                    DoError(ex);
                    if (nume > 10)
                    {
                        isrunning = false;
                        DoError(new Exception("10 consecutive errors were encountered, stopping Smart"));
                        isrunning = false;
                        try
                        {
                            cRIOConnection.Close();
                        }
                        catch { }
                        return;
                    }
                }
            }
        }



        /// <summary>
        /// Convert the DS Bytes to a FRCDSStatus
        /// </summary>
        /// <param name="buffer">The bytes from the Robot packet</param>
        /// <returns>A FRCDSStatus containg the robot status</returns>
        static protected FRCDSStatus ParseDSBytes(byte[] buffer)
        {
            //TODO: Find and Fix errors here
            FRCDSStatus ret = new FRCDSStatus();
            ret.PacketNumber = buffer[0];
            ret.PacketNumber += (ushort)(buffer[1] << 8);
            ret.DigitalIn = new DIOBitField(buffer[2]);
            ret.DigitalOut = new DIOBitField(buffer[3]);
            ret.Battery = float.Parse(buffer[4].ToString("x") + "." + buffer[5].ToString("x"));
            ret.Status = new StatusBitField(buffer[6]);
            ret.Error = new ErrorBitField(buffer[7]);
            ret.Team = (int)(buffer[8] * 100) + (int)(buffer[9]);

            //there's got to be a better way to do this
            //TODO: This does not work DS Bytes
            //int Month = int.Parse(new string(new char[] { (char)buffer[10], (char)buffer[11] }));
            //int Day = int.Parse(new string(new char[] { (char)buffer[12], (char)buffer[13] }));
            //int year = 2000 + int.Parse(new string(new char[] { (char)buffer[14], (char)buffer[15] }));

            //ret.Version = new DateTime(year, Month, Day);
            //ret.Revision = (ushort)((ushort)(buffer[16]) + ((ushort)(buffer[17] >> 8)));

            return ret;
        }

        /// <summary>
        /// Verifies the DS packet was transmitted successfully
        /// </summary>
        /// <param name="data">The Packet Data</param>
        /// <returns>Is the packet valid?</returns>
        /// <remarks>
        /// Thanks to EHaskins for the bulk of this function (http://www.chiefdelphi.com/forums/showpost.php?p=955762&postcount=4)
        /// </remarks>
        static protected bool VerifyPacket(byte[] data)
        {
            if (data.Length != 1018)
                return false;

            uint calculatedCrc = 0;
            uint dataCrc = 0;
            Libs.Crc32 crc = new Libs.Crc32();

            dataCrc = BitConverter.ToUInt32(data, data.Length - 4);

            //remove CRC bytes from data before calculating CRC.
            byte[] crcData = new byte[data.Length];
            Buffer.BlockCopy(data, 0, crcData, 0, data.Length - 4);

            calculatedCrc = BitConverter.ToUInt32(crc.ComputeHash(crcData), 0);

            return (dataCrc == calculatedCrc);
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
            return new ZomBUrlInfo { DefaultPort = 1165 };
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

    public enum SmartDataTypes : byte
    {
        None = 0xFE,
        Byte = 0,
        Char = 1,
        Int = 2,
        Long = 3,
        Short = 4,
        Float = 5,
        Double = 6,
        String = 7,
        Bool = 8,
        UTF8String = 9
    }

    public class SmartInfo
    {
        public SmartDataTypes Type { get; set; }
        public string Name { get; set; }

        public string Parse(EBinaryReader binreader)
        {
            switch (Type)
            {
                case SmartDataTypes.Byte:
                case SmartDataTypes.Char:
                case SmartDataTypes.Int:
                case SmartDataTypes.Long:
                case SmartDataTypes.Short:
                case SmartDataTypes.Float:
                case SmartDataTypes.Double:
                case SmartDataTypes.String:
                case SmartDataTypes.Bool:
                case SmartDataTypes.UTF8String:
                    binreader.ReadByte();
                    break;
                default:
                    break;
            }
            switch (Type)
            {
                case SmartDataTypes.Byte:
                    return binreader.ReadByte().ToString();
                case SmartDataTypes.Char:
                    binreader.ReadByte();//I don't know why, just do it (always '\0')
                    return binreader.ReadChar().ToString();
                case SmartDataTypes.Int:
                    return binreader.ReadInt32().ToString();
                case SmartDataTypes.Long:
                    return binreader.ReadInt64().ToString();
                case SmartDataTypes.Short:
                    return binreader.ReadInt16().ToString();
                case SmartDataTypes.Float:
                    return binreader.ReadSingle().ToString();
                case SmartDataTypes.Double:
                    return binreader.ReadDouble().ToString();
                case SmartDataTypes.String:
                    return binreader.ReadString();
                case SmartDataTypes.Bool:
                    return binreader.ReadByte().ToString();
                case SmartDataTypes.UTF8String:
                    return binreader.ReadUTFString();
                default:
                    return "";
            }
        }
    }
}
