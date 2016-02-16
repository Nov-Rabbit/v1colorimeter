using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FlyCapture2Managed;
using FlyCapture2Managed.Gui;

using AForge;
using AForge.Imaging;
using AForge.Math;
using AForge.Math.Geometry;

namespace DUTclass
{
    public class hodor
    {
        private ManagedImage m_dislayImage;

        public static void spec()
        {
            string panelvendor = "Innolux";
            double width = 293.472; // Unit: mm
            double height = 165.078;
            double pixelpitch = 0.15285;
            double contrast_min = 500;
            double contrast_max = 700;
            double lvuniformity5 = 1.25; // 80% non-uniformity for 5 pt
            double lvuniformity13 = 1.60; // 67.5% non-uniformity for 13 pt
            double lv_white_min = 340; // nits
            double lv_white_max = 400;
            double vdd = 3.3; //V
            double delta_vdd = 0.3; // allow LCD driver voltage with +/- 0.3 V
            double power = 5.4; // max 5.4W
            int pixel_w = 1920;
            int pixel_h = 1080;
            // primary color RGBW typical CIE1931 xy values
            double rx = 0.635;
            double ry = 0.335;
            double gx = 0.300;
            double gy = 0.620;
            double bx = 0.150;
            double by = 0.045;
            double wx = 0.313;
            double wy = 0.329;
            // +/- x,y delta
            double delta_xy = 0.03; // apply to x and y, +/-0.03 is expected.
            double gamut_over_ntsc = 0.72; // 72% over NTSC standard.
            double crosstalk = 4; // maximum cross talk value allowed, 4%
        }

        public int ui_width = 960;
        public int ui_height = 540;

        //assuming 1mm resolution
        public int bin_width = 293;
        public int bin_height = 165;
        // below are the manual mode. need customer to provide the command line to drive units to auto mode.
        public void setwhite()
        {
            MessageBox.Show("Send CMD to Set White State");
        }

        public void setblack()
        {
            MessageBox.Show("Send CMD to Set Black State");
        }

        public void setred()
        {
            MessageBox.Show("Send CMD to Set Red State");
        }

        public void setgreen()
        {
            MessageBox.Show("Send CMD to Set Green State");
        }

        public void setblue()
        {
            MessageBox.Show("Send CMD to Set Blue State");
        }

    }

    public class bran
    {
        public static void spec()
        {
            string panelvendor = "KD";
            double width = 94.20; // Unit: mm
            double height = 150.72;
            double pixelpitch = 0.11775;
            double contrast_min = 500;
            double contrast_max = 700;
            double lvuniformity5 = 1.33; // 75% non-uniformity for 5 pt
            double lvuniformity13 = 1.60; // 67.5% non-uniformity for 13 pt
            double lv_white_min = 300; // nits
            double lv_white_max = 400;
            double vdd = 3.3; //V
            double delta_vdd = 0.3; // allow LCD driver voltage with +/- 0.3 V
            double power = 0.612; // max 3.6V and 170mA
            int pixel_w = 800;
            int pixel_h = 1280;
            // primary color RGBW typical CIE1931 xy values
            double rx = 0.635;
            double ry = 0.335;
            double gx = 0.300;
            double gy = 0.620;
            double bx = 0.150;
            double by = 0.045;
            double wx = 0.300;
            double wy = 0.320;
            // +/- x,y delta
            double delta_xy = 0.03; // apply to x and y, +/-0.03 is expected.
            double gamut_over_ntsc = 0.72; // 72% over NTSC standard.
            double crosstalk = 4; // maximum cross talk value allowed, 4%
        }

        public void setwhite()
        {
            MessageBox.Show("Send CMD to Set White State");
        }

        public void setblack()
        {
            MessageBox.Show("Send CMD to Set Black State");
        }

        public void setred()
        {
            MessageBox.Show("Send CMD to Set Red State");
        }

        public void setgreen()
        {
            MessageBox.Show("Send CMD to Set Green State");
        }

        public void setblue()
        {
            MessageBox.Show("Send CMD to Set Blue State");
        }
    }
}
