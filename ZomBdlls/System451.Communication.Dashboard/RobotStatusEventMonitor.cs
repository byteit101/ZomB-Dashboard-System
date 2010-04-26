using System;
using System.Collections.Generic;
using System.Text;

namespace System451.Communication.Dashboard
{
    class RobotStatusEventMonitor : IZomBMonitor
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


        public RobotStatusEventMonitor()
        {
            EnableEvents = false;
        }
        public RobotStatusEventMonitor(bool enableEvents)
        {
            EnableEvents = enableEvents;
        }

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

        void IZomBMonitor.UpdateData(Dictionary<string, string> data, byte[] packetData)
        {
            //Nothing important happens here
        }

        #endregion
    }
}
