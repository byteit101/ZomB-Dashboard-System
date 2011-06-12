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

namespace System451.Communication.Dashboard.Net
{
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
        public override int ReadInt32()
        {
            int part = 0;
            part += (int)(ReadByte() << 24);
            part += (int)(ReadByte() << 16);
            part += (int)(ReadByte() << 8);
            part += (int)(ReadByte());
            return part;
        }
    }
}
