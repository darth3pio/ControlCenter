namespace BF2Statistics
{
    partial class ASPConfigForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ASPConfigForm));
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.IgnoreAi = new System.Windows.Forms.ComboBox();
            this.MinRoundTime = new System.Windows.Forms.NumericUpDown();
            this.MinRoundPlayers = new System.Windows.Forms.NumericUpDown();
            this.RankChecking = new System.Windows.Forms.ComboBox();
            this.RankTenure = new System.Windows.Forms.NumericUpDown();
            this.SmocProcessing = new System.Windows.Forms.ComboBox();
            this.GeneralProcessing = new System.Windows.Forms.ComboBox();
            this.AwdRoundComplete = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.AuthGameServers = new System.Windows.Forms.TextBox();
            this.CustMapId = new System.Windows.Forms.NumericUpDown();
            this.OfflinePid = new System.Windows.Forms.NumericUpDown();
            this.UnlocksOption = new System.Windows.Forms.ComboBox();
            this.DebugLvl = new System.Windows.Forms.ComboBox();
            this.SaveBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.Tipsy = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MinRoundTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinRoundPlayers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RankTenure)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CustMapId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.OfflinePid)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ignore AI Stats:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.AwdRoundComplete);
            this.groupBox1.Controls.Add(this.GeneralProcessing);
            this.groupBox1.Controls.Add(this.SmocProcessing);
            this.groupBox1.Controls.Add(this.RankTenure);
            this.groupBox1.Controls.Add(this.RankChecking);
            this.groupBox1.Controls.Add(this.MinRoundPlayers);
            this.groupBox1.Controls.Add(this.MinRoundTime);
            this.groupBox1.Controls.Add(this.IgnoreAi);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(350, 305);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Stats Processing";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(129, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Min. Round Time (Player):";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 107);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Min. Players:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 140);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Rank Checking:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 170);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(111, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Special Rank Tenure:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 202);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(92, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Smoc Processing:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 236);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(102, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "General Processing:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(11, 274);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(175, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "Awards Require Round Completion:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.DebugLvl);
            this.groupBox2.Controls.Add(this.UnlocksOption);
            this.groupBox2.Controls.Add(this.OfflinePid);
            this.groupBox2.Controls.Add(this.CustMapId);
            this.groupBox2.Controls.Add(this.AuthGameServers);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Location = new System.Drawing.Point(371, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(350, 305);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Global Settings";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(18, 37);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(209, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "Authorized Gameservers (One IP Per Line):";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(18, 168);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(78, 13);
            this.label11.TabIndex = 2;
            this.label11.Text = "Custom MapId:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(18, 202);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(86, 13);
            this.label12.TabIndex = 3;
            this.label12.Text = "Offline Player ID:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(18, 236);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(83, 13);
            this.label13.TabIndex = 4;
            this.label13.Text = "Unlocks Option:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(18, 271);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(104, 13);
            this.label15.TabIndex = 6;
            this.label15.Text = "Stats Logging Level:";
            // 
            // IgnoreAi
            // 
            this.IgnoreAi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.IgnoreAi.FormattingEnabled = true;
            this.IgnoreAi.Items.AddRange(new object[] {
            "False",
            "True"});
            this.IgnoreAi.Location = new System.Drawing.Point(160, 35);
            this.IgnoreAi.Name = "IgnoreAi";
            this.IgnoreAi.Size = new System.Drawing.Size(180, 21);
            this.IgnoreAi.TabIndex = 9;
            this.Tipsy.SetToolTip(this.IgnoreAi, "Save AI Bot stats at the end of the round?");
            // 
            // MinRoundTime
            // 
            this.MinRoundTime.Location = new System.Drawing.Point(160, 71);
            this.MinRoundTime.Maximum = new decimal(new int[] {
            86400,
            0,
            0,
            0});
            this.MinRoundTime.Name = "MinRoundTime";
            this.MinRoundTime.Size = new System.Drawing.Size(97, 20);
            this.MinRoundTime.TabIndex = 11;
            this.Tipsy.SetToolTip(this.MinRoundTime, "Minimum amount of time (in seconds) the player must play in the round for thier s" +
                    "tats to be saved.");
            this.MinRoundTime.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // MinRoundPlayers
            // 
            this.MinRoundPlayers.Location = new System.Drawing.Point(160, 105);
            this.MinRoundPlayers.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.MinRoundPlayers.Name = "MinRoundPlayers";
            this.MinRoundPlayers.Size = new System.Drawing.Size(97, 20);
            this.MinRoundPlayers.TabIndex = 12;
            this.Tipsy.SetToolTip(this.MinRoundPlayers, "Number of players (and Bots) needed to spawn in order for stats to be saved at th" +
                    "e end of the round.");
            this.MinRoundPlayers.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // RankChecking
            // 
            this.RankChecking.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RankChecking.FormattingEnabled = true;
            this.RankChecking.Items.AddRange(new object[] {
            "Disabled",
            "Enabled"});
            this.RankChecking.Location = new System.Drawing.Point(160, 137);
            this.RankChecking.Name = "RankChecking";
            this.RankChecking.Size = new System.Drawing.Size(180, 21);
            this.RankChecking.TabIndex = 13;
            this.Tipsy.SetToolTip(this.RankChecking, "Only if your having issues with rank being reset to 0 at the end of the round, en" +
                    "able this option.");
            // 
            // RankTenure
            // 
            this.RankTenure.Location = new System.Drawing.Point(160, 168);
            this.RankTenure.Maximum = new decimal(new int[] {
            365,
            0,
            0,
            0});
            this.RankTenure.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.RankTenure.Name = "RankTenure";
            this.RankTenure.Size = new System.Drawing.Size(97, 20);
            this.RankTenure.TabIndex = 14;
            this.RankTenure.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            // 
            // SmocProcessing
            // 
            this.SmocProcessing.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SmocProcessing.FormattingEnabled = true;
            this.SmocProcessing.Items.AddRange(new object[] {
            "Disabled",
            "Enabled"});
            this.SmocProcessing.Location = new System.Drawing.Point(160, 199);
            this.SmocProcessing.Name = "SmocProcessing";
            this.SmocProcessing.Size = new System.Drawing.Size(180, 21);
            this.SmocProcessing.TabIndex = 15;
            this.Tipsy.SetToolTip(this.SmocProcessing, "Automatcially assign new Sergeant Major of the Corp every <Special Rank Tenure> d" +
                    "ays?");
            // 
            // GeneralProcessing
            // 
            this.GeneralProcessing.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GeneralProcessing.FormattingEnabled = true;
            this.GeneralProcessing.Items.AddRange(new object[] {
            "Disabled",
            "Enabled"});
            this.GeneralProcessing.Location = new System.Drawing.Point(160, 233);
            this.GeneralProcessing.Name = "GeneralProcessing";
            this.GeneralProcessing.Size = new System.Drawing.Size(180, 21);
            this.GeneralProcessing.TabIndex = 16;
            this.Tipsy.SetToolTip(this.GeneralProcessing, "Automatcially assign new General every <Special Rank Tenure> days?");
            // 
            // AwdRoundComplete
            // 
            this.AwdRoundComplete.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AwdRoundComplete.FormattingEnabled = true;
            this.AwdRoundComplete.Items.AddRange(new object[] {
            "False",
            "True"});
            this.AwdRoundComplete.Location = new System.Drawing.Point(190, 268);
            this.AwdRoundComplete.Name = "AwdRoundComplete";
            this.AwdRoundComplete.Size = new System.Drawing.Size(150, 21);
            this.AwdRoundComplete.TabIndex = 17;
            this.Tipsy.SetToolTip(this.AwdRoundComplete, "Does the player need to complete the round for his or hers awards to be saved?");
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(263, 73);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(49, 13);
            this.label17.TabIndex = 19;
            this.label17.Text = "Seconds";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(263, 172);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(31, 13);
            this.label18.TabIndex = 20;
            this.label18.Text = "Days";
            // 
            // AuthGameServers
            // 
            this.AuthGameServers.Location = new System.Drawing.Point(21, 59);
            this.AuthGameServers.Multiline = true;
            this.AuthGameServers.Name = "AuthGameServers";
            this.AuthGameServers.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.AuthGameServers.Size = new System.Drawing.Size(311, 92);
            this.AuthGameServers.TabIndex = 7;
            this.Tipsy.SetToolTip(this.AuthGameServers, resources.GetString("AuthGameServers.ToolTip"));
            // 
            // CustMapId
            // 
            this.CustMapId.Location = new System.Drawing.Point(152, 164);
            this.CustMapId.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.CustMapId.Name = "CustMapId";
            this.CustMapId.Size = new System.Drawing.Size(97, 20);
            this.CustMapId.TabIndex = 13;
            this.Tipsy.SetToolTip(this.CustMapId, "Default MapID. All maps that do not have an ID number will increment off of this " +
                    "number.\r\nNot recomended to change");
            this.CustMapId.Value = new decimal(new int[] {
            700,
            0,
            0,
            0});
            // 
            // OfflinePid
            // 
            this.OfflinePid.Location = new System.Drawing.Point(152, 198);
            this.OfflinePid.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.OfflinePid.Minimum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.OfflinePid.Name = "OfflinePid";
            this.OfflinePid.Size = new System.Drawing.Size(97, 20);
            this.OfflinePid.TabIndex = 14;
            this.Tipsy.SetToolTip(this.OfflinePid, "Default Offline PlayerID. All offline accounts that do not have an ID number will" +
                    " decrement off of this number.\r\nNot recomended to change");
            this.OfflinePid.Value = new decimal(new int[] {
            29000000,
            0,
            0,
            0});
            // 
            // UnlocksOption
            // 
            this.UnlocksOption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.UnlocksOption.FormattingEnabled = true;
            this.UnlocksOption.Items.AddRange(new object[] {
            "Unlocks are Earned",
            "All Unlocks Unlocked for All Players",
            "All Unlocks Disabled"});
            this.UnlocksOption.Location = new System.Drawing.Point(152, 231);
            this.UnlocksOption.Name = "UnlocksOption";
            this.UnlocksOption.Size = new System.Drawing.Size(180, 21);
            this.UnlocksOption.TabIndex = 15;
            // 
            // DebugLvl
            // 
            this.DebugLvl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DebugLvl.FormattingEnabled = true;
            this.DebugLvl.Items.AddRange(new object[] {
            "(1) Errors and Security",
            "(2) Warnings, Errors, and Security",
            "(3) All Messages"});
            this.DebugLvl.Location = new System.Drawing.Point(152, 268);
            this.DebugLvl.Name = "DebugLvl";
            this.DebugLvl.Size = new System.Drawing.Size(180, 21);
            this.DebugLvl.TabIndex = 18;
            // 
            // SaveBtn
            // 
            this.SaveBtn.Location = new System.Drawing.Point(603, 331);
            this.SaveBtn.Name = "SaveBtn";
            this.SaveBtn.Size = new System.Drawing.Size(118, 29);
            this.SaveBtn.TabIndex = 3;
            this.SaveBtn.Text = "Save";
            this.SaveBtn.UseVisualStyleBackColor = true;
            this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
            // 
            // CancelBtn
            // 
            this.CancelBtn.Location = new System.Drawing.Point(481, 331);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(118, 29);
            this.CancelBtn.TabIndex = 4;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // Tipsy
            // 
            this.Tipsy.AutomaticDelay = 200;
            this.Tipsy.AutoPopDelay = 10000;
            this.Tipsy.InitialDelay = 200;
            this.Tipsy.ReshowDelay = 40;
            // 
            // ASPConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 372);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.SaveBtn);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ASPConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ASP Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MinRoundTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinRoundPlayers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RankTenure)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CustMapId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OfflinePid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox IgnoreAi;
        private System.Windows.Forms.NumericUpDown MinRoundPlayers;
        private System.Windows.Forms.NumericUpDown MinRoundTime;
        private System.Windows.Forms.ComboBox AwdRoundComplete;
        private System.Windows.Forms.ComboBox GeneralProcessing;
        private System.Windows.Forms.ComboBox SmocProcessing;
        private System.Windows.Forms.NumericUpDown RankTenure;
        private System.Windows.Forms.ComboBox RankChecking;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox AuthGameServers;
        private System.Windows.Forms.ComboBox DebugLvl;
        private System.Windows.Forms.ComboBox UnlocksOption;
        private System.Windows.Forms.NumericUpDown OfflinePid;
        private System.Windows.Forms.NumericUpDown CustMapId;
        private System.Windows.Forms.Button SaveBtn;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.ToolTip Tipsy;
    }
}