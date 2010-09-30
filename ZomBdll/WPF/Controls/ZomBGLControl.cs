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
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace System451.Communication.Dashboard.WPF.Controls
{
    /// <summary>
    /// WPF Base ZomB Control
    /// </summary>
    public class ZomBGLControl : Control, IZomBControl
    {
        DashboardDataHub localDDH;

        public string StringValue
        {
            get { return (string)GetValue(StringValueProperty); }
            set { SetValue(StringValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StringValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StringValueProperty =
            DependencyProperty.Register("StringValue", typeof(string), typeof(ZomBGLControl), new UIPropertyMetadata(""));


        public int IntValue
        {
            get { return (int)GetValue(IntValueProperty); }
            set { SetValue(IntValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IntValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IntValueProperty =
            DependencyProperty.Register("IntValue", typeof(int), typeof(ZomBGLControl), new UIPropertyMetadata(0));



        public double DoubleValue
        {
            get { return (double)GetValue(DoubleValueProperty); }
            set { SetValue(DoubleValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DoubleValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DoubleValueProperty =
            DependencyProperty.Register("DoubleValue", typeof(double), typeof(ZomBGLControl), new UIPropertyMetadata(0.0));


        public bool BoolValue
        {
            get { return (bool)GetValue(BoolValueProperty); }
            set { SetValue(BoolValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BoolValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BoolValueProperty =
            DependencyProperty.Register("BoolValue", typeof(bool), typeof(ZomBGLControl), new UIPropertyMetadata(false));

        static ZomBGLControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ZomBGLControl),
                new FrameworkPropertyMetadata(typeof(ZomBGLControl)));
        }

        /// <summary>
        /// Creates the ZomBGLControl
        /// </summary>
        public ZomBGLControl()
        {
            BoolValue = false;
            DoubleValue = 0;
            IntValue = 0;
            StringValue = "";
            this.Foreground = Brushes.Black;
            ControlAdded += new ControlAddedDelegate(ZomBGLControl_ControlAdded);
        }

        #region IZomBControl Members

        /// <summary>
        /// Gets the IsMultiWatch field. Default false.
        /// </summary>
        [Browsable(false)]
        virtual public bool IsMultiWatch
        {
            get { return false; }
        }

        /// <summary>
        /// The control name
        /// </summary>
        [Browsable(false), Category("ZomB"), Description("What this control will get the value of from the packet Data")]
        virtual public string ControlName
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        /// <summary>
        /// Updates the control
        /// </summary>
        virtual public void UpdateControl(string value)
        {
            StringValue = value;
            int o;
            int.TryParse(value, out o);
            IntValue = o;
            double d;
            double.TryParse(value, out d);
            DoubleValue = d;
            BoolValue = (IntValue != 0 || value.ToLower() == "true" || value.ToLower() == "yes");
        }

        /// <summary>
        /// When this control is added to a DashboardDataHub
        /// </summary>
        public event ControlAddedDelegate ControlAdded;

        void IZomBControl.ControlAdded(object sender, ZomBControlAddedEventArgs e)
        {
            if (ControlAdded != null)
                ControlAdded(sender, e);
        }

        #endregion

        void ZomBGLControl_ControlAdded(object sender, ZomBControlAddedEventArgs e)
        {
            localDDH = e.Controller.GetDashboardDataHub();
        }

        /// <summary>
        /// Gets the current DashboardDataHub
        /// </summary>
        [Browsable(false)]
        public DashboardDataHub LocalDashboardDataHub
        {
            get
            {
                return localDDH;
            }
        }
    }

    public class ThicknessDoublerZomB : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((value is Thickness))
            {
                return (double)(((Thickness)value).Left * (double.Parse(parameter.ToString())));
            }
            if ((value is double || value is int))
            {
                return ((double)value) * (double.Parse(parameter.ToString()));
            }
            
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((value is double || value is int))
            {
                return ((double)value) / (double.Parse(parameter.ToString()));
            }
            if ((value is Thickness))
            {
                return new Thickness(((Thickness)value).Left / (double.Parse(parameter.ToString())));
            }
            return value;
        }

        #endregion
    }
}
