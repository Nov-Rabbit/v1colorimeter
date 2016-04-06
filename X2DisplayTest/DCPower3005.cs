using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace Colorimeter_Config_GUI
{
    class DCPower3005
    {
        private SerialPort serialPort;
        public CRCClass crcObj;
        byte[] command = new byte[7] {0xA5,0x5A,  //帧起始 2Bytes
                                            0x00, //目标地址 1Byte
                                            0xFB, //源地址    1Byte
                                            0x00, //CMD命令字1Bytes
                                            0x80, //类型     1Bytes
                                            0x00, //长度(仅数据)1Byte
                                           // 0x00, //数据可变 
                                            //0x00, 0x00 CRC校验码 2Bytes
        };

        byte[] maxVoltage = new byte[] { 0xA5, 0x5A, 0x00, 0xFB, 0x22, 0x80, 0x02 };//maxVoltage protect
        byte[] maxCurrent = new byte[] { 0xA5, 0x5A, 0x00, 0xFB, 0x22, 0x80, 0x02 };//max current protect
        byte[] outPutON = new byte[] { 0xA5, 0x5A, 0x00, 0xFB, 0x24, 0x80, 0x01, 0x01, 0x36, 0x5C };
        byte[] outPutOFF = new byte[] { 0xA5, 0x5A, 0x00, 0xFB, 0x24, 0x80, 0x01, 0x00, 0x9C, 0x90 };
        byte[] remoteControl = new byte[] { 0xA5, 0x5A, 0x00, 0xFB, 0x26, 0x80, 0x01, 0x00, 0xCB, 0x01 };//
        byte[] currentAndVoltage = new byte[] { 0xA5, 0x5A, 0x00, 0xFB, 0x28, 0x80, 0x00, 0xB5, 0xAD };

        public DCPower3005(string portname)
        {
            serialPort = new SerialPort();
            crcObj = new CRCClass();

            SetPortProperty(portname);
        }

        //设置串口属性
        private void SetPortProperty(string portname)
        {
           // serialPort.Close();
            if (!SerialPort.GetPortNames().Contains(portname)) {
                return;
            }

            serialPort.PortName = portname;         //选择串口
            serialPort.BaudRate = int.Parse("38400"); //波特率
            serialPort.StopBits = StopBits.One;                                  //停止位
            serialPort.DataBits = 8;                                             //数据位
            serialPort.Parity = Parity.Even;                                     //奇偶校验

            try
            {
                serialPort.Open(); //打开串口
                bool flag = serialPort.IsOpen;


            }
            catch (Exception)
            {
               System.Windows.Forms.MessageBox.Show("打开串口失败!");
            }

        }

        /// <summary>
        /// 设置输出开关
        /// </summary>
        /// <param name="isOutPutON">ture 表示打开输出</param>
        public bool setOutputStatues(bool isOutPutON)
        {
            if (isOutPutON)
                Send(outPutON);
            else
                Send(outPutOFF);

            return isCommandWorked();
        }

        /// <summary>
        /// 设置远程连接
        /// </summary>
        /// <returns></returns>
        public bool setRemoteControl()
        {
            Send(remoteControl);
            return isCommandWorked();
        }

        public bool isCommandWorked()
        {
            byte[] result = new byte[]{};
            result= readData();
            if (result.Length >= 10 && result[9] == 0x0)
            {
                return true;     
            }

            return false;
        }

        public byte[] formatValue(int value)
        {
            string hex= Convert.ToString(value, 16);
           int len = (hex.Length + 1) / 2;
           hex = hex.PadLeft(len * 2,'0');
          //int a = Convert.ToInt32(hex,16);
           byte[] valueResult = new byte[len];
           for (int i = 0; i < len;i++ )
           {
               string temp = hex.Substring(i * 2, 2);
               valueResult[i] = Convert.ToByte(temp,16);
           }
           return valueResult;
        }

        /// <summary>
        /// 设置电源的电压和电流输出值
        /// </summary>
        /// <param name="mValue">值单位mV/mA</param>
        /// <param name="isVoltage">是否是电压</param>
        /// <returns>是否设置成功</returns>
        public bool setValue(int mValue, bool isVoltage)
        {     
            int value = mValue;
            value = isVoltage ? mValue / 10 : mValue;
            command[4] = isVoltage ? (byte)0x20 : (byte)0x21;

           byte[] valueResult =formatValue(value);// new ASCIIEncoding().GetBytes(hex);//System.BitConverter.GetBytes(value);//Convert.ToString(value, 16);//System.BitConverter.GetBytes(value);
            //int sh = System.BitConverter.ToInt32(valueResult, 0);
           command[6] = (byte)valueResult.Length;
            byte[] crc = getCRC(valueResult);//获得校验码
            int cmdLen = command.Length+valueResult.Length+2;
            byte[] newCmd = new byte[cmdLen];
            for (int i = 0; i < command.Length;i++ )
            {
                newCmd[i] = command[i];
            }
            for (int j = 0; j < valueResult.Length; j++)
            {
                newCmd[command.Length + j] = valueResult[j];
            }
            newCmd[cmdLen-2] = crc[1];
            newCmd[cmdLen-1] = crc[0];

            Send(newCmd);

            return isCommandWorked();
        }

        public byte[] getCRC(byte[] mValue)
        {
            List<byte> com = new List<byte>();
            int crcSourceLen = command.Length-2+mValue.Length;
            ushort[] crcSource = new ushort[crcSourceLen];
            for (int i = 0; i < command.Length-2; i++)
            {
                crcSource[i] = command[i + 2];
            }
            for (int j = 0; j < mValue.Length; j++)
            {
                crcSource[command.Length-2+j] = mValue[j];
            }

            ushort crc = crcObj.CRC_16_CCITT(crcSource);

            return BitConverter.GetBytes(crc);
        }

        public void Send(byte[] cmd)
        {
            serialPort.Write(cmd, 0, cmd.Length);
            Thread.Sleep(100);
        }

        /// <summary>
        /// 获得当前电源的输出电压和电流
        /// </summary>
        /// <param name="voltage">电压</param>
        /// <param name="current">电流</param>
        public void getCurrentAndVoltage(ref int voltage,ref int current)
        {
            Send(currentAndVoltage);
            byte[] result = readData();
            if (result.Length == 14)
            {
                byte[] vol = new byte[2];
                byte[] cur = new byte[2];
                vol[0] = result[9];
                vol[1] = result[10];
                cur[0] = result[11];
                cur[1] = result[12];
                voltage = System.Convert.ToInt16(vol);
                current = System.Convert.ToInt16(cur);
            }
        }

        public byte[] readData()
        {
            int nData = serialPort.BytesToRead;   //获取接收缓冲区中数据的字节数
            byte[] buffer = new byte[nData];
            serialPort.Read(buffer, 0, nData);      //把从串口读取到的数据放到数组buffer里

            StringBuilder strBuilder = new StringBuilder();
            foreach (byte bData in buffer)    //用foreach把数组buffer里的数据逐个添加到strBuilder里
            {
                    strBuilder.Append(bData.ToString() + "");
            }
            Console.WriteLine(strBuilder);

            return buffer;
        }
    }
}
