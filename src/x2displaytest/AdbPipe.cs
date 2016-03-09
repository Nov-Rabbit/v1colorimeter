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
                this.ReadToEnd();
                process.StandardInput.AutoFlush = true;
                string str = this.GetPipeData(string.Format("cd {0}", adbStartPath));
                Debug.WriteLine(str);
                str = this.GetPipeData("adb root");
                Debug.WriteLine(str);
            }
        }

        private Process process;
        private bool isHasDUT;
        private readonly string adbStartPath;

        public string ReadToEnd()
        {
            StringBuilder result = new StringBuilder();
            string line = null;

            do {
                line = process.StandardOutput.ReadLine();
               
                if (!string.IsNullOrEmpty(line)) {
                    result.Append(line);
                }
                System.Threading.Thread.Sleep(20);
            }
            while (!string.IsNullOrEmpty(line));

            return result.ToString();
        }

        private string GetPipeData(string command, int timeout = 0, string readTo = "")
        {
            string line = null;
            StringBuilder result = new StringBuilder();            

            if (string.IsNullOrEmpty(command)) {
                return result.ToString();
            }

            process.StandardOutput.DiscardBufferedData();
            process.StandardInput.WriteLine(command);
            Console.WriteLine("send command: {0}", command);

            if (timeout <= 0) {
                System.Threading.Thread.Sleep(100);
                result.Append(this.ReadToEnd());
            }
            else {
                DateTime timeNow = DateTime.Now;

                do {
                    if (DateTime.Now.Subtract(timeNow).TotalMilliseconds > timeout) {
                        break;
                    }

                    line = process.StandardOutput.ReadLine();
                    Console.WriteLine(line);

                    if (!string.IsNullOrEmpty(line)) {
                        result.Append(line);
                    }

                    if (!string.IsNullOrEmpty(readTo)) {
                        if (line.LastIndexOf(readTo) > 0)
                        {
                            break;
                        }
                    }
                    System.Threading.Thread.Sleep(20);
                }
                while (true);                
            }

            return result.ToString();
        }

        private bool SetMode(string colorName)
        {
            bool flag = false;
            string result = null;

            if (!isHasDUT) {
                this.ReadToEnd();
                result = this.GetPipeData("adb devices");
                Regex regex = new Regex(@"\d{8}");

                if (!regex.IsMatch(result))
                {
                    Debug.WriteLine("Can't find device");
                    return false;
                }
            }

            result = this.GetPipeData(string.Format("adb shell \"mmi -c lcd -d {0}\"", colorName), 70000, "edited");

            if (result.Contains("edited"))
            {
                flag = true;
            }
           // process.StandardInput.WriteLine(string.Format("adb shell \"mmi -c lcd -d {0}\"", colorName));

           // flag = true;
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

            for (int i = 0; i < 3; i++)
            {
                System.Threading.Thread.Sleep(100);
                process.StandardOutput.DiscardBufferedData();
                result = this.GetPipeData("adb devices");                
            }            

            Regex regex = new Regex(@"\d{8}");

            if (regex.IsMatch(result))
            {
                result = regex.Match(result).Value;
                isHasDUT = true;
            }
            else
            {
                result = null;
                isHasDUT = false;
            }

            return result;
        }

        public void ExitAdbPipe()
        {
            process.Kill();
        }
    }
}
