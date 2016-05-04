namespace X2DisplayTest
{
    partial class TestNodePanel
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label7 = new System.Windows.Forms.Label();
            this.tbUpper = new System.Windows.Forms.TextBox();
            this.tbLower = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbUnit = new System.Windows.Forms.TextBox();
            this.tbErrorCode = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbItemName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbIsNeedTest = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 48);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 24;
            this.label7.Text = "upper:";
            // 
            // tbUpper
            // 
            this.tbUpper.Location = new System.Drawing.Point(52, 46);
            this.tbUpper.Name = "tbUpper";
            this.tbUpper.Size = new System.Drawing.Size(53, 21);
            this.tbUpper.TabIndex = 25;
            this.tbUpper.Text = "0";
            this.tbUpper.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbLower
            // 
            this.tbLower.Location = new System.Drawing.Point(183, 46);
            this.tbLower.Name = "tbLower";
            this.tbLower.Size = new System.Drawing.Size(53, 21);
            this.tbLower.TabIndex = 27;
            this.tbLower.Text = "0";
            this.tbLower.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(141, 48);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(41, 12);
            this.label12.TabIndex = 26;
            this.label12.Text = "lower:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 85);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 28;
            this.label1.Text = "unit:";
            // 
            // tbUnit
            // 
            this.tbUnit.Location = new System.Drawing.Point(52, 82);
            this.tbUnit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbUnit.Name = "tbUnit";
            this.tbUnit.Size = new System.Drawing.Size(52, 21);
            this.tbUnit.TabIndex = 29;
            this.tbUnit.Text = "nt";
            this.tbUnit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbErrorCode
            // 
            this.tbErrorCode.Location = new System.Drawing.Point(183, 82);
            this.tbErrorCode.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbErrorCode.Name = "tbErrorCode";
            this.tbErrorCode.Size = new System.Drawing.Size(52, 21);
            this.tbErrorCode.TabIndex = 31;
            this.tbErrorCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(116, 84);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 30;
            this.label2.Text = "ErrorCode:";
            // 
            // tbItemName
            // 
            this.tbItemName.Location = new System.Drawing.Point(72, 8);
            this.tbItemName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbItemName.Name = "tbItemName";
            this.tbItemName.Size = new System.Drawing.Size(164, 21);
            this.tbItemName.TabIndex = 33;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 10);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 32;
            this.label3.Text = "item name:";
            // 
            // cbIsNeedTest
            // 
            this.cbIsNeedTest.AutoSize = true;
            this.cbIsNeedTest.Location = new System.Drawing.Point(17, 117);
            this.cbIsNeedTest.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbIsNeedTest.Name = "cbIsNeedTest";
            this.cbIsNeedTest.Size = new System.Drawing.Size(102, 16);
            this.cbIsNeedTest.TabIndex = 34;
            this.cbIsNeedTest.Text = "Is Need Test?";
            this.cbIsNeedTest.UseVisualStyleBackColor = true;
            // 
            // TestNodePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbIsNeedTest);
            this.Controls.Add(this.tbItemName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbErrorCode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbUnit);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tbUpper);
            this.Controls.Add(this.tbLower);
            this.Controls.Add(this.label12);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "TestNodePanel";
            this.Size = new System.Drawing.Size(243, 140);
            this.Load += new System.EventHandler(this.TestNodePanel_Load);
            this.Leave += new System.EventHandler(this.TestNodePanel_Leave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbUpper;
        private System.Windows.Forms.TextBox tbLower;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbUnit;
        private System.Windows.Forms.TextBox tbErrorCode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbItemName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbIsNeedTest;
    }
}
