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

namespace System451.Communication.Dashboard.ViZ
{
    [TemplatePart(Name = "PART_Resize")]
    public class SurfaceControl : Control
    {
        Control sizer;

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
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            sizer = base.GetTemplateChild("PART_Resize") as Control;
        }

        /// <summary>
        /// The control we are designing
        /// </summary>
        public UIElement Control
        {
            get { return (UIElement)GetValue(ControlProperty); }
            set { SetValue(ControlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Control.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControlProperty =
            DependencyProperty.Register("Control", typeof(UIElement), typeof(SurfaceControl), new UIPropertyMetadata(null));


    }
}
