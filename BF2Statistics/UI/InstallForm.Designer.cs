namespace BF2Statistics
{
    partial class InstallForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ClientPath = new System.Windows.Forms.TextBox();
            this.ServerPath = new System.Windows.Forms.TextBox();
            this.ClientBtn = new System.Windows.Forms.Button();
            this.ServerBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.SaveBtn = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.DescLabel = new System.Windows.Forms.Label();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.lineShape1 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.shapeContainer2 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.lineShape2 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.UpdateCheckBox = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.label1.Location = new System.Drawing.Point(21, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(167, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Battlefield 2 Client Path (Optional):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.label2.Location = new System.Drawing.Point(21, 130);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(176, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Battlefield 2 Dedicated Server Path:";
            // 
            // ClientPath
            // 
            this.ClientPath.Location = new System.Drawing.Point(22, 96);
            this.ClientPath.Name = "ClientPath";
            this.ClientPath.ReadOnly = true;
            this.ClientPath.Size = new System.Drawing.Size(326, 20);
            this.ClientPath.TabIndex = 2;
            // 
            // ServerPath
            // 
            this.ServerPath.Location = new System.Drawing.Point(22, 156);
            this.ServerPath.Name = "ServerPath";
            this.ServerPath.ReadOnly = true;
            this.ServerPath.Size = new System.Drawing.Size(326, 20);
            this.ServerPath.TabIndex = 3;
            // 
            // ClientBtn
            // 
            this.ClientBtn.Location = new System.Drawing.Point(354, 93);
            this.ClientBtn.Name = "ClientBtn";
            this.ClientBtn.Size = new System.Drawing.Size(90, 25);
            this.ClientBtn.TabIndex = 4;
            this.ClientBtn.Text = "Select Path";
            this.ClientBtn.UseVisualStyleBackColor = true;
            this.ClientBtn.Click += new System.EventHandler(this.ClientBtn_Click);
            // 
            // ServerBtn
            // 
            this.ServerBtn.Location = new System.Drawing.Point(354, 153);
            this.ServerBtn.Name = "ServerBtn";
            this.ServerBtn.Size = new System.Drawing.Size(90, 25);
            this.ServerBtn.TabIndex = 5;
            this.ServerBtn.Text = "Select Path";
            this.ServerBtn.UseVisualStyleBackColor = true;
            this.ServerBtn.Click += new System.EventHandler(this.ServerBtn_Click);
            // 
            // CancelBtn
            // 
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Location = new System.Drawing.Point(129, 320);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(100, 29);
            this.CancelBtn.TabIndex = 6;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            // 
            // SaveBtn
            // 
            this.SaveBtn.Location = new System.Drawing.Point(235, 320);
            this.SaveBtn.Name = "SaveBtn";
            this.SaveBtn.Size = new System.Drawing.Size(100, 29);
            this.SaveBtn.TabIndex = 7;
            this.SaveBtn.Text = "Save";
            this.SaveBtn.UseVisualStyleBackColor = true;
            this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.Controls.Add(this.DescLabel);
            this.panel1.Controls.Add(this.TitleLabel);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.shapeContainer1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(464, 69);
            this.panel1.TabIndex = 9;
            // 
            // DescLabel
            // 
            this.DescLabel.AutoSize = true;
            this.DescLabel.Location = new System.Drawing.Point(34, 39);
            this.DescLabel.Name = "DescLabel";
            this.DescLabel.Size = new System.Drawing.Size(312, 13);
            this.DescLabel.TabIndex = 2;
            this.DescLabel.Text = "Installation path of your Battlefield 2 Dedicated server is required.";
            // 
            // TitleLabel
            // 
            this.TitleLabel.AutoSize = true;
            this.TitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TitleLabel.Location = new System.Drawing.Point(24, 21);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(160, 13);
            this.TitleLabel.TabIndex = 1;
            this.TitleLabel.Text = "BF2Statistics Configuration";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::BF2Statistics.Properties.Resources.Logo;
            this.pictureBox1.Location = new System.Drawing.Point(398, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.lineShape1});
            this.shapeContainer1.Size = new System.Drawing.Size(464, 69);
            this.shapeContainer1.TabIndex = 3;
            this.shapeContainer1.TabStop = false;
            // 
            // lineShape1
            // 
            this.lineShape1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.lineShape1.Name = "lineShape1";
            this.lineShape1.X1 = 0;
            this.lineShape1.X2 = 464;
            this.lineShape1.Y1 = 66;
            this.lineShape1.Y2 = 66;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Window;
            this.panel2.Controls.Add(this.UpdateCheckBox);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.ClientPath);
            this.panel2.Controls.Add(this.ServerPath);
            this.panel2.Controls.Add(this.ServerBtn);
            this.panel2.Controls.Add(this.ClientBtn);
            this.panel2.Controls.Add(this.shapeContainer2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 69);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(464, 235);
            this.panel2.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(408, 39);
            this.label3.TabIndex = 7;
            this.label3.Text = resources.GetString("label3.Text");
            // 
            // shapeContainer2
            // 
            this.shapeContainer2.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer2.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer2.Name = "shapeContainer2";
            this.shapeContainer2.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.lineShape2});
            this.shapeContainer2.Size = new System.Drawing.Size(464, 235);
            this.shapeContainer2.TabIndex = 6;
            this.shapeContainer2.TabStop = false;
            // 
            // lineShape2
            // 
            this.lineShape2.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.lineShape2.Name = "lineShape2";
            this.lineShape2.X1 = 0;
            this.lineShape2.X2 = 466;
            this.lineShape2.Y1 = 234;
            this.lineShape2.Y2 = 234;
            // 
            // UpdateCheckBox
            // 
            this.UpdateCheckBox.AutoSize = true;
            this.UpdateCheckBox.Checked = true;
            this.UpdateCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.UpdateCheckBox.Location = new System.Drawing.Point(119, 200);
            this.UpdateCheckBox.Name = "UpdateCheckBox";
            this.UpdateCheckBox.Size = new System.Drawing.Size(227, 17);
            this.UpdateCheckBox.TabIndex = 8;
            this.UpdateCheckBox.Text = "Enable Program Update Check On Startup";
            this.UpdateCheckBox.UseVisualStyleBackColor = true;
            // 
            // InstallForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(464, 362);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.SaveBtn);
            this.Controls.Add(this.CancelBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InstallForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BF2Statistics Setup";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ClientPath;
        private System.Windows.Forms.TextBox ServerPath;
        private System.Windows.Forms.Button ClientBtn;
        private System.Windows.Forms.Button ServerBtn;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Button SaveBtn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label DescLabel;
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape1;
        private System.Windows.Forms.Panel panel2;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer2;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox UpdateCheckBox;
    }
}