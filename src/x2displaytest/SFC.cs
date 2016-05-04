using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO.Pipes;
using System.Security.Principal;

namespace X2DisplayTest
{
    class SFC
    {
        // 这函数就是读取handle文件,给sfc发送0x0802消息. dsn參數代表SN,port代表机台数一般写成1
        [DllImport("FIH_SFC.dll", EntryPoint = "ReportStatus", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static short ReportStatus(string sn, ushort port);

        // sfc给PC端的信息，也就是讀取N_WIP_Info.txt文件
        [DllImport("FIH_SFC.dll", EntryPoint = "AddTestLog", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static int AddTestLog(uint port, uint testNum, string testName,
            string upper, string lower, string testValue, string testResult);

        // 將測試結果發送給sfc，result發送“PASS”或者“FAIL”，其他無法識別
        [DllImport("FIH_SFC.dll", EntryPoint = "CreateResultFile", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static int CreateResultFile(uint port, string result);

        public static void SFCInit()
        {
            string path = @"C:\eBook_Test\";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            /*
            process = new Process();
            process.StartInfo.FileName = Path.Combine(System.Windows.Forms.Application.StartupPath, "SFCTool.exe");
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            if (process.Start())
            {
                pipeClient = new NamedPipeClientStream(".", "SFCTool",
                    PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.Impersonation);
                pipeClient.Connect();
                sr = new StreamReader(pipeClient);
                sw = new StreamWriter(pipeClient)
                {
                    AutoFlush = true
                };
            }
              */
        }

        private static Process process;
        private static NamedPipeClientStream pipeClient;
        private static StreamReader sr;
        private static StreamWriter sw;

        public static bool ReportStatus(string sn)
        {
            bool flag = false;

            if (sn == null) { return flag; }

            sw.WriteLine("status");
            sw.WriteLine(sn);

            if ("OK" == sr.ReadLine()) {
                flag = true;
            }

            return flag;
        }

        public static bool AddTestLog(uint testNum, string testName, 
            string upper, string lower, string testValue, string testResult)
        {
            bool flag = false;
            string sendData = string.Format("{0},{1},{2},{3},{4},{5}", testNum, testName, upper, lower, testValue, testResult);

            sw.WriteLine("test_value");
            sw.WriteLine(sendData);

            if ("OK" == sr.ReadLine()) {
                flag = true;
            }

            return flag;
        }

        public static bool CreateResultFile(string result)
        {
            bool flag = false;

            sw.WriteLine("test_result");
            sw.WriteLine(result);

            if ("OK" == sr.ReadLine()) {
                flag = true;
            }

            return flag;
        }

        public static void Exit()
        {
            if (pipeClient != null)
                pipeClient.Close();
            process.Close();
            process.Kill();
        }
    }
}
