namespace X2DisplayTest
{
    partial class FrmSetting
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
            this.tpDevice = new System.Windows.Forms.TabPage();
            this.cbDCPowerPort = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbSFC = new System.Windows.Forms.CheckBox();
            this.cbSN = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbBran = new System.Windows.Forms.RadioButton();
            this.rbHodor = new System.Windows.Forms.RadioButton();
            this.panel = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.cbDevice = new System.Windows.Forms.ComboBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.scMain = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpDevice.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).BeginInit();
            this.scMain.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tpDevice
            // 
            this.tpDevice.Controls.Add(this.cbDCPowerPort);
            this.tpDevice.Controls.Add(this.label4);
            this.tpDevice.Controls.Add(this.cbSFC);
            this.tpDevice.Controls.Add(this.cbSN);
            this.tpDevice.Controls.Add(this.groupBox1);
            this.tpDevice.Controls.Add(this.panel);
            this.tpDevice.Controls.Add(this.label2);
            this.tpDevice.Controls.Add(this.cbDevice);
            this.tpDevice.Location = new System.Drawing.Point(4, 22);
            this.tpDevice.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tpDevice.Name = "tpDevice";
            this.tpDevice.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tpDevice.Size = new System.Drawing.Size(660, 447);
            this.tpDevice.TabIndex = 2;
            this.tpDevice.Text = "parameters";
            this.tpDevice.UseVisualStyleBackColor = true;
            // 
            // cbDCPowerPort
            // 
            this.cbDCPowerPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDCPowerPort.FormattingEnabled = true;
            this.cbDCPowerPort.Location = new System.Drawing.Point(486, 184);
            this.cbDCPowerPort.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbDCPowerPort.Name = "cbDCPowerPort";
            this.cbDCPowerPort.Size = new System.Drawing.Size(164, 20);
            this.cbDCPowerPort.TabIndex = 22;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(488, 169);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 21;
            this.label4.Text = "DCPower port";
            // 
            // cbSFC
            // 
            this.cbSFC.AutoSize = true;
            this.cbSFC.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbSFC.Location = new System.Drawing.Point(486, 102);
            this.cbSFC.Name = "cbSFC";
            this.cbSFC.Size = new System.Drawing.Size(159, 18);
            this.cbSFC.TabIndex = 17;
            this.cbSFC.Text = "Shopfloor Connected";
            this.cbSFC.UseVisualStyleBackColor = true;
            // 
            // cbSN
            // 
            this.cbSN.AutoSize = true;
            this.cbSN.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbSN.Location = new System.Drawing.Point(486, 128);
            this.cbSN.Name = "cbSN";
            this.cbSN.Size = new System.Drawing.Size(152, 18);
            this.cbSN.TabIndex = 18;
            this.cbSN.Text = "Scan serial number";
            this.cbSN.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbBran);
            this.groupBox1.Controls.Add(this.rbHodor);
            this.groupBox1.Location = new System.Drawing.Point(486, 38);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(169, 37);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            // 
            // rbBran
            // 
            this.rbBran.AutoSize = true;
            this.rbBran.Location = new System.Drawing.Point(102, 17);
            this.rbBran.Name = "rbBran";
            this.rbBran.Size = new System.Drawing.Size(47, 16);
            this.rbBran.TabIndex = 14;
            this.rbBran.TabStop = true;
            this.rbBran.Text = "Bran";
            this.rbBran.UseVisualStyleBackColor = true;
            // 
            // rbHodor
            // 
            this.rbHodor.AutoSize = true;
            this.rbHodor.Location = new System.Drawing.Point(26, 17);
            this.rbHodor.Name = "rbHodor";
            this.rbHodor.Size = new System.Drawing.Size(53, 16);
            this.rbHodor.TabIndex = 14;
            this.rbHodor.TabStop = true;
            this.rbHodor.Text = "Hodor";
            this.rbHodor.UseVisualStyleBackColor = true;
            // 
            // panel
            // 
            this.panel.Location = new System.Drawing.Point(14, 38);
            this.panel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(467, 405);
            this.panel.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 12);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Device:";
            // 
            // cbDevice
            // 
            this.cbDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDevice.FormattingEnabled = true;
            this.cbDevice.Location = new System.Drawing.Point(68, 11);
            this.cbDevice.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbDevice.Name = "cbDevice";
            this.cbDevice.Size = new System.Drawing.Size(157, 20);
            this.cbDevice.TabIndex = 0;
            this.cbDevice.SelectedIndexChanged += new System.EventHandler(this.cbDevice_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.scMain);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage1.Size = new System.Drawing.Size(660, 447);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Items";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // scMain
            // 
            this.scMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.scMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.scMain.Location = new System.Drawing.Point(4, 4);
            this.scMain.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.scMain.Name = "scMain";
            this.scMain.Size = new System.Drawing.Size(652, 445);
            this.scMain.SplitterDistance = 100;
            this.scMain.SplitterWidth = 2;
            this.scMain.TabIndex = 7;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tpDevice);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(668, 473);
            this.tabControl1.TabIndex = 0;
            // 
            // FrmSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 492);
            this.Controls.Add(this.tabControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSetting";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "setting";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmSetting_FormClosing);
            this.Load += new System.EventHandler(this.FrmSetting_Load);
            this.tpDevice.ResumeLayout(false);
            this.tpDevice.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).EndInit();
            this.scMain.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tpDevice;
        private System.Windows.Forms.ComboBox cbDCPowerPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox cbSFC;
        private System.Windows.Forms.CheckBox cbSN;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbBran;
        private System.Windows.Forms.RadioButton rbHodor;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbDevice;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.SplitContainer scMain;
        private System.Windows.Forms.TabControl tabControl1;

    }
}