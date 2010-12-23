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
using System451.Communication.Dashboard.Libs.Xbox360Controller;
using System.Windows;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Timers;
using System451.Communication.Dashboard.Utils;
using System.Windows.Data;

namespace System451.Communication.Dashboard.Net.DriverStation
{
    [WPF.Design.Designer(typeof(JoystickDesigner))]
    public class Joystick : DependencyObject
    {
        static GamepadState[] Pads = new[] { new GamepadState(0), new GamepadState(1), new GamepadState(2), new GamepadState(3) };
        Collection<HWconfig> hw = new Collection<HWconfig>();
        Collection<SWconfig> sw = new Collection<SWconfig>();
        static Timer stmr = new Timer(20);
        Timer tmr = new Timer(20);
        Func<string, object> findfunc;

        public class HWconfig
        {
            public DependencyPropertyKey dpKey { get; set; }
            public GamepadState Pad { get; set; }
            public PropertyInfo Property { get; set; }
            public Joystick me { get; set; }
            public void Update()
            {
                me.SetValue(dpKey, Property.GetValue(Pad, null));
            }
        }

        public class SWconfig
        {
            public string SourceProperty { get; set; }
            private PropertyInfo SoProperty { get; set; }
            public DependencyPropertyKey dpKey { get; set; }
            public string src { get; set; }
            public Joystick me { get; set; }
            public Func<string, object> seter { get; set; }
            private object obj { get; set; }
            public void Update()
            {
                if (seter != null)
                {
                    obj = seter(src);
                    SoProperty = obj.GetType().GetProperty(SourceProperty);
                    seter = null;
                }
                if (seter == null && obj != null && SoProperty != null)
                    me.SetValue(dpKey, SoProperty.GetValue(obj, null));
            }
        }

        static Joystick()
        {
            var cdisp = System.Windows.Threading.Dispatcher.CurrentDispatcher;
            //stmr.AutoReset = true;
            //stmr.Elapsed += delegate { cdisp.Invoke(new VoidFunction(updateJoys), null); };
            //if (ZDesigner.IsRunMode)
            //stmr.Start();
        }

        public Joystick()
        {
            var cdisp = System.Windows.Threading.Dispatcher.CurrentDispatcher;
            //tmr.AutoReset = false;
            //tmr.Elapsed += delegate { cdisp.Invoke(new VoidFunction(updateJoy), null); };
            //if (ZDesigner.IsRunMode)
            //tmr.Start();
        }

        internal void SetFindName(Func<string, object> find)
        {
            try
            {
                findfunc = find;
                foreach (var config in sw)
                {
                    config.seter = find;
                }
            }
            catch { }
        }

        static void updateJoys()
        {
            try
            {
                foreach (var pad in Pads)
                {
                    pad.Update();
                }
            }
            catch { }
        }

        void updateJoy()
        {
            try
            {
                foreach (var item in hw)
                {
                    item.Update();
                }
                foreach (var config in sw)
                {
                    config.Update();
                }
            }
            catch { }
        }

        public string XSource
        {
            get { return (string)GetValue(XSourceProperty); }
            set { SetValue(XSourceProperty, value); }
        }

        public static readonly DependencyProperty XSourceProperty =
            DependencyProperty.Register("XSource", typeof(string), typeof(Joystick), new FrameworkPropertyMetadata(sourceChanged));

        public double X
        {
            get { return (double)GetValue(XProperty.DependencyProperty); }
            private set { SetValue(XProperty, value); }
        }

        private static readonly DependencyPropertyKey XProperty = DependencyProperty.RegisterReadOnly("X", typeof(double), typeof(Joystick), new UIPropertyMetadata(0.0));

        public string YSource
        {
            get { return (string)GetValue(YSourceProperty); }
            set { SetValue(YSourceProperty, value); }
        }

        public static readonly DependencyProperty YSourceProperty =
            DependencyProperty.Register("YSource", typeof(string), typeof(Joystick), new FrameworkPropertyMetadata(sourceChanged));

        public double Y
        {
            get { return (double)GetValue(YProperty.DependencyProperty); }
            private set { SetValue(YProperty, value); }
        }

        private static readonly DependencyPropertyKey YProperty = DependencyProperty.RegisterReadOnly("Y", typeof(double), typeof(Joystick), new UIPropertyMetadata(0.0));

        private static void sourceChanged(object o, DependencyPropertyChangedEventArgs e)
        {
            //TODO: change
            if (e.Property == XSourceProperty)
            {
                (o as Joystick).setup(XProperty, e.NewValue.ToString(), "LeftX");
            }
            else if (e.Property == YSourceProperty)
            {
                (o as Joystick).setup(YProperty, e.NewValue.ToString(), "LeftY");
            }
        }

        private void setup(DependencyPropertyKey propkey, string srcString, string Axis)
        {
            try
            {
                switch (srcString.ToLower()[0])
                {
                    case 'h'://ardware
                        {
                            var port = int.Parse(srcString[8].ToString());
                            var axis = Axis;
                            if (srcString.Length > 9)
                            {
                                axis = srcString.Substring(9);
                            }
                            HardwareConfig(propkey, port, axis);
                        }
                        break;
                    case 'v'://irtual
                        {
                            var str = srcString.Substring(7).Split('@');
                            var name = str[0];
                            SoftwareConfig(propkey, str[0], str[1]);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch { }
        }

        private void SoftwareConfig(DependencyPropertyKey propkey, string name, string property)
        {
            if (ZDesigner.IsRunMode)
            {
                sw.Add(new SWconfig { me = this, dpKey = propkey, src = name, SourceProperty = property, seter=findfunc });
            }
        }

        private void HardwareConfig(DependencyPropertyKey propkey, int port, string axis)
        {
            hw.Add(new HWconfig { me = this, dpKey = propkey, Pad = Pads[port - 1], Property = Pads[port - 1].GetType().GetProperty(axis) });
        }

        internal void SaveDataTo(byte[] stream, int offset)
        {
            try
            {
                updateJoys();
                updateJoy();
                unchecked
                {
                    stream[offset] = (byte)((X * 127.5));
                    stream[offset + 1] = (byte)((Y * 127.5));
                }
            }
            catch { }
        }
    }
}
