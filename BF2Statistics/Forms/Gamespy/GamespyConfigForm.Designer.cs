namespace BF2Statistics
{
    partial class GamespyConfigForm
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.StatusText = new System.Windows.Forms.Label();
            this.StatusPic = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.AllowExtChkBox = new System.Windows.Forms.CheckBox();
            this.FetchAddressBtn = new System.Windows.Forms.Button();
            this.EnableChkBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.AddressTextBox = new System.Windows.Forms.TextBox();
            this.shapeContainer2 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.lineShape2 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.panel1 = new System.Windows.Forms.Panel();
            this.DescLabel = new System.Windows.Forms.Label();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.lineShape1 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.SaveBtn = new System.Windows.Forms.Button();
            this.DebugChkBox = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.StatusPic)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Window;
            this.panel2.Controls.Add(this.DebugChkBox);
            this.panel2.Controls.Add(this.StatusText);
            this.panel2.Controls.Add(this.StatusPic);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.AllowExtChkBox);
            this.panel2.Controls.Add(this.FetchAddressBtn);
            this.panel2.Controls.Add(this.EnableChkBox);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.AddressTextBox);
            this.panel2.Controls.Add(this.shapeContainer2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 69);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(467, 208);
            this.panel2.TabIndex = 13;
            // 
            // StatusText
            // 
            this.StatusText.AutoSize = true;
            this.StatusText.Location = new System.Drawing.Point(209, 177);
            this.StatusText.Name = "StatusText";
            this.StatusText.Size = new System.Drawing.Size(151, 13);
            this.StatusText.TabIndex = 19;
            this.StatusText.Text = "Fetching External Ip Address...";
            this.StatusText.Visible = false;
            // 
            // StatusPic
            // 
            this.StatusPic.Image = global::BF2Statistics.Properties.Resources.loading;
            this.StatusPic.Location = new System.Drawing.Point(178, 170);
            this.StatusPic.Name = "StatusPic";
            this.StatusPic.Size = new System.Drawing.Size(25, 24);
            this.StatusPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.StatusPic.TabIndex = 18;
            this.StatusPic.TabStop = false;
            this.StatusPic.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(414, 26);
            this.label3.TabIndex = 17;
            this.label3.Text = "In order for external clients to connect to your local servers from the server br" +
    "owser, \r\nYou must enter your External IP address so these external clients get a" +
    " proper address.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 16;
            // 
            // AllowExtChkBox
            // 
            this.AllowExtChkBox.AutoSize = true;
            this.AllowExtChkBox.Location = new System.Drawing.Point(115, 45);
            this.AllowExtChkBox.Name = "AllowExtChkBox";
            this.AllowExtChkBox.Size = new System.Drawing.Size(236, 17);
            this.AllowExtChkBox.TabIndex = 15;
            this.AllowExtChkBox.Text = "Allow External Servers to Report to Serverlist";
            this.AllowExtChkBox.UseVisualStyleBackColor = true;
            // 
            // FetchAddressBtn
            // 
            this.FetchAddressBtn.Location = new System.Drawing.Point(340, 141);
            this.FetchAddressBtn.Name = "FetchAddressBtn";
            this.FetchAddressBtn.Size = new System.Drawing.Size(79, 22);
            this.FetchAddressBtn.TabIndex = 14;
            this.FetchAddressBtn.Text = "Lookup MyIP";
            this.FetchAddressBtn.UseVisualStyleBackColor = true;
            this.FetchAddressBtn.Click += new System.EventHandler(this.FetchAddressBtn_Click);
            // 
            // EnableChkBox
            // 
            this.EnableChkBox.AutoSize = true;
            this.EnableChkBox.Location = new System.Drawing.Point(164, 20);
            this.EnableChkBox.Name = "EnableChkBox";
            this.EnableChkBox.Size = new System.Drawing.Size(138, 17);
            this.EnableChkBox.TabIndex = 4;
            this.EnableChkBox.Text = "Enable Online Serverlist";
            this.EnableChkBox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(48, 146);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Local Server External IP:";
            // 
            // AddressTextBox
            // 
            this.AddressTextBox.Location = new System.Drawing.Point(178, 143);
            this.AddressTextBox.Name = "AddressTextBox";
            this.AddressTextBox.Size = new System.Drawing.Size(145, 20);
            this.AddressTextBox.TabIndex = 2;
            this.AddressTextBox.Text = "127.0.0.1";
            // 
            // shapeContainer2
            // 
            this.shapeContainer2.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer2.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer2.Name = "shapeContainer2";
            this.shapeContainer2.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.lineShape2});
            this.shapeContainer2.Size = new System.Drawing.Size(467, 208);
            this.shapeContainer2.TabIndex = 5;
            this.shapeContainer2.TabStop = false;
            // 
            // lineShape2
            // 
            this.lineShape2.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.lineShape2.Name = "lineShape2";
            this.lineShape2.X1 = -1;
            this.lineShape2.X2 = 467;
            this.lineShape2.Y1 = 205;
            this.lineShape2.Y2 = 205;
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
            this.panel1.Size = new System.Drawing.Size(467, 69);
            this.panel1.TabIndex = 12;
            // 
            // DescLabel
            // 
            this.DescLabel.AutoSize = true;
            this.DescLabel.Location = new System.Drawing.Point(34, 39);
            this.DescLabel.Name = "DescLabel";
            this.DescLabel.Size = new System.Drawing.Size(355, 13);
            this.DescLabel.TabIndex = 2;
            this.DescLabel.Text = "Ports: 28910, 29900, 29901, 27900 and 29910 may need to be forwarded";
            // 
            // TitleLabel
            // 
            this.TitleLabel.AutoSize = true;
            this.TitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TitleLabel.Location = new System.Drawing.Point(24, 21);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(190, 13);
            this.TitleLabel.TabIndex = 1;
            this.TitleLabel.Text = "Gamespy Emulator Configuration";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::BF2Statistics.Properties.Resources.GamespyIcon;
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
            this.shapeContainer1.Size = new System.Drawing.Size(467, 69);
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
            // CancelBtn
            // 
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Location = new System.Drawing.Point(113, 290);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(118, 29);
            this.CancelBtn.TabIndex = 11;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            // 
            // SaveBtn
            // 
            this.SaveBtn.Location = new System.Drawing.Point(235, 290);
            this.SaveBtn.Name = "SaveBtn";
            this.SaveBtn.Size = new System.Drawing.Size(118, 29);
            this.SaveBtn.TabIndex = 10;
            this.SaveBtn.Text = "Save";
            this.SaveBtn.UseVisualStyleBackColor = true;
            this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
            // 
            // DebugChkBox
            // 
            this.DebugChkBox.AutoSize = true;
            this.DebugChkBox.Location = new System.Drawing.Point(115, 72);
            this.DebugChkBox.Name = "DebugChkBox";
            this.DebugChkBox.Size = new System.Drawing.Size(244, 17);
            this.DebugChkBox.TabIndex = 20;
            this.DebugChkBox.Text = "Enable Debugging (Logs/GamespyDebug.log)";
            this.DebugChkBox.UseVisualStyleBackColor = true;
            // 
            // GamespyConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 332);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.SaveBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GamespyConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Gamespy Emulator Configuration";
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.StatusPic)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox EnableChkBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox AddressTextBox;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label DescLabel;
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Button SaveBtn;
        private System.Windows.Forms.Button FetchAddressBtn;
        private System.Windows.Forms.CheckBox AllowExtChkBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label StatusText;
        private System.Windows.Forms.PictureBox StatusPic;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape1;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape2;
        private System.Windows.Forms.CheckBox DebugChkBox;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}