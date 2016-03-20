using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Colorimeter_Config_GUI
{
    public abstract class ICalibration
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
    }

    public class LuminanceCalibration : ICalibration
    {
        public LuminanceCalibration(Ca310Pipe pipe, Fixture fixture, Colorimeter colorimeter)
        {
            this.ca310Pipe = pipe;
            this.fixture = fixture;
            this.camera = colorimeter;
        }

        private Ca310Pipe ca310Pipe;
        private float minExp = 1, maxExp = 100;
        private float runExp;

        public float OptimalExposure
        {
            get;
            private set;
        }
        
        private double[] Mean(int[, ,] color)
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

        public override void Calibration(float exposure = 0)
        {
            try {
                runExp = exposure;
                fixture.IntegratingSphereUp();
                fixture.RotateOn();
                System.Threading.Thread.Sleep(1000);
                CIE1931Value value = ca310Pipe.GetCa310Data();
                fixture.RotateOff();
                System.Threading.Thread.Sleep(1000);

                do {
                    camera.ExposureTime = runExp;
                    System.Drawing.Bitmap bitmap = camera.GrabImage();
                    double[] rgbMean = this.Mean(this.BitmapToRGB(bitmap));

                    if (rgbMean[0] > 225 || rgbMean[1] > 255 || rgbMean[2] > 255) {
                        maxExp = runExp;
                        runExp = (runExp + minExp) / 2;                        
                    }
                    else if (rgbMean[0] < 215 || rgbMean[1] < 215 || rgbMean[2] < 215) {
                        minExp = runExp;
                        runExp = (runExp + maxExp) / 2;
                    }
                    else {
                        OptimalExposure = runExp;
                        break;
                    }
                }
                while (true);

                fixture.IntegratingSphereDown();
                System.Threading.Thread.Sleep(1000);
            }
            catch (Exception e) {

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
            string fullname = string.Format("{0}\\{1}_{2}.txt", PATH, this.serialNumber, filename);

            using (StreamWriter sw = new StreamWriter(fullname, true))
            {
                foreach (double[] line in matrix)
                {
                    matrixBuildStr.AppendFormat("{0},{1},{2}\r\n", line[0], line[1], line[2]);
                }
                sw.Write(matrixBuildStr.ToString());
                sw.Flush();
                sw.Close();
            }
        }

        private float CalExposureTime(float exposureTime, int[] rgb)
        {        
            double faction = (redWeight * rgb[0] + greenWeight * rgb[1] + blueWeight * rgb[1]) / 255;
            float exposure = (float)(exposureTime / faction);
            return exposure;
        }
        // calibrate the xyz
        private void CalibrateXYZ()
        {
            List<double[]> xyzList = new List<double[]>();

            this.ReadRGBConfig();
            fixture.RotateOn(); // ready ca310

            foreach (int[] rgb in this.rgbList)
            {
                double[] xyz = new double[3];

                if (rgb.Length == 3)
                {
                    if (dut.ChangePanelColor(rgb[0], rgb[1], rgb[2]))
                    {
                        System.Threading.Thread.Sleep(1000);
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
            string fullname = string.Format("{0}\\{1}_RGB.txt", PATH, this.serialNumber);

            List<double[, ,]> rgbValue = new List<double[, ,]>();
            System.Drawing.Bitmap bitmap = null;
            System.Drawing.Color pixel;

            foreach (int[] item in this.rgbList)
            {
                matrixBuildStr.AppendFormat("[Set Panel's RGB = ({0},{1},{2})]\r\n", item[0], item[1], item[2]);
                if (dut.ChangePanelColor(item[0], item[1], item[2])) {
                    System.Threading.Thread.Sleep(1000);
                    camera.ExposureTime = this.CalExposureTime(exposureTime, item);
                    bitmap = camera.GrabImage();

                    for (int i = 0; i < bitmap.Height; i++)
                    {
                        for (int j = 0; j < bitmap.Width; j++)
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
        public FlexCalibration(Colorimeter colorimeter)
        {
            this.camera = colorimeter;
            this.PATH = this.filepath + @"\FlexCalibration\";
        }

        private readonly string PATH;

        public override void Calibration(float exposure = 0)
        {
            double flexPixel = 0;
            double maxFlexPixel = 0;
            System.Drawing.Color pixel;
            StringBuilder matrixStr = new StringBuilder();

            camera.ExposureTime = exposure;
            System.Drawing.Bitmap bitmap = camera.GrabImage();
            double[,] matrix = new double[bitmap.Width, bitmap.Height];

            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width; j++)
                {
                    pixel = bitmap.GetPixel(i, j);
                    flexPixel = redWeight * pixel.R + greenWeight * pixel.G + blueWeight * pixel.B;
                    matrix[i, j] = flexPixel;

                    if (flexPixel > maxFlexPixel) {
                        maxFlexPixel = flexPixel;
                    }
                }
            }

            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width; j++)
                {
                    matrix[i, j] = maxFlexPixel / matrix[i, j];
                    matrixStr.Append(matrix[i, j]);

                    if (j != bitmap.Width - 1) {
                        matrixStr.Append(", ");
                    }
                }
                matrixStr.AppendLine();
            }

            string fullname = string.Format("{0}\\{1}_RGB.txt", PATH, this.serialNumber);
            using (StreamWriter sw = new StreamWriter(fullname, true))
            {
                sw.Write(matrixStr.ToString());
                sw.Flush();
                sw.Close();
            }
        }
    }
}
