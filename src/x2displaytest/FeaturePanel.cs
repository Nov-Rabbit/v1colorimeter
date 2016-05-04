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
    public partial class FeaturePanel : UserControl
    {
        public FeaturePanel(TestItem item)
        {
            InitializeComponent();
            this.testItem = item;
        }

        private Button preActiveBtn;
        private TestItem testItem;
        private Dictionary<string, TestNodePanel> testpanels;

        private void FeatureParam_Load(object sender, EventArgs e)
        {
            this.testpanels = new Dictionary<string, TestNodePanel>();
            tbTestName.Text = this.testItem.TestName;
            nudRed.Value = this.testItem.RGB.R;
            nudGreen.Value = this.testItem.RGB.G;
            nudBlue.Value = this.testItem.RGB.B;
            ndExrosureTime.Value = (decimal)this.testItem.Exposure;
            cbIsNeedTest.Checked = this.testItem.IsNeedTest;

            foreach (TestNode node in this.testItem.TestNodes)
            {
                this.testpanels.Add(node.NodeName, new TestNodePanel(node));
            }
            panelItem.Controls.Clear();
            panelItem.Controls.Add(this.testpanels[btnLv.Text]);
            btnLv.BackColor = Color.DarkCyan;
            btnLv.ForeColor = Color.White;
            preActiveBtn = btnLv;
        }

        private void Item_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            if (preActiveBtn != null) {
                preActiveBtn.BackColor = SystemColors.Control;
                preActiveBtn.ForeColor = SystemColors.WindowText;
            }

            panelItem.Controls.Clear();
            panelItem.Controls.Add(this.testpanels[button.Text]);
            button.BackColor = Color.DarkCyan;
            button.ForeColor = Color.White;
            preActiveBtn = button;
        }

        private void FeaturePanel_Leave(object sender, EventArgs e)
        {
            this.testItem.TestName = tbTestName.Text;
            this.testItem.RGB = Color.FromArgb((int)nudRed.Value, (int)nudGreen.Value, (int)nudBlue.Value);
            this.testItem.Exposure = (double)ndExrosureTime.Value;
            this.testItem.IsNeedTest = cbIsNeedTest.Checked;
        }
    }
}
