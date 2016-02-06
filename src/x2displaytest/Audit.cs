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
    public class Audit : Form_Config
    {
        private ManagedCameraBase m_camera = null;
        private ManagedImage m_rawImage;
        private ManagedImage m_processedImage;
        private System.Windows.Forms.PictureBox picturebox_audit;

        private void UpdateAuditUI(object sender, ProgressChangedEventArgs e)
        {
            picturebox_audit.Image = m_processedImage.bitmap;
            picturebox_audit.Invalidate();
        }

    }
}
