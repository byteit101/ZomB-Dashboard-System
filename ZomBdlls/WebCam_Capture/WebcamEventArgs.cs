/*
 * This code was found on http://www.Planet-Source-Code.com/vb/scripts/ShowCode.asp?txtCodeId=1339&lngWId=10
 * Since this code was not found with a license, it is subject
 * to the ZomB Dashboard System's license
 * This source code is unmodified from the original source (aside from this header)
 **/
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
