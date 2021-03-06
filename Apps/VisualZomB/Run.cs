﻿/*
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
using System.IO;
using System.Text;
using System.Windows.Markup;
using System451.Communication.Dashboard.WPF.Controls;

namespace System451.Communication.Dashboard.ViZ
{
    public class Run : DashboardDataHubWindow
    {
        public Run(string xaml)
            : base(true)
        {
            this.Content = XamlReader.Load(new MemoryStream(Encoding.UTF8.GetBytes(xaml)));
        }
    }

    public class AppRunner : MarshalByRefObject
    {
        Run r;
        public AppRunner()
        {

        }

        public void Run(string xaml)
        {
            System451.Communication.Dashboard.ViZ.App app = new System451.Communication.Dashboard.ViZ.App();
            App.LoadPlugins();
            App.LoadAssembliesGeneric();
            r = new Run(xaml);
        }
        public void Start()
        {
            {
                try
                {
                    r.ShowDialog();
                }
                catch { }
                try
                {
                    r.DashboardDataHub.Stop();
                    r.StopAll();
                }
                catch { }
                r = null;
            }
            GC.Collect();
        }
    }
}
