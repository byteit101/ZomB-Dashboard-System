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
using System.Windows.Controls;
using System.Windows;

namespace System451.Communication.Dashboard.WPF.Controls
{
    public class FlowPropertyGrid : Panel
    {
        double recmaxr, recmaxl, recmaxh;
        
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(FlowPropertyGrid), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsArrange));

        public double LeftCollumnWidth
        {
            get { return (double)GetValue(LeftCollumnWidthProperty); }
            set { SetValue(LeftCollumnWidthProperty, value); }
        }

        public static readonly DependencyProperty LeftCollumnWidthProperty = DependencyProperty.Register("LeftCollumnWidth", typeof(double), typeof(FlowPropertyGrid), new UIPropertyMetadata(double.NaN));

        public double RightCollumnWidth
        {
            get { return (double)GetValue(RightCollumnWidthProperty); }
            set { SetValue(RightCollumnWidthProperty, value); }
        }

        public static readonly DependencyProperty RightCollumnWidthProperty = DependencyProperty.Register("RightCollumnWidth", typeof(double), typeof(FlowPropertyGrid), new UIPropertyMetadata(double.NaN));
        
        public double CollumnPadding
        {
            get { return (double)GetValue(CollumnPaddingProperty); }
            set { SetValue(CollumnPaddingProperty, value); }
        }

        public static readonly DependencyProperty CollumnPaddingProperty = DependencyProperty.Register("CollumnPadding", typeof(double), typeof(FlowPropertyGrid), new UIPropertyMetadata(0.0));

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Children.Count < 1)
                return new Size(0, 0);
            Size InfiniteSize = new Size(Double.PositiveInfinity, Double.PositiveInfinity);


            UIElement child = null;
            double maxl = 0, maxr = 0, maxh = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                child = Children[i];
                child.Measure(InfiniteSize);
                if (i % 2 == 0)//left side
                {
                    maxl = Math.Max(maxl, child.DesiredSize.Width);
                }
                else
                {
                    maxr = Math.Max(maxr, child.DesiredSize.Width);
                }
                maxh = Math.Max(maxh, child.DesiredSize.Height);
            }
            if (maxr == 0)//1 element
                maxr = maxl;
            if (LeftCollumnWidth == LeftCollumnWidth)//not NaN
                maxl = LeftCollumnWidth;
            if (RightCollumnWidth == RightCollumnWidth)//not NaN
                maxr = RightCollumnWidth;
            recmaxl = maxl;
            recmaxr = maxr;
            recmaxh = maxh;
            if (Orientation == Orientation.Horizontal)
            {
                double numrows = Math.Ceiling(Children.Count / 2.0);
                numrows = ((numrows * recmaxh) > availableSize.Height) ? Math.Floor(availableSize.Height / recmaxh) : numrows;
                double numcols = Math.Ceiling(Math.Ceiling(Children.Count / 2.0) / numrows);
                return new Size((numcols * (recmaxr + recmaxl)) + ((numcols * CollumnPadding) - CollumnPadding), numrows * recmaxh);
            }
            return new Size(maxl + maxr, maxh * Math.Ceiling(Children.Count / 2.0));
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count < 1)
                return finalSize;
            double numrows = Math.Ceiling(Children.Count / 2.0);
            double finalHeight = finalSize.Height / numrows;
            double finalWidthl = finalSize.Width / 2.0;
            double finalWidthr = finalWidthl;
            double y = 0;
            double numcols=1;
            if (Orientation == Orientation.Horizontal)
            {
                //must use columnmunmunmunmnunmmnunmununnnmnmunnms
                //must learn to spell above word
                numrows = ((numrows * recmaxh) > finalSize.Height) ? Math.Floor(finalSize.Height / recmaxh) : numrows;
                finalHeight = recmaxh;
                numcols = Math.Ceiling(Math.Ceiling(Children.Count / 2.0) / numrows);
                finalWidthl = recmaxl;
                finalWidthr = recmaxr;
            }

            double x = -(finalWidthl + finalWidthr);

            UIElement child = null;
            for (int i = 0; i < Children.Count; i++)
            {
                child = Children[i];
                if (i % (numrows * 2) == 0)//start new row
                {
                    x += finalWidthl + finalWidthr + CollumnPadding;
                    y = 0;
                }
                if (i % 2 == 0)//left side
                {
                    child.Arrange(new Rect(x, y, finalWidthl, finalHeight));
                }
                else
                {
                    child.Arrange(new Rect(x+finalWidthl, y, finalWidthr, finalHeight));
                    y += finalHeight;
                }
            }
            return new Size(numcols * (finalWidthr + finalWidthl), numrows * finalHeight);
        }
    }
}
