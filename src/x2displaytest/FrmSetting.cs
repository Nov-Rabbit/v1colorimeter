using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Colorimeter_Config_GUI
{
    public partial class FrmSetting : Form
    {
        public FrmSetting(List<TestItem> allItems, Fixture fixture)
        {
            InitializeComponent();
            this.allItems = allItems;
            this.fixture = fixture;
        }

        private List<TestItem> allItems;
        private Fixture fixture;
        private FeatureParam feature;
        private Button preSelectBtn;

        private void FrmSetting_Load(object sender, EventArgs e)
        {
            this.PanelSelect_Click(btnWhite, e);
        }

        private void PanelSelect_Click(object sender, EventArgs e)
        {
            Button activeBtn = (sender as Button);
            activeBtn.BackColor = Color.DarkBlue;
            activeBtn.ForeColor = Color.White;
            ColorPanel panel = (ColorPanel)Enum.Parse(typeof(ColorPanel), FrmSetting.UpperFirstChar(activeBtn.Text));

            List<TestNode> testNodes = allItems[(int)panel].SubNodes;
            List<double> panelParam = new List<double>();

            foreach (TestNode node in testNodes)
            {
                panelParam.Add(node.Upper);
                panelParam.Add(node.Lower);
            }
            panelParam.Add(allItems[(int)panel].Exposure);
            feature = new FeatureParam(panel, panelParam);

            if (pnCloth.Controls.Count > 0)
            {
                FeatureParam fp = pnCloth.Controls[0] as FeatureParam;
                fp.Save();

                int index = 0;
                ColorPanel prePanel = (ColorPanel)Enum.Parse(typeof(ColorPanel), FrmSetting.UpperFirstChar(this.preSelectBtn.Text));
                List<TestNode> nodes = allItems[(int)prePanel].SubNodes;

                foreach (TestNode nd in nodes)
                {
                    nd.Upper = fp.Param[index++];
                    nd.Lower = fp.Param[index++];
                }
                allItems[(int)prePanel].Exposure = (float)fp.Param[index];

                pnCloth.Controls.Clear();
                this.preSelectBtn.BackColor = SystemColors.Control;
                this.preSelectBtn.ForeColor = SystemColors.WindowText;
            }

            pnCloth.Controls.Add(feature);
            this.preSelectBtn = activeBtn;
        }

        private static string UpperFirstChar(string str)
        {
            return str.Replace((char)str[0], (char)(str[0] - 32));
        }

        private void FrmSetting_FormClosing(object sender, FormClosingEventArgs e)
        {
            //this.config.WriteProfile();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            int step = (int)nudStep.Value;
            fixture.MotorMove(-Math.Abs(step));
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            int step = (int)nudStep.Value;
            fixture.MotorMove(Math.Abs(step));
        }

        private void btnHolderIn_Click(object sender, EventArgs e)
        {
            fixture.HoldIn();
        }

        private void btnHolderOut_Click(object sender, EventArgs e)
        {
            fixture.HoldOut();
        }

        private void btnIntergeUp_Click(object sender, EventArgs e)
        {
            fixture.IntegratingSphereUp();
        }

        private void btnIntergeDown_Click(object sender, EventArgs e)
        {
            fixture.IntegratingSphereDown();
        }

        private void btnRotateOn_Click(object sender, EventArgs e)
        {
            fixture.RotateOn();
        }

        private void btnRotateOff_Click(object sender, EventArgs e)
        {
            fixture.RotateOff();
        }
    }
}
