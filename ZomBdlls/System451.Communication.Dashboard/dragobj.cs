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
using System.Drawing.Drawing2D;
using System451.Communication.Dashboard.Properties;

namespace System451.Communication.Dashboard
{
    public partial class dragobj : UserControl
    {
        //Point obj = new Point(50, 50);
        //Point obj2 = new Point(50, 50);
        //Point objoff = new Point();
        //bool ina = false;
        dragableobj[] Drag1 = new dragableobj[10];
        
        public dragobj()
        {
            InitializeComponent();
            for (int i = 0; i < Drag1.Length; i++)
            {
                Drag1[i] = new dragableobj();
            }
            foreach (dragableobj dobj in Drag1)
            {
                dobj.Image = Resources.Apollo451Small;
                dobj.Location = new Point(50, 50);
                dobj.obj2 = new Point(50, 50);
                dobj.Size = new Size(90, 90);                
            }

            this.Invalidate();
        }

        private void dragobj_Paint(object sender, PaintEventArgs e)
        {
            using (Brush mb = new HatchBrush(HatchStyle.Weave, Color.Blue, Color.BurlyWood))
            {
                e.Graphics.FillRectangle(mb, 0, 0, this.Width, this.Height);
            }
            for (int i = Drag1.Length-1; i >= 0; i--)
            {
                e.Graphics.DrawImage(Drag1[i].Image, new Rectangle(Drag1[i].Location, Drag1[i].Size));
            }


        }

        private void dragobj_MouseMove(object sender, MouseEventArgs e)
        {
            foreach (dragableobj dobj in Drag1)
            {
                if (e.Button == MouseButtons.Left && dobj.ina)
                {
                    dobj.Location = new Point(dobj.obj2.X + (e.X - dobj.objoff.X), dobj.obj2.Y + (e.Y - dobj.objoff.Y));
                    this.Invalidate();
                    break;
                }

            }
        }

        private void dragobj_MouseDown(object sender, MouseEventArgs e)
        {
            foreach (dragableobj dobj in Drag1)
            {
                if (inare(e.Location, dobj.Location, dobj.Size.Width, dobj.Size.Height))
                {
                    dobj.ina = true;
                    dobj.objoff = e.Location;
                    break;
                }
                else
                    dobj.ina = false;
            }
        }

        private bool inare(Point point, Point obj, int width, int height)
        {
            return (point.X >= obj.X && point.X <= obj.X + width && point.Y >= obj.Y && point.Y <= obj.Y + height);
        }

        private void dragobj_Move(object sender, EventArgs e)
        {

        }

        private void dragobj_MouseUp(object sender, MouseEventArgs e)
        {
            foreach (dragableobj dobj in Drag1)
            {
                if (dobj.ina)
                {
                    dobj.obj2 = dobj.Location;
                    break;
                }
            }
        }
    }
    public class dragableobj
    {
        Point obj = new Point();
        public Point obj2 = new Point();
        public Point objoff = new Point();
        public bool ina = false;
        public dragableobj()
        {

        }
        public Point Location
        {
            get
            {
                return obj;
            }

            set
            {
                if (obj != value)
                    obj = value;
            }

        }
        private Size size;

        public Size Size
        {
            get
            {
                return size;
            }

            set
            {
                if (size != value)
                    size = value;
            }

        }
        private Image img;

        public Image Image
        {
            get
            {
                return img;
            }

            set
            {
                if (img != value)
                    img = value;
            }

        }

    }
}
