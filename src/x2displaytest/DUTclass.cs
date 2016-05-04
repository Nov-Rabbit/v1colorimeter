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

using X2DisplayTest;

namespace DUTclass
{
    public abstract class DUT : IDisposable
    {
        public DUT()
        {
            contrast_min = 500;
            contrast_max = 700;
            lv_white_min = 340;
            lv_white_max = 400;
            vdd = 3.3;
            delta_vdd = 0.3;

            // primary color RGBW typical CIE1931 xy values
            rx = 0.635;
            ry = 0.335;
            gx = 0.300;
            gy = 0.620;
            bx = 0.150;
            by = 0.045;
            wx = 0.313;
            wy = 0.329;

            // +/- x,y delta
            delta_xy = 0.03;        // apply to x and y, +/-0.03 is expected.
            gamut_over_ntsc = 0.72; // 72% over NTSC standard.
            crosstalk = 4;          // maximum cross talk value allowed, 4%
        }

        protected string panelvendor;
        protected double width;             // Unit: mm
        protected double height;
        protected double pixelpitch;
        protected double contrast_min;
        protected double contrast_max;
        protected double lvuniformity5;     // 80% non-uniformity for 5 pt  
        protected double lvuniformity13;    // 67.5% non-uniformity for 13 pt
        protected double lv_white_min;      // nits
        protected double lv_white_max;
        protected double vdd;
        protected double delta_vdd; // allow LCD driver voltage with +/- 0.3 V
        protected double power;     // max 5.4W
        protected int pixel_w;
        protected int pixel_h;

        // primary color RGBW typical CIE1931 xy values
        protected double rx;
        protected double ry;
        protected double gx;
        protected double gy;
        protected double bx;
        protected double by;
        protected double wx;
        protected double wy;

        // +/- x, y delta
        protected double delta_xy; // apply to x and y, +/-0.03 is expected.
        protected double gamut_over_ntsc;   // 72% over NTSC standard.
        protected double crosstalk; // maximum cross talk value allowed, 4%

        private AdbPipe pipe;

        public static DUT Instance()
        {
            string id = null;
            DUT dut = null;
            AdbPipe pipe = new AdbPipe();

            while ((id = pipe.GetDeviceID()) == null) {
                System.Threading.Thread.Sleep(100);
            }

            if (id == "xxxx")
                dut = new Hodor();
            else
                dut = new Bran();

            return dut;
        }

        public abstract int ui_width { get; }
        public abstract int ui_height { get; }

        //assuming 1mm resolution
        public abstract int bin_width { get; }
        public abstract int bin_height { get; }

        public string DeviceID { get; private set; }

        public virtual bool CheckDUT()
        {
            if (pipe == null) {
                pipe = new AdbPipe();
            }

            return ((DeviceID=pipe.GetDeviceID()) != null);
        }

        public virtual bool ChangePanelColor(string panelColorName)
        {
            bool flag = false;

            if (string.IsNullOrEmpty(panelColorName))
            {
                return flag;
            }

            string name = panelColorName.ToLower();

            if (pipe == null) {
                pipe = new AdbPipe();
            }
           // pipe.ReadToEnd();

            if (name.Equals("white")) {
                flag = pipe.SetWhiteMode();
            } else if (name.Equals("black")) {
                flag = pipe.SetBlackMode();
            } else if (name.Equals("red")) {
                flag = pipe.SetRedMode();
            } else if (name.Equals("green")) {
                flag = pipe.SetGreenMode();
            } else if (name.Equals("blue")) {
                flag = pipe.SetBlueMode();
            }

            return flag;
        }

        public virtual bool ChangePanelColor(int r, int g, int b)
        {
            bool flag = false;

            if (r < 0) { r = 0; }
            else if (r > 255) { r = 255; }
            if (g < 0) { g = 0; }
            else if (g > 255) { g = 255; }
            if (b < 0) { b = 0; }
            else if (b > 255) { b = 255; }

            flag = pipe.SetRGBValue(r, g, b);

            return flag;
        }

        public string GetSerialNumber()
        {
            if (pipe == null) {
                pipe = new AdbPipe();
            }

            return pipe.GetDeviceDSN();
        }

        public void Dispose()
        {
            if (pipe != null)
            {
                pipe.ExitAdbPipe();
            }
        }
    }

    public class Hodor : DUT
    {
        public Hodor()
        {
            panelvendor = "Innolux";
            width = 293.472;
            height = 165.078;
            pixelpitch = 0.15285;
            lvuniformity5 = 1.25;
            lvuniformity13 = 1.60;
            power = 5.4; 
            pixel_w = 1920;
            pixel_h = 1080;
            //m_dislayImage = new ManagedImage();
        }

        //private ManagedImage m_dislayImage;

        public override int ui_width
        {
            get { return 960; }
        }

        public override int ui_height 
        {
            get { return 540; }
        }

        public override int bin_width
        {
            get { return 293; }
        }

        public override int bin_height
        {
            get { return 165; }
        }

        public override string ToString()
        {
            return "Hodor";
        }
    }

    public class Bran : DUT
    {
        public Bran()
        {
            panelvendor = "KD";
            width = 94.20; // Unit: mm
            height = 150.72;
            pixelpitch = 0.11775;
            lvuniformity5 = 1.33; // 75% non-uniformity for 5 pt
            lvuniformity13 = 1.60; // 67.5% non-uniformity for 13 pt
            power = 0.612; // max 3.6V and 170mA
            pixel_w = 800;
            pixel_h = 1280;
        }

        public override int ui_width
        {
            get { return 960; }
        }

        public override int ui_height
        {
            get { return 540; }
        }

        public override int bin_width
        {
            get { return 293; }
        }

        public override int bin_height
        {
            get { return 165; }
        }

        public override string ToString()
        {
            return "Bran";
        }
    }
}
