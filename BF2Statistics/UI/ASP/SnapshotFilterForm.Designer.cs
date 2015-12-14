namespace BF2Statistics
{
    partial class SnapshotFilterForm
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
            this.label4 = new System.Windows.Forms.Label();
            this.PrefixBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.MapNameBox = new System.Windows.Forms.TextBox();
            this.EndDateTime = new System.Windows.Forms.DateTimePicker();
            this.StartDateTime = new System.Windows.Forms.DateTimePicker();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.PrefixBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.MapNameBox);
            this.groupBox1.Controls.Add(this.EndDateTime);
            this.groupBox1.Controls.Add(this.StartDateTime);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(348, 210);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Basic Search";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Server Prefix (optional)";
            // 
            // PrefixBox
            // 
            this.PrefixBox.Location = new System.Drawing.Point(19, 98);
            this.PrefixBox.Name = "PrefixBox";
            this.PrefixBox.Size = new System.Drawing.Size(265, 20);
            this.PrefixBox.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(145, 167);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "- through -";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 137);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Date Range:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Map Name (optional)";
            // 
            // MapNameBox
            // 
            this.MapNameBox.Location = new System.Drawing.Point(19, 44);
            this.MapNameBox.Name = "MapNameBox";
            this.MapNameBox.Size = new System.Drawing.Size(265, 20);
            this.MapNameBox.TabIndex = 1;
            this.MapNameBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // EndDateTime
            // 
            this.EndDateTime.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.EndDateTime.Location = new System.Drawing.Point(206, 163);
            this.EndDateTime.Name = "EndDateTime";
            this.EndDateTime.Size = new System.Drawing.Size(120, 20);
            this.EndDateTime.TabIndex = 4;
            // 
            // StartDateTime
            // 
            this.StartDateTime.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.StartDateTime.Location = new System.Drawing.Point(19, 163);
            this.StartDateTime.Name = "StartDateTime";
            this.StartDateTime.Size = new System.Drawing.Size(120, 20);
            this.StartDateTime.TabIndex = 3;
            this.StartDateTime.Value = new System.DateTime(2005, 6, 21, 0, 0, 0, 0);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(263, 240);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Filter";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(182, 240);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // SnapshotFilterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(370, 280);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SnapshotFilterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Filter Results";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        public System.Windows.Forms.TextBox PrefixBox;
        public System.Windows.Forms.TextBox MapNameBox;
        public System.Windows.Forms.DateTimePicker EndDateTime;
        public System.Windows.Forms.DateTimePicker StartDateTime;

    }
}