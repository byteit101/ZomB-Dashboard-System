using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System451.Communication.Dashboard.WPF.Design;
using System.Windows;
using System.Windows.Controls;
using System451.Communication.Dashboard.WPF.Controls.Designer;

namespace System451.Communication.Dashboard.WPF.Controls.Designers
{
    public class CameraTargetCollectionDesigner : DesignerBase
    {
        Button b;
        public override FrameworkElement GetProperyField()
        {
            b = new Button();
            b.Content = " ... ";
            b.Click += new RoutedEventHandler(b_Click);
            return b;
        }

        void b_Click(object sender, RoutedEventArgs e)
        {
            var win = new CameraDesignerWindow(Object as CameraView);
            win.ShowDialog();
        }

        public override bool IsExpanded()
        {
            return true;
        }
        public override string GetValue()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in (Object as CameraView).GetControls())
            {
                var ict = item as CameraTarget;
                sb.Append("<ZomB:CameraTarget ControlName=\"");
                sb.Append(ict.ControlName);
                sb.Append("\" Fill=\"");
                sb.Append(ict.Fill);
                sb.Append("\"><ZomB:CameraTarget.Border><Pen Brush=\"");
                sb.Append(ict.Border.Brush);
                sb.Append("\" Thickness=\"");
                sb.Append(ict.Border.Thickness);
                sb.Append("\" /></ZomB:CameraTarget.Border></ZomB:CameraTarget>");
            }
            return sb.ToString();
        }
        public override bool IsDefaultValue()
        {
            return (Object as CameraView).GetControls().Count == 0;
        }
    }
}
