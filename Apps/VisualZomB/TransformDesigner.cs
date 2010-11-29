using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System451.Communication.Dashboard.WPF.Controls.Designers;
using System.Windows;
using System.Windows.Controls;
using System451.Communication.Dashboard.WPF.Controls.Designer;
using System.Windows.Media;

namespace System451.Communication.Dashboard.ViZ
{
    public class TransformDesigner : DesignerBase
    {
        Button b;
        public override FrameworkElement GetProperyField()
        {
            b = new Button();
            b.Content = " ... ";
            b.Click += new RoutedEventHandler(b_Click);
            var s = new StackPanel();
            s.FlowDirection = FlowDirection.RightToLeft;
            s.Orientation = Orientation.Horizontal;
            s.Children.Add(b);
            return s;
        }

        void b_Click(object sender, RoutedEventArgs e)
        {
            var win = new TransformDesignerWindow(Object as UIElement);
            win.ShowDialog();
        }

        public override bool IsExpanded()
        {
            return true;
        }
        public override string GetValue()
        {
            StringBuilder sb = new StringBuilder();
            var ict = (Object as UIElement).RenderTransform;
            SerializeTransform(ict, sb);
            return sb.ToString();
        }

        private void SerializeTransform(Transform ict, StringBuilder sb)
        {
            if (ict.GetType() == typeof(RotateTransform))
            {
                sb.Append("<RotateTransform><RotateTransform.Angle>");
                sb.Append((ict as RotateTransform).Angle);
                sb.Append("</RotateTransform.Angle></RotateTransform>");
            }
        }
        public override bool IsDefaultValue()
        {
            return false;//TODO: make return true sometimes
        }
    }
}
