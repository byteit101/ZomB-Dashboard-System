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
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.cameraView1 = new System451.Communication.Dashboard.Controls.CameraView();
            this.videoContextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.startBothToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.robotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.webcamToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.roundSpeedMeter2 = new System451.Communication.Dashboard.Controls.RoundSpeedMeter();
            this.roundSpeedMeter1 = new System451.Communication.Dashboard.Controls.RoundSpeedMeter();
            this.onOffControl2 = new System451.Communication.Dashboard.Controls.OnOffControl();
            this.onOffControl1 = new System451.Communication.Dashboard.Controls.OnOffControl();
            this.spikeControl2 = new System451.Communication.Dashboard.Controls.SpikeControl();
            this.spikeControl1 = new System451.Communication.Dashboard.Controls.SpikeControl();
            this.directionMeter2 = new System451.Communication.Dashboard.Controls.DirectionMeter();
            this.directionMeter1 = new System451.Communication.Dashboard.Controls.DirectionMeter();
            this.analogMeter3 = new System451.Communication.Dashboard.Controls.AnalogMeter();
            this.analogMeter2 = new System451.Communication.Dashboard.Controls.AnalogMeter();
            this.analogMeter1 = new System451.Communication.Dashboard.Controls.AnalogMeter();
            this.dataGraph2 = new System451.Communication.Dashboard.Controls.DataGraph();
            this.dataGraph1 = new System451.Communication.Dashboard.Controls.DataGraph();
            this.tacoMeter1 = new System451.Communication.Dashboard.Controls.TacoMeter();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.onOffControl3 = new System451.Communication.Dashboard.Controls.OnOffControl();
            this.controlBoxMenuButton1 = new System451.Communication.Dashboard.ControlBoxMenuButton();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.valueMeter1 = new System451.Communication.Dashboard.Controls.ValueMeter();
            this.videoContextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(369, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 13;
            this.button1.Text = "See Names";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(450, 10);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(92, 23);
            this.button2.TabIndex = 15;
            this.button2.Text = "Reset Camera";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // cameraView1
            // 
            this.cameraView1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.cameraView1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.cameraView1.ContextMenuStrip = this.videoContextMenuStrip1;
            this.cameraView1.Location = new System.Drawing.Point(330, 43);
            this.cameraView1.Name = "cameraView1";
            this.cameraView1.Size = new System.Drawing.Size(333, 243);
            this.cameraView1.TabIndex = 14;
            this.cameraView1.TargetFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.cameraView1.TargetWidth = 1.5F;
            this.cameraView1.TeamNumber = 451;
            // 
            // videoContextMenuStrip1
            // 
            this.videoContextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startBothToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.sourceToolStripMenuItem});
            this.videoContextMenuStrip1.Name = "contextMenuStrip1";
            this.videoContextMenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.videoContextMenuStrip1.ShowImageMargin = false;
            this.videoContextMenuStrip1.Size = new System.Drawing.Size(242, 94);
            // 
            // startBothToolStripMenuItem
            // 
            this.startBothToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startBothToolStripMenuItem.Name = "startBothToolStripMenuItem";
            this.startBothToolStripMenuItem.Size = new System.Drawing.Size(241, 30);
            this.startBothToolStripMenuItem.Text = "Start Saving Video";
            this.startBothToolStripMenuItem.Click += new System.EventHandler(this.startBothToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(241, 30);
            this.stopToolStripMenuItem.Text = "End Video Capture";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // sourceToolStripMenuItem
            // 
            this.sourceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.robotToolStripMenuItem,
            this.webcamToolStripMenuItem});
            this.sourceToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sourceToolStripMenuItem.Name = "sourceToolStripMenuItem";
            this.sourceToolStripMenuItem.Size = new System.Drawing.Size(241, 30);
            this.sourceToolStripMenuItem.Text = "Source";
            // 
            // robotToolStripMenuItem
            // 
            this.robotToolStripMenuItem.Checked = true;
            this.robotToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.robotToolStripMenuItem.Name = "robotToolStripMenuItem";
            this.robotToolStripMenuItem.Size = new System.Drawing.Size(186, 30);
            this.robotToolStripMenuItem.Text = "Robot";
            this.robotToolStripMenuItem.Click += new System.EventHandler(this.robotToolStripMenuItem_Click);
            // 
            // webcamToolStripMenuItem
            // 
            this.webcamToolStripMenuItem.Name = "webcamToolStripMenuItem";
            this.webcamToolStripMenuItem.Size = new System.Drawing.Size(186, 30);
            this.webcamToolStripMenuItem.Text = "Webcam";
            this.webcamToolStripMenuItem.Click += new System.EventHandler(this.robotToolStripMenuItem_Click);
            // 
            // roundSpeedMeter2
            // 
            this.roundSpeedMeter2.ControlName = "left";
            this.roundSpeedMeter2.Location = new System.Drawing.Point(669, 12);
            this.roundSpeedMeter2.Name = "roundSpeedMeter2";
            this.roundSpeedMeter2.Size = new System.Drawing.Size(148, 101);
            this.roundSpeedMeter2.TabIndex = 11;
            this.roundSpeedMeter2.Value = 0F;
            // 
            // roundSpeedMeter1
            // 
            this.roundSpeedMeter1.ControlName = "right";
            this.roundSpeedMeter1.Location = new System.Drawing.Point(823, 12);
            this.roundSpeedMeter1.Name = "roundSpeedMeter1";
            this.roundSpeedMeter1.Size = new System.Drawing.Size(150, 101);
            this.roundSpeedMeter1.TabIndex = 11;
            this.roundSpeedMeter1.Value = 0F;
            // 
            // onOffControl2
            // 
            this.onOffControl2.ControlName = "sw2";
            this.onOffControl2.Location = new System.Drawing.Point(979, 104);
            this.onOffControl2.Name = "onOffControl2";
            this.onOffControl2.Size = new System.Drawing.Size(25, 25);
            this.onOffControl2.TabIndex = 10;
            this.onOffControl2.Value = false;
            // 
            // onOffControl1
            // 
            this.onOffControl1.ControlName = "sw1";
            this.onOffControl1.Location = new System.Drawing.Point(979, 74);
            this.onOffControl1.Name = "onOffControl1";
            this.onOffControl1.Size = new System.Drawing.Size(25, 25);
            this.onOffControl1.TabIndex = 10;
            this.onOffControl1.Value = false;
            // 
            // spikeControl2
            // 
            this.spikeControl2.ControlName = "spk2";
            this.spikeControl2.Location = new System.Drawing.Point(979, 43);
            this.spikeControl2.Name = "spikeControl2";
            this.spikeControl2.Size = new System.Drawing.Size(25, 25);
            this.spikeControl2.TabIndex = 9;
            this.spikeControl2.Value = System451.Communication.Dashboard.SpikePositions.Off;
            // 
            // spikeControl1
            // 
            this.spikeControl1.ControlName = "spike1";
            this.spikeControl1.Location = new System.Drawing.Point(979, 12);
            this.spikeControl1.Name = "spikeControl1";
            this.spikeControl1.Size = new System.Drawing.Size(25, 25);
            this.spikeControl1.TabIndex = 8;
            this.spikeControl1.Value = System451.Communication.Dashboard.SpikePositions.Off;
            // 
            // directionMeter2
            // 
            this.directionMeter2.ArrowWidth = 4F;
            this.directionMeter2.CircleWidth = 5F;
            this.directionMeter2.ControlName = "dir";
            this.directionMeter2.GuidesWidth = 1F;
            this.directionMeter2.Location = new System.Drawing.Point(12, 13);
            this.directionMeter2.Max = 360F;
            this.directionMeter2.Min = 0F;
            this.directionMeter2.Name = "directionMeter2";
            this.directionMeter2.Size = new System.Drawing.Size(100, 100);
            this.directionMeter2.TabIndex = 5;
            this.directionMeter2.Value = 1F;
            // 
            // directionMeter1
            // 
            this.directionMeter1.ArrowWidth = 4F;
            this.directionMeter1.CircleWidth = 5F;
            this.directionMeter1.ControlName = "dir2";
            this.directionMeter1.GuidesWidth = 1F;
            this.directionMeter1.Location = new System.Drawing.Point(118, 13);
            this.directionMeter1.Max = 360F;
            this.directionMeter1.Min = 0F;
            this.directionMeter1.Name = "directionMeter1";
            this.directionMeter1.Size = new System.Drawing.Size(100, 100);
            this.directionMeter1.TabIndex = 4;
            this.directionMeter1.Value = 0F;
            // 
            // analogMeter3
            // 
            this.analogMeter3.ArrowWidth = 4F;
            this.analogMeter3.ControlName = "anl3";
            this.analogMeter3.GuidesWidth = 1F;
            this.analogMeter3.Location = new System.Drawing.Point(562, 279);
            this.analogMeter3.Name = "analogMeter3";
            this.analogMeter3.Size = new System.Drawing.Size(100, 75);
            this.analogMeter3.TabIndex = 3;
            this.analogMeter3.Use0To1023 = true;
            this.analogMeter3.Value = 0F;
            // 
            // analogMeter2
            // 
            this.analogMeter2.ArrowWidth = 4F;
            this.analogMeter2.ControlName = "anl2";
            this.analogMeter2.GuidesWidth = 1F;
            this.analogMeter2.Location = new System.Drawing.Point(446, 279);
            this.analogMeter2.Name = "analogMeter2";
            this.analogMeter2.Size = new System.Drawing.Size(100, 75);
            this.analogMeter2.TabIndex = 3;
            this.analogMeter2.Use0To1023 = true;
            this.analogMeter2.Value = 0F;
            // 
            // analogMeter1
            // 
            this.analogMeter1.ArrowWidth = 4F;
            this.analogMeter1.ControlName = "anl1";
            this.analogMeter1.GuidesWidth = 1F;
            this.analogMeter1.Location = new System.Drawing.Point(330, 279);
            this.analogMeter1.Name = "analogMeter1";
            this.analogMeter1.Size = new System.Drawing.Size(100, 75);
            this.analogMeter1.TabIndex = 3;
            this.analogMeter1.Use0To1023 = true;
            this.analogMeter1.Value = 0F;
            // 
            // dataGraph2
            // 
            this.dataGraph2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dataGraph2.BackColor = System.Drawing.Color.Black;
            this.dataGraph2.ControlName = "grph";
            this.dataGraph2.ForeColor = System.Drawing.Color.Green;
            this.dataGraph2.Location = new System.Drawing.Point(12, 132);
            this.dataGraph2.Max = 1F;
            this.dataGraph2.Min = -1F;
            this.dataGraph2.Name = "dataGraph2";
            this.dataGraph2.Size = new System.Drawing.Size(312, 128);
            this.dataGraph2.TabIndex = 0;
            this.dataGraph2.Value = 0F;
            // 
            // dataGraph1
            // 
            this.dataGraph1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dataGraph1.BackColor = System.Drawing.Color.Black;
            this.dataGraph1.ControlName = "grph2";
            this.dataGraph1.ForeColor = System.Drawing.Color.Green;
            this.dataGraph1.Location = new System.Drawing.Point(12, 266);
            this.dataGraph1.Max = 1F;
            this.dataGraph1.Min = -1F;
            this.dataGraph1.Name = "dataGraph1";
            this.dataGraph1.Size = new System.Drawing.Size(312, 122);
            this.dataGraph1.TabIndex = 0;
            this.dataGraph1.Value = 0F;
            // 
            // tacoMeter1
            // 
            this.tacoMeter1.ControlName = "taco";
            this.tacoMeter1.Location = new System.Drawing.Point(775, 161);
            this.tacoMeter1.Name = "tacoMeter1";
            this.tacoMeter1.Size = new System.Drawing.Size(233, 193);
            this.tacoMeter1.TabIndex = 7;
            this.tacoMeter1.Value = 0F;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(548, 13);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(75, 20);
            this.numericUpDown1.TabIndex = 17;
            this.numericUpDown1.Value = new decimal(new int[] {
            451,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 116);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(268, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Default ZomB Dashboard By Team 451 The Cat Attack";
            // 
            // onOffControl3
            // 
            this.onOffControl3.ControlName = "GO";
            this.onOffControl3.Location = new System.Drawing.Point(224, 13);
            this.onOffControl3.Name = "onOffControl3";
            this.onOffControl3.Size = new System.Drawing.Size(100, 100);
            this.onOffControl3.TabIndex = 19;
            this.onOffControl3.Value = false;
            // 
            // controlBoxMenuButton1
            // 
            this.controlBoxMenuButton1.Location = new System.Drawing.Point(331, 360);
            this.controlBoxMenuButton1.Name = "controlBoxMenuButton1";
            this.controlBoxMenuButton1.Size = new System.Drawing.Size(332, 28);
            this.controlBoxMenuButton1.TabIndex = 21;
            this.controlBoxMenuButton1.Text = "Exit ZomB, Restart ZomB, or Restart DS";
            this.controlBoxMenuButton1.UseVisualStyleBackColor = true;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(775, 304);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(237, 84);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 20;
            this.pictureBox2.TabStop = false;
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
            // valueMeter1
            // 
            this.valueMeter1.BarColor = System.Drawing.Color.DeepSkyBlue;
            this.valueMeter1.BarWidth = 4F;
            this.valueMeter1.ControlName = "bat";
            this.valueMeter1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.valueMeter1.ForeColor = System.Drawing.Color.Peru;
            this.valueMeter1.HighColor = System.Drawing.Color.Orange;
            this.valueMeter1.HighThreshold = 13.25F;
            this.valueMeter1.Label = true;
            this.valueMeter1.Location = new System.Drawing.Point(669, 185);
            this.valueMeter1.LowColor = System.Drawing.Color.Red;
            this.valueMeter1.LowThreshold = 9.5F;
            this.valueMeter1.Max = 14.5F;
            this.valueMeter1.Min = 7F;
            this.valueMeter1.Name = "valueMeter1";
            this.valueMeter1.Size = new System.Drawing.Size(100, 169);
            this.valueMeter1.TabIndex = 22;
            this.valueMeter1.Value = 12F;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.ClientSize = new System.Drawing.Size(1024, 400);
            this.ControlBox = false;
            this.Controls.Add(this.valueMeter1);
            this.Controls.Add(this.controlBoxMenuButton1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.onOffControl3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.cameraView1);
            this.Controls.Add(this.roundSpeedMeter2);
            this.Controls.Add(this.roundSpeedMeter1);
            this.Controls.Add(this.onOffControl2);
            this.Controls.Add(this.onOffControl1);
            this.Controls.Add(this.spikeControl1);
            this.Controls.Add(this.directionMeter2);
            this.Controls.Add(this.directionMeter1);
            this.Controls.Add(this.spikeControl2);
            this.Controls.Add(this.analogMeter3);
            this.Controls.Add(this.analogMeter2);
            this.Controls.Add(this.dataGraph2);
            this.Controls.Add(this.dataGraph1);
            this.Controls.Add(this.tacoMeter1);
            this.Controls.Add(this.analogMeter1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.Text = "Default Dashboard";
            this.videoContextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System451.Communication.Dashboard.Controls.DataGraph dataGraph1;
        private System451.Communication.Dashboard.Controls.DataGraph dataGraph2;
        private System451.Communication.Dashboard.Controls.AnalogMeter analogMeter1;
        private System451.Communication.Dashboard.Controls.AnalogMeter analogMeter2;
        private System451.Communication.Dashboard.Controls.AnalogMeter analogMeter3;
        private System451.Communication.Dashboard.Controls.DirectionMeter directionMeter1;
        private System451.Communication.Dashboard.Controls.DirectionMeter directionMeter2;
        private System451.Communication.Dashboard.Controls.TacoMeter tacoMeter1;
        private System451.Communication.Dashboard.Controls.SpikeControl spikeControl1;
        private System451.Communication.Dashboard.Controls.SpikeControl spikeControl2;
        private System451.Communication.Dashboard.Controls.OnOffControl onOffControl1;
        private System451.Communication.Dashboard.Controls.OnOffControl onOffControl2;
        private System451.Communication.Dashboard.Controls.RoundSpeedMeter roundSpeedMeter1;
        private System451.Communication.Dashboard.Controls.RoundSpeedMeter roundSpeedMeter2;
        private System.Windows.Forms.Button button1;
        private System451.Communication.Dashboard.Controls.CameraView cameraView1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label1;
        private System451.Communication.Dashboard.Controls.OnOffControl onOffControl3;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System451.Communication.Dashboard.ControlBoxMenuButton controlBoxMenuButton1;
        private System.Windows.Forms.ContextMenuStrip videoContextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem startBothToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System451.Communication.Dashboard.Controls.ValueMeter valueMeter1;
        private System.Windows.Forms.ToolStripMenuItem sourceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem robotToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem webcamToolStripMenuItem;
    }
}

