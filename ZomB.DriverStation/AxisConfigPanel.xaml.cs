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
using System.Windows;
using System.Windows.Controls;

namespace System451.Communication.Dashboard.Net.DriverStation
{
    public partial class AxisConfigPanel : GroupBox
    {
        public AxisConfigPanel()
        {
            InitializeComponent();

            var nams = ZDesigner.GetChildren();
            foreach (var item in nams)
            {
                xsourcebox.Items.Add("Virtual " + (item as FrameworkElement).Name);
            }
            xsourcebox.SelectedIndex = 0;
        }

        public void SetConfig(string p)
        {
            try
            {
                if (p.Contains("Virtual"))
                {
                    string[] splt = p.Split('@');
                    splt[0] = splt[0].Replace("Virtual", "Virtual ");
                    xsourcebox.SelectedIndex = SelectBox(xsourcebox, splt[0]);
                    xdetailbox.SelectedIndex = SelectBox(xdetailbox, splt[1]);
                }
                else//Hardware
                {
                    p = p.Substring(8);
                    xsourcebox.SelectedIndex = int.Parse(p[0].ToString()) - 1;
                    xdetailbox.SelectedIndex = SelectBox(xdetailbox, p.Substring(1));
                }
            }
            catch { }
        }

        private int SelectBox(ComboBox bx, string splt)
        {
            for (int i = 0; i < bx.Items.Count; i++)
            {
                if (bx.Items[i].ToString() == splt)
                {
                    return i;
                }
            }
            return 0;
        }

        private void Source_changed(object sender, SelectionChangedEventArgs e)
        {
            xdetailbox.Items.Clear();
            //0-3 are hw
            if (xsourcebox.SelectedIndex < 4)
            {
                xdetailbox.Items.Add("LeftX");
                xdetailbox.Items.Add("LeftY");
                xdetailbox.Items.Add("RightX");
                xdetailbox.Items.Add("RightY");
                xdetailbox.Items.Add("LeftTrigger");
                xdetailbox.Items.Add("RightTrigger");
            }
            else
            {
                var nams = ZDesigner.GetChildByName(xsourcebox.SelectedItem.ToString().Substring(8)).GetType().GetProperties();
                foreach (var item in nams)
                {
                    xdetailbox.Items.Add(item.Name);
                }
            }
            xdetailbox.SelectedIndex = 0;
        }

        public string GetConfigString()
        {
            if (xsourcebox.SelectedItem.ToString().Contains("Virtual"))
                return xsourcebox.SelectedItem.ToString().Replace(" ", "") + "@" + xdetailbox.SelectedItem.ToString();
            else
                return xsourcebox.SelectedItem.ToString().Replace(" ", "") + xdetailbox.SelectedItem.ToString();
        }
    }
}
