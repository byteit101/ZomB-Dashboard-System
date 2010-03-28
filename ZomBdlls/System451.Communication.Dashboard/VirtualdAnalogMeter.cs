/* 
 * Copyright (c)2008, Dustin Spicuzza
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * The name of the Dustin Spicuzza may not be used to endorse or promote 
 *       products derived from this software without specific prior written 
 *       permission.
 *
 * THIS SOFTWARE IS PROVIDED BY Dustin Spicuzza ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL Dustin Spicuzza BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
/*
 * Added To (and therefore modded slightly) the ZomB Dashboard System by Patrick Plenefisch 
 * Original source can be found at http://www.virtualroadside.com/download/Instruments-1.0.zip
 */

using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Forms;
using System451.Communication.Dashboard;

namespace Instruments {

	/// <summary>
	/// Awesome analog meter. :) 
	/// </summary>
	public class VirtualdAnalogMeter : Control,IDashboardControl {

		Bitmap bgImage = null;
		Graphics realGraphics = null;

		/// <summary>
		/// Constructor
		/// </summary>
        public VirtualdAnalogMeter()
        {
            this.DoubleBuffered = true;
			BackColor = Color.White;
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserPaint, true);
		}
        /// <summary>
        /// Change the Default Meter Size
        /// </summary>
        /// <remarks>Added For ZomB Dashboard</remarks>
        protected override Size DefaultSize
        {
            get
            {
                return new Size(212, 111);
            }
        }

		float r1x;
		float r1y;

		/// <summary>
		/// This does all of the drawing, but it definitely could
		/// be optimized... the biggest one is that the value
		/// could be drawn seperately from this, since thats the value
		/// that would probably be updated the most.
		/// </summary>
		private void CreateBackground() {

			float i;

			if ( Width < 1 || Height < 1)
				return;

			Bitmap bmp = new Bitmap(Width, Height);
			Graphics g = Graphics.FromImage(bmp);
			g.SmoothingMode = SmoothingMode.HighQuality;

			// frame
			using (Brush b = new SolidBrush(frameColor)){
				g.FillRectangle(b, ClientRectangle);
			}

			Rectangle meterLocation;

			// setup a clip rectangle for the meter itself
			if (stretch)
				meterLocation = new Rectangle(framePadding.Left, framePadding.Top, Width - framePadding.Horizontal, Height - framePadding.Vertical);
			else
				meterLocation = new Rectangle(framePadding.Left, framePadding.Top, Width - framePadding.Horizontal, internalPadding.Vertical + Width/2 - framePadding.Top);

			// set the clip rectangle
			g.IntersectClip(meterLocation);

			// fill meter with its background
			using (Brush b = new SolidBrush(BackColor)){
				g.FillRectangle(b, meterLocation);
			}

			// 1 is outer point, 2 is inner point
			r1x = (float)( meterLocation.Width - internalPadding.Horizontal ) / 2;
			r1y = (float)( meterLocation.Height - internalPadding.Vertical );
			float r2x;
			float r2y;

			// draw tiny ticks
			if ( tickTinyFrequency > 0 ) {
				using ( Pen p = new Pen(ForeColor, tickTinyWidth) ) {

					r2x = r1x - tickTinySize;
					r2y = r1y - tickTinySize;
					
					for ( i = minValue; i <= maxValue; i += tickTinyFrequency ) {
						if ( (tickSmallFrequency > 0 && (i - minValue) % tickSmallFrequency == 0) || (tickLargeFrequency > 0 && ( i - minValue ) % tickLargeFrequency == 0) )
							continue;
						PointF[] pts = GetLine(i, r1x, r1y, r2x, r2y);
						g.DrawLine(p, pts[0], pts[1]);
					}
				}
			}

			// draw small ticks
			if ( tickSmallFrequency > 0 ) {
				using ( Pen p = new Pen(ForeColor, tickSmallWidth) ) {

					r2x = r1x - tickSmallSize;
					r2y = r1y - tickSmallSize;

					for ( i = minValue; i <= maxValue; i += tickSmallFrequency ) {
						if ( tickLargeFrequency > 0 && (i - minValue) % tickLargeFrequency == 0 )
							continue;
						PointF[] pts = GetLine(i, r1x, r1y, r2x, r2y);
						g.DrawLine(p, pts[0], pts[1]);
					}
				}
			}

			// draw large ticks and numbers
			if ( tickLargeFrequency > 0 ) {
				using ( Pen p = new Pen(ForeColor, tickLargeWidth) ) {

					r2x = r1x - tickLargeSize;
					r2y = r1y - tickLargeSize;

					float r3x = r2x - Font.Height;
					float r3y = r2y - Font.Height;

					for ( i = minValue; i <= maxValue; i += tickLargeFrequency ) {
						PointF[] pts = GetLine(i, r1x, r1y, r2x, r2y);
						g.DrawLine(p, pts[0], pts[1]);

						SizeF sz = g.MeasureString(i.ToString(), Font);
						pts = GetLine(i, r1x, r1y, r3x, r3y);
						g.DrawString(i.ToString(), Font, p.Brush, pts[1].X - sz.Width / 2, pts[1].Y - sz.Height / 2);

					}
				}
			}

			// finally, the title
			if ( Text != "" ) {
				using ( Brush b = new SolidBrush(ForeColor) ) {
					SizeF sz = g.MeasureString(Text, Font);
					g.DrawString(Text, Font, b, framePadding.Left + ( meterLocation.Width / 2 ) - sz.Width / 2, framePadding.Top + ( meterLocation.Height * 3 ) / 4 - sz.Height / 2);
				}
			}

			g.Dispose();

			// done
			if ( bgImage != null )
				bgImage.Dispose();

			bgImage = bmp;

			if ( realGraphics != null )
				DrawMeter(realGraphics);
		}

		private PointF[] GetLine(float value, float r1x, float r1y, float r2x, float r2y) {
			
			PointF[] p = new PointF[2];

			float angle = ((value > maxValue ? maxValue : (value < minValue ? minValue : value)) - minValue) * ( ( (float)Math.PI - tickStartAngle * 2 ) / ( maxValue - minValue ) ) + tickStartAngle;

			// need to figure out where to calculate from

			p[0] = new PointF((float)( framePadding.Left + internalPadding.Left + ( r1x - r1x * Math.Cos(angle) ) ), (float)( framePadding.Top + internalPadding.Top + ( r1y - r1y * Math.Sin(angle) ) ));
			p[1] = new PointF((float)( framePadding.Left + internalPadding.Left + ( r1x - r2x * Math.Cos(angle) ) ), (float)( framePadding.Top + internalPadding.Top + ( r1y - r2y * Math.Sin(angle) ) ));

			return p;
		}

		private void DrawMeter(Graphics g) {

			g.DrawImageUnscaled(bgImage, 0, 0);

			// draw value -- this can be optimized, draw this outside of this routine
			using ( Pen p = new Pen(pointerColor, tickLargeWidth) ) {
				PointF[] pts = GetLine(value, r1x, r1y, 0, 0);
				g.SmoothingMode = SmoothingMode.HighQuality;
				g.DrawLine(p, pts[0], pts[1]);
			}
		}
		
		/// <summary>
		/// Title to display on the meter
		/// </summary>
		[Category("Meter"),Description("Title to display on the meter")]
		public override string Text {
			get {
				return base.Text;
			}
			set {
				base.Text = value;
				CreateBackground();
			}
		}

		private float value = 0;

		/// <summary>
		/// Value of meter
		/// </summary>
		[DefaultValue(0), Category("Meter"), Description("Value of meter")]
		public float Value {
			get { return this.value; }
			set {
				if ( value != this.value ) {
					this.value = value;
					DrawMeter(realGraphics);
				}
			}
		}


		private float maxValue = 1;

		/// <summary>
		/// Maximum value of the meter
		/// </summary>
		[DefaultValue(15), Category("Meter"), Description("Maximum value of the meter")]
		public float MaxValue {
			get { return maxValue; }
			set { 
				maxValue = value;
				CreateBackground();
			}
		}

		private float minValue = -1;

		/// <summary>
		/// Minimum value of the meter
		/// </summary>
		[DefaultValue(0), Category("Meter"), Description("Minimum value of the meter")]
		public float MinValue {
			get { return minValue; }
			set { 
				minValue = value;
				CreateBackground();
			}
		}

		private float tickTinyFrequency = 0.01F;

		/// <summary>
		/// Frequency of tiny ticks (0 to disable)
		/// </summary>
		[DefaultValue(0.2F), Category("Meter"), Description("Frequency of tiny ticks (0 to disable)")]
		public float TickTinyFrequency {
			get { return tickTinyFrequency; }
			set {
				tickTinyFrequency = value;
				CreateBackground();
			}
		}


		private float tickSmallFrequency = 0.05F;

		/// <summary>
		/// Frequency of small ticks (0 to disable)
		/// </summary>
		[DefaultValue(1F), Category("Meter"), Description("Frequency of small ticks (0 to disable)")]
		public float TickSmallFrequency {
			get { return tickSmallFrequency; }
			set {
				tickSmallFrequency = value;
				CreateBackground();
			}
		}

		private float tickLargeFrequency = 0.25F;

		/// <summary>
		/// Frequency of large ticks (0 to disable)
		/// </summary>
		[DefaultValue(5F), Category("Meter"), Description("Frequency of large ticks (0 to disable)")]
		public float TickLargeFrequency {
			get { return tickLargeFrequency; }
			set { 
				tickLargeFrequency = value;
				CreateBackground();
			}
		}

		private float tickTinyWidth = 1F;

		/// <summary>
		/// Stroke width of tiny ticks
		/// </summary>
		[DefaultValue(1F), Category("Meter"), Description("Stroke width of tiny ticks")]
		public float TickTinyWidth {
			get { return tickTinyWidth; }
			set {
				tickTinyWidth = value;
				CreateBackground();
			}
		}

		private float tickSmallWidth = 1F;

		/// <summary>
		/// Stroke width of small ticks
		/// </summary>
		[DefaultValue(1F), Category("Meter"), Description("Stroke width of small ticks")]
		public float TickSmallWidth {
			get { return tickSmallWidth; }
			set {
				tickSmallWidth = value;
				CreateBackground();
			}
		}

		private float tickLargeWidth = 2F;

		/// <summary>
		/// Stroke width of large ticks
		/// </summary>
		[DefaultValue(2F), Category("Meter"), Description("Stroke width of large ticks")]
		public float TickLargeWidth {
			get { return tickLargeWidth; }
			set {
				tickLargeWidth = value;
				CreateBackground();
			}
		}

		private float tickStartAngle = 20 * (float)(Math.PI / 180);

		/// <summary>
		/// Angle the meter starts display at in degrees
		/// </summary>
		[DefaultValue(20 * (float)( Math.PI / 180 )), Category("Meter"), Description("Angle the meter starts display at in degrees")]
		public float TickStartAngle {
			get {
				return tickStartAngle * (float)(180 / Math.PI); 
			}
			set {
				if ( value < 0 || value > 75 )
					throw new Exception("The angle must be between a value of 0 and 75 degrees");
				tickStartAngle = value * (float)(Math.PI / 180);
				CreateBackground();
			}
		}



		private float tickTinySize = 5F;

		/// <summary>
		/// Size of the tiny tick marks
		/// </summary>
		[DefaultValue(5F), Category("Meter")]
		public float TickTinySize {
			get { return tickTinySize; }
			set { 
				tickTinySize = value;
				CreateBackground();
			}
		}

		
		private float tickSmallSize = 15F;

		/// <summary>
		/// Size of the small tick marks
		/// </summary>
		[DefaultValue(15F), Category("Meter")]
		public float TickSmallSize {
			get { return tickSmallSize; }
			set { 
				tickSmallSize = value;
				CreateBackground();
			}
		}

		private float tickLargeSize = 20F;

		/// <summary>
		/// Size of the large tick marks
		/// </summary>
		[DefaultValue(20F), Category("Meter")]
		public float TickLargeSize {
			get { return tickLargeSize; }
			set {
				tickLargeSize = value;
				CreateBackground();
			}
		}

		/// <summary>
		/// Background color of the meter
		/// </summary>
		[DefaultValue(typeof(Color),"White")]
		public override Color BackColor {
			get {
				return base.BackColor;
			}
			set {
				base.BackColor = value;
				CreateBackground();
			}
		}

		/// <summary>
		/// Font of the control
		/// </summary>
		public override Font Font {
			get {
				return base.Font;
			}
			set {
				base.Font = value;
				CreateBackground();
			}
		}

		/// <summary>
		/// Color of tickmarks and text on meter
		/// </summary>
		public override Color ForeColor {
			get {
				return base.ForeColor;
			}
			set {
				base.ForeColor = value;
				CreateBackground();
			}
		}

		private Color pointerColor = Color.Red;

		/// <summary>
		/// Color of the primary pointer
		/// </summary>
		[DefaultValue(typeof(Color),"Red"), Category("Meter"), Description("Color of the primary pointer")]
		public Color PointerColor {
			get { return pointerColor; }
			set {
				pointerColor = value;
				CreateBackground();
			}
		}

		private Color frameColor = Color.Black;

		/// <summary>
		/// Color of the frame of the meter
		/// </summary>
		[Category("Meter")]
		public Color FrameColor {
			get { return frameColor; }
			set { 
				frameColor = value;
				CreateBackground();
			}
		}

		private Padding framePadding = new Padding(0);
		/// <summary>
		/// Size of the frame around the primary part of the meter
		/// </summary>
		[Category("Meter")]
		public Padding FramePadding {
			get { return framePadding; }
			set { 
				framePadding = value;
				CreateBackground();
			}
		}

		private Padding internalPadding = new Padding(5);
		/// <summary>
		/// Internal padding for the meter display
		/// </summary>
		[Category("Meter")]
		public Padding InternalPadding {
			get { return internalPadding; }
			set { 
				internalPadding = value;
				CreateBackground();
			}
		}

		private bool stretch = false;

		/// <summary>
		/// Set to true if the meter should fill the entire control. Set to false to maintain a
		/// rectangular outline.
		/// </summary>
		[DefaultValue(false),Category("Meter"),Description("Set to true if the meter should fill the entire control. Set to false to maintain a rectangular outline.")]
		public bool Stretch {
			get { return stretch; }
			set { 
				stretch = value;
				CreateBackground();
			}
		}

		/// <summary>
		/// Overrides paint event
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);

			if ( bgImage != null )
				DrawMeter(e.Graphics);
		}

		/// <summary>
		/// Overrides resize event
		/// </summary>
		/// <param name="e"></param>
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			if ( this.realGraphics != null )
				this.realGraphics.Dispose();
			this.realGraphics = this.CreateGraphics();
			CreateBackground();
		}


        #region IDashboardControl Members

        [DefaultValue("VdAnalog1"), Category("ZomB"), Description("What this control will get the value of from the packet Data")]
        public string BindToInput
        {
            get
            {
                return bti;
            }
            set { bti = value; }
        }
        private string bti = "VdAnalog1";
        string[] IDashboardControl.ParamName
        {
            get{
                return new string[] { BindToInput };
            }
            set{
                BindToInput = value[0];
            }
        }

        string IDashboardControl.Value
        {
            get
            {
                return this.Value.ToString();
            }
            set
            {
                this.Value = float.Parse(value);
            }
        }

        string IDashboardControl.DefalutValue
        {
            get { return this.Value.ToString(); }
        }

        delegate void UpdaterDelegate();

        void IDashboardControl.Update()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdaterDelegate(Update));
            }
            else
            {
                this.Invalidate();
            }
        }

        #endregion
    }
}
