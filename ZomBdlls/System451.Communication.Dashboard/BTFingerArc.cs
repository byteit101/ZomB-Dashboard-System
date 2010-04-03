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
using System.IO;
using System.Threading;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System.Collections.ObjectModel;

namespace System451.Communication.Dashboard
{
    //BT is Bluetooth
    //All BT classes are optional
    //The classmate is the BTZomBServer
    //The display computer is the BTFinger
    //Create a BTZomBFingerFactory, and pull out the appropriate class,
    //then Start the component

    /// <summary>
    /// Creates a Bluetooth Finger
    /// </summary>
    public class BTZomBFingerFactory
    {
        BluetoothRadio radio;
        BluetoothListener listen;
        BluetoothClient client;
        static Guid teamgid;
        string saveTo, pullFrom;
        BTFinger bfinger;
        BTZomBServer bserve;
        /// <summary>
        /// Create a new ZomB FingerServer Factory
        /// </summary>
        /// <param name="teamNumber">Your team number</param>
        /// <param name="from">Path to saved data folder</param>
        /// <param name="to">Save data to this folder</param>
        /// <remarks>
        /// The path should be a reference to a folder that contains all the data files.
        /// Once all the files are sent, the tag .ZomBarchiveXXX is appended to their file name, where
        /// XXX is the time ticks.
        /// The To path works similarly, but .ZomBYYY is appended before the file extension, where 
        /// YYY is the transfer number stored in the file .ZomB in the To path
        /// </remarks>
        public BTZomBFingerFactory(int teamNumber, string from, string to)
        {
            saveTo = to;
            pullFrom = from;
            //team GUID, last part is random, don't want to calculate
            if (teamgid != null)
                teamgid = new Guid(teamNumber * teamNumber, (short)teamNumber, (short)teamNumber, new byte[] { 0xa1, 0xfc, 0xf7, 0x95, 0x4a, 0x58, 0x6f, 0x25 });
            radio = InTheHand.Net.Bluetooth.BluetoothRadio.PrimaryRadio;
            listen = new BluetoothListener(teamgid);
            bserve = new BTZomBServer(listen, pullFrom, teamNumber, this);
            //radio.Mode = RadioMode.Connectable;
            client = new BluetoothClient();
            bfinger = new BTFinger(client, saveTo, teamNumber, this);
            TeamNumber = teamNumber;

        }
        internal BTZomBFingerFactory()
        {
            if (teamgid == null)
                throw new NullReferenceException("Please call the Public constructor first!");
        }
        public int TeamNumber
        {
            get;
            private set;
        }

        public Guid BTGuid
        {
            get
            {
                return teamgid;
            }
            private set
            {
                if (teamgid != null)
                    teamgid = value;
            }
        }

        public static readonly byte[] BTHelloMessage = { 72, 101, 108, 108, 111, 32, 90, 111, 109, 66 };
        public static readonly byte[] BTVerifyMessage = { 84, 101, 97, 109, 32, 86, 101, 114, 105, 102, 121 };
        public static readonly byte[] BTGoodVerifyMessage = { 71, 111, 111, 100, 86, 101, 114, 105 };
        public static readonly byte[] BTSendFileMessage = { 83, 101, 110, 100, 70, 105, 108, 101 };
        public static readonly byte[] BTNameFileMessage = { 78, 97, 109, 101, 70, 105, 108, 101 };
        public static readonly byte[] BTAcceptFileMessage = { 65, 99, 99, 101, 112, 116, 70, 84 };
        public static readonly byte[] BTRecievedMessage = { 82, 101, 99, 105, 101, 118, 101, 100 };
        public static readonly byte[] BTErrRecievedMessage = { 69, 114, 114, 84, 114, 97, 110, 115 };
        public static readonly byte[] BTSignOffMessage = { 83, 105, 103, 110, 32, 79, 102, 102 };
        public static readonly byte[] BTFileSequenceMessage = { 255, 0, 255, 0 };
        public static readonly byte[] BTEOFTMessage = { 69, 79, 70, 84 };
        public static readonly byte[] BTByeMessage = { 66, 121, 101 };
        public static readonly byte[] BTNoFilesMessage = { 78, 111, 32, 70, 105, 108, 101, 115 };

        public BTFinger GetFinger()
        {
            return bfinger;
        }
        public BTZomBServer GetServer()
        {
            return bserve;
        }
        public void DisableBT()
        {
            //radio.Mode = RadioMode.PowerOff;
        }
        public void EnableBT()
        {
            //radio.Mode = RadioMode.Connectable;
        }
        public static string DefaultSaveLocation { get { return @"C:\Program Files\ZomB\Data"; } }
        public static string DefaultLoadLocation { get { return @"C:\Program Files\ZomB\Data\BluArc"; } }
    }

    public class BTFinger
    {
        BluetoothClient blucli;
        string To;
        BTZomBFingerFactory ff;
        Thread sliceThread;
        bool sliceing = false;

        internal BTFinger(BluetoothClient client, string to, int tn, BTZomBFingerFactory finger)
        {
            blucli = client;
            To = to;
            if (!To.EndsWith("\\"))
                To += "\\";
            TeamNumber = tn;
            ff = finger;
            sliceThread = new Thread(ZomBworker);
            sliceThread.IsBackground = true;
            if (!Directory.Exists(To))
                Directory.CreateDirectory(To);
        }
        ~BTFinger()
        {
            sliceing = false;
            Thread.Sleep(0);
            Thread.Sleep(1);
            if (sliceThread.ThreadState != System.Threading.ThreadState.Stopped)
                sliceThread.Abort();
        }
        public event EventHandler DataRecieving;
        public event EventHandler DataRecieved;

        protected int TeamNumber { get; private set; }

        public void Start()
        {
            sliceing = true;
            if (sliceThread.ThreadState != ThreadState.Unstarted)
            {
                try
                {
                    sliceThread.Abort();
                }
                catch { }
                sliceThread = new Thread(ZomBworker);
            }
            sliceThread.Start();

        }
        public void Stop()
        {
            sliceing = false;
        }
        protected byte ReadStatus(Stream strm)
        {
            try
            {
                return ReadStatus(strm, 1)[0];
            }
            catch
            {
                return 0;
            }
        }

        protected byte[] ReadStatus(Stream strm, int maxlength)
        {
            byte[] hlMsg = new byte[maxlength];
            while (strm.CanRead == false)
            { Thread.Sleep(5); }
            int readcount = strm.Read(hlMsg, 0, maxlength);
            while (readcount < maxlength)
            {
                Thread.Sleep(40);
                while (strm.CanRead == false)
                { Thread.Sleep(5); }
                readcount += strm.Read(hlMsg, readcount, maxlength - readcount);
            }
            return hlMsg;
        }
        private void ReplyStatus(Stream strm, byte[] Message)
        {
            ReplyStatus(strm, Message, null);
        }

        private void ReplyStatus(Stream strm, byte[] Message, params byte[] additions)
        {
            strm.Write(Message, 0, Message.Length);
            if (additions != null)
                strm.Write(additions, 0, additions.Length);
        }

        private void ZomBworker()
        {
            while (sliceing)
            {
                try
                {

                    blucli.Connect(GetZomBServerAddress(), ff.BTGuid);//finds a server blocking

                    using (Stream strm = blucli.GetStream())
                    {
                        ReplyStatus(strm, BTZomBFingerFactory.BTHelloMessage);
                        if (BS(ReadStatus(strm, 10)) == BS(BTZomBFingerFactory.BTHelloMessage))//if good Verify
                        {
                            if (ReadStatus(strm) == ((byte)(TeamNumber >> 8)) && ReadStatus(strm) == (byte)TeamNumber)//team # validate
                            {
                                ReplyStatus(strm, BTZomBFingerFactory.BTVerifyMessage, ((byte)(TeamNumber >> 8)), (byte)TeamNumber);
                                if (DataRecieving != null)
                                    DataRecieving(this, new EventArgs());
                                if (BS(ReadStatus(strm, 8)) == BS(BTZomBFingerFactory.BTGoodVerifyMessage))
                                {
                                    while (true)
                                    {
                                        switch (ByteArrayToString(ReadStatus(strm, 8)))
                                        {
                                            case "SendFile":
                                                if (BS(ReadStatus(strm, 8)) == BS(BTZomBFingerFactory.BTNameFileMessage))
                                                {
                                                    if (ReadStatus(strm) == 0x0)
                                                    {
                                                        string newfilename = ByteArrayToString(ReadStatus(strm, (int)ReadStatus(strm)));
                                                        if (ReadStatus(strm) == 0x0)
                                                        {
                                                            ReplyStatus(strm, BTZomBFingerFactory.BTAcceptFileMessage);
                                                            if (BS(ReadStatus(strm, 4)) == BS(BTZomBFingerFactory.BTFileSequenceMessage))
                                                            {
                                                                string filelength = "";
                                                                byte last = ReadStatus(strm);
                                                                while (last != 0xFF)
                                                                {
                                                                    filelength += (char)last;
                                                                    last = ReadStatus(strm);
                                                                }
                                                                if (ReadStatus(strm) == 0x0 && ReadStatus(strm) == 0xff && ReadStatus(strm) == 0x0)
                                                                {

                                                                    File.WriteAllBytes(To + newfilename, ReadStatus(strm, int.Parse(filelength)));
                                                                    if (BS(ReadStatus(strm, 4)) == BS(BTZomBFingerFactory.BTFileSequenceMessage))
                                                                        if (BS(ReadStatus(strm, 4)) == BS(BTZomBFingerFactory.BTEOFTMessage))
                                                                        {
                                                                            ReplyStatus(strm, BTZomBFingerFactory.BTRecievedMessage);
                                                                            break;
                                                                        }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                break;
                                            case "Sign Off":
                                                if (DataRecieved != null)
                                                    DataRecieved(this, new EventArgs());
                                                ReplyStatus(strm, BTZomBFingerFactory.BTByeMessage);
                                                goto end;
                                            case "No Files":
                                                //Ok, I don't care, continue
                                                break;
                                            default:
                                                //TODO: AHHH!
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
                catch
                {
                    continue;
                }
            end:
                if (!blucli.Connected)
                { blucli = new BluetoothClient(); }
            }
        }

        private BluetoothAddress GetZomBServerAddress()
        {
            while (sliceing)
            {
                BluetoothDeviceInfo[] bdi = blucli.DiscoverDevices();
                foreach (BluetoothDeviceInfo item in bdi)
                {
                    string DN = item.DeviceName;
                    if (DN == "ZomBServer" + TeamNumber && sliceing)
                        return item.DeviceAddress;
                }
                Thread.Sleep(10);
            }
            return null;
        }

        private string ByteArrayToString(byte[] pureName)
        {
            string r = System.Text.Encoding.ASCII.GetString(pureName);
            return r;
        }
        private string BS(byte[] pureName)
        {
            string r = System.Text.Encoding.ASCII.GetString(pureName);
            return r;
        }
    }
    public class BTZomBServer
    {
        BluetoothListener listen;
        string from;
        Thread severThread;
        bool severing = false;//Gotta have some ZomB humor
        BTZomBFingerFactory ff;
        internal BTZomBServer(BluetoothListener listener, string from, int tn, BTZomBFingerFactory finger)
        {
            listen = listener;
            this.from = from;
            severThread = new Thread(ZomBworker);
            TeamNumber = tn;
            ff = finger;
        }
        ~BTZomBServer()
        {
            severing = false;
            Thread.Sleep(0);
            Thread.Sleep(1);
            if (severThread.ThreadState != System.Threading.ThreadState.Stopped)
                severThread.Abort();
        }
        public event EventHandler DataSending;
        public event EventHandler DataSent;

        protected int TeamNumber { get; private set; }

        public void Start()
        {
            severing = true;
            if (severThread.ThreadState != ThreadState.Unstarted)
            {
                try
                {
                    severThread.Abort();
                }
                catch { }
                severThread = new Thread(ZomBworker);
            }
            severThread.Start();
        }
        public void Stop()
        {
            severing = false;
        }
        protected byte ReadStatus(Stream strm)
        {
            try
            {
                return ReadStatus(strm, 1)[0];
            }
            catch
            {
                return 0;
            }
        }

        protected byte[] ReadStatus(Stream strm, int maxlength)
        {
            byte[] hlMsg = new byte[maxlength];
            while (strm.CanRead == false)
            { Thread.Sleep(5); }
            int readcount = strm.Read(hlMsg, 0, maxlength);
            while (readcount < maxlength)
            {
                Thread.Sleep(40);
                while (strm.CanRead == false)
                { Thread.Sleep(5); }
                readcount += strm.Read(hlMsg, readcount, maxlength - readcount);
            }
            return hlMsg;
        }
        private void ReplyStatus(Stream strm, byte[] Message)
        {
            ReplyStatus(strm, Message, null);
        }

        private void ReplyStatus(Stream strm, byte[] Message, params byte[] additions)
        {
            strm.Write(Message, 0, Message.Length);
            if (additions != null)
                strm.Write(additions, 0, additions.Length);
        }

        private void ZomBworker()
        {
            ff.EnableBT();
            Thread.Sleep(30);
            listen.Start();
            //while (severing)
            // {
            string connectString;
            while (!listen.Pending())
            {

                Thread.Sleep(30);
            }
            using (BluetoothClient bcli = listen.AcceptBluetoothClient())//This blocks
            {
                using (Stream strm = bcli.GetStream())
                {
                    if (BS(ReadStatus(strm, 10)) == BS(BTZomBFingerFactory.BTHelloMessage))//if good sign on
                    {
                        //Reply with team Verification
                        ReplyStatus(strm, BTZomBFingerFactory.BTHelloMessage, ((byte)(TeamNumber >> 8)), (byte)TeamNumber);

                        if (BS(ReadStatus(strm, 11)) == BS(BTZomBFingerFactory.BTVerifyMessage))//if good Verify
                        {
                            if (ReadStatus(strm) == ((byte)(TeamNumber >> 8)) && ReadStatus(strm) == (byte)TeamNumber)//team # validate
                            {
                                if (DataSending != null)
                                    DataSending(this, new EventArgs());
                                ReplyStatus(strm, BTZomBFingerFactory.BTGoodVerifyMessage);
                                SendFiles(strm, GetNewFiles());
                                if (DataSent != null)
                                    DataSent(this, new EventArgs());
                                ReplyStatus(strm, BTZomBFingerFactory.BTSignOffMessage);
                            }
                        }
                    }
                }
            }

            listen.Stop();
            ff.DisableBT();
        }

        private void SendFiles(Stream strm, string[] files)
        {
            if (files == null || files.Length < 1)
            {
                ReplyStatus(strm, BTZomBFingerFactory.BTNoFilesMessage);
                return;
            }
            foreach (string file in files)
            {
                ReplyStatus(strm, BTZomBFingerFactory.BTSendFileMessage);
                string pureName = Path.GetFileName(file);
                ReplyStatus(strm, BTZomBFingerFactory.BTNameFileMessage, 0, (byte)pureName.Length);
                ReplyStatus(strm, StringToByteArray(pureName), 0);
                if (BS(ReadStatus(strm, 8)) == BS(BTZomBFingerFactory.BTAcceptFileMessage))
                {
                    byte[] tmp = File.ReadAllBytes(file);
                    ReplyStatus(strm, BTZomBFingerFactory.BTFileSequenceMessage);
                    ReplyStatus(strm, StringToByteArray(tmp.Length.ToString()));
                    ReplyStatus(strm, BTZomBFingerFactory.BTFileSequenceMessage);
                    ReplyStatus(strm, tmp);//send file
                    ReplyStatus(strm, BTZomBFingerFactory.BTFileSequenceMessage);
                    ReplyStatus(strm, BTZomBFingerFactory.BTEOFTMessage);
                    if (BS(ReadStatus(strm, 8)) == BS(BTZomBFingerFactory.BTRecievedMessage))
                    {
                        //TODO: implement this

                        File.Move(file, file + ".ZomBarchive" + (((short)DateTime.Now.Ticks).ToString("x").PadLeft(4, '0')));
                    }
                }

            }

        }
        private string BS(byte[] pureName)
        {
            string r = System.Text.Encoding.ASCII.GetString(pureName);
            return r;
        }

        private byte[] StringToByteArray(string pureName)
        {
            return System.Text.Encoding.ASCII.GetBytes(pureName);
        }

        private string[] GetNewFiles()
        {
            //TODO: weed out large files
            //TODO: ingore previous
            string[] r = Directory.GetFiles(from);//, "*.ZomBarchive????");
            Collection<string> c = new Collection<string>();
            foreach (string item in r)
            {
                if (!item.Contains(".ZomBarchive"))
                    c.Add(item);
            }
            r = new string[c.Count];
            c.CopyTo(r, 0);
            return r;
        }

    }

}
/*
ZomB Bluetooth Protocol: > is client (computer), : is server (classmate), [] indicate byte instead of ascii, < is note, test for team 451
>Hello ZomB
:Hello ZomB[1][195] <451 shifted into 2 bytes
>Team Verify[1][195]
:GoodVeri <could move to next line or alt line below
:SendFile
:NameFile[0][10]myfile.txt[0] <10 is the file name length
>AcceptFT
:[255][0][255][0]123[255][0][255][0] <123 is the file size
: <sends byte data from File.GetAllBytes()
:[255][0][255][0]EOFT
>Recieved <can also be ErrTrans
<Send file to Recieved for each file
:Sign Off
>Bye     


Alt Logon
:No Files





End of ZomB bluetooth Protocol
*/