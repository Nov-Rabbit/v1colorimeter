using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hnwlxy.HmzSysPlatform;

namespace X2DisplayTest
{
    public class Config
    {
        public string FixturePortName { get; set; }
        public string LCP3005PortName { get; set; }
        public float RedWeight { get; set; }
        public float GreenWeight { get; set; }
        public float BlueWeight { get; set; }

        public bool IsOnlineShopfloor { get; set; }
        public bool IsScanSerialNumber { get; set; }
        public bool IsSimulation { get; set; }
        public string Station { get; set; }
        public string ProductType { get; set; }
        public string TestMode { get; set; }
        public string ScriptName { get; set; }

        public Config(string path)
        {
            this.FixturePortName = "";
            this.LCP3005PortName = "";
            this.RedWeight = 0.72f;
            this.GreenWeight = 0.18f;
            this.BlueWeight = 0.1f;
            this.IsOnlineShopfloor = true;
            this.IsScanSerialNumber = false;
            this.IsSimulation = false;
            this.Station = "FATP";
            this.ProductType = "Hodor";
            this.TestMode = "Automatic";
            this.ScriptName = @".\x2configfile.xml";

            this.path = path;
            ini = new HmzIniFile(this.path);
            ini.Create();
            this.ReadProfile();
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
                this.IsOnlineShopfloor = bool.Parse(ini.ReadString("shopfloor", "is_need_check"));
                this.IsScanSerialNumber = bool.Parse(ini.ReadString("x2params", "is_need_scan_serialnumber"));
                this.IsSimulation = bool.Parse(ini.ReadString("x2params", "is_simulation_mode"));
                this.Station = ini.ReadString("x2params", "station");
                this.ProductType = ini.ReadString("x2params", "product_type");
                this.TestMode = ini.ReadString("x2params", "testmode");
                this.ScriptName = ini.ReadString("x2params", "scriptname");
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
            ini.WriteString("shopfloor", "is_need_check", this.IsOnlineShopfloor.ToString());
            ini.WriteString("x2params", "is_need_scan_serialnumber", this.IsScanSerialNumber.ToString());
            ini.WriteString("x2params", "is_simulation_mode", this.IsSimulation.ToString());
            ini.WriteString("x2params", "station", this.Station);
            ini.WriteString("x2params", "product_type", this.ProductType);
            ini.WriteString("x2params", "testmode", this.TestMode);
            ini.WriteString("x2params", "scriptname", this.ScriptName);
        }
    }
}
