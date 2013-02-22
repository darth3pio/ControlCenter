namespace BF2Statistics
{
    partial class GamespyConfig
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
            this.TypeSelect = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Username = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Hostname = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Password = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.DBName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.TestBtn = new System.Windows.Forms.Button();
            this.SaveBtn = new System.Windows.Forms.Button();
            this.Port = new System.Windows.Forms.NumericUpDown();
            this.Debug = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.Port)).BeginInit();
            this.SuspendLayout();
            // 
            // TypeSelect
            // 
            this.TypeSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TypeSelect.FormattingEnabled = true;
            this.TypeSelect.Items.AddRange(new object[] {
            "SQLite",
            "MySQL"});
            this.TypeSelect.Location = new System.Drawing.Point(139, 29);
            this.TypeSelect.Name = "TypeSelect";
            this.TypeSelect.Size = new System.Drawing.Size(166, 21);
            this.TypeSelect.TabIndex = 17;
            this.TypeSelect.SelectedIndexChanged += new System.EventHandler(this.TypeSelect_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(34, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Database Type:";
            // 
            // Username
            // 
            this.Username.Location = new System.Drawing.Point(139, 123);
            this.Username.Name = "Username";
            this.Username.Size = new System.Drawing.Size(166, 20);
            this.Username.TabIndex = 15;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(34, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Database User:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Database Port:";
            // 
            // Hostname
            // 
            this.Hostname.Location = new System.Drawing.Point(139, 62);
            this.Hostname.Name = "Hostname";
            this.Hostname.Size = new System.Drawing.Size(166, 20);
            this.Hostname.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Database Host:";
            // 
            // Password
            // 
            this.Password.Location = new System.Drawing.Point(139, 152);
            this.Password.Name = "Password";
            this.Password.Size = new System.Drawing.Size(166, 20);
            this.Password.TabIndex = 19;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(34, 155);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Database Pass:";
            // 
            // DBName
            // 
            this.DBName.Location = new System.Drawing.Point(139, 182);
            this.DBName.Name = "DBName";
            this.DBName.Size = new System.Drawing.Size(166, 20);
            this.DBName.TabIndex = 21;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(34, 185);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(87, 13);
            this.label6.TabIndex = 20;
            this.label6.Text = "Database Name:";
            // 
            // TestBtn
            // 
            this.TestBtn.Location = new System.Drawing.Point(66, 249);
            this.TestBtn.Name = "TestBtn";
            this.TestBtn.Size = new System.Drawing.Size(100, 27);
            this.TestBtn.TabIndex = 22;
            this.TestBtn.Text = "Test Connection";
            this.TestBtn.UseVisualStyleBackColor = true;
            this.TestBtn.Click += new System.EventHandler(this.TestBtn_Click);
            // 
            // SaveBtn
            // 
            this.SaveBtn.Location = new System.Drawing.Point(172, 249);
            this.SaveBtn.Name = "SaveBtn";
            this.SaveBtn.Size = new System.Drawing.Size(100, 27);
            this.SaveBtn.TabIndex = 23;
            this.SaveBtn.Text = "Save";
            this.SaveBtn.UseVisualStyleBackColor = true;
            this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
            // 
            // Port
            // 
            this.Port.Location = new System.Drawing.Point(139, 93);
            this.Port.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.Port.Name = "Port";
            this.Port.Size = new System.Drawing.Size(120, 20);
            this.Port.TabIndex = 24;
            this.Port.Value = new decimal(new int[] {
            3306,
            0,
            0,
            0});
            // 
            // Debug
            // 
            this.Debug.AutoSize = true;
            this.Debug.Location = new System.Drawing.Point(39, 217);
            this.Debug.Name = "Debug";
            this.Debug.Size = new System.Drawing.Size(261, 17);
            this.Debug.TabIndex = 25;
            this.Debug.Text = "Log GPSP / GPCM Network Stream Transactions";
            this.Debug.UseVisualStyleBackColor = true;
            // 
            // GamespyConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(339, 288);
            this.Controls.Add(this.Debug);
            this.Controls.Add(this.Port);
            this.Controls.Add(this.SaveBtn);
            this.Controls.Add(this.TestBtn);
            this.Controls.Add(this.DBName);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.Password);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.TypeSelect);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Username);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Hostname);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GamespyConfig";
            this.Text = "GamespyConfig";
            ((System.ComponentModel.ISupportInitialize)(this.Port)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox TypeSelect;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox Username;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Hostname;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Password;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox DBName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button TestBtn;
        private System.Windows.Forms.Button SaveBtn;
        private System.Windows.Forms.NumericUpDown Port;
        private System.Windows.Forms.CheckBox Debug;
    }
}