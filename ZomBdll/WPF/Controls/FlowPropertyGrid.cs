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
            recmaxl = maxl;
            recmaxr = maxr;
            recmaxh = maxh;
            if (Orientation == Orientation.Horizontal)
            {
                double numrows = Math.Ceiling(Children.Count / 2.0);
                numrows = ((numrows * recmaxh) > availableSize.Height) ? Math.Floor(availableSize.Height / recmaxh) : numrows;
                double numcols = Math.Ceiling(Math.Ceiling(Children.Count / 2.0) / numrows);
                return new Size(numcols * (recmaxr + recmaxl), numrows * recmaxh);
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
                //TODO: use these better
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
                    x += finalWidthl + finalWidthr;
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
