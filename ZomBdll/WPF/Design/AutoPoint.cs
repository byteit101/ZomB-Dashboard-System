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
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace System451.Communication.Dashboard.WPF.Controls
{
    [TemplatePart(Name = "PART_Pop", Type = typeof(Popup))]
    public class AutoPoint : Control
    {
        Popup pup;
        static AutoPoint()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoPoint),
            new FrameworkPropertyMetadata(typeof(AutoPoint)));
        }

        public AutoPoint()
        {
            this.Width = 16;
            this.Height = 17;
            this.ClipToBounds = false;
            this.MouseLeftButtonUp += delegate
            {
                if (pup != null)
                    pup.IsOpen = true;
            };
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            pup = base.GetTemplateChild("PART_Pop") as Popup;
        }

        new public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...
        new public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(AutoPoint), new UIPropertyMetadata(""));

        public object Toolbox
        {
            get { return (object)GetValue(ToolboxProperty); }
            set { SetValue(ToolboxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Toolbox.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToolboxProperty =
            DependencyProperty.Register("Toolbox", typeof(object), typeof(AutoPoint), new UIPropertyMetadata());
    }
}
