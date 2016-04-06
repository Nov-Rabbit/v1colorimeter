using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace Colorimeter_Config_GUI
{
    public class Fixture : IDevice
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
                port.ReadTimeout = 60000;
            }
        }

        public Fixture()
        {
            this.ReadProfile();

            if (SerialPort.GetPortNames().Contains(this.PortName)) {
                port = new SerialPort(PortName);
                port.DataBits = 8;
                port.BaudRate = 115200;
                port.StopBits = StopBits.One;
                port.Parity = Parity.None;
                port.ReadTimeout = 60000;
            }
        }

        public enum Cylinder
        {
            None =0,
            FanOn,
            FanOff,
            HoldIn,
            HoldOut,
            IntegralSphereUp,
            IntegralSphereDown,
            Ca310Test,
            Ca310Back,
            CameraDown,
            CameraUp,
        }

        private SerialPort port;
        private const string READTOSTR = "*_*";

        public string PortName 
        {
            get;
            set;
        }

        public int InitPosition
        {
            get;
            set;
        }

        public int MeasurePosition
        {
            get;
            set;
        }

        public int CurrentPosition
        {
            get {
                return this.GetCurrentPos();
            }
        }

        public override System.Windows.Forms.Control DeviceConfigPanel
        {
            get {
                if (this.panel == null)
                    this.panel = new FixturePanel(this);

                return this.panel;
            }
            protected set
            {
                this.panel = value;
            }
        }

        protected override void ReadProfile()
        {
            try {
                string segName = this.GetType().Name;
                this.PortName = fileHandle.ReadString(segName, "portname");
                this.InitPosition = fileHandle.ReadInt(segName, "init_pos");
                this.MeasurePosition = fileHandle.ReadInt(segName, "measure_pos");
            }
            catch {
                this.PortName = "";
                this.InitPosition = this.MeasurePosition = 0;
                this.WriteProfile();
            }
        }

        protected override void WriteProfile()
        {
            string segName = this.GetType().Name;
            fileHandle.WriteString(segName, "portname", this.PortName);
            fileHandle.WriteDouble(segName, "init_pos", this.InitPosition);
            fileHandle.WriteDouble(segName, "measure_pos", this.MeasurePosition);
        }

        private string SendCommand(string command, string readTo = null)
        {
            string data = "";

            try {
                if (!port.IsOpen)
                {
                    port.Open();
                }

                port.DiscardInBuffer();
                port.DiscardOutBuffer();
                port.WriteLine(command);

                if (string.IsNullOrEmpty(readTo))
                {
                    System.Threading.Thread.Sleep(200);
                    data = port.ReadExisting();
                }
                else
                {
                    data = port.ReadTo(readTo);
                }
            }
            catch (NullReferenceException e) {
                throw new Exception("Open fixture communication port fail");
            }
            catch {
                throw new Exception("Fixture no respose.");
            }

            return data;
        }

        private bool ParseCmd(string command)
        {
            if (SendCommand(command, READTOSTR).ToLower().Contains("pass")) {
                return true;
            }
            else {
                return false;
            }
        }

        public bool CylinderMove(Cylinder name)
        {
            bool flag = false;

            switch (name)
            {
                case Cylinder.FanOn:
                    flag = this.ParseCmd("FAN ON");
                    break;
                case Cylinder.FanOff:
                    flag = this.ParseCmd("FAN OFF");
                    break;
                case Cylinder.HoldIn:
                    flag = this.ParseCmd("CY1 ON");
                    break;
                case Cylinder.HoldOut:
                    flag = this.ParseCmd("CY1 OFF");
                    break;
                case Cylinder.IntegralSphereUp:
                    flag = this.ParseCmd("CY2 ON");
                    break;
                case Cylinder.IntegralSphereDown:
                    flag = this.ParseCmd("CY2 OFF");
                    break;
                case Cylinder.Ca310Test:
                    flag = this.ParseCmd("CY3 ON");
                    break;
                case Cylinder.Ca310Back:
                    flag = this.ParseCmd("CY3 OFF");
                    break;
                case Cylinder.CameraDown:
                    flag = this.ParseCmd("CY4 ON");
                    break;
                case Cylinder.CameraUp:
                    flag = this.ParseCmd("CY4 OFF");
                    break;
            }

            return flag;
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
            string str = this.SendCommand(command);
            Console.WriteLine(str);
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

        public bool CameraDown()
        {
            return this.ParseCmd("CY4 ON");
        }

        public bool CameraUp()
        {
            return this.ParseCmd("CY4 OFF");
        }

        public bool Reset()
        {
            return this.ParseCmd("reset");
        }

        public bool BatteryOn()
        {
            return this.ParseCmd("BATTERY ON");
        }

        public bool BatteryOff()
        {
            return this.ParseCmd("BATTERY OFF");
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
