/*
 * This code was found on http://www.codeproject.com/KB/audio-video/avifilewrapper.aspx
 * and was released under The Code Project Open License (CPOL) @ http://www.codeproject.com/info/cpol10.aspx
 * This, and all other source files from this project are not under the ZomB Dashboard System's 
 * license, as this source code is unmodified (aside from this header)
 **/
using System;

namespace AviFile
{
	public abstract class AviStream
	{
		protected int aviFile;
		protected IntPtr aviStream;
		protected IntPtr compressedStream;
		protected bool writeCompressed;

        /// <summary>Pointer to the unmanaged AVI file</summary>
        internal int FilePointer {
            get { return aviFile; }
        }

        /// <summary>Pointer to the unmanaged AVI Stream</summary>
        internal virtual IntPtr StreamPointer {
            get { return aviStream; }
        }

        /// <summary>Flag: The stream is compressed/uncompressed</summary>
        internal bool WriteCompressed {
            get { return writeCompressed; }
        }

        /// <summary>Close the stream</summary>
        public virtual void Close(){
			if(writeCompressed){
				Avi.AVIStreamRelease(compressedStream);
			}
			Avi.AVIStreamRelease(StreamPointer);
		}

        /// <summary>Export the stream into a new file</summary>
        /// <param name="fileName"></param>
		public abstract void ExportStream(String fileName);
		
	}
}
