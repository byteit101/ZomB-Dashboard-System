using System;
using System.Collections.Generic;
using System.Text;

namespace SmashTcpDashboard
{
    // base class to allow the dashboard to
    // handle more than one type of receiver
    class Receiver: IDisposable
    {
        public delegate void ImageUpdate();
        public event ImageUpdate OnImageUpdate;

        public delegate void DateReceived(int count);
        public event DateReceived OnDataReceived;

        public delegate void Error(string message);
        public event Error OnError;

        public virtual bool Start()
        {
            Running = true;
            return Running;
        }

        public virtual bool Stop()
        {
            Running = false;
            return !Running;
        }

        protected virtual void ProcImageUpdate()
        {
            if (OnImageUpdate != null)
            {
                // proc OnImageUpdate event
                OnImageUpdate();
            }
        }

        public virtual void Dispose()
        {
            Stop();
        }

        protected virtual void ProcDataReceived(int count)
        {
            if (OnDataReceived != null)
            {
                // proc OnDataReceived event
                OnDataReceived(count);
            }
        }

        protected virtual void ProcError(string message)
        {
            if (OnError != null)
            {
                // proc OnError event
                OnError(message);
            }
        }

        public byte[] ImageData { get; protected set; }
        public bool Running { get; protected set; }
    }
}
