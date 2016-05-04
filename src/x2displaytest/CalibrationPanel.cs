using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace X2DisplayTest
{
    public partial class CalibrationPanel : UserControl
    {
        public CalibrationPanel(ICalibration calibration)
        {
            InitializeComponent();
            this.calibration = calibration;
        }

        private ICalibration calibration;

        private void CalibrationPanel_Load(object sender, EventArgs e)
        {
            nudRed.Value = (decimal)calibration.RedWeight;
            nudGreen.Value = (decimal)calibration.GreenWeight;
            nudBlue.Value = (decimal)calibration.BlueWeight;
        }

        private void CalibrationPanel_Leave(object sender, EventArgs e)
        {
            calibration.RedWeight = (float)nudRed.Value;
            calibration.GreenWeight = (float)nudGreen.Value;
            calibration.BlueWeight = (float)nudBlue.Value;
        }

        private void Weight_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown nud = (NumericUpDown)sender;

            if ((nudRed.Value + nudGreen.Value + nudBlue.Value) >= 1)
            {
                nudBlue.Value = 1 - nudRed.Value - nudGreen.Value;
            }

            if ((nudRed.Value + nudGreen.Value) >= 1) 
            {
                nudBlue.Value = 0;
                nudGreen.Value = 1 - nudRed.Value;
            }

            if (nud == nudRed) {
                if (nud.Value == 1) {
                    nudGreen.Value = 0;
                    nudBlue.Value = 0;
                }
            }
            else if (nud == nudGreen) {
                if (nud.Value == 1) {
                    nudRed.Value = 0;
                    nudBlue.Value = 0;
                }
            }
            else if (nud == nudBlue) {
                if (nud.Value == 1) {
                    nudRed.Value = 0;
                    nudGreen.Value = 0;
                }
            }
        }
    }
}
