using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace X2DisplayTest
{
    public abstract class ICalibration : IDevice
    {
        protected float redWeight = 0.72f;
        protected float greenWeight = 0.18f;
        protected float blueWeight = 0.1f;

        /// <summary>
        /// get/set red weight [0 - 1]
        /// </summary>
        public float RedWeight 
        {
            get {
                return redWeight;
            }
            set {
                redWeight = Verity(value);
            }
        }
        /// <summary>
        /// get/set green weight [0 - 1]
        /// </summary>
        public float GreenWeight
        {
            get {
                return greenWeight;
            }
            set {
                greenWeight = Verity(value);
            }
        }
        /// <summary>
        /// get/set blue weight [0 - 1]
        /// </summary>
        public float BlueWeight
        {
            get {
                return blueWeight;
            }
            set {
                blueWeight = Verity(value);
            }
        }

        public virtual string FilePath
        {
            get {
                return filepath;
            }
        }

        protected Colorimeter camera;
        protected Fixture fixture;

        protected string filepath;

        protected string serialNumber;
        public virtual string SerialNumber
        {
            get {
                return serialNumber;
            }
            set {
                serialNumber = value;
            }
        }

        private float Verity(float value)
        {
            float result = value;

            if (value > 1)
                result = 1;
            else if (value < 0)
                result = 0;
            else
                result = value;

            return result;
        }

        public float CalExposureTime(float exposureTime, int[] rgb)
        {
            double faction = (redWeight * rgb[0] + greenWeight * rgb[1] + blueWeight * rgb[1]) / 255;

            if (faction == 0)
            {
                return (4000);
            }

            float exposure = (float)(exposureTime / faction);
            return exposure;
        }

        protected int[, ,] BitmapToRGB(System.Drawing.Bitmap bitmap)
        {
            int[, ,] avgRGB = new int[bitmap.Width, bitmap.Height, 3];

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    avgRGB[i, j, 0] += bitmap.GetPixel(i, j).R;
                    avgRGB[i, j, 1] += bitmap.GetPixel(i, j).G;
                    avgRGB[i, j, 2] += bitmap.GetPixel(i, j).B;
                    //Console.WriteLine("RGB({0}, {1}, {2})", avgRGB[i, j, 0], avgRGB[i, j, 1], avgRGB[i, j, 2]);
                }
            }

            return avgRGB;
        }

        public abstract void Calibration(float exposure = 0);

        public ICalibration()
        {
            this.serialNumber = "No serial number";
            this.filepath = System.Windows.Forms.Application.StartupPath;
        }

        public override System.Windows.Forms.Control DeviceConfigPanel
        {
            get {
                if (this.panel == null)
                    this.panel = new CalibrationPanel(this);
                return this.panel;
            }
            protected set {
                this.panel = value;
            }
        }

        protected override void ReadProfile()
        {
            try {
                string segName = this.GetType().Name;
                this.redWeight = (float)fileHandle.ReadDouble(segName, "red_weight");
                this.greenWeight = (float)fileHandle.ReadDouble(segName, "green_weight");
                this.blueWeight = (float)fileHandle.ReadDouble(segName, "blue_weight");
            }
            catch {
                this.redWeight = 0.72f;
                this.greenWeight = 0.18f;
                this.blueWeight = 0.1f;
                this.WriteProfile();
            }
        }

        protected override void WriteProfile()
        {
            string segName = this.GetType().Name;
            fileHandle.WriteDouble(segName, "red_weight", this.redWeight);
            fileHandle.WriteDouble(segName, "green_weight", this.greenWeight);
            fileHandle.WriteDouble(segName, "blue_weight", this.blueWeight);
        }
    }

    public class LuminanceCalibration : ICalibration
    {
        public LuminanceCalibration(Ca310Pipe pipe, Fixture fixture, Colorimeter colorimeter, IntegratingSphere integrate)
        {
            this.ca310Pipe = pipe;
            this.fixture = fixture;
            this.camera = colorimeter;
            this.integrate = integrate;
            this.ReadProfile();
        }

        private Ca310Pipe ca310Pipe;
        private IntegratingSphere integrate;
        private float minExp = 1, maxExp = 4000;
        private float runExp;
        private System.Windows.Forms.PictureBox videoCavaus;

        public float OptimalExposure
        {
            get;
            private set;
        }
        
        private double[] Mean(int[, ,] color)
        {
            double[] v = new double[3];
            int count = color.GetLength(0) * color.GetLength(1);

            for (int i = 0; i < color.GetLength(0); i++)
            {
                for (int j = 0; j < color.GetLength(1); j++)
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

        public void SetVideoCavaus(System.Windows.Forms.PictureBox cavaus)
        {
            this.videoCavaus = cavaus;
        }

        public override void Calibration(float exposure = 0)
        {
            try {
                runExp = exposure;
                maxExp = exposure;
                fixture.HoldOut();
                integrate.MoveTestPos();
                //System.Windows.Forms.MessageBox.Show("Software will waitting 5 minutes");
                //System.Threading.Thread.Sleep(300000);
                fixture.RotateOn();
                CIE1931Value value = ca310Pipe.GetCa310Data();
                Console.WriteLine(value.ToString());
                fixture.RotateOff();
                fixture.CameraDown();
                fixture.MotorMove(150000);
                System.Threading.Thread.Sleep(26000);

                do {
                    camera.ExposureTime = runExp;
                    System.Drawing.Bitmap bitmap = camera.GrabImage();

                    if (this.videoCavaus != null) {
                        this.videoCavaus.Image = System.Drawing.Image.FromHbitmap(bitmap.GetHbitmap());
                    }
                    
                    double[] rgbMean = this.Mean(this.BitmapToRGB(bitmap));

                    if (rgbMean[0] > 230 || rgbMean[1] > 230 || rgbMean[2] > 230)
                    {
                        maxExp = runExp;
                        runExp = (runExp + minExp) / 2;
                    }
                    else if (rgbMean[0] < 215 || rgbMean[1] < 215 || rgbMean[2] < 215)
                    {
                        minExp = runExp;
                        runExp = (runExp + maxExp) / 2;
                    }
                    else
                    {
                        OptimalExposure = runExp;
                        break;
                    }

                    if (Math.Abs(minExp) == Math.Abs(maxExp)) {
                        throw new Exception("Luminance calibration fail.");
                    }
                }
                while (true);
            }
            catch (Exception e) {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
            finally {
                if (fixture != null) {
                    fixture.CameraUp();
                    fixture.Reset();
                }                    
                if (integrate != null)
                    integrate.MoveReadyPos();
            }
        }
    }

    public class ColorCalibration : ICalibration
    {
        public ColorCalibration(DUTclass.DUT dut, Ca310Pipe pipe, Fixture fixture, Colorimeter colorimeter)
        {
            this.dut = dut;
            this.ca310Pipe = pipe;
            this.fixture = fixture;
            this.camera = colorimeter;
            this.rgbList = new List<int[]>();
            FULLNAME = this.filepath + @"\RGB.txt";
            PATH = this.filepath + @"\ColorCalibration\";
            Directory.CreateDirectory(PATH);
            this.ReadProfile();
        }

        private readonly string FULLNAME;
        private readonly string PATH;

        private Ca310Pipe ca310Pipe;
        private DUTclass.DUT dut;
        private List<int[]> rgbList;

        private void ReadRGBConfig()
        {
            if (!File.Exists(FULLNAME)) {
                throw new Exception("Error");
            }

            string data = null;
            rgbList.Clear();

            using (StreamReader sr = new StreamReader(FULLNAME)) {
                do {
                    data = sr.ReadLine();                    

                    if (data != null) {
                        int[] rgb = new int[3];
                        string[] rgbSet = data.Split(',');

                        if (rgbSet.Length == 3) {
                            rgb[0] = int.Parse(rgbSet[0]);
                            rgb[1] = int.Parse(rgbSet[1]);
                            rgb[2] = int.Parse(rgbSet[2]);
                        }

                        rgbList.Add(rgb);
                    }
                }
                while(data != null);

                sr.Close();
            }
        }

        private void WriteMatrixData( List<double[]> matrix, string filename)
        {
            StringBuilder matrixBuildStr = new StringBuilder();
            string fullname = string.Format("{0}{1}_{2}.txt", PATH, this.serialNumber, filename);

            using (StreamWriter sw = new StreamWriter(fullname, true))
            {
                foreach (double[] line in matrix)
                {
                    matrixBuildStr.AppendFormat("{0},{1},{2}\r\n", line[0], line[1], line[2]);
                }
                sw.Write(matrixBuildStr.ToString());
                sw.WriteLine();
                sw.Flush();
                sw.Close();
            }
        }

        // calibrate the xyz
        private void CalibrateXYZ()
        {
            List<double[]> xyzList = new List<double[]>();

            this.ReadRGBConfig();
            fixture.RotateOff();
            fixture.IntegratingSphereDown();
            fixture.HoldIn();
            fixture.BatteryOn();            
            fixture.RotateOn(); // ready ca310
            while (!dut.CheckDUT()) { System.Threading.Thread.Sleep(100); }
            System.Threading.Thread.Sleep(10000);

            foreach (int[] rgb in this.rgbList)
            {
                double[] xyz = new double[3];

                if (rgb.Length == 3)
                {
                    if (dut.ChangePanelColor(rgb[0], rgb[1], rgb[2]))
                    {
                        System.Threading.Thread.Sleep(5000);
                        CIE1931Value cie = ca310Pipe.GetCa310Data();
                        xyz[0] = cie.x; xyz[1] = cie.y; xyz[2] = cie.Y;
                        xyzList.Add(xyz);
                    }
                    else
                    {
                        xyz[0] = 0; xyz[1] = 0; xyz[2] = 0;
                        xyzList.Add(xyz);
                    }
                }
            }

            fixture.RotateOff();
            this.WriteMatrixData(xyzList, "xyz");
        }
        // calibrate the rgb
        private void CalibrateRGB(float exposureTime)
        {
            StringBuilder matrixBuildStr = new StringBuilder();
            string fullname = string.Format("{0}{1}_RGB.txt", PATH, this.serialNumber);

            List<double[, ,]> rgbValue = new List<double[, ,]>();
            System.Drawing.Bitmap bitmap = null;
            System.Drawing.Color pixel;

            foreach (int[] item in this.rgbList)
            {
                matrixBuildStr.AppendFormat("[Set Panel's RGB = ({0},{1},{2})]\r\n", item[0], item[1], item[2]);
                if (dut.ChangePanelColor(item[0], item[1], item[2])) {
                    System.Threading.Thread.Sleep(5000);
                    camera.ExposureTime = this.CalExposureTime(exposureTime, item);
                    bitmap = camera.GrabImage();

                    int w = Convert.ToInt32(System.Math.Round(bitmap.Width * 0.5));
                    int h = Convert.ToInt32(System.Math.Round(bitmap.Height * 0.5));

                    for (int i = h - 5; i < h + 5; i++)
                    {
                        for (int j = w - 5; j < w + 5; j++)
                        {
                            pixel = bitmap.GetPixel(i, j);
                            matrixBuildStr.AppendFormat("({0},{1},{2})", pixel.R, pixel.G, pixel.B);
                        }
                        matrixBuildStr.AppendLine();
                    }
                }
                matrixBuildStr.AppendLine();
            }

            using (StreamWriter sw = new StreamWriter(fullname, true))
            {
                sw.Write(matrixBuildStr.ToString());
                sw.Flush();
                sw.Close();
            }
        }

        /// <summary>
        /// calibrate the xyz
        /// </summary>
        /// <param name="serialnumber"></param>
        public override void Calibration(float exposure)
        {
            this.CalibrateXYZ();
            this.CalibrateRGB(exposure);
        }
    }

    public class FlexCalibration : ICalibration
    {
        public FlexCalibration(DUTclass.DUT dut, Colorimeter colorimeter, imagingpipeline pipe)
        {
            this.ReadProfile();
            this.dut = dut;
            this.pipe = pipe;
            this.camera = colorimeter;
            FULLNAME = this.filepath + @"\RGB.txt";
            this.PATH = this.filepath + @"\FlexCalibration\";
            Directory.CreateDirectory(PATH);
            this.rgbList = new List<int[]>();
            this.ReadRGBConfig();
        }

        private readonly string FULLNAME;
        private readonly string PATH;
        private DUTclass.DUT dut;
        private imagingpipeline pipe;
        private List<int[]> rgbList;
        private System.Windows.Forms.PictureBox videoCavaus;

        private void ReadRGBConfig()
        {
            if (!File.Exists(FULLNAME))
            {
                throw new Exception("Error");
            }

            string data = null;
            rgbList.Clear();

            using (StreamReader sr = new StreamReader(FULLNAME))
            {
                do {
                    data = sr.ReadLine();

                    if (data != null)
                    {
                        int[] rgb = new int[3];
                        string[] rgbSet = data.Split(',');

                        if (rgbSet.Length == 3)
                        {
                            rgb[0] = int.Parse(rgbSet[0]);
                            rgb[1] = int.Parse(rgbSet[1]);
                            rgb[2] = int.Parse(rgbSet[2]);
                        }

                        rgbList.Add(rgb);
                    }
                }
                while (data != null);

                sr.Close();
            }
        }

        public void SetVideoCavaus(System.Windows.Forms.PictureBox cavaus)
        {
            this.videoCavaus = cavaus;
        }

        public override void Calibration(float exposure = 0)
        {
            double flexPixel = 0;
            double maxFlexPixel = 0;
            System.Drawing.Color pixel;
            StringBuilder matrixStr = new StringBuilder();
            System.Threading.Thread.Sleep(3000);

            foreach (int[] item in rgbList)
            {
                List<AForge.IntPoint> cors = null;

                if (dut.ChangePanelColor(item[0], item[1], item[2])) {
                    System.Threading.Thread.Sleep(5000);
                    matrixStr.Clear();

                    camera.ExposureTime = this.CalExposureTime(exposure, item);
                    System.Drawing.Bitmap bitmap = camera.GrabImage();
                    if (this.videoCavaus != null) {
                        this.videoCavaus.Image = System.Drawing.Image.FromHbitmap(bitmap.GetHbitmap());
                    }
                    //Process Image to 1bpp to increase SNR 
                    Bitmap m_orig = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
                    // only support the 32bppArgb for Aforge Blob Counter
                    Bitmap processbmp = m_orig.Clone(new Rectangle(0, 0, m_orig.Width, m_orig.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    pipe.GetDisplayCornerfrombmp(processbmp, out cors);

                    Bitmap cropimg = null;

                    if (cors != null) {
                        cropimg = pipe.croppedimage(bitmap, cors, dut.ui_width, dut.ui_height);
                        if (this.videoCavaus != null) {
                            this.videoCavaus.Image = System.Drawing.Image.FromHbitmap(bitmap.GetHbitmap());
                        }
                    }

                    double[,] matrix = new double[cropimg.Width, cropimg.Height];

                    for (int i = 0; i < cropimg.Height; i++)
                    {
                        for (int j = 0; j < cropimg.Width; j++)
                        {
                            pixel = cropimg.GetPixel(i, j);
                            flexPixel = redWeight * pixel.R + greenWeight * pixel.G + blueWeight * pixel.B;
                            matrix[i, j] = flexPixel;

                            if (flexPixel > maxFlexPixel)
                            {
                                maxFlexPixel = flexPixel;
                            }
                        }
                    }

                    for (int i = 0; i < cropimg.Height; i++)
                    {
                        for (int j = 0; j < cropimg.Width; j++)
                        {
                            if (matrix[i, j] != 0) {
                                matrix[i, j] = maxFlexPixel / matrix[i, j];
                            }

                            matrixStr.Append(matrix[i, j]);

                            if (j != cropimg.Width - 1)
                            {
                                matrixStr.Append(", ");
                            }
                        }
                        matrixStr.AppendLine();
                    }

                    string fullname = string.Format("{0}{1}_{2:000}{3:000}{4:000}_Flex.csv", PATH, serialNumber, item[0], item[1], item[2]);
                    using (StreamWriter sw = new StreamWriter(fullname, true))
                    {
                        sw.Write(matrixStr.ToString());
                        sw.Flush();
                        sw.Close();
                    }
                }
            }           
        }
    }
}
