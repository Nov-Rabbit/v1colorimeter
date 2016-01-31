namespace Colorimeter_Config_GUI
{
    partial class Form_Config
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Config));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonStart = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonStop = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonCameraControl = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelImageSize = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelFrameRate = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelTimestamp = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.picture_box = new System.Windows.Forms.PictureBox();
            this.Btn_Size = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.Btn_FF = new System.Windows.Forms.Button();
            this.Btn_Color = new System.Windows.Forms.Button();
            this.Btn_Lv = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picture_box)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonStart,
            this.toolStripButtonStop,
            this.toolStripSeparator1,
            this.toolStripButtonCameraControl});
            this.toolStrip1.Location = new System.Drawing.Point(0, 47);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStrip1.Size = new System.Drawing.Size(1372, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonStart
            // 
            this.toolStripButtonStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonStart.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonStart.Image")));
            this.toolStripButtonStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonStart.Name = "toolStripButtonStart";
            this.toolStripButtonStart.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonStart.Text = "Play";
            this.toolStripButtonStart.Click += new System.EventHandler(this.toolStripButtonStart_Click);
            // 
            // toolStripButtonStop
            // 
            this.toolStripButtonStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonStop.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonStop.Image")));
            this.toolStripButtonStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonStop.Name = "toolStripButtonStop";
            this.toolStripButtonStop.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonStop.Text = "Stop";
            this.toolStripButtonStop.Click += new System.EventHandler(this.toolStripButtonStop_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonCameraControl
            // 
            this.toolStripButtonCameraControl.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCameraControl.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonCameraControl.Image")));
            this.toolStripButtonCameraControl.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCameraControl.Name = "toolStripButtonCameraControl";
            this.toolStripButtonCameraControl.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonCameraControl.Text = "Controls";
            this.toolStripButtonCameraControl.Click += new System.EventHandler(this.toolStripButtonCameraControl_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelImageSize,
            this.toolStripStatusLabelFrameRate,
            this.toolStripStatusLabelTimestamp});
            this.statusStrip1.Location = new System.Drawing.Point(0, 990);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 21, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1372, 42);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelImageSize
            // 
            this.toolStripStatusLabelImageSize.Name = "toolStripStatusLabelImageSize";
            this.toolStripStatusLabelImageSize.Size = new System.Drawing.Size(59, 37);
            this.toolStripStatusLabelImageSize.Text = "0x0";
            // 
            // toolStripStatusLabelFrameRate
            // 
            this.toolStripStatusLabelFrameRate.Name = "toolStripStatusLabelFrameRate";
            this.toolStripStatusLabelFrameRate.Size = new System.Drawing.Size(99, 37);
            this.toolStripStatusLabelFrameRate.Text = "0.00Hz";
            // 
            // toolStripStatusLabelTimestamp
            // 
            this.toolStripStatusLabelTimestamp.Name = "toolStripStatusLabelTimestamp";
            this.toolStripStatusLabelTimestamp.Size = new System.Drawing.Size(244, 37);
            this.toolStripStatusLabelTimestamp.Text = "Camera not started";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(9, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1372, 47);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newCameraToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(70, 41);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newCameraToolStripMenuItem
            // 
            this.newCameraToolStripMenuItem.Name = "newCameraToolStripMenuItem";
            this.newCameraToolStripMenuItem.Size = new System.Drawing.Size(241, 42);
            this.newCameraToolStripMenuItem.Text = "New camera";
            this.newCameraToolStripMenuItem.Click += new System.EventHandler(this.OnNewCameraClick);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(238, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(241, 42);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.AutoSize = true;
            this.panel1.Location = new System.Drawing.Point(-4, 75);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(0, 0);
            this.panel1.TabIndex = 3;
            // 
            // picture_box
            // 
            this.picture_box.Location = new System.Drawing.Point(400, 123);
            this.picture_box.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.picture_box.Name = "picture_box";
            this.picture_box.Size = new System.Drawing.Size(606, 641);
            this.picture_box.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picture_box.TabIndex = 5;
            this.picture_box.TabStop = false;
            // 
            // Btn_Size
            // 
            this.Btn_Size.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_Size.Location = new System.Drawing.Point(13, 30);
            this.Btn_Size.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Btn_Size.Name = "Btn_Size";
            this.Btn_Size.Size = new System.Drawing.Size(297, 80);
            this.Btn_Size.TabIndex = 6;
            this.Btn_Size.Text = "Size Calibration";
            this.Btn_Size.UseVisualStyleBackColor = true;
            this.Btn_Size.Click += new System.EventHandler(this.Size_Calibration_Btn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(34, 108);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 33);
            this.label1.TabIndex = 7;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.panel2.Controls.Add(this.Btn_FF);
            this.panel2.Controls.Add(this.Btn_Color);
            this.panel2.Controls.Add(this.Btn_Lv);
            this.panel2.Controls.Add(this.Btn_Size);
            this.panel2.Location = new System.Drawing.Point(13, 123);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(328, 474);
            this.panel2.TabIndex = 9;
            // 
            // Btn_FF
            // 
            this.Btn_FF.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_FF.Location = new System.Drawing.Point(13, 359);
            this.Btn_FF.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Btn_FF.Name = "Btn_FF";
            this.Btn_FF.Size = new System.Drawing.Size(297, 80);
            this.Btn_FF.TabIndex = 9;
            this.Btn_FF.Text = "Flat Field Calibration";
            this.Btn_FF.UseVisualStyleBackColor = true;
            // 
            // Btn_Color
            // 
            this.Btn_Color.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_Color.Location = new System.Drawing.Point(13, 247);
            this.Btn_Color.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Btn_Color.Name = "Btn_Color";
            this.Btn_Color.Size = new System.Drawing.Size(297, 80);
            this.Btn_Color.TabIndex = 8;
            this.Btn_Color.Text = "Color Calibration";
            this.Btn_Color.UseVisualStyleBackColor = true;
            // 
            // Btn_Lv
            // 
            this.Btn_Lv.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_Lv.Location = new System.Drawing.Point(13, 137);
            this.Btn_Lv.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Btn_Lv.Name = "Btn_Lv";
            this.Btn_Lv.Size = new System.Drawing.Size(297, 80);
            this.Btn_Lv.TabIndex = 7;
            this.Btn_Lv.Text = "Luminance Calibration";
            this.Btn_Lv.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 618);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(187, 25);
            this.label3.TabIndex = 11;
            this.label3.Text = "Calibration Output";
            // 
            // Form_Config
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1372, 1032);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.picture_box);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(949, 705);
            this.Name = "Form_Config";
            this.Text = "X2 Display Test";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picture_box)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox picture_box;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelTimestamp;
        private System.Windows.Forms.ToolStripButton toolStripButtonStart;
        private System.Windows.Forms.ToolStripButton toolStripButtonStop;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonCameraControl;
        private System.Windows.Forms.ToolStripMenuItem newCameraToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelImageSize;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelFrameRate;
        private System.Windows.Forms.Button Btn_Size;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button Btn_FF;
        private System.Windows.Forms.Button Btn_Color;
        private System.Windows.Forms.Button Btn_Lv;
        private System.Windows.Forms.Label label3;
    }
}

