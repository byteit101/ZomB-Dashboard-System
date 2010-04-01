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
            // DashboardDataHubForm
            // 
            this.BackgroundImage = global::System451.Communication.Dashboard.Properties.Resources.ZomB_text;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(1016, 366);
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
