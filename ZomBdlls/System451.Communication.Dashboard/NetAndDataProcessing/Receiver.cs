/*
 * This code was found on http://www.chiefdelphi.com/forums/showthread.php?t=82422
 * Since this code was not found with a license, it is subject
 * to the ZomB Dashboard System's license
 * Original source can be found at http://www.chiefdelphi.com/forums/attachment.php?attachmentid=8654&d=1266113910
 **/
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
