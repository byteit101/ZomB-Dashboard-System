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
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System451.Communication.Dashboard.WPF.Design;

namespace System451.Communication.Dashboard.WPF.Controls
{
    /// <summary>
    /// Interaction logic for AnalogMeter.xaml
    /// </summary>
    [TemplatePart(Name = "PART_Rect", Type = typeof(Rectangle)), Design.ZomBControl("Alert Control", Description = "This is a square version of the OnOffControl, and shows a true/false value", IconName = "AlertControlIcon")]
    [Design.ZomBDesignableProperty("Foreground")]
    [Design.ZomBDesignableProperty("Background")]
    [Design.ZomBDesignableProperty("BoolValue", DisplayName = "Value")]
    public class AlertControl : ZomBGLControl, IZomBDataControl, IZTrigger
    {
        Rectangle PART_Rect;
        static AlertControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AlertControl),
            new FrameworkPropertyMetadata(typeof(AlertControl)));
            BoolValueProperty.OverrideMetadata(typeof(AlertControl), new FrameworkPropertyMetadata(false, boolchange));
        }

        public AlertControl()
        {
            this.Background = Brushes.Red;
            this.Foreground = Brushes.Green;
            this.SnapsToDevicePixels = true;
            this.Width = 50;
            this.Height = 50;
        }

        private static void boolchange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = sender as AlertControl;
            if (s.PART_Rect != null)
            {
                Binding b = new Binding(s.BoolValue ? "Foreground" : "Background");
                b.Source = s;
                s.PART_Rect.SetBinding(Rectangle.FillProperty, b);
            }
            if (s.BoolValue && s.Triggered != null)
                s.Triggered();
        }

        [ZomBDesignable(DisplayName = "Triggers"), Category("Behavior")]
        public string TriggerListeners { get; set; }

        public event Utils.VoidFunction Triggered;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_Rect = base.GetTemplateChild("PART_Rect") as Rectangle;
            boolchange(this, new DependencyPropertyChangedEventArgs());
        }

        #region IZomBDataControl Members

        public event ZomBDataControlUpdatedEventHandler DataUpdated;

        bool dce = false;
        public bool DataControlEnabled
        {
            get
            {
                return dce;
            }
            set
            {
                if (dce != value)
                {
                    dce = value;
                    if (dce)
                    {
                        this.MouseLeftButtonUp += AlertControl_MouseLeftButtonUp;
                    }
                    else
                    {
                        this.MouseLeftButtonUp -= AlertControl_MouseLeftButtonUp;
                    }
                }
            }
        }

        void AlertControl_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BoolValue = !BoolValue;
            if (DataUpdated != null)
                DataUpdated(this, new ZomBDataControlUpdatedEventArgs(ControlName, BoolValue.ToString()));
        }

        #endregion
    }
}
