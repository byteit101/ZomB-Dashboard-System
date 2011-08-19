/*
 * ZomB Dashboard System <http://firstforge.wpi.edu/sf/projects/zombdashboard>
 * Copyright (C) 2011, Patrick Plenefisch and FIRST Robotics Team 451 "The Cat Attack"
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace System451.Communication.Dashboard.WPF.Controls.Designer.PrimitiveControls
{
    class GraphScale : Decorator
    {
        public SolidColorBrush Foreground
        {
            get { return (SolidColorBrush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Foreground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(SolidColorBrush), typeof(GraphScale), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool ShowScale
        {
            get { return (bool)GetValue(ShowScaleProperty); }
            set { SetValue(ShowScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowScale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowScaleProperty =
            DependencyProperty.Register("ShowScale", typeof(bool), typeof(GraphScale), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public double MaxX
        {
            get { return (double)GetValue(MaxXProperty); }
            set { SetValue(MaxXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxXProperty =
            DependencyProperty.Register("MaxX", typeof(double), typeof(GraphScale), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double MinX
        {
            get { return (double)GetValue(MinXProperty); }
            set { SetValue(MinXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinXProperty =
            DependencyProperty.Register("MinX", typeof(double), typeof(GraphScale), new FrameworkPropertyMetadata(-300.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double MaxY
        {
            get { return (double)GetValue(MaxYProperty); }
            set { SetValue(MaxYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxYProperty =
            DependencyProperty.Register("MaxY", typeof(double), typeof(GraphScale), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double MinY
        {
            get { return (double)GetValue(MinYProperty); }
            set { SetValue(MinYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinYProperty =
            DependencyProperty.Register("MinY", typeof(double), typeof(GraphScale), new FrameworkPropertyMetadata(-1.0, FrameworkPropertyMetadataOptions.AffectsRender));

        //Step function based off of the Perl Chart::Math::Axis library (http://search.cpan.org/~adamk/Chart-Math-Axis-1.06/)
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (!ShowScale)
                return;
            var linepen = new Pen(Foreground, 1.0);
            var tface = new Typeface("Verdana");
            var emz = 12.0;
            var fmstring = "0.000";
            var maxytext = new FormattedText("-451.236789", CultureInfo.CurrentUICulture, System.Windows.FlowDirection.LeftToRight, tface, emz, Foreground);
            var minytext = new FormattedText("0", CultureInfo.CurrentUICulture, System.Windows.FlowDirection.LeftToRight, tface, emz, Foreground);
            var height = Math.Max(minytext.Height, maxytext.Height);
            var workheight = this.ActualHeight - (height * 2);
            var numofmarkers = Math.Floor(this.ActualHeight / (height * 2.0));
            
            var mag = Math.Min(299, Math.Max(GetMagnitude(MaxY), GetMagnitude(MinY)));
            var step = Math.Pow(10, mag + 1);
            var goodmax = (Math.Floor(MaxY / step) + 1) * step;
            var goodmin = (Math.Ceiling(MinY / step) - 1) * step;
            if (MinY == 0)
                goodmin = 0;

            int li = 0;
            while (++li < 1000)
            {
                //reduce
                var nextstep = Reduce(step);
                var nextgoodmax = (Math.Floor(MaxY / nextstep)) * nextstep;
                var nextgoodmin = (Math.Ceiling(MinY / nextstep)) * nextstep;

                if (((nextgoodmax - nextgoodmin) / nextstep) > numofmarkers)
                    break;
                step = nextstep;
                goodmax = nextgoodmax;
                goodmin = nextgoodmin;
                
            }
            drawingContext.DrawLine(linepen, new Point(-2, 0), new Point(-2, this.ActualHeight));
            var vdist = (goodmax - goodmin) / (numofmarkers);
            if (vdist > 1)
                fmstring = "0";
            else if (vdist > 0.1)
                fmstring = "0.0";
            else if (vdist > 0.01)
                fmstring = "0.00";
            for (int i = 0; i < numofmarkers+1; i++)
            {
                var realy = Math.Round(goodmax - vdist * (i), 3);
                var virty = ((goodmax - realy) * (this.ActualHeight * (goodmax - goodmin) / (MaxY - MinY))) / (goodmax - goodmin) + ((MaxY - goodmax) * this.ActualHeight / (MaxY - MinY));
                var localtext = new FormattedText(realy.ToString(fmstring), CultureInfo.CurrentUICulture, System.Windows.FlowDirection.LeftToRight, tface, emz, Foreground);
                
                drawingContext.DrawText(localtext, new Point(-localtext.Width - 5, virty-(localtext.Height/2.0)));
                drawingContext.DrawLine(linepen, new Point(-2, virty), new Point(-4, virty));
            }

            drawingContext.DrawLine(linepen, new Point(0, ActualHeight + 2.0), new Point(ActualWidth, ActualHeight + 2.0));
            drawingContext.DrawLine(linepen, new Point(0, ActualHeight + 2), new Point(0, ActualHeight + 4));
            drawingContext.DrawLine(linepen, new Point(ActualWidth, ActualHeight + 2), new Point(ActualWidth, ActualHeight + 4));
            var TextX = new FormattedText(MinX.ToString(), CultureInfo.CurrentUICulture, System.Windows.FlowDirection.LeftToRight, tface, emz, Foreground);
            drawingContext.DrawText(TextX, new Point(TextX.Width / -2, ActualHeight + 5));
            TextX = new FormattedText(MaxX.ToString(), CultureInfo.CurrentUICulture, System.Windows.FlowDirection.LeftToRight, tface, emz, Foreground);
            drawingContext.DrawText(TextX, new Point(ActualWidth + TextX.Width / -2, ActualHeight + 5));

            for (int i = 1; i < Math.Floor(ActualWidth/80.0); i++)
            {
                var realx = Math.Round(MinX + ((i * 80.0 * (MaxX - MinX) / ActualWidth)) * ActualWidth / (Math.Floor(ActualWidth / 80.0) * 80.0));
                var virtx = (((realx - MinX) * (Math.Floor(ActualWidth / 80.0) * 80.0)) / ActualWidth) * ActualWidth / (MaxX - MinX)* ActualWidth / (Math.Floor(ActualWidth / 80.0) * 80.0);
                drawingContext.DrawLine(linepen, new Point(virtx, ActualHeight + 2), new Point(virtx, ActualHeight + 4));
                TextX = new FormattedText(realx.ToString(), CultureInfo.CurrentUICulture, System.Windows.FlowDirection.LeftToRight, tface, emz, Foreground);
                drawingContext.DrawText(TextX, new Point(virtx - TextX.Width / 2, ActualHeight + 5));
            }
        }

        private double Reduce(double step)
        {
            long medusa;
            int exp;
            GetFloatParts(step, out medusa, out exp);
            if (medusa % 5 == 0)
            {
                return step * (2.0 / medusa);

            }
            else if (medusa % 2  == 0)
            {
                return step * (1.0 / medusa*2);

            }
            else if (medusa % 1 == 0)
            {
                return step * (5.0 / (medusa*10.0));

            }
            else
            {
                //return 4;
                //wait, what?
                throw new ArgumentOutOfRangeException();
            }
        }

        private double GetMagnitude(double value)
        {
            if (value == 0)
                return 0;
            long medusa;
            int exp;
            GetFloatParts(value, out medusa, out exp);
            return medusa + exp - 1;
        }

        /// <summary>
        /// Returns the Mantissa and exponent (ex: 1e7 returns 1 and 7)
        /// </summary>
        /// <param name="value">Initial value</param>
        /// <param name="mantissa">place to put mantisessa</param>
        /// <param name="exponent">place to put exponent</param>
        /// <returns>Is it negative?</returns>
        public static bool GetFloatParts(double value, out long mantissa, out int exponent)
        {

            //based on code from http://stackoverflow.com/questions/389993/extracting-mantissa-and-exponent-from-double-in-c
            // Translate the double into sign, exponent and mantissa.
            long bits = BitConverter.DoubleToInt64Bits(value);
            // Note that the shift is sign-extended, hence the test against -1 not 1
            bool negative = (bits < 0);
            exponent = (int)((bits >> 52) & 0x7ffL);
            mantissa = bits & 0x000fffffffffffffL;            

            // Subnormal numbers; exponent is effectively one higher,
            // but there's no extra normalisation bit in the mantissa
            if (exponent == 0)
            {
                exponent++;
            }
            // Normal numbers; leave exponent as it is but add extra
            // bit to the front of the mantissa
            else
            {
                mantissa = mantissa | (1L << 52);
            }

            // Bias the exponent. It's actually biased by 1023, but we're
            // treating the mantissa as m.0 rather than 0.m, so we need
            // to subtract another 52 from it.
            exponent -= 1075;

            if (mantissa == 0)
            {
                return negative;
            }

            if ((value % 1) != 0)//if decimal
            {
                exponent = 2-Math.Abs(value % 1).ToString().Length;
                mantissa = (long)Math.Abs(value * Math.Pow(10, Math.Abs(value % 1).ToString().Length-2));
            }

            /* Normalize */
            while ((mantissa & 1) == 0)
            {    /*  i.e., Mantissa is even */
                mantissa >>= 1;
                exponent++;
            }

            return negative;
        }
    }
}
