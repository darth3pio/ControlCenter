namespace BF2Statistics
{
    partial class BF2sConfig
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
            this.AspCallback = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.AspPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.AspAddress = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.CentralCallback = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.CentralPort = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.CentralAddress = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.CentralDatabase = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.ForceKeyString = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.MedalData = new System.Windows.Forms.ComboBox();
            this.SnapshotPrefix = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.Logging = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Debugging = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label21 = new System.Windows.Forms.Label();
            this.CmCountry = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.CmBanCount = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.CmKDRatio = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.CmMinRank = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.CmGlobalTime = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.CmGlobalScore = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.CmClanTag = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.CmServerMode = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.ClanManager = new System.Windows.Forms.ComboBox();
            this.SaveButton = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.AspCallback);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.AspPort);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.AspAddress);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 247);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(400, 163);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ASP Settings";
            // 
            // AspCallback
            // 
            this.AspCallback.Location = new System.Drawing.Point(183, 89);
            this.AspCallback.Name = "AspCallback";
            this.AspCallback.Size = new System.Drawing.Size(197, 20);
            this.AspCallback.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(38, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(139, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "BF2Statistics ASP Callback:";
            // 
            // AspPort
            // 
            this.AspPort.Location = new System.Drawing.Point(183, 61);
            this.AspPort.Name = "AspPort";
            this.AspPort.Size = new System.Drawing.Size(197, 20);
            this.AspPort.TabIndex = 5;
            this.AspPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AspPort_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(46, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "ASP Backend HTTP Port:";
            // 
            // AspAddress
            // 
            this.AspAddress.Location = new System.Drawing.Point(183, 30);
            this.AspAddress.Name = "AspAddress";
            this.AspAddress.Size = new System.Drawing.Size(197, 20);
            this.AspAddress.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(150, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "ASP Backend HTTP Address:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.CentralCallback);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.CentralPort);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.CentralAddress);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.CentralDatabase);
            this.groupBox2.Location = new System.Drawing.Point(418, 247);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(400, 163);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Central Database";
            // 
            // CentralCallback
            // 
            this.CentralCallback.Location = new System.Drawing.Point(185, 129);
            this.CentralCallback.Name = "CentralCallback";
            this.CentralCallback.Size = new System.Drawing.Size(197, 20);
            this.CentralCallback.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 129);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(175, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "BF2Statistics Central ASP Callback:";
            // 
            // CentralPort
            // 
            this.CentralPort.Location = new System.Drawing.Point(185, 98);
            this.CentralPort.Name = "CentralPort";
            this.CentralPort.Size = new System.Drawing.Size(197, 20);
            this.CentralPort.TabIndex = 5;
            this.CentralPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CentralPort_KeyPress);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(58, 101);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(121, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "ASP Central HTTP Port:";
            // 
            // CentralAddress
            // 
            this.CentralAddress.Location = new System.Drawing.Point(185, 67);
            this.CentralAddress.Name = "CentralAddress";
            this.CentralAddress.Size = new System.Drawing.Size(197, 20);
            this.CentralAddress.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(39, 70);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(140, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "ASP Central HTTP Address:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(87, 36);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(92, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Central Database:";
            // 
            // CentralDatabase
            // 
            this.CentralDatabase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CentralDatabase.FormattingEnabled = true;
            this.CentralDatabase.Items.AddRange(new object[] {
            "Disabled",
            "Sync",
            "Minimal"});
            this.CentralDatabase.Location = new System.Drawing.Point(185, 33);
            this.CentralDatabase.Name = "CentralDatabase";
            this.CentralDatabase.Size = new System.Drawing.Size(146, 21);
            this.CentralDatabase.TabIndex = 0;
            this.CentralDatabase.SelectedIndexChanged += new System.EventHandler(this.CentralDatabase_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.ForceKeyString);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.MedalData);
            this.groupBox3.Controls.Add(this.SnapshotPrefix);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.Logging);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.Debugging);
            this.groupBox3.Location = new System.Drawing.Point(12, 14);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(343, 223);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Basic Settings";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(26, 150);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(115, 13);
            this.label12.TabIndex = 11;
            this.label12.Text = "Force Medal Keystring:";
            // 
            // ForceKeyString
            // 
            this.ForceKeyString.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ForceKeyString.FormattingEnabled = true;
            this.ForceKeyString.Items.AddRange(new object[] {
            "Disabled",
            "Enabled"});
            this.ForceKeyString.Location = new System.Drawing.Point(147, 147);
            this.ForceKeyString.Name = "ForceKeyString";
            this.ForceKeyString.Size = new System.Drawing.Size(146, 21);
            this.ForceKeyString.TabIndex = 10;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(57, 120);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(84, 13);
            this.label11.TabIndex = 9;
            this.label11.Text = "Medal Data File:";
            // 
            // MedalData
            // 
            this.MedalData.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MedalData.FormattingEnabled = true;
            this.MedalData.Items.AddRange(new object[] {
            "Default"});
            this.MedalData.Location = new System.Drawing.Point(147, 117);
            this.MedalData.Name = "MedalData";
            this.MedalData.Size = new System.Drawing.Size(146, 21);
            this.MedalData.TabIndex = 8;
            // 
            // SnapshotPrefix
            // 
            this.SnapshotPrefix.Location = new System.Drawing.Point(147, 88);
            this.SnapshotPrefix.Name = "SnapshotPrefix";
            this.SnapshotPrefix.Size = new System.Drawing.Size(146, 20);
            this.SnapshotPrefix.TabIndex = 7;
            this.SnapshotPrefix.Validating += new System.ComponentModel.CancelEventHandler(this.SnapshotPrefix_Validating);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(57, 91);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(84, 13);
            this.label10.TabIndex = 6;
            this.label10.Text = "Snapshot Prefix:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(45, 62);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(96, 13);
            this.label9.TabIndex = 5;
            this.label9.Text = "Snapshot Logging:";
            // 
            // Logging
            // 
            this.Logging.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Logging.FormattingEnabled = true;
            this.Logging.Items.AddRange(new object[] {
            "Disabled",
            "Enabled",
            "Only on Error"});
            this.Logging.Location = new System.Drawing.Point(147, 59);
            this.Logging.Name = "Logging";
            this.Logging.Size = new System.Drawing.Size(146, 21);
            this.Logging.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(79, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Debugging:";
            // 
            // Debugging
            // 
            this.Debugging.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Debugging.FormattingEnabled = true;
            this.Debugging.Items.AddRange(new object[] {
            "Disabled",
            "Enabled"});
            this.Debugging.Location = new System.Drawing.Point(147, 29);
            this.Debugging.Name = "Debugging";
            this.Debugging.Size = new System.Drawing.Size(146, 21);
            this.Debugging.TabIndex = 2;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.groupBox5);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.CmServerMode);
            this.groupBox4.Controls.Add(this.label17);
            this.groupBox4.Controls.Add(this.ClanManager);
            this.groupBox4.Location = new System.Drawing.Point(361, 14);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(457, 223);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Clan Manager";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label21);
            this.groupBox5.Controls.Add(this.CmCountry);
            this.groupBox5.Controls.Add(this.label20);
            this.groupBox5.Controls.Add(this.CmBanCount);
            this.groupBox5.Controls.Add(this.label19);
            this.groupBox5.Controls.Add(this.CmKDRatio);
            this.groupBox5.Controls.Add(this.label18);
            this.groupBox5.Controls.Add(this.CmMinRank);
            this.groupBox5.Controls.Add(this.label16);
            this.groupBox5.Controls.Add(this.CmGlobalTime);
            this.groupBox5.Controls.Add(this.label14);
            this.groupBox5.Controls.Add(this.CmGlobalScore);
            this.groupBox5.Controls.Add(this.label15);
            this.groupBox5.Controls.Add(this.CmClanTag);
            this.groupBox5.Location = new System.Drawing.Point(17, 84);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(421, 129);
            this.groupBox5.TabIndex = 10;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Criteria";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(41, 77);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(46, 13);
            this.label21.TabIndex = 18;
            this.label21.Text = "Country:";
            // 
            // CmCountry
            // 
            this.CmCountry.Location = new System.Drawing.Point(93, 74);
            this.CmCountry.MaxLength = 2;
            this.CmCountry.Name = "CmCountry";
            this.CmCountry.Size = new System.Drawing.Size(91, 20);
            this.CmCountry.TabIndex = 19;
            this.CmCountry.Validating += new System.ComponentModel.CancelEventHandler(this.CmCountry_Validating);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(211, 77);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(83, 13);
            this.label20.TabIndex = 16;
            this.label20.Text = "Max Ban Count:";
            // 
            // CmBanCount
            // 
            this.CmBanCount.Location = new System.Drawing.Point(300, 74);
            this.CmBanCount.Name = "CmBanCount";
            this.CmBanCount.Size = new System.Drawing.Size(91, 20);
            this.CmBanCount.TabIndex = 17;
            this.CmBanCount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CmBanCount_KeyPress);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(236, 51);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(58, 13);
            this.label19.TabIndex = 14;
            this.label19.Text = "K/D Ratio:";
            // 
            // CmKDRatio
            // 
            this.CmKDRatio.Location = new System.Drawing.Point(300, 48);
            this.CmKDRatio.Name = "CmKDRatio";
            this.CmKDRatio.Size = new System.Drawing.Size(91, 20);
            this.CmKDRatio.TabIndex = 15;
            this.CmKDRatio.Validating += new System.ComponentModel.CancelEventHandler(this.CmKDRatio_Validating);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(72, 103);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(80, 13);
            this.label18.TabIndex = 13;
            this.label18.Text = "Minimum Rank:";
            // 
            // CmMinRank
            // 
            this.CmMinRank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmMinRank.FormattingEnabled = true;
            this.CmMinRank.Items.AddRange(new object[] {
            "None",
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
            "Sergeant Major Of The Corps",
            "2nd Lieutenant",
            "1st Lieutenant",
            "Captain",
            "Major",
            "Leiutenant Colonel",
            "Colonel",
            "Brigadier General",
            "Major General",
            "Lieutenant General",
            "General"});
            this.CmMinRank.Location = new System.Drawing.Point(158, 100);
            this.CmMinRank.Name = "CmMinRank";
            this.CmMinRank.Size = new System.Drawing.Size(184, 21);
            this.CmMinRank.TabIndex = 12;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(193, 28);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(101, 13);
            this.label16.TabIndex = 10;
            this.label16.Text = "Global Time Played:";
            // 
            // CmGlobalTime
            // 
            this.CmGlobalTime.Location = new System.Drawing.Point(300, 22);
            this.CmGlobalTime.Name = "CmGlobalTime";
            this.CmGlobalTime.Size = new System.Drawing.Size(91, 20);
            this.CmGlobalTime.TabIndex = 11;
            this.CmGlobalTime.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CmGlobalTime_KeyPress);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(16, 52);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(71, 13);
            this.label14.TabIndex = 8;
            this.label14.Text = "Global Score:";
            // 
            // CmGlobalScore
            // 
            this.CmGlobalScore.Location = new System.Drawing.Point(93, 49);
            this.CmGlobalScore.Name = "CmGlobalScore";
            this.CmGlobalScore.Size = new System.Drawing.Size(91, 20);
            this.CmGlobalScore.TabIndex = 9;
            this.CmGlobalScore.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CmGlobalScore_KeyPress);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(34, 25);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(53, 13);
            this.label15.TabIndex = 6;
            this.label15.Text = "Clan Tag:";
            // 
            // CmClanTag
            // 
            this.CmClanTag.Location = new System.Drawing.Point(93, 22);
            this.CmClanTag.Name = "CmClanTag";
            this.CmClanTag.Size = new System.Drawing.Size(91, 20);
            this.CmClanTag.TabIndex = 7;
            this.CmClanTag.Validating += new System.ComponentModel.CancelEventHandler(this.CmClanTag_Validating);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(134, 60);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(71, 13);
            this.label13.TabIndex = 9;
            this.label13.Text = "Server Mode:";
            // 
            // CmServerMode
            // 
            this.CmServerMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmServerMode.FormattingEnabled = true;
            this.CmServerMode.Items.AddRange(new object[] {
            "Public (Free For All)",
            "Clan Only",
            "Priority Proving Grounds",
            "Proving Grounds",
            "Experts Only"});
            this.CmServerMode.Location = new System.Drawing.Point(211, 57);
            this.CmServerMode.Name = "CmServerMode";
            this.CmServerMode.Size = new System.Drawing.Size(146, 21);
            this.CmServerMode.TabIndex = 8;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(93, 28);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(112, 13);
            this.label17.TabIndex = 3;
            this.label17.Text = "Enable Clan Manager:";
            // 
            // ClanManager
            // 
            this.ClanManager.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ClanManager.FormattingEnabled = true;
            this.ClanManager.Items.AddRange(new object[] {
            "Disabled",
            "Enabled"});
            this.ClanManager.Location = new System.Drawing.Point(211, 25);
            this.ClanManager.Name = "ClanManager";
            this.ClanManager.Size = new System.Drawing.Size(146, 21);
            this.ClanManager.TabIndex = 2;
            this.ClanManager.SelectedIndexChanged += new System.EventHandler(this.ClanManager_SelectedIndexChanged);
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(422, 421);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(106, 32);
            this.SaveButton.TabIndex = 4;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(306, 421);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(106, 32);
            this.Cancel.TabIndex = 5;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // BF2sConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(829, 462);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BF2sConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BF2Statistics Config";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox AspCallback;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox AspPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox AspAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox CentralPort;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox CentralAddress;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox CentralDatabase;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox SnapshotPrefix;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox Logging;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox Debugging;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox MedalData;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox ForceKeyString;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox CmServerMode;
        private System.Windows.Forms.TextBox CmClanTag;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ComboBox ClanManager;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.ComboBox CmMinRank;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox CmGlobalTime;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox CmGlobalScore;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox CmCountry;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox CmBanCount;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox CmKDRatio;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.TextBox CentralCallback;
    }
}