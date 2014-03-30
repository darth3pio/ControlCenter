namespace BF2Statistics
{
    partial class MapList
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
            this.AddToMapList = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.MapSizeSelect = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.GameModeSelect = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.MapListSelect = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.MapListBox = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.RandomizeBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.SaveButton = new System.Windows.Forms.Button();
            this.ClearButton = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.MapPictureBox = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MapPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Window;
            this.groupBox1.Controls.Add(this.AddToMapList);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.MapSizeSelect);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.GameModeSelect);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.MapListSelect);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(360, 109);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Map Select";
            // 
            // AddToMapList
            // 
            this.AddToMapList.Enabled = false;
            this.AddToMapList.Location = new System.Drawing.Point(84, 77);
            this.AddToMapList.Name = "AddToMapList";
            this.AddToMapList.Size = new System.Drawing.Size(197, 24);
            this.AddToMapList.TabIndex = 6;
            this.AddToMapList.Text = "Add Map To List";
            this.AddToMapList.UseVisualStyleBackColor = true;
            this.AddToMapList.Click += new System.EventHandler(this.AddToMapList_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(195, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Maps Size:";
            // 
            // MapSizeSelect
            // 
            this.MapSizeSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MapSizeSelect.Enabled = false;
            this.MapSizeSelect.FormattingEnabled = true;
            this.MapSizeSelect.Location = new System.Drawing.Point(257, 46);
            this.MapSizeSelect.Name = "MapSizeSelect";
            this.MapSizeSelect.Size = new System.Drawing.Size(84, 21);
            this.MapSizeSelect.TabIndex = 4;
            this.MapSizeSelect.SelectedIndexChanged += new System.EventHandler(this.MapSizeSelect_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Gamemode:";
            // 
            // GameModeSelect
            // 
            this.GameModeSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GameModeSelect.Enabled = false;
            this.GameModeSelect.FormattingEnabled = true;
            this.GameModeSelect.Location = new System.Drawing.Point(76, 46);
            this.GameModeSelect.Name = "GameModeSelect";
            this.GameModeSelect.Size = new System.Drawing.Size(105, 21);
            this.GameModeSelect.TabIndex = 2;
            this.GameModeSelect.SelectedIndexChanged += new System.EventHandler(this.GameModeSelect_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(84, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Maps:";
            // 
            // MapListSelect
            // 
            this.MapListSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MapListSelect.FormattingEnabled = true;
            this.MapListSelect.Location = new System.Drawing.Point(123, 19);
            this.MapListSelect.Name = "MapListSelect";
            this.MapListSelect.Size = new System.Drawing.Size(159, 21);
            this.MapListSelect.TabIndex = 0;
            this.MapListSelect.SelectedIndexChanged += new System.EventHandler(this.MapListSelect_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.Window;
            this.groupBox2.Controls.Add(this.MapListBox);
            this.groupBox2.Location = new System.Drawing.Point(15, 129);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(356, 198);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "MapList";
            // 
            // MapListBox
            // 
            this.MapListBox.Location = new System.Drawing.Point(13, 21);
            this.MapListBox.Multiline = true;
            this.MapListBox.Name = "MapListBox";
            this.MapListBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.MapListBox.Size = new System.Drawing.Size(330, 166);
            this.MapListBox.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.RandomizeBtn);
            this.groupBox3.Controls.Add(this.CancelBtn);
            this.groupBox3.Controls.Add(this.SaveButton);
            this.groupBox3.Controls.Add(this.ClearButton);
            this.groupBox3.Location = new System.Drawing.Point(17, 332);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(642, 50);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            // 
            // RandomizeBtn
            // 
            this.RandomizeBtn.Location = new System.Drawing.Point(182, 14);
            this.RandomizeBtn.Name = "RandomizeBtn";
            this.RandomizeBtn.Size = new System.Drawing.Size(135, 26);
            this.RandomizeBtn.TabIndex = 5;
            this.RandomizeBtn.Text = "Shuffle Map Order";
            this.RandomizeBtn.UseVisualStyleBackColor = true;
            this.RandomizeBtn.Click += new System.EventHandler(this.RandomizeBtn_Click);
            // 
            // CancelBtn
            // 
            this.CancelBtn.Location = new System.Drawing.Point(325, 14);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(135, 26);
            this.CancelBtn.TabIndex = 4;
            this.CancelBtn.Text = "Cancel Changes";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(466, 14);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(135, 26);
            this.SaveButton.TabIndex = 1;
            this.SaveButton.Text = "Save and Close";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // ClearButton
            // 
            this.ClearButton.Location = new System.Drawing.Point(41, 14);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(135, 26);
            this.ClearButton.TabIndex = 0;
            this.ClearButton.Text = "Clear Maplist";
            this.ClearButton.UseVisualStyleBackColor = true;
            this.ClearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.MapPictureBox);
            this.groupBox4.Location = new System.Drawing.Point(384, 13);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(275, 314);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Map Image";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(27, 288);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(220, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Double click map image to see full size image";
            // 
            // MapPictureBox
            // 
            this.MapPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MapPictureBox.Location = new System.Drawing.Point(12, 23);
            this.MapPictureBox.Name = "MapPictureBox";
            this.MapPictureBox.Size = new System.Drawing.Size(250, 250);
            this.MapPictureBox.TabIndex = 0;
            this.MapPictureBox.TabStop = false;
            this.MapPictureBox.DoubleClick += new System.EventHandler(this.MapPictureBox_DoubleClick);
            // 
            // MapList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(674, 392);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MapList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Maplist Editor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MapPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button AddToMapList;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox MapSizeSelect;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox GameModeSelect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox MapListSelect;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox MapListBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button ClearButton;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox MapPictureBox;
        private System.Windows.Forms.Button RandomizeBtn;
        private System.Windows.Forms.Button CancelBtn;
    }
}