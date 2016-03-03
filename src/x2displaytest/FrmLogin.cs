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
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        private const string password = "miCrotEst";

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (mtbPasscode.Text == password)
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {           
                this.DialogResult = DialogResult.Cancel;
                MessageBox.Show("Error password.");
            }
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            mtbPasscode.Focus();
        }        
    }
}
