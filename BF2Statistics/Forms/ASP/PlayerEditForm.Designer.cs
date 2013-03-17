namespace BF2Statistics
{
    partial class PlayerEditForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.PlayerNick = new System.Windows.Forms.TextBox();
            this.PlayerId = new System.Windows.Forms.NumericUpDown();
            this.ClanTag = new System.Windows.Forms.TextBox();
            this.Rank = new System.Windows.Forms.ComboBox();
            this.PermBan = new System.Windows.Forms.ComboBox();
            this.ResetBtn = new System.Windows.Forms.Button();
            this.DeleteBtn = new System.Windows.Forms.Button();
            this.SaveBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.PlayerId)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Player ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Player Nick";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Clan Tag";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Rank";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 171);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Perm Ban";
            // 
            // PlayerNick
            // 
            this.PlayerNick.Location = new System.Drawing.Point(94, 59);
            this.PlayerNick.Name = "PlayerNick";
            this.PlayerNick.ReadOnly = true;
            this.PlayerNick.Size = new System.Drawing.Size(220, 20);
            this.PlayerNick.TabIndex = 5;
            // 
            // PlayerId
            // 
            this.PlayerId.Enabled = false;
            this.PlayerId.Location = new System.Drawing.Point(94, 24);
            this.PlayerId.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.PlayerId.Name = "PlayerId";
            this.PlayerId.Size = new System.Drawing.Size(119, 20);
            this.PlayerId.TabIndex = 6;
            // 
            // ClanTag
            // 
            this.ClanTag.Location = new System.Drawing.Point(94, 95);
            this.ClanTag.Name = "ClanTag";
            this.ClanTag.Size = new System.Drawing.Size(220, 20);
            this.ClanTag.TabIndex = 7;
            // 
            // Rank
            // 
            this.Rank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Rank.FormattingEnabled = true;
            this.Rank.Items.AddRange(new object[] {
            "Private",
            "Private First Class",
            "Lance Corporal",
            "Corporal",
            "Sergeant",
            "Staff Sergeant",
            "Gunnery Sergeant",
            "Master Sergeant",
            "First Sergeant",
            "Master Gunnery Sergeant",
            "Sergeant Major",
            "Sergeant Major of the Corps",
            "2nd Lieutenant",
            "1st Lieutenant",
            "Captain",
            "Major",
            "Lieutenant Colonol",
            "Colonol",
            "Brigadier General",
            "Major General",
            "Lieutenant General",
            "General"});
            this.Rank.Location = new System.Drawing.Point(93, 133);
            this.Rank.Name = "Rank";
            this.Rank.Size = new System.Drawing.Size(220, 21);
            this.Rank.TabIndex = 8;
            // 
            // PermBan
            // 
            this.PermBan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PermBan.FormattingEnabled = true;
            this.PermBan.Items.AddRange(new object[] {
            "No",
            "Yes"});
            this.PermBan.Location = new System.Drawing.Point(93, 168);
            this.PermBan.Name = "PermBan";
            this.PermBan.Size = new System.Drawing.Size(220, 21);
            this.PermBan.TabIndex = 9;
            // 
            // ResetBtn
            // 
            this.ResetBtn.Location = new System.Drawing.Point(22, 223);
            this.ResetBtn.Name = "ResetBtn";
            this.ResetBtn.Size = new System.Drawing.Size(93, 29);
            this.ResetBtn.TabIndex = 10;
            this.ResetBtn.Text = "Reset Unlocks";
            this.ResetBtn.UseVisualStyleBackColor = true;
            this.ResetBtn.Click += new System.EventHandler(this.ResetBtn_Click);
            // 
            // DeleteBtn
            // 
            this.DeleteBtn.Location = new System.Drawing.Point(121, 223);
            this.DeleteBtn.Name = "DeleteBtn";
            this.DeleteBtn.Size = new System.Drawing.Size(93, 29);
            this.DeleteBtn.TabIndex = 11;
            this.DeleteBtn.Text = "Delete Player";
            this.DeleteBtn.UseVisualStyleBackColor = true;
            this.DeleteBtn.Click += new System.EventHandler(this.DeleteBtn_Click);
            // 
            // SaveBtn
            // 
            this.SaveBtn.Location = new System.Drawing.Point(220, 223);
            this.SaveBtn.Name = "SaveBtn";
            this.SaveBtn.Size = new System.Drawing.Size(93, 29);
            this.SaveBtn.TabIndex = 12;
            this.SaveBtn.Text = "Save And Close";
            this.SaveBtn.UseVisualStyleBackColor = true;
            this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
            // 
            // PlayerEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 272);
            this.Controls.Add(this.SaveBtn);
            this.Controls.Add(this.DeleteBtn);
            this.Controls.Add(this.ResetBtn);
            this.Controls.Add(this.PermBan);
            this.Controls.Add(this.Rank);
            this.Controls.Add(this.ClanTag);
            this.Controls.Add(this.PlayerId);
            this.Controls.Add(this.PlayerNick);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PlayerEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Player Name";
            ((System.ComponentModel.ISupportInitialize)(this.PlayerId)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox PlayerNick;
        private System.Windows.Forms.NumericUpDown PlayerId;
        private System.Windows.Forms.TextBox ClanTag;
        private System.Windows.Forms.ComboBox Rank;
        private System.Windows.Forms.ComboBox PermBan;
        private System.Windows.Forms.Button ResetBtn;
        private System.Windows.Forms.Button DeleteBtn;
        private System.Windows.Forms.Button SaveBtn;
    }
}