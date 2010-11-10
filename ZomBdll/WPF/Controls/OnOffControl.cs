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
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace System451.Communication.Dashboard.WPF.Controls
{
    /// <summary>
    /// Interaction logic for AnalogMeter.xaml
    /// </summary>
    [TemplatePart(Name = "PART_Rect", Type = typeof(Ellipse)), Design.ZomBControl("On/Off Control", Description = "This is a true/false or yes/no control", IconName = "OnOffControlIcon")]
    [Design.ZomBDesignableProperty("Foreground")]
    [Design.ZomBDesignableProperty("Background")]
    public class OnOffControl : ZomBGLControl
    {
        Ellipse PART_Rect;
        static OnOffControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OnOffControl),
            new FrameworkPropertyMetadata(typeof(OnOffControl)));
        }

        public OnOffControl()
        {
            this.Background = Brushes.Red;
            this.Foreground = Brushes.Green;
            this.SnapsToDevicePixels = true;
            this.Width = 50;
            this.Height = 50;
            try
            {
                BoolValueProperty.OverrideMetadata(typeof(OnOffControl), new FrameworkPropertyMetadata(false, boolchange));
            }
            catch { }
        }

        private void boolchange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (PART_Rect != null)
            {
                Binding b = new Binding(BoolValue ? "Foreground" : "Background");
                b.Source = this;
                PART_Rect.SetBinding(Ellipse.FillProperty, b);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_Rect = base.GetTemplateChild("PART_Rect") as Ellipse;
            boolchange(null, new DependencyPropertyChangedEventArgs());
        }
    }
}
