using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Colorimeter_Config_GUI
{
    public partial class FeatureParam : UserControl
    {
        public FeatureParam(ColorPanel panel, List<double> param)
        {
            InitializeComponent();

            this.panel = panel;
            this.param = param;
        }

        private ColorPanel panel;
        private List<double> param;

        public List<double> Param
        {
            get {
                return param;
            }
        }

        private void FeatureParam_Load(object sender, EventArgs e)
        {
            switch (this.panel) { 
                case ColorPanel.White:
                case ColorPanel.Black:
                    gbCIE1931.Enabled = false;
                    tbLvUpper.Text = param[0].ToString();
                    tbLvLower.Text = param[1].ToString();
                    tbUnifoUpper.Text = param[2].ToString();
                    tbUnifoLower.Text = param[3].ToString();
                    tbMuraUpper.Text = param[4].ToString();
                    tbMuraLower.Text = param[5].ToString();
                    break;
                case ColorPanel.Red:
                case ColorPanel.Green:
                case ColorPanel.Blue:
                    gbLuminance.Enabled = gbUniformity.Enabled = gbMura.Enabled = false;
                    tbCIE1931xUpper.Text = param[0].ToString();
                    tbCIE1931xLower.Text = param[1].ToString();
                    tbCIE1931yUpper.Text = param[2].ToString();
                    tbCIE1931yLower.Text = param[3].ToString();
                    tbCIE1931zUpper.Text = param[4].ToString();
                    tbCIE1931zLower.Text = param[5].ToString();
                    break;
            }
            ndExrosureTime.Value = (decimal)param[6];
        }

        public void Save()
        {
            switch (this.panel) { 
                case ColorPanel.White:
                case ColorPanel.Black:
                    param[0] = double.Parse(tbLvUpper.Text);
                    param[1] = double.Parse(tbLvLower.Text);
                    param[2] = double.Parse(tbUnifoUpper.Text);
                    param[3] = double.Parse(tbUnifoLower.Text);
                    param[4] = double.Parse(tbMuraUpper.Text);
                    param[5] = double.Parse(tbMuraLower.Text);
                    break;
                case ColorPanel.Red:
                case ColorPanel.Green:
                case ColorPanel.Blue:
                    param[0] = double.Parse(tbCIE1931xUpper.Text);
                    param[1] = double.Parse(tbCIE1931xLower.Text);
                    param[2] = double.Parse(tbCIE1931yUpper.Text);
                    param[3] = double.Parse(tbCIE1931yLower.Text);
                    param[4] = double.Parse(tbCIE1931zUpper.Text);
                    param[5] = double.Parse(tbCIE1931zLower.Text);
                    break;
            }
            param[6] = (double)ndExrosureTime.Value;
        }
    }
}
