namespace BF2Statistics
{
    partial class ManageStatsDBForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManageStatsDBForm));
            this.ClearStatsBtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ImportASPBtn = new System.Windows.Forms.Button();
            this.ImportSqlBtn = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ExportAsASPBtn = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // ClearStatsBtn
            // 
            this.ClearStatsBtn.ForeColor = System.Drawing.Color.Firebrick;
            this.ClearStatsBtn.Location = new System.Drawing.Point(227, 21);
            this.ClearStatsBtn.Name = "ClearStatsBtn";
            this.ClearStatsBtn.Size = new System.Drawing.Size(150, 28);
            this.ClearStatsBtn.TabIndex = 29;
            this.ClearStatsBtn.Text = "Clear Stats Database";
            this.ClearStatsBtn.UseVisualStyleBackColor = true;
            this.ClearStatsBtn.Click += new System.EventHandler(this.ClearStatsBtn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.ImportASPBtn);
            this.groupBox1.Controls.Add(this.ImportSqlBtn);
            this.groupBox1.Location = new System.Drawing.Point(12, 23);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(606, 142);
            this.groupBox1.TabIndex = 30;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Import Stats From Backup";
            // 
            // ImportASPBtn
            // 
            this.ImportASPBtn.Location = new System.Drawing.Point(306, 95);
            this.ImportASPBtn.Name = "ImportASPBtn";
            this.ImportASPBtn.Size = new System.Drawing.Size(175, 28);
            this.ImportASPBtn.TabIndex = 5;
            this.ImportASPBtn.Text = "Import ASP Database Backup";
            this.ImportASPBtn.UseVisualStyleBackColor = true;
            this.ImportASPBtn.Click += new System.EventHandler(this.ImportASPBtn_Click);
            // 
            // ImportSqlBtn
            // 
            this.ImportSqlBtn.Enabled = false;
            this.ImportSqlBtn.Location = new System.Drawing.Point(125, 95);
            this.ImportSqlBtn.Name = "ImportSqlBtn";
            this.ImportSqlBtn.Size = new System.Drawing.Size(175, 28);
            this.ImportSqlBtn.TabIndex = 4;
            this.ImportSqlBtn.Text = "Import SQL File";
            this.ImportSqlBtn.UseVisualStyleBackColor = true;
            this.ImportSqlBtn.Click += new System.EventHandler(this.ImportSqlBtn_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.ExportAsASPBtn);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Location = new System.Drawing.Point(12, 176);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(605, 115);
            this.groupBox2.TabIndex = 31;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Create Stats Backup";
            // 
            // ExportAsASPBtn
            // 
            this.ExportAsASPBtn.Location = new System.Drawing.Point(305, 68);
            this.ExportAsASPBtn.Name = "ExportAsASPBtn";
            this.ExportAsASPBtn.Size = new System.Drawing.Size(175, 28);
            this.ExportAsASPBtn.TabIndex = 7;
            this.ExportAsASPBtn.Text = "Export as ASP Database Backup";
            this.ExportAsASPBtn.UseVisualStyleBackColor = true;
            this.ExportAsASPBtn.Click += new System.EventHandler(this.ExportAsASPBtn_Click);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(124, 68);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(175, 28);
            this.button2.TabIndex = 6;
            this.button2.Text = "Export as SQL Dump";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.ClearStatsBtn);
            this.groupBox3.Location = new System.Drawing.Point(12, 300);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(605, 65);
            this.groupBox3.TabIndex = 32;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Erase Stats";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 24);
            this.label1.MaximumSize = new System.Drawing.Size(545, 0);
            this.label1.MinimumSize = new System.Drawing.Size(545, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(545, 26);
            this.label1.TabIndex = 6;
            this.label1.Text = "If you wish to import stats from another database, you may do so here. Simply cre" +
                "ate an ASP backup using the Web ASP, or connect to your other database, using th" +
                "e ASP server, and create an ASP Backup";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Firebrick;
            this.label2.Location = new System.Drawing.Point(180, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(246, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "All current stats will be erased, so use with caution!";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 25);
            this.label3.MaximumSize = new System.Drawing.Size(545, 0);
            this.label3.MinimumSize = new System.Drawing.Size(545, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(545, 26);
            this.label3.TabIndex = 8;
            this.label3.Text = "If you wish to backup your stats, you may do so here. An ASP backup will be creat" +
                "ed under your \r\n\"My Documents/BF2Statstics/Backups\" folder.";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ManageStatsDBForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(630, 378);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ManageStatsDBForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Manage Stats Database ";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ClearStatsBtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button ImportSqlBtn;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button ImportASPBtn;
        private System.Windows.Forms.Button ExportAsASPBtn;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}