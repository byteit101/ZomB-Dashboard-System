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
    [DataSource("DBPacket")]
    [DataSource("DBPkt")]
    public class DashboardPacketDataSource : IDashboardDataSource, IDashboardStatusDataSource, IDashboardDataDataSource, IDashboardPeekableDataSource
    {
        public const int DBPacketPort = 1165;
        UdpClient cRIOConnection;
        bool isrunning;
        Thread backThread;
        DashboardDataHub ddh;

        FRCDSStatus cStat = new FRCDSStatus();
        Dictionary<string, string> kys = new Dictionary<string, string>();

        public DashboardPacketDataSource(IZomBController ddh)
        {
            this.ddh = ddh.GetDashboardDataHub();
        }

        public DashboardPacketDataSource(ZomBUrl info, IZomBController ddh)
        {
            if (ddh != null)
                this.ddh = ddh.GetDashboardDataHub();
        }

        ~DashboardPacketDataSource()
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
                    string Output;

                    //Convert
                    Output = UTF8Encoding.UTF8.GetString(buffer);

                    //Find segment of data
                    if (Output.Contains("@@ZomB:|") && Output.Contains("|:ZomB@@") && !Output.Contains("@@ZomB:|:ZomB@@"))
                    {
                        Output = Output.Substring(Output.IndexOf("@@ZomB:|") + 8, (Output.IndexOf("|:ZomB@@") - (Output.IndexOf("@@ZomB:|") + 8)));
                        if (Output != "")
                        {
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

                            //Get the items in a dictionary
                            Dictionary<string, string> vals = SplitParams(Output);
                            if (peeking)
                            {
                                foreach (var keys in vals)
                                {
                                    dp.Invoke(cb, keys.Key);
                                }
                            }
                            else
                            {
                                FRCDSStatus status = ParseDSBytes(buffer);
                                cStat = status;
                                kys = vals;

                                //Fire events
                                if (DataRecieved != null)
                                    DataRecieved(this, new EventArgs());
                                if (NewStatusRecieved != null)
                                    NewStatusRecieved(this, new NewStatusRecievedEventArgs(status));
                                if (NewDataRecieved != null)
                                    NewDataRecieved(this, new NewDataRecievedEventArgs(vals));
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
                    nume++;
                    DoError(ex);
                    if (nume > 10)
                    {
                        isrunning = false;
                        DoError(new Exception("10 consecutive errors were encountered, stopping DashboardDataHub"));
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
        /// Convert the name=value|n=v form to a Dictionary of name and values
        /// </summary>
        /// <param name="Output"></param>
        /// <returns></returns>
        private Dictionary<string, string> SplitParams(string Output)
        {
            //Split the main string
            string[] s = Output.Split('|');
            Dictionary<string, string> k = new Dictionary<string, string>(s.Length);
            foreach (string item in s)
            {
                //split and add each item to the Dictionary
                string ky, val;
                ky = item.Split('=')[0];
                val = item.Split('=')[1];
                k[ky] = val;//Latter will overwrite
            }
            return k;
        }

        /// <summary>
        /// Processes any errors that may have been encountered, and either fires the OnError, or Alerts the user
        /// </summary>
        /// <param name="ex">The error</param>
        internal void DoError(Exception ex)
        {
            if (OnError == null)
                ddh.DoError(ex);
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
}
