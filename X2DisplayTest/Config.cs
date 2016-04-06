using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hnwlxy.HmzSysPlatform;

namespace Colorimeter_Config_GUI
{
    public class Config
    {
        public string FixturePortName { get; set; }
        public string LCP3005PortName { get; set; }
        public float RedWeight { get; set; }
        public float GreenWeight { get; set; }
        public float BlueWeight { get; set; }
        public bool IsShopfloor { get; set; }
        public string ProductType { get; set; }

        public Config(string path)
        {
            this.FixturePortName = "";
            this.LCP3005PortName = "";
            this.RedWeight = 0.72f;
            this.GreenWeight = 0.18f;
            this.BlueWeight = 0.1f;
            this.IsShopfloor = true;
            this.ProductType = "Hodor";

            this.path = path;
            ini = new HmzIniFile(this.path);
            ini.Create();
        }

        private string path;
        private HmzIniFile ini;

        public void ReadProfile()
        {
            try {
                this.FixturePortName = ini.ReadString("fixture", "portname");
                this.LCP3005PortName = ini.ReadString("lcp3005", "portname");
                this.RedWeight = (float)ini.ReadDouble("calibration", "red_weight");
                this.GreenWeight = (float)ini.ReadDouble("calibration", "green_weight");
                this.BlueWeight = (float)ini.ReadDouble("calibration", "blue_weight");
                this.IsShopfloor = bool.Parse(ini.ReadString("shopfloor", "is_need_check"));
                this.ProductType = ini.ReadString("product", "type");
            }
            catch {
                this.WriteProfile();
            }            
        }

        public void WriteProfile()
        {
            ini.WriteString("fixture", "portname", this.FixturePortName);
            ini.WriteString("lcp3005", "portname", this.LCP3005PortName);
            ini.WriteDouble("calibration", "red_weight", this.RedWeight);
            ini.WriteDouble("calibration", "green_weight", this.GreenWeight);
            ini.WriteDouble("calibration", "blue_weight", this.BlueWeight);
            ini.WriteString("shopfloor", "is_need_check", this.IsShopfloor.ToString());
            ini.WriteString("product", "type", this.ProductType);
        }
    }
}
