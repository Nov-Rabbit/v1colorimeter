//=============================================================================
// Copyright © 2010 Point Grey Research, Inc. All Rights Reserved.
//
// This software is the confidential and proprietary information of Point
// Grey Research, Inc. ("Confidential Information").  You shall not
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
//=============================================================================
// $Id: Form1.cs,v 1.4 2011-02-03 23:34:52 soowei Exp $
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
        private Graphics g;
        private bool mdraw = false;
        private Color mcolor = Color.Red;

        public Form_Config()
        {
            InitializeComponent();

            m_rawImage = new ManagedImage();
            m_processedImage = new ManagedImage();
            m_camCtlDlg = new CameraControlDialog();

            m_grabThreadExited = new AutoResetEvent(false);

            
        }

        private void UpdateTestUI(object sender, ProgressChangedEventArgs e)
        {
            String statusString;
            try
            {
                statusString = String.Format(
                    m_camera.GetProperty(PropertyType.Temperature).absValue.ToString());
            }
            catch
            {
                statusString = "N/A";
            }
            tbox_ccdtemp.Text = statusString;
            tbox_ccdtemp.Refresh();

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

            tbox_uptime.Text = statusString;
            tbox_uptime.Refresh();
            picturebox_test.Image = m_processedImage.bitmap;
            picturebox_test.Invalidate();
        }

        private void UpdateAuditUI(object sender, ProgressChangedEventArgs e)
        {
            picturebox_audit.Image = m_processedImage.bitmap;
            picturebox_audit.Invalidate();
        }

        private void UpdateConfigUI(object sender, ProgressChangedEventArgs e)
        {
            picturebox_config.Image = m_processedImage.bitmap;
            picturebox_config.Invalidate();

        }


        private void UpdateUI(object sender, ProgressChangedEventArgs e)
        {
            UpdateStatusBar();

            if (Tabs.SelectedTab == Tabs.TabPages["Tab_Test"])
            {
                UpdateTestUI(null, null);
            }
            else if (Tabs.SelectedTab == Tabs.TabPages["Tab_Audit"])
            {
                UpdateAuditUI(null, null);
            }
            else if (Tabs.SelectedTab == Tabs.TabPages["Tab_Config"])
            {
                UpdateConfigUI(null, null);
            }            
            else
            {
                MessageBox.Show("Please select running mode", "Reminder");
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
        }

        private void UpdateFormCaption(CameraInfo camInfo)
        {
            String captionString = String.Format(
                "FlyCapture2SimpleGUI_CSharp - {0} {1} ({2})",
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

        private void Size_Calibration_Btn_Click(object sender, EventArgs e)
        {
            Btn_Size.Enabled = false;
            Btn_Size.BackColor = System.Drawing.Color.LightSteelBlue;
            Btn_Color.Enabled = false;
            Btn_FF.Enabled = false;
            Btn_Lv.Enabled = false;

            // pop out the message box.
            object size_cal_msg = "Choose the DUT boundaries and type in dimension in mm";
            object size_cal_title = "Size Calibration";
            MessageBox.Show(size_cal_msg.ToString(), size_cal_title.ToString());
            
            // at cal mode, the picture freeze.
            m_grabImages = false;
            m_camera.StopCapture();

            // Mouse Down Event and Pick the Left Point

            // Mouse pressed down to draw the horizontal line

            // Mouse Up Evlent and Pick the Right Point

            // Get the relative value and calculate the size paramter

        }
        
        private void picturebox_config_MouseDown(object sender, MouseEventArgs e)
        {
            mdraw = true;
            g = Graphics.FromImage(picturebox_config.Image);
            Pen pen1 = new Pen(mcolor, 4);

            Point mouseDownLocatoion = new Point(e.X, e.Y);
            Point pointup = new Point(e.X, picturebox_config.Location.Y);
            Point pointdown = new Point(e.X, picturebox_config.Location.Y + picturebox_config.Height);
            g.DrawLine(pen1, pointup, pointdown);
            g.Save();
            picturebox_config.Image = picturebox_config.Image;
        }

        private void picturebox_config_MouseUp(object sender, MouseEventArgs e)
        {
            mdraw = false;
            Pen pen1 = new Pen(mcolor, 4);

            Point mouseUpLocatoion = new Point(e.X, e.Y);
            Point pointup = new Point(e.X, picturebox_config.Location.Y);
            Point pointdown = new Point(e.X, picturebox_config.Location.Y + picturebox_config.Height);
            g.DrawLine(pen1, pointup, pointdown);
            g.Save();

            Point pointleft = new Point(picturebox_config.Location.X + picturebox_config.Size.Width/2 , e.Y);
            Point pointright = new Point(e.X, e.Y);
            g.DrawLine(pen1, pointleft, pointright);
            g.Save();
            picturebox_config.Image = picturebox_config.Image;

        }



    }
}

