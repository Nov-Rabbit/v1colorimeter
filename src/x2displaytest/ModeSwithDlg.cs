using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace X2DisplayTest
{
    public partial class ModeSwithDlg : Form
    {
        public ModeSwithDlg()
        {
            InitializeComponent();
        }

        public bool IsTestPanel {
            get {
                return rbTestPanel.Checked; 
            }
        }

        public bool IsAnalysisPanel {
            get {
                return rbAnalysisPanel.Checked;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void ModeSwithDlg_Load(object sender, EventArgs e)
        {

        }
    }
}
