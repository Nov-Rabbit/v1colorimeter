using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using AForge;
using AForge.Imaging;
using AForge.Math;
using DUTclass;

namespace Colorimeter_Config_GUI
{
    public delegate void DataDelegate(object sender, DataChangeEventArgs args);

    public class DataChangeEventArgs
    {
        public string StatusInfo { get; set; }
        public Colorimeter Colorimeter { get; set; }
        public Bitmap Image { get; set; }
    }

    public class Engine
    {
        public Engine(string scriptName)
        {
            colorimeter = new Colorimeter();
            xml = new XMLManage(scriptName);
            dut = new Hodor();
            ip = new imagingpipeline();
            args = new DataChangeEventArgs();
            log = new testlog();
            SerialNumber = "";

            if (!System.IO.Directory.Exists(IMAGE_SAVE_PATH))
            {
                System.IO.Directory.CreateDirectory(IMAGE_SAVE_PATH);
            }
        }

        private readonly string IMAGE_SAVE_PATH = System.Windows.Forms.Application.StartupPath + "\\temp\\";

        public event DataDelegate dataChange;
        private DataChangeEventArgs args;

        private bool flagExit;
        private bool flagAutoMode;
        private Colorimeter colorimeter;
        private XMLManage xml;
        private testlog log;

        //dut setup
        private DUT dut;
        private imagingpipeline ip;

        public bool IsDutReady { get; private set; }
        public bool IsShopFlowReady { get; private set; }
        public string SerialNumber { private get; set; }
        public bool TestResult { get; private set; }

        private Thread tdBlock;

        public void Initilazie()
        {
            if (!colorimeter.Connect())
            {
                args.StatusInfo = "No Camera";
            }

            new Action(delegate() {
                while (!flagExit) {
                    if (dataChange != null) {
                        this.args.Colorimeter = colorimeter;
                        dataChange.Invoke(this, args);
                        //dataChange(this, args);
                    }
                    System.Threading.Thread.Sleep(100);
                }
            }).BeginInvoke(null, null);
        }

        public void Start()
        {
            if (tdBlock != null) {
                tdBlock.Abort();
                tdBlock = null;
            }

            tdBlock = new Thread(RunSignalSequence){
                IsBackground = true
            };
        }

        private void RunSequence()
        {
            do {
                this.RunSignalSequence();
            }
            while (!flagExit);
        }

        private void RunSignalSequence()
        {
            args.StatusInfo = "Checking DUT";
            dataChange.Invoke(this, args);
            
            // check dut
            while (!dut.checkDUT()) { Thread.Sleep(100); }
            log.WriteUartLog(string.Format("DUT connected, DeviceID: {0}\r\n", dut.DeviceID));            
            IsDutReady = true;
            args.StatusInfo = "Checking SN, please type in 16 digit SN";
            dataChange.Invoke(this, args);

            // check sn
            while (SerialNumber.Length != 16);
            log.SerialNumber = SerialNumber;
            log.WriteUartLog(string.Format("Serial number: {0}\r\n", SerialNumber));
            args.StatusInfo = string.Format("Serial number: {0}", SerialNumber);
            dataChange.Invoke(this, args);

            // check shopfloor
            args.StatusInfo = "Checking Shopfloor";
            dataChange.Invoke(this, args);
            IsShopFlowReady = this.CheckShopFloor();

            if (!IsShopFlowReady) {
                args.StatusInfo = "Shopfloor system is not working";
                log.WriteUartLog("Shopfloor system is not working.\r\n");
                dataChange.Invoke(this, args);
            }
            else {
                log.WriteUartLog("Shopfloor has connected.\r\n");
                args.StatusInfo = "Testing...";
                dataChange.Invoke(this, args);

                DateTime startTime = DateTime.Now, stopTime;
                List<IntPoint> ptCorners = new List<IntPoint>();
                TestResult = true;

                foreach (TestItem testItem in xml.Items)
                {
                    log.WriteUartLog(string.Format("Set panel to {0}\r\n", testItem.TestName));

                    if (dut.ChangePanelColor(testItem.TestName)) {
                        Thread.Sleep(3000);
                        colorimeter.ExposureTime = testItem.Exposure;
                        Bitmap bitmap = colorimeter.GrabImage();
                        args.Image = bitmap;
                        dataChange.Invoke(this, args);
                        TestResult &= this.RunDisplayTest(testItem, bitmap, ptCorners);
                        //TestResult &= this.DisplayTest(ptCorners, bitmap, (ColorPanel)Enum.Parse(typeof(ColorPanel), testItem.TestName));
                        //this.Invoke(new Action<Bitmap, PictureBox>(this.refreshtestimage), bitmap, picturebox_test);
                    }
                    else {
                        string str = string.Format("Can't set panel color to {0}\r\n", testItem.TestName);
                        //sslStatus.Text = str;
                        //pf = false;
                        break;
                    }
                }
                log.WriteUartLog(string.Format("Test result is {0}\r\n", (TestResult ? "PASS" : "FAIL")));
                log.UartFlush();

                stopTime = DateTime.Now;
                log.WriteCsv(SerialNumber, startTime, stopTime, xml.Items);
                //SFC.CreateResultFile(1, pf ? "PASS" : "FAIL");

                args.StatusInfo = "Please take out DUT";
                dataChange.Invoke(this, args);
                while (dut.checkDUT()){ Thread.Sleep(200); }
                SerialNumber = "";
            }
        }

        private bool CheckShopFloor()
        {
            bool flag = false;
            DateTime timeNow = DateTime.Now;

            do {
                // do something to check shopfloor
                flag = true;
            }
            while (DateTime.Now.Subtract(timeNow).TotalMilliseconds < 5000);

            return flag;
        }

        private bool RunDisplayTest(TestItem testItem, Bitmap bitmap, List<IntPoint> ptCorners)
        {
            bool flag = true;
            ColorPanel panel = (ColorPanel)Enum.Parse(typeof(ColorPanel), testItem.TestName);
            string imageName = string.Format("{0}{1}{2:yyyyMMddHHmmss}_{3}", IMAGE_SAVE_PATH, SerialNumber, DateTime.Now, panel.ToString());

            if (panel == ColorPanel.White) {
                ip.GetDisplayCornerfrombmp(bitmap, out ptCorners);
            }

            // save original image
            bitmap.Save(imageName);

            // cropping screen image
            Bitmap srcimg = new Bitmap(System.Drawing.Image.FromFile(imageName, true));
            Bitmap updateimg = CroppingImage(srcimg, ptCorners);
            args.Image = updateimg;
            dataChange.Invoke(this, args);

            // 截取区域图像
            Bitmap cropimg = ip.croppedimage(srcimg, ptCorners, dut.ui_width, dut.ui_height);
            cropimg.Save(imageName + "_cropped.bmp");
            args.Image = cropimg;
            dataChange.Invoke(this, args);

            // anaylse
            ColorimeterResult colorimeterRst = new ColorimeterResult(cropimg, panel);
            colorimeterRst.Analysis();

            switch (panel) { 
                case ColorPanel.White:
                case ColorPanel.Black:
                    testItem.SubNodes[0].Value = colorimeterRst.Luminance;
                    testItem.SubNodes[1].Value = colorimeterRst.Uniformity5;
                    testItem.SubNodes[2].Value = colorimeterRst.Mura;
                    break;
                case ColorPanel.Red:
                case ColorPanel.Green:
                case ColorPanel.Blue:
                    testItem.SubNodes[0].Value = colorimeterRst.CIE1931xyY.x;
                    testItem.SubNodes[1].Value = colorimeterRst.CIE1931xyY.y;
                    testItem.SubNodes[2].Value = colorimeterRst.CIE1931xyY.Y;
                    break;
            }

            foreach (TestNode node in testItem.SubNodes)
            {
                flag &= node.Run();
            }
            dataChange.Invoke(this, args);

            return flag;
        }

        //private void DrawZone(Bitmap binImage, ColorPanel panel)
        //{
        //    zoneresult zr = new zoneresult();
        //    Graphics g = Graphics.FromImage(binImage);

        //    for (int i = 1; i < 6; i++)
        //    {
        //        // get corner coordinates
        //        flagPoints = zr.zonecorners(i, zonesize, XYZ);
        //        // zone image
        //        g = zoneingimage(g, flagPoints);
        //        binImage.Save(tempdirectory + i.ToString() + "_" + panel.ToString() + "_bin_zone.bmp");
        //        flagPoints.Clear();
        //    }

        //    binImage.Save(tempdirectory + tbox_sn.Text + str_DateTime + "_" + panel.ToString() + "_bin_zone1-5.bmp");
        //    refreshtestimage(binImage, picturebox_test);
        //    g.Dispose();
        //}

        private Bitmap CroppingImage(Bitmap srcimg, List<IntPoint> cornerPoints)
        {
            Graphics g = Graphics.FromImage(srcimg);
            List<System.Drawing.Point> Points = new List<System.Drawing.Point>();

            foreach (var point in cornerPoints)
            {
                Points.Add(new System.Drawing.Point(point.X, point.Y));
            }
            g.DrawPolygon(new Pen(Color.Red, 15.0f), Points.ToArray());
            srcimg.Save(IMAGE_SAVE_PATH + SerialNumber + DateTime.Now.ToString("yyyyMMddHHmmss") + "_cropping.bmp");
            g.Dispose();
            return srcimg;
        }

        private bool DisplayTest(List<IntPoint> displaycornerPoints, Bitmap bitmap, ColorPanel panelType)
        {
            if (panelType == ColorPanel.White)
            {
                ip.GetDisplayCornerfrombmp(bitmap, out displaycornerPoints);
            }

            // 原始图像
            string imageName = string.Format("{0}{1}{2:yyyyMMddHHmmss}_{3}", IMAGE_SAVE_PATH, SerialNumber, DateTime.Now, panelType.ToString());
            bitmap.Save(imageName + ".bmp");

            //need save bmp outside as file format and reload so that 
            Bitmap srcimg = new Bitmap(System.Drawing.Image.FromFile(imageName, true));

            // 找出屏幕区域的图像
            Bitmap updateimg = CroppingImage(srcimg, displaycornerPoints);
            args.Image = updateimg;
            dataChange.Invoke(this, args);
           // this.refreshtestimage(updateimg, picturebox_test);

            // 截取区域图像
            Bitmap cropimg = ip.croppedimage(srcimg, displaycornerPoints, dut.ui_width, dut.ui_height);
            cropimg.Save(imageName + "_cropped.bmp");
            args.Image = cropimg;
            dataChange.Invoke(this, args);
            //picturebox_test.Width = cropimg.Width;
            //picturebox_test.Height = cropimg.Height;
            //this.refreshtestimage(cropimg, picturebox_test);

            // binary 图像
            Bitmap binimg = new Bitmap(cropimg, new Size(dut.bin_width, dut.bin_height));
            binimg.Save(imageName + "_bin.bmp");

            ColorimeterResult colorimeterRst = new ColorimeterResult(bitmap, panelType);
            colorimeterRst.Analysis();

            switch (panelType)
            {
                case ColorPanel.White:
                    //this.DrawZone(binimg, panelType);
                    //cbox_white_lv.Checked = cbox_white_uniformity.Checked = cbox_white_mura.Checked = true;
                    //tbox_whitelv.Text = colorimeterRst.Luminance.ToString();
                    //tbox_whiteunif.Text = (colorimeterRst.Uniformity5 * 100).ToString();
                    //tbox_whitemura.Text = colorimeterRst.Mura.ToString();
                    log.WriteUartLog(string.Format("luminance: {0}, uniformity5: {1}, mura: {2}",
                        colorimeterRst.Luminance, colorimeterRst.Uniformity5, colorimeterRst.Mura));
                    break;
                case ColorPanel.Black:
                    //this.DrawZone(binimg, panelType);
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

            return true;//this.AnaylseResult(colorimeterRst, panelType);
        }
    }
}
