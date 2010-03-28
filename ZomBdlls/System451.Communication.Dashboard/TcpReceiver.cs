using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SmashTcpDashboard
{
    class TcpReceiver: Receiver
    {
        // axis camera sends video in the following order :
        // header { 0x1, 0x0, 0x0, 0x0 } raw bytes - no endianness
        // image size { 4 bytes, variable data } - big endian
        // image data { variable size, variable data } - always big endian
        enum TpcState
        {
            Header,
            ImageSize,
            Image
        }

        // does not start the receiver
        public TcpReceiver(string robotIP)
        {
            Running = false;
            RobotIP = robotIP;
            // initialize tcp/ip
            RobotSocket = null;//Be nice
            //RobotSocket.ReceiveBufferSize = 1048576;
            //RobotSocket.ReceiveTimeout = 128;
            ReceiverThread = null;
        }

        // used to clean up the program after exit
        public override void Dispose()
        {
            Stop();
        }
        ~TcpReceiver()
        {
            Dispose();
        }

        // turns on the receiver
        public override bool Start()
        {
            if (Running)
            {
                Stop();
            }
            try
            {
                RobotSocket = new TcpClient(new IPEndPoint(IPAddress.Any, RobotPort));
                RobotSocket.ReceiveBufferSize = 2048576;
                RobotSocket.ReceiveTimeout = 128;
                // connect to the target
                RobotSocket.Connect(RobotIP, RobotPort);
                //RobotSocket.Client.Bind(new IPEndPoint(IPAddress.Loopback, RobotPort));
                // start the method to handle image data
                if (ReceiverThread != null)
                {
                    ReceiverThread.Abort();
                }
                ReceiverThread = new Thread(ReceiverHelper);
                ReceiverThread.IsBackground = true;
                ReceiverThread.Start();
                Running = true;
            }
            catch (ArgumentNullException anex)
            {
                ProcError("Robot IP is null");
            }
            catch (ArgumentOutOfRangeException aoorex)
            {
                ProcError("Port " + RobotPort + " is out of range");
            }
            catch (SocketException sex)
            {
                ProcError("General Socket Exception");
            }
            catch (ObjectDisposedException odex)
            {
                ProcError("A strange error has occured -- a restart maybe required");
            }
            catch (ThreadStateException tsex)
            {
                ProcError("Failed to start receiver");
            }
            catch (OutOfMemoryException oomex)
            {
                ProcError("Failed to start receiver -- out of memory");
            }
            catch (Exception ex)
            {
                ProcError("An error has occured");
            }
            return Running;
        }

        public override bool Stop()
        {
            Running = false;

            RobotSocket.Close();
            //RobotSocket.Client.Disconnect(false);


            return !Running;
        }

        public readonly int RobotPort = 1180;
        public string RobotIP { get; set; }

        private void ReceiverHelper()
        {
            bgn:
            TpcState state = TpcState.Header;
            int image_size = 0;
            try
            {
                while (Running)
                {
                    Thread.Sleep(1);
                    switch (state)
                    {
                        // receiver the header
                        case TpcState.Header:
                            {
                                // can value of ImageHeaderSize bytes be read
                                if (RobotSocket.Available < ImageHeaderSize)
                                {
                                    continue;
                                }
                                byte[] buffer = new byte[ImageHeaderSize];
                                int count = RobotSocket.GetStream().Read(buffer, 0, ImageHeaderSize);
                                ProcDataReceived(count);
                                // first byte is always 0x1
                                // the receiver is in an unknown state if true
                                if (buffer[0] != 0x1 || buffer[1] != 0x0 || buffer[2] != 0x0 || buffer[3] != 0x0)
                                {
                                    while (true)
                                    {
                                        //Actually move forward
                                        while (RobotSocket.GetStream().ReadByte() != 0x1)
                                        {

                                        }
                                        if (RobotSocket.GetStream().ReadByte() != 0x0)
                                            continue;
                                        if (RobotSocket.GetStream().ReadByte() != 0x0)
                                            continue;
                                        if (RobotSocket.GetStream().ReadByte() != 0x0)
                                            continue;
                                        break;
                                    }

                                    //goto default;
                                }
                                state = TpcState.ImageSize;
                                break;
                            }
                        // receive image size
                        case TpcState.ImageSize:
                            {
                                // can value of ImageLengthSize bytes be read
                                if (RobotSocket.Available < ImageLengthSize)
                                {
                                    continue;
                                }
                                byte[] buffer = new byte[ImageLengthSize];
                                ProcDataReceived(RobotSocket.GetStream().Read(buffer, 0, ImageLengthSize));
                                image_size = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, 0));
                                // image size being less than zero wouldn't make sense
                                if (image_size < 0 || image_size > 2048576)
                                {
                                    goto default;
                                }
                                state = TpcState.Image;
                                break;
                            }
                        // receive image
                        case TpcState.Image:
                            {
                                // can value of image_size bytes be read
                                if (RobotSocket.Available < image_size)
                                {
                                    continue;
                                }
                                byte[] buffer = new byte[image_size];
                                ProcDataReceived(RobotSocket.GetStream().Read(buffer, 0, image_size));
                                image_size = 0;
                                ImageData = buffer;
                                ProcImageUpdate();
                                state = TpcState.Header;
                                break;
                            }
                        default:
                            {
                                ProcError("Unknown state");
                                state = TpcState.Header;
                                break;
                                //return;
                            }
                    }
                }
            }
            catch (ThreadAbortException ex)
            {
                return;
            }
            catch (Exception ex)
            {
                ProcError("Unknown state");
            }
            goto bgn;
        }

        private readonly int ImageHeaderSize = 4;
        private readonly int ImageLengthSize = 4;

        private TcpClient RobotSocket { get; set; }
        private Thread ReceiverThread { get; set; }
    }
}
