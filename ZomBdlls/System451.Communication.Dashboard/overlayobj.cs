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
using System.Threading;

namespace System451.Communication.Dashboard
{

    [DefaultEvent("SourcePaint")]
    public partial class overlayobj : UserControl
    {
        //oj mouse = new oj();
        Point cmouse = new Point();
        //public delegate void sourcePaint(object sender, PaintEventArgs e);
        public event PaintEventHandler SourcePaint;
        //public delegate void overlayPaint(object sender, PaintEventArgs e);
        public event PaintEventHandler OverlayPaint;
        public overlayobj()
        {
            InitializeComponent();
            SourcePaint += new PaintEventHandler(overlayobj_SourcePaint);
            OverlayPaint += new PaintEventHandler(overlayobj_OverlayPaint);
        }

        void overlayobj_OverlayPaint(object sender, PaintEventArgs e)
        {

        }

        void overlayobj_SourcePaint(object sender, PaintEventArgs e)
        {

        }

        private bool overlaymode = false;
        /// <summary>
        /// Overlay the overlay image?
        /// </summary>
        [DefaultValue(false), Category("ZomB"), Description("Put the overlay image on the source image with 50% trancparency")]
        public bool Overlay
        {
            get
            {
                return overlaymode;
            }

            set
            {
                if (overlaymode != value)
                    overlaymode = value;
                if (value)
                {
                    this.Width = this.Height;
                }
                else
                    this.Width = Height * 2;
                this.Invalidate();
            }

        }
        private bool corpoints = true;
        /// <summary>
        /// show the cross eye
        /// </summary>
        [DefaultValue(true), Category("ZomB"), Description("Show the crosseye")]
        public bool RelatePoints
        {
            get
            {
                return corpoints;
            }

            set
            {
                if (corpoints != value)
                    corpoints = value;
                this.Invalidate();
            }

        }
        private int alphaovmode = 127;
        /// <summary>
        /// The Alpha of the overlay Image
        /// </summary>
        [DefaultValue(127), Category("ZomB"), Description("Alpha of the overlay")]
        public int Alpha
        {
            get
            {
                return alphaovmode;
            }

            set
            {
                if (alphaovmode != value)
                    alphaovmode = value;
                if (Overlay)
                    this.Invalidate();
            }

        }

        private void overlayobj_Paint(object sender, PaintEventArgs e)
        {
            if (Overlay)
            {
                int w = this.Width;
                int h = this.Height;
                SourcePaint(sender, e);
                Bitmap ovla = new Bitmap(w, h);
                Graphics go = Graphics.FromImage(ovla);
                PaintEventArgs e2 = new PaintEventArgs(go, new Rectangle(0, 0, w, h));
                OverlayPaint(sender, e2);
                for (int x = 0; x < w; x++)
                {
                    for (int y = 0; y < h; y++)
                    {
                        ovla.SetPixel(x, y, Color.FromArgb(Alpha, ovla.GetPixel(x, y)));
                    }
                }
                e.Graphics.DrawImage(ovla, 0, 0);

            }
            else
            {
                //lock (mouse)
                //{
                    int w = this.Width;
                    int h = this.Height;
                    PaintEventArgs e2 = new PaintEventArgs(e.Graphics, new Rectangle(0, 0, h, h));
                    SourcePaint(sender, e2);
                    Bitmap ovla = new Bitmap(w, h);
                    Graphics go = Graphics.FromImage(ovla);
                    e2 = new PaintEventArgs(go, new Rectangle(h, 0, h, h));
                    OverlayPaint(sender, e2);
                    e.Graphics.DrawImage(ovla, h, 0);
                    e.Graphics.DrawLine(Pens.Black, h, 0, h, h);
                    if (corpoints)
                    {
                        if (cmouse.X < this.Height)
                        {
                            e.Graphics.DrawLine(Pens.Gray, cmouse.X, 0, cmouse.X, Height);
                            e.Graphics.DrawLine(Pens.Gray, 0, cmouse.Y, Height, cmouse.Y);
                            e.Graphics.DrawLine(Pens.Black, cmouse.X + Height, 0, cmouse.X + Height, Height);
                            e.Graphics.DrawLine(Pens.Black, Height, cmouse.Y, Height + Height, cmouse.Y);
                        }
                        else
                        {
                            cmouse.X -= this.Height;
                            e.Graphics.DrawLine(Pens.Black, cmouse.X, 0, cmouse.X, Height);
                            e.Graphics.DrawLine(Pens.Black, 0, cmouse.Y, Height, cmouse.Y);
                            e.Graphics.DrawLine(Pens.Gray, cmouse.X + Height, 0, cmouse.X + Height, Height);
                            e.Graphics.DrawLine(Pens.Gray, Height, cmouse.Y, Height + Height, cmouse.Y);
                        }
                    }
                //}
            }
        }

        private void overlayobj_MouseMove(object sender, MouseEventArgs e)
        {
            if (!Overlay)
            {
                cmouse = e.Location;

                    this.Invalidate();

                
            }
        }
    }
}
