using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using Imageprocess;
using AForge;
using AForge.Imaging;
using AForge.Math;
using DUTclass;
using System.Data;

namespace X2DisplayTest
{
    public delegate void DataDelegate(object sender, DataChangeEventArgs args);
    public delegate void TableViewDelegate(object sender, TableViewEventArgs args);

    public class TableViewEventArgs
    {
        public string CurrentDevice { get; set; }
        public int Index { get; set; }
        public List<TestItem> Items { get; set; }
    }

    public class DataChangeEventArgs
    {
        public string StatusInfo { get; set; }
        public string CCDStatusInfo { get; set; }
        public double CCDTemperature { get; set; }
        public TimeSpan Uptime { get; set; }
        public Bitmap Image { get; set; }

        public DataChangeEventArgs()
        {
            StatusInfo = "Ready";
            CCDStatusInfo = "";
            CCDTemperature = 0;
        }
    }

    public class Engine
    {
        public Engine(Config config)
        {
            this.config = config;
            //this.colorimeter = new Colorimeter();
            this.xml = new Xml(this.config.ScriptName);

            if (!this.config.IsSimulation) {
                this.fixture = new Fixture();
                //this.ca310Pipe = new Ca310Pipe(System.Windows.Forms.Application.StartupPath);
                IDevice intergrate = new IntegratingSphere(this.fixture, this.config.LCP3005PortName);
                DevManage.Instance.AddDevice(fixture);
                DevManage.Instance.AddDevice(intergrate);
            }

            dut = (DUT)Activator.CreateInstance(Type.GetType("DUTclass." + this.config.ProductType));
            mode = (TestMode)Enum.Parse(typeof(TestMode), this.config.TestMode);

            ip = new imagingpipeline();
            args = new DataChangeEventArgs();
            tableArgs = new TableViewEventArgs();
            tableArgs.Items = xml.Items;

            log = new Testlog();
            SerialNumber = "";

            if (!System.IO.Directory.Exists(IMAGE_SAVE_PATH))
            {
                System.IO.Directory.CreateDirectory(IMAGE_SAVE_PATH);
            }
        }

        private readonly string IMAGE_SAVE_PATH = System.Windows.Forms.Application.StartupPath + "\\temp\\";

        public event DataDelegate dataChange;
        private DataChangeEventArgs args;

        public event TableViewDelegate tableDataChange;
        private TableViewEventArgs tableArgs;

        private bool flagFan;
        private bool flagExit;
        private bool flagAutoMode;
        private bool flagCa310Mode;

        private Colorimeter colorimeter;
        private Config config;
        private Xml xml;

        private Testlog log;
        private Ca310Pipe ca310Pipe;
        private Fixture fixture;
        private imagingpipeline ip;

        //private Imageprocess.ImagingPipeline pipeline;

        //dut setup
        private DUT dut;
        public DUT Dut
        {
            get {
                return dut;
            }
            set {
                dut = value;
            }
        }

        // test mode
        private TestMode mode;
        public TestMode TestMode
        {
            get {
                return mode;
            }
            set {
                mode = value;
            }
        }

        public List<TestItem> Items {
            get {
                if (this.xml == null) {
                    this.xml = new Xml(this.config.ScriptName);
                }

                return this.xml.Items;
            }
        }

        public bool IsCa310Test { get; private set; }
        public bool IsDutReady { get; private set; }
        public bool IsShopFlowReady { get; private set; }
        public string SerialNumber { private get; set; }
        public bool TestResult { get; private set; }

        private Thread tdBlock;

        public void Initilazie()
        {
            new Action(delegate() {
                try {
                    if (!colorimeter.Connect()) {
                        args.StatusInfo = "No Camera";
                    }
                    colorimeter.Shutter = 60;

                    DateTime timezero = DateTime.Now;

                    while (!flagExit) {
                        if (dataChange != null) {
                            args.Uptime = DateTime.Now.Subtract(timezero);

                            if (colorimeter.Temperature < 20 && args.Uptime.Hours < 24) {
                                if (!this.config.IsSimulation) {
                                    if (flagFan) {
                                        fixture.FanOff();
                                        flagFan = !flagFan;
                                    }
                                }
                                args.CCDStatusInfo = "OK";
                            }
                            else if (colorimeter.Temperature < 50 && args.Uptime.Hours < 24) {
                                if (!this.config.IsSimulation) {
                                    if (flagFan) {
                                        fixture.FanOn();
                                        flagFan = !flagFan;
                                    }
                                }
                                args.CCDStatusInfo = "Warm CCD";
                            }
                            args.CCDTemperature = colorimeter.Temperature;
                            dataChange.Invoke(this, args);
                        }
                        System.Threading.Thread.Sleep(100);
                    }
                }
                catch (Exception e){
                    args.StatusInfo = e.Message;
                    dataChange.Invoke(this, args);
                }                
            }).BeginInvoke(null, null);
        }

        public void Start()
        {
            tdBlock = new Thread(RunSignalSequence){
                IsBackground = true
            };
            tdBlock.Start();
        }

        public void Stop()
        {
            flagExit = true;
            Thread.Sleep(100);

            if (tdBlock != null) {
                tdBlock.Abort();
                tdBlock = null;
            }
        }

        public void Exit()
        {
            Stop();
            dut.Dispose();

            if (!this.config.IsSimulation) { 
                //fixture
                fixture.Exit();
            }
        }

        private bool CheckShopFloor()
        {
            bool flag = false;
            int status = -1;
            DateTime timeNow = DateTime.Now;

            do {
                // do something to check shopfloor
                if (this.config.IsOnlineShopfloor) {
                    SFC.SFCInit();
                    status = SFC.ReportStatus(this.SerialNumber, 1);

                    if (status == 0) {
                        flag = true;
                        break;
                    }
                }
                else {
                    flag = true;
                    break;
                }
            }
            while (DateTime.Now.Subtract(timeNow).TotalMilliseconds < 5000);

            return flag;
        }

        private void UploadItemDataToSFC(TestItem testItem, string testDevice)
        {
            string name = null;
            string passfail = "PASS", upperStr = "", lowerStr = "";

            for (int i = 0; i < testItem.TestNodes.Count; i++)
            {
                name = string.Format("{0}_{1}[{2}]", testItem.TestName, testItem.TestNodes[0].NodeName, testDevice);
                passfail = testItem.TestNodes[i].Result ? "PASS" : "FAIL";
                upperStr = (testItem.TestNodes[1].Upper == double.NaN) ? "_" : testItem.TestNodes[1].Upper.ToString();
                lowerStr = (testItem.TestNodes[2].Lower == double.NaN) ? "_" : testItem.TestNodes[2].Lower.ToString();

                int nFlag = SFC.AddTestLog(1, (uint)i, name, upperStr, lowerStr, testItem.TestNodes[i].Value.ToString(), passfail);

                if (nFlag != 0)
                {
                    args.StatusInfo = "Fail to upload SFC.";
                }
            }
        }

        private void InitCa310()
        {
            #region Init Ca310
            if (mode == TestMode.Ca310) {
                if (ca310Pipe == null) {
                    ca310Pipe = new Ca310Pipe(System.Windows.Forms.Application.StartupPath); 
                    args.StatusInfo = "Initilaze Ca310 device.";

                    if (!ca310Pipe.Connect()) {
                        args.StatusInfo = ca310Pipe.ErrorMessage;
                    }
                    else {
                        args.StatusInfo = "Ca310 has Connected.";
                        ca310Pipe.ResetZero();
                    }
                }
            }
            else {
                if (ca310Pipe != null) {
                    ca310Pipe.Disconnect();
                    ca310Pipe = null;
                }
            }
            #endregion
        }

        private void RunCa310Test()
        {
            if (mode == TestMode.Ca310)
            {
                int index = 0;
                const string deviceName = "Ca310";
                Dictionary<string, CIE1931Value> items = new Dictionary<string, CIE1931Value>();

                if (!this.config.IsSimulation) {
                    fixture.RotateOn();
                }

                foreach (TestItem testItem in xml.Items)
                {
                    log.WriteUartLog(string.Format("Ca310Mode - Set panel to {0}\r\n", testItem.TestName));

                    if (dut.ChangePanelColor(testItem.RGB.R, testItem.RGB.G, testItem.RGB.B))
                    {
                        Thread.Sleep(3000);
                        CIE1931Value cie = ca310Pipe.GetCa310Data();
                        log.WriteUartLog(string.Format("Ca310Mode - CIE1931xyY: {0}\r\n", cie.ToString()));
                        items.Add(testItem.TestName, cie.Copy());

                        if (this.config.IsOnlineShopfloor) {
                            UploadItemDataToSFC(testItem, deviceName);
                        }
                        
                        // flush UI
                        if (tableDataChange != null) {
                            tableArgs.CurrentDevice = deviceName;
                            tableArgs.Index = index++;
                            tableDataChange(this, tableArgs);
                        }
                    }
                    else 
                    {
                        args.StatusInfo = string.Format("Can't set panel color to {0}\r\n", testItem.TestName);
                        break;
                    }
                }

                if (!this.config.IsSimulation) {
                    fixture.RotateOff();
                }

                log.WriteCa310Log(SerialNumber, items);
            }
        }

        private void RunSequence()
        {
            do {
                this.InitCa310();
                this.RunSignalSequence();
            }
            while (!flagExit);
        }

        private void RunSignalSequence()
        {
            if (!this.config.IsSimulation) {
                args.StatusInfo = "Waitting double-satrt pressed...";
                fixture.CheckDoubleStart();
                fixture.BatteryOn();
            }

            args.StatusInfo = "Checking DUT";
            while (!dut.CheckDUT()) { Thread.Sleep(100); } // check dut
            log.WriteUartLog(string.Format("DUT connected, DeviceID: {0}\r\n", dut.DeviceID));            
            IsDutReady = true;

            if (this.config.IsScanSerialNumber) {
                args.StatusInfo = "Checking SN, please type in 16 digit SN";
                while (SerialNumber.Length != 16) { Thread.Sleep(100); }
            }
            else {
                args.StatusInfo = "Checking SN";
                do{
                    SerialNumber = dut.GetSerialNumber();
                    Thread.Sleep(100);
                }
                while (SerialNumber.Length != 16);
            }
            
            log.SerialNumber = SerialNumber;
            log.WriteUartLog(string.Format("Serial number: {0}\r\n", SerialNumber));
            args.StatusInfo = string.Format("Serial number: {0}", SerialNumber);

            // check shopfloor
            args.StatusInfo = "Checking Shopfloor";
            IsShopFlowReady = this.CheckShopFloor();

            if (!IsShopFlowReady) {
                args.StatusInfo = "Shopfloor system is not working";
                log.WriteUartLog("Shopfloor system is not working.\r\n");
            }
            else {
                log.WriteUartLog("Shopfloor has connected.\r\n");
                args.StatusInfo = "Testing...";

                // run Ca310 if the mode is Ca310Mode
                this.RunCa310Test();
                
                DateTime startTime = DateTime.Now, stopTime;
                List<IntPoint> ptCorners = new List<IntPoint>();
                TestResult = true;

                int index = 0;
                const string deviceName = "Camera";

                foreach (TestItem testItem in xml.Items)
                {
                    log.WriteUartLog(string.Format("Set panel to {0}\r\n", testItem.TestName));

                    bool flag = false;

                    if (mode == TestMode.Manual) {
                        System.Windows.Forms.MessageBox.Show(string.Format("Please set panel to \"{0}\"", testItem.TestName));
                        flag = true;
                    }
                    else {
                        args.StatusInfo = string.Format("Set panel to \"{0}\"", testItem.TestName);
                        flag = dut.ChangePanelColor(testItem.RGB.R, testItem.RGB.G, testItem.RGB.B);
                    }   

                    if (flag) {
                        Thread.Sleep(3000);
                        colorimeter.Shutter = (float)testItem.Exposure;
                        Bitmap bitmap = colorimeter.GrabImage();
                        args.Image = bitmap;
                        TestResult &= this.RunDisplayTest(testItem, bitmap, ptCorners);

                        if (this.config.IsOnlineShopfloor) {
                            UploadItemDataToSFC(testItem, deviceName);
                        }

                        // flush UI
                        if (tableDataChange != null) {
                            tableArgs.CurrentDevice = deviceName;
                            tableArgs.Index = index++;
                            tableDataChange(this, tableArgs);
                        }
                    }
                    else {
                        args.StatusInfo = string.Format("Can't set panel color to {0}\r\n", testItem.TestName);
                        //dataChange.Invoke(this, args);
                        break;
                    }
                }
                log.WriteUartLog(string.Format("Test result is {0}\r\n", (TestResult ? "PASS" : "FAIL")));
                log.UartFlush();

                stopTime = DateTime.Now;
                log.WriteCsv(SerialNumber, startTime, stopTime, xml.Items);

                if (this.config.IsOnlineShopfloor) {
                    SFC.CreateResultFile(1, TestResult ? "PASS" : "FAIL");
                }

                SerialNumber = "";
            }
        }

        private bool RunDisplayTest(TestItem testItem, Bitmap bitmap, List<IntPoint> ptCorners)
        {
            //ColorPanel panel = (ColorPanel)Enum.Parse(typeof(ColorPanel), testItem.TestName);
            string imageName = string.Format("{0}{1}_{2:yyyyMMddHHmmss}_{3}", IMAGE_SAVE_PATH, SerialNumber, DateTime.Now, testItem.TestName);

            if (testItem.RGB == Color.FromArgb(255,255,255)) {
                ip.GetDisplayCornerfrombmp(bitmap, out ptCorners);
            }

            //if (panel == ColorPanel.White) {
            //    ip.GetDisplayCornerfrombmp(bitmap, out ptCorners);
            //}

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
            ColorimeterResult colorimeterRst = new ColorimeterResult(cropimg);
            colorimeterRst.Analysis(ref testItem);

            //colorimeterRst.Analysis();
            this.DrawZone(cropimg, testItem.TestName);

            return testItem.Run();
        }

        private Graphics ZoneImage(Graphics g, List<IntPoint> cornerPoints)
        {
            List<System.Drawing.Point> Points = new List<System.Drawing.Point>();
            foreach (var point in cornerPoints)
            {
                Points.Add(new System.Drawing.Point(point.X, point.Y));
            }
            g.DrawPolygon(new Pen(Color.Red, 1.0f), Points.ToArray());
            return g;
        }

        private void DrawZone(Bitmap binImage, string panelName)
        {
            string imageName = string.Format("{0}{1}{2:yyyyMMddHHmmss}_{3}", IMAGE_SAVE_PATH, SerialNumber, DateTime.Now, panelName);
            zoneresult zr = new zoneresult();
            Graphics g = Graphics.FromImage(binImage);

            for (int i = 1; i < 6; i++)
            {
                // get corner coordinates
                List<IntPoint> flagPoints = zr.zonecorners(i, 10, ip.bmp2rgb(binImage));
                // zone image
                g = ZoneImage(g, flagPoints);
                binImage.Save(IMAGE_SAVE_PATH + i.ToString() + "_" + panelName.ToString() + "_bin_zone.bmp");
                flagPoints.Clear();
            }

            binImage.Save(imageName + "_bin_zone1-5.bmp");
            //refreshtestimage(binImage, picturebox_test);
            args.Image = binImage;
            dataChange.Invoke(this, args);
            g.Dispose();
        }

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

        private void LvCalibration(Action callBack)
        {
            try
            {
                fixture.IntegratingSphereUp();
                Thread.Sleep(1000);
                fixture.RotateOn();
                Thread.Sleep(1000);
                CIE1931Value value = ca310Pipe.GetCa310Data();
                fixture.RotateOff();
                Thread.Sleep(1000);

                for (int i = 0; i < 10; i++)
                {
                    colorimeter.ExposureTime = 10 * (i + 1);
                    Bitmap bitmap = colorimeter.GrabImage();
                    double[] rgbMean = this.Mean(ip.bmp2rgb(bitmap));

                    if (Math.Abs(rgbMean[0] - 220) < 3
                        && Math.Abs(rgbMean[1] - 220) < 3
                        && Math.Abs(rgbMean[2] - 220) < 3)
                    {
                        //xml.SetWhiteExposure(colorimeter.ExposureTime);
                        break;
                    }
                }

                fixture.IntegratingSphereDown();
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
              //  MessageBox.Show(ex.Message);
            }
        }

        private double[] Mean(double[, ,] color)
        {
            double[] v = new double[3];
            int count = v.GetLength(0) * v.GetLength(1);

            for (int i = 0; i < v.GetLength(0); i++)
            {
                for (int j = 0; j < v.GetLength(1); j++)
                {
                    v[0] += color[i, j, 0];
                    v[1] += color[i, j, 1];
                    v[2] += color[i, j, 2];
                }
            }

            v[0] /= count;
            v[1] /= count;
            v[2] /= count;

            return v;
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

        public void Video(System.Windows.Forms.PictureBox cavaus, bool alive)
        {
            if (alive) {
                colorimeter.SetVideoCavaus(cavaus);
                colorimeter.PlayVideo();
            }
            else {
                colorimeter.StopVideo();
            }            
        }

        public void FocusCalibration()
        {
            IntegratingSphere intergrate = (IntegratingSphere)DevManage.Instance.SelectDevice(typeof(IntegratingSphere).Name);
            
            if (fixture != null)
            {
                fixture.RotateOff();
                fixture.IntegratingSphereUp();
                intergrate.Lighten();
                fixture.HoldIn();
                fixture.BatteryOn();
                while (!dut.CheckDUT()) { Thread.Sleep(100); }
                Thread.Sleep(8000);
                dut.ChangePanelColor(255, 255, 255);
            }
        }

        public int ShowColorimeterDialog(bool isModalMode)
        {
            int result = 0;

            if (isModalMode) {
                colorimeter.ShowCCDControlDialog();
            }
            else {
                if (colorimeter.CameraCtlDlg.IsVisible()) {
                    colorimeter.CameraCtlDlg.Hide();
                    result = 1;
                }
                else {
                    colorimeter.CameraCtlDlg.Show();
                    result = 2;
                }
            }

            return result;
        }
    }

    public enum TestMode
    {
        None,
        Manual,
        Automatic,
        Ca310,
    }
}
