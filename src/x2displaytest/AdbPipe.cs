using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Colorimeter_Config_GUI
{
    class AdbPipe
    {
        public AdbPipe()
        {
            adbStartPath = System.Windows.Forms.Application.StartupPath + @"\adb\";
            this.process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;

            if (process.Start())
            {
                string str = null;
                while ("" != (str = process.StandardOutput.ReadLine()))
                {
                    Console.WriteLine(str);
                }

                process.StandardInput.AutoFlush = true;
                str = this.GetPipeData(string.Format("cd {0}", adbStartPath));
                Console.WriteLine(str);
            }
        }

        private Process process;
        private readonly string adbStartPath;

        private string GetPipeData(string command)
        {
            StringBuilder result = new StringBuilder();
            string line = null;

            if (string.IsNullOrEmpty(command))
            {
                return result.ToString();
            }

            process.StandardInput.WriteLine(command);
            System.Threading.Thread.Sleep(50);

            do {
                line = process.StandardOutput.ReadLine();

                if (!string.IsNullOrEmpty(line))
                {
                    result.Append(line);
                }
            }
            while (!string.IsNullOrEmpty(line));
            
            return result.ToString();
        }

        private bool SetMode(string colorName)
        {
            bool flag = false;
            string result = null;
            result = this.GetPipeData("adb devices");

            Regex regex = new Regex(@"\d{8}");

            if (regex.IsMatch(result))
            {
                Console.WriteLine("Device ID: {0}", regex.Match(result).Value);
                result = this.GetPipeData("adb root");

                Console.WriteLine(result);
                result = this.GetPipeData(string.Format("adb shell mmi -c lcd -d \"{0}\"", colorName));
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine("Can't find device");
            }

            return flag;
        }

        public bool SetWhiteMode()
        {
            return this.SetMode("white");
        }

        public bool SetBlackMode()
        {
            return this.SetMode("black");
        }

        public bool SetRedMode()
        {
            return this.SetMode("red");
        }

        public bool SetGreenMode()
        {
            return this.SetMode("green");
        }

        public bool SetBlueMode()
        {
            return  this.SetMode("blue");
        }

        public string GetDeviceID()
        {
            string result = null;
            result = this.GetPipeData("adb devices");

            Regex regex = new Regex(@"\d{8}");

            if (regex.IsMatch(result))
            {
                result = regex.Match(result).Value;
            }
            else
            {
                result = null;
            }

            return result;
        }
    }
}
