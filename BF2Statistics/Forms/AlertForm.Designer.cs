namespace BF2Statistics
{
    partial class AlertForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlertForm));
            this.CloseTimer = new System.Windows.Forms.Timer(this.components);
            this.OpenTimer = new System.Windows.Forms.Timer(this.components);
            this.AlertIconBox = new System.Windows.Forms.PictureBox();
            this.CollapseTimer = new System.Windows.Forms.Timer(this.components);
            this.LogoText = new BF2Statistics.TransparentLabel();
            this.labelHeader = new BF2Statistics.TransparentLabel();
            this.labelDetails = new BF2Statistics.TransparentLabel();
            ((System.ComponentModel.ISupportInitialize)(this.AlertIconBox)).BeginInit();
            this.SuspendLayout();
            // 
            // CloseTimer
            // 
            this.CloseTimer.Interval = 3000;
            this.CloseTimer.Tick += new System.EventHandler(this.CloseTimer_Tick);
            // 
            // OpenTimer
            // 
            this.OpenTimer.Enabled = true;
            this.OpenTimer.Interval = 15;
            this.OpenTimer.Tick += new System.EventHandler(this.OpenTimer_Tick);
            // 
            // AlertIconBox
            // 
            this.AlertIconBox.BackColor = System.Drawing.Color.Transparent;
            this.AlertIconBox.Image = ((System.Drawing.Image)(resources.GetObject("AlertIconBox.Image")));
            this.AlertIconBox.Location = new System.Drawing.Point(8, 9);
            this.AlertIconBox.Name = "AlertIconBox";
            this.AlertIconBox.Size = new System.Drawing.Size(54, 54);
            this.AlertIconBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.AlertIconBox.TabIndex = 1;
            this.AlertIconBox.TabStop = false;
            // 
            // CollapseTimer
            // 
            this.CollapseTimer.Interval = 20;
            this.CollapseTimer.Tick += new System.EventHandler(this.CollapseTimer_Tick);
            // 
            // LogoText
            // 
            this.LogoText.AutoSize = true;
            this.LogoText.BackColor = System.Drawing.Color.Transparent;
            this.LogoText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LogoText.ForeColor = System.Drawing.Color.Gray;
            this.LogoText.Location = new System.Drawing.Point(91, 9);
            this.LogoText.Name = "LogoText";
            this.LogoText.Size = new System.Drawing.Size(166, 13);
            this.LogoText.TabIndex = 2;
            this.LogoText.Text = "BF2Statistics Control Center";
            // 
            // labelHeader
            // 
            this.labelHeader.AutoSize = true;
            this.labelHeader.BackColor = System.Drawing.Color.Transparent;
            this.labelHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHeader.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.labelHeader.Location = new System.Drawing.Point(66, 31);
            this.labelHeader.MaximumSize = new System.Drawing.Size(260, 0);
            this.labelHeader.MinimumSize = new System.Drawing.Size(260, 0);
            this.labelHeader.Name = "labelHeader";
            this.labelHeader.Size = new System.Drawing.Size(260, 13);
            this.labelHeader.TabIndex = 0;
            this.labelHeader.Text = "Snapshot Processed Successfully!";
            // 
            // labelDetails
            // 
            this.labelDetails.AutoSize = true;
            this.labelDetails.BackColor = System.Drawing.Color.Transparent;
            this.labelDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDetails.ForeColor = System.Drawing.Color.DarkGray;
            this.labelDetails.Location = new System.Drawing.Point(67, 49);
            this.labelDetails.Margin = new System.Windows.Forms.Padding(3, 0, 3, 12);
            this.labelDetails.MaximumSize = new System.Drawing.Size(260, 0);
            this.labelDetails.MinimumSize = new System.Drawing.Size(260, 0);
            this.labelDetails.Name = "labelDetails";
            this.labelDetails.Size = new System.Drawing.Size(260, 12);
            this.labelDetails.TabIndex = 3;
            this.labelDetails.Text = "From IP: 174.128.111.100";
            // 
            // AlertForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.ClientSize = new System.Drawing.Size(348, 73);
            this.ControlBox = false;
            this.Controls.Add(this.LogoText);
            this.Controls.Add(this.AlertIconBox);
            this.Controls.Add(this.labelHeader);
            this.Controls.Add(this.labelDetails);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MinimumSize = new System.Drawing.Size(350, 75);
            this.Name = "AlertForm";
            this.Opacity = 0.9D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.LocationChanged += new System.EventHandler(this.AlertForm_LocationChanged);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.AlertForm_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.AlertIconBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer CloseTimer;
        private System.Windows.Forms.Timer OpenTimer;
        private System.Windows.Forms.PictureBox AlertIconBox;
        private BF2Statistics.TransparentLabel labelHeader;
        private BF2Statistics.TransparentLabel LogoText;
        private BF2Statistics.TransparentLabel labelDetails;
        private System.Windows.Forms.Timer CollapseTimer;
    }
}