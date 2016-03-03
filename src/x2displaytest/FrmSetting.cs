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
        public FrmSetting(Config config)
        {
            InitializeComponent();
            this.config = config;
            this.configParams = this.config.ConfigParams;
        }

        private Dictionary<string, List<double>> configParams;
        private Config config;
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
            feature = new FeatureParam(panel, this.configParams[activeBtn.Text]);

            if (pnCloth.Controls.Count > 0)
            {
                FeatureParam fp = pnCloth.Controls[0] as FeatureParam;
                fp.Save();
                this.configParams[this.preSelectBtn.Text] = fp.Param;
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
            this.config.WriteProfile();
        }
    }
}
