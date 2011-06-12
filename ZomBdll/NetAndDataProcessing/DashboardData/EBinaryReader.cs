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
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace System451.Communication.Dashboard.Net
{
    //TODO: flush out!
    public class EBinaryReader : BinaryReader
    {
        public EBinaryReader(Stream input): base(input)
        {

        }

        public override ushort ReadUInt16()
        {
            ushort part = 0;
            part += (ushort)(ReadByte() << 8);
            part += (ushort)(ReadByte());
            return part;
        }

        public override uint ReadUInt32()
        {
            uint part = 0;
            part += (uint)(ReadByte() << 24);
            part += (uint)(ReadByte() << 16);
            part += (uint)(ReadByte() << 8);
            part += (uint)(ReadByte());
            return part;
        }
        public override short ReadInt16()
        {
            short part = 0;
            part += (short)(ReadByte() << 8);
            part += (short)(ReadByte());
            return part;
        }
        public override int ReadInt32()
        {
            int part = 0;
            part += (int)(ReadByte() << 24);
            part += (int)(ReadByte() << 16);
            part += (int)(ReadByte() << 8);
            part += (int)(ReadByte());
            return part;
        }
        public override long ReadInt64()
        {
            long part = 0;
            part += (long)(((long)ReadByte()) << 56);
            part += (long)(((long)ReadByte()) << 48);
            part += (long)(((long)ReadByte()) << 40);
            part += (long)(((long)ReadByte()) << 32);
            part += (long)(ReadByte() << 24);
            part += (long)(ReadByte() << 16);
            part += (long)(ReadByte() << 8);
            part += (long)(ReadByte());
            return part;
        }

        public string ReadUTFString()
        {
            int length = ReadUInt16();
            byte[] ba = new byte[length];
            Read(ba, 0, length);
            return UTF8Encoding.UTF8.GetString(ba);
        }

        public override string ReadString()
        {
            int length = ReadUInt16();
            byte[] ba = new byte[length];
            Read(ba, 0, length);
            return ASCIIEncoding.ASCII.GetString(ba);
        }

        public override double ReadDouble()
        {
            return BitConverter.Int64BitsToDouble(ReadInt64());
        }

        public override float ReadSingle()
        {
            return new Int32SingleUnion(ReadInt32()).AsSingle;
        }
    }

    //http://bytes.com/topic/c-sharp/answers/274876-how-do-bitconverter-singletoint32bits
    [StructLayout(LayoutKind.Explicit)]
    struct Int32SingleUnion
    {
        /// <summary>
        /// Int32 version of the value.
        /// </summary>
        [FieldOffset(0)]
        int i;
        /// <summary>
        /// Single version of the value.
        /// </summary>
        [FieldOffset(0)]
        float f;

        /// <summary>
        /// Creates an instance representing the given integer.
        /// </summary>
        /// <param name="i">The integer value of the new instance.</param>
        internal Int32SingleUnion(int i)
        {
            this.f = 0; // Just to keep the compiler happy
            this.i = i;
        }

        /// <summary>
        /// Creates an instance representing the given floating point
        /// number.
        /// </summary>
        /// <param name="f">The floating point value of the new instance</param>
        internal Int32SingleUnion(float f)
        {
            this.i = 0; // Just to keep the compiler happy
            this.f = f;
        }

        /// <summary>
        /// Returns the value of the instance as an integer.
        /// </summary>
        internal int AsInt32
        {
            get { return i; }
        }

        /// <summary>
        /// Returns the value of the instance as a floating point number.
        /// </summary>
        internal float AsSingle
        {
            get { return f; }
        }
    }
}
