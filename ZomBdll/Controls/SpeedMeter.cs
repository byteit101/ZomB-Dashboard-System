﻿/*
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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace System451.Communication.Dashboard.Controls
{
    [ToolboxBitmap(typeof(icofinds), "System451.Communication.Dashboard.TBB.DashboardSpeed.bmp")]
    public class RoundSpeedMeter : ZomBControl
    {
        float speedval = 0;

        public RoundSpeedMeter()
        {
            this.DoubleBuffered = true;
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.RoundSpeedMeter_Paint);
            speedval = 0;
            this.ControlName = "pwm1";
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(125, 75);
            }
        }

        [DefaultValue("0"), Category("ZomB"), Description("The Value of the Speed Meter")]
        public float Value
        {
            get
            {
                return speedval / .95f;
            }
            set
            {
                speedval = value * .95f;
                this.Invalidate();
            }
        }

        public override void UpdateControl(ZomBDataObject value)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Utils.ZomBDataFunction(UpdateControl), value);
            }
            else
            {
                this.Value = float.Parse(value);
            }
        }
        private void RoundSpeedMeter_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.Clear(this.BackColor);
            e.Graphics.ScaleTransform((float)this.Width / 125f, (float)this.Height / 75f);

            GraphicsPath gp = new GraphicsPath();
            gp.AddBezier(62.5f, 22f, 40.180261f, 22, 19.886554f, 30.646245f, 4.78125f, 44.78125f);
            gp.AddLine(4.78125f, 44.78125f, 20.03125f, 60.03125f);
            gp.AddBezier(20.03125f, 60.03125f, 31.224609f, 49.79343f, 46.134196f, 43.5625f, 62.5f, 43.5625f);
            gp.AddBezier(62.5f, 43.5625f, 78.865804f, 43.5625f, 93.775641f, 49.79343f, 104.969f, 60.03125f);
            gp.AddLine(104.969f, 60.03125f, 120.21875f, 44.78125f);
            gp.AddBezier(120.21875f, 44.78125f, 105.11345f, 30.646245f, 84.819737f, 22f, 62.5f, 22f);
            gp.CloseFigure();
            using (Brush bb = new SolidBrush(Color.FromArgb(128, 128, 0)))
            {
                e.Graphics.FillPath(bb, gp);
            }
            Region r = new Region(gp);
            float startangle = -90f;
            float sweepangle = (speedval * 45f);
            r.Intersect(GetPie(-22f, 22f, 169f, 169f, startangle, sweepangle));
            using (Brush flufbrush = new LinearGradientBrush(new Point(4, 0), new Point(121, 0), Color.Red, Color.Lime))
            {
                e.Graphics.FillRegion(flufbrush, r);
            }
            using (Pen p = new Pen(ForeColor))
            {
                e.Graphics.DrawLine(p,
                    new PointF((float)Math.Cos((startangle + sweepangle) * Math.PI / 180) * (85.5f) + 62.5f,
                               (float)Math.Sin((startangle + sweepangle) * Math.PI / 180) * (85.5f) + 107.5f),
                    new PointF((float)Math.Cos((startangle + sweepangle) * Math.PI / 180) * (60f) + 62.5f,//2.5,46.5
                               (float)Math.Sin((startangle + sweepangle) * Math.PI / 180) * (60f) + 106.5f));//24.5
            }
            using (Brush b = new SolidBrush(ForeColor))
            {
                e.Graphics.DrawString(this.Value.ToString("0.00"), Font, b,
                    (125f - e.Graphics.MeasureString(this.Value.ToString("0.00"), Font).Width) / 2f,
                    73 - (e.Graphics.MeasureString(this.Value.ToString("0.00"), Font).Height));
            }



        }

        private Region GetPie(float x, float y, float width, float height, float startangle, float sweepangle)
        {
            GraphicsPath elp = new GraphicsPath();
            elp.AddEllipse(x, y, width, height);
            GraphicsPath pe = new GraphicsPath();
            PointF last = new PointF((float)Math.Cos(startangle * Math.PI / 180) * (width / 2) + x + (width / 2), (float)Math.Sin(startangle * Math.PI / 180) * (height / 2) + y + (height / 2));
            pe.AddLine(new PointF((x + width / 2), (y + height / 2)), last);

            float sa = startangle % 360;
            while (sa < 0) sa += 360;

            float sw = (startangle + sweepangle) % 360;
            while (sw < 0) sw += 360;
            if (sw == 180)
            {
                if (sweepangle < 0)
                {
                    sw = -180;
                }
            }
            else if (sw > 180)
                sw = -sw;

            switch ((int)(sa / 90))
            {
                case 0: //lower right VI
                    if (((sw / 90)) >= 0)
                    {
                        pe.AddLine(last, new PointF((x + width) + 1, (y + height) + 1));
                        last = new PointF((x + width) + 1, (y + height) + 1);

                        if (((sw / 90)) >= 1)
                        {
                            pe.AddLine(last, new PointF((x) - 1, (y + height) + 1));
                            last = new PointF((x) - 1, (y + height) + 1);
                        }
                    }
                    else
                    {
                        if ((sa / 90) != 0)
                        {
                            pe.AddLine(last, new PointF((x + width) + 1, (y + height) + 1));
                            last = new PointF((x + width) + 1, (y + height) + 1);
                        }

                        pe.AddLine(last, new PointF((x + width) + 1, (y) - 1));
                        last = new PointF((x + width) + 1, (y) - 1);

                        if (((sw / 90)) >= -3)
                        {
                            pe.AddLine(last, new PointF((x) - 1, (y) - 1));
                            last = new PointF((x) - 1, (y) - 1);
                        }
                    }
                    break;
                case 1://Lower left III
                    sw = (startangle + sweepangle - 90) % 360;
                    while (sw < 0) sw += 360;
                    if (sw == 180)
                    {
                        if (sweepangle < 0)
                        {
                            sw = -180;
                        }
                    }
                    else if (sw > 180)
                        sw = -sw;
                    if (((sw / 90)) >= 0)
                    {
                        pe.AddLine(last, new PointF((x) - 1, (y + height) + 1));
                        last = new PointF((x) - 1, (y + height) + 1);

                        if (((sw / 90)) >= 1)
                        {
                            pe.AddLine(last, new PointF((x) - 1, (y) - 1));
                            last = new PointF((x) - 1, (y) - 1);
                        }
                    }
                    else
                    {
                        if ((sa / 90) != 1)
                        {
                            pe.AddLine(last, new PointF((x) - 1, (y + height) + 1));
                            last = new PointF((x) - 1, (y + height) + 1);
                        }
                        pe.AddLine(last, new PointF((x + width) + 1, (y + height) + 1));
                        last = new PointF((x + width) + 1, (y + height) + 1);
                        if (((sw / 90)) >= -3)
                        {
                            pe.AddLine(last, new PointF((x + width) + 1, (y) - 1));
                            last = new PointF((x + width) + 1, (y) - 1);
                        }
                    }
                    break;
                case 2://Upper left II
                    sw = (startangle + sweepangle - 180) % 360;
                    while (sw < 0) sw += 360;
                    if (sw == 180)
                    {
                        if (sweepangle < 0)
                        {
                            sw = -180;
                        }
                    }
                    else if (sw > 180)
                        sw = -sw;

                    if (((sw / 90)) >= 0)
                    {
                        pe.AddLine(last, new PointF((x) - 1, (y) - 1));
                        last = new PointF((x) - 1, (y) - 1);

                        if (((sw / 90)) >= 1)
                        {
                            pe.AddLine(last, new PointF((x + width) + 1, (y) - 1));
                            last = new PointF((x + width) + 1, (y) - 1);
                        }
                    }
                    else
                    {
                        if ((sa / 90) != 2)
                        {
                            pe.AddLine(last, new PointF((x) - 1, (y) - 1));
                            last = new PointF((x) - 1, (y) - 1);
                        }
                        pe.AddLine(last, new PointF((x) - 1, (y + height) + 1));
                        last = new PointF((x) - 1, (y + height) + 1);

                        if (((sw / 90)) >= -3)
                        {
                            pe.AddLine(last, new PointF((x + width) + 1, (y + height) + 1));
                            last = new PointF((x + width) + 1, (y + height) + 1);
                        }
                    }
                    break;
                case 3://Upper right
                    sw = (startangle + sweepangle + 90) % 360;
                    while (sw < 0) sw += 360;
                    if (sw == 180)
                    {
                        if (sweepangle < 0)
                        {
                            sw = -180;
                        }
                    }
                    else if (sw > 180)
                        sw = -sw;

                    if (((int)(sw / 90)) >= 0)
                    {
                        pe.AddLine(last, new PointF((x + width) + 1, (y) - 1));
                        last = new PointF((x + width) + 1, (y) - 1);

                        if (((int)(sw / 90)) >= 1)
                        {
                            pe.AddLine(last, new PointF((x + width) + 1, (y + height) + 1));
                            last = new PointF((x + width) + 1, (y + height) + 1);
                        }
                    }
                    else
                    {
                        if ((sa / 90) != 3)
                        {
                            pe.AddLine(last, new PointF((x + width) + 1, (y) - 1));
                            last = new PointF((x + width) + 1, (y) - 1);
                        }
                        pe.AddLine(last, new PointF((x) - 1, (y) - 1));
                        last = new PointF((x) - 1, (y) - 1);

                        if ((sw / 90) >= -3)
                        {
                            pe.AddLine(last, new PointF((x) - 1, (y + height) + 1));
                            last = new PointF((x) - 1, (y + height) + 1);
                        }
                    }
                    break;
                default:
                    MessageBox.Show("AH! Math Error (in RSM, GetPie)");
                    throw new Exception("Math Error");
            }

            last = new PointF((float)Math.Cos((startangle + sweepangle) * Math.PI / 180) * (width / 2) + x + (width / 2), (float)Math.Sin((startangle + sweepangle) * Math.PI / 180) * (height / 2) + y + (height / 2));
            pe.AddLine(last, new PointF((x + width / 2), (y + height / 2)));

            Region ret = new Region(elp);
            ret.Intersect(pe);
            return ret;
        }
    }
}
