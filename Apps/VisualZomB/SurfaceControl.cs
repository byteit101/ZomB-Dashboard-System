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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System451.Communication.Dashboard.WPF.Design;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace System451.Communication.Dashboard.ViZ
{
    [TemplatePart(Name = "PART_Resize")]
    [TemplatePart(Name = "PART_ctxMenu")]
    public class SurfaceControl : Control
    {
        Control sizer, tlsize, lsize, tsize, trsize, blsize, rsize, bsize;
        ContextMenu mnu;
        SortedDictionary<string, List<PropertyElement>> proplist;
        Collection<SnapLine> snaps = new Collection<SnapLine>();

        static SurfaceControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SurfaceControl),
                new FrameworkPropertyMetadata(typeof(SurfaceControl)));
        }

        public SurfaceControl()
        {
            this.SnapsToDevicePixels = true;
            this.Focusable = true;
            FocusManager.SetIsFocusScope(this, false);
            this.Focusable = true;
            this.SizeChanged += new SizeChangedEventHandler(SurfaceControl_SizeChanged);
        }

        void SurfaceControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Control != null && !(Double.IsNaN(Control.Width) || Double.IsNaN(Control.Height)))
            {
                Control.Width = e.NewSize.Width;
                Control.Height = e.NewSize.Height;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            sizer = base.GetTemplateChild("PART_Resize") as Control;
            tlsize = base.GetTemplateChild("PART_Resize_tl") as Control;
            lsize = base.GetTemplateChild("PART_Resize_l") as Control;
            tsize = base.GetTemplateChild("PART_Resize_t") as Control;
            trsize = base.GetTemplateChild("PART_Resize_tr") as Control;
            blsize = base.GetTemplateChild("PART_Resize_bl") as Control;
            rsize = base.GetTemplateChild("PART_Resize_r") as Control;
            bsize = base.GetTemplateChild("PART_Resize_b") as Control;
            mnu = base.GetTemplateChild("PART_ctxMenu") as ContextMenu;
            if (Control != null)
            {
                if (Double.IsNaN(Control.Width) || Double.IsNaN(Control.Height))
                {
                    tlsize.Visibility = lsize.Visibility = tsize.Visibility = trsize.Visibility
                        = blsize.Visibility = rsize.Visibility = bsize.Visibility = sizer.Visibility = Visibility.Collapsed;
                }
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            foreach (var snap in snaps)
            {
                drawingContext.DrawLine(new Pen(new SolidColorBrush(snap.color), 1.0), new Point(snap.x1, snap.y1), new Point(snap.x2, snap.y2));
            }
            base.OnRender(drawingContext);
        }
        
        public void SetSnap(SnapLine snap)
        {
            snaps.Add(snap);
        }

        public void DrawSnaps()
        {
            Snapers = null;
            Snapers = snaps;
        }

        public void ClearSnap()
        {
            snaps.Clear();
        }

        private Collection<SnapLine> Snapers
        {
            get { return (Collection<SnapLine>)GetValue(SnapersProperty); }
            set { SetValue(SnapersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Snapers.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty SnapersProperty =
            DependencyProperty.Register("Snapers", typeof(Collection<SnapLine>), typeof(SurfaceControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// The control we are designing
        /// </summary>
        public Control Control
        {
            get { return (Control)GetValue(ControlProperty); }
            set { SetValue(ControlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Control.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControlProperty =
            DependencyProperty.Register("Control", typeof(Control), typeof(SurfaceControl), new UIPropertyMetadata(null, ControlChanged));

        static void ControlChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as SurfaceControl).SetSize();
        }

        private void SetSize()
        {
            Control.SizeChanged += new SizeChangedEventHandler(Control_SizeChanged);
        }

        void Control_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize != new Size(ActualWidth, ActualHeight))
            {
                this.Width = e.NewSize.Width;
                this.Height = e.NewSize.Height;
            }
        }

        private void loadCtx(Panel prophld, object ctrl)
        {
            if (proplist == null)
                LoadPropList(ctrl);
            if (prophld == null)
                return;
            prophld.Children.Clear();
            bool toped=false, lefted=false;
            foreach (var category in proplist)
            {
                var lb = new Label();
                lb.Content = category.Key;
                lb.Style = (Style)lb.FindResource("PropCatStyle");
                prophld.Children.Add(lb);
                category.Value.Sort();
                foreach (var itm in category.Value)
                {
                    //Sneak in the Canvas.Left and Canvas.Top into the property editors
                    if (category.Key == "Layout")
                    {
                        if (!lefted && string.Compare("Left", itm.Name) < 0)
                            prophld.Children.Add(GetTLBox(false));
                        if (!toped && string.Compare("Top", itm.Name) < 0)
                            prophld.Children.Add(GetTLBox(true));
                    }
                    prophld.Children.Add(itm.GetEntry());
                }
            }
        }

        private FrameworkElement GetTLBox(bool top)
        {
            var Name = "Left: ";
            var rprop = Canvas.LeftProperty;
            if (top)
            {
                Name = "Top: ";
                rprop = Canvas.TopProperty;
            }
            var itm = new StackPanel();
            itm.Orientation = Orientation.Horizontal;
            itm.Children.Add(new TextBlock());
            (itm.Children[0] as TextBlock).Text = Name;
            itm.Children.Add(new TextBox());
            (itm.Children[1] as TextBox).Width = 50.0;
            Binding bind = new Binding();
            bind.Mode = BindingMode.TwoWay;
            bind.Source = this;
            bind.Path = new PropertyPath(rprop);
            bind.Converter = new StringValueConverter();
            (itm.Children[1] as TextBox).SetBinding(TextBox.TextProperty, bind);
            itm.Margin = new Thickness(1);
            return itm;
        }

        private void LoadPropList(object ctrl)
        {
            proplist = new SortedDictionary<string, List<PropertyElement>>(new CategoryComparer());
            foreach (var prop in ctrl.GetType().GetProperties())
            {
                foreach (var at in prop.GetCustomAttributes(typeof(ZomBDesignableAttribute), true))
                {
                    PropertyElement pe = new PropertyElement(ctrl, prop);

                    if (!proplist.ContainsKey(pe.Category))
                        proplist.Add(pe.Category, new List<PropertyElement>());

                    proplist[pe.Category].Add(pe);
                }                
            }
            foreach (var at in ctrl.GetType().GetCustomAttributes(typeof(ZomBDesignablePropertyAttribute), true))
            {
                PropertyElement pe = new PropertyElement(ctrl, (at as ZomBDesignablePropertyAttribute).PropertyName);

                if (!proplist.ContainsKey(pe.Category))
                    proplist.Add(pe.Category, new List<PropertyElement>());

                proplist[pe.Category].Add(pe);
            }
        }

        public Dictionary<string, string> GetProps()
        {
            var ret = new Dictionary<string, string>();
            foreach (var item in proplist)
            {
                foreach (var p in item.Value)
                {
                    ret.Add(p.Property.Name, p.Value.ToString());
                }
            }
            return ret;
        }

        public Designer.CurrentDragMove GetResizeDirection(FrameworkElement elm)
        {
            if (elm == sizer || elm.TemplatedParent == sizer)
            {
                return Designer.CurrentDragMove.Width | Designer.CurrentDragMove.Height;
            }
            if (elm == tlsize || elm.TemplatedParent == tlsize)
            {
                return Designer.CurrentDragMove.X | Designer.CurrentDragMove.Y;
            }
            if (elm == lsize || elm.TemplatedParent == lsize)
            {
                return Designer.CurrentDragMove.X;
            }
            if (elm == tsize || elm.TemplatedParent == tsize)
            {
                return Designer.CurrentDragMove.Y;
            }
            if (elm == trsize || elm.TemplatedParent == trsize)
            {
                return Designer.CurrentDragMove.Y | Designer.CurrentDragMove.Width;
            }
            if (elm == blsize || elm.TemplatedParent == blsize)
            {
                return Designer.CurrentDragMove.X | Designer.CurrentDragMove.Height;
            }
            if (elm == bsize || elm.TemplatedParent == bsize)
            {
                return Designer.CurrentDragMove.Height;
            }
            if (elm == rsize || elm.TemplatedParent == rsize)
            {
                return Designer.CurrentDragMove.Width;
            }
            return Designer.CurrentDragMove.None;
        }

        public void PopulateProperties(Panel propHolder)
        {
            loadCtx(propHolder, Control);
        }
    }
}
