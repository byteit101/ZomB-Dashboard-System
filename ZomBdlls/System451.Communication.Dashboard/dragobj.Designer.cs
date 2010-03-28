namespace System451.Communication.Dashboard
{
    partial class dragobj
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
            // dragobj
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.DoubleBuffered = true;
            this.Name = "dragobj";
            this.Size = new System.Drawing.Size(371, 317);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.dragobj_Paint);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dragobj_MouseMove);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dragobj_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dragobj_MouseUp);
            this.Move += new System.EventHandler(this.dragobj_Move);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
