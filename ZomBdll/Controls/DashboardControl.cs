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
using System.ComponentModel;
using System.Windows.Forms;
using System451.Communication.Dashboard.Utils;
using System.Collections.ObjectModel;

namespace System451.Communication.Dashboard
{
    /// <summary>
    /// Defines a ZomB Dashboard System Controller
    /// </summary>
    public interface IZomBController
    {
        /// <summary>
        /// Gets the DashboardDataHub associated with this controller
        /// </summary>
        /// <returns>The DashboardDataHub associated with this controller</returns>
        DashboardDataHub GetDashboardDataHub();
    }

    /// <summary>
    /// EventArgs for the ControlAdded event in IZomBControl
    /// </summary>
    public class ZomBControlAddedEventArgs : EventArgs
    {
        IZomBController controller;
        /// <summary>
        /// Create a new ZomBControlAddedEventArgs
        /// </summary>
        /// <param name="Controller">The controller that contains the DashboardDataHub that fired this event</param>
        public ZomBControlAddedEventArgs(IZomBController Controller)
        {
            this.controller = Controller;
        }

        /// <summary>
        /// Get the Controller that contains the DashboardDataHub that fired this event
        /// </summary>
        public IZomBController Controller
        {
            get
            {
                return controller;
            }
        }
    }

    /// <summary>
    /// Used by IZomBControl.ControlAdded to notify when a control is added to the DashboardDataHub
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">The event data</param>
    public delegate void ControlAddedDelegate(object sender, ZomBControlAddedEventArgs e);

    /// <summary>
    /// Defines the framework of a ZomB Dashboard System control
    /// </summary>
    public interface IZomBControl
    {
        /// <summary>
        /// The name of the control. This is the name you send values to.
        /// </summary>
        string ControlName { get; }

        /// <summary>
        /// Updates the control with new data
        /// </summary>
        /// <param name="value">The new value of the control.</param>
        void UpdateControl(ZomBDataObject value);

        /// <summary>
        /// Notifies when this control is added to a DashboardDataHub
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The Arrrghs</param>
        void ControlAdded(object sender, ZomBControlAddedEventArgs e);
    }

    /// <summary>
    /// A default implementation of the IZomBControl so you don't have to do boring work
    /// </summary>
    public class ZomBControl : Control, IZomBControl
    {
        DashboardDataHub localDDH;

        /// <summary>
        /// Creates the ZomBControl
        /// </summary>
        protected ZomBControl()
        {
            ControlAdded += new ControlAddedDelegate(ZomBControl_ControlAdded);
        }

        #region IZomBControl Members

        /// <summary>
        /// The control name. This needs to be implemented.
        /// </summary>
        [Category("ZomB"), Description("What this control will get the value of from the packet Data")]
        virtual public string ControlName
        {
            get;
            set;
        }

        /// <summary>
        /// Updates the control. This needs to be implemented.
        /// </summary>
        /// <exception cref="System.NotImplementedException">Always throws NotImplementedException</exception>
        virtual public void UpdateControl(ZomBDataObject value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When this control is added to a DashboardDataHub
        /// </summary>
        new public event ControlAddedDelegate ControlAdded;

        void IZomBControl.ControlAdded(object sender, ZomBControlAddedEventArgs e)
        {
            if (ControlAdded != null)
                ControlAdded(sender, e);
        }

        #endregion

        void ZomBControl_ControlAdded(object sender, ZomBControlAddedEventArgs e)
        {
            localDDH = e.Controller.GetDashboardDataHub();
        }

        /// <summary>
        /// Overide Text for easy values
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get
            {
                return ControlName;
            }
            set
            {
                ControlName = value;
            }
        }

        /// <summary>
        /// Gets the current DashboardDataHub
        /// </summary>
        [Browsable(false)]
        public DashboardDataHub LocalDashboardDataHub
        {
            get
            {
                return localDDH;
            }
        }
    }

    /// <summary>
    /// Defines a ZomB control with multiple, seperate, and dynamic controls
    /// </summary>
    public interface IZomBControlGroup
    {

        /// <summary>
        /// Notifies when this control is added to a DashboardDataHub
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The Arrrghs</param>
        void ControlAdded(object sender, ZomBControlAddedEventArgs e);

        /// <summary>
        /// Gets the controls in this group
        /// </summary>
        /// <returns>The controls as a collection</returns>
        ZomBControlCollection GetControls();
    }

    /// <summary>
    /// EventArgs for the ControlUpdated event in IZomBRemoteControl
    /// </summary>
    public class ZomBControlUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// Create a new ZomBControlUpdatedEventArgs
        /// </summary>
        /// <param name="value">The new value of the control.</param>
        public ZomBControlUpdatedEventArgs(ZomBDataObject value)
        {
            this.Value = value;
        }

        /// <summary>
        /// The new value of the control.
        /// </summary>
        public ZomBDataObject Value
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// Used by IZomBRemoteControl.ControlUpdated to notify when a control is updated by the the DashboardDataHub
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">The event data</param>
    public delegate void ControlUpdatedDelegate(object sender, ZomBControlUpdatedEventArgs e);

    /// <summary>
    /// Defines a remote ZomBControl. Use this for the ZomBControlCollection
    /// </summary>
    public interface IZomBRemoteControl : IZomBControl
    {
        /// <summary>
        /// Replaces the UpdateControl Method
        /// </summary>
        event ControlUpdatedDelegate ControlUpdated;
    }

    /// <summary>
    /// This is used to make simple having multiple ZomB controls on one physical control
    /// </summary>
    [DesignTimeVisible(false), Browsable(false)]
    public class ZomBRemoteControl : ZomBControl, IZomBRemoteControl
    {
        /// <summary>
        /// Creates a new ZomBRemoteControl
        /// </summary>
        public ZomBRemoteControl()
        {

        }

        /// <summary>
        /// The name of the control. This is the name you send values to.
        /// </summary>
        public override string ControlName
        {
            get;
            set;
        }

        /// <summary>
        /// Updates the control with new data
        /// </summary>
        /// <param name="value">The new value of the control.</param>
        public override void UpdateControl(ZomBDataObject value)
        {
            if (ControlUpdated != null)
                ControlUpdated(this, new ZomBControlUpdatedEventArgs(value));
        }

        #region IZomBRemoteControl Members

        /// <summary>
        /// Fires when the Control is updated via IZomBControl.UpdateControl
        /// </summary>
        public event ControlUpdatedDelegate ControlUpdated;

        #endregion
    }

    /// <summary>
    /// This is used to make simple having multiple ZomB controls on one physical control that have Data sending
    /// </summary>
    [DesignTimeVisible(false), Browsable(false)]
    public class ZomBRemoteDataControl : ZomBRemoteControl, IZomBDataControl
    {
        bool dcEnabled = false;
        /// <summary>
        /// Creates a new ZomBRemoteDataControl
        /// </summary>
        public ZomBRemoteDataControl()
        {

        }

        public event EventHandler DataControlEnabledChanged;

        public void SafeFireDataUpdated(string newname, string newvalue)
        {
            if (DataUpdated != null)
            {
                DataUpdated(this, new ZomBDataControlUpdatedEventArgs(newname, newvalue));
            }
        }

        #region IZomBDataControl Members

        public event ZomBDataControlUpdatedEventHandler DataUpdated;

        public bool DataControlEnabled
        {
            get
            {
                return dcEnabled;
            }
            set
            {
                if (dcEnabled != value)
                {
                    dcEnabled = value;
                    if (DataControlEnabledChanged != null)
                        DataControlEnabledChanged(this, new EventArgs());
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Easily manage a bunch of virtual or physical IZomBcontrols by name
    /// </summary>
    public class ZomBControlCollection : Collection<IZomBControl>
    {
        /// <summary>
        /// Create a new ZomBControlCollection
        /// </summary>
        public ZomBControlCollection()
        {

        }
    }

    /// <summary>
    /// Defines the interface for a ZomB Dashboard System monitor
    /// </summary>
    public interface IZomBMonitor
    {
        /// <summary>
        /// Updates the status of the robot
        /// </summary>
        /// <param name="status">The new robot status</param>
        void UpdateStatus(FRCDSStatus status);

        /// <summary>
        /// Updates with the new data from the robot
        /// </summary>
        /// <param name="data">A keyed collection of the name/value pairs of robot data</param>
        void UpdateData(ZomBDataLookup data);
    }

    /// <summary>
    /// Defines the FRC Robot to Dashboard user packet
    /// </summary>
    public struct FRCDSStatus
    {
        /// <summary>
        /// Gets or sets the packet number
        /// </summary>
        public ushort PacketNumber { get; set; }

        /// <summary>
        /// Gets or sets the digital in byte
        /// </summary>
        public DIOBitField DigitalIn { get; set; }

        /// <summary>
        /// Gets or sets the digital out byte
        /// </summary>
        public DIOBitField DigitalOut { get; set; }

        /// <summary>
        /// Gets or sets the battery voltage
        /// </summary>
        public float Battery { get; set; }

        /// <summary>
        /// Gets or sets the status byte
        /// </summary>
        public StatusBitField Status { get; set; }

        /// <summary>
        /// Gets or sets the error byte
        /// </summary>
        public ErrorBitField Error { get; set; }

        /// <summary>
        /// Gets or sets the team number
        /// </summary>
        public int Team { get; set; }

        /// <summary>
        /// Gets or sets the version date
        /// </summary>
        public DateTime Version { get; set; }

        /// <summary>
        /// Gets or sets the version revision
        /// </summary>
        public ushort Revision { get; set; }
    }

    /// <summary>
    /// BitField encapsulation for the FRCDSStatus DigitalIn and DigitalOut fields
    /// </summary>
    public struct DIOBitField
    {
        /// <summary>
        /// Internal BitField
        /// </summary>
        BitField mainData;

        /// <summary>
        /// Create a new DIOBitField with the specified initial value
        /// </summary>
        /// <param name="value">Initial value</param>
        public DIOBitField(byte value)
        {
            mainData = new BitField(value);
        }

        /// <summary>
        /// Create a new DIOBitField with the specified initial value
        /// </summary>
        /// <param name="value">Initial value</param>
        public DIOBitField(int value)
        {
            mainData = new BitField(value);
        }

        /// <summary>
        /// Get or set the value of this BitField
        /// </summary>
        public byte Byte
        {
            get { return mainData.Byte; }
            set { mainData.Byte = value; }
        }

        /// <summary>
        /// Get the internal BitField
        /// </summary>
        public BitField BitField
        {
            get { return mainData; }
        }

        /// <summary>
        /// Get or set the value of Digital Input/Output 1
        /// </summary>
        public bool DIO1
        {
            get
            {
                return mainData[0];
            }
            set
            {
                mainData[0] = value;
            }
        }

        /// <summary>
        /// Get or set the value of Digital Input/Output 2
        /// </summary>
        public bool DIO2
        {
            get
            {
                return mainData[1];
            }
            set
            {
                mainData[1] = value;
            }
        }

        /// <summary>
        /// Get or set the value of Digital Input/Output 3
        /// </summary>
        public bool DIO3
        {
            get
            {
                return mainData[2];
            }
            set
            {
                mainData[2] = value;
            }
        }

        /// <summary>
        /// Get or set the value of Digital Input/Output 4
        /// </summary>
        public bool DIO4
        {
            get
            {
                return mainData[3];
            }
            set
            {
                mainData[3] = value;
            }
        }

        /// <summary>
        /// Get or set the value of Digital Input/Output 5
        /// </summary>
        public bool DIO5
        {
            get
            {
                return mainData[4];
            }
            set
            {
                mainData[4] = value;
            }
        }

        /// <summary>
        /// Get or set the value of Digital Input/Output 6
        /// </summary>
        public bool DIO6
        {
            get
            {
                return mainData[5];
            }
            set
            {
                mainData[5] = value;
            }
        }

        /// <summary>
        /// Get or set the value of Digital Input/Output 7
        /// </summary>
        public bool DIO7
        {
            get
            {
                return mainData[6];
            }
            set
            {
                mainData[6] = value;
            }
        }

        /// <summary>
        /// Get or set the value of Digital Input/Output 8
        /// </summary>
        public bool DIO8
        {
            get
            {
                return mainData[7];
            }
            set
            {
                mainData[7] = value;
            }
        }
    }

    /// <summary>
    /// BitField encapsulation for the FRCDSStatus Status field
    /// </summary>
    public struct StatusBitField
    {
        BitField mainData;

        /// <summary>
        /// Create a new StatusBitField with the specified initial value
        /// </summary>
        /// <param name="value">Initial value</param>
        public StatusBitField(byte value)
        {
            mainData = new BitField(value);
        }

        /// <summary>
        /// Create a new StatusBitField with the specified initial value
        /// </summary>
        /// <param name="value">Initial value</param>
        public StatusBitField(int value)
        {
            mainData = new BitField(value);
        }

        /// <summary>
        /// Get or set the value of this BitField
        /// </summary>
        public byte Byte
        {
            get { return mainData.Byte; }
            set { mainData.Byte = value; }
        }

        /// <summary>
        /// Get the internal BitField
        /// </summary>
        public BitField BitField
        {
            get { return mainData; }
        }

        public bool Byte1
        {
            get
            {
                return mainData[0];
            }
            set
            {
                mainData[0] = value;
            }
        }
        public bool EmergencyStopped
        {
            get
            {
                return !mainData[1];
            }
            set
            {
                mainData[1] = !value;
            }
        }
        public bool Enabled
        {
            get
            {
                return mainData[2];
            }
            set
            {
                mainData[2] = value;
            }
        }
        public bool Auto
        {
            get
            {
                return mainData[3];
            }
            set
            {
                mainData[3] = value;
            }
        }
        public bool Byte5
        {
            get
            {
                return mainData[4];
            }
            set
            {
                mainData[4] = value;
            }
        }
        public bool Byte6
        {
            get
            {
                return mainData[5];
            }
            set
            {
                mainData[5] = value;
            }
        }
        public bool RobotAttached
        {
            get
            {
                return mainData[6];
            }
            set
            {
                mainData[6] = value;
            }
        }
        public bool FMSAttached
        {
            get
            {
                return mainData[7];
            }
            set
            {
                mainData[7] = value;
            }
        }
    }

    /// <summary>
    /// BitField Encapsulation for the FRCDSStatus Error field
    /// </summary>
    /// <remarks>
    /// This class is incomplete, we need to know what each field is
    /// </remarks>
    public struct ErrorBitField
    {
        BitField mainData;

        /// <summary>
        /// Create a new ErrorBitField with the specified initial value
        /// </summary>
        /// <param name="value">Initial value</param>
        public ErrorBitField(byte value)
        {
            mainData = new BitField(value);
        }

        /// <summary>
        /// Create a new ErrorBitField with the specified initial value
        /// </summary>
        /// <param name="value">Initial value</param>
        public ErrorBitField(int value)
        {
            mainData = new BitField(value);
        }

        /// <summary>
        /// Get or set the value of this BitField
        /// </summary>
        public byte Byte
        {
            get { return mainData.Byte; }
            set { mainData.Byte = value; }
        }

        /// <summary>
        /// Get the internal BitField
        /// </summary>
        public BitField BitField
        {
            get { return mainData; }
        }

        /// <summary>
        /// Get or set the value of error 1
        /// </summary>
        public bool PacketLost
        {
            get
            {
                return mainData[0];
            }
            set
            {
                mainData[0] = value;
            }
        }
        public bool TeamMismatch
        {
            get
            {
                return mainData[1];
            }
            set
            {
                mainData[1] = value;
            }
        }
        public bool cRIOVersionMismatch
        {
            get
            {
                return mainData[2];
            }
            set
            {
                mainData[2] = value;
            }
        }
        public bool FPGAVersionMismatch
        {
            get
            {
                return mainData[3];
            }
            set
            {
                mainData[3] = value;
            }
        }
        public bool Byte5
        {
            get
            {
                return mainData[4];
            }
            set
            {
                mainData[4] = value;
            }
        }
        public bool Byte6
        {
            get
            {
                return mainData[5];
            }
            set
            {
                mainData[5] = value;
            }
        }
        public bool Byte7
        {
            get
            {
                return mainData[6];
            }
            set
            {
                mainData[6] = value;
            }
        }
        public bool DSVersionError
        {
            get
            {
                return mainData[7];
            }
            set
            {
                mainData[7] = value;
            }
        }
    }
}
