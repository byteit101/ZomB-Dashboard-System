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
using System.IO;
using System.Text;
using System.Threading;

namespace System451.Communication.Dashboard.Net
{
    /// <summary>
    /// DataPlayerSource "plays" the data saved by DataSaver
    /// </summary>
    [DataSource("File")]
    public class DataPlayerSource : IDashboardDataSource, IDashboardStatusDataSource, IDashboardDataDataSource
    {
        ZomBDataLookup vls = new ZomBDataLookup();
        FRCDSStatus sts = new FRCDSStatus();
        Thread workthread;
        BinaryReader redbit;

        /// <summary>
        /// Create a new DataPlayerSource from the specified file
        /// </summary>
        /// <param name="file">The file name</param>
        public DataPlayerSource(string file)
        {
            FilePath = file;
        }

        internal DataPlayerSource(ZomBUrl url)
        {
            FilePath = url.Path.Substring(1);//remove first char
        }

        /// <summary>
        /// The file to read
        /// </summary>
        public string FilePath
        {
            get;
            set;
        }

        /// <summary>
        /// Are we playing
        /// </summary>
        public bool Running
        {
            get;
            private set;
        }

        private void worker()
        {
            char c;
            ushort us;
            int iv;
            while (Running)
            {
                try
                {
                    c = (char)redbit.ReadByte();
                    switch (c)
                    {
                        case 'T':
                            iv = (((int)redbit.ReadByte()) << 8) + redbit.ReadByte();
                            while (Running && iv > 1)//takes some time to read the next one
                            {
                                Thread.Sleep(2);
                                iv -= 2;
                            }
                            break;
                        case 'S':
                            sts = new FRCDSStatus();
                            sts.Status = new StatusBitField(redbit.ReadByte());
                            sts.Error = new ErrorBitField(redbit.ReadByte());
                            sts.DigitalIn = new DIOBitField(redbit.ReadByte());
                            sts.DigitalOut = new DIOBitField(redbit.ReadByte());
                            sts.PacketNumber = (ushort)((((int)redbit.ReadByte()) << 8) + redbit.ReadByte());
                            sts.Battery = redbit.ReadByte() + (((int)redbit.ReadByte()) / 100);
                            if (NewStatusRecieved != null)
                                NewStatusRecieved(this, new NewStatusRecievedEventArgs(sts));
                            break;
                        case 'D':
                            iv = (((int)redbit.ReadByte()) << 8) + redbit.ReadByte();
                            byte[] buffer = new byte[iv];
                            us = 0;
                            us = (ushort)redbit.Read(buffer, 0, iv);
                            while (us != iv)
                            {
                                us += (ushort)redbit.Read(buffer, us, iv - us);
                            }
                            vls = SplitParams(UTF8Encoding.UTF8.GetString(buffer));
                            if (DataRecieved != null)
                                DataRecieved(this, new EventArgs());
                            if (NewDataRecieved != null)
                                NewDataRecieved(this, new NewDataRecievedEventArgs(vls));
                            break;
                        case 'Q':
                            return;
                        default:
                            if (OnError != null)
                                OnError(this, new ErrorEventArgs(new Exception("missed command statements")));
                            break;
                    }
                }
                catch (Exception ex)
                {
                    if (OnError != null)
                        OnError(this, new ErrorEventArgs(ex));
                }
            }
        }

        /// <summary>
        /// Convert the name=value|n=v form to a Dictionary of name and values
        /// </summary>
        /// <param name="Output"></param>
        /// <returns></returns>
        private ZomBDataLookup SplitParams(string Output)
        {
            //Split the main string
            string[] s = Output.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            ZomBDataLookup k = new ZomBDataLookup(s.Length);
            foreach (string item in s)
            {
                //split and add each item to the Dictionary
                string ky, val;
                ky = item.Split('=')[0];
                val = item.Split('=')[1];
                k[ky] = new ZomBDataObject(val, ZomBDataTypeHint.Unknown);//Latter will overwrite
            }
            return k;
        }


        #region IDashboardDataSource Members

        void IDashboardDataSource.Start()
        {
            if (!Running)
            {
                redbit = new BinaryReader(File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read));
                workthread = new Thread(worker);
                workthread.IsBackground = true;
                workthread.Start();
                Running = true;
            }
        }

        void IDashboardDataSource.Stop()
        {
            if (Running)
            {
                try
                {

                }
                finally
                {
                    Running = false;
                }
            }
        }

        bool IDashboardDataSource.HasStatus
        {
            get { return true; }
        }

        bool IDashboardDataSource.HasData
        {
            get { return true; }
        }

        IDashboardStatusDataSource IDashboardDataSource.GetStatusSource()
        {
            return this;
        }

        IDashboardDataDataSource IDashboardDataSource.GetDataSource()
        {
            return this;
        }

        public event EventHandler DataRecieved;

        event InvalidPacketRecievedEventHandler IDashboardDataSource.InvalidPacketRecieved
        {
            add { }
            remove { }
        }

        public event ErrorEventHandler OnError;

        #endregion

        #region IDashboardDataDataSource Members

        ZomBDataLookup IDashboardDataDataSource.GetData()
        {
            return vls;
        }

        IDashboardDataSource IDashboardDataDataSource.ParentDataSource
        {
            get { return this; }
        }

        public event NewDataRecievedEventHandler NewDataRecieved;

        #endregion

        #region IDashboardStatusDataSource Members

        FRCDSStatus IDashboardStatusDataSource.GetStatus()
        {
            return sts;
        }

        IDashboardDataSource IDashboardStatusDataSource.ParentDataSource
        {
            get { return this; }
        }

        public event NewStatusRecievedEventHandler NewStatusRecieved;

        #endregion

        /// <summary>
        /// Magic method for zomb:// urls
        /// </summary>
        /// <returns></returns>
        private static ZomBUrlInfo GetZomBUrlInfo()
        {
            return new ZomBUrlInfo { DefaultPort = 0 };
        }
    }
    //Format: T(ushort)[timespan]S[data]D(ushort)[length][data]
}
