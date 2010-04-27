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

namespace System451.Communication.Dashboard
{
    public enum BitFlags : byte
    {
        Bit0 = 1,
        Bit1 = 2,
        Bit2 = 4,
        Bit3 = 8,
        Bit4 = 16,
        Bit5 = 32,
        Bit6 = 64,
        Bit7 = 128
    }
    public struct BitField
    {
        private byte mainData;
        public static readonly byte[] bitoptions = new byte[] { 1, 2, 4, 8, 16, 32, 64, 128 };
        public BitField(byte value)
        {
            mainData = value;
        }
        public BitField(int value)
        {
            mainData = (byte)value;
        }
        public byte Byte
        {
            get { return mainData; }
            set { mainData = value; }
        }

        public bool this[int index]
        {
            get
            {
                if (index > 7)
                    throw new IndexOutOfRangeException(index + " is out of range of a bit (0-7)");
                byte temp = mainData;
                return (temp & (byte)(bitoptions[index])) == ((byte)(bitoptions[index]));
            }
            set
            {
                if (index > 7)
                    throw new IndexOutOfRangeException(index + " is out of range of a bit (0-7)");
                if (value)
                    mainData |= (bitoptions[index]);
                else
                    mainData &= (byte)(~(bitoptions[index]));
            }
        }

    }
}
