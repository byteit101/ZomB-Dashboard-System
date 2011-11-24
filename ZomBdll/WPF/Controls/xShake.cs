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
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System451.Communication.Dashboard.Libs.Xbox360Controller;
using System451.Communication.Dashboard.Utils;

namespace System451.Communication.Dashboard.WPF.Controls
{
    /// <summary>
    /// Interaction logic for AnalogMeter.xaml
    /// </summary>
    [Design.ZomBControl("X Shaker",
        Description = "This will send the shake command to a Direct X controller",
        IconName = "xShakeIcon",
        TypeHints = ZomBDataTypeHint.Feedback)]
    [Design.ZomBDesignableProperty("Background")]
    public class xShake : ZomBGLControl
    {
        GamepadState pad;
        static xShake()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(xShake),
            new FrameworkPropertyMetadata(typeof(xShake)));
            StringValueProperty.OverrideMetadata(typeof(xShake), new UIPropertyMetadata("", valChange));
        }

        public xShake()
        {
            this.Background = Brushes.Wheat;
            this.Width = 10;
            this.Height = 10;
            if (ZDesigner.IsRunMode)
                pad = new GamepadState(0);
        }

        [Design.ZomBDesignable(DisplayName = "Controller #"), Description("The Gamepad Number"), Category("ZomB")]
        public int GamepadNumber
        {
            get { return (int)GetValue(GamepadNumberProperty); }
            set { SetValue(GamepadNumberProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GamepadNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GamepadNumberProperty =
            DependencyProperty.Register("GamepadNumber", typeof(int), typeof(xShake), new UIPropertyMetadata(0, gameChange, coerece));

        static void gameChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (ZDesigner.IsRunMode)
                (sender as xShake).pad = new GamepadState((int)e.NewValue);
        }

        static object coerece(DependencyObject sender, object e)
        {
            return Math.Max(0, Math.Min(3, (int)e));
        }

        private static void valChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = sender as xShake;
            if (s.pad != null)
            {
                float l, r;
                if ((e.NewValue as string).Contains(";"))
                {
                    try
                    {
                        l = float.Parse((e.NewValue as string).Substring(0, (e.NewValue as string).IndexOf(';')));
                        r = float.Parse((e.NewValue as string).Substring(1 + (e.NewValue as string).IndexOf(';')));
                    }
                    catch
                    {
                        l = r = 0;
                    }
                }
                else
                {
                    try
                    {
                        l = r = float.Parse((e.NewValue as string));
                    }
                    catch
                    {
                        l = r = 0;
                    }
                }
                s.pad.Vibrate(l, r);
            }
        }
    }
}
