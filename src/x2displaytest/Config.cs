using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hnwlxy.HmzSysPlatform;

namespace Colorimeter_Config_GUI
{
    class Config
    {
        public Config(string path)
        {
            this.path = path;
            ini = new HmzIniFile(this.path);
            ini.Create();
        }

        private string path;
        private HmzIniFile ini;
        private double[] param = new double[7];

        public void ReadProfile(string path)
        {
            param[0] = ini.ReadDouble("white", "luminance");
            param[1] = ini.ReadDouble("white", "uniformity5");
            param[2] = ini.ReadDouble("white", "mura");
        }
    }
}
