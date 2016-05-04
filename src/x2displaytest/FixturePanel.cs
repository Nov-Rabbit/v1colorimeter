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
    public partial class FixturePanel : UserControl
    {
        public FixturePanel(Fixture fixture)
        {
            InitializeComponent();
            this.fixture = fixture;
        }

        private Fixture fixture;

        private void FixturePanel_Load(object sender, EventArgs e)
        {
            cbPort.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());

            if (cbPort.Items.Contains(fixture.PortName)) {
                cbPort.SelectedText = fixture.PortName;
            }
            else {
                //cbPort.SelectedItem = cbPort.Items[0];
            }

            tbInitPos.Text = fixture.InitPosition.ToString();
            tbMeasurePos.Text = fixture.MeasurePosition.ToString();
            //lbLocation.Text = "position: " + fixture.CurrentPosition.ToString();
        }

        private void MotorMove_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int position = (int)nudStep.Value;

            if (btn == btnUp)
                fixture.MotorMove(-Math.Abs(position));
            else if (btn == btnDown)
                fixture.MotorMove(Math.Abs(position));
        }

        private void Cylider_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if (btn == btnFanOn) { fixture.FanOn(); }
            else if (btn == btnFanOff) { fixture.FanOff(); }
            else if (btn == btnHolderIn) { fixture.HoldIn(); }
            else if (btn == btnHolderOut) { fixture.HoldOut(); }
            else if (btn == btnRotateOn) { fixture.RotateOn(); }
            else if (btn == btnRotateOff) { fixture.RotateOff(); }
            else if (btn == btnIntergeUp) { fixture.IntegratingSphereUp(); }
            else if (btn == btnIntergeDown) { fixture.IntegratingSphereDown(); }
        }
    }
}
