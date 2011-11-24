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
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace System451.Communication.Dashboard.WPF.Controls
{
    /// <summary>
    /// Interaction logic for AnalogMeter.xaml
    /// </summary>
    [TemplatePart(Name = "PART_Rect")]
    [Design.ZomBControl("Spike Control",
        Description = "This is a tri-state control, forward, reverse, or off",
        IconName = "SpikeControlIcon",
        TypeHints = ZomBDataTypeHint.Number | ZomBDataTypeHint.Boolean)]
    [Design.ZomBDesignableProperty("Foreground")]
    [Design.ZomBDesignableProperty("Background")]
    [Design.ZomBDesignableProperty("BorderBrush")]
    public class SpikeControl : ZomBGLControl, IZomBDataControl
    {
        Rectangle PART_Rect;
        static SpikeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SpikeControl),
            new FrameworkPropertyMetadata(typeof(SpikeControl)));
        }

        public SpikeControl()
        {
            this.Background = Brushes.Red;
            this.Foreground = Brushes.Green;
            this.BorderBrush = Brushes.Black;
            this.SnapsToDevicePixels = true;
            this.Width = 50;
            this.Height = 50;
        }

        private void boolchange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (PART_Rect != null)
            {
                Binding b = new Binding(Value == SpikePositions.Forward ? "Foreground" : Value == SpikePositions.Reverse ? "Background" : "BorderBrush");
                b.Source = this;
                PART_Rect.SetBinding(Rectangle.FillProperty, b);
            }
        }

        private static void boolchanges(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((SpikeControl)sender).boolchange(sender, e);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_Rect = base.GetTemplateChild("PART_Rect") as Rectangle;
            boolchange(null, new DependencyPropertyChangedEventArgs());
        }


        public override void UpdateControl(ZomBDataObject value)
        {
            base.UpdateControl(value);
            try
            {
                if (0 != (value.TypeHint & (ZomBDataTypeHint.Number | ZomBDataTypeHint.Boolean)) || DoubleValue.ToString() == StringValue)
                {
                    if (DoubleValue == 0)
                        Value = SpikePositions.Off;
                    else if (DoubleValue > 0)
                        Value = SpikePositions.Forward;
                    else
                        Value = SpikePositions.Reverse;
                }
                else
                    Value = (SpikePositions)Enum.Parse(typeof(SpikePositions), StringValue);
            }
            catch { }
        }

        public SpikePositions Value
        {
            get { return (SpikePositions)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(SpikePositions), typeof(SpikeControl),
            new FrameworkPropertyMetadata(SpikePositions.Off, boolchanges));

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
            switch (Value)
            {
                case SpikePositions.Forward:
                    Value = SpikePositions.Off;
                    break;
                case SpikePositions.Off:
                    Value = SpikePositions.Reverse;
                    break;
                case SpikePositions.Reverse:
                default:
                    Value = SpikePositions.Forward;
                    break;
            }
            if (DataUpdated != null)
                DataUpdated(this, new ZomBDataControlUpdatedEventArgs(ControlName, ((int)Value).ToString()));
        }

        #endregion
    }
}
