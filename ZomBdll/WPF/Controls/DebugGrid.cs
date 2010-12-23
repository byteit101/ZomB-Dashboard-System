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
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace System451.Communication.Dashboard.WPF.Controls
{
    [Design.ZomBControl("Debug Grid", Description = "This displays variable values debugged with var()", IconName = "DebugGridIcon")]
    [Design.ZomBDesignableProperty("Width", Dynamic = true, Category = "Layout")]
    [Design.ZomBDesignableProperty("Height", Dynamic = true, Category = "Layout")]
    [Design.ZomBDesignableProperty("Background", Category = "Appearance")]
    public class DebugGrid : FlowPropertyGrid, IZomBMonitor
    {
        Dictionary<string, TextBlock> elms = new Dictionary<string, TextBlock>();
        Queue<TextBlock> communists = new Queue<TextBlock>();//Its the reds!
        public DebugGrid()
        {
            this.Orientation = Orientation.Vertical;
            this.Background = Brushes.PeachPuff;
            this.Height = 100;
            this.Width = 125;
        }

        public void UpdateStatus(FRCDSStatus status) { }

        public void UpdateData(Dictionary<string, string> data)
        {
            while (communists.Count>0)
            {
                communists.Dequeue().Foreground = Brushes.Black;
            }
            var ks = from d in data where d.Key.StartsWith("dbg-") select new { Name = d.Key.Substring(4), Data = d.Value };
            foreach (var item in ks)
            {
                if (!elms.ContainsKey(item.Name))
                {
                    elms.Add(item.Name, new TextBlock());
                    var lbl = new TextBlock();
                    lbl.HorizontalAlignment = HorizontalAlignment.Right;
                    lbl.Text = item.Name+": ";
                    Children.Add(lbl);
                    Children.Add(elms[item.Name]);
                }
                if (elms[item.Name].Text != item.Data)
                {
                    elms[item.Name].Foreground = Brushes.Red;
                    communists.Enqueue(elms[item.Name]);
                }
                elms[item.Name].Text = item.Data;
            }
        }
    }
}
