using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;

namespace Colorimeter_Config_GUI
{
    public class Ca310Pipe
    {
        public Ca310Pipe(string path)
        {
            process = new Process();
            process.StartInfo.FileName = Path.Combine(path, "KonicaCa310Tool.exe");
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            
            if (process.Start())
            {
                pipeClient = new NamedPipeClientStream(".", "Ca310Pipe", 
                    PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.Impersonation);
                pipeClient.Connect();
                sr = new StreamReader(pipeClient);
                sw = new StreamWriter(pipeClient) { 
                    AutoFlush = true
                };
            }
        }

        private Process process;
        private NamedPipeClientStream pipeClient;
        private StreamReader sr;
        private StreamWriter sw;

        private string errorInfo;
        public string ErrorMessage {
            get {
                if (errorInfo == null)
                    errorInfo = "";

                return errorInfo;
            }
        }

        public CIE1931Value CIE1931xyY { get; private set; }

        // connect to Ca310
        public bool Connect()
        {
            bool flag = true;
            string str;

            sw.WriteLine("init");

            if ((str = sr.ReadLine()) != "OK") {
                errorInfo = str;
                flag = false;
            }

            return flag;
        }

        // init ca310 device
        public bool ResetZero()
        {
            bool flag = true;
            string str;

            sw.WriteLine("zero");

            if ((str = sr.ReadLine()) != "OK")
            {
                errorInfo = str;
                flag = false;
            }

            return flag;
        }

        public CIE1931Value GetCa310Data()
        {
            if (CIE1931xyY == null) {
                CIE1931xyY = new CIE1931Value();
            }
            CIE1931xyY.x = CIE1931xyY.y = CIE1931xyY.Y = 0;

            sw.WriteLine("mes");
            string result = sr.ReadLine();

            if (result.Equals("OK")) {
                result = sr.ReadLine();

                if (!string.IsNullOrEmpty(result))
                {
                    string[] arrayStr = result.Split(new char[] { ',' });

                    if (arrayStr.Length == 3)
                    {
                        CIE1931xyY.Y = double.Parse(arrayStr[0].Substring(3));
                        CIE1931xyY.x = double.Parse(arrayStr[1].Substring(3));
                        CIE1931xyY.y = double.Parse(arrayStr[2].Substring(3));
                    }
                }
            }
            else {
                errorInfo = result;
            }

            return CIE1931xyY;
        }

        public void Disconnect()
        {
            if (pipeClient != null)
                pipeClient.Close();
            process.Kill();
        }
    }
}
