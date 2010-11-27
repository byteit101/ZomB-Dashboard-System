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
            var s = new StackPanel();
            s.FlowDirection = FlowDirection.RightToLeft;
            s.Orientation = Orientation.Horizontal;
            s.Children.Add(b);
            return s;
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
