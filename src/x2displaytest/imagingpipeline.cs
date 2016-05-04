//=============================================================================
// Imaging Pipeline for display test. after zone parsing and resizing, the next 
// step is to process the XYZ data to meaningful display test metric. If possible
// there might be need to convert to other color space like La*b* 
//=============================================================================




using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using AForge;
using AForge.Imaging;
using AForge.Math;
using AForge.Math.Geometry;
using FlyCapture2Managed;

namespace X2DisplayTest
{
    public class imagingpipeline
    {
        private List<IntPoint> flagPoints;

        public Bitmap croppedimage(Bitmap src, List<IntPoint> displaycornerPoints, int width, int height)
        {
            //Create crop filter
            AForge.Imaging.Filters.SimpleQuadrilateralTransformation filter = new AForge.Imaging.Filters.SimpleQuadrilateralTransformation(displaycornerPoints, width, height);
            //Create cropped display image
            Bitmap des = filter.Apply(src);
            
            return des;
        }

        public void GetDisplayCornerfrombmp(Bitmap processbmp, out List<IntPoint> displaycornerPoints)
        {
            BlobCounter bbc = new BlobCounter();
            bbc.FilterBlobs = true;
            bbc.MinHeight = 5;
            bbc.MinWidth = 5;

            bbc.ProcessImage(processbmp);

            Blob[] blobs = bbc.GetObjectsInformation();
            SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

            foreach (var blob in blobs)
            {
                List<IntPoint> edgePoints = bbc.GetBlobsEdgePoints(blob);
                List<IntPoint> cornerPoints;


                // use the shape checker to extract the corner points
                if (shapeChecker.IsQuadrilateral(edgePoints, out cornerPoints))
                {
                    // only do things if the corners from a rectangle 
                    if (shapeChecker.CheckPolygonSubType(cornerPoints) == PolygonSubType.Rectangle)
                    {
                        flagPoints = cornerPoints;
                        continue;
                    }
                    else
                    {                        
                        flagPoints = null;
                        //                       picturebox_test.Image = m;
                        continue;
                    }
                }

            }

            if (flagPoints == null)
                MessageBox.Show("Cannot Find the Display");

            displaycornerPoints = flagPoints;

        }

        public void GetDisplayCorner(ManagedImage m_processedImage, out List<IntPoint> displaycornerPoints)
        {
            // get display corner position 
            //Process Image to 1bpp to increase SNR 
            Bitmap m_orig = m_processedImage.bitmap.Clone(new Rectangle(0, 0, m_processedImage.bitmap.Width, m_processedImage.bitmap.Height), System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
            // only support the 32bppArgb for Aforge Blob Counter
            Bitmap processbmp = m_orig.Clone(new Rectangle(0, 0, m_orig.Width, m_orig.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            BlobCounter bbc = new BlobCounter();
            bbc.FilterBlobs = true;
            bbc.MinHeight = 5;
            bbc.MinWidth = 5;

            bbc.ProcessImage(processbmp);

            Blob[] blobs = bbc.GetObjectsInformation();
            SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

            foreach (var blob in blobs)
            {
                List<IntPoint> edgePoints = bbc.GetBlobsEdgePoints(blob);
                List<IntPoint> cornerPoints;
                int count = 0;

                // use the shape checker to extract the corner points
                if (shapeChecker.IsQuadrilateral(edgePoints, out cornerPoints))
                {
                    // only do things if the corners from a rectangle 
                    if (shapeChecker.CheckPolygonSubType(cornerPoints) == PolygonSubType.Rectangle)
                    {
                        flagPoints = cornerPoints;
                        continue;
                    }
                    else
                    {
                        MessageBox.Show("Cannot Find the Display");
                        flagPoints = null;
                        //                       picturebox_test.Image = m;
                        count++;
                        if (count < 3)
                        {
                            continue;
                        }
                        else
                        {
                            MessageBox.Show("Cannot Find the Display for 3 times. Quit Program");
                            Environment.ExitCode = -1;
                            Application.Exit();


                        }
                    }
                }

            }
            displaycornerPoints = flagPoints;

        }

        // get X/Y/Z
        private double getweight(double[, ,] XYZ, int index)
        {
            if (index < 0) {
                index = 0;
            }
            else if (index > 2) {
                index = 2;
            }

            double sum = 0;
            double mean = 0;
            float w = XYZ.GetLength(0);
            float h = XYZ.GetLength(1);

            for (int r = 0; r < w; r++)
            {
                for (int c = 0; c < h; c++)
                {
                    sum += XYZ[r, c, index];
                }

            }
            mean = sum / (w * h);
            return mean;
        }

        // get xyY, return:x = xyY[0], y = xyY[1], Y = luminance
        // 5 decimal places
        public double[] getxyY(double[, ,] XYZ)
        {
            double[] xyY = new double[3];
            double X = getweight(XYZ, 0);
            double Y = getweight(XYZ, 1);
            double Z = getweight(XYZ, 2);

            double sum = X + Y + Z;
            xyY[0] = Math.Round(X / sum, 5);
            xyY[1] = Math.Round(Z / sum, 5);
            xyY[2] = Y;

            return xyY;
        }

        // get average Y from input XYZ matrix
        public double getlv(double[, ,] XYZ)
        {
            double sum = 0;
            double mean = 0;
            float w = XYZ.GetLength(0);
            float h = XYZ.GetLength(1);

            for (int r = 0; r < w; r++)
            {
                for (int c = 0; c < h; c++)
                {
                    sum += XYZ[r, c, 1];
                }

            }
            mean = sum / (w * h);
            return mean;
        }

        // get the uniformity by 5 zones.
        public double getuniformity(double[, ,] XYZ)
        {
            zoneresult zr = new zoneresult();
            double[, ,] XYZ1, XYZ2, XYZ3, XYZ4, XYZ5;
            double lv1, lv2, lv3, lv4, lv5;

            XYZ1 = zr.XYZlocalzone(1, 10, XYZ);
            lv1 = getlv(XYZ1);
            XYZ2 = zr.XYZlocalzone(2, 10, XYZ);
            lv2 = getlv(XYZ2);
            XYZ3 = zr.XYZlocalzone(3, 10, XYZ);
            lv3 = getlv(XYZ3);
            XYZ4 = zr.XYZlocalzone(4, 10, XYZ);
            lv4 = getlv(XYZ4);
            XYZ5 = zr.XYZlocalzone(5, 10, XYZ);
            lv5 = getlv(XYZ5);

            double lvmin = new double[]{lv1, lv2, lv3, lv4, lv5}.Min();
            double lvmax = new double[]{lv1, lv2, lv3, lv4, lv5}.Max();
            double unif = lvmin /lvmax;
            return unif;

        }

        public double getmura(double[, ,] XYZ)
        {
            double muraresult = 1;
            return muraresult;
        }
        
        public double[, ,] bmp2rgb(Bitmap processedBitmap)
        {
            int h = processedBitmap.Height;
            int w = processedBitmap.Width;
            double[, ,] rgbstr = new double[w, h, 3];

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    rgbstr[i, j, 0] = processedBitmap.GetPixel(i, j).R;
                    rgbstr[i, j, 1] = processedBitmap.GetPixel(i, j).G;
                    rgbstr[i, j, 2] = processedBitmap.GetPixel(i, j).B;
                }

            }
            return rgbstr;

        }

        public double[, ,] rgb2xyz(double[, ,] rgbstr)
        {
            int w = rgbstr.GetLength(0);
            int h = rgbstr.GetLength(1);
            double[, ,] xyzstr = new double[w, h, 3];
            var ccm = new[,]
               {
                    {0.5767309, 0.2973769, 0.0270343},
                    {0.1855540, 0.6273491, 0.0706872},
                    {0.1881852, 0.0752741, 0.9911085}
               };


            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    double[] rgb = new double[] { rgbstr[i, j, 0], rgbstr[i, j, 1], rgbstr[i, j, 2] };
                    double[] xyz = MultiplyVector(ccm, rgb);
                    xyzstr[i, j, 0] = xyz[0];
                    xyzstr[i, j, 1] = xyz[1];
                    xyzstr[i, j, 2] = xyz[2];
                }
            }

            return xyzstr;
        }

        private double[,] MultiplyMatrix(double[,] A, double[,] B)
        {
            int rA = A.GetLength(0);
            int cA = A.GetLength(1);
            int rB = B.GetLength(0);
            int cB = B.GetLength(1);
            double temp = 0;
            double[,] kHasil = new double[rA, cB];
            if (cA != rB)
            {
                Console.WriteLine("matrik can't be multiplied !!");
                return null;
            }
            else
            {
                for (int i = 0; i < rA; i++)
                {
                    for (int j = 0; j < cB; j++)
                    {
                        temp = 0;
                        for (int k = 0; k < cA; k++)
                        {
                            temp += A[i, k] * B[k, j];
                        }
                        kHasil[i, j] = temp;
                    }
                }
                return kHasil;
            }
        }

        private double[] MultiplyVector(double[,] A, double[] B)
        {
            int rA = A.GetLength(0);
            int cA = A.GetLength(1);
            int rB = B.GetLength(0);
            double temp = 0;
            double[] kHasil = new double[rA];
            if (cA != rB)
            {
                Console.WriteLine("matrik can't be multiplied !!");
                return null;
            }
            else
            {
                for (int i = 0; i < rA; i++)
                {
                    temp = 0;
                    for (int k = 0; k < cA; k++)
                    {
                        temp += A[i, k] * B[k];
                    }
                    kHasil[i] = temp;

                }
                return kHasil;
            }
        }


    }
}
