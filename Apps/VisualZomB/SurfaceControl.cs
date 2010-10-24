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

namespace System451.Communication.Dashboard.ViZ
{
    [TemplatePart(Name = "PART_Resize")]
    [TemplatePart(Name = "PART_ctxMenu")]
    [TemplatePart(Name = "PART_props")]
    public class SurfaceControl : Control
    {
        Control sizer;
        ContextMenu mnu;
        StackPanel prophld;
        SortedDictionary<string, List<PropertyElement>> proplist;

        static SurfaceControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SurfaceControl),
                new FrameworkPropertyMetadata(typeof(SurfaceControl)));
        }

        public SurfaceControl()
        {
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
            prophld = base.GetTemplateChild("PART_props") as StackPanel;
            mnu = base.GetTemplateChild("PART_ctxMenu") as ContextMenu;
            if (Control != null)
            {
                if (Double.IsNaN(Control.Width) || Double.IsNaN(Control.Height))
                {
                    sizer.Visibility = Visibility.Collapsed;
                }
                loadCtx(Control);
            }
        }

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
            (o as SurfaceControl).loadCtx(e.NewValue);
        }

        private void loadCtx(object ctrl)
        {
            if (proplist == null)
                LoadPropList(ctrl);
            if (prophld == null)
                return;
            prophld.Children.Clear();
            foreach (var category in proplist)
            {
                var lb = new Label();
                lb.Content = category.Key;
                lb.Style = (Style)lb.FindResource("PropCatStyle");
                prophld.Children.Add(lb);
                category.Value.Sort();
                foreach (var itm in category.Value)
                {
                    prophld.Children.Add(itm.GetEntry());
                }
            }
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
                    ret.Add(p.Name, p.Value.ToString());
                }
            }
            return ret;
        }
    }
}
