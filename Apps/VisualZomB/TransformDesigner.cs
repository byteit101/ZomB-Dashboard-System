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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System451.Communication.Dashboard.WPF.Controls.Designers;

namespace System451.Communication.Dashboard.ViZ
{
    public class TransformDesigner : DesignerBase
    {
        Button b;
        Label stat;
        public override FrameworkElement GetProperyField()
        {
            b = new Button();
            b.Content = " ... ";
            b.Click += new RoutedEventHandler(b_Click);
            stat = new Label();
            var name = (SurfaceControl.GetSurfaceControlFromControl(Object)).RenderTransform.GetType().Name;
            var it = (SurfaceControl.GetSurfaceControlFromControl(Object)).RenderTransform.ToString();
            stat.Content = (it != "Identity") ? name.Substring(0, name.IndexOf("Transform")) : it;
            var s = new StackPanel();
            s.FlowDirection = FlowDirection.RightToLeft;
            s.Orientation = Orientation.Horizontal;
            s.Children.Add(b);
            s.Children.Add(stat);
            return s;
        }

        void b_Click(object sender, RoutedEventArgs e)
        {
            var win = new TransformDesignerWindow(Object as UIElement);
            win.ShowDialog();
            var name = (SurfaceControl.GetSurfaceControlFromControl(Object)).RenderTransform.GetType().Name;
            var it = (SurfaceControl.GetSurfaceControlFromControl(Object)).RenderTransform.ToString();
            stat.Content = (it != "Identity") ? name.Substring(0, name.IndexOf("Transform")) : it;
        }

        public override bool IsExpanded()
        {
            return true;
        }
        public override string GetValue()
        {
            StringBuilder sb = new StringBuilder();
            var ict = (SurfaceControl.GetSurfaceControlFromControl(Object)).RenderTransform;
            SerializeTransform(ict, sb);
            return sb.ToString();
        }

        private void SerializeTransform(Transform ict, StringBuilder sb)
        {
            if (ict.GetType() == typeof(RotateTransform))
            {
                sb.Append("<RotateTransform><RotateTransform.Angle>");
                var be = BindingOperations.GetBindingExpression(ict, RotateTransform.AngleProperty);
                if (be != null)
                {
                    var bpd = new BoundPropertyDesigner();
                    bpd.Initialize(ict, ict.GetType().GetProperty("Angle"));
                    sb.Append(bpd.GetValue());
                }
                else
                    sb.Append((ict as RotateTransform).Angle);
                sb.Append("</RotateTransform.Angle></RotateTransform>");
            }
            else if (ict is TranslateTransform)
            {
                sb.Append("<TranslateTransform><TranslateTransform.X>");
                var be = BindingOperations.GetBindingExpression(ict, TranslateTransform.XProperty);//TODO: hope Y is the same
                if (be != null)
                {
                    var bpd = new BoundPropertyDesigner();
                    bpd.Initialize(ict, ict.GetType().GetProperty("X"));
                    sb.Append(bpd.GetValue());
                    sb.Append("</TranslateTransform.X><TranslateTransform.Y>");
                }
                else
                    sb.Append("0</TranslateTransform.X><TranslateTransform.Y>");
                be = BindingOperations.GetBindingExpression(ict, TranslateTransform.YProperty);//TODO: hope Y is the same
                if (be != null)
                {
                    var bpd = new BoundPropertyDesigner();
                    bpd.Initialize(ict, ict.GetType().GetProperty("Y"));
                    sb.Append(bpd.GetValue());
                }
                else
                    sb.Append("0");
                sb.Append("</TranslateTransform.Y></TranslateTransform>");
            }
        }
        public override bool IsDefaultValue()
        {
            return (SurfaceControl.GetSurfaceControlFromControl(Object)).RenderTransform == MatrixTransform.Identity;
        }
    }
}
