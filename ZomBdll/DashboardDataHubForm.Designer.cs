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
namespace System451.Communication.Dashboard
{
    partial class DashboardDataHubForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dashboardDataHub1 = new System451.Communication.Dashboard.DashboardDataHub();
            this.SuspendLayout();
            // 
            // dashboardDataHub1
            // 
            this.dashboardDataHub1.StartSources = "zomb://0.0.0.0/DBPkt";
            // 
            // DashboardDataHubForm
            // 
            this.BackgroundImage = global::System451.Communication.Dashboard.Properties.Resources.ZomBWFrame;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(1024, 400);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.Name = "DashboardDataHubForm";
            this.Text = "DashboardDataHub";
            this.Load += new System.EventHandler(this.DashboardDataHubForm_Load);
            this.SizeChanged += new System.EventHandler(this.DashboardDataHubForm_SizeChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private DashboardDataHub dashboardDataHub1;
    }
}
