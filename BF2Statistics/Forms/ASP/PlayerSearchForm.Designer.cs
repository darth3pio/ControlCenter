namespace BF2Statistics
{
    partial class PlayerSearchForm
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
            this.DataTable = new System.Windows.Forms.DataGridView();
            this.rank = new System.Windows.Forms.DataGridViewImageColumn();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clantag = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.score = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.country = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.permban = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deletePlayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.importPlayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromEAStatsServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromPlayerExportSheetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportPlayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.SearchBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.LimitSelect = new System.Windows.Forms.ComboBox();
            this.RowCountDesc = new System.Windows.Forms.Label();
            this.FirstBtn = new System.Windows.Forms.Button();
            this.LastBtn = new System.Windows.Forms.Button();
            this.NextBtn = new System.Windows.Forms.Button();
            this.PreviousBtn = new System.Windows.Forms.Button();
            this.PageNumber = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.DataTable)).BeginInit();
            this.contextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PageNumber)).BeginInit();
            this.SuspendLayout();
            // 
            // DataTable
            // 
            this.DataTable.AllowUserToAddRows = false;
            this.DataTable.AllowUserToDeleteRows = false;
            this.DataTable.AllowUserToOrderColumns = true;
            this.DataTable.AllowUserToResizeRows = false;
            this.DataTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.rank,
            this.id,
            this.name,
            this.clantag,
            this.score,
            this.country,
            this.permban});
            this.DataTable.ContextMenuStrip = this.contextMenu;
            this.DataTable.Location = new System.Drawing.Point(25, 84);
            this.DataTable.MultiSelect = false;
            this.DataTable.Name = "DataTable";
            this.DataTable.ReadOnly = true;
            this.DataTable.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.DataTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DataTable.Size = new System.Drawing.Size(744, 432);
            this.DataTable.TabIndex = 0;
            this.DataTable.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataTable_CellDoubleClick);
            this.DataTable.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataTable_ColumnHeaderMouseClick);
            // 
            // rank
            // 
            this.rank.HeaderText = "";
            this.rank.Name = "rank";
            this.rank.ReadOnly = true;
            this.rank.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.rank.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.rank.Width = 50;
            // 
            // id
            // 
            this.id.HeaderText = "PID";
            this.id.Name = "id";
            this.id.ReadOnly = true;
            this.id.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.id.Width = 75;
            // 
            // name
            // 
            this.name.HeaderText = "Nick";
            this.name.Name = "name";
            this.name.ReadOnly = true;
            this.name.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.name.Width = 200;
            // 
            // clantag
            // 
            this.clantag.HeaderText = "Clan Tag";
            this.clantag.Name = "clantag";
            this.clantag.ReadOnly = true;
            this.clantag.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // score
            // 
            this.score.HeaderText = "Global Score";
            this.score.Name = "score";
            this.score.ReadOnly = true;
            this.score.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // country
            // 
            this.country.HeaderText = "Country";
            this.country.Name = "country";
            this.country.ReadOnly = true;
            this.country.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // permban
            // 
            this.permban.HeaderText = "PermBan";
            this.permban.Name = "permban";
            this.permban.ReadOnly = true;
            this.permban.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.permban.Width = 75;
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deletePlayerToolStripMenuItem,
            this.toolStripSeparator1,
            this.importPlayerToolStripMenuItem,
            this.exportPlayerToolStripMenuItem});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(153, 98);
            this.contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenu_Opening);
            // 
            // deletePlayerToolStripMenuItem
            // 
            this.deletePlayerToolStripMenuItem.Name = "deletePlayerToolStripMenuItem";
            this.deletePlayerToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deletePlayerToolStripMenuItem.Text = "Delete Player";
            this.deletePlayerToolStripMenuItem.Click += new System.EventHandler(this.deletePlayerMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // importPlayerToolStripMenuItem
            // 
            this.importPlayerToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fromEAStatsServerToolStripMenuItem,
            this.fromPlayerExportSheetToolStripMenuItem});
            this.importPlayerToolStripMenuItem.Name = "importPlayerToolStripMenuItem";
            this.importPlayerToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.importPlayerToolStripMenuItem.Text = "Import Player";
            this.importPlayerToolStripMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            // 
            // fromEAStatsServerToolStripMenuItem
            // 
            this.fromEAStatsServerToolStripMenuItem.Name = "fromEAStatsServerToolStripMenuItem";
            this.fromEAStatsServerToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.fromEAStatsServerToolStripMenuItem.Text = "From A Different ASP Server";
            this.fromEAStatsServerToolStripMenuItem.Click += new System.EventHandler(this.fromEAStatsServerMenuItem_Click);
            // 
            // fromPlayerExportSheetToolStripMenuItem
            // 
            this.fromPlayerExportSheetToolStripMenuItem.Name = "fromPlayerExportSheetToolStripMenuItem";
            this.fromPlayerExportSheetToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.fromPlayerExportSheetToolStripMenuItem.Text = "From Player Export Sheet";
            this.fromPlayerExportSheetToolStripMenuItem.Click += new System.EventHandler(this.fromPlayerExportSheetMenuItem_Click);
            // 
            // exportPlayerToolStripMenuItem
            // 
            this.exportPlayerToolStripMenuItem.Name = "exportPlayerToolStripMenuItem";
            this.exportPlayerToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exportPlayerToolStripMenuItem.Text = "Export Player";
            this.exportPlayerToolStripMenuItem.Click += new System.EventHandler(this.exportPlayerMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Window;
            this.label1.Location = new System.Drawing.Point(473, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Search By Name:";
            // 
            // SearchBox
            // 
            this.SearchBox.Location = new System.Drawing.Point(569, 54);
            this.SearchBox.Name = "SearchBox";
            this.SearchBox.Size = new System.Drawing.Size(200, 20);
            this.SearchBox.TabIndex = 2;
            this.SearchBox.TextChanged += new System.EventHandler(this.SearchBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.Window;
            this.label2.Location = new System.Drawing.Point(26, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Records Per Page: ";
            // 
            // LimitSelect
            // 
            this.LimitSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LimitSelect.FormattingEnabled = true;
            this.LimitSelect.Items.AddRange(new object[] {
            "10",
            "25",
            "50",
            "100",
            "150",
            "250"});
            this.LimitSelect.Location = new System.Drawing.Point(132, 54);
            this.LimitSelect.Name = "LimitSelect";
            this.LimitSelect.Size = new System.Drawing.Size(100, 21);
            this.LimitSelect.TabIndex = 4;
            this.LimitSelect.SelectedIndexChanged += new System.EventHandler(this.LimitSelect_SelectedIndexChanged);
            // 
            // RowCountDesc
            // 
            this.RowCountDesc.AutoSize = true;
            this.RowCountDesc.Location = new System.Drawing.Point(26, 532);
            this.RowCountDesc.Name = "RowCountDesc";
            this.RowCountDesc.Size = new System.Drawing.Size(65, 13);
            this.RowCountDesc.TabIndex = 5;
            this.RowCountDesc.Text = "PlaceHolder";
            // 
            // FirstBtn
            // 
            this.FirstBtn.Location = new System.Drawing.Point(385, 527);
            this.FirstBtn.Name = "FirstBtn";
            this.FirstBtn.Size = new System.Drawing.Size(70, 25);
            this.FirstBtn.TabIndex = 6;
            this.FirstBtn.Text = "First";
            this.FirstBtn.UseVisualStyleBackColor = true;
            this.FirstBtn.Click += new System.EventHandler(this.FirstBtn_Click);
            // 
            // LastBtn
            // 
            this.LastBtn.Location = new System.Drawing.Point(690, 526);
            this.LastBtn.Name = "LastBtn";
            this.LastBtn.Size = new System.Drawing.Size(70, 25);
            this.LastBtn.TabIndex = 7;
            this.LastBtn.Text = "Last";
            this.LastBtn.UseVisualStyleBackColor = true;
            this.LastBtn.Click += new System.EventHandler(this.LastBtn_Click);
            // 
            // NextBtn
            // 
            this.NextBtn.Location = new System.Drawing.Point(614, 526);
            this.NextBtn.Name = "NextBtn";
            this.NextBtn.Size = new System.Drawing.Size(70, 25);
            this.NextBtn.TabIndex = 9;
            this.NextBtn.Text = "Next";
            this.NextBtn.UseVisualStyleBackColor = true;
            this.NextBtn.Click += new System.EventHandler(this.NextBtn_Click);
            // 
            // PreviousBtn
            // 
            this.PreviousBtn.Location = new System.Drawing.Point(461, 527);
            this.PreviousBtn.Name = "PreviousBtn";
            this.PreviousBtn.Size = new System.Drawing.Size(70, 25);
            this.PreviousBtn.TabIndex = 8;
            this.PreviousBtn.Text = "Previous";
            this.PreviousBtn.UseVisualStyleBackColor = true;
            this.PreviousBtn.Click += new System.EventHandler(this.PreviousBtn_Click);
            // 
            // PageNumber
            // 
            this.PageNumber.Location = new System.Drawing.Point(538, 530);
            this.PageNumber.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.PageNumber.Name = "PageNumber";
            this.PageNumber.ReadOnly = true;
            this.PageNumber.Size = new System.Drawing.Size(70, 20);
            this.PageNumber.TabIndex = 10;
            this.PageNumber.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.PageNumber.ValueChanged += new System.EventHandler(this.PageNumber_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(574, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Below is a list of all the players in your stats database. Select a player to edi" +
                "t or view by double clicking on a player below";
            // 
            // PlayerSearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(794, 572);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.PageNumber);
            this.Controls.Add(this.NextBtn);
            this.Controls.Add(this.PreviousBtn);
            this.Controls.Add(this.LastBtn);
            this.Controls.Add(this.FirstBtn);
            this.Controls.Add(this.RowCountDesc);
            this.Controls.Add(this.LimitSelect);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.SearchBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DataTable);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PlayerSearchForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Player Search";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PlayerSearchForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.DataTable)).EndInit();
            this.contextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PageNumber)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView DataTable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox SearchBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox LimitSelect;
        private System.Windows.Forms.Label RowCountDesc;
        private System.Windows.Forms.Button FirstBtn;
        private System.Windows.Forms.Button LastBtn;
        private System.Windows.Forms.Button NextBtn;
        private System.Windows.Forms.Button PreviousBtn;
        private System.Windows.Forms.NumericUpDown PageNumber;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem deletePlayerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem importPlayerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportPlayerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromEAStatsServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromPlayerExportSheetToolStripMenuItem;
        private System.Windows.Forms.DataGridViewImageColumn rank;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
        private System.Windows.Forms.DataGridViewTextBoxColumn clantag;
        private System.Windows.Forms.DataGridViewTextBoxColumn score;
        private System.Windows.Forms.DataGridViewTextBoxColumn country;
        private System.Windows.Forms.DataGridViewTextBoxColumn permban;
    }
}