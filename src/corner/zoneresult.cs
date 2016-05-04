//=============================================================================
// Main zone analysis. display test revolved with zones. This class will resize
// XYZ matrix (which is already in mm binning level for this case) to different 
// zones and perform uniformity analysis.
//=============================================================================

// Location 1 : Top left zone with 10% padding
// Location 2:  Top right zone with 10% padding
// Location 3:  Center Zone
// Location 4:  Bottom right zone with 10% padding
// Location 5:  Bottom left zone with 10% padding

// If location is a point pair to tell the relative XY coordinates in mm level. To be done later.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge;

namespace Imageprocess
{
    public class ZoneResult
    {
        private int mLocation;
        public int Location
        {
            get { return mLocation; }
            set { mLocation = value; }
        }

        private int mzonesize;
        public int ZoneSize
        {
            get { return mzonesize; }
            set { mzonesize = value; }
        }

        private double[, ,] XYZlocal; // XYZ tristimulus value matrix in this zone
        private List<IntPoint> points = new List<IntPoint>();
        System.Drawing.Point pp, pp1, pp2, pp3, pp4 = new System.Drawing.Point();

        /// <summary>
        /// get the local zone XYZ matrix by inputting the predefined zone. 
        /// </summary>
        /// <param name="Location"></param>
        /// <param name="zonesize"></param>
        /// <param name="XYZ"></param>
        /// <returns></returns>
        public double[, ,] XYZLocalZone(int Location, int zonesize, double[, ,] XYZ)
        {
            mLocation = Location;
            int[] xy_index = ZoneIndex(mLocation, zonesize, XYZ);
            XYZlocal = CropXYZ(xy_index[0], xy_index[1], xy_index[2], xy_index[3], XYZ);
            return XYZlocal;
        }

        // get the local XYZ matrix by inputting the starting and endding xy cooordinate in mm 
        // same as the XYZ raw in this study.
        private double[,,] CropXYZ(int x_start, int x_end, int y_start, int y_end, double[, ,] XYZ)
        {

            double[, ,] XYZlocal = new double[x_end - x_start, y_end - y_start, XYZ.GetLength(2)];

            for (int k = 0; k < XYZ.GetLength(2); k++)
            {
                for (int i = x_start; i < x_end; i++)
                {
                    for (int j = y_start; j < y_end; j++)
                    {
                        XYZlocal[i - x_start, j - y_start, k] = XYZ[i, j, k];
                    }
                }
            }
            return XYZlocal;

        }

        // derive the xy coordinate in mm from the predefined zone location and zonesize.
        private int[] ZoneIndex(int Location, int zonesize, double[, ,] XYZ)
        {
            int w = XYZ.GetLength(0);
            int h = XYZ.GetLength(1);
            int x_start, y_start;
            int[] zoneindex = new int[4]; // start x, start y, and end x, end y;

            switch (Location)
            {
                case 1:
                    x_start = Convert.ToInt32(System.Math.Round(w * 0.1));
                    y_start = Convert.ToInt32(System.Math.Round(h * 0.1));
                    break;
                case 2:
                    x_start = Convert.ToInt32(System.Math.Round(w * 0.9)) - zonesize;
                    y_start = Convert.ToInt32(System.Math.Round(h * 0.1));
                    break;
                case 3:
                    x_start = Convert.ToInt32(System.Math.Round(w * 0.9)) - zonesize;
                    y_start = Convert.ToInt32(System.Math.Round(h * 0.9)) - zonesize;
                    break;
                case 4:
                    x_start = Convert.ToInt32(System.Math.Round(w * 0.1));
                    y_start = Convert.ToInt32(System.Math.Round(h * 0.9)) - zonesize;
                    break;
                case 5:
                    x_start = Convert.ToInt32(System.Math.Round(w * 0.5)) - zonesize/2;
                    y_start = Convert.ToInt32(System.Math.Round(h * 0.5)) - zonesize/2;
                    break;
                default:
                    x_start = 1;
                    y_start = 1;
                    break;
            }

            zoneindex[0] = x_start;
            zoneindex[1] = x_start + zonesize;
            zoneindex[2] = y_start;
            zoneindex[3] = y_start + zonesize;

            return zoneindex;
        }

        /// <summary>
        /// derive the point coordinate in pixel from the predefined zone location and zonesize.
        /// will be used for UI drawing if needed
        /// </summary>
        /// <param name="Location"></param>
        /// <param name="zonesize"></param>
        /// <param name="XYZ"></param>
        /// <returns></returns>
        public List<IntPoint> ZoneCorners(int Location, int zonesize, double[, ,] XYZ)
        {
            int w = XYZ.GetLength(0);
            int h = XYZ.GetLength(1);

            switch (Location)
            {
                case 1:
                    // zone 1 : left top
                    pp1.X = Convert.ToInt32(System.Math.Round(w * 0.1));
                    pp1.Y = Convert.ToInt32(System.Math.Round(h * 0.1));
                    pp2.X = pp1.X + zonesize;
                    pp2.Y = pp1.Y;
                    pp3.X = pp1.X + zonesize;
                    pp3.Y = pp1.Y + zonesize;
                    pp4.X = pp1.X;
                    pp4.Y = pp1.Y + zonesize;
                    break;
                case 2:
                    // zone 2 : right top
                    pp2.X = Convert.ToInt32(System.Math.Round(w * 0.9));
                    pp2.Y = Convert.ToInt32(System.Math.Round(h * 0.1));
                    pp1.X = pp2.X - zonesize;
                    pp1.Y = pp2.Y;
                    pp3.X = pp2.X;
                    pp3.Y = pp2.Y + zonesize;
                    pp4.X = pp2.X - zonesize;
                    pp4.Y = pp2.Y + zonesize;
                    break;
                case 3:
                    // zone  3:  Right Bottom
                    pp3.X = Convert.ToInt32(System.Math.Round(w * 0.9));
                    pp3.Y = Convert.ToInt32(System.Math.Round(h * 0.9));
                    pp1.X = pp3.X - zonesize;
                    pp1.Y = pp3.Y - zonesize;
                    pp2.X = pp3.X;
                    pp2.Y = pp3.Y - zonesize;
                    pp4.X = pp3.X - zonesize;
                    pp4.Y = pp3.Y;
                    break;
                case 4:
                    // zone 4: Left Bottom
                    pp4.X = Convert.ToInt32(System.Math.Round(w * 0.1));
                    pp4.Y = Convert.ToInt32(System.Math.Round(h * 0.9));
                    pp1.X = pp4.X;
                    pp1.Y = pp4.Y - zonesize;
                    pp2.X = pp4.X + zonesize;
                    pp2.Y = pp4.Y - zonesize;
                    pp3.X = pp4.X + zonesize;
                    pp3.Y = pp4.Y;
                    break;
                case 5:
                    // zone 5 : center
                    pp.X = Convert.ToInt32(System.Math.Round(w * 0.5));
                    pp.Y = Convert.ToInt32(System.Math.Round(h * 0.5));
                    pp1.X = pp.X - zonesize / 2;
                    pp1.Y = pp.Y - zonesize / 2;
                    pp2.X = pp.X + zonesize / 2;
                    pp2.Y = pp.Y - zonesize / 2;
                    pp3.X = pp.X + zonesize / 2;
                    pp3.Y = pp.Y + zonesize / 2;
                    pp4.X = pp.X - zonesize / 2;
                    pp4.Y = pp.Y + zonesize / 2;
                    break;
                default:
                    pp1.X = 1;
                    pp1.Y = 1;
                    pp2.X = w;
                    pp2.Y = 1;
                    pp3.X = w;
                    pp3.Y = h;
                    pp4.X = 1;
                    pp4.Y = h;
                    break;
            }

            points.Add(new IntPoint(pp1.X, pp1.Y));
            points.Add(new IntPoint(pp2.X, pp2.Y));
            points.Add(new IntPoint(pp3.X, pp3.Y));
            points.Add(new IntPoint(pp4.X, pp4.Y));

            return points;
        }
    }
}
