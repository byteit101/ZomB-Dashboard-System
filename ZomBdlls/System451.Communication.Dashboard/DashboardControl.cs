/*
 * Copyright (c) 2009-2010, Patrick Plenefisch and FIRST Robotics Team 451 "The Cat Attack"
 * 
 * Permission to use, copy, modify, and distribute this software, its source, and its documentation
 * for any purpose, without fee, and without a written agreement is hereby granted, 
 * provided this paragraph and the following paragraph appear in all copies, and all
 * software that uses this code is released under this license. All projects that use
 * this code MUST release their source without fee.
 * 
 * THIS SOFTWARE IS PROVIDED "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
 * AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL
 * Patrick Plenefisch OR FIRST Robotics Team 451 "The Cat Attack" BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace System451.Communication.Dashboard
{
    [Obsolete("This will be replaced with IZomBControl, and removed in v0.7 and later")]
    public interface IDashboardControl
    {
        string[] ParamName { get; set; }
        string Value { get; set; }
        string DefalutValue { get; }
        void Update();
    }

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
        /// Does this control need all the data, or just its own.
        /// </summary>
        bool RequiresAllData { get; }

        /// <summary>
        /// Does this control watch multiple values, or just one.
        /// </summary>
        bool IsMultiWatch { get; }

        /// <summary>
        /// The name of the control. This is the name you send values to.
        /// If IsMultiWatch is true, this is a semicolin seperated list of names.
        /// </summary>
        string ControlName { get; }

        /// <summary>
        /// Updates the control with new data
        /// </summary>
        /// <param name="value">The new value of the control. If IsMultiWatch is true, this is a pipe seperated list of values</param>
        /// <param name="packetData">If RequiresAllData is true, this contains all the packet data</param>
        void UpdateControl(string value, byte[] packetData);

        /// <summary>
        /// Notifies when this control is added to a DashboardDataHub
        /// </summary>
        event ControlAddedDelegate ControlAdded;
    }

    /// <summary>
    /// A default implementation of the IZomBControl so you don't have to do boring work
    /// </summary>
    public class ZomBControl : IZomBControl
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
        /// Gets the RequiresAllData field. Default false.
        /// </summary>
        virtual public bool RequiresAllData
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the IsMultiWatch field. Default false.
        /// </summary>
        virtual public bool IsMultiWatch
        {
            get { return false; }
        }

        /// <summary>
        /// The control name. This needs to be implemented.
        /// </summary>
        /// <exception cref="System.NotImplementedException">Always throws NotImplementedException</exception>
        virtual public string ControlName
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Updates the control. This needs to be implemented if UpdateControl(string value) is not implemented.
        /// </summary>
        /// <exception cref="System.NotImplementedException">Always throws NotImplementedException</exception>
        virtual public void UpdateControl(string value, byte[] packetData)
        {
            this.UpdateControl(value);
        }

        /// <summary>
        /// Updates the control. This needs to be implemented if UpdateControl(string value, byte[] packetData) is not implemented.
        /// </summary>
        /// <exception cref="System.NotImplementedException">Always throws NotImplementedException</exception>
        virtual public void UpdateControl(string value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When this control is added to a DashboardDataHub
        /// </summary>
        public event ControlAddedDelegate ControlAdded;

        #endregion

        void ZomBControl_ControlAdded(object sender, ZomBControlAddedEventArgs e)
        {
            localDDH = e.Controller.GetDashboardDataHub();
        }

        /// <summary>
        /// Gets the current DashboardDataHub
        /// </summary>
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
        string value;
        byte[] packetData;

        /// <summary>
        /// Create a new ZomBControlUpdatedEventArgs
        /// </summary>
        /// <param name="value">The new value of the control. If IsMultiWatch is true, this is a pipe seperated list of values</param>
        /// <param name="packetData">If RequiresAllData is true, this contains all the packet data</param>
        public ZomBControlUpdatedEventArgs(string value, byte[] packetData)
        {
            this.value = value;
            this.packetData = packetData;;
        }

        /// <summary>
        /// The new value of the control. If IsMultiWatch is true, this is a pipe seperated list of values
        /// </summary>
        public string Value
        {
            get
            {
                return value;
            }
        }

        /// <summary>
        /// If RequiresAllData is true, this contains all the packet data
        /// </summary>
        public byte[] PacketData
        {
            get
            {
                return packetData;
            }
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
        /// If IsMultiWatch is true, this is a semicolin seperated list of names.
        /// </summary>
        public override string ControlName
        {
            get;
            set;
        }

        /// <summary>
        /// Updates the control with new data
        /// </summary>
        /// <param name="value">The new value of the control. If IsMultiWatch is true, this is a pipe seperated list of values</param>
        /// <param name="packetData">If RequiresAllData is true, this contains all the packet data</param>
        public override void UpdateControl(string value, byte[] packetData)
        {
            if (ControlUpdated != null)
                ControlUpdated(this, new ZomBControlUpdatedEventArgs(value, packetData));
        }

        #region IZomBRemoteControl Members

        /// <summary>
        /// Fires when the Control is updated via IZomBControl.UpdateControl
        /// </summary>
        public event ControlUpdatedDelegate ControlUpdated;

        #endregion
    }

    /// <summary>
    /// Easily manage a bunch of virtual or physical IZomBcontrols by name
    /// </summary>
    public class ZomBControlCollection : Dictionary<string, IZomBControl>
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
        /// <param name="packetData">The raw packet data</param>
        void UpdateData(Dictionary<string, string> data, byte[] packetData);
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

        public bool Reset
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
        public bool Field1
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
        public bool Field2
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
        public bool Field3
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
        public bool Field4
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
        public bool Field5
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
        public bool Field6
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
        public bool Field7
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
        public bool Field8
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
