using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using AForge;
using AForge.Imaging;
using AForge.Math;
using AForge.Math.Geometry;

using FlyCapture2Managed;


namespace X2DisplayTest
{
    class corner
    {
        private List<IntPoint> flagPoints;

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
                        MessageBox.Show("Cannot Find the Display");
                        flagPoints = null;
                        //                       picturebox_test.Image = m;
                        continue;
                    }
                }

            }
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

    }
}
