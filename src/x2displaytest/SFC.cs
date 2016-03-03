using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Colorimeter_Config_GUI
{
    class SFC
    {
        // 这函数就是读取handle文件,给sfc发送0x0802消息. dsn參數代表SN,port代表机台数一般写成1
        [DllImport("FIH_SFC.dll", EntryPoint = "ReportStatus", CharSet = CharSet.Ansi)]
        public extern static int ReportStatus(string sn, uint port);

        // sfc给PC端的信息，也就是讀取N_WIP_Info.txt文件
        [DllImport("FIH_SFC.dll", EntryPoint = "AddTestLog", CharSet = CharSet.Ansi)]
        public extern static int AddTestLog(uint port, uint testNum, string testName,
            string upper, string lower, string testValue, string testResult);

        // 將測試結果發送給sfc，result發送“PASS”或者“FAIL”，其他無法識別
        [DllImport("FIH_SFC.dll", EntryPoint = "CreateResultFile", CharSet = CharSet.Ansi)]
        public extern static int CreateResultFile(uint port, string result);

        public static void SFCInit()
        {
            string path = @"C:\eBook_Test\";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
