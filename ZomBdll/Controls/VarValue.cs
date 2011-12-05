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
using System.Collections.Generic;
using System.Windows.Forms;

namespace System451.Communication.Dashboard.Controls
{
    public partial class VarValue : UserControl, IZomBControl
    {
        ZomBDataLookup vrs = new ZomBDataLookup();

        public VarValue()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        public string ControlName
        {
            get
            {
                return "*";
            }
        }

        public void UpdateControl(ZomBDataObject valuea)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Utils.ZomBDataFunction(UpdateControl), valuea);
            }
            else
            {
                string value = valuea.ToString();
                if (value != "")
                {
                    if (value.Contains("="))
                        vrs[value.Substring(0, value.IndexOf("="))] = value.Substring(value.IndexOf("=") + 1);
                }
                label1.Text = label2.Text = "";
                foreach (KeyValuePair<string, ZomBDataObject> kv in vrs)
                {
                    label1.Text += kv.Key + "\r\n";
                    label2.Text += kv.Value + "\r\n";
                }
                label1.Text += " \r\n";
                label2.Text += " \r\n";
                this.Update();
            }
        }
        private void panel1_Scroll(object sender, ScrollEventArgs e)
        {
            this.Invalidate();
        }

        #region IZomBControl Members

        void IZomBControl.UpdateControl(ZomBDataObject value)
        {
            //this needs to be tested, but should work
            string Output = value;
            string[] vars = Output.Split('|');
            foreach (string item in vars)
            {
                if (item.StartsWith("dbg-"))
                    UpdateControl(item.Substring(4));
            }
        }

        public new void ControlAdded(object sender, ZomBControlAddedEventArgs e)
        {

        }

        #endregion
    }
}
