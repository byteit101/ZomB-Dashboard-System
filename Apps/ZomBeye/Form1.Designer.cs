namespace ZomBeye
{
    partial class Form1
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
            this.zomBeye1 = new System451.Communication.Dashboard.ZomBeye();
            this.SuspendLayout();
            // 
            // zomBeye1
            // 
            this.zomBeye1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.zomBeye1.Location = new System.Drawing.Point(67, 33);
            this.zomBeye1.Name = "zomBeye1";
            this.zomBeye1.Size = new System.Drawing.Size(855, 660);
            this.zomBeye1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(988, 727);
            this.ControlBox = false;
            this.Controls.Add(this.zomBeye1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.Text = "ZomB eye";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System451.Communication.Dashboard.ZomBeye zomBeye1;
    }
}

