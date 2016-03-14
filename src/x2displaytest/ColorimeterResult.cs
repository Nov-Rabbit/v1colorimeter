using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Colorimeter_Config_GUI
{
    public enum ColorPanel
    {
        White,
        Black,
        Red,
        Green,
        Blue,
    }

    public class CIE1931Value
    {
        public double x { get; set; }
        public double y { get; set; }
        public double Y { get; set; }

        public override string ToString()
        {
            return string.Format("([{0}, {1}], {2})", x, y, Y);
        }

        public CIE1931Value Copy()
        {
            CIE1931Value cie = new CIE1931Value();
            cie.x = this.x;
            cie.y = this.y;
            cie.Y = this.Y;

            return cie;
        }
    }

    public class ColorimeterResult
    {
        private Bitmap m_bitmap;
        private ColorPanel m_panel;
        private imagingpipeline m_pipeline;

        public double Luminance { get; private set; }
        public double Uniformity5 { get; private set; }
        public double Mura { get; private set; }
        public CIE1931Value CIE1931xyY { get; set; }

        public ColorimeterResult(Bitmap bitmap, ColorPanel panel)
        {
            this.m_bitmap = bitmap;
            this.m_panel = panel;
            m_pipeline = new imagingpipeline();
            CIE1931xyY = new CIE1931Value();
        }

        public void Analysis()
        {
            if (m_bitmap != null)
            {
                double[, ,] rgb = m_pipeline.bmp2rgb(m_bitmap);
                double[, ,] XYZ = m_pipeline.rgb2xyz(rgb);

                switch (m_panel)
                {
                    case ColorPanel.White:
                    case ColorPanel.Black:
                        this.Luminance = m_pipeline.getlv(XYZ);
                        this.Uniformity5 = m_pipeline.getuniformity(XYZ);
                        this.Mura = m_pipeline.getmura(XYZ);
                        break;
                    case ColorPanel.Red:
                    case ColorPanel.Green:
                    case ColorPanel.Blue:
                        {
                            double[] xyY = m_pipeline.getxyY(XYZ);
                            CIE1931xyY.x = xyY[0];
                            CIE1931xyY.y = xyY[1];
                            CIE1931xyY.Y = xyY[2];
                        }
                        break;
                }
            }
        }
    }
}
