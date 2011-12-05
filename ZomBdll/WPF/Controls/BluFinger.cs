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
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System451.Communication.Dashboard.Net;
using System451.Communication.Dashboard.Utils;

namespace System451.Communication.Dashboard.WPF.Controls
{
    /// <summary>
    /// Interaction logic for BluFinger.xaml
    /// </summary>
    [Design.ZomBControl("BluFinger Server", Description = "This will run the BluFinger server, enabling bluetooth transfer to a display computer", IconName = "BluFingerIcon")]
    [Design.ZomBDesignableProperty("Width", Dynamic = true, Category = "Layout")]
    [Design.ZomBDesignableProperty("Height", Dynamic = true, Category = "Layout")]
    [TemplatePart(Name="PART_Status", Type=typeof(TextBlock))]
    public class BluFinger : Control, IZomBControl
    {
        TextBlock status;
        int team = 0;
        BTZomBServer bf;

        static BluFinger()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BluFinger),
            new FrameworkPropertyMetadata(typeof(BluFinger)));
        }

        public BluFinger()
        {
            this.Background = Brushes.LightBlue;
            this.MouseDoubleClick += delegate
            {
                if (bf != null)
                    bf.Start();
            };
        }

        ~BluFinger()
        {
            if (bf != null)
                bf.Stop();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            status = base.GetTemplateChild("PART_Status") as TextBlock;
            if (status!=null)
            status.Text = "Initializing...";
            init();
        }

        private void init()
        {
            if (team == 0 || ZDesigner.IsDesignMode)
                return;
            bf = (BlueFinger.GetFactory(team, BTZomBFingerFactory.DefaultLoadLocation, BTZomBFingerFactory.DefaultSaveLocation)).GetServer();
            bf.DataSending += delegate
            {
                if (status != null)
                    status.Text = "Sending...";
            };
            bf.DataSent += delegate
            {
                if (status != null)
                    status.Text = "Ready";
            };
            if (status != null)
                status.Text = "Ready";
        }

        #region IZomBControl Members

        public string ControlName
        {
            get { return "BluFinger"; }
        }

        public void UpdateControl(ZomBDataObject value)
        {
            
        }

        public void ControlAdded(object sender, ZomBControlAddedEventArgs e)
        {
            team = e.Controller.GetDashboardDataHub().Team;
            e.Controller.GetDashboardDataHub().Remove(this);
            init();
        }

        #endregion
    }
}
