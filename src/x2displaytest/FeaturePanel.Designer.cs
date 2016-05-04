namespace X2DisplayTest
{
    partial class FeaturePanel
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
            this.ndExrosureTime = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.tbTestName = new System.Windows.Forms.TextBox();
            this.cbIsNeedTest = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.nudRed = new System.Windows.Forms.NumericUpDown();
            this.nudGreen = new System.Windows.Forms.NumericUpDown();
            this.nudBlue = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.btnLv = new System.Windows.Forms.Button();
            this.btnUniformity = new System.Windows.Forms.Button();
            this.btnMura = new System.Windows.Forms.Button();
            this.btnCIEx = new System.Windows.Forms.Button();
            this.btnCIEy = new System.Windows.Forms.Button();
            this.btnCIELv = new System.Windows.Forms.Button();
            this.panelItem = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.ndExrosureTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBlue)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ndExrosureTime
            // 
            this.ndExrosureTime.Location = new System.Drawing.Point(218, 101);
            this.ndExrosureTime.Margin = new System.Windows.Forms.Padding(6);
            this.ndExrosureTime.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.ndExrosureTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ndExrosureTime.Name = "ndExrosureTime";
            this.ndExrosureTime.Size = new System.Drawing.Size(130, 35);
            this.ndExrosureTime.TabIndex = 19;
            this.ndExrosureTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.ndExrosureTime.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 107);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(178, 24);
            this.label2.TabIndex = 18;
            this.label2.Text = "exposure time:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(354, 107);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 24);
            this.label1.TabIndex = 24;
            this.label1.Text = "ms";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(22, 44);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(190, 24);
            this.label8.TabIndex = 29;
            this.label8.Text = "Test item name:";
            // 
            // tbTestName
            // 
            this.tbTestName.Location = new System.Drawing.Point(218, 39);
            this.tbTestName.Name = "tbTestName";
            this.tbTestName.Size = new System.Drawing.Size(329, 35);
            this.tbTestName.TabIndex = 30;
            // 
            // cbIsNeedTest
            // 
            this.cbIsNeedTest.AutoSize = true;
            this.cbIsNeedTest.Location = new System.Drawing.Point(643, 43);
            this.cbIsNeedTest.Name = "cbIsNeedTest";
            this.cbIsNeedTest.Size = new System.Drawing.Size(198, 28);
            this.cbIsNeedTest.TabIndex = 31;
            this.cbIsNeedTest.Text = "Is need test?";
            this.cbIsNeedTest.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(26, 27);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(0, 24);
            this.label14.TabIndex = 32;
            // 
            // nudRed
            // 
            this.nudRed.Location = new System.Drawing.Point(46, 23);
            this.nudRed.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudRed.Name = "nudRed";
            this.nudRed.Size = new System.Drawing.Size(76, 35);
            this.nudRed.TabIndex = 33;
            // 
            // nudGreen
            // 
            this.nudGreen.Location = new System.Drawing.Point(200, 24);
            this.nudGreen.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudGreen.Name = "nudGreen";
            this.nudGreen.Size = new System.Drawing.Size(76, 35);
            this.nudGreen.TabIndex = 34;
            // 
            // nudBlue
            // 
            this.nudBlue.Location = new System.Drawing.Point(353, 23);
            this.nudBlue.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudBlue.Name = "nudBlue";
            this.nudBlue.Size = new System.Drawing.Size(76, 35);
            this.nudBlue.TabIndex = 35;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.nudBlue);
            this.groupBox1.Controls.Add(this.nudGreen);
            this.groupBox1.Controls.Add(this.nudRed);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Location = new System.Drawing.Point(425, 82);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(440, 67);
            this.groupBox1.TabIndex = 36;
            this.groupBox1.TabStop = false;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(315, 29);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(34, 24);
            this.label19.TabIndex = 38;
            this.label19.Text = "B:";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(165, 29);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(34, 24);
            this.label18.TabIndex = 37;
            this.label18.Text = "G:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(12, 29);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(34, 24);
            this.label17.TabIndex = 36;
            this.label17.Text = "R:";
            // 
            // btnLv
            // 
            this.btnLv.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnLv.Location = new System.Drawing.Point(8, 41);
            this.btnLv.Name = "btnLv";
            this.btnLv.Size = new System.Drawing.Size(151, 42);
            this.btnLv.TabIndex = 37;
            this.btnLv.Text = "luminance";
            this.btnLv.UseVisualStyleBackColor = true;
            this.btnLv.Click += new System.EventHandler(this.Item_Click);
            // 
            // btnUniformity
            // 
            this.btnUniformity.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnUniformity.Location = new System.Drawing.Point(8, 83);
            this.btnUniformity.Name = "btnUniformity";
            this.btnUniformity.Size = new System.Drawing.Size(151, 42);
            this.btnUniformity.TabIndex = 38;
            this.btnUniformity.Text = "uniformity";
            this.btnUniformity.UseVisualStyleBackColor = true;
            this.btnUniformity.Click += new System.EventHandler(this.Item_Click);
            // 
            // btnMura
            // 
            this.btnMura.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnMura.Location = new System.Drawing.Point(8, 125);
            this.btnMura.Name = "btnMura";
            this.btnMura.Size = new System.Drawing.Size(151, 42);
            this.btnMura.TabIndex = 39;
            this.btnMura.Text = "mura";
            this.btnMura.UseVisualStyleBackColor = true;
            this.btnMura.Click += new System.EventHandler(this.Item_Click);
            // 
            // btnCIEx
            // 
            this.btnCIEx.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCIEx.Location = new System.Drawing.Point(8, 167);
            this.btnCIEx.Name = "btnCIEx";
            this.btnCIEx.Size = new System.Drawing.Size(151, 42);
            this.btnCIEx.TabIndex = 40;
            this.btnCIEx.Text = "CIE1931.x";
            this.btnCIEx.UseVisualStyleBackColor = true;
            this.btnCIEx.Click += new System.EventHandler(this.Item_Click);
            // 
            // btnCIEy
            // 
            this.btnCIEy.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCIEy.Location = new System.Drawing.Point(8, 209);
            this.btnCIEy.Name = "btnCIEy";
            this.btnCIEy.Size = new System.Drawing.Size(151, 42);
            this.btnCIEy.TabIndex = 41;
            this.btnCIEy.Text = "CIE1931.y";
            this.btnCIEy.UseVisualStyleBackColor = true;
            this.btnCIEy.Click += new System.EventHandler(this.Item_Click);
            // 
            // btnCIELv
            // 
            this.btnCIELv.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCIELv.Location = new System.Drawing.Point(8, 251);
            this.btnCIELv.Name = "btnCIELv";
            this.btnCIELv.Size = new System.Drawing.Size(151, 42);
            this.btnCIELv.TabIndex = 42;
            this.btnCIELv.Text = "CIE1931.Lv";
            this.btnCIELv.UseVisualStyleBackColor = true;
            this.btnCIELv.Click += new System.EventHandler(this.Item_Click);
            // 
            // panelItem
            // 
            this.panelItem.Location = new System.Drawing.Point(178, 24);
            this.panelItem.Name = "panelItem";
            this.panelItem.Size = new System.Drawing.Size(657, 450);
            this.panelItem.TabIndex = 43;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panelItem);
            this.groupBox2.Controls.Add(this.btnCIELv);
            this.groupBox2.Controls.Add(this.btnCIEy);
            this.groupBox2.Controls.Add(this.btnCIEx);
            this.groupBox2.Controls.Add(this.btnMura);
            this.groupBox2.Controls.Add(this.btnUniformity);
            this.groupBox2.Controls.Add(this.btnLv);
            this.groupBox2.Location = new System.Drawing.Point(18, 151);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(847, 483);
            this.groupBox2.TabIndex = 44;
            this.groupBox2.TabStop = false;
            // 
            // FeaturePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cbIsNeedTest);
            this.Controls.Add(this.tbTestName);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ndExrosureTime);
            this.Controls.Add(this.label2);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "FeaturePanel";
            this.Size = new System.Drawing.Size(876, 647);
            this.Load += new System.EventHandler(this.FeatureParam_Load);
            this.Leave += new System.EventHandler(this.FeaturePanel_Leave);
            ((System.ComponentModel.ISupportInitialize)(this.ndExrosureTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBlue)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown ndExrosureTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbTestName;
        private System.Windows.Forms.CheckBox cbIsNeedTest;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown nudRed;
        private System.Windows.Forms.NumericUpDown nudGreen;
        private System.Windows.Forms.NumericUpDown nudBlue;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Button btnLv;
        private System.Windows.Forms.Button btnUniformity;
        private System.Windows.Forms.Button btnMura;
        private System.Windows.Forms.Button btnCIEx;
        private System.Windows.Forms.Button btnCIEy;
        private System.Windows.Forms.Button btnCIELv;
        private System.Windows.Forms.Panel panelItem;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}
