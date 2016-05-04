//=============================================================================
// Main UI function. Start from Program.cs and enter Form1_load 
//=============================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using System.Diagnostics;

using FlyCapture2Managed;
using FlyCapture2Managed.Gui;

using AForge;
using AForge.Imaging;
using AForge.Math;

using DUTclass;

namespace X2DisplayTest
{
    public partial class Form_Config : Form
    {
        //colorimeter parameters
        private bool m_flagExit;
        private bool m_flagAutoMode;
        private bool m_flagCa310Mode;
        private bool m_flagFan;
        private Colorimeter m_colorimeter;        
        //private Config m_config;
        private TabPage m_preTabPage;
        private Thread m_process;

        private Config config;
        private Engine engine;
        private System.Drawing.Image preImage;

        private PointF m_ptStart, m_ptStop;

        private XMLManage xml;
        private Testlog log = new Testlog();
        private Fixture fixture;
        private IntegratingSphere integrate;
        private Ca310Pipe ca310Pipe;

        private string serialNumber;
        private uint testNum;

        //dut setup
        DUTclass.DUT dut = new DUTclass.Hodor();
        imagingpipeline ip = new imagingpipeline();

        private float calibExposure;

        private bool m_grabImages;
        private List<IntPoint> flagPoints, displaycornerPoints;
        
        
        //test setup
        private bool isdemomode = false; //Demo mode can only be used for analysis tab.
        private bool istestimagelock = false; // Lock the picturebox_test or not
        DateTime timezero = DateTime.Now;
        int systemidletime = 1500; // in millisecond

        //log setup
        string currentdirectory = System.IO.Directory.GetCurrentDirectory();               // current working folder
        string tempdirectory = System.IO.Directory.GetCurrentDirectory() + "\\temp\\";     // temprary folder. Will clean after one test iteration
        string logdirectory = System.IO.Directory.GetCurrentDirectory()  + "\\log\\";      // facrtory test logs. Pass/fail.
        string debugdirectory = System.IO.Directory.GetCurrentDirectory() + "\\debug\\";   // factory test station debug logs 
        string rawdirectory = System.IO.Directory.GetCurrentDirectory() + "\\raw\\";       // raw test logs including important test images.
        string summarydirectory = System.IO.Directory.GetCurrentDirectory() + "\\log\\summary\\";   // summary logs. 
        
        
        //colorimeter setup
        private bool useSoftwareTrigger = true;

        //log setup
        string str_DateTime = string.Format("{0:yyyyMMdd}" + "{0:HHmmss}", DateTime.Now, DateTime.Now);

        //test items
        double whitelv, blacklv, contrast; // luminance of white, black and contrast 
        double whiteuniformity5, whiteuniformity13; // uniformity of white state, 5 pt and 13 pt standard
        double wx, wy, rx, ry, gx, gy, bx, by, gamutarea; // color tristimulus values of RGB at CIE1931
        double whitemura, blackmura; // mura at white and black state

        // test data
        double[,] CIE_Y, CIE_x, CIE_y;  //CIE 1931 x, y
        double[, ,] RGB, XYZ;
        int zonesize = 10; // 10mm for now. 
        double[, ,] XYZzone; // used to represent the zone size XYZ array
        private bool pf;  //final pass/fail

        public Form_Config()
        {
            InitializeComponent();
            Form.CheckForIllegalCrossThreadCalls = false;
            picturebox_config.SizeMode = PictureBoxSizeMode.StretchImage;
            serialNumber = "";
        }

        // UI related
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Hide();
            m_preTabPage = Tabs.TabPages[0];

            try {
                config = new Config(@".\profile.ini");

                ModeSwithDlg dlg = new ModeSwithDlg();
                dlg.ShowDialog();
                isdemomode = dlg.IsAnalysisPanel;

                if (!isdemomode) {
                    engine = new Engine(config);
                    engine.Initilazie();
                    engine.dataChange += new DataDelegate(engine_dataChange);
                    engine.tableDataChange += new TableViewDelegate(engine_tableDataChange);

                    FlushDataGridView(engine.Items);

                    if (config.IsOnlineShopfloor) {
                        tbSFCMode.Text = "Online";
                        tbSFCMode.ForeColor = Color.White;
                        tbSFCMode.BackColor = Color.Green;
                    }
                    else {
                        tbSFCMode.Text = "Offline";
                        tbSFCMode.ForeColor = Color.DarkRed;
                        tbSFCMode.BackColor = SystemColors.Info;                        
                    }

                    tsslProduct.Text = "Product: " + engine.Dut.ToString();
                    tbox_sn.Enabled = config.IsScanSerialNumber;

                    if (engine.TestMode == TestMode.Automatic) {
                        rbtn_auto.Checked = true;
                        dgvCa310Data.Visible = false;
                    }
                    else if (engine.TestMode == TestMode.Ca310) {
                        rbtn_Ca310.Checked = true;
                        dgvCa310Data.Visible = true;
                    }
                    else if (engine.TestMode == TestMode.Manual) {
                        rbtn_manual.Checked = true;
                        dgvCa310Data.Visible = false;
                    }
                }
                else {
                    Tabs.SelectedTab = tab_Analysis;
                }
            }
            catch (IndexOutOfRangeException)
            {
                MessageBox.Show("Cann't find colorimeter.");
                Application.Exit();
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(ex.Message);
                Application.Exit();
            }
            catch
            {
                MessageBox.Show("Unexpect exception.");
            }

            this.Show();
            tbox_sn.Focus();
        }

        private void  engine_tableDataChange(object sender, TableViewEventArgs args)
        {
            if (args.CurrentDevice == "Camera") {
                dgvData.Rows[args.Index].SetValues(args.Items[args.Index].TestName, 
                    args.Items[args.Index].TestNodes[0].Value, args.Items[args.Index].TestNodes[1].Value,
                    args.Items[args.Index].TestNodes[2].Value, args.Items[args.Index].TestNodes[3].Value, 
                    args.Items[args.Index].TestNodes[4].Value, args.Items[args.Index].TestNodes[5].Value);
            }
            else if (args.CurrentDevice == "Ca310") {
                dgvCa310Data.Rows[args.Index].SetValues(args.Items[args.Index].TestName,
                        args.Items[args.Index].TestNodes[3].Value, args.Items[args.Index].TestNodes[4].Value, 
                        args.Items[args.Index].TestNodes[5].Value);
            }
        }

        private void engine_dataChange(object sender, DataChangeEventArgs args)
        {
            sslStatus.Text = args.StatusInfo;
            tbox_ccdtemp.Text = args.CCDTemperature.ToString();
            tbox_uptime.Text = String.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                args.Uptime.Hours, args.Uptime.Minutes, args.Uptime.Seconds);
            tbox_colorimeterstatus.Text = args.CCDStatusInfo;

            if (args.Image != null && args.Image != preImage) {
                picturebox_test.Image = args.Image;
                preImage = args.Image;
            }

            if (engine.IsDutReady) {
                tbox_dut_connect.Text = "Connected";
                tbox_dut_connect.ForeColor = Color.White;
                tbox_dut_connect.BackColor = Color.Green;
            }
            else {
                tbox_dut_connect.Text = "TBD";
                tbox_dut_connect.ForeColor = Color.Black;
                tbox_dut_connect.BackColor = Color.FromArgb(244, 244, 244);
            }

            if (engine.IsShopFlowReady) {
                tbox_shopfloor.Text = "Connected";
                tbox_shopfloor.ForeColor = Color.White;
                tbox_shopfloor.BackColor = Color.Green;
            }
            else {
                tbox_shopfloor.Text = "TBD";
                tbox_shopfloor.ForeColor = Color.Black;
                tbox_shopfloor.BackColor = Color.FromArgb(244, 244, 244);
            }
        }

        private void FlushDataGridView(List<TestItem> items)
        {
            dgvData.Rows.Clear();
            dgvCa310Data.Rows.Clear();

            dgvData.Rows.Add(items.Count);
            dgvCa310Data.Rows.Add(items.Count);

            for (int i = 0; i < items.Count; i++)
            {
                dgvData.Rows[i].SetValues(items[i].TestName, items[i].TestNodes[0].Value, items[i].TestNodes[1].Value, 
                    items[i].TestNodes[2].Value, items[i].TestNodes[3].Value, items[i].TestNodes[4].Value, items[i].TestNodes[5].Value);

                if (engine.TestMode == TestMode.Ca310)
                {
                    dgvCa310Data.Rows[i].SetValues(items[i].TestName, 
                        items[i].TestNodes[3].Value, items[i].TestNodes[4].Value, items[i].TestNodes[5].Value);
                }
            }
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            if (btn_start.Text == "Start") {
                engine.Start();
                btn_start.Text = "Stop";
            }
            else if (btn_start.Text == "Stop") {
                engine.Stop();
                btn_start.Text = "Start";
            }
        }

        // mode choice
        private void TestMode_Changed(object sender, EventArgs e)
        {
            //fixture.Reset();
            engine.Stop();
            btn_start.Text = "Start";

            RadioButton mode = sender as RadioButton;

            if (mode.Checked && mode.Text == "Manual")
            {
                sslMode.Text = "Manual mode";
                engine.TestMode = TestMode.Manual;
                dgvCa310Data.Visible = false;
            }
            else if (mode.Checked && mode.Text == "Automatic")
            {
                sslMode.Text = "Automatic mode";
                engine.TestMode = TestMode.Automatic;
                dgvCa310Data.Visible = false;
            }
            else if (mode.Checked && mode.Text == "Ca310")
            {
                sslMode.Text = "Ca-310 mode";
                engine.TestMode = TestMode.Ca310;
                dgvCa310Data.Visible = true;
                FlushDataGridView(engine.Items);
            }
        }

        private void tsbtnSetting_Click(object sender, EventArgs e)
        {
            FrmLogin login = new FrmLogin();

            if (DialogResult.OK == login.ShowDialog())
            {
                login.Close();
                FrmSetting setDlg = new FrmSetting(config, engine.Items);
                setDlg.ShowDialog();
                engine.Dut = setDlg.ActiveDUT;
                tsslProduct.Text = "Product: " + engine.Dut.ToString();
            }
        }

        // tab page select
        private void Tabs_Selecting(object sender, TabControlCancelEventArgs e)
        {
            TabControl tabControl = sender as TabControl;

            if ((tabControl.SelectedTab != m_preTabPage))
            {
                if (tabControl.SelectedTab == tabControl.TabPages[0]) {
                    m_preTabPage = tabControl.SelectedTab;
                }
                else {
                    engine.Stop();
                    rbtn_manual.Checked = true;

                    FrmLogin login = new FrmLogin();

                    if (DialogResult.OK == login.ShowDialog()) {
                        m_preTabPage = tabControl.SelectedTab;
                        Btn_Lv.Enabled = Btn_Size.Enabled = Btn_Color.Enabled = Btn_FF.Enabled = false;
                        btn_focus.Enabled = true;
                    }
                    else {
                        tabControl.SelectedTab = m_preTabPage;
                    }
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                engine.Exit();
                toolStripButtonStop.PerformClick();       
            }
            catch (FC2Exception)
            {
                // Nothing to do here
            }
            catch (NullReferenceException)
            {
                // Nothing to do here
            }
        }

        #region calibration event

        private System.Drawing.PointF ptFirstLine;
        private bool isFirstLine = true;

        private void Tabs_Selected(object sender, TabControlEventArgs e)
        {
            TabControl page = (TabControl)sender;

            if (page.SelectedTab == Tab_Config) {
                Btn_Size.Enabled = Btn_Lv.Enabled = Btn_Color.Enabled = Btn_FF.Enabled = false;
                lbMM.Visible = lbTips.Visible = tbSizeCal.Visible = false;
                engine.Video(picturebox_config, true);
            }
            else {
                engine.Video(picturebox_config, false);
            }
        }

        private void btn_focus_Click(object sender, EventArgs e)
        {
            lbCalibration.Text = "Focus calibration...";
            new Action(delegate() {
                engine.FocusCalibration();
                this.Invoke(new Action(delegate()
                {
                    MessageBox.Show("Adjust Colorimeter Focus");
                    Btn_Size.Enabled = true;
                    btn_focus.Enabled = false;
                    m_colorimeter.StopVideo();
                }));
            }).BeginInvoke(null, null);
        }

        private void Btn_Size_Click(object sender, EventArgs e)
        {
            int x = (picturebox_config.Location.X + picturebox_config.Width) / 2;
            int y = (picturebox_config.Location.Y + picturebox_config.Height) / 2;
            Cursor.Position = this.PointToScreen(new System.Drawing.Point(x, y));
            this.picturebox_config.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picturebox_config_MouseClick);
        }

        private void picturebox_config_MouseClick(object sender, MouseEventArgs e)
        {
            if (isFirstLine) {
                this.picturebox_config.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picturebox_config_MouseMove);
            }
            else {
                this.picturebox_config.MouseMove -= new System.Windows.Forms.MouseEventHandler(this.picturebox_config_MouseMove);
            }

            if (picturebox_config.Image != null) {
                Pen pen = new Pen(Color.Red, 3);
                Graphics g = Graphics.FromImage(picturebox_config.Image);
                System.Drawing.PointF pt = picturebox_config.PointToClient(Cursor.Position);
                ptFirstLine = new PointF(2016.0f / picturebox_config.Width * pt.X, 2016.0f / picturebox_config.Height * pt.Y);
                g.DrawLine(pen, new PointF(ptFirstLine.X, 0), new PointF(ptFirstLine.X, 2016));
                picturebox_config.Invalidate();

                if (isFirstLine) {
                    m_ptStart = ptFirstLine;
                }
                else {
                    m_ptStop = ptFirstLine;
                }
            }
            isFirstLine = !isFirstLine;

            // 3条线已画完
            if (isFirstLine) {
                this.picturebox_config.MouseClick -= new System.Windows.Forms.MouseEventHandler(this.picturebox_config_MouseClick);
                lbMM.Visible = lbTips.Visible = tbSizeCal.Visible = true;
                tbSizeCal.Focus();
            }
        }

        private void picturebox_config_MouseMove(object sender, MouseEventArgs e)
        {
            if (picturebox_config.Image != null) {
                Pen pen = new Pen(Color.Red, 3);
                Graphics g = Graphics.FromImage(picturebox_config.Image);
                System.Drawing.PointF pt = picturebox_config.PointToClient(Cursor.Position);
                pt = new PointF(2016.0f / picturebox_config.Width * pt.X, 2016.0f / picturebox_config.Height * pt.Y);
                g.DrawLine(pen, new PointF(ptFirstLine.X, ptFirstLine.Y), new PointF(pt.X, ptFirstLine.Y));
                picturebox_config.Invalidate();
            }
        }

        private void picturebox_config_MouseHover(object sender, EventArgs e)
        {
            System.Drawing.Point ptCursor = picturebox_config.PointToClient(Cursor.Position);

            if (ptCursor.X <= 0)
            {
                ptCursor.X = 0;
            }
            if (ptCursor.X >= picturebox_config.Width)
            {
                ptCursor.X = picturebox_config.Width;
            }
            if (ptCursor.Y <= 0)
            {
                ptCursor.Y = 0;
            }
            if (ptCursor.Y >= picturebox_config.Height)
            {
                ptCursor.Y = picturebox_config.Height;
            }
        }

        private void tbSizeCal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                double value;

                if (!double.TryParse(tbSizeCal.Text.TrimEnd('\n'), out value))
                {
                    MessageBox.Show("Please type a number.");
                    tbSizeCal.Text = "";
                    return;
                }
                lbMM.Visible = lbTips.Visible = tbSizeCal.Visible = false;
                btn_focus.Enabled = Btn_Size.Enabled = Btn_Color.Enabled = Btn_FF.Enabled = false;
                Btn_Lv.Enabled = true;
                float pixel = m_ptStop.X - m_ptStart.X;
                xml.SaveSizeCalibrationValue(pixel / value);
            }
        }

        private void Btn_Lv_Click(object sender, EventArgs e)
        {
            lbCalibration.Text = "Luminance calibration...";
            btn_focus.Enabled = Btn_Size.Enabled = Btn_Lv.Enabled = Btn_FF.Enabled = Btn_Color.Enabled = false;

            new Action(delegate()
            {
                if (ca310Pipe == null)
                {
                    ca310Pipe = new Ca310Pipe(Application.StartupPath);
                    sslStatus.Text = "Initilaze Ca310 device.";

                    if (!ca310Pipe.Connect())
                    {
                        sslStatus.Text = ca310Pipe.ErrorMessage;
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Please switch the Ca310 to init mode.");
                        sslStatus.Text = "Ca310 has Connected.";
                        ca310Pipe.ResetZero();
                        MessageBox.Show("Please switch the Ca310 to measure mode.");
                    }
                }

                LuminanceCalibration lv = new LuminanceCalibration(ca310Pipe, fixture, m_colorimeter, integrate);
                lv.SetVideoCavaus(picturebox_config);
                lv.Calibration(1023);
                calibExposure = lv.OptimalExposure;
                xml.SaveCalibrationExposureTime(lv.OptimalExposure, "White");

                float blackTime = lv.CalExposureTime(lv.OptimalExposure, new int[] { 0, 0, 0 });
                float redTime = lv.CalExposureTime(lv.OptimalExposure, new int[] { 255, 0, 0 });
                float greenTime = lv.CalExposureTime(lv.OptimalExposure, new int[] { 0, 255, 0 });
                float blueTime = lv.CalExposureTime(lv.OptimalExposure, new int[] { 0, 0, 255 });
                xml.SaveCalibrationExposureTime(blackTime, "Black");
                xml.SaveCalibrationExposureTime(redTime, "Red");
                xml.SaveCalibrationExposureTime(greenTime, "Green");
                xml.SaveCalibrationExposureTime(blueTime, "Blue");
                xml.LoadScript();


                this.Invoke(new Action(delegate()
                {
                    btn_focus.Enabled = Btn_Size.Enabled = Btn_Lv.Enabled = Btn_FF.Enabled = false;
                    Btn_Color.Enabled = true;
                    lbCalibration.Text = "Luminance calibration finish.";
                }));
            }).BeginInvoke(null, null);
        }

        private void Btn_Color_Click(object sender, EventArgs e)
        {
            lbCalibration.Text = "Color calibration...";
            btn_focus.Enabled = Btn_Size.Enabled = Btn_Lv.Enabled = Btn_Color.Enabled = Btn_FF.Enabled = false;

            new Action(delegate()
            {
                ICalibration clrCalibration = new ColorCalibration(dut, ca310Pipe, fixture, m_colorimeter);
                clrCalibration.SerialNumber = serialNumber;
                clrCalibration.Calibration(calibExposure);

                this.Invoke(new Action(delegate()
                {
                    btn_focus.Enabled = Btn_Size.Enabled = Btn_Lv.Enabled = Btn_Color.Enabled = false;
                    Btn_FF.Enabled = true;
                    lbCalibration.Text = "Color calibration finish.";
                }));
            }).BeginInvoke(null, null);
        }

        private void Btn_FF_Click(object sender, EventArgs e)
        {
            lbCalibration.Text = "Flex calibration...";
            Btn_FF.Enabled = Btn_Size.Enabled = Btn_Lv.Enabled = Btn_Color.Enabled = btn_focus.Enabled = false;

            new Action(delegate()
            {
                ICalibration flexCalib = new FlexCalibration(dut, m_colorimeter, ip);
                flexCalib.SerialNumber = serialNumber;
                flexCalib.Calibration(calibExposure);
                this.Invoke(new Action(delegate()
                {
                    Btn_Color.Enabled = Btn_Size.Enabled = Btn_Lv.Enabled = Btn_FF.Enabled = false;
                    btn_focus.Enabled = true;
                    lbCalibration.Text = "Flex calibration finish.";
                }));
            }).BeginInvoke(null, null);
        }

        #endregion

        #region events
        private void OnNewCameraClick(object sender, EventArgs e)
        {
            toolStripButtonStart.PerformClick();
            engine.Stop();
            engine = null;
            config = null;
            Form1_Load(sender, e);
        }

        private void toolStripButtonStart_Click(object sender, EventArgs e)
        {
            engine.Video(picturebox_test, true);
            toolStripButtonStart.Enabled = false;
            toolStripButtonStop.Enabled = true;
        }

        private void toolStripButtonStop_Click(object sender, EventArgs e)
        {
            engine.Video(picturebox_test, false);
            toolStripButtonStart.Enabled = true;
            toolStripButtonStop.Enabled = false;
        }

        private void toolStripButtonCameraControl_Click(object sender, EventArgs e)
        {
            int result = engine.ShowColorimeterDialog(true);

            if (result == 1) {
                tsbCameraControl.Enabled = true;
            }
            else if ( result == 2) {
                tsbCameraControl.Enabled = false;
            }
        }

        private void realSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            realSizeToolStripMenuItem.CheckState = CheckState.Checked;
            stretchToFillToolStripMenuItem.CheckState = CheckState.Unchecked;
            picturebox_test.SizeMode = PictureBoxSizeMode.Normal;
            picturebox_test.Refresh();
        }

        private void stretchToFillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            realSizeToolStripMenuItem.CheckState = CheckState.Unchecked;
            stretchToFillToolStripMenuItem.CheckState = CheckState.Checked;
            picturebox_test.SizeMode = PictureBoxSizeMode.StretchImage;
            picturebox_test.Refresh();
        }
        #endregion


        private void UpdateFormCaption(CameraInfo camInfo)
        {
            String captionString = String.Format(
                "X2 Display Test Station - {0} {1} ({2})",
                camInfo.vendorName,
                camInfo.modelName,
                camInfo.serialNumber);
            this.Text = captionString;
        }

        private void StartGrabLoop()
        {
            //m_grabThread = new BackgroundWorker();
            //m_grabThread.ProgressChanged += new ProgressChangedEventHandler(UpdateUI);
            //m_grabThread.DoWork += new DoWorkEventHandler(GrabLoop);
            //m_grabThread.WorkerReportsProgress = true;
            //m_grabThread.RunWorkerAsync();
        }






        private void tbox_sn_TextChanged(object sender, EventArgs e)
        {
            if (tbox_sn.Text.Length == 16) //fake condition. More input is needed from Square
            {
                tbox_sn.SelectAll();
                tbox_sn.Focus();
                serialNumber = tbox_sn.Text;
            }
        }

        // test prerequisite
        private bool checksnformat()
        {
            if (tbox_sn.Text.Length == 16) //fake condition. More input is needed from Square
            {
                tbox_sn.SelectAll();
                tbox_sn.Focus();
                serialNumber = tbox_sn.Text;
                return true;
            }
            else
            {
               // MessageBox.Show("Please type in 16 digit SN");
                return false;
            }
        }

        private bool checkshopfloor()
        {
            bool flag = false;
            int sfcHandle = 0;
            const ushort port = 1;
            DateTime timeNow = DateTime.Now;

            do {
                // do something to check shopfloor
                if (config.IsOnlineShopfloor) {
                    SFC.SFCInit();
                    sfcHandle = SFC.ReportStatus(serialNumber, 1);
                }
                flag = (sfcHandle == 0) ? true : false;
                //flag = true;

                if (flag) { break; }
            }
            while (DateTime.Now.Subtract(timeNow).TotalMilliseconds < 5000);

            if (flag) {
                tbox_shopfloor.Text = "OK";
                tbox_shopfloor.BackColor = Color.Green;
            }
            else {
                tbox_shopfloor.Text = "NG";
                tbox_shopfloor.BackColor = Color.Red;
            }

            return flag;
        }

        // test related 
        private bool istestmanual()
        {
            if (rbtn_manual.Checked)
            {
                return true;
            }
            else if (rbtn_auto.Checked)
            {
                return false;
            }
            else 
            {
                MessageBox.Show("Please check if DUT is in auto or manual mode");
                return true;
            }
        }
       
        private void DrawZone(Bitmap binImage, ColorPanel panel)
        {
            zoneresult zr = new zoneresult();
            Graphics g = Graphics.FromImage(binImage);

            for (int i = 1; i < 6; i++)
            {
                // get corner coordinates
                flagPoints = zr.zonecorners(i, zonesize, ip.bmp2rgb(binImage));
                // zone image
                g = zoneingimage(g, flagPoints);
                binImage.Save(tempdirectory + i.ToString() + "_" + panel.ToString() + "_bin_zone.bmp");
                flagPoints.Clear();
            }

            binImage.Save(tempdirectory + tbox_sn.Text + str_DateTime + "_" + panel.ToString() + "_bin_zone1-5.bmp");
            refreshtestimage(binImage, picturebox_test);
            g.Dispose();
        }

        private bool DisplayTest(List<IntPoint> displaycornerPoints, Bitmap bitmap, ColorPanel panelType)
        {
            // 原始图像
            string imageName = tempdirectory + tbox_sn.Text + str_DateTime + "_" + panelType.ToString() + ".bmp";
            bitmap.Save(imageName);
            //need save bmp outside as file format and reload so that 
            Bitmap srcimg = new Bitmap(System.Drawing.Image.FromFile(imageName, true));
            // 找出屏幕区域的图像
            Bitmap updateimg = croppingimage(srcimg, displaycornerPoints);
            this.refreshtestimage(updateimg, picturebox_test);

            // 截取区域图像
            Bitmap cropimg = ip.croppedimage(srcimg, displaycornerPoints, dut.ui_width, dut.ui_height);
            cropimg.Save(tempdirectory + tbox_sn.Text + str_DateTime + "_cropped.bmp");
            //picturebox_test.Width = cropimg.Width;
            //picturebox_test.Height = cropimg.Height;
            this.refreshtestimage(cropimg, picturebox_test);

            // binary 图像
            Bitmap binimg = new Bitmap(cropimg, new Size(dut.bin_width, dut.bin_height));
            binimg.Save(tempdirectory + tbox_sn.Text + str_DateTime + "_" + panelType.ToString() + "_bin.bmp");

            ColorimeterResult colorimeterRst = new ColorimeterResult(binimg, panelType);
            colorimeterRst.Analysis();

            switch (panelType)
            {
                case ColorPanel.White:                  
                    this.DrawZone(binimg, panelType);                 
                    //cbox_white_lv.Checked = cbox_white_uniformity.Checked = cbox_white_mura.Checked = true;
                    //tbox_whitelv.Text = colorimeterRst.Luminance.ToString();   
                    //tbox_whiteunif.Text = (colorimeterRst.Uniformity5 * 100).ToString();
                    //tbox_whitemura.Text = colorimeterRst.Mura.ToString();
                    log.WriteUartLog(string.Format("luminance: {0}, uniformity5: {1}, mura: {2}", 
                        colorimeterRst.Luminance, colorimeterRst.Uniformity5, colorimeterRst.Mura));
                    break;
                case ColorPanel.Black:
                    this.DrawZone(binimg, panelType);
                    //cbox_black_lv.Checked = cbox_black_uniformity.Checked = cbox_black_mura.Checked = true;
                    //tbox_blacklv.Text = colorimeterRst.Luminance.ToString();
                    //tbox_blackunif.Text = (colorimeterRst.Uniformity5 * 100).ToString();
                    //tbox_blackmura.Text = colorimeterRst.Mura.ToString();
                    log.WriteUartLog(string.Format("luminance: {0}, uniformity5: {1}, mura: {2}",
                        colorimeterRst.Luminance, colorimeterRst.Uniformity5, colorimeterRst.Mura));
                    break;
                case ColorPanel.Red:
                    //cbox_red.Checked = true;
                    //tbox_red.Text = colorimeterRst.CIE1931xyY.ToString();
                    log.WriteUartLog(colorimeterRst.CIE1931xyY.ToString());
                    break;
                case ColorPanel.Green:
                    //cbox_green.Checked = true;
                    //tbox_green.Text = colorimeterRst.CIE1931xyY.ToString();
                    log.WriteUartLog(colorimeterRst.CIE1931xyY.ToString());
                    break;
                case ColorPanel.Blue:
                    //cbox_blue.Checked = true;
                    //tbox_blue.Text = colorimeterRst.CIE1931xyY.ToString();
                    log.WriteUartLog(colorimeterRst.CIE1931xyY.ToString());
                    break;
            }
            log.WriteUartLog("\r\n");

            return this.AnaylseResult(colorimeterRst, panelType);
        }

        private bool AnaylseResult(ColorimeterResult colorimeterRst, ColorPanel panel)
        {
            bool flag = false;
            List<TestNode> nodes = null;// xml.Items[(int)panel].TestNodes.Values;

            switch (panel)
            {
                case ColorPanel.White:
                case ColorPanel.Black:
                    {
                        nodes[0].Value = colorimeterRst.Luminance;
                        nodes[1].Value = colorimeterRst.Uniformity5;
                        nodes[2].Value = colorimeterRst.Mura;

                        for (int i = 0; i < nodes.Count; i++)
                        {
                            flag &= nodes[i].Run();
                        }
                    }
                    break;
                case ColorPanel.Red:
                case ColorPanel.Green:
                case ColorPanel.Blue:
                    {
                        nodes[0].Value = colorimeterRst.CIE1931xyY.x;
                        nodes[1].Value = colorimeterRst.CIE1931xyY.y;
                        nodes[2].Value = colorimeterRst.CIE1931xyY.Y;

                        for (int i = 0; i < nodes.Count; i++)
                        {
                            flag &= nodes[i].Run();
                        }
                    }
                    break;
            }
            flag = true;

            return flag;
        }        

        private Bitmap croppingimage(Bitmap srcimg, List<IntPoint> cornerPoints)
        {
            Graphics g = Graphics.FromImage(srcimg);
            List<System.Drawing.Point> Points = new List<System.Drawing.Point>();
            foreach (var point in cornerPoints)
            {
                Points.Add(new System.Drawing.Point(point.X, point.Y));
            }
            g.DrawPolygon(new Pen(Color.Red, 15.0f), Points.ToArray());
            srcimg.Save(tempdirectory + tbox_sn.Text + str_DateTime + "_cropping.bmp");
            g.Dispose();
            return srcimg;
        }

        private Graphics zoneingimage(Graphics g, List<IntPoint> cornerPoints)
        {
            
            List<System.Drawing.Point> Points = new List<System.Drawing.Point>();
            foreach (var point in cornerPoints)
            {
                Points.Add(new System.Drawing.Point(point.X, point.Y));
            }
            g.DrawPolygon(new Pen(Color.Red, 1.0f), Points.ToArray());
            return g;
        }

        // crop the source image to the new crop bmp, returned and stored also in the temp folder

        private void refreshtestimage(Bitmap srcimg, PictureBox picturebox_flag)
        {
          // istestimagelock = false;

            if (picturebox_flag.Image != null)
            {
                picturebox_flag.Image.Dispose();
            }
            picturebox_flag.Image = srcimg;
            picturebox_flag.SizeMode = PictureBoxSizeMode.StretchImage;

            this.Refresh();

           if (picturebox_flag == picturebox_test)
           {
               istestimagelock = true;
           }

           Thread.Sleep(TimeSpan.FromMilliseconds(systemidletime));
        }

         // analysis related
        private void btn_openrawfile_Click(object sender, EventArgs e)
        {
            try
            {
                if (rbtn_colorimeter.Checked)
                {
                    //m_rawImage.Convert(FlyCapture2Managed.PixelFormat.PixelFormatBgr, m_processedImage);
                    //picturebox_raw.Image = m_processedImage.bitmap;
                }
                else if (rbtn_loadfile.Checked)
                {

                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Title = "Open Image";
                    ofd.Filter = "bmp files (*.bmp) | *.bmp";
                    if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        picturebox_raw.Refresh();
                        picturebox_raw.Image = (Bitmap)System.Drawing.Image.FromFile(ofd.FileName);

                    }
                    else
                    {
                        MessageBox.Show("Please check Image Source");
                    }
                    ofd.Dispose();
                }
                Show();
            }
            catch (FC2Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                return;
            }
        }

        private void btn_process_Click(object sender, EventArgs e)
        {
            try
            {

                if (rbtn_corner.Checked)
                {

                }
                else if (rbtn_9ptuniformity.Checked)
                {
                }
                else if (rbtn_16ptuniformity.Checked)
                {
                }
                else if (rbtn_worstzone.Checked)
                {
                }
                else if (rbtn_cropping.Checked)
                {
                    Bitmap rawimg = new Bitmap(picturebox_raw.Image);
                    //Process Image to 1bpp to increase SNR 
                    Bitmap m_orig = rawimg.Clone(new Rectangle(0, 0, rawimg.Width, rawimg.Height), System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
                    // only support the 32bppArgb for Aforge Blob Counter
                    Bitmap processbmp = m_orig.Clone(new Rectangle(0, 0, m_orig.Width, m_orig.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    ip.GetDisplayCornerfrombmp(processbmp, out displaycornerPoints);
                    Bitmap desimage = croppingimage(rawimg, displaycornerPoints);
                    refreshtestimage(desimage, pictureBox_processed);
                    Bitmap cropimage = ip.croppedimage(rawimg, displaycornerPoints, dut.ui_width, dut.ui_height);
                    refreshtestimage(cropimage, pictureBox_processed);
                }
                else if (rbtn_5zone.Checked)
                {
                    // load cropped bin image
                }
                    
                else
                {
                    MessageBox.Show("Please check processing item");
                }
            }
            catch (FC2Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                return;
            }
        }

        public static byte[] ImageToByte2(System.Drawing.Image img)
        {
            byte[] byteArray = new byte[0];
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Close();

                byteArray = stream.ToArray();
            }
            return byteArray;
        }
        
        private void btn_savedata_Click(object sender, EventArgs e)
        {
            byte[] imgdata = System.IO.File.ReadAllBytes(@"c:\v1colorimeter\src\x2displaytest\bin\Debug\temp\12345620160209232604_cropped.bmp");

            Bitmap myBitmap = new Bitmap(@"c:\v1colorimeter\src\x2displaytest\bin\Debug\temp\12345620160209232604_cropped.bmp");
            int height = myBitmap.Height;
            int width = myBitmap.Width;
            double[, ,] rgbstr = new double[width, height, 3];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    rgbstr[i, j, 0] = myBitmap.GetPixel(i, j).R;
                    rgbstr[i, j, 1] = myBitmap.GetPixel(i, j).G;
                    rgbstr[i, j, 2] = myBitmap.GetPixel(i, j).B;
                }

            }
            XYZ = ip.rgb2xyz(rgbstr);

            double sum = 0;
            double mean = 0;
            double w = XYZ.GetLength(0);
            double h = XYZ.GetLength(1);

            for (int r = 0; r < w; r++)
            {
                for (int c = 0; c < h; c++)
                {
                    sum += XYZ[r, c, 2];
                }

            }
            mean = sum / (w * h);
        }
        
        private void tbSerialNumber_TextChanged(object sender, EventArgs e)
        {
            serialNumber = "";

            if (tbSerialNumber.Text.Length == 16) //fake condition. More input is needed from Square
            {
                tbSerialNumber.SelectAll();
                tbSerialNumber.Focus();
                serialNumber = tbSerialNumber.Text;
            }
        }
    }   
}


