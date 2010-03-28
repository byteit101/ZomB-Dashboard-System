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
using System.Collections.Generic;
using System.Text;

namespace System451.Communication.Dashboard
{
    public enum BitFlags:byte
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
                for (int i = 7 - index; i > 0; i--)
                {
                    temp = (byte)(temp >> 1);
                }
                temp = (byte)(temp << 7);
                return temp == 0 ? false : true;
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
