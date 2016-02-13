//=============================================================================
// Copyright © 2016 Microtest Inc. All Rights Reserved.
//
// This software is the confidential and proprietary information of Microtest, Inc.
// ("Confidential Information").  You shall not
// disclose such Confidential Information and shall use it only in
// accordance with the terms of the license agreement you entered into
// with PGR.
//
// PGR MAKES NO REPRESENTATIONS OR WARRANTIES ABOUT THE SUITABILITY OF THE
// SOFTWARE, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE, OR NON-INFRINGEMENT. PGR SHALL NOT BE LIABLE FOR ANY DAMAGES
// SUFFERED BY LICENSEE AS A RESULT OF USING, MODIFYING OR DISTRIBUTING
// THIS SOFTWARE OR ITS DERIVATIVES.
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
using AForge.Math.Geometry;

using DUTclass;

namespace Colorimeter_Config_GUI
{

    public partial class Form_Config : Form
    {
        //colorimeter parameters
        private FlyCapture2Managed.Gui.CameraControlDialog m_camCtlDlg;
        private ManagedCameraBase m_camera = null;
        private ManagedImage m_rawImage;
        private ManagedImage m_processedImage;
        private bool m_grabImages;
        private AutoResetEvent m_grabThreadExited;
        private BackgroundWorker m_grabThread;
        private List<IntPoint> flagPoints, displaycornerPoints;

        //test setup
        private bool isdemomode = true; //Demo mode can only be used for analysis tab.
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

        //dut setup
        DUTclass.hodor dut = new DUTclass.hodor();
        
        //log setup
        string str_DateTime = string.Format("{0:yyyyMMdd}" + "{0:HHmmss}", DateTime.Now, DateTime.Now);

        //test items
        double whitelv, blacklv, contrast; // luminance of white, black and contrast 
        double whiteuniformity5, whiteuniformity13; // uniformity of white state, 5 pt and 13 pt standard
        double wx, wy, rx, ry, gx, gy, bx, by, gamutarea; // color tristimulus values of RGB at CIE1931
        double whitemura, blackmura; // mura at white and black state

        // test data
        float[,] CIE_Y, CIE_x, CIE_y;  //CIE 1931 x, y
        float[, ,] RGB, XYZ;

        
        public Form_Config()
        {
            InitializeComponent();
            m_rawImage = new ManagedImage();
            m_processedImage = new ManagedImage();
            m_camCtlDlg = new CameraControlDialog();
            m_grabThreadExited = new AutoResetEvent(false);
            
        }


        // colorimeter status

        private double UpdateUpTime()
        {

            TimeSpan uptime = DateTime.Now.Subtract(timezero);

            string statusString = String.Format("{0:D2}h:{1:D2}m.{2:D2}s",
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
                double ccd_temp = m_camera.GetProperty(PropertyType.Temperature).valueA / 10 - 273.15;

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
            Hide();


                CameraSelectionDialog camSlnDlg = new CameraSelectionDialog();
                bool retVal = camSlnDlg.ShowModal();
                if (retVal && ! isdemomode) 
                {
                    try
                    {
                        ManagedPGRGuid[] selectedGuids = camSlnDlg.GetSelectedCameraGuids();
                        ManagedPGRGuid guidToUse = selectedGuids[0];

                        ManagedBusManager busMgr = new ManagedBusManager();
                        InterfaceType ifType = busMgr.GetInterfaceTypeFromGuid(guidToUse);

                        if (ifType == InterfaceType.GigE)
                        {
                            m_camera = new ManagedGigECamera();
                        }
                        else
                        {
                            m_camera = new ManagedCamera();
                        }

                    // Connect to the first selected GUID
                        m_camera.Connect(guidToUse);

                        m_camCtlDlg.Connect(m_camera);

                        CameraInfo camInfo = m_camera.GetCameraInfo();
                        camInfo.vendorName = "MicroTest";
                        camInfo.modelName = "v1";
                        UpdateFormCaption(camInfo);

                    // Set embedded timestamp to on
                        EmbeddedImageInfo embeddedInfo = m_camera.GetEmbeddedImageInfo();
                        embeddedInfo.timestamp.onOff = true;
                        tbox_uptime.Text = embeddedInfo.timestamp.ToString();
                        m_camera.SetEmbeddedImageInfo(embeddedInfo);
                        m_camera.StartCapture();
                        m_grabImages = true;
                        StartGrabLoop();
                    }

                catch (FC2Exception ex)
                {
                    Debug.WriteLine("Failed to load form successfully: " + ex.Message);
                    Environment.ExitCode = -1;
                    Application.Exit();
                    return;
                }

                toolStripButtonStart.Enabled = false;
                toolStripButtonStop.Enabled = true;
            }
                else if (isdemomode)
                {
                    Tabs.SelectedTab = tab_Analysis;
                    MessageBox.Show("Demo Mode with no Colorimeter. Only for Analysis", "Remind");
                }

                else
                {
                    Environment.ExitCode = -1;
                    Application.Exit();
                    return;
                }

            Show();
            tbox_sn.Focus();

        }

        private void UpdateUI(object sender, ProgressChangedEventArgs e)
        {
            UpdateStatusBar();
            UpdateCCDTemperature();
            UpdateUpTime();
            if (!istestimagelock)
            {
                picturebox_test.Image = m_processedImage.bitmap;
                picturebox_test.Invalidate();
            }
            
        }

        private void UpdateStatusBar()
        {

            String statusString;

            statusString = String.Format(
                "Image size: {0} x {1}",
                m_rawImage.cols,
                m_rawImage.rows);

            toolStripStatusLabelImageSize.Text = statusString;

            try
            {
                statusString = String.Format(
                "Requested frame rate: {0}Hz",
                m_camera.GetProperty(PropertyType.FrameRate).absValue);
            }
            catch (FC2Exception ex)
            {
                statusString = "Requested frame rate: 0.00Hz";
            }

            toolStripStatusLabelFrameRate.Text = statusString;

            TimeStamp timestamp;

            lock (this)
            {
                timestamp = m_rawImage.timeStamp;
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
                toolStripButtonStop_Click(sender, e);
                m_camera.Disconnect();
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
            m_grabThread = new BackgroundWorker();
            m_grabThread.ProgressChanged += new ProgressChangedEventHandler(UpdateUI);
            m_grabThread.DoWork += new DoWorkEventHandler(GrabLoop);
            m_grabThread.WorkerReportsProgress = true;
            m_grabThread.RunWorkerAsync();
        }

        private void GrabLoop(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            while (m_grabImages)
            {
                try
                {
                    m_camera.RetrieveBuffer(m_rawImage);
                }
                catch (FC2Exception ex)
                {
                    Debug.WriteLine("Error: " + ex.Message);
                    continue;
                }

                lock (this)
                {
                    m_rawImage.Convert(FlyCapture2Managed.PixelFormat.PixelFormatBgr, m_processedImage);
                }

                worker.ReportProgress(0);
            }

            m_grabThreadExited.Set();
        }

        private void toolStripButtonStart_Click(object sender, EventArgs e)
        {
            m_camera.StartCapture();

            m_grabImages = true;

            StartGrabLoop();

            toolStripButtonStart.Enabled = false;
            toolStripButtonStop.Enabled = true;
        }

        private void toolStripButtonStop_Click(object sender, EventArgs e)
        {
            m_grabImages = false;

            try
            {
                m_camera.StopCapture();
            }
            catch (FC2Exception ex)
            {
                Debug.WriteLine("Failed to stop camera: " + ex.Message);
            }
            catch (NullReferenceException)
            {
                Debug.WriteLine("Camera is null");
            }

            toolStripButtonStart.Enabled = true;
            toolStripButtonStop.Enabled = false;
        }

        private void toolStripButtonCameraControl_Click(object sender, EventArgs e)
        {
            if (m_camCtlDlg.IsVisible())
            {
                m_camCtlDlg.Hide();
                toolStripButtonCameraControl.Checked = false;
            }
            else
            {
                m_camCtlDlg.Show();
                toolStripButtonCameraControl.Checked = true;
            }
        }

        private void OnNewCameraClick(object sender, EventArgs e)
        {
            if (m_grabImages == true)
            {
                toolStripButtonStop_Click(sender, e);
                m_camCtlDlg.Hide();
                m_camCtlDlg.Disconnect();
                m_camera.Disconnect();
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

        private bool checkDUT()
        {
            // check DUT is inserted into fixture or not
            if (true)
            {
                tbox_dut_connect.Text = "DUT connected";
                tbox_dut_connect.BackColor = Color.Green;
                return true;
            }
            else
            {
                tbox_dut_connect.Text = "No DUT";
                tbox_dut_connect.BackColor = Color.Red;
                MessageBox.Show("DUT Not Detected", "Warning");
                return false;
            }

        }

        private bool checksnformat()
        {
            if (tbox_sn.Text.Length == 6) //fake condition. More input is needed from Square
            {
                
                return true;
            }
            else
            {
                MessageBox.Show("Please type in 6 digit SN");
                return false;
            }
        }

        private bool checkshopfloor()
        {
            tbox_shopfloor.Text = "OK";
            tbox_shopfloor.BackColor = Color.Green;
            return true;
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
            if (!colorimeterstatus())
            {
                MessageBox.Show("Reset Colorimeter");
            }
            else if (!checkDUT())
            {
                MessageBox.Show("Please insert DUT");
            }
            else if (!checkshopfloor())
            {
                MessageBox.Show("Shopfloor system is not working");
            }
            else if (string.IsNullOrEmpty(tbox_sn.Text))
            {
                MessageBox.Show("Please type SN");
            }
            else if (!checksnformat())
            {
                MessageBox.Show("SN format is wrong");
            }

            else
            {
                btn_start.Enabled = false;
                btn_start.BackColor = Color.LightBlue;

                GetDisplayCorner(m_processedImage, out displaycornerPoints);


                // Show the cropped test image in the UI;
                // Bitmap srcimg = m_processedImage.bitmap;
                m_processedImage.bitmap.Save(tempdirectory + tbox_sn.Text + str_DateTime + "_raw.bmp");

                //need save bmp outside as file format and reload so that 
                Bitmap srcimg = new Bitmap(System.Drawing.Image.FromFile(tempdirectory + tbox_sn.Text + str_DateTime + "_raw.bmp", true));
                Bitmap updateimg = croppingimage(srcimg, displaycornerPoints);

                // show cropping image
                refreshtestimage(updateimg);
                Thread.Sleep(TimeSpan.FromMilliseconds(systemidletime));

                // show cropped image
                updateimg = croppedimage(m_processedImage.bitmap);
                picturebox_test.Width = updateimg.Width;
                picturebox_test.Height = updateimg.Height;
                refreshtestimage(updateimg);
                Thread.Sleep(TimeSpan.FromMilliseconds(systemidletime));

                displaytest(displaycornerPoints);

 
            }
        }

       private float[, ,] bmp2rgb(Bitmap processedBitmap)
       {
            int h = processedBitmap.Height;
            int w = processedBitmap.Width;
            float[, ,] rgbstr = new float[w, h, 3];

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    rgbstr[i, j, 0] = processedBitmap.GetPixel(i, j).R;
                    rgbstr[i, j, 1] = processedBitmap.GetPixel(i, j).G;
                    rgbstr[i, j, 2] = processedBitmap.GetPixel(i, j).B;
                }

            }
            return rgbstr;

       }
       
       private float[, ,] rgb2xyz(float[, ,] rgbstr)
       {
           
           float [ , , ] xyzstr = rgbstr;
           // need correct CCM.
           return xyzstr;
       }

        private bool displaytest(List<IntPoint> displaycornerPoints)
        {

            // set display to desired pattern following test sequence
            m_camera.StopCapture();
            dut.setwhite();
            
            m_camera.StartCapture();

            m_camera.RetrieveBuffer(m_rawImage);
            m_rawImage.Convert(FlyCapture2Managed.PixelFormat.PixelFormatBgr, m_processedImage);

            m_processedImage.bitmap.Save(tempdirectory + tbox_sn.Text + str_DateTime + "_white.bmp");

            //need save bmp outside as file format and reload so that 
            Bitmap srcimg = new Bitmap(System.Drawing.Image.FromFile(tempdirectory + tbox_sn.Text + str_DateTime + "_white.bmp", true));
            Bitmap cropimg = croppedimage(srcimg);
            Bitmap binimg = new Bitmap(cropimg, new Size(dut.bin_width, dut.bin_height));
            
            RGB = bmp2rgb(binimg);
            XYZ = rgb2xyz(RGB);

           // byte[] imgdata = System.IO.File.ReadAllBytes(@"D:\v1colorimeter\src\x2displaytest\bin\Debug\temp\12345620160210135726_cropped.bmp");

            whitelv = getlv(XYZ);
            cbox_white_lv.Checked = true;

            whiteuniformity5 = getuniformity()[0];
            whiteuniformity13 = getuniformity()[1];
            cbox_white_uniformity.Checked = true;

            whitemura = getmura();
            cbox_white_mura.Checked = true;

            m_camera.StopCapture(); 
            dut.setblack();
            m_camera.StartCapture();
            
            m_camera.RetrieveBuffer(m_rawImage);
            m_rawImage.Convert(FlyCapture2Managed.PixelFormat.PixelFormatBgr, m_processedImage);


            m_processedImage.bitmap.Save(tempdirectory + tbox_sn.Text + str_DateTime + "_black.bmp");

            //need save bmp outside as file format and reload so that 
            srcimg = new Bitmap(System.Drawing.Image.FromFile(tempdirectory + tbox_sn.Text + str_DateTime + "_black.bmp", true));
            cropimg = croppedimage(srcimg);
            binimg = new Bitmap(cropimg, new Size(dut.bin_width, dut.bin_height));

            RGB = bmp2rgb(binimg);
            XYZ = rgb2xyz(RGB);

            blacklv = getlv(XYZ);
            cbox_black_lv.Checked = true;

            blackmura = getmura();
            cbox_black_mura.Checked = true;

            m_camera.StopCapture();
            dut.setred();
            m_camera.StartCapture();
            

            // Colorimeter to grab image 


            // divide into the data array of interests

            // load pass/fail time 

            // decide the test item pass or fail

            return true;
        }
        
        private float getlv(float[ , , ] XYZ)
        {
            float sum = 0;
            float mean = 0;
            float w = XYZ.GetLength(0);
            float h = XYZ.GetLength(1);

            for (int r = 0; r < w; r++)
            {
                for (int c = 0; c < h; c++)
                {
                    sum += XYZ[r,c, 2];
                }

            }
            mean = sum / (w * h);
            return mean;
        }

        private double[] getuniformity()
        {
            double unif5 = 1;
            double unif13 = 1;
            double[] unif = new double[]{unif5, unif13};
            return unif;
        }

        private double getmura()
        {
            double mura = 1;
            return mura;
        }


        // crop the bitmap with display rectangle inside and get 4 points list
        private void GetDisplayCorner(ManagedImage m_processedImage, out List<IntPoint> displaycornerPoints)
        {
            // get display corner position 
            //Process Image to 1bpp to increase SNR 
            Bitmap m_orig = m_processedImage.bitmap.Clone(new Rectangle(0, 0, m_processedImage.bitmap.Width, m_processedImage.bitmap.Height), System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
            // only support the 32bppArgb for Aforge Blob Counter
            Bitmap processbmp = m_orig.Clone(new Rectangle(0, 0, m_orig.Width, m_orig.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            BlobCounter bbc = new BlobCounter();
            bbc.FilterBlobs = true;
            bbc.MinHeight = 5;
            bbc.MinWidth = 5;

            bbc.ProcessImage(processbmp);

            Blob[] blobs = bbc.GetObjectsInformation();
            SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

            foreach (var blob in blobs)
            {
                List<IntPoint> edgePoints = bbc.GetBlobsEdgePoints(blob);
                List<IntPoint> cornerPoints;


                // use the shape checker to extract the corner points
                if (shapeChecker.IsQuadrilateral(edgePoints, out cornerPoints))
                {
                    // only do things if the corners from a rectangle 
                    if (shapeChecker.CheckPolygonSubType(cornerPoints) == PolygonSubType.Rectangle)
                    {
                        flagPoints = cornerPoints;
                        continue;
                    }
                    else
                    {
                        MessageBox.Show("Cannot Find the Display");
                        flagPoints = null;
 //                       picturebox_test.Image = m;
                        continue;
                    }
                }
                
            }
            displaycornerPoints = flagPoints;

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
        
        // crop the source image to the new crop bmp, returned and stored also in the temp folder
        private Bitmap croppedimage(Bitmap src)
        {
            //Create crop filter
            AForge.Imaging.Filters.SimpleQuadrilateralTransformation filter = new AForge.Imaging.Filters.SimpleQuadrilateralTransformation(displaycornerPoints, dut.ui_width, dut.ui_height);

            //Create cropped display image
            Bitmap des = filter.Apply(src);
            des.Save(tempdirectory + tbox_sn.Text + str_DateTime + "_cropped.bmp");
            return des;
        }

        private void refreshtestimage(Bitmap srcimg)
        {
          // istestimagelock = false;
           if (picturebox_test.Image != null)
           {
               picturebox_test.Image.Dispose();
           }
           picturebox_test.Image = srcimg;
           this.Refresh();
           istestimagelock = true;
        }


// analysis related
        private void btn_openrawfile_Click(object sender, EventArgs e)
        {
            try
            {
                
                if (rbtn_colorimeter.Checked)
                {
                    m_rawImage.Convert(FlyCapture2Managed.PixelFormat.PixelFormatBgr, m_processedImage);
                    picturebox_raw.Image = m_processedImage.bitmap;
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
                    Bitmap desimage = croppingimage(rawimg, displaycornerPoints);
                    pictureBox_processed.Image = desimage;
                    this.Refresh();

                    pictureBox_processed.Show();
                    

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
            byte[] imgdata = System.IO.File.ReadAllBytes(@"D:\v1colorimeter\src\x2displaytest\bin\Debug\temp\12345620160209232604_cropped.bmp");

            Bitmap myBitmap = new Bitmap(@"D:\v1colorimeter\src\x2displaytest\bin\Debug\temp\12345620160209232604_cropped.bmp");
            int height = myBitmap.Height;
            int width = myBitmap.Width;
            float[, ,] rgbstr = new float[width, height, 3];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    rgbstr[i, j, 0] = myBitmap.GetPixel(i, j).R;
                    rgbstr[i, j, 1] = myBitmap.GetPixel(i, j).G;
                    rgbstr[i, j, 2] = myBitmap.GetPixel(i, j).B;
                }

            }
            XYZ = rgb2xyz(rgbstr);

            float sum = 0;
            float mean = 0;
            float w = XYZ.GetLength(0);
            float h = XYZ.GetLength(1);

            for (int r = 0; r < w; r++)
            {
                for (int c = 0; c < h; c++)
                {
                    sum += XYZ[r, c, 2];
                }

            }
            mean = sum / (w * h);

            
            getunif(XYZ);

            
        }

        private double getunif(float [, ,] XYZ)
        {
            

            double unif5 = 1;

            return unif5;
        }

        private void btn_data_Click(object sender, EventArgs e)
        {
           
        }



    }
}


