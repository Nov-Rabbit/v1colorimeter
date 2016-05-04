namespace X2DisplayTest
{
    partial class ModeSwithDlg
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
            this.rbTestPanel = new System.Windows.Forms.RadioButton();
            this.rbAnalysisPanel = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rbTestPanel
            // 
            this.rbTestPanel.AutoSize = true;
            this.rbTestPanel.Location = new System.Drawing.Point(36, 40);
            this.rbTestPanel.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rbTestPanel.Name = "rbTestPanel";
            this.rbTestPanel.Size = new System.Drawing.Size(89, 28);
            this.rbTestPanel.TabIndex = 0;
            this.rbTestPanel.TabStop = true;
            this.rbTestPanel.Text = "Test";
            this.rbTestPanel.UseVisualStyleBackColor = true;
            // 
            // rbAnalysisPanel
            // 
            this.rbAnalysisPanel.AutoSize = true;
            this.rbAnalysisPanel.Location = new System.Drawing.Point(288, 40);
            this.rbAnalysisPanel.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rbAnalysisPanel.Name = "rbAnalysisPanel";
            this.rbAnalysisPanel.Size = new System.Drawing.Size(137, 28);
            this.rbAnalysisPanel.TabIndex = 1;
            this.rbAnalysisPanel.TabStop = true;
            this.rbAnalysisPanel.Text = "Analysis";
            this.rbAnalysisPanel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbTestPanel);
            this.groupBox1.Controls.Add(this.rbAnalysisPanel);
            this.groupBox1.Location = new System.Drawing.Point(24, 17);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox1.Size = new System.Drawing.Size(480, 98);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(358, 134);
            this.btnOK.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(150, 46);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "Enter";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // ModeSwithDlg
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(528, 206);
            this.ControlBox = false;
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModeSwithDlg";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "mode switch";
            this.Load += new System.EventHandler(this.ModeSwithDlg_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton rbTestPanel;
        private System.Windows.Forms.RadioButton rbAnalysisPanel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnOK;
    }
}