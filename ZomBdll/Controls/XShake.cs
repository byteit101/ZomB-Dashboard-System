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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System451.Communication.Dashboard.Utils;
using System451.Communication.Dashboard.Libs.Xbox360Controller;
using SlimDX.XInput;

namespace System451.Communication.Dashboard.Controls
{
    /// <summary>
    /// A ZomB control that displays text at different locations
    /// </summary>
    [ToolboxBitmap(typeof(icofinds), "System451.Communication.Dashboard.TBB.Vibrate.ico")]
    public class XShake : ZomBControl
    {
        int padNumber = 0;
        GamepadState pad;

        /// <summary>
        /// Creates a new Shaker
        /// </summary>
        public XShake()
        {
            AutoExtractor.Extract(AutoExtractor.Files.SlimDX);
            pad = null;
        }

        [Category("ZomB"), Description("Gamepad number")]
        public int GamepadNumber
        {
            get
            {
                return padNumber;
            }
            set
            {
                if (value >= 0 && value <= 3)
                {
                    padNumber = value;
                    try
                    {
                        pad = new GamepadState(padNumber);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("No Gamepad found on that port.\r\n\r\n" + ex.Message);
                    }
                }
                else
                    throw new IndexOutOfRangeException("Pad Number must be between 0 and 3");
            }
        }

        public override void UpdateControl(string value)
        {
            if (pad != null)
            {
                float l, r;
                if (value.Contains(";"))
                {
                    try
                    {
                        l = float.Parse(value.Substring(0, value.IndexOf(';')));
                        r = float.Parse(value.Substring(1 + value.IndexOf(';')));
                    }
                    catch
                    {
                        l = r = 0;
                    }
                }
                else
                {
                    try
                    {
                        l = r = float.Parse(value);
                    }
                    catch
                    {
                        l = r = 0;
                    }
                }
                pad.Vibrate(l, r);
            }
        }
    }
}
