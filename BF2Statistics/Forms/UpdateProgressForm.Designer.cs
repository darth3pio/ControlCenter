namespace BF2Statistics
{
    partial class UpdateProgressForm
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
            this.StatusText = new System.Windows.Forms.RichTextBox();
            this.ProgBar = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.ProgBar)).BeginInit();
            this.SuspendLayout();
            // 
            // StatusText
            // 
            this.StatusText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.StatusText.Location = new System.Drawing.Point(12, 28);
            this.StatusText.Name = "StatusText";
            this.StatusText.ReadOnly = true;
            this.StatusText.Size = new System.Drawing.Size(268, 22);
            this.StatusText.TabIndex = 2;
            this.StatusText.Text = "";
            // 
            // ProgBar
            // 
            this.ProgBar.Image = global::BF2Statistics.Properties.Resources.loading11;
            this.ProgBar.Location = new System.Drawing.Point(36, 56);
            this.ProgBar.Name = "ProgBar";
            this.ProgBar.Size = new System.Drawing.Size(223, 22);
            this.ProgBar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.ProgBar.TabIndex = 3;
            this.ProgBar.TabStop = false;
            // 
            // UpdateProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(294, 92);
            this.Controls.Add(this.ProgBar);
            this.Controls.Add(this.StatusText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateProgressForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Progress";
            ((System.ComponentModel.ISupportInitialize)(this.ProgBar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox StatusText;
        private System.Windows.Forms.PictureBox ProgBar;
    }
}