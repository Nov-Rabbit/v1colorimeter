namespace Colorimeter_Config_GUI
{
    partial class FeatureParam
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
            this.tbLvLower = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tbLvUpper = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.ndExrosureTime = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.gbLuminance = new System.Windows.Forms.GroupBox();
            this.gbUniformity = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbUnifoUpper = new System.Windows.Forms.TextBox();
            this.tbUnifoLower = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.gbMura = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbMuraUpper = new System.Windows.Forms.TextBox();
            this.tbMuraLower = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.gbCIE1931 = new System.Windows.Forms.GroupBox();
            this.label15 = new System.Windows.Forms.Label();
            this.tbCIE1931zUpper = new System.Windows.Forms.TextBox();
            this.tbCIE1931zLower = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.tbCIE1931yUpper = new System.Windows.Forms.TextBox();
            this.tbCIE1931yLower = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.tbCIE1931xUpper = new System.Windows.Forms.TextBox();
            this.tbCIE1931xLower = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ndExrosureTime)).BeginInit();
            this.gbLuminance.SuspendLayout();
            this.gbUniformity.SuspendLayout();
            this.gbMura.SuspendLayout();
            this.gbCIE1931.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbLvLower
            // 
            this.tbLvLower.Location = new System.Drawing.Point(150, 20);
            this.tbLvLower.Name = "tbLvLower";
            this.tbLvLower.Size = new System.Drawing.Size(44, 21);
            this.tbLvLower.TabIndex = 23;
            this.tbLvLower.Text = "0";
            this.tbLvLower.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(109, 24);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(41, 12);
            this.label12.TabIndex = 22;
            this.label12.Text = "lower:";
            // 
            // tbLvUpper
            // 
            this.tbLvUpper.Location = new System.Drawing.Point(50, 20);
            this.tbLvUpper.Name = "tbLvUpper";
            this.tbLvUpper.Size = new System.Drawing.Size(44, 21);
            this.tbLvUpper.TabIndex = 21;
            this.tbLvUpper.Text = "0";
            this.tbLvUpper.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 20;
            this.label7.Text = "upper:";
            // 
            // ndExrosureTime
            // 
            this.ndExrosureTime.Location = new System.Drawing.Point(115, 9);
            this.ndExrosureTime.Name = "ndExrosureTime";
            this.ndExrosureTime.Size = new System.Drawing.Size(65, 21);
            this.ndExrosureTime.TabIndex = 19;
            this.ndExrosureTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 18;
            this.label2.Text = "exposure time:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(183, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 12);
            this.label1.TabIndex = 24;
            this.label1.Text = "ms";
            // 
            // gbLuminance
            // 
            this.gbLuminance.Controls.Add(this.label7);
            this.gbLuminance.Controls.Add(this.tbLvUpper);
            this.gbLuminance.Controls.Add(this.tbLvLower);
            this.gbLuminance.Controls.Add(this.label12);
            this.gbLuminance.Location = new System.Drawing.Point(9, 36);
            this.gbLuminance.Name = "gbLuminance";
            this.gbLuminance.Size = new System.Drawing.Size(200, 53);
            this.gbLuminance.TabIndex = 25;
            this.gbLuminance.TabStop = false;
            this.gbLuminance.Text = "luminance";
            // 
            // gbUniformity
            // 
            this.gbUniformity.Controls.Add(this.label3);
            this.gbUniformity.Controls.Add(this.tbUnifoUpper);
            this.gbUniformity.Controls.Add(this.tbUnifoLower);
            this.gbUniformity.Controls.Add(this.label4);
            this.gbUniformity.Location = new System.Drawing.Point(9, 91);
            this.gbUniformity.Name = "gbUniformity";
            this.gbUniformity.Size = new System.Drawing.Size(200, 53);
            this.gbUniformity.TabIndex = 26;
            this.gbUniformity.TabStop = false;
            this.gbUniformity.Text = "uniformity5";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 24;
            this.label3.Text = "upper:";
            // 
            // tbUnifoUpper
            // 
            this.tbUnifoUpper.Location = new System.Drawing.Point(50, 20);
            this.tbUnifoUpper.Name = "tbUnifoUpper";
            this.tbUnifoUpper.Size = new System.Drawing.Size(44, 21);
            this.tbUnifoUpper.TabIndex = 25;
            this.tbUnifoUpper.Text = "0";
            this.tbUnifoUpper.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbUnifoLower
            // 
            this.tbUnifoLower.Location = new System.Drawing.Point(150, 20);
            this.tbUnifoLower.Name = "tbUnifoLower";
            this.tbUnifoLower.Size = new System.Drawing.Size(44, 21);
            this.tbUnifoLower.TabIndex = 27;
            this.tbUnifoLower.Text = "0";
            this.tbUnifoLower.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(109, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 26;
            this.label4.Text = "lower:";
            // 
            // gbMura
            // 
            this.gbMura.Controls.Add(this.label5);
            this.gbMura.Controls.Add(this.tbMuraUpper);
            this.gbMura.Controls.Add(this.tbMuraLower);
            this.gbMura.Controls.Add(this.label6);
            this.gbMura.Location = new System.Drawing.Point(9, 147);
            this.gbMura.Name = "gbMura";
            this.gbMura.Size = new System.Drawing.Size(200, 53);
            this.gbMura.TabIndex = 27;
            this.gbMura.TabStop = false;
            this.gbMura.Text = "mura";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 20;
            this.label5.Text = "upper:";
            // 
            // tbMuraUpper
            // 
            this.tbMuraUpper.Location = new System.Drawing.Point(50, 20);
            this.tbMuraUpper.Name = "tbMuraUpper";
            this.tbMuraUpper.Size = new System.Drawing.Size(44, 21);
            this.tbMuraUpper.TabIndex = 21;
            this.tbMuraUpper.Text = "0";
            this.tbMuraUpper.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbMuraLower
            // 
            this.tbMuraLower.Location = new System.Drawing.Point(150, 20);
            this.tbMuraLower.Name = "tbMuraLower";
            this.tbMuraLower.Size = new System.Drawing.Size(44, 21);
            this.tbMuraLower.TabIndex = 23;
            this.tbMuraLower.Text = "0";
            this.tbMuraLower.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(109, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 22;
            this.label6.Text = "lower:";
            // 
            // gbCIE1931
            // 
            this.gbCIE1931.Controls.Add(this.label15);
            this.gbCIE1931.Controls.Add(this.tbCIE1931zUpper);
            this.gbCIE1931.Controls.Add(this.tbCIE1931zLower);
            this.gbCIE1931.Controls.Add(this.label16);
            this.gbCIE1931.Controls.Add(this.label11);
            this.gbCIE1931.Controls.Add(this.tbCIE1931yUpper);
            this.gbCIE1931.Controls.Add(this.tbCIE1931yLower);
            this.gbCIE1931.Controls.Add(this.label13);
            this.gbCIE1931.Controls.Add(this.label9);
            this.gbCIE1931.Controls.Add(this.tbCIE1931xUpper);
            this.gbCIE1931.Controls.Add(this.tbCIE1931xLower);
            this.gbCIE1931.Controls.Add(this.label10);
            this.gbCIE1931.Location = new System.Drawing.Point(213, 36);
            this.gbCIE1931.Name = "gbCIE1931";
            this.gbCIE1931.Size = new System.Drawing.Size(179, 164);
            this.gbCIE1931.TabIndex = 28;
            this.gbCIE1931.TabStop = false;
            this.gbCIE1931.Text = "CIE1931xyY";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(13, 115);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(59, 12);
            this.label15.TabIndex = 34;
            this.label15.Text = "upper(Y):";
            // 
            // tbCIE1931zUpper
            // 
            this.tbCIE1931zUpper.Location = new System.Drawing.Point(15, 131);
            this.tbCIE1931zUpper.Name = "tbCIE1931zUpper";
            this.tbCIE1931zUpper.Size = new System.Drawing.Size(68, 21);
            this.tbCIE1931zUpper.TabIndex = 35;
            this.tbCIE1931zUpper.Text = "0";
            this.tbCIE1931zUpper.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbCIE1931zLower
            // 
            this.tbCIE1931zLower.Location = new System.Drawing.Point(99, 132);
            this.tbCIE1931zLower.Name = "tbCIE1931zLower";
            this.tbCIE1931zLower.Size = new System.Drawing.Size(68, 21);
            this.tbCIE1931zLower.TabIndex = 37;
            this.tbCIE1931zLower.Text = "0";
            this.tbCIE1931zLower.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(97, 116);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(59, 12);
            this.label16.TabIndex = 36;
            this.label16.Text = "lower(Y):";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(13, 68);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 12);
            this.label11.TabIndex = 29;
            this.label11.Text = "upper(y):";
            // 
            // tbCIE1931yUpper
            // 
            this.tbCIE1931yUpper.Location = new System.Drawing.Point(15, 84);
            this.tbCIE1931yUpper.Name = "tbCIE1931yUpper";
            this.tbCIE1931yUpper.Size = new System.Drawing.Size(68, 21);
            this.tbCIE1931yUpper.TabIndex = 30;
            this.tbCIE1931yUpper.Text = "0";
            this.tbCIE1931yUpper.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbCIE1931yLower
            // 
            this.tbCIE1931yLower.Location = new System.Drawing.Point(99, 84);
            this.tbCIE1931yLower.Name = "tbCIE1931yLower";
            this.tbCIE1931yLower.Size = new System.Drawing.Size(68, 21);
            this.tbCIE1931yLower.TabIndex = 32;
            this.tbCIE1931yLower.Text = "0";
            this.tbCIE1931yLower.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(97, 68);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(59, 12);
            this.label13.TabIndex = 31;
            this.label13.Text = "lower(y):";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(59, 12);
            this.label9.TabIndex = 24;
            this.label9.Text = "upper(x):";
            // 
            // tbCIE1931xUpper
            // 
            this.tbCIE1931xUpper.Location = new System.Drawing.Point(15, 37);
            this.tbCIE1931xUpper.Name = "tbCIE1931xUpper";
            this.tbCIE1931xUpper.Size = new System.Drawing.Size(68, 21);
            this.tbCIE1931xUpper.TabIndex = 25;
            this.tbCIE1931xUpper.Text = "0";
            this.tbCIE1931xUpper.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbCIE1931xLower
            // 
            this.tbCIE1931xLower.Location = new System.Drawing.Point(99, 37);
            this.tbCIE1931xLower.Name = "tbCIE1931xLower";
            this.tbCIE1931xLower.Size = new System.Drawing.Size(68, 21);
            this.tbCIE1931xLower.TabIndex = 27;
            this.tbCIE1931xLower.Text = "0";
            this.tbCIE1931xLower.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(97, 22);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(59, 12);
            this.label10.TabIndex = 26;
            this.label10.Text = "lower(x):";
            // 
            // FeatureParam
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbCIE1931);
            this.Controls.Add(this.gbMura);
            this.Controls.Add(this.gbUniformity);
            this.Controls.Add(this.gbLuminance);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ndExrosureTime);
            this.Controls.Add(this.label2);
            this.Name = "FeatureParam";
            this.Size = new System.Drawing.Size(401, 208);
            this.Load += new System.EventHandler(this.FeatureParam_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ndExrosureTime)).EndInit();
            this.gbLuminance.ResumeLayout(false);
            this.gbLuminance.PerformLayout();
            this.gbUniformity.ResumeLayout(false);
            this.gbUniformity.PerformLayout();
            this.gbMura.ResumeLayout(false);
            this.gbMura.PerformLayout();
            this.gbCIE1931.ResumeLayout(false);
            this.gbCIE1931.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbLvLower;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox tbLvUpper;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown ndExrosureTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gbLuminance;
        private System.Windows.Forms.GroupBox gbUniformity;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbUnifoUpper;
        private System.Windows.Forms.TextBox tbUnifoLower;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox gbMura;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbMuraUpper;
        private System.Windows.Forms.TextBox tbMuraLower;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox gbCIE1931;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox tbCIE1931zUpper;
        private System.Windows.Forms.TextBox tbCIE1931zLower;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbCIE1931yUpper;
        private System.Windows.Forms.TextBox tbCIE1931yLower;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbCIE1931xUpper;
        private System.Windows.Forms.TextBox tbCIE1931xLower;
        private System.Windows.Forms.Label label10;
    }
}
