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
    public partial class TestNodePanel : UserControl
    {
        public TestNodePanel(TestNode node)
        {
            InitializeComponent();
            this.testNode = node;
        }

        private TestNode testNode;

        private void TestNodePanel_Load(object sender, EventArgs e)
        {
            tbItemName.Text = this.testNode.NodeName;
            tbUpper.Text = this.testNode.Upper.ToString();
            tbLower.Text = this.testNode.Lower.ToString();
            tbUnit.Text = this.testNode.Unit;
            tbErrorCode.Text = this.testNode.Error;
            cbIsNeedTest.Checked = this.testNode.IsNeedTest;
        }

        private void TestNodePanel_Leave(object sender, EventArgs e)
        {
            if (tbItemName.Text == "") {
                tbItemName.BackColor = Color.Red;
                MessageBox.Show("Please type item name.");
            }
            else {
                tbItemName.BackColor = SystemColors.Window;
            }

            this.testNode.NodeName = tbItemName.Text;
            this.testNode.Upper = Convert.ToDouble(tbUpper.Text);
            this.testNode.Lower = Convert.ToDouble(tbLower.Text);
            this.testNode.Unit = tbUnit.Text;
            this.testNode.Error = tbErrorCode.Text;
            this.testNode.IsNeedTest = cbIsNeedTest.Checked;
        }
    }
}
