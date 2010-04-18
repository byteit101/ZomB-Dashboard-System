namespace System451.Communication.Dashboard
{
    partial class TacoMeter
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // TacoMeter
            // 
            this.DoubleBuffered = true;
            this.Name = "TacoMeter";
            this.Size = new System.Drawing.Size(400, 300);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.TacoMeter_Paint);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
