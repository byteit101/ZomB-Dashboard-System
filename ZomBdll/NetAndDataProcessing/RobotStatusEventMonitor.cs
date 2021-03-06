﻿/*
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

namespace System451.Communication.Dashboard.Utils
{
    /// <summary>
    /// This class monitors for changes in the status of the Robot
    /// </summary>
    public class RobotStatusEventMonitor : IZomBMonitor
    {
        StatusBitField stat;
        ErrorBitField ebf;
        DIOBitField indio, outdio;
        int team;
        float battery;
        DateTime matchStart;

        public event EventHandler NewPacketRecieved;
        public event EventHandler PacketLost;
        public event EventHandler Enabled;
        public event EventHandler Disabled;
        public event EventHandler EStopped;
        public event EventHandler FMSAttached;
        public event EventHandler RobotConnected;
        public event EventHandler TelopBegin;
        public event EventHandler TelopEnd;
        public event EventHandler AutoBegin;
        public event EventHandler AutoEnd;
        public event EventHandler MatchStarted;
        public event EventHandler MatchEnded;
        public event EventHandler MatchAborted;

        /// <summary>
        /// Create a gnu RobotStatusEventMonitor
        /// </summary>
        public RobotStatusEventMonitor()
        {
            EnableEvents = false;
        }

        /// <summary>
        /// Create a gnu RobotStatusEventMonitor
        /// </summary>
        /// <param name="enableEvents">Should we enable the events? Default false.
        /// Setting this to true allows you to bind to the events, but causes an increase in overhead for each packet</param>
        public RobotStatusEventMonitor(bool enableEvents)
        {
            EnableEvents = enableEvents;
        }

        /// <summary>
        /// Should we enable the events?
        /// Setting this to true allows you to bind to the events, but causes an increase in overhead for each packet
        /// </summary>
        public bool EnableEvents
        {
            get;
            set;
        }

        public int TeamNumber
        {
            get
            {
                return team;
            }
        }

        public float Battery
        {
            get
            {
                return battery;
            }
        }

        public bool IsAutonomous
        {
            get
            {
                return stat.Auto;
            }
        }

        public bool IsAuto
        {
            get
            {
                return stat.Auto;
            }
        }

        public bool IsOperatorControlled
        {
            get
            {
                return !stat.Auto;
            }
        }

        public bool IsTelop
        {
            get
            {
                return !stat.Auto;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return stat.Enabled;
            }
        }

        public bool IsDisabled
        {
            get
            {
                return !stat.Enabled;
            }
        }

        public bool IsEStopped
        {
            get
            {
                return stat.EmergencyStopped;
            }
        }

        public bool IsRobotConnected
        {
            get
            {
                return stat.RobotAttached;
            }
        }

        public bool IsFMSConnected
        {
            get
            {
                return stat.FMSAttached;
            }
        }

        public bool DSPacketLost
        {
            get
            {
                return ebf.PacketLost;
            }
        }

        public bool cRIOVersionMismatch
        {
            get
            {
                return ebf.cRIOVersionMismatch;
            }
        }

        public bool DSVersionError
        {
            get
            {
                return ebf.DSVersionError;
            }
        }

        public bool TeamMismatch
        {
            get
            {
                return ebf.TeamMismatch;
            }
        }

        public bool FPGAVersionMismatch
        {
            get
            {
                return ebf.FPGAVersionMismatch;
            }
        }

        public DIOBitField DigitalOut
        {
            get
            {
                return outdio;
            }
        }

        public DIOBitField DigitalIn
        {
            get
            {
                return indio;
            }
        }

        #region IZomBMonitor Members

        void IZomBMonitor.UpdateStatus(FRCDSStatus status)
        {
            if (EnableEvents)
            {
                EventArgs e = new EventArgs();

                if (NewPacketRecieved != null)
                    NewPacketRecieved(this, e);
                if (status.Error.PacketLost && PacketLost != null)
                    PacketLost(this, e);
                //RobotCom
                if (status.Status.RobotAttached)
                {
                    if (!stat.RobotAttached && RobotConnected != null)
                        RobotConnected(this, e);
                    //Enable
                    if (status.Status.Enabled && !stat.Enabled)
                    {
                        if (Enabled != null)
                            Enabled(this, e);
                        //Begin Auto
                        if (status.Status.Auto)
                        {
                            if (AutoBegin != null)
                                AutoBegin(this, e);
                            if (status.Status.FMSAttached)
                            {
                                matchStart = new DateTime(DateTime.Now.Ticks);
                                if (MatchStarted != null)
                                    MatchStarted(this, e);
                            }
                        }
                        //Begin Telop
                        else
                        {
                            if (TelopBegin != null)
                                TelopBegin(this, e);
                            if (status.Status.FMSAttached)
                            {
                                //Give Leeway
                                if (DateTime.Now.Subtract(matchStart).Seconds >= 14 && DateTime.Now.Subtract(matchStart).Seconds <= 18)
                                {
                                }
                                //uh oh!
                                else
                                {
                                    if (MatchAborted != null)
                                        MatchAborted(this, e);
                                }
                            }
                        }
                    }
                    //Disable
                    if (!status.Status.Enabled && stat.Enabled)
                    {
                        if (Disabled != null)
                            Disabled(this, e);
                        //end Auto
                        if (status.Status.Auto)
                        {
                            if (AutoEnd != null)
                                AutoEnd(this, e);
                            if (status.Status.FMSAttached)
                            {
                                //Give Leeway
                                if (DateTime.Now.Subtract(matchStart).Seconds >= 14 && DateTime.Now.Subtract(matchStart).Seconds <= 18)
                                {
                                }
                                //uh oh!
                                else
                                {
                                    if (MatchAborted != null)
                                        MatchAborted(this, e);
                                }
                            }
                        }
                        //end Telop
                        else
                        {
                            if (TelopEnd != null)
                                TelopEnd(this, e);
                            if (status.Status.FMSAttached)
                            {
                                //Give Leeway
                                if (DateTime.Now.Subtract(matchStart).Seconds >= 14 && DateTime.Now.Subtract(matchStart).Seconds <= 18
                                    && DateTime.Now.Subtract(matchStart).Minutes == 2)
                                {
                                    if (MatchEnded != null)
                                        MatchEnded(this, e);
                                }
                                //uh oh!
                                else
                                {
                                    if (MatchAborted != null)
                                        MatchAborted(this, e);
                                }
                            }
                        }
                    }
                    if (status.Status.EmergencyStopped && !stat.EmergencyStopped && EStopped != null)
                        EStopped(this, e);
                    if (status.Status.FMSAttached && !stat.FMSAttached && FMSAttached != null)
                        FMSAttached(this, e);
                }
            }
            stat.Byte = status.Status.Byte;
            ebf.Byte = status.Error.Byte;
            indio.Byte = status.DigitalIn.Byte;
            outdio.Byte = status.DigitalOut.Byte;
            team = status.Team;
            battery = status.Battery;
        }

        void IZomBMonitor.UpdateData(ZomBDataLookup data)
        {
            //Nothing important happens here
        }

        #endregion
    }
}
