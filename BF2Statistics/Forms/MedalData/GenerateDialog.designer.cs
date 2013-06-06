namespace BF2Statistics.MedalData
{
    partial class GenerateDialog
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SfBtn = new System.Windows.Forms.Button();
            this.VanillaBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Location = new System.Drawing.Point(10, 17);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(269, 49);
            this.textBox1.TabIndex = 5;
            this.textBox1.Text = "Select medal data type. Special forces medal data files will be generated with th" +
                "e Special Forces awards data in them, allowing them to be earned";
            // 
            // SfBtn
            // 
            this.SfBtn.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.SfBtn.Location = new System.Drawing.Point(149, 72);
            this.SfBtn.Name = "SfBtn";
            this.SfBtn.Size = new System.Drawing.Size(102, 35);
            this.SfBtn.TabIndex = 3;
            this.SfBtn.Text = "Special Forces";
            this.SfBtn.UseVisualStyleBackColor = true;
            // 
            // VanillaBtn
            // 
            this.VanillaBtn.DialogResult = System.Windows.Forms.DialogResult.No;
            this.VanillaBtn.Location = new System.Drawing.Point(29, 72);
            this.VanillaBtn.Name = "VanillaBtn";
            this.VanillaBtn.Size = new System.Drawing.Size(102, 35);
            this.VanillaBtn.TabIndex = 4;
            this.VanillaBtn.Text = "Vanilla";
            this.VanillaBtn.UseVisualStyleBackColor = true;
            // 
            // GenerateDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 133);
            this.Controls.Add(this.VanillaBtn);
            this.Controls.Add(this.SfBtn);
            this.Controls.Add(this.textBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GenerateDialog";
            this.Text = "Generate Medal Data";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button SfBtn;
        private System.Windows.Forms.Button VanillaBtn;
    }
}