namespace DefaultDash
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cameraView1 = new System451.Communication.Dashboard.CameraView();
            this.roundSpeedMeter2 = new System451.Communication.Dashboard.RoundSpeedMeter();
            this.roundSpeedMeter3 = new System451.Communication.Dashboard.RoundSpeedMeter();
            this.roundSpeedMeter1 = new System451.Communication.Dashboard.RoundSpeedMeter();
            this.onOffControl4 = new System451.Communication.Dashboard.OnOffControl();
            this.onOffControl3 = new System451.Communication.Dashboard.OnOffControl();
            this.onOffControl2 = new System451.Communication.Dashboard.OnOffControl();
            this.onOffControl1 = new System451.Communication.Dashboard.OnOffControl();
            this.spikeControl3 = new System451.Communication.Dashboard.SpikeControl();
            this.spikeControl2 = new System451.Communication.Dashboard.SpikeControl();
            this.spikeControl1 = new System451.Communication.Dashboard.SpikeControl();
            this.distanceMeter2 = new System451.Communication.Dashboard.DistanceMeter();
            this.distanceMeter1 = new System451.Communication.Dashboard.DistanceMeter();
            this.directionMeter2 = new System451.Communication.Dashboard.DirectionMeter();
            this.directionMeter1 = new System451.Communication.Dashboard.DirectionMeter();
            this.analogMeter4 = new System451.Communication.Dashboard.AnalogMeter();
            this.analogMeter3 = new System451.Communication.Dashboard.AnalogMeter();
            this.analogMeter2 = new System451.Communication.Dashboard.AnalogMeter();
            this.analogMeter1 = new System451.Communication.Dashboard.AnalogMeter();
            this.messageDisp1 = new System451.Communication.Dashboard.MessageDisp();
            this.varValue1 = new System451.Communication.Dashboard.VarValue();
            this.dataGraph2 = new System451.Communication.Dashboard.DataGraph();
            this.dataGraph1 = new System451.Communication.Dashboard.DataGraph();
            this.tacoMeter1 = new System451.Communication.Dashboard.TacoMeter();
            this.dashboardDataHub1 = new System451.Communication.Dashboard.DashboardDataHub();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(288, 105);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 13;
            this.button1.Text = "See Names";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(369, 105);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(92, 23);
            this.button2.TabIndex = 15;
            this.button2.Text = "Reset Camera";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::DefaultDash.Properties.Resources.varsexp2;
            this.pictureBox1.Location = new System.Drawing.Point(12, -32);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(41, 26);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 16;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // cameraView1
            // 
            this.cameraView1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.cameraView1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.cameraView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cameraView1.IPAddress = "10.4.51.2";
            this.cameraView1.Location = new System.Drawing.Point(224, 138);
            this.cameraView1.Name = "cameraView1";
            this.cameraView1.Size = new System.Drawing.Size(320, 240);
            this.cameraView1.TabIndex = 14;
            this.cameraView1.TargetFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.cameraView1.TargetWidth = 1.5F;
            // 
            // roundSpeedMeter2
            // 
            this.roundSpeedMeter2.BindToInput = "LMot";
            this.roundSpeedMeter2.Location = new System.Drawing.Point(561, 12);
            this.roundSpeedMeter2.Name = "roundSpeedMeter2";
            this.roundSpeedMeter2.Size = new System.Drawing.Size(119, 167);
            this.roundSpeedMeter2.TabIndex = 11;
            this.roundSpeedMeter2.Value = 0F;
            // 
            // roundSpeedMeter3
            // 
            this.roundSpeedMeter3.BindToInput = "Mot1";
            this.roundSpeedMeter3.Location = new System.Drawing.Point(550, 211);
            this.roundSpeedMeter3.Name = "roundSpeedMeter3";
            this.roundSpeedMeter3.Size = new System.Drawing.Size(119, 167);
            this.roundSpeedMeter3.TabIndex = 11;
            this.roundSpeedMeter3.Value = 0F;
            // 
            // roundSpeedMeter1
            // 
            this.roundSpeedMeter1.BindToInput = "RMot";
            this.roundSpeedMeter1.Location = new System.Drawing.Point(686, 12);
            this.roundSpeedMeter1.Name = "roundSpeedMeter1";
            this.roundSpeedMeter1.Size = new System.Drawing.Size(119, 167);
            this.roundSpeedMeter1.TabIndex = 11;
            this.roundSpeedMeter1.Value = 0F;
            // 
            // onOffControl4
            // 
            this.onOffControl4.BindToInput = "sw4";
            this.onOffControl4.Location = new System.Drawing.Point(828, 192);
            this.onOffControl4.Name = "onOffControl4";
            this.onOffControl4.Size = new System.Drawing.Size(25, 25);
            this.onOffControl4.TabIndex = 10;
            this.onOffControl4.Value = false;
            // 
            // onOffControl3
            // 
            this.onOffControl3.BindToInput = "sw3";
            this.onOffControl3.Location = new System.Drawing.Point(828, 166);
            this.onOffControl3.Name = "onOffControl3";
            this.onOffControl3.Size = new System.Drawing.Size(25, 25);
            this.onOffControl3.TabIndex = 10;
            this.onOffControl3.Value = false;
            // 
            // onOffControl2
            // 
            this.onOffControl2.BindToInput = "sw2";
            this.onOffControl2.Location = new System.Drawing.Point(828, 135);
            this.onOffControl2.Name = "onOffControl2";
            this.onOffControl2.Size = new System.Drawing.Size(25, 25);
            this.onOffControl2.TabIndex = 10;
            this.onOffControl2.Value = false;
            // 
            // onOffControl1
            // 
            this.onOffControl1.BindToInput = "sw1";
            this.onOffControl1.Location = new System.Drawing.Point(828, 105);
            this.onOffControl1.Name = "onOffControl1";
            this.onOffControl1.Size = new System.Drawing.Size(25, 25);
            this.onOffControl1.TabIndex = 10;
            this.onOffControl1.Value = false;
            // 
            // spikeControl3
            // 
            this.spikeControl3.BindToInput = "spk3";
            this.spikeControl3.Location = new System.Drawing.Point(828, 74);
            this.spikeControl3.Name = "spikeControl3";
            this.spikeControl3.Size = new System.Drawing.Size(25, 25);
            this.spikeControl3.TabIndex = 9;
            this.spikeControl3.Value = System451.Communication.Dashboard.SpikePositions.Off;
            // 
            // spikeControl2
            // 
            this.spikeControl2.BindToInput = "spk2";
            this.spikeControl2.Location = new System.Drawing.Point(828, 43);
            this.spikeControl2.Name = "spikeControl2";
            this.spikeControl2.Size = new System.Drawing.Size(25, 25);
            this.spikeControl2.TabIndex = 9;
            this.spikeControl2.Value = System451.Communication.Dashboard.SpikePositions.Off;
            // 
            // spikeControl1
            // 
            this.spikeControl1.Location = new System.Drawing.Point(828, 12);
            this.spikeControl1.Name = "spikeControl1";
            this.spikeControl1.Size = new System.Drawing.Size(25, 25);
            this.spikeControl1.TabIndex = 8;
            this.spikeControl1.Value = System451.Communication.Dashboard.SpikePositions.Off;
            // 
            // distanceMeter2
            // 
            this.distanceMeter2.BindToInput = "bat2";
            this.distanceMeter2.DistanceColor = System.Drawing.Color.Peru;
            this.distanceMeter2.Location = new System.Drawing.Point(118, 228);
            this.distanceMeter2.Name = "distanceMeter2";
            this.distanceMeter2.Size = new System.Drawing.Size(100, 150);
            this.distanceMeter2.TabIndex = 6;
            this.distanceMeter2.Value = 0F;
            // 
            // distanceMeter1
            // 
            this.distanceMeter1.BindToInput = "bat";
            this.distanceMeter1.DistanceColor = System.Drawing.Color.Peru;
            this.distanceMeter1.Location = new System.Drawing.Point(12, 228);
            this.distanceMeter1.Name = "distanceMeter1";
            this.distanceMeter1.Size = new System.Drawing.Size(100, 150);
            this.distanceMeter1.TabIndex = 6;
            this.distanceMeter1.Value = 0F;
            // 
            // directionMeter2
            // 
            this.directionMeter2.ArrowColor = System.Drawing.Color.Navy;
            this.directionMeter2.ArrowWidth = 4F;
            this.directionMeter2.BindToInput = "dir";
            this.directionMeter2.CircleColor = System.Drawing.Color.IndianRed;
            this.directionMeter2.CircleWidth = 5F;
            this.directionMeter2.GuidesColor = System.Drawing.Color.DeepSkyBlue;
            this.directionMeter2.GuidesWidth = 1F;
            this.directionMeter2.Location = new System.Drawing.Point(12, 122);
            this.directionMeter2.Name = "directionMeter2";
            this.directionMeter2.Size = new System.Drawing.Size(100, 100);
            this.directionMeter2.TabIndex = 5;
            this.directionMeter2.Value = 1F;
            // 
            // directionMeter1
            // 
            this.directionMeter1.ArrowColor = System.Drawing.Color.Navy;
            this.directionMeter1.ArrowWidth = 4F;
            this.directionMeter1.BindToInput = "dir2";
            this.directionMeter1.CircleColor = System.Drawing.Color.IndianRed;
            this.directionMeter1.CircleWidth = 5F;
            this.directionMeter1.GuidesColor = System.Drawing.Color.DeepSkyBlue;
            this.directionMeter1.GuidesWidth = 1F;
            this.directionMeter1.Location = new System.Drawing.Point(118, 122);
            this.directionMeter1.Name = "directionMeter1";
            this.directionMeter1.Size = new System.Drawing.Size(100, 100);
            this.directionMeter1.TabIndex = 4;
            this.directionMeter1.Value = 0F;
            // 
            // analogMeter4
            // 
            this.analogMeter4.ArrowColor = System.Drawing.Color.SlateGray;
            this.analogMeter4.ArrowWidth = 4F;
            this.analogMeter4.BindToInput = "anl4";
            this.analogMeter4.CircleColor = System.Drawing.Color.PaleGreen;
            this.analogMeter4.GuidesColor = System.Drawing.Color.Black;
            this.analogMeter4.GuidesWidth = 1F;
            this.analogMeter4.Location = new System.Drawing.Point(330, 12);
            this.analogMeter4.Name = "analogMeter4";
            this.analogMeter4.Size = new System.Drawing.Size(100, 75);
            this.analogMeter4.TabIndex = 3;
            this.analogMeter4.Use0To1023 = true;
            this.analogMeter4.Value = 0F;
            // 
            // analogMeter3
            // 
            this.analogMeter3.ArrowColor = System.Drawing.Color.SlateGray;
            this.analogMeter3.ArrowWidth = 4F;
            this.analogMeter3.BindToInput = "anl3";
            this.analogMeter3.CircleColor = System.Drawing.Color.PaleGreen;
            this.analogMeter3.GuidesColor = System.Drawing.Color.Black;
            this.analogMeter3.GuidesWidth = 1F;
            this.analogMeter3.Location = new System.Drawing.Point(224, 12);
            this.analogMeter3.Name = "analogMeter3";
            this.analogMeter3.Size = new System.Drawing.Size(100, 75);
            this.analogMeter3.TabIndex = 3;
            this.analogMeter3.Use0To1023 = true;
            this.analogMeter3.Value = 0F;
            // 
            // analogMeter2
            // 
            this.analogMeter2.ArrowColor = System.Drawing.Color.SlateGray;
            this.analogMeter2.ArrowWidth = 4F;
            this.analogMeter2.BindToInput = "anl2";
            this.analogMeter2.CircleColor = System.Drawing.Color.PaleGreen;
            this.analogMeter2.GuidesColor = System.Drawing.Color.Black;
            this.analogMeter2.GuidesWidth = 1F;
            this.analogMeter2.Location = new System.Drawing.Point(118, 12);
            this.analogMeter2.Name = "analogMeter2";
            this.analogMeter2.Size = new System.Drawing.Size(100, 75);
            this.analogMeter2.TabIndex = 3;
            this.analogMeter2.Use0To1023 = true;
            this.analogMeter2.Value = 0F;
            // 
            // analogMeter1
            // 
            this.analogMeter1.ArrowColor = System.Drawing.Color.SlateGray;
            this.analogMeter1.ArrowWidth = 4F;
            this.analogMeter1.BindToInput = "anl1";
            this.analogMeter1.CircleColor = System.Drawing.Color.PaleGreen;
            this.analogMeter1.GuidesColor = System.Drawing.Color.Black;
            this.analogMeter1.GuidesWidth = 1F;
            this.analogMeter1.Location = new System.Drawing.Point(12, 12);
            this.analogMeter1.Name = "analogMeter1";
            this.analogMeter1.Size = new System.Drawing.Size(100, 75);
            this.analogMeter1.TabIndex = 3;
            this.analogMeter1.Use0To1023 = true;
            this.analogMeter1.Value = 0F;
            // 
            // messageDisp1
            // 
            this.messageDisp1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.messageDisp1.Append = false;
            this.messageDisp1.BindToInput = "MSG";
            this.messageDisp1.Location = new System.Drawing.Point(343, 384);
            this.messageDisp1.Name = "messageDisp1";
            this.messageDisp1.Size = new System.Drawing.Size(313, 206);
            this.messageDisp1.TabIndex = 2;
            this.messageDisp1.Value = "";
            // 
            // varValue1
            // 
            this.varValue1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.varValue1.AutoScroll = true;
            this.varValue1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("varValue1.BackgroundImage")));
            this.varValue1.Location = new System.Drawing.Point(662, 384);
            this.varValue1.Name = "varValue1";
            this.varValue1.ParamName = new string[] {
        "dbg"};
            this.varValue1.Size = new System.Drawing.Size(191, 206);
            this.varValue1.TabIndex = 1;
            // 
            // dataGraph2
            // 
            this.dataGraph2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dataGraph2.BackColor = System.Drawing.Color.Black;
            this.dataGraph2.BindToInput = "grph2";
            this.dataGraph2.Location = new System.Drawing.Point(12, 490);
            this.dataGraph2.Max = 1F;
            this.dataGraph2.Min = -1F;
            this.dataGraph2.Name = "dataGraph2";
            this.dataGraph2.Size = new System.Drawing.Size(325, 100);
            this.dataGraph2.TabIndex = 0;
            this.dataGraph2.Value = 0F;
            // 
            // dataGraph1
            // 
            this.dataGraph1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dataGraph1.BackColor = System.Drawing.Color.Black;
            this.dataGraph1.BindToInput = "grph";
            this.dataGraph1.Location = new System.Drawing.Point(12, 384);
            this.dataGraph1.Max = 1F;
            this.dataGraph1.Min = -1F;
            this.dataGraph1.Name = "dataGraph1";
            this.dataGraph1.Size = new System.Drawing.Size(325, 100);
            this.dataGraph1.TabIndex = 0;
            this.dataGraph1.Value = 0F;
            // 
            // tacoMeter1
            // 
            this.tacoMeter1.Location = new System.Drawing.Point(673, 255);
            this.tacoMeter1.Name = "tacoMeter1";
            this.tacoMeter1.Size = new System.Drawing.Size(200, 150);
            this.tacoMeter1.TabIndex = 7;
            this.tacoMeter1.Value = 0F;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(865, 602);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.cameraView1);
            this.Controls.Add(this.roundSpeedMeter2);
            this.Controls.Add(this.roundSpeedMeter3);
            this.Controls.Add(this.roundSpeedMeter1);
            this.Controls.Add(this.onOffControl4);
            this.Controls.Add(this.onOffControl3);
            this.Controls.Add(this.onOffControl2);
            this.Controls.Add(this.onOffControl1);
            this.Controls.Add(this.spikeControl3);
            this.Controls.Add(this.spikeControl2);
            this.Controls.Add(this.spikeControl1);
            this.Controls.Add(this.distanceMeter2);
            this.Controls.Add(this.distanceMeter1);
            this.Controls.Add(this.directionMeter2);
            this.Controls.Add(this.directionMeter1);
            this.Controls.Add(this.analogMeter4);
            this.Controls.Add(this.analogMeter3);
            this.Controls.Add(this.analogMeter2);
            this.Controls.Add(this.analogMeter1);
            this.Controls.Add(this.messageDisp1);
            this.Controls.Add(this.varValue1);
            this.Controls.Add(this.dataGraph2);
            this.Controls.Add(this.dataGraph1);
            this.Controls.Add(this.tacoMeter1);
            this.Name = "Form1";
            this.Text = "Default Dashboard";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System451.Communication.Dashboard.DataGraph dataGraph1;
        private System451.Communication.Dashboard.DataGraph dataGraph2;
        private System451.Communication.Dashboard.VarValue varValue1;
        private System451.Communication.Dashboard.MessageDisp messageDisp1;
        private System451.Communication.Dashboard.AnalogMeter analogMeter1;
        private System451.Communication.Dashboard.AnalogMeter analogMeter2;
        private System451.Communication.Dashboard.AnalogMeter analogMeter3;
        private System451.Communication.Dashboard.AnalogMeter analogMeter4;
        private System451.Communication.Dashboard.DashboardDataHub dashboardDataHub1;
        private System451.Communication.Dashboard.DirectionMeter directionMeter1;
        private System451.Communication.Dashboard.DirectionMeter directionMeter2;
        private System451.Communication.Dashboard.DistanceMeter distanceMeter1;
        private System451.Communication.Dashboard.DistanceMeter distanceMeter2;
        private System451.Communication.Dashboard.TacoMeter tacoMeter1;
        private System451.Communication.Dashboard.SpikeControl spikeControl1;
        private System451.Communication.Dashboard.SpikeControl spikeControl2;
        private System451.Communication.Dashboard.SpikeControl spikeControl3;
        private System451.Communication.Dashboard.OnOffControl onOffControl1;
        private System451.Communication.Dashboard.OnOffControl onOffControl2;
        private System451.Communication.Dashboard.OnOffControl onOffControl3;
        private System451.Communication.Dashboard.OnOffControl onOffControl4;
        private System451.Communication.Dashboard.RoundSpeedMeter roundSpeedMeter1;
        private System451.Communication.Dashboard.RoundSpeedMeter roundSpeedMeter2;
        private System451.Communication.Dashboard.RoundSpeedMeter roundSpeedMeter3;
        private System.Windows.Forms.Button button1;
        private System451.Communication.Dashboard.CameraView cameraView1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

