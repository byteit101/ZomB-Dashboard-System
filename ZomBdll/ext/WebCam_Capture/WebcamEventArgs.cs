/*
 * This code was found on http://www.Planet-Source-Code.com/vb/scripts/ShowCode.asp?txtCodeId=1339&lngWId=10
 * Since this code was not found with a license, it is subject
 * to the ZomB Dashboard System's license
 * This source code is unmodified from the original source (aside from this header, and the notice below)
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

namespace WebCam_Capture
{
	/// <summary>
	/// EventArgs for the webcam control
	/// </summary>
	public class WebcamEventArgs : System.EventArgs 
	{
		private System.Drawing.Image m_Image;
		private ulong m_FrameNumber = 0;

		public WebcamEventArgs()
		{
		}

		/// <summary>
		///  WebCamImage
		///  This is the image returned by the web camera capture
		/// </summary>
		public System.Drawing.Image WebCamImage
		{
			get
			{ return m_Image; }

			set
			{ m_Image = value; }
		}

		/// <summary>
		/// FrameNumber
		/// Holds the sequence number of the frame capture
		/// </summary>
		public ulong FrameNumber
		{
			get
			{ return m_FrameNumber; }

			set
			{ m_FrameNumber = value; }
		}
	}
}
