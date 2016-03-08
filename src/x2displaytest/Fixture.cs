using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

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
                port.ReadTimeout = 2000;
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

            return port.ReadTo("*_*");
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

        public void MotorMove(int position)
        {
            string command = string.Format("GETPOSITION {0}", position);
            this.SendCommand(command);
        }

        public bool FanOn()
        {
            return this.ParseCmd("FANON");
        }

        public bool FanOff()
        {
            return this.ParseCmd("FANOFF");
        }

        public bool IntegratingSphereUp()
        {
            return this.ParseCmd("CY1ON");
        }

        public bool IntegratingSphereDown()
        {
            return this.ParseCmd("CY1OFF");
        }

        public bool HoldIn()
        {
            return this.ParseCmd("CY2ON");
        }

        public bool HoldOut()
        {
            return this.ParseCmd("CY2OFF");
        }

        public bool RotateOn() 
        {
            return this.ParseCmd("CY3ON");
        }

        public bool RotateOff()
        {
            return this.ParseCmd("CY3OFF");
        }
    }
}
