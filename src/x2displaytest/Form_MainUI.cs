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
using System.Text;
using System.Threading;
using System.Windows.Forms;

using System.Diagnostics;

using FlyCapture2Managed;
using FlyCapture2Managed.Gui;

namespace Colorimeter_Config_GUI
{
   
    public partial class Form_Config : Form
    {
        private FlyCapture2Managed.Gui.CameraControlDialog m_camCtlDlg;
        private ManagedCameraBase m_camera = null;
        private ManagedImage m_rawImage;
        private ManagedImage m_processedImage;
        private bool m_grabImages;
        private AutoResetEvent m_grabThreadExited;
        private BackgroundWorker m_grabThread;

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
            String statusString;
            TimeStamp timestamp;
            lock (this)
            {
                timestamp = m_rawImage.timeStamp;
            }

            TimeSpan cam_ontime = TimeSpan.FromSeconds(timestamp.cycleSeconds);
            statusString = String.Format("{0:D2}h:{1:D2}m.{2:D2}s",
                cam_ontime.Hours, cam_ontime.Minutes, cam_ontime.Seconds);

            tbox_uptime.Text = statusString;
            tbox_uptime.Refresh();

            return cam_ontime.Hours;

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

            if (CCDTemperature < 50 && UpTime < 24)
            {
                tbox_colorimeterstatus.Text = "OK";
                tbox_colorimeterstatus.BackColor = Color.Green;
                return true;
            }
            else
            {
                tbox_colorimeterstatus.Text = "Fail";
                tbox_colorimeterstatus.BackColor = Color.Red;
                return false;
            }
        }

// UI related

        private void Form1_Load(object sender, EventArgs e)
        {
            Hide();
            CameraSelectionDialog camSlnDlg = new CameraSelectionDialog();
            bool retVal = camSlnDlg.ShowModal();
            if (retVal)
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
            picturebox_test.Image = m_processedImage.bitmap;
            picturebox_test.Invalidate();

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
                    m_rawImage.Convert(PixelFormat.PixelFormatBgr, m_processedImage);
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
            if (tbox_sn.Text.Length == 6)
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
                //Time of start test
                DateTime testTime = DateTime.Now;
                string str_StartTestTime = string.Format("{0:yyyyMMdd}" + "{0:HHmmss}", testTime, testTime);
            }
        }


        private Rectangle GetCropRectangle(Bitmap m_prossedimage, float CropThreshold, Size FilterSize, out PointF[] cornerpointsf) 
        {
            Rectangle CropRect = new Rectangle(0, 0, int(m_processedImage.cols) - 1, int(m_processedImage.rows) - 1);
            
        }

        /*
        
        private Rectangle GetCropRectangle(Measurement m, float CropThreshold, Size FilterSize, out ROIRectangle ROIRect)
        {
            Rectangle CropRect = new Rectangle(0, 0, m.NbrCols - 1, m.NbrRows - 1);
            Measurement tmpMeas = m;

            if (FilterSize.Width > 1 || FilterSize.Height > 1)
            {
                tmpMeas = RadiantCommon20.ImageProcess.MedianFilter(ref tmpMeas, RadiantCommonCS20.TristimType.TrisY, FilterSize, 1, CropRect, -1);
            }

            CropRect = RadiantCommon20.ImageProcess.CalcClipRange(tmpMeas, MeasurementBase.TristimlusType.TrisY, ImageProcess.ThresholdMethod.PercentOfMax, CropThreshold);

            ROIRect = new ROIRectangle(CropRect);
            Pen PenColor = new Pen(Color.LightGray);
            PenColor.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            PenColor.DashPattern = new float[] { 5.0F, 2.0F };
            ROIRect.PenColor = PenColor;
            return CropRect;
        }

        private PMMeasurement CropOutUnit(PMMeasurement Meas, int Col, int Row, int PanelCols, int PanelRows, Rectangle Rect)
        {
            float DisplayWidthMM = 196f; // orig 110
            float DisplayHeightMM = 147f; // orig 70

            float GapXmm = ((Rect.Width + 1) * Meas.ScaleFactorCol * 1000 - PanelCols * DisplayWidthMM) / (PanelCols - 1);
            float GapYmm = ((Rect.Height + 1) * Meas.ScaleFactorRow * 1000 - PanelRows * DisplayHeightMM) / (PanelRows - 1);

            float w = ((DisplayWidthMM + GapXmm) / 1000f) / Meas.ScaleFactorCol;
            int x0 = Rect.Left - (int)((GapXmm / 2000f) / Meas.ScaleFactorCol);

            int xl = x0 + (int)(Col * w);
            int xr = x0 + (int)((Col + 1) * w);
            xl = Math.Max(xl, 0);
            xr = Math.Min(xr, Meas.NbrCols - 1);

            float h = ((DisplayHeightMM + GapYmm) / 1000f) / Meas.ScaleFactorRow;
            int y0 = Rect.Top - (int)((GapYmm / 2000f) / Meas.ScaleFactorRow);

            int yt = y0 + (int)(Row * h);
            int yb = y0 + (int)((Row + 1) * h);
            yt = Math.Max(yt, 0);
            yb = Math.Min(yb, Meas.NbrRows - 1);

            int AdditionalCropping = 5;

            xl += AdditionalCropping;
            xr -= AdditionalCropping;
            yt += AdditionalCropping;
            yb -= AdditionalCropping;

            Rectangle CropRect = new Rectangle(xl, yt, xr - xl, yb - yt);
            return Meas.CropOut(CropRect);
        }

         * */


        // test log



    }
}

