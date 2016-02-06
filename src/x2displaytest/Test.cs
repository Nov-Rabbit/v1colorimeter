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
    public partial class Test : Form_Config
    {
        private FlyCapture2Managed.Gui.CameraControlDialog m_camCtlDlg;
        private ManagedCameraBase m_camera = null;
        private ManagedImage m_rawImage;
        private ManagedImage m_processedImage;
        private bool m_grabImages;
        private AutoResetEvent m_grabThreadExited;
        private BackgroundWorker m_grabThread;
        // UI 
        private System.Windows.Forms.TextBox tbox_ccdtemp;
        private System.Windows.Forms.TextBox tbox_uptime;
        private System.Windows.Forms.TextBox tbox_errorcode;
        private System.Windows.Forms.TextBox tbox_colorimeterstatus;
        private System.Windows.Forms.PictureBox picturebox_test;

        public void UpdateTestUI(object sender, ProgressChangedEventArgs e)
        {
            String statusString;

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
            picturebox_test.Image = m_processedImage.bitmap;
            picturebox_test.Invalidate();

            if (ccd_temp > 50.0 || timestamp.cycleSeconds > 3600 * 24)
            {
                tbox_colorimeterstatus.BackColor = Color.Red;
                tbox_colorimeterstatus.Text = "Fail. Reset Station";
            }
            else
            {
                tbox_colorimeterstatus.BackColor = Color.LightGray;
                tbox_colorimeterstatus.Text = "OK";
            }

            tbox_colorimeterstatus.Refresh();

        }

    }
}
