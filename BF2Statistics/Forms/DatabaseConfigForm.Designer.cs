namespace BF2Statistics
{
    partial class DatabaseConfigForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.DescLabel = new System.Windows.Forms.Label();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.lineShape1 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ShowHideBtn = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.Port = new System.Windows.Forms.NumericUpDown();
            this.DBName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.Password = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.TypeSelect = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Username = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Hostname = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.shapeContainer2 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.lineShape2 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.SaveBtn = new System.Windows.Forms.Button();
            this.TestBtn = new System.Windows.Forms.Button();
            this.NextBtn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Port)).BeginInit();
            this.SuspendLayout();
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
            this.panel1.Size = new System.Drawing.Size(468, 69);
            this.panel1.TabIndex = 0;
            // 
            // DescLabel
            // 
            this.DescLabel.AutoSize = true;
            this.DescLabel.Location = new System.Drawing.Point(34, 39);
            this.DescLabel.Name = "DescLabel";
            this.DescLabel.Size = new System.Drawing.Size(227, 13);
            this.DescLabel.TabIndex = 2;
            this.DescLabel.Text = "Which database should statistics be saved to?";
            // 
            // TitleLabel
            // 
            this.TitleLabel.AutoSize = true;
            this.TitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TitleLabel.Location = new System.Drawing.Point(24, 21);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(173, 13);
            this.TitleLabel.TabIndex = 1;
            this.TitleLabel.Text = "Stats Database Configuration";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::BF2Statistics.Properties.Resources.dedicated_server;
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
            this.shapeContainer1.Size = new System.Drawing.Size(468, 69);
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
            this.panel2.Controls.Add(this.ShowHideBtn);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.Port);
            this.panel2.Controls.Add(this.DBName);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.Password);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.TypeSelect);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.Username);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.Hostname);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.shapeContainer2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 69);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(468, 235);
            this.panel2.TabIndex = 1;
            // 
            // ShowHideBtn
            // 
            this.ShowHideBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ShowHideBtn.Image = global::BF2Statistics.Properties.Resources.Password_dots;
            this.ShowHideBtn.Location = new System.Drawing.Point(405, 160);
            this.ShowHideBtn.Name = "ShowHideBtn";
            this.ShowHideBtn.Size = new System.Drawing.Size(32, 21);
            this.ShowHideBtn.TabIndex = 49;
            this.ShowHideBtn.UseVisualStyleBackColor = true;
            this.ShowHideBtn.Click += new System.EventHandler(this.ShowHideBtn_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(45, 16);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(378, 13);
            this.label9.TabIndex = 48;
            this.label9.Text = "Bf2Statistics requires a database, for which the given user has full admin rights" +
                ".";
            // 
            // Port
            // 
            this.Port.Location = new System.Drawing.Point(137, 103);
            this.Port.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.Port.Name = "Port";
            this.Port.Size = new System.Drawing.Size(120, 20);
            this.Port.TabIndex = 38;
            this.Port.Value = new decimal(new int[] {
            3306,
            0,
            0,
            0});
            // 
            // DBName
            // 
            this.DBName.Location = new System.Drawing.Point(137, 191);
            this.DBName.Name = "DBName";
            this.DBName.Size = new System.Drawing.Size(300, 20);
            this.DBName.TabIndex = 41;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.SystemColors.Window;
            this.label6.Location = new System.Drawing.Point(32, 194);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(87, 13);
            this.label6.TabIndex = 47;
            this.label6.Text = "Database Name:";
            // 
            // Password
            // 
            this.Password.Location = new System.Drawing.Point(137, 161);
            this.Password.Name = "Password";
            this.Password.Size = new System.Drawing.Size(262, 20);
            this.Password.TabIndex = 40;
            this.Password.UseSystemPasswordChar = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.SystemColors.Window;
            this.label5.Location = new System.Drawing.Point(32, 164);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 13);
            this.label5.TabIndex = 46;
            this.label5.Text = "Database Pass:";
            // 
            // TypeSelect
            // 
            this.TypeSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TypeSelect.FormattingEnabled = true;
            this.TypeSelect.Items.AddRange(new object[] {
            "SQLite",
            "MySQL"});
            this.TypeSelect.Location = new System.Drawing.Point(137, 42);
            this.TypeSelect.Name = "TypeSelect";
            this.TypeSelect.Size = new System.Drawing.Size(300, 21);
            this.TypeSelect.TabIndex = 36;
            this.TypeSelect.SelectedIndexChanged += new System.EventHandler(this.TypeSelect_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.SystemColors.Window;
            this.label4.Location = new System.Drawing.Point(32, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 13);
            this.label4.TabIndex = 45;
            this.label4.Text = "Database Type:";
            // 
            // Username
            // 
            this.Username.Location = new System.Drawing.Point(137, 132);
            this.Username.Name = "Username";
            this.Username.Size = new System.Drawing.Size(300, 20);
            this.Username.TabIndex = 39;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.Window;
            this.label3.Location = new System.Drawing.Point(32, 135);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 44;
            this.label3.Text = "Database User:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.Window;
            this.label2.Location = new System.Drawing.Point(32, 105);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 43;
            this.label2.Text = "Database Port:";
            // 
            // Hostname
            // 
            this.Hostname.Location = new System.Drawing.Point(137, 73);
            this.Hostname.Name = "Hostname";
            this.Hostname.Size = new System.Drawing.Size(300, 20);
            this.Hostname.TabIndex = 37;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Window;
            this.label1.Location = new System.Drawing.Point(32, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 42;
            this.label1.Text = "Database Host:";
            // 
            // shapeContainer2
            // 
            this.shapeContainer2.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer2.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer2.Name = "shapeContainer2";
            this.shapeContainer2.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.lineShape2});
            this.shapeContainer2.Size = new System.Drawing.Size(468, 235);
            this.shapeContainer2.TabIndex = 50;
            this.shapeContainer2.TabStop = false;
            // 
            // lineShape2
            // 
            this.lineShape2.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.lineShape2.Name = "lineShape2";
            this.lineShape2.X1 = 0;
            this.lineShape2.X2 = 468;
            this.lineShape2.Y1 = 234;
            this.lineShape2.Y2 = 234;
            // 
            // CancelBtn
            // 
            this.CancelBtn.Location = new System.Drawing.Point(131, 317);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(100, 29);
            this.CancelBtn.TabIndex = 6;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // SaveBtn
            // 
            this.SaveBtn.Location = new System.Drawing.Point(237, 317);
            this.SaveBtn.Name = "SaveBtn";
            this.SaveBtn.Size = new System.Drawing.Size(100, 29);
            this.SaveBtn.TabIndex = 5;
            this.SaveBtn.Text = "Save";
            this.SaveBtn.UseVisualStyleBackColor = true;
            this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
            // 
            // TestBtn
            // 
            this.TestBtn.Location = new System.Drawing.Point(25, 317);
            this.TestBtn.Name = "TestBtn";
            this.TestBtn.Size = new System.Drawing.Size(100, 29);
            this.TestBtn.TabIndex = 8;
            this.TestBtn.Text = "Test Connection";
            this.TestBtn.UseVisualStyleBackColor = true;
            this.TestBtn.Click += new System.EventHandler(this.TestBtn_Click);
            // 
            // NextBtn
            // 
            this.NextBtn.Location = new System.Drawing.Point(343, 317);
            this.NextBtn.Name = "NextBtn";
            this.NextBtn.Size = new System.Drawing.Size(100, 29);
            this.NextBtn.TabIndex = 9;
            this.NextBtn.Text = "Next >";
            this.NextBtn.UseVisualStyleBackColor = true;
            this.NextBtn.Click += new System.EventHandler(this.NextBtn_Click);
            // 
            // DatabaseConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(468, 360);
            this.Controls.Add(this.NextBtn);
            this.Controls.Add(this.TestBtn);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.SaveBtn);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DatabaseConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Database Configuration & Setup";
            this.TopMost = true;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Port)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Button SaveBtn;
        private System.Windows.Forms.NumericUpDown Port;
        private System.Windows.Forms.TextBox DBName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox Password;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox TypeSelect;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox Username;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Hostname;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button TestBtn;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label DescLabel;
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button ShowHideBtn;
        private System.Windows.Forms.Button NextBtn;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape1;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer2;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape2;
    }
}