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
using System451.Communication.Dashboard.WPF.Controls.Designers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace System451.Communication.Dashboard.WPF.Controls.Designer
{
    public class ImageSourceDesigner : DesignerBase
    {
        TextBox t;
        public override FrameworkElement GetProperyField()
        {
            var pan = new DockPanel();
            t = (new TextBox());
            try
            {
                t.Text = GetVaueAsType<ImageSource>().ToString();
            }
            catch { }//Null value
            t.TextChanged += delegate(object sender, TextChangedEventArgs e) { SetString(t.Text); };
            var btn = new Button();
            btn.Content = "...";
            btn.Click += delegate
            {
                var fpd = new System.Windows.Forms.OpenFileDialog();
                fpd.Filter = "Images|*.jpg;*.jpeg;*.png;*.gif;*.tif;*.bmp";
                if (fpd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    t.Text = fpd.FileName;
                }
            };
            DockPanel.SetDock(btn, Dock.Right);
            pan.Children.Add(btn);
            pan.Children.Add(t);
            return pan;
        }
        public override bool IsDefaultValue()
        {
            return t.Text == "";
        }

        private void SetString(string p)
        {
            Set(new ImageSourceConverter().ConvertFrom(null, System.Globalization.CultureInfo.CurrentCulture, p));
        }
    }
}
