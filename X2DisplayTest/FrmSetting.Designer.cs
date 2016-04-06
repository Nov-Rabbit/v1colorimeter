namespace Colorimeter_Config_GUI
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
            this.cbSFC = new System.Windows.Forms.CheckBox();
            this.cbSN = new System.Windows.Forms.CheckBox();
            this.btnHolderIn = new System.Windows.Forms.Button();
            this.btnIntergeDown = new System.Windows.Forms.Button();
            this.btnIntergeUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnHolderOut = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.nudStep = new System.Windows.Forms.NumericUpDown();
            this.lbLocation = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.pnCloth = new System.Windows.Forms.Panel();
            this.btnBlue = new System.Windows.Forms.Button();
            this.btnGreen = new System.Windows.Forms.Button();
            this.btnRed = new System.Windows.Forms.Button();
            this.btnBlack = new System.Windows.Forms.Button();
            this.btnWhite = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnFanOff = new System.Windows.Forms.Button();
            this.btnFanOn = new System.Windows.Forms.Button();
            this.btnRotateOff = new System.Windows.Forms.Button();
            this.btnRotateOn = new System.Windows.Forms.Button();
            this.rbHodor = new System.Windows.Forms.RadioButton();
            this.rbBran = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudStep)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbSFC
            // 
            this.cbSFC.AutoSize = true;
            this.cbSFC.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbSFC.Location = new System.Drawing.Point(300, 30);
            this.cbSFC.Name = "cbSFC";
            this.cbSFC.Size = new System.Drawing.Size(187, 20);
            this.cbSFC.TabIndex = 0;
            this.cbSFC.Text = "Shop Floor Connected";
            this.cbSFC.UseVisualStyleBackColor = true;
            this.cbSFC.CheckedChanged += new System.EventHandler(this.cbSFC_CheckedChanged);
            // 
            // cbSN
            // 
            this.cbSN.AutoSize = true;
            this.cbSN.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbSN.Location = new System.Drawing.Point(300, 56);
            this.cbSN.Name = "cbSN";
            this.cbSN.Size = new System.Drawing.Size(171, 20);
            this.cbSN.TabIndex = 1;
            this.cbSN.Text = "Scan serial number";
            this.cbSN.UseVisualStyleBackColor = true;
            // 
            // btnHolderIn
            // 
            this.btnHolderIn.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnHolderIn.Location = new System.Drawing.Point(300, 106);
            this.btnHolderIn.Name = "btnHolderIn";
            this.btnHolderIn.Size = new System.Drawing.Size(113, 34);
            this.btnHolderIn.TabIndex = 6;
            this.btnHolderIn.Text = "holder in";
            this.btnHolderIn.UseVisualStyleBackColor = true;
            this.btnHolderIn.Click += new System.EventHandler(this.btnHolderIn_Click);
            // 
            // btnIntergeDown
            // 
            this.btnIntergeDown.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnIntergeDown.Location = new System.Drawing.Point(413, 141);
            this.btnIntergeDown.Name = "btnIntergeDown";
            this.btnIntergeDown.Size = new System.Drawing.Size(113, 34);
            this.btnIntergeDown.TabIndex = 7;
            this.btnIntergeDown.Text = "Interge down";
            this.btnIntergeDown.UseVisualStyleBackColor = true;
            this.btnIntergeDown.Click += new System.EventHandler(this.btnIntergeDown_Click);
            // 
            // btnIntergeUp
            // 
            this.btnIntergeUp.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnIntergeUp.Location = new System.Drawing.Point(413, 106);
            this.btnIntergeUp.Name = "btnIntergeUp";
            this.btnIntergeUp.Size = new System.Drawing.Size(113, 34);
            this.btnIntergeUp.TabIndex = 5;
            this.btnIntergeUp.Text = "Interge up";
            this.btnIntergeUp.UseVisualStyleBackColor = true;
            this.btnIntergeUp.Click += new System.EventHandler(this.btnIntergeUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnDown.Location = new System.Drawing.Point(166, 173);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(113, 34);
            this.btnDown.TabIndex = 4;
            this.btnDown.Text = "motor down";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnUp.Location = new System.Drawing.Point(166, 139);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(113, 34);
            this.btnUp.TabIndex = 3;
            this.btnUp.Text = "motor up";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnHolderOut
            // 
            this.btnHolderOut.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnHolderOut.Location = new System.Drawing.Point(300, 141);
            this.btnHolderOut.Name = "btnHolderOut";
            this.btnHolderOut.Size = new System.Drawing.Size(113, 34);
            this.btnHolderOut.TabIndex = 8;
            this.btnHolderOut.Text = "holder out";
            this.btnHolderOut.UseVisualStyleBackColor = true;
            this.btnHolderOut.Click += new System.EventHandler(this.btnHolderOut_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(29, 167);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 16);
            this.label1.TabIndex = 9;
            this.label1.Text = "step:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // nudStep
            // 
            this.nudStep.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nudStep.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudStep.Location = new System.Drawing.Point(80, 163);
            this.nudStep.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudStep.Name = "nudStep";
            this.nudStep.Size = new System.Drawing.Size(80, 26);
            this.nudStep.TabIndex = 2;
            // 
            // lbLocation
            // 
            this.lbLocation.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbLocation.Location = new System.Drawing.Point(29, 216);
            this.lbLocation.Name = "lbLocation";
            this.lbLocation.Size = new System.Drawing.Size(194, 24);
            this.lbLocation.TabIndex = 11;
            this.lbLocation.Text = "position: 0";
            this.lbLocation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(553, 350);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.pnCloth);
            this.tabPage1.Controls.Add(this.btnBlue);
            this.tabPage1.Controls.Add(this.btnGreen);
            this.tabPage1.Controls.Add(this.btnRed);
            this.tabPage1.Controls.Add(this.btnBlack);
            this.tabPage1.Controls.Add(this.btnWhite);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(545, 324);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "colorimeter";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // pnCloth
            // 
            this.pnCloth.Location = new System.Drawing.Point(111, 15);
            this.pnCloth.Name = "pnCloth";
            this.pnCloth.Size = new System.Drawing.Size(410, 282);
            this.pnCloth.TabIndex = 5;
            // 
            // btnBlue
            // 
            this.btnBlue.Location = new System.Drawing.Point(18, 112);
            this.btnBlue.Name = "btnBlue";
            this.btnBlue.Size = new System.Drawing.Size(75, 23);
            this.btnBlue.TabIndex = 4;
            this.btnBlue.Text = "blue";
            this.btnBlue.UseVisualStyleBackColor = true;
            this.btnBlue.Click += new System.EventHandler(this.PanelSelect_Click);
            // 
            // btnGreen
            // 
            this.btnGreen.Location = new System.Drawing.Point(18, 92);
            this.btnGreen.Name = "btnGreen";
            this.btnGreen.Size = new System.Drawing.Size(75, 23);
            this.btnGreen.TabIndex = 3;
            this.btnGreen.Text = "green";
            this.btnGreen.UseVisualStyleBackColor = true;
            this.btnGreen.Click += new System.EventHandler(this.PanelSelect_Click);
            // 
            // btnRed
            // 
            this.btnRed.Location = new System.Drawing.Point(18, 70);
            this.btnRed.Name = "btnRed";
            this.btnRed.Size = new System.Drawing.Size(75, 23);
            this.btnRed.TabIndex = 2;
            this.btnRed.Text = "red";
            this.btnRed.UseVisualStyleBackColor = true;
            this.btnRed.Click += new System.EventHandler(this.PanelSelect_Click);
            // 
            // btnBlack
            // 
            this.btnBlack.Location = new System.Drawing.Point(18, 49);
            this.btnBlack.Name = "btnBlack";
            this.btnBlack.Size = new System.Drawing.Size(75, 23);
            this.btnBlack.TabIndex = 1;
            this.btnBlack.Text = "black";
            this.btnBlack.UseVisualStyleBackColor = true;
            this.btnBlack.Click += new System.EventHandler(this.PanelSelect_Click);
            // 
            // btnWhite
            // 
            this.btnWhite.Location = new System.Drawing.Point(18, 28);
            this.btnWhite.Name = "btnWhite";
            this.btnWhite.Size = new System.Drawing.Size(75, 23);
            this.btnWhite.TabIndex = 0;
            this.btnWhite.Text = "white";
            this.btnWhite.UseVisualStyleBackColor = true;
            this.btnWhite.Click += new System.EventHandler(this.PanelSelect_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.btnFanOff);
            this.tabPage2.Controls.Add(this.btnFanOn);
            this.tabPage2.Controls.Add(this.btnRotateOff);
            this.tabPage2.Controls.Add(this.btnRotateOn);
            this.tabPage2.Controls.Add(this.btnIntergeUp);
            this.tabPage2.Controls.Add(this.lbLocation);
            this.tabPage2.Controls.Add(this.cbSFC);
            this.tabPage2.Controls.Add(this.nudStep);
            this.tabPage2.Controls.Add(this.cbSN);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.btnHolderIn);
            this.tabPage2.Controls.Add(this.btnHolderOut);
            this.tabPage2.Controls.Add(this.btnIntergeDown);
            this.tabPage2.Controls.Add(this.btnUp);
            this.tabPage2.Controls.Add(this.btnDown);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(545, 324);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "fixture";
            this.tabPage2.UseVisualStyleBackColor = true;
            this.tabPage2.Enter += new System.EventHandler(this.tabPage2_Enter);
            // 
            // btnFanOff
            // 
            this.btnFanOff.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnFanOff.Location = new System.Drawing.Point(413, 211);
            this.btnFanOff.Name = "btnFanOff";
            this.btnFanOff.Size = new System.Drawing.Size(113, 34);
            this.btnFanOff.TabIndex = 13;
            this.btnFanOff.Text = "fan off";
            this.btnFanOff.UseVisualStyleBackColor = true;
            // 
            // btnFanOn
            // 
            this.btnFanOn.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnFanOn.Location = new System.Drawing.Point(413, 176);
            this.btnFanOn.Name = "btnFanOn";
            this.btnFanOn.Size = new System.Drawing.Size(113, 34);
            this.btnFanOn.TabIndex = 13;
            this.btnFanOn.Text = "fan on";
            this.btnFanOn.UseVisualStyleBackColor = true;
            // 
            // btnRotateOff
            // 
            this.btnRotateOff.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnRotateOff.Location = new System.Drawing.Point(300, 211);
            this.btnRotateOff.Name = "btnRotateOff";
            this.btnRotateOff.Size = new System.Drawing.Size(113, 34);
            this.btnRotateOff.TabIndex = 12;
            this.btnRotateOff.Text = "rotate off";
            this.btnRotateOff.UseVisualStyleBackColor = true;
            this.btnRotateOff.Click += new System.EventHandler(this.btnRotateOff_Click);
            // 
            // btnRotateOn
            // 
            this.btnRotateOn.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnRotateOn.Location = new System.Drawing.Point(300, 176);
            this.btnRotateOn.Name = "btnRotateOn";
            this.btnRotateOn.Size = new System.Drawing.Size(113, 34);
            this.btnRotateOn.TabIndex = 12;
            this.btnRotateOn.Text = "rotate on";
            this.btnRotateOn.UseVisualStyleBackColor = true;
            this.btnRotateOn.Click += new System.EventHandler(this.btnRotateOn_Click);
            // 
            // rbHodor
            // 
            this.rbHodor.AutoSize = true;
            this.rbHodor.Location = new System.Drawing.Point(17, 13);
            this.rbHodor.Name = "rbHodor";
            this.rbHodor.Size = new System.Drawing.Size(53, 16);
            this.rbHodor.TabIndex = 14;
            this.rbHodor.TabStop = true;
            this.rbHodor.Text = "Hodor";
            this.rbHodor.UseVisualStyleBackColor = true;
            this.rbHodor.Click += new System.EventHandler(this.ProductSelect_Click);
            // 
            // rbBran
            // 
            this.rbBran.AutoSize = true;
            this.rbBran.Location = new System.Drawing.Point(109, 13);
            this.rbBran.Name = "rbBran";
            this.rbBran.Size = new System.Drawing.Size(47, 16);
            this.rbBran.TabIndex = 14;
            this.rbBran.TabStop = true;
            this.rbBran.Text = "Bran";
            this.rbBran.UseVisualStyleBackColor = true;
            this.rbBran.Click += new System.EventHandler(this.ProductSelect_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbBran);
            this.groupBox1.Controls.Add(this.rbHodor);
            this.groupBox1.Location = new System.Drawing.Point(43, 39);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(169, 37);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            // 
            // FrmSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(577, 379);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSetting";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "setting";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmSetting_FormClosing);
            this.Load += new System.EventHandler(this.FrmSetting_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudStep)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox cbSFC;
        private System.Windows.Forms.CheckBox cbSN;
        private System.Windows.Forms.Button btnHolderIn;
        private System.Windows.Forms.Button btnIntergeDown;
        private System.Windows.Forms.Button btnIntergeUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnHolderOut;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudStep;
        private System.Windows.Forms.Label lbLocation;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnBlue;
        private System.Windows.Forms.Button btnGreen;
        private System.Windows.Forms.Button btnRed;
        private System.Windows.Forms.Button btnBlack;
        private System.Windows.Forms.Button btnWhite;
        private System.Windows.Forms.Panel pnCloth;
        private System.Windows.Forms.Button btnRotateOff;
        private System.Windows.Forms.Button btnRotateOn;
        private System.Windows.Forms.Button btnFanOff;
        private System.Windows.Forms.Button btnFanOn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbBran;
        private System.Windows.Forms.RadioButton rbHodor;
    }
}