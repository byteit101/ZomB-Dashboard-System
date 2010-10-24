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
using System.Windows.Media;
using System.Windows.Shapes;

namespace System451.Communication.Dashboard.WPF.Controls
{
    /// <summary>
    /// Interaction logic for AnalogMeter.xaml
    /// </summary>
    [TemplatePart(Name = "PART_Rect", Type = typeof(Rectangle)), Design.ZomBControl("Alert Control", Description = "This is a square version of the OnOffControl, and shows a true/false value", IconName="AlertControlIcon")]
    [Design.ZomBDesignableProperty("Foreground")]
    [Design.ZomBDesignableProperty("Background")]
    public class AlertControl : ZomBGLControl
    {
        Rectangle PART_Rect;
        static AlertControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AlertControl),
            new FrameworkPropertyMetadata(typeof(AlertControl)));
        }

        public AlertControl()
        {
            this.Background = Brushes.Red;
            this.Foreground = Brushes.Green;
            this.SnapsToDevicePixels = true;
            this.Width = 50;
            this.Height = 50;
            try
            {
                BoolValueProperty.OverrideMetadata(typeof(AlertControl), new FrameworkPropertyMetadata(false, boolchange));
            }
            catch { }
        }

        private void boolchange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (PART_Rect != null)
            {
                PART_Rect.Fill = BoolValue ? Foreground : Background;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_Rect = base.GetTemplateChild("PART_Rect") as Rectangle;
            boolchange(null, new DependencyPropertyChangedEventArgs());
        }
    }
}
