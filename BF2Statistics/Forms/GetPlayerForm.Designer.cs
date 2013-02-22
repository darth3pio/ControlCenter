namespace BF2Statistics
{
    partial class GetPlayerForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.Input = new System.Windows.Forms.TextBox();
            this.SubmitBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(56, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enter Player Nick or PID:";
            // 
            // Input
            // 
            this.Input.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.Input.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.Input.Location = new System.Drawing.Point(19, 63);
            this.Input.Name = "Input";
            this.Input.Size = new System.Drawing.Size(201, 20);
            this.Input.TabIndex = 1;
            this.Input.TextChanged += new System.EventHandler(this.Input_TextChanged);
            // 
            // SubmitBtn
            // 
            this.SubmitBtn.Location = new System.Drawing.Point(76, 101);
            this.SubmitBtn.Name = "SubmitBtn";
            this.SubmitBtn.Size = new System.Drawing.Size(75, 27);
            this.SubmitBtn.TabIndex = 2;
            this.SubmitBtn.Text = "Submit";
            this.SubmitBtn.UseVisualStyleBackColor = true;
            this.SubmitBtn.Click += new System.EventHandler(this.SubmitBtn_Click);
            // 
            // GetPlayerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(236, 142);
            this.Controls.Add(this.SubmitBtn);
            this.Controls.Add(this.Input);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GetPlayerForm";
            this.Text = "Player Info";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Input;
        private System.Windows.Forms.Button SubmitBtn;
    }
}