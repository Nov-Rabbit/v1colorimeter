using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace X2DisplayTest
{
    public partial class FrmSetting : Form
    {
        public FrmSetting(Config config, List<TestItem> items)
        {
            InitializeComponent();
            this.devManage = DevManage.Instance;
            this.config = config;
            this.allItems = items;
        }

        private Button preActiveBtn;
        private DevManage devManage;

        private List<TestItem> allItems;
        private FeaturePanel feature;
        private Config config;

        private DUTclass.DUT dut;
        public DUTclass.DUT ActiveDUT
        {
            get {
                return dut;
            }
        }

        private void FrmSetting_Load(object sender, EventArgs e)
        {
            foreach (string deviceName in devManage.Devices.Keys)
            {
                cbDevice.Items.Add(deviceName);
            }

            if (cbDevice.Items.Count > 0) {
                cbDevice.SelectedIndex = 0;
            }
            
             //product choice
            if (config.ProductType == "Hodor") {
                rbHodor.Checked = true;
                rbBran.Checked = false;
            }
            else if (config.ProductType == "Bran") {
                rbHodor.Checked = false;
                rbBran.Checked = true;
            }

            int i = 0;

            foreach (TestItem item in allItems)
            {
                Button btn = new Button();
                btn.Text = item.TestName;
                btn.FlatStyle = FlatStyle.Popup;
                btn.Tag = i;
                btn.Click += new EventHandler(btn_Click);
                btn.Location = new Point((scMain.Panel1.Width - btn.Width)/2, 10 +(i++) * btn.Size.Height);
                scMain.Panel1.Controls.Add(btn);
            }

            this.preActiveBtn = scMain.Panel1.Controls[0] as Button;
            this.preActiveBtn.PerformClick();
        }

        private void btn_Click(object sender, EventArgs e)
        {
            if (this.preActiveBtn != null) {
                this.preActiveBtn.BackColor = SystemColors.Control;
                this.preActiveBtn.ForeColor = SystemColors.WindowText;
            }

            Button button = sender as Button;
            button.BackColor = Color.DarkCyan;
            button.ForeColor = Color.White;
            feature = new FeaturePanel(allItems[(int)button.Tag]);
            scMain.Panel2.Controls.Clear();
            scMain.Panel2.Controls.Add(feature);
            this.preActiveBtn = button;
        }

        private void cbDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (panel.Controls.Count > 0)
            {
                panel.Controls.Clear();
            }
            panel.Controls.Add(devManage.Devices[cbDevice.SelectedItem.ToString()].DeviceConfigPanel);
        }

        private void FrmSetting_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (rbHodor.Checked) {
                config.ProductType = "Hodor";
            }
            else {
                config.ProductType = "Bran";
            }
            dut = (DUTclass.DUT)Activator.CreateInstance(Type.GetType("DUTclass." + config.ProductType));

            config.WriteProfile();
        }

        private void cbSFC_CheckedChanged(object sender, EventArgs e)
        {
            config.IsOnlineShopfloor = cbSFC.Checked;
            config.IsScanSerialNumber = cbSN.Checked;
            //isCheckShopfloor = cbSFC.Checked;
        }
    }
}
