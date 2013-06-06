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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BF2sConfig));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.AspCallback = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.AspAddress = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.CentralCallback = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
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
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.CmMinRank = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.CmClanTag = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.CmServerMode = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.ClanManager = new System.Windows.Forms.ComboBox();
            this.SaveButton = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.AspPort = new System.Windows.Forms.NumericUpDown();
            this.CentralPort = new System.Windows.Forms.NumericUpDown();
            this.CmGlobalScore = new System.Windows.Forms.NumericUpDown();
            this.CmGlobalTime = new System.Windows.Forms.NumericUpDown();
            this.CmBanCount = new System.Windows.Forms.NumericUpDown();
            this.Tipsy = new System.Windows.Forms.ToolTip(this.components);
            this.CmKDRatio = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AspPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CentralPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CmGlobalScore)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CmGlobalTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CmBanCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CmKDRatio)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.AspPort);
            this.groupBox1.Controls.Add(this.AspCallback);
            this.groupBox1.Controls.Add(this.label4);
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
            this.Tipsy.SetToolTip(this.AspCallback, "The relative path to the bf2statistics.php file on the ASP backend address\r\n\r\nNot" +
                    " Recomended To Change!");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(38, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(139, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "BF2Statistics ASP Callback:";
            this.Tipsy.SetToolTip(this.label4, "The relative path to the bf2statistics.php file on the ASP backend address\r\n\r\nNot" +
                    " Recomended To Change!");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(46, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "ASP Backend HTTP Port:";
            this.Tipsy.SetToolTip(this.label3, "Asp backend server port (default: 80)");
            // 
            // AspAddress
            // 
            this.AspAddress.Location = new System.Drawing.Point(183, 30);
            this.AspAddress.Name = "AspAddress";
            this.AspAddress.Size = new System.Drawing.Size(197, 20);
            this.AspAddress.TabIndex = 3;
            this.Tipsy.SetToolTip(this.AspAddress, "Asp Backend server address");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(150, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "ASP Backend HTTP Address:";
            this.Tipsy.SetToolTip(this.label2, "Asp Backend server address");
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.CentralPort);
            this.groupBox2.Controls.Add(this.CentralCallback);
            this.groupBox2.Controls.Add(this.label5);
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
            this.Tipsy.SetToolTip(this.CentralCallback, "The relative path to the bf2statistics.php file on the ASP Central backend addres" +
                    "s\r\n\r\nNot Recomended To Change!");
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 129);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(175, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "BF2Statistics Central ASP Callback:";
            this.Tipsy.SetToolTip(this.label5, "The relative path to the bf2statistics.php file on the ASP Central backend addres" +
                    "s");
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(58, 101);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(121, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "ASP Central HTTP Port:";
            this.Tipsy.SetToolTip(this.label6, "Asp Central backend server port (default: 80)");
            // 
            // CentralAddress
            // 
            this.CentralAddress.Location = new System.Drawing.Point(185, 67);
            this.CentralAddress.Name = "CentralAddress";
            this.CentralAddress.Size = new System.Drawing.Size(197, 20);
            this.CentralAddress.TabIndex = 3;
            this.Tipsy.SetToolTip(this.CentralAddress, "Asp Central Backend server address");
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(39, 70);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(140, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "ASP Central HTTP Address:";
            this.Tipsy.SetToolTip(this.label7, "Asp Central Backend server address");
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(87, 36);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(92, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Central Database:";
            this.Tipsy.SetToolTip(this.label8, resources.GetString("label8.ToolTip"));
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
            this.Tipsy.SetToolTip(this.CentralDatabase, resources.GetString("CentralDatabase.ToolTip"));
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
            this.Tipsy.SetToolTip(this.label12, "Removing medal requirements can mess up the keystring, Use this to force the corr" +
                    "ect string\r\n\r\nRecommended to br enabled if some medal requirements are removed.");
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
            this.Tipsy.SetToolTip(this.ForceKeyString, "Removing medal requirements can mess up the keystring, Use this to force the corr" +
                    "ect string\r\n\r\nRecommended to br enabled if some medal requirements are removed.");
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(44, 120);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(97, 13);
            this.label11.TabIndex = 9;
            this.label11.Text = "Medal Data Profile:";
            this.Tipsy.SetToolTip(this.label11, "The chosen profile\'s medal criteria\'s will be used the next time the server is st" +
                    "arted");
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
            this.Tipsy.SetToolTip(this.MedalData, "The chosen profile\'s medal criteria\'s will be used the next time the server is st" +
                    "arted");
            // 
            // SnapshotPrefix
            // 
            this.SnapshotPrefix.Location = new System.Drawing.Point(147, 88);
            this.SnapshotPrefix.Name = "SnapshotPrefix";
            this.SnapshotPrefix.Size = new System.Drawing.Size(146, 20);
            this.SnapshotPrefix.TabIndex = 7;
            this.Tipsy.SetToolTip(this.SnapshotPrefix, "Prefix Snapshots with this tag. This is also your Server\'s Prefix. Multiple \r\nser" +
                    "vers on the same IP Must use different Prefix\'s to tell which is which");
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
            this.Tipsy.SetToolTip(this.label10, "Prefix Snapshots with this tag. This is also your Server\'s Prefix. Multiple \r\nser" +
                    "vers on the same IP Must use different Prefix\'s to tell which is which");
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(45, 62);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(96, 13);
            this.label9.TabIndex = 5;
            this.label9.Text = "Snapshot Logging:";
            this.Tipsy.SetToolTip(this.label9, "Enables server to make snapshot backups");
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
            this.Tipsy.SetToolTip(this.Logging, "Enables server to make snapshot backups");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(79, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Debugging:";
            this.Tipsy.SetToolTip(this.label1, "Enable server debugging?");
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
            this.Tipsy.SetToolTip(this.Debugging, "Enable server debugging?");
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
            this.groupBox5.Controls.Add(this.CmKDRatio);
            this.groupBox5.Controls.Add(this.CmBanCount);
            this.groupBox5.Controls.Add(this.CmGlobalTime);
            this.groupBox5.Controls.Add(this.CmGlobalScore);
            this.groupBox5.Controls.Add(this.label21);
            this.groupBox5.Controls.Add(this.CmCountry);
            this.groupBox5.Controls.Add(this.label20);
            this.groupBox5.Controls.Add(this.label19);
            this.groupBox5.Controls.Add(this.label18);
            this.groupBox5.Controls.Add(this.CmMinRank);
            this.groupBox5.Controls.Add(this.label16);
            this.groupBox5.Controls.Add(this.label14);
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
            this.Tipsy.SetToolTip(this.label21, "Registered Country of Origin Code (Separate multiple by comma \',\')");
            // 
            // CmCountry
            // 
            this.CmCountry.Location = new System.Drawing.Point(93, 74);
            this.CmCountry.MaxLength = 2;
            this.CmCountry.Name = "CmCountry";
            this.CmCountry.Size = new System.Drawing.Size(91, 20);
            this.CmCountry.TabIndex = 19;
            this.Tipsy.SetToolTip(this.CmCountry, "Registered Country of Origin Code (Separate multiple by comma \',\')");
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
            this.Tipsy.SetToolTip(this.label20, "A player with more then this many bans, will not be allowed to join your server.\r" +
                    "\n\r\nPermBan is ALWAY BlackListed.");
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
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(72, 103);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(80, 13);
            this.label18.TabIndex = 13;
            this.label18.Text = "Minimum Rank:";
            this.Tipsy.SetToolTip(this.label18, "Player must have this Global Rank as a minimum.");
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
            this.Tipsy.SetToolTip(this.CmMinRank, "Player must have this Global Rank as a minimum.");
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(193, 28);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(101, 13);
            this.label16.TabIndex = 10;
            this.label16.Text = "Global Time Played:";
            this.Tipsy.SetToolTip(this.label16, "Player must have a minimum of this amount of seconds played");
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(16, 52);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(71, 13);
            this.label14.TabIndex = 8;
            this.label14.Text = "Global Score:";
            this.Tipsy.SetToolTip(this.label14, "Player must have this Global score as a minimum to join your server");
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(34, 25);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(53, 13);
            this.label15.TabIndex = 6;
            this.label15.Text = "Clan Tag:";
            this.Tipsy.SetToolTip(this.label15, "Clan Tag (Matches First Part of Player Name, used for Whitelist)");
            // 
            // CmClanTag
            // 
            this.CmClanTag.Location = new System.Drawing.Point(93, 22);
            this.CmClanTag.Name = "CmClanTag";
            this.CmClanTag.Size = new System.Drawing.Size(91, 20);
            this.CmClanTag.TabIndex = 7;
            this.Tipsy.SetToolTip(this.CmClanTag, "Clan Tag (Matches First Part of Player Name, used for Whitelist)");
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
            this.Tipsy.SetToolTip(this.label17, "Use the Clan Manager to control Access to your server.");
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
            this.Tipsy.SetToolTip(this.ClanManager, "Use the Clan Manager to control Access to your server.");
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
            // AspPort
            // 
            this.AspPort.Location = new System.Drawing.Point(183, 62);
            this.AspPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.AspPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.AspPort.Name = "AspPort";
            this.AspPort.Size = new System.Drawing.Size(60, 20);
            this.AspPort.TabIndex = 19;
            this.Tipsy.SetToolTip(this.AspPort, "Asp backend server port (default: 80)");
            this.AspPort.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            // 
            // CentralPort
            // 
            this.CentralPort.Location = new System.Drawing.Point(185, 99);
            this.CentralPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.CentralPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.CentralPort.Name = "CentralPort";
            this.CentralPort.Size = new System.Drawing.Size(60, 20);
            this.CentralPort.TabIndex = 20;
            this.Tipsy.SetToolTip(this.CentralPort, "Asp Central backend server port (default: 80)");
            this.CentralPort.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            // 
            // CmGlobalScore
            // 
            this.CmGlobalScore.Location = new System.Drawing.Point(93, 48);
            this.CmGlobalScore.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.CmGlobalScore.Name = "CmGlobalScore";
            this.CmGlobalScore.Size = new System.Drawing.Size(91, 20);
            this.CmGlobalScore.TabIndex = 20;
            this.CmGlobalScore.ThousandsSeparator = true;
            this.Tipsy.SetToolTip(this.CmGlobalScore, "Player must have this Global score as a minimum to join your server");
            // 
            // CmGlobalTime
            // 
            this.CmGlobalTime.Location = new System.Drawing.Point(300, 23);
            this.CmGlobalTime.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.CmGlobalTime.Name = "CmGlobalTime";
            this.CmGlobalTime.Size = new System.Drawing.Size(91, 20);
            this.CmGlobalTime.TabIndex = 21;
            this.CmGlobalTime.ThousandsSeparator = true;
            this.Tipsy.SetToolTip(this.CmGlobalTime, "Player must have a minimum of this amount of seconds played");
            // 
            // CmBanCount
            // 
            this.CmBanCount.Location = new System.Drawing.Point(300, 75);
            this.CmBanCount.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.CmBanCount.Name = "CmBanCount";
            this.CmBanCount.Size = new System.Drawing.Size(91, 20);
            this.CmBanCount.TabIndex = 22;
            this.CmBanCount.ThousandsSeparator = true;
            this.Tipsy.SetToolTip(this.CmBanCount, "A player with more then this many bans, will not be allowed to join your server.\r" +
                    "\n\r\nPermBan is ALWAY BlackListed.");
            // 
            // Tipsy
            // 
            this.Tipsy.AutoPopDelay = 10000;
            this.Tipsy.InitialDelay = 500;
            this.Tipsy.ReshowDelay = 100;
            // 
            // CmKDRatio
            // 
            this.CmKDRatio.DecimalPlaces = 1;
            this.CmKDRatio.Location = new System.Drawing.Point(300, 49);
            this.CmKDRatio.Name = "CmKDRatio";
            this.CmKDRatio.Size = new System.Drawing.Size(91, 20);
            this.CmKDRatio.TabIndex = 23;
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
            ((System.ComponentModel.ISupportInitialize)(this.AspPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CentralPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CmGlobalScore)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CmGlobalTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CmBanCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CmKDRatio)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox AspCallback;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox AspAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label5;
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
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox CmCountry;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.TextBox CentralCallback;
        private System.Windows.Forms.NumericUpDown AspPort;
        private System.Windows.Forms.NumericUpDown CentralPort;
        private System.Windows.Forms.NumericUpDown CmGlobalScore;
        private System.Windows.Forms.NumericUpDown CmBanCount;
        private System.Windows.Forms.NumericUpDown CmGlobalTime;
        private System.Windows.Forms.ToolTip Tipsy;
        private System.Windows.Forms.NumericUpDown CmKDRatio;
    }
}