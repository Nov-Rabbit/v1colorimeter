//=============================================================================
// Imaging Pipeline for display test. after zone parsing and resizing, the next 
// step is to process the XYZ data to meaningful display test metric. If possible
// there might be need to convert to other color space like La*b* 
//=============================================================================




using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colorimeter_Config_GUI
{
    class imagingpipeline
    {

        // get average Y from input XYZ matrix
        public float getlv(float[, ,] XYZ)
        {
            float sum = 0;
            float mean = 0;
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
        public float getuniformity(float[, ,] XYZ)
        {
            zoneresult zr = new zoneresult();
            float[, ,] XYZ1, XYZ2, XYZ3, XYZ4, XYZ5;
            float lv1, lv2, lv3, lv4, lv5;

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

            float lvmin = new float[]{lv1, lv2, lv3, lv4, lv5}.Min();
            float lvmax = new float[]{lv1, lv2, lv3, lv4, lv5}.Max();
            float unif = lvmin /lvmax;
            return unif;

        }

        public float getmura(float[, ,] XYZ)
        {
            float muraresult = 1;
            return muraresult;
        }
    }
}
