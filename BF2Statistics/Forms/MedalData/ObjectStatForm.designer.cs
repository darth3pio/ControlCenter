namespace BF2Statistics.MedalData
{
    partial class ObjectStatForm
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
            this.ObjectSelect = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ConditionSelect = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.StatSelect = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.ObjectTypeLabel = new System.Windows.Forms.Label();
            this.TypeSelect = new System.Windows.Forms.ComboBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.ValueBox = new System.Windows.Forms.NumericUpDown();
            this.Hms2SecBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ValueBox)).BeginInit();
            this.SuspendLayout();
            // 
            // ObjectSelect
            // 
            this.ObjectSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ObjectSelect.FormattingEnabled = true;
            this.ObjectSelect.Items.AddRange(new object[] {
            "Kit",
            "Weapon",
            "Vehicle"});
            this.ObjectSelect.Location = new System.Drawing.Point(107, 42);
            this.ObjectSelect.Name = "ObjectSelect";
            this.ObjectSelect.Size = new System.Drawing.Size(170, 21);
            this.ObjectSelect.TabIndex = 0;
            this.ObjectSelect.SelectedIndexChanged += new System.EventHandler(this.ObjectSelect_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Object Catagory";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(46, 138);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Condition";
            // 
            // ConditionSelect
            // 
            this.ConditionSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ConditionSelect.FormattingEnabled = true;
            this.ConditionSelect.Items.AddRange(new object[] {
            "Return Stat Value",
            "Equals or Greater Than"});
            this.ConditionSelect.Location = new System.Drawing.Point(107, 135);
            this.ConditionSelect.Name = "ConditionSelect";
            this.ConditionSelect.Size = new System.Drawing.Size(170, 21);
            this.ConditionSelect.TabIndex = 2;
            this.ConditionSelect.SelectedIndexChanged += new System.EventHandler(this.ConditionSelect_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(37, 107);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Object Stat";
            // 
            // StatSelect
            // 
            this.StatSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.StatSelect.FormattingEnabled = true;
            this.StatSelect.Items.AddRange(new object[] {
            "Kills",
            "Round Time",
            "Road Kills",
            "Deploys"});
            this.StatSelect.Location = new System.Drawing.Point(107, 104);
            this.StatSelect.Name = "StatSelect";
            this.StatSelect.Size = new System.Drawing.Size(170, 21);
            this.StatSelect.TabIndex = 4;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(70, 200);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(160, 29);
            this.button1.TabIndex = 8;
            this.button1.Text = "Finish";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ObjectTypeLabel
            // 
            this.ObjectTypeLabel.AutoSize = true;
            this.ObjectTypeLabel.Location = new System.Drawing.Point(32, 76);
            this.ObjectTypeLabel.Name = "ObjectTypeLabel";
            this.ObjectTypeLabel.Size = new System.Drawing.Size(65, 13);
            this.ObjectTypeLabel.TabIndex = 10;
            this.ObjectTypeLabel.Text = "Object Type";
            // 
            // TypeSelect
            // 
            this.TypeSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TypeSelect.FormattingEnabled = true;
            this.TypeSelect.Location = new System.Drawing.Point(107, 73);
            this.TypeSelect.Name = "TypeSelect";
            this.TypeSelect.Size = new System.Drawing.Size(170, 21);
            this.TypeSelect.TabIndex = 9;
            // 
            // textBox2
            // 
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Location = new System.Drawing.Point(51, 15);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(180, 23);
            this.textBox2.TabIndex = 11;
            this.textBox2.Text = "Object stats are In Round stats only.";
            // 
            // ValueBox
            // 
            this.ValueBox.Location = new System.Drawing.Point(107, 166);
            this.ValueBox.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.ValueBox.Name = "ValueBox";
            this.ValueBox.Size = new System.Drawing.Size(120, 20);
            this.ValueBox.TabIndex = 12;
            // 
            // Hms2SecBtn
            // 
            this.Hms2SecBtn.Location = new System.Drawing.Point(241, 163);
            this.Hms2SecBtn.Name = "Hms2SecBtn";
            this.Hms2SecBtn.Size = new System.Drawing.Size(36, 23);
            this.Hms2SecBtn.TabIndex = 18;
            this.Hms2SecBtn.Text = "Hms";
            this.Hms2SecBtn.UseVisualStyleBackColor = true;
            this.Hms2SecBtn.Click += new System.EventHandler(this.Hms2SecBtn_Click);
            // 
            // ObjectStatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(294, 247);
            this.Controls.Add(this.Hms2SecBtn);
            this.Controls.Add(this.ValueBox);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.ObjectTypeLabel);
            this.Controls.Add(this.TypeSelect);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.StatSelect);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ConditionSelect);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ObjectSelect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ObjectStatForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Object Stat";
            ((System.ComponentModel.ISupportInitialize)(this.ValueBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox ObjectSelect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox ConditionSelect;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox StatSelect;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label ObjectTypeLabel;
        private System.Windows.Forms.ComboBox TypeSelect;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.NumericUpDown ValueBox;
        private System.Windows.Forms.Button Hms2SecBtn;
    }
}