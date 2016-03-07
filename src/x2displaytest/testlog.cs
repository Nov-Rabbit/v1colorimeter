using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Colorimeter_Config_GUI
{
    class testlog
    {
        public testlog()
        {
            if (!Directory.Exists(strPath_SummaryLog))
            {
                Directory.CreateDirectory(strPath_SummaryLog);
            }
        }
 
        // save the log to local for summary and detail information
        string strPath_SummaryLog = "C:\\vault\\LocalLog\\";
        string strPath_UnitLog = "C:\\vault\\LocalLog\\UnitSpecialLog\\";
        string strName_SummaryLog = "J1J2_LCD Uniformity";
        string strHead_SummaryLog = "SerialNumber,Operator,TestStatus,Fail List Item,Start Time,Stop Time";
        string strValue_SummaryLog = "";
        string strUpperLimit_SummaryLog = "UpperLimit,,,,,";
        string strLowerLimit_SummaryLog = "LowerLimit,,,,,";
        string strUnits_SummaryLog = "Units,,,,,";
        string strUsingLogName = "";
        string str_FailList = "";

        // add the Sys ID for the unit
        string SYS_ID;
        bool bol_IsCheckOneDay = false;

        StreamWriter stream_SummaryLog;
        string str_DateDay;
        string Str_DateTime;
        string str_StartTestTime;
        string str_StopTestTime;

        private StringBuilder uartData;

        private void InitCsvTitle(string fullFilePath, List<TestItem> allItems)
        {
            if (!File.Exists(fullFilePath))
            {
                foreach (TestItem testItem in allItems)
                {
                    foreach (TestNode node in testItem.SubNodes)
                    {
                        strHead_SummaryLog += "," + node.NodeName;
                        strUpperLimit_SummaryLog += "," + node.Upper;
                        strLowerLimit_SummaryLog += "," + node.Lower;
                        strUnits_SummaryLog += "," + node.Unit;
                    }
                }

                using (StreamWriter wf = new StreamWriter(File.Create(fullFilePath)))
                {
                    wf.WriteLine(strHead_SummaryLog);
                    wf.WriteLine(strUpperLimit_SummaryLog);
                    wf.WriteLine(strLowerLimit_SummaryLog);
                    wf.WriteLine(strUnits_SummaryLog);
                    wf.Close();
                }
            }
        }

        public void WriteCsv(string sn, DateTime startTime, DateTime stopTime, List<TestItem> allItems)
        {
            string csvFileName = "X2DisplayTest_" + DateTime.Now.ToString("yyyy-MM-dd") + ".csv";
            string fullFilePath = Path.Combine(strPath_SummaryLog, csvFileName);
            bool flagResult = true;

            this.InitCsvTitle(fullFilePath, allItems);

            StringBuilder nodedata = new StringBuilder();
            StringBuilder data = new StringBuilder();

            if (string.IsNullOrEmpty(sn)) {
                data.Append("xxxxxxxxxxxxxxxx");
            }
            else {
                data.Append(sn);
            }
            data.AppendFormat(",{0:yyyy-MM-dd HH:mm:ss},{1:yyyy-MM-dd HH:mm:ss},adminstator,", startTime, stopTime);

            foreach (TestItem item in allItems)
            {
                foreach (TestNode node in item.SubNodes)
                {
                    flagResult &= node.Run();
                    nodedata.AppendFormat("{0}", node.Value);
                }
            }
            data.AppendFormat("{0},", flagResult ? "PASS" : "FAIL");
            data.Append(nodedata.ToString());
            data.AppendLine();

            using (StreamWriter sw = new StreamWriter(fullFilePath, true))
            {
                sw.Write(data.ToString());
                sw.Flush();
                sw.Close();
            }
        }

        public void WriteUartLog(string data)
        {
            if (uartData == null)
            {
                uartData = new StringBuilder();
            }

            uartData.AppendFormat("[{0:yyyy-MM-dd HH:mm:ss}]: {1}\n", DateTime.Now, data);
        }

        public void UartFlush()
        {
            using (StreamWriter sw = new StreamWriter(""))
            {
            }
        }


        public void writecsv(string dir, string filename, double[,] matrix)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            
            StreamWriter fileLV = File.AppendText(filename);

            int rowNum = matrix.GetLength(0);
            int columnNum = matrix.GetLength(1);
            for (int i = 0; i < rowNum; i++)
            {
                for (int j = 0; j < columnNum; j++)
                {
                    if (j == columnNum - 1)
                    {
                        fileLV.Write(matrix[j, i].ToString());
                    }
                    else
                    {
                        fileLV.Write(matrix[j, i].ToString() + "\t");
                    }
                }

                fileLV.Write("\r\n");
            }

            fileLV.Flush();
            fileLV.Close();

        }

        public void AddLocalAndPDCALog(string str_ItemName, string str_TestValue, 
            string str_Result, string str_UpperLimit, string str_LowerLimit, string str_Units)
        {
            string str_UpperLimit_Tmp = str_UpperLimit;
            string str_LowerLimit_Tmp = str_LowerLimit;
            string str_Units_Tmp = str_Units;
            //string str_FailInfor_Tmp = str_FailInfor;

            if (str_UpperLimit_Tmp == string.Empty)
            {
                str_UpperLimit_Tmp = "NA";
            }

            if (str_LowerLimit_Tmp == string.Empty)
            {
                str_LowerLimit_Tmp = "NA";
            }

            if (str_Units_Tmp == string.Empty)
            {
                str_Units_Tmp = "NA";
            }

            //if (str_FailInfor_Tmp == string.Empty)
            //{
            //    str_FailInfor_Tmp = "Test Failed.";
            //}

            //list_ItemName.Add(str_ItemName);
            //list_UpperLimit.Add(str_UpperLimit);
            //list_LowerLimit.Add(str_LowerLimit);
            //list_Units.Add(str_Units);
            ////list_FailInfor.Add(str_FailInfor);


            if (string.Compare(str_Result, "FAIL", true) == 0)
            {
                str_FailList += str_ItemName + ";";
            }

            strValue_SummaryLog += "," + str_TestValue;
            strHead_SummaryLog += "," + str_ItemName;
            strUpperLimit_SummaryLog += "," + str_UpperLimit;
            strLowerLimit_SummaryLog += "," + str_LowerLimit;
            strUnits_SummaryLog += "," + str_Units;

            //     ip_Pudding.AddTestItem(str_ItemName, "", "", str_LowerLimit_Tmp, str_UpperLimit_Tmp, str_Units_Tmp, str_Result, str_TestValue, "The Valule is out of spec.", "0");

        }

        ///
        public void CheckLocalLogForOneDay()
        {
            if (bol_IsCheckOneDay)
            {
                return;
            }

            DateTime date_Today = DateTime.Now;
            // get the date of today
            string str_Date = string.Format("{0:yyyyMMdd}", date_Today);
            string str_tmp = strHead_SummaryLog + "\n" + strUpperLimit_SummaryLog + "\n" + strLowerLimit_SummaryLog + "\n" + strUnits_SummaryLog;

            // get the log list
            DirectoryInfo diInfo = new DirectoryInfo(strPath_SummaryLog);
            FileInfo[] files = diInfo.GetFiles();

            foreach (FileInfo file in files)
            {
                DateTime date_LastWriteTime = file.LastWriteTime;
                string str_data_LastWriteTime = string.Format("{0:yyyyMMdd}", date_LastWriteTime);

                // the log should be 24h
                if (str_data_LastWriteTime.CompareTo(str_Date) == 0)
                {
                    StreamReader stream_Reader = new StreamReader(strPath_SummaryLog + file);
                    string str_head = stream_Reader.ReadLine() + "\n";
                    str_head += stream_Reader.ReadLine() + "\n";
                    str_head += stream_Reader.ReadLine() + "\n";
                    str_head += stream_Reader.ReadLine() + "\n";
                    str_head += stream_Reader.ReadLine();
                    stream_Reader.Close();

                    str_head = str_head.Replace("\n", " ");
                    string str_newhead_tmp = str_tmp.Replace("\n", " ");

                    if (str_head.CompareTo(str_newhead_tmp) == 0)
                    {
                        strUsingLogName = strPath_SummaryLog + file;
                        stream_SummaryLog = File.AppendText(strPath_SummaryLog + file);
                        bol_IsCheckOneDay = true;
                        break;
                    }
                }
            }

            if (!bol_IsCheckOneDay)
            {
                if (stream_SummaryLog != null)
                {
                    stream_SummaryLog.Flush();
                    stream_SummaryLog.Close();
                }

                strUsingLogName = strPath_SummaryLog + strName_SummaryLog + "_" + string.Format("{0:yyyyMMdd}" + "{0:HHmmss}", date_Today, date_Today) + ".csv";
                stream_SummaryLog = File.AppendText(strUsingLogName);
                bol_IsCheckOneDay = true;
                stream_SummaryLog.Write(str_tmp);
                stream_SummaryLog.Flush();
            }

            str_DateDay = str_Date;
        }


        public void FinishLocalLog()
        {
            DateTime testTime = DateTime.Now;
            //Str_DateTime = string.Format("{0:yyyyMMdd}" + "_" + "{0:HHmmss}", testTime, testTime);
            str_StopTestTime = string.Format("{0:yyyyMMdd}" + "{0:HHmmss}", testTime, testTime);
            string str_TestStatus = "PASS";

            string str_tmp = string.Format("{0:yyyyMMdd}", testTime);

            if (string.Compare(str_DateDay, str_tmp, true) != 0)// check the 24h
            {
                bol_IsCheckOneDay = false;
            }

            if (!bol_IsCheckOneDay)
            {
                CheckLocalLogForOneDay();
            }

            if (SYS_ID == string.Empty)
            {
                SYS_ID = "NO_SN";
                str_TestStatus = "FAIL";
                str_FailList += "SN;";
            }

            if (str_FailList != string.Empty)
            {
                str_TestStatus = "FAIL";
            }

            strValue_SummaryLog = "\n" + SYS_ID + "," + str_TestStatus + "," + str_FailList + "," + str_StartTestTime + "," + str_StopTestTime + strValue_SummaryLog;
            stream_SummaryLog.Write(strValue_SummaryLog);
            stream_SummaryLog.Flush();
            stream_SummaryLog.Close();

            stream_SummaryLog = File.AppendText(strUsingLogName);

            strValue_SummaryLog = "";
            strHead_SummaryLog = "";
            strUpperLimit_SummaryLog = "";
            strLowerLimit_SummaryLog = "";
            strUnits_SummaryLog = "";
            str_FailList = "";

            //         ip_Pudding.commitInstantPudding(true);
        }

    }
}
