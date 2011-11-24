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
using System451.Communication.Dashboard.Utils;
using System.Net;
using System.Windows.Controls;
using System451.Communication.Dashboard.Net;

namespace System451.Communication.Dashboard.WPF.Controls
{
    /// <summary>
    /// Interaction logic for AnalogMeter.xaml
    /// </summary>
    [Design.ZomBControl("Network Forwarder",
        Description = "This will forward stuff to a different computer",
        IconName = "NetForwardIcon")]
    [Design.ZomBDesignableProperty("Background")]
    [Design.ZomBDesignableProperty("Width", Dynamic = true, Category = "Layout")]
    [Design.ZomBDesignableProperty("Height", Dynamic = true, Category = "Layout")]
    [Design.ZomBDesignableProperty("RenderTransform", DisplayName = "Transform")]
    [Design.ZomBDesignableProperty("RenderTransformOrigin", DisplayName = "Transform Origin", Description = "The location the transform modifies about. In the range 0-1.")]
    public class NetForward : Control
    {
        TCPProxy tcp = null;
        UDPProxy udp = null;
        static NetForward()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NetForward),
            new FrameworkPropertyMetadata(typeof(NetForward)));
        }

        public NetForward()
        {
            this.Background = Brushes.Wheat;
            this.Width = 20;
            this.Height = 20;
        }

        [Design.ZomBDesignable(), Category("Network")]
        public NetForwardType Protocol
        {
            get { return (NetForwardType)GetValue(ProtocolProperty); }
            set { SetValue(ProtocolProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Protocol.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProtocolProperty =
            DependencyProperty.Register("Protocol", typeof(NetForwardType), typeof(NetForward), new UIPropertyMetadata(NetForwardType.TCP, update));


        [Design.ZomBDesignable(), Category("Network")]
        public int Port
        {
            get { return (int)GetValue(PortProperty); }
            set { SetValue(PortProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Port.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PortProperty =
            DependencyProperty.Register("Port", typeof(int), typeof(NetForward), new UIPropertyMetadata(update));


        [Design.ZomBDesignable(), Category("Network")]
        public string To
        {
            get { return (string)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        // Using a DependencyProperty as the backing store for To.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(string), typeof(NetForward), new UIPropertyMetadata(update), ipvalidate);

        private static bool ipvalidate(object o)
        {
            if (o == null)
                return true;
            IPAddress iad;
            return IPAddress.TryParse(o.ToString(), out iad);
        }

        private static void update(object o, DependencyPropertyChangedEventArgs e)
        {
            if (ZDesigner.IsRunMode)
            {
                (o as NetForward).Hookup();
            }
        }

        private void Hookup()
        {
            if (To != null && Port != 0)
            {
                if (tcp != null)
                {
                    tcp.Stop();
                    tcp = null;
                }
                if (udp != null)
                {
                    udp.Stop();
                    udp = null;
                }
                if (Protocol == NetForwardType.TCP)
                {
                    tcp = new TCPProxy(new IPEndPoint(IPAddress.Any, Port), new IPEndPoint(IPAddress.Parse(To), Port));
                    tcp.Start();
                }
                else
                {
                    udp = new UDPProxy(new IPEndPoint(IPAddress.Any, Port), new IPEndPoint(IPAddress.Parse(To), Port));
                    udp.Start();
                }
            }
        }

    }
    public enum NetForwardType
    {
        TCP, UDP
    }
}
