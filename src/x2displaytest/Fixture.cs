using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace Colorimeter_Config_GUI
{
    public class Fixture
    {
        public Fixture(string portName)
        {
            if (SerialPort.GetPortNames().Contains(portName))
            {
                port = new SerialPort(portName);
                port.DataBits = 8;
                port.BaudRate = 115200;
                port.StopBits = StopBits.One;
                port.Parity = Parity.None;
                port.ReadTimeout = 5000;
            }
        }

        private SerialPort port;

        private string SendCommand(string command)
        {
            if (!port.IsOpen)
            {
                port.Open();
            }

            port.WriteLine(command);
            System.Threading.Thread.Sleep(100);

            return port.ReadExisting(); //port.ReadTo("*_*");
        }

        private bool ParseCmd(string command)
        {
            if (SendCommand(command).ToLower().Contains("pass")) {
                return true;
            }
            else {
                return false;
            }
        }

        public int GetCurrentPos()
        {
            string value = this.SendCommand("GETPOSITION");
            if (value == "") { return 0; }

            Regex regex = new Regex(@"\d+");            
            Match match = regex.Match(value);

            return int.Parse(match.Value);
        }

        public void MotorMove(int position)
        {
            int pos = this.GetCurrentPos() + position;
            string command = string.Format("MOVE 1 {0}", pos);
            this.SendCommand(command);
        }

        public bool FanOn()
        {
            return this.ParseCmd("FAN ON");
        }

        public bool FanOff()
        {
            return this.ParseCmd("FAN OFF");
        }

        public bool IntegratingSphereUp()
        {
            return this.ParseCmd("CY2 ON");
        }

        public bool IntegratingSphereDown()
        {
            return this.ParseCmd("CY2 OFF");
        }

        public bool HoldIn()
        {
            return this.ParseCmd("CY1 ON");
        }

        public bool HoldOut()
        {
            return this.ParseCmd("CY1 OFF");
        }

        public bool RotateOn() 
        {
            return this.ParseCmd("CY3 ON");
        }

        public bool RotateOff()
        {
            return this.ParseCmd("CY3 OFF");
        }

        public bool Reset()
        {
            return this.ParseCmd("reset");
        }

        public bool CheckDoubleStart()
        {
            bool flag = false;

            while (true) {
                if (!port.IsOpen) {
                    port.Open();
                }
                string str = port.ReadExisting();

                if (str.ToLower().Contains("start pass"))
                {
                    flag = true;
                    break;
                }
            }

            return flag;
        }
    }
}
