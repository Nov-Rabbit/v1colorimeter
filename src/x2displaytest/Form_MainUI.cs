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

namespace Colorimeter_Config_GUI
{
    public partial class Form_Config : Form
    {
        //colorimeter parameters
        private bool m_flagExit;
        private bool m_flagAutoMode;
        private Colorimeter m_colorimeter;        
        private Config m_config;
        private TabPage m_preTabPage;
        private Thread m_process;

        //dut setup
        DUTclass.DUT dut = new DUTclass.Hodor();
        imagingpipeline ip = new imagingpipeline();



        //private FlyCapture2Managed.Gui.CameraControlDialog m_camCtlDlg;
        //private ManagedCameraBase m_camera = null;
        //private ManagedImage m_rawImage;
        //private ManagedImage m_processedImage;
        private bool m_grabImages;
        //private AutoResetEvent m_grabThreadExited;
        //private BackgroundWorker m_grabThread;
        private List<IntPoint> flagPoints, displaycornerPoints;
        
        
        //test setup
        private bool isdemomode = false; //Demo mode can only be used for analysis tab.
        private bool istestimagelock = false; // Lock the picturebox_test or not
        DateTime timezero = DateTime.Now;
        int systemidletime = 1500; // in millisecond

        //log setup
        string currentdirectory = System.IO.Directory.GetCurrentDirectory();             // current working folder
        string tempdirectory = System.IO.Directory.GetCurrentDirectory() + "\\temp\\";     // temprary folder. Will clean after one test iteration
        string logdirectory = System.IO.Directory.GetCurrentDirectory()  + "\\log\\";      // facrtory test logs. Pass/fail.
        string debugdirectory = System.IO.Directory.GetCurrentDirectory() + "\\debug\\";   // factory test station debug logs 
        string rawdirectory = System.IO.Directory.GetCurrentDirectory() + "\\raw\\";       // raw test logs including important test images.
        string summarydirectory = System.IO.Directory.GetCurrentDirectory() + "\\log\\summary\\"; // summary logs. 
        
        
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
            //m_rawImage = new ManagedImage();
            //m_processedImage = new ManagedImage();
            //m_camCtlDlg = new CameraControlDialog();
            //m_grabThreadExited = new AutoResetEvent(false);
            Form.CheckForIllegalCrossThreadCalls = false;
        }

        // Online mode
        private void RunSequence()
        {
            do {
                while (!dut.checkDUT())
                {
                    sslStatus.Text = "Wait DUT.";
                }
                sslStatus.Text = "Please type in 16 digit SN";
                Thread.Sleep(500);

                while (!checksnformat())
                {
                    sslStatus.Text = "Wait type SN";
                }

                if (!checkshopfloor())
                {
                    sslStatus.Text = "Shopfloor system is not working";
                    MessageBox.Show("Shopfloor system is not working");
                }
                else
                {
                    sslStatus.Text = "DUT connected";
                    tbox_dut_connect.Text = "DUT connected";
                    tbox_dut_connect.BackColor = Color.Green;

                    btn_start.Enabled = false;
                    btn_start.BackColor = Color.LightBlue;

                    try
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            string name = Enum.GetName(typeof(ColorPanel), i);

                            if (dut.setpanelcolor(name))
                            {
                                m_colorimeter.ExposureTime = (i + 1) * 20;
                                Bitmap bitmap = m_colorimeter.GrabImage();
                                //pf &= this.DisplayTest(displaycornerPoints, bitmap, (ColorPanel)Enum.Parse(typeof(ColorPanel), name));
                                this.refreshtestimage(bitmap, picturebox_test);
                            }
                            else
                            {
                                sslStatus.Text = string.Format("Can't set panel color to {0}", name);
                                pf = false;
                                break;
                            }
                        }

                        tbox_pf.Visible = true;

                        if (pf) {
                            tbox_pf.BackColor = Color.Green;
                            tbox_pf.Text = "Pass";
                        }
                        else {
                            tbox_pf.BackColor = Color.Red;
                            tbox_pf.Text = "Fail";
                        }
                        //SFC.CreateResultFile(1, pf ? "PASS" : "FAIL");

                        //while (dut.checkDUT()) {
                        //    sslStatus.Text = "Please take out DUT.";
                        //    Thread.Sleep(50);
                        //}
                        tbox_sn.Text = "";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            while (!m_flagExit);           
        }

        // colorimeter status
        private double UpdateUpTime()
        {
            TimeSpan uptime = DateTime.Now.Subtract(timezero);

            string statusString = String.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                uptime.Hours, uptime.Minutes, uptime.Seconds);

            tbox_uptime.Text = statusString;
            tbox_uptime.Refresh();

            return uptime.Hours;
        }

        private double UpdateCCDTemperature()
        {
            String statusString;
            try
            {
                double ccd_temp = m_colorimeter.Temperature;

                try
                {
                    statusString = String.Format(ccd_temp.ToString());
                }
                catch
                {
                    statusString = "N/A";
                }
                tbox_ccdtemp.Text = statusString;
                tbox_ccdtemp.Refresh();
                return ccd_temp;
            }
            catch (FC2Exception ex)
            {
                Debug.WriteLine("Failed to load form successfully: " + ex.Message);
                Environment.ExitCode = -1;
                Application.Exit();
                return 0.0;
            }

        }

        private bool colorimeterstatus()
        {
            double CCDTemperature = UpdateCCDTemperature();
            double UpTime = UpdateUpTime();

            if (CCDTemperature < 20 && UpTime < 24)
            {
                tbox_colorimeterstatus.Text = "OK";
                tbox_colorimeterstatus.BackColor = Color.Green;
                return true;
            }
            else if (CCDTemperature < 50 && UpTime < 24)
            {
                tbox_colorimeterstatus.Text = "Warm CCD";
                tbox_colorimeterstatus.BackColor = Color.LightYellow;
                colorimeter_cooling_on();
                return true;
            
            }
            else
            {
                tbox_colorimeterstatus.Text = "Fail";
                tbox_colorimeterstatus.BackColor = Color.Red;
                MessageBox.Show("Reset Colorimeter");
                return false;
            }
        }

        private void colorimeter_cooling_on()
        {
            // Fan is on and cooling of CCD is on.
        }
        // UI related

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Hide();
            m_preTabPage = Tabs.TabPages[0];            

            if (!isdemomode)
            {
                m_colorimeter = new Colorimeter();

                if (!m_colorimeter.Connect())
                {
                    MessageBox.Show("No camera.");
                    Application.Exit();
                    return;
                }
                new Action(delegate() {
                    while (!m_flagExit) {
                        UpdateCCDTemperature();
                        UpdateUpTime();
                        UpdateStatusBar();
                        colorimeterstatus();
                        System.Threading.Thread.Sleep(100);
                        Debug.WriteLine(m_colorimeter.ExposureTime);
                    }
                }).BeginInvoke(null, null);

                m_config = new Config(Application.StartupPath + @"\profile.ini");
                m_config.ReadProfile();
            }
            else
            {
                Tabs.SelectedTab = tab_Analysis;
                MessageBox.Show("Demo Mode with no Colorimeter. Only for Analysis", "Remind");
            }

            Show();
            tbox_sn.Focus();
        }

        private void tsbtnSetting_Click(object sender, EventArgs e)
        {            
            FrmLogin login = new FrmLogin();

            if (DialogResult.OK == login.ShowDialog())
            {
                login.Close();
                FrmSetting setDlg = new FrmSetting(m_config);

                setDlg.ShowDialog();
            }
        }

        // tab page select
        private void Tabs_Selecting(object sender, TabControlCancelEventArgs e)
        {
            TabControl tabControl = sender as TabControl;

            if ((tabControl.SelectedTab != m_preTabPage) )
            {
                if (tabControl.SelectedTab == tabControl.TabPages[0])
                {
                    m_preTabPage = tabControl.SelectedTab;
                }
                else
                {
                    FrmLogin login = new FrmLogin();

                    if (DialogResult.OK == login.ShowDialog())
                    {
                        m_preTabPage = tabControl.SelectedTab;
                    }
                    else
                    {
                        tabControl.SelectedTab = m_preTabPage;
                    }
                }                
            }
        }

        // mode choice
        private void TestMode_Changed(object sender, EventArgs e)
        {
            RadioButton mode = sender as RadioButton;

            if (mode.Checked && mode.Text == "Manual")
            {
                m_flagAutoMode = false;
                sslMode.Text = "Manual mode";
                btn_start.Show();

                if (m_process != null)
                    m_process.Abort();
            }
            else if (mode.Checked && mode.Text == "Automatic")
            {
                m_flagAutoMode = true;
                sslMode.Text = "Automatic mode";
                btn_start.Hide();

                if (m_process != null)
                {
                    m_process.Abort();
                }

                m_process = new Thread(this.RunSequence)
                {
                    IsBackground = true
                };
                m_process.Start();
            }
        }

        //private void UpdateUI(object sender, ProgressChangedEventArgs e)
        //{
        //    if (!istestimagelock)
        //    {
        //        picturebox_test.Image = m_processedImage.bitmap;
        //        picturebox_test.Invalidate();
        //    }
        //}

        private void UpdateStatusBar()
        {
            String statusString;

            statusString = String.Format(
                "Image size: {0} x {1}",
                m_colorimeter.ImageSize.Width,
                m_colorimeter.ImageSize.Height);

            toolStripStatusLabelImageSize.Text = statusString;

            try
            {
                statusString = String.Format(
                "Requested frame rate: {0}Hz",
                m_colorimeter.FrameRate);
            }
            catch (FC2Exception ex) 
            {
                statusString = "Requested frame rate: 0.00Hz";
            }

            toolStripStatusLabelFrameRate.Text = statusString;

            TimeStamp timestamp;

            lock (this)
            {
                timestamp = m_colorimeter.TimeStamp;
            }

            statusString = String.Format(
                "Timestamp: {0:000}.{1:0000}.{2:0000}",
                timestamp.cycleSeconds,
                timestamp.cycleCount,
                timestamp.cycleOffset);

            toolStripStatusLabelTimestamp.Text = statusString;
            statusStrip1.Refresh();

        }

        private void UpdateFormCaption(CameraInfo camInfo)
        {

            String captionString = String.Format(
                "X2 Display Test Station - {0} {1} ({2})",
                camInfo.vendorName,
                camInfo.modelName,
                camInfo.serialNumber);
            this.Text = captionString;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                m_flagExit = true;
                m_colorimeter.Disconnect();
                toolStripButtonStop_Click(sender, e);
                //m_camera.Disconnect();
            }
            catch (FC2Exception ex)
            {
                // Nothing to do here
            }
            catch (NullReferenceException ex)
            {
                // Nothing to do here
            }
        }

        private void StartGrabLoop()
        {
            //m_grabThread = new BackgroundWorker();
            //m_grabThread.ProgressChanged += new ProgressChangedEventHandler(UpdateUI);
            //m_grabThread.DoWork += new DoWorkEventHandler(GrabLoop);
            //m_grabThread.WorkerReportsProgress = true;
            //m_grabThread.RunWorkerAsync();
        }

        private void toolStripButtonStart_Click(object sender, EventArgs e)
        {
            m_colorimeter.SetVideoCavaus(picturebox_test);
            m_colorimeter.PlayVideo();
            //m_camera.StartCapture();

            m_grabImages = true;

            //StartGrabLoop();

            toolStripButtonStart.Enabled = false;
            toolStripButtonStop.Enabled = true;
        }

        private void toolStripButtonStop_Click(object sender, EventArgs e)
        {
            m_colorimeter.StopVideo();
            m_grabImages = false;

            //try
            //{
            //    //m_camera.StopCapture();
            //}
            //catch (FC2Exception ex)
            //{
            //    Debug.WriteLine("Failed to stop camera: " + ex.Message);
            //}
            //catch (NullReferenceException)
            //{
            //    Debug.WriteLine("Camera is null");
            //}

            toolStripButtonStart.Enabled = true;
            toolStripButtonStop.Enabled = false;
        }

        private void toolStripButtonCameraControl_Click(object sender, EventArgs e)
        {
            m_colorimeter.ShowCCDControlDialog();

            //if (m_camCtlDlg.IsVisible())
            //{
            //    m_camCtlDlg.Hide();
            //    toolStripButtonCameraControl.Checked = false;
            //}
            //else
            //{
            //    m_camCtlDlg.Show();
            //    toolStripButtonCameraControl.Checked = true;
            //}
        }

        private void OnNewCameraClick(object sender, EventArgs e)
        {
            if (m_grabImages == true)
            {
                toolStripButtonStop_Click(sender, e);
                //m_camCtlDlg.Hide();
                //m_camCtlDlg.Disconnect();
                //m_camera.Disconnect();
                m_colorimeter.Disconnect();
                m_colorimeter = null;
            }

            Form1_Load(sender, e);
        }

        private void realSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {

            picturebox_test.SizeMode = PictureBoxSizeMode.Normal;
            picturebox_test.Refresh();

        }

        private void stretchToFillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            picturebox_test.SizeMode = PictureBoxSizeMode.StretchImage;
            picturebox_test.Refresh();
        }

        // test prerequisite


        private bool checksnformat()
        {
            if (tbox_sn.Text.Length == 16) //fake condition. More input is needed from Square
            {
                tbox_sn.SelectAll();
                tbox_sn.Focus();
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
            bool flag = true;
            int sfcHandle = 1;
            const uint port = 1;

            SFC.SFCInit();
            //sfcHandle = SFC.ReportStatus(tbox_sn.Text, port);

            if (sfcHandle == 1)
            {
                tbox_shopfloor.Text = "OK";
                tbox_shopfloor.BackColor = Color.Green;
            }
            else
            {
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

        
        private void btn_start_Click(object sender, EventArgs e)
        {
            tbox_pf.Visible = false;

            if (!dut.checkDUT())
            {
                tbox_dut_connect.Text = "No DUT";
                tbox_dut_connect.BackColor = Color.Red;
                MessageBox.Show("Please insert DUT");
            }
            else if (string.IsNullOrEmpty(tbox_sn.Text))
            {
                MessageBox.Show("Please type SN");
            }
            else if (!checksnformat())
            {
                MessageBox.Show("SN format is wrong");
            }
            else if (!checkshopfloor())
            {
                MessageBox.Show("Shopfloor system is not working");
            }
            else
            {
                tbox_dut_connect.Text = "DUT connected";
                tbox_dut_connect.BackColor = Color.Green;

                btn_start.Enabled = false;
                btn_start.BackColor = Color.LightBlue;

                try 
                {
                    for (int i = 0; i < 5; i++)
                    {
                        string name = Enum.GetName(typeof(ColorPanel), i);
                        dut.setpanelcolor(name);
                        m_colorimeter.ExposureTime = (float)m_config.ConfigParams[name.ToLower()][6];
                        Bitmap bitmap = m_colorimeter.GrabImage();
                        //pf &= this.DisplayTest(displaycornerPoints, bitmap, (ColorPanel)Enum.Parse(typeof(ColorPanel), name));
                        this.refreshtestimage(bitmap, picturebox_test);
                    }

                    tbox_pf.Visible = true;

                    if (pf) {
                        tbox_pf.BackColor = Color.Green;
                        tbox_pf.Text = "Pass";
                    }
                    else {
                        tbox_pf.BackColor = Color.Red;
                        tbox_pf.Text = "Fail";
                    }
                    //SFC.CreateResultFile(1, pf ? "PASS" : "FAIL");
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void DrawZone(Bitmap binImage, ColorPanel panel)
        {
            zoneresult zr = new zoneresult();
            Graphics g = Graphics.FromImage(binImage);

            for (int i = 1; i < 6; i++)
            {
                // get corner coordinates
                flagPoints = zr.zonecorners(i, zonesize, XYZ);
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
            // show cropping image
            this.refreshtestimage(bitmap, picturebox_test);

            if (panelType == ColorPanel.White)
            {
                ip.GetDisplayCornerfrombmp(bitmap, out displaycornerPoints);
            }

            // Ô­Ê¼Í¼Ïñ
           // bitmap.Save(tempdirectory + tbox_sn.Text + str_DateTime + "_raw.bmp");

            //need save bmp outside as file format and reload so that ===reload
            //Bitmap srcimg = new Bitmap(System.Drawing.Image.FromFile(tempdirectory + tbox_sn.Text + str_DateTime + "_raw.bmp", true));
            //Bitmap updateimg = croppingimage(srcimg, displaycornerPoints);
            //this.refreshtestimage(updateimg, picturebox_test);

            // show cropped image
            //updateimg = ip.croppedimage(bitmap, displaycornerPoints, dut.ui_width, dut.ui_height);
            //updateimg.Save(tempdirectory + tbox_sn.Text + str_DateTime + "_cropped.bmp");

            //picturebox_test.Width = updateimg.Width;
            //picturebox_test.Height = updateimg.Height;
            //this.refreshtestimage(updateimg, picturebox_test);

            // Ô­Ê¼Í¼Ïñ
            string imageName = tempdirectory + tbox_sn.Text + str_DateTime + "_" + panelType.ToString() + ".bmp";
            bitmap.Save(imageName);
            //need save bmp outside as file format and reload so that 
            Bitmap srcimg = new Bitmap(System.Drawing.Image.FromFile(imageName, true));
            // ÕÒ³öÆÁÄ»ÇøÓòµÄÍ¼Ïñ
            Bitmap updateimg = croppingimage(srcimg, displaycornerPoints);
            this.refreshtestimage(updateimg, picturebox_test);

            // ½ØÈ¡ÇøÓòÍ¼Ïñ
            Bitmap cropimg = ip.croppedimage(srcimg, displaycornerPoints, dut.ui_width, dut.ui_height);
            cropimg.Save(tempdirectory + tbox_sn.Text + str_DateTime + "_cropped.bmp");
            picturebox_test.Width = cropimg.Width;
            picturebox_test.Height = cropimg.Height;
            this.refreshtestimage(cropimg, picturebox_test);

            // binary Í¼Ïñ
            Bitmap binimg = new Bitmap(cropimg, new Size(dut.bin_width, dut.bin_height));
            binimg.Save(tempdirectory + tbox_sn.Text + str_DateTime + "_" + panelType.ToString() + "_bin.bmp");

            ColorimeterResult colorimeterRst = new ColorimeterResult(binimg, panelType);
            colorimeterRst.Analysis();

            switch (panelType)
            {
                case ColorPanel.White:
                    this.DrawZone(binimg, panelType);
                    cbox_white_lv.Checked = cbox_white_uniformity.Checked = cbox_white_mura.Checked = true;
                    tbox_whitelv.Text = colorimeterRst.Luminance.ToString();   
                    tbox_whiteunif.Text = (colorimeterRst.Uniformity5 * 100).ToString();
                    tbox_whitemura.Text = colorimeterRst.Mura.ToString();                    
                    break;
                case ColorPanel.Black:
                    this.DrawZone(binimg, panelType);
                    cbox_black_lv.Checked = cbox_black_uniformity.Checked = cbox_black_mura.Checked = true;
                    tbox_blacklv.Text = colorimeterRst.Luminance.ToString();
                    tbox_blackunif.Text = (colorimeterRst.Uniformity5 * 100).ToString();
                    tbox_blackmura.Text = colorimeterRst.Mura.ToString();
                    break;
                case ColorPanel.Red:
                    cbox_red.Checked = true;
                    tbox_red.Text = colorimeterRst.CIE1931xyY.ToString();
                    break;
                case ColorPanel.Green:
                    cbox_green.Checked = true;
                    tbox_green.Text = colorimeterRst.CIE1931xyY.ToString();
                    break;
                case ColorPanel.Blue:
                    cbox_blue.Checked = true;
                    tbox_blue.Text = colorimeterRst.CIE1931xyY.ToString();
                    break;
            }

            return this.AnaylseResult(colorimeterRst, panelType);
        }

        private bool AnaylseResult(ColorimeterResult colorimeterRst, ColorPanel panel)
        {
            int index = 0;
            bool flag = false;
            List<double> param = m_config.ConfigParams[panel.ToString().ToLower()];

            switch (panel)
            {
                case ColorPanel.White:
                case ColorPanel.Black:
                    {
                        flag &= (colorimeterRst.Luminance <= param[index++] && colorimeterRst.Luminance >= param[index++]);
                        flag &= (colorimeterRst.Uniformity5 <= param[index++] && colorimeterRst.Luminance >= param[index++]);
                        flag &= (colorimeterRst.Mura <= param[index++] && colorimeterRst.Mura >= param[index++]);
                    }
                    break;
                case ColorPanel.Red:
                case ColorPanel.Green:
                case ColorPanel.Blue:
                    {
                        flag &= (colorimeterRst.CIE1931xyY.x <= param[index++] && colorimeterRst.CIE1931xyY.x >= param[index++]);
                        flag &= (colorimeterRst.CIE1931xyY.y <= param[index++] && colorimeterRst.CIE1931xyY.y >= param[index++]);
                        flag &= (colorimeterRst.CIE1931xyY.Y <= param[index++] && colorimeterRst.CIE1931xyY.Y >= param[index++]);
                    }
                    break;
            }

            return flag;
        }

        //private bool displaytest(List<IntPoint> displaycornerPoints)
        //{
        //    // set display to desired pattern following test sequence
        //    m_processedImage.bitmap.Save(tempdirectory + tbox_sn.Text + str_DateTime + "_white.bmp");

        //    //need save bmp outside as file format and reload so that 
        //    Bitmap srcimg = new Bitmap(System.Drawing.Image.FromFile(tempdirectory + tbox_sn.Text + str_DateTime + "_white.bmp", true));
        //    Bitmap cropimg = ip.croppedimage(srcimg, displaycornerPoints, dut.ui_width, dut.ui_height);
        //    cropimg.Save(tempdirectory + tbox_sn.Text + str_DateTime + "_cropped.bmp");
            
        //    Bitmap binimg = new Bitmap(cropimg, new Size(dut.bin_width, dut.bin_height));
        //    binimg.Save(tempdirectory + tbox_sn.Text + str_DateTime + "_white_bin.bmp");
            
        //    RGB = ip.bmp2rgb(binimg);
        //    XYZ = ip.rgb2xyz(RGB);

        //   // byte[] imgdata = System.IO.File.ReadAllBytes(@"D:\v1colorimeter\src\x2displaytest\bin\Debug\temp\12345620160210135726_cropped.bmp");
            

        //    whitelv = ip.getlv(XYZ);
        //    cbox_white_lv.Checked = true;
        //    tbox_whitelv.Text = whitelv.ToString();

        //    //SFC.AddTestLog(1, 1, "luminance", "500", "0", whitelv.ToString(), "-");
            
        //    whiteuniformity5 = ip.getuniformity(XYZ);

        //    cbox_white_uniformity.Checked = true;
        //    double whiteuniformity5_percentage = whiteuniformity5 * 100;
        //    tbox_whiteunif.Text = whiteuniformity5_percentage.ToString();
        //    //SFC.AddTestLog(1, 1, "uniformity", "100", "0", whiteuniformity5_percentage.ToString(), "-");

        //    zoneresult zr = new zoneresult();

        //    Graphics g = Graphics.FromImage(binimg);
        //    for (int i = 1; i < 6; i++)
        //    {
        //        // get corner coordinates
                
                
        //        flagPoints = zr.zonecorners(i, zonesize, XYZ);
        //        // zone image
        //        g = zoneingimage(g, flagPoints);
        //        binimg.Save(tempdirectory + i.ToString() + "_white_bin_zone.bmp");
        //        flagPoints.Clear();
        //    }

        //    binimg.Save(tempdirectory + tbox_sn.Text + str_DateTime + "_white_bin_zone1-5.bmp");
        //    refreshtestimage(binimg, picturebox_test);
        //    g.Dispose();

        //    whitemura = ip.getmura(XYZ);
        //    cbox_white_mura.Checked = true;
        //    tbox_whitemura.Text = whitemura.ToString();

            


        //    /*
        //    dut.setblack();
        //    m_camera.StartCapture();
            
        //    m_camera.RetrieveBuffer(m_rawImage);
        //    m_rawImage.Convert(FlyCapture2Managed.PixelFormat.PixelFormatBgr, m_processedImage);

        //    m_processedImage.bitmap.Save(tempdirectory + tbox_sn.Text + str_DateTime + "_black.bmp");

        //    //need save bmp outside as file format and reload so that 
        //    srcimg = new Bitmap(System.Drawing.Image.FromFile(tempdirectory + tbox_sn.Text + str_DateTime + "_black.bmp", true));
        //    cropimg = ip.croppedimage(srcimg, displaycornerPoints, dut.ui_width, dut.ui_height);
        //    binimg = new Bitmap(cropimg, new Size(dut.bin_width, dut.bin_height));

        //    RGB = ip.bmp2rgb(binimg);
        //    XYZ = ip.rgb2xyz(RGB);

        //    blacklv = ip.getlv(XYZ);
        //    cbox_black_lv.Checked = true;
        //    tbox_blacklv.Text = blacklv.ToString();

        //    blackmura = ip.getmura(XYZ);
        //    cbox_black_mura.Checked = true;
        //    tbox_blackmura.Text = blackmura.ToString();

        //    m_camera.StopCapture();
        //    dut.setred();
        //    // will develop the color patterns later
            
        //    // load pass/fail item

        //    // decide the test item pass or fail
        //    */
        //    return true;
        //}
        

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

                    ip.GetDisplayCornerfrombmp(rawimg, out displaycornerPoints);
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

        private void btn_focus_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Adjust Colorimeter Focus");
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
    }

    public enum ColorPanel{
        White,
        Black,
        Red,
        Green,
        Blue,
    }

    public class CIE1931Value
    {
        public double x { get; set; }
        public double y { get; set; }
        public double Y { get; set; }

        public override string ToString()
        {
            return string.Format("{({0}, {1}), {2}}", x, y, Y);
        }
    }

    public class ColorimeterResult
    {
        private Bitmap m_bitmap;
        private ColorPanel m_panel;
        private imagingpipeline m_pipeline;

        public double Luminance { get; private set; }
        public double Uniformity5 { get; private set; }
        public double Mura { get; private set; }
        public CIE1931Value CIE1931xyY { get; set; }

        public ColorimeterResult(Bitmap bitmap, ColorPanel panel)
        {
            this.m_bitmap = bitmap;
            this.m_panel = panel;
            m_pipeline = new imagingpipeline();
            CIE1931xyY = new CIE1931Value();
        }

        public void Analysis()
        {
            if (m_bitmap != null)
            {
                double[, ,] rgb = m_pipeline.bmp2rgb(m_bitmap);
                double[, ,] XYZ = m_pipeline.rgb2xyz(rgb);

                switch (m_panel) { 
                    case ColorPanel.White:
                    case ColorPanel.Black:
                        this.Luminance = m_pipeline.getlv(XYZ);
                        this.Uniformity5 = m_pipeline.getuniformity(XYZ);
                        this.Mura = m_pipeline.getmura(XYZ);
                        break;
                    case ColorPanel.Red:
                    case ColorPanel.Green:
                    case ColorPanel.Blue:
                        {
                            double[] xyY = m_pipeline.getxyY(XYZ);
                            CIE1931xyY.x = xyY[0];
                            CIE1931xyY.y = xyY[1];
                            CIE1931xyY.Y = xyY[2];
                        }
                        break;
                }
            }
        }
    }

}


