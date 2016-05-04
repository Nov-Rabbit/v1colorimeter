using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Windows.Forms;
using System.Drawing;
using AForge;
using AForge.Imaging;
using AForge.Math;
using AForge.Math.Geometry;
using AForge.Imaging.Filters;

using FlyCapture2Managed;

namespace Imageprocess
{
    public class Corner
    {
        private List<IntPoint> flagPoints;

        /// <summary>
        /// Crop image
        /// </summary>
        /// <param name="src">orignal image</param>
        /// <param name="displaycornerPoints"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Bitmap CroppedImage(Bitmap src, List<IntPoint> displaycornerPoints, int width, int height)
        {
            //Create crop filter
            SimpleQuadrilateralTransformation filter
                = new SimpleQuadrilateralTransformation(displaycornerPoints, width, height);
            //Create cropped display image
            Bitmap des = filter.Apply(src);

            return des;
        }

        public List<IntPoint> GetDisplayCorner(Bitmap bitmap)
        {
            BlobCounter bbc = new BlobCounter();
            bbc.FilterBlobs = true;
            bbc.MinHeight = 5;
            bbc.MinWidth = 5;
            bbc.ProcessImage(bitmap);

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
                        continue;
                    }
                }
            }

            return flagPoints;
        }

        public List<IntPoint> GetDisplayCorner(ManagedImage processedImage)
        {
            // get display corner position 
            //Process Image to 1bpp to increase SNR 
            Bitmap bitmap = processedImage.bitmap;
            Bitmap bmpOrignal = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
            // only support the 32bppArgb for Aforge Blob Counter
            Bitmap processbmp = bmpOrignal.Clone(new Rectangle(0, 0, bmpOrignal.Width, bmpOrignal.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            return GetDisplayCorner(processbmp);
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
                        continue;
                    }
                }
            }

            if (flagPoints == null)
                throw new Exception();
            //  MessageBox.Show("Cannot Find the Display");

            displaycornerPoints = flagPoints;
        }

        public void GetDisplayCorner(ManagedImage processedImage, out List<IntPoint> displaycornerPoints)
        {
            // get display corner position 
            //Process Image to 1bpp to increase SNR 
            Bitmap bitmap = processedImage.bitmap;
            Bitmap bmpOrignal = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
            // only support the 32bppArgb for Aforge Blob Counter
            Bitmap processbmp = bmpOrignal.Clone(new Rectangle(0, 0, bmpOrignal.Width, bmpOrignal.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            this.GetDisplayCornerfrombmp(processbmp, out displaycornerPoints);
        }
    }
}
