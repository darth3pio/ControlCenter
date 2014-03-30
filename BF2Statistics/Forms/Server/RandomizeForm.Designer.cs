namespace BF2Statistics
{
    partial class RandomizeForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ConquestBox = new System.Windows.Forms.CheckBox();
            this.CoopBox = new System.Windows.Forms.CheckBox();
            this.s16Box = new System.Windows.Forms.CheckBox();
            this.s32Box = new System.Windows.Forms.CheckBox();
            this.s64Box = new System.Windows.Forms.CheckBox();
            this.NumMaps = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.GenerateBtn = new System.Windows.Forms.Button();
            this.MapListBox = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.SaveBtn = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaps)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.NumMaps);
            this.groupBox1.Controls.Add(this.s64Box);
            this.groupBox1.Controls.Add(this.s32Box);
            this.groupBox1.Controls.Add(this.s16Box);
            this.groupBox1.Controls.Add(this.CoopBox);
            this.groupBox1.Controls.Add(this.ConquestBox);
            this.groupBox1.Location = new System.Drawing.Point(13, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(240, 210);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Randomize Criteria";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.SaveBtn);
            this.groupBox2.Controls.Add(this.GenerateBtn);
            this.groupBox2.Controls.Add(this.CancelBtn);
            this.groupBox2.Location = new System.Drawing.Point(13, 228);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(505, 56);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // ConquestBox
            // 
            this.ConquestBox.AutoSize = true;
            this.ConquestBox.Location = new System.Drawing.Point(37, 127);
            this.ConquestBox.Name = "ConquestBox";
            this.ConquestBox.Size = new System.Drawing.Size(71, 17);
            this.ConquestBox.TabIndex = 0;
            this.ConquestBox.Text = "Conquest";
            this.ConquestBox.UseVisualStyleBackColor = true;
            // 
            // CoopBox
            // 
            this.CoopBox.AutoSize = true;
            this.CoopBox.Location = new System.Drawing.Point(37, 150);
            this.CoopBox.Name = "CoopBox";
            this.CoopBox.Size = new System.Drawing.Size(56, 17);
            this.CoopBox.TabIndex = 1;
            this.CoopBox.Text = "COOP";
            this.CoopBox.UseVisualStyleBackColor = true;
            // 
            // s16Box
            // 
            this.s16Box.AutoSize = true;
            this.s16Box.Location = new System.Drawing.Point(153, 127);
            this.s16Box.Name = "s16Box";
            this.s16Box.Size = new System.Drawing.Size(38, 17);
            this.s16Box.TabIndex = 2;
            this.s16Box.Text = "16";
            this.s16Box.UseVisualStyleBackColor = true;
            // 
            // s32Box
            // 
            this.s32Box.AutoSize = true;
            this.s32Box.Location = new System.Drawing.Point(153, 150);
            this.s32Box.Name = "s32Box";
            this.s32Box.Size = new System.Drawing.Size(38, 17);
            this.s32Box.TabIndex = 3;
            this.s32Box.Text = "32";
            this.s32Box.UseVisualStyleBackColor = true;
            // 
            // s64Box
            // 
            this.s64Box.AutoSize = true;
            this.s64Box.Location = new System.Drawing.Point(153, 173);
            this.s64Box.Name = "s64Box";
            this.s64Box.Size = new System.Drawing.Size(38, 17);
            this.s64Box.TabIndex = 4;
            this.s64Box.Text = "64";
            this.s64Box.UseVisualStyleBackColor = true;
            // 
            // NumMaps
            // 
            this.NumMaps.Location = new System.Drawing.Point(133, 50);
            this.NumMaps.Name = "NumMaps";
            this.NumMaps.Size = new System.Drawing.Size(68, 20);
            this.NumMaps.TabIndex = 5;
            this.NumMaps.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Number of Maps:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Game Modes:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(150, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Map Sizes";
            // 
            // CancelBtn
            // 
            this.CancelBtn.Location = new System.Drawing.Point(306, 19);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 23);
            this.CancelBtn.TabIndex = 0;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // GenerateBtn
            // 
            this.GenerateBtn.Location = new System.Drawing.Point(80, 19);
            this.GenerateBtn.Name = "GenerateBtn";
            this.GenerateBtn.Size = new System.Drawing.Size(75, 23);
            this.GenerateBtn.TabIndex = 1;
            this.GenerateBtn.Text = "Generate";
            this.GenerateBtn.UseVisualStyleBackColor = true;
            this.GenerateBtn.Click += new System.EventHandler(this.GenerateBtn_Click);
            // 
            // MapListBox
            // 
            this.MapListBox.Location = new System.Drawing.Point(10, 21);
            this.MapListBox.Multiline = true;
            this.MapListBox.Name = "MapListBox";
            this.MapListBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.MapListBox.Size = new System.Drawing.Size(230, 180);
            this.MapListBox.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.SystemColors.Window;
            this.groupBox3.Controls.Add(this.MapListBox);
            this.groupBox3.Location = new System.Drawing.Point(268, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(250, 210);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "MapList Preview";
            // 
            // SaveBtn
            // 
            this.SaveBtn.Location = new System.Drawing.Point(387, 19);
            this.SaveBtn.Name = "SaveBtn";
            this.SaveBtn.Size = new System.Drawing.Size(75, 23);
            this.SaveBtn.TabIndex = 2;
            this.SaveBtn.Text = "Save";
            this.SaveBtn.UseVisualStyleBackColor = true;
            this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
            // 
            // RandomizeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(531, 304);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RandomizeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Random Maplist Generator";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NumMaps)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox CoopBox;
        private System.Windows.Forms.CheckBox ConquestBox;
        private System.Windows.Forms.NumericUpDown NumMaps;
        private System.Windows.Forms.CheckBox s64Box;
        private System.Windows.Forms.CheckBox s32Box;
        private System.Windows.Forms.CheckBox s16Box;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button GenerateBtn;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.TextBox MapListBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button SaveBtn;
    }
}