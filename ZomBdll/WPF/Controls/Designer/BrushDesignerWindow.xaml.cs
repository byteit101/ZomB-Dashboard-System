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
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using System451.Communication.Dashboard.WPF.Controls.Designer.PrimitiveControls;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Controls;

namespace System451.Communication.Dashboard.WPF.Controls.Designer
{
    /// <summary>
    /// Interaction logic for ColorDesignerWindow.xaml
    /// </summary>
    public partial class BrushDesignerWindow : Window
    {
        Brush b;
        enum Mode
        {
            Solid, LinearGradient
        };
        Mode cmode = Mode.Solid;
        int gradIndex = 0;
        Action<Brush> setv;
        List<StopMarker> contrls = new List<StopMarker>();
        public BrushDesignerWindow(object obj, PropertyInfo prop)
        {
            InitializeComponent();
            b = prop.GetValue(obj, null) as Brush;
            setv = (x) => prop.SetValue(obj, x, null);
            if (b.IsFrozen)
            {
                b = b.Clone();
                setv(b);
            }
            if (b is SolidColorBrush)
            {
                cmode = Mode.Solid;
            }
            else if (b is LinearGradientBrush)
            {
                cmode = Mode.LinearGradient;
            }
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            ColorPicker.ColorChanged += ColorControl_ColorChanged;
            if (cmode == Mode.Solid)
            {
                ColorPicker.Color = (b as SolidColorBrush).Color;
            }
            else if (cmode == Mode.LinearGradient)
            {
                GradientGrid.Background = b;
                foreach (var item in (b as LinearGradientBrush).GradientStops)
                {
                    var sm = GetNewStopMarker(item.Color,item.Offset);
                    contrls.Add(sm);
                    GradientGrid.Children.Add(sm);
                }
                Stop_Click(contrls[0], null);
                tc.SelectedItem = LinGradTab;
            }
        }

        private void ColorControl_ColorChanged(object sender, RoutedEventArgs e)
        {
            if (cmode == Mode.Solid)
            {
                (b as SolidColorBrush).Color = ColorPicker.Color;
            }
            else if (cmode == Mode.LinearGradient)
            {
                (b as LinearGradientBrush).GradientStops[gradIndex].Color = ColorPicker.Color;
                contrls[gradIndex].Forecolor = ColorPicker.Color;
            }
        }

        void Stop_Click(object sender, MouseButtonEventArgs e)
        {
            gradIndex = contrls.IndexOf(sender as StopMarker);
            ColorPicker.Color = (b as LinearGradientBrush).GradientStops[gradIndex].Color;
        }

        void Stop_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            (b as LinearGradientBrush).GradientStops[gradIndex].Offset = (sender as StopMarker).Value;
        }

        private void TabItem_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            if (b is LinearGradientBrush)
            {
                b = new SolidColorBrush((b as LinearGradientBrush).GradientStops[0].Color);
                setv(b);
                cmode = Mode.Solid;
            }
        }

        private void TabItemlg_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            if (b is SolidColorBrush)
            {
                b = new LinearGradientBrush((b as SolidColorBrush).Color, (b as SolidColorBrush).Color, 0);
                setv(b);
                GradientGrid.Background = b;
                GradientGrid.Children.Clear();
                contrls.Clear();
                (b as LinearGradientBrush).GradientStops.Changed += new EventHandler(GradientStops_Changed);
                foreach (var item in (b as LinearGradientBrush).GradientStops)
                {
                    var sm = GetNewStopMarker(item.Color, item.Offset);
                    contrls.Add(sm);
                    GradientGrid.Children.Add(sm);
                }
                cmode = Mode.LinearGradient;
            }
        }

        void GradientStops_Changed(object sender, EventArgs e)
        {
        }

        private void AddStopBtn_Click(object sender, RoutedEventArgs e)
        {
            AddStop(gradIndex);
        }

        private static Comparison<GradientStop> cgscompare = (x, y) => x.Offset.CompareTo(y.Offset);

        private void AddStop(int indx)
        {
            var gs = (b as LinearGradientBrush).GradientStops;
            if (indx < 0)
                indx = 0;
            if (indx >= gs.Count - 1)
                indx = gs.Count - 2;//TODO: need at least 2 points
            List<GradientStop> cs = new List<GradientStop>(gs);
            cs.Sort(cgscompare);
            indx = cs.IndexOf(gs[indx]);
            if (indx < 0)
                indx = 0;
            if (indx >= cs.Count - 1)
                indx = cs.Count - 2;//TODO: need at least 2 points
            double offset = (cs[indx].Offset + cs[indx + 1].Offset) / 2.0;
            Color c = ColorMidpoint(cs[indx].Color, cs[indx + 1].Color);
            var sm = GetNewStopMarker(c, offset);
            contrls.Add(sm);
            GradientGrid.Children.Add(sm);
            gs.Add(new GradientStop(c, offset));
        }

        private Color ColorMidpoint(Color c1, Color c2)
        {
            return Color.FromArgb((byte)((c1.A + c2.A) / 2), (byte)((c1.R + c2.R) / 2), (byte)((c1.G + c2.G) / 2), (byte)((c1.B + c2.B) / 2));
        }

        private StopMarker GetNewStopMarker(Color c, double off)
        {
            var sm = new StopMarker();
            sm.Value = off;
            sm.Forecolor = c;
            Binding bi = new Binding("ActualWidth");
            bi.Source = GradientGrid;
            bi.Converter = Resources["Lefter"] as IValueConverter;
            sm.SetBinding(StopMarker.WidthProperty, bi);
            Canvas.SetLeft(sm, -7);
            Canvas.SetTop(sm, -10);
            sm.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(Stop_Click);
            sm.ValueChanged += new RoutedPropertyChangedEventHandler<double>(Stop_ValueChanged);
            return sm;
        }

        private void DeleteStopBtn_Click(object sender, RoutedEventArgs e)
        {
            DeleteStop(gradIndex);
        }

        private void DeleteStop(int indx)
        {
            if ((b as LinearGradientBrush).GradientStops.Count <= 2)
                return;//sorry, there must be two or more
            (b as LinearGradientBrush).GradientStops.RemoveAt(indx);
            contrls.RemoveAt(indx);
            GradientGrid.Children.RemoveAt(indx);
            --indx;
            if (indx < 0)
                indx = 0;
            if (indx > (b as LinearGradientBrush).GradientStops.Count - 1)
                indx = (b as LinearGradientBrush).GradientStops.Count - 1;
        }

        private void Vertical_Checked(object sender, RoutedEventArgs e)
        {
            (b as LinearGradientBrush).EndPoint = new Point(0, 1);
            Horizontal.IsChecked = false;
        }

        private void Horizontal_Checked(object sender, RoutedEventArgs e)
        {
            (b as LinearGradientBrush).EndPoint = new Point(1, 0);
            Vertical.IsChecked = false;
        }
    }

    public class StopMarkerPositionConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((double)value) + 14;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((double)value) - 14;
        }

        #endregion
    }
}
