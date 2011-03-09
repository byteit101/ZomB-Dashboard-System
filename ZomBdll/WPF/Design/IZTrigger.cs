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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System451.Communication.Dashboard.Utils;
using System451.Communication.Dashboard.WPF.Controls.Designers;

namespace System451.Communication.Dashboard
{
    /// <summary>
    /// Used to trigger an action
    /// </summary>
    public interface IZTrigger
    {
        /// <summary>
        /// Use the format name:function;name2:function
        /// </summary>
        TriggerListeners TriggerListeners { get; set; }

        /// <summary>
        /// Called when the trigger is tripped
        /// </summary>
        event VoidFunction Triggered;
    }

    [WPF.Design.Designer(typeof(TriggerListenersDesigner))]
    public class TriggerListeners
    {
        public string Listeners { get; set; }

        public static implicit operator string(TriggerListeners triggerListen)
        {
            return triggerListen.Listeners;
        }

        public static implicit operator TriggerListeners(string triggerListen)
        {
            return new TriggerListeners { Listeners = triggerListen };
        }

        public override string ToString()
        {
            return Listeners;
        }

        public string[] Split()
        {
            return Listeners.Split(';');
        }
    }
}
