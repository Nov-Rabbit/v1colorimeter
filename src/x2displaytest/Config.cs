using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hnwlxy.HmzSysPlatform;

namespace Colorimeter_Config_GUI
{
    public class Config
    {
        public Config(string path)
        {
            this.path = path;
            this.param = new Dictionary<string, List<double>>();
            ini = new HmzIniFile(this.path);
            ini.Create();
        }

        private string path;
        private HmzIniFile ini;

        private Dictionary<string, List<double>> param;
        public Dictionary<string, List<double>> ConfigParams
        {
            get
            {
                return param;
            }
        }

        public void ReadProfile()
        {
            string[] names = { "white", "black", "red", "green", "blue" };

            try {
                foreach (string panelName in names)
                {
                    List<double> data = new List<double>();

                    if (panelName == "white" || panelName == "black")
                    {
                        data.Add(ini.ReadDouble(panelName, "luminance_upper"));
                        data.Add(ini.ReadDouble(panelName, "luminance_lower"));
                        data.Add(ini.ReadDouble(panelName, "uniformity5_upper"));
                        data.Add(ini.ReadDouble(panelName, "uniformity5_lower"));
                        data.Add(ini.ReadDouble(panelName, "mura_upper"));
                        data.Add(ini.ReadDouble(panelName, "mura_lower"));
                    }
                    else
                    {
                        data.Add(ini.ReadDouble(panelName, "CIE1931x_upper"));
                        data.Add(ini.ReadDouble(panelName, "CIE1931x_lower"));
                        data.Add(ini.ReadDouble(panelName, "CIE1931y_upper"));
                        data.Add(ini.ReadDouble(panelName, "CIE1931y_lower"));
                        data.Add(ini.ReadDouble(panelName, "CIE1931z_upper"));
                        data.Add(ini.ReadDouble(panelName, "CIE1931z_lower"));
                    }

                    data.Add(ini.ReadDouble(panelName, "exposure"));
                    param.Add(panelName, data);
                }
            }
            catch {
                this.WriteProfile();
            }
        }

        public void WriteProfile()
        {
            string[] names = { "white", "black", "red", "green", "blue" };

            foreach (string panelName in names)
            {
                int index = 0;
                List<double> data = param[panelName];

                if (data.Count != 7)
                {
                    data = new List<double>(7);
                    data.AddRange(new double[] { 0,0,0,0,0,0,0 });
                }

                if (panelName == "white" || panelName == "black")
                {
                    ini.WriteDouble(panelName, "luminance_upper", data[index++]);
                    ini.WriteDouble(panelName, "luminance_lower", data[index++]);
                    ini.WriteDouble(panelName, "uniformity5_upper", data[index++]);
                    ini.WriteDouble(panelName, "uniformity5_lower", data[index++]);
                    ini.WriteDouble(panelName, "mura_upper", data[index++]);
                    ini.WriteDouble(panelName, "mura_lower", data[index++]);
                }
                else
                {
                    ini.WriteDouble(panelName, "CIE1931x_upper", data[index++]);
                    ini.WriteDouble(panelName, "CIE1931x_lower", data[index++]);
                    ini.WriteDouble(panelName, "CIE1931y_upper", data[index++]);
                    ini.WriteDouble(panelName, "CIE1931y_lower", data[index++]);
                    ini.WriteDouble(panelName, "CIE1931z_upper", data[index++]);
                    ini.WriteDouble(panelName, "CIE1931z_lower", data[index++]);
                }
                ini.WriteDouble(panelName, "exposure", data[index++]);
            }
        }
    }
}
