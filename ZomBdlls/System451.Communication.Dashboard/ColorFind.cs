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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace System451.Communication.Dashboard
{
  
    public partial class ColorFind : UserControl
    {
        public ColorFind()
        {
            InitializeComponent();
        }
        Color GetPix(Bitmap img, int x,int y){
            return img.GetPixel(x, y);
        }
     /*  static public Rectangle GetLargestPixel(Bitmap img, ColorRange color)
        {
            //TODO Make vxWorks
                Size sz = img.Size;
                bool[,] pixes = new bool[sz.Width, sz.Height];
                Rectangle bigest = new Rectangle(0, 0,0,0);
            //end 
                for (int x = 0; x < sz.Width; x++)
                {
                    for (int y = 0; y < sz.Height; y++)
                    {
                        pixes[x, y] = InColorRange(color, GetPix(img, x, y));
                    }
                }
                for (int x = 0; x < sz.Width; x++)
                {
                    for (int y = 0; y < sz.Height; y++)
                    {
                        if (pixes[x, y])
                        {
                            Rectangle thisbloc = new Rectangle(0, 0, 0, 0);
                            //TODO Make vxWorks ref
                            int neibs = Checknext(pixes, x, y, ref thisbloc);
                        }
                    }
                }
        }
        //TODO Make vxWorks ref
        private int Checknext(bool[,] pixes, int x, int y, ref Rectangle thisbloc)
        {
            int c = 0;
            //TODO Make vxWorks []
            bool[,] works = new bool[3, 3];
            if (pixes[x - 1, y - 1])
            {
                works[0, 0] = true;
                c++;
            }
            if (pixes[x - 1, y])
            {
                works[0, 1] = true;
                c++;
            }
            if (pixes[x - 1, y + 1])
            {
                works[0, 2] = true;
                c++;
            }
            if (pixes[x, y - 1])
            {
                works[1, 0] = true;
                c++;
            }
            if (pixes[x, y])
            {
                works[1, 1] = true;
            }//this is the act pix
            if (pixes[x, y + 1])
            {
                works[1, 2] = true;
                c++;
            }
            if (pixes[x + 1, y - 1])
            {
                works[2, 0] = true;
                c++;
            }
            if (pixes[x + 1, y])
            {
                works[2, 1] = true;
                c++;
            }
            if (pixes[x + 1, y + 1])
            {
                works[2, 2] = true;
                c++;
            }
            thisbloc = rectfrom3x3bool(works);
            return c;
        }

        private Rectangle rectfrom3x3bool(bool[,] works)
        {
            Rectangle ret = new Rectangle(0, 0, 0, 0);
            bool ful = false;
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (x == 1 && y == 1)
                        continue;
                    ful = (works[x, y] == true);
                    if (ful)
                        break;
                }
                if (ful)
                    break;
            }
            if (!ful)
            {
                return ret;
            }
            if (!(works[0, 0] && works[1, 0] && works[2, 0]))
            {
                ret.Y = 1;
                if (!(works[0, 1] && works[1, 1] && works[2, 1]))
                {
                    ret.Y = 2;
                }
            }
            if (!(works[0, 0] && works[0, 1] && works[0, 2]))
            {
                ret.X = 1;
                if (!(works[1, 0] && works[1, 1] && works[1, 2]))
                {
                    ret.X = 2;
                }
            }
            switch (ret.X)
            {
                case 1:

                    break;
                case 2:
                    break;
                default:
                    break;
            }
        }

        private bool InColorRange(ColorRange color, Color pixcolor)
        {
            return ((pixcolor.R >= color.R.max && pixcolor.R <= color.R.min) 
                && (pixcolor.G >= color.G.max && pixcolor.G <= color.G.min) 
                && (pixcolor.B >= color.B.max && pixcolor.B <= color.B.min));
        }*/
    }
    class ColorRange
    { //TODO Make vxWorks
        ColorRange()
        {
        }
        public Range R, G, B;
    }
    class Range
    { //TODO Make vxWorks
       public int max=0, min=0;
    }
}
