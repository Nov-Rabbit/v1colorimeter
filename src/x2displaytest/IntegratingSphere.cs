using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace X2DisplayTest
{
    public class IntegratingSphere : IDevice
    {
        public IntegratingSphere(IDevice fixture, string portname)
        {
            this.fixture = fixture as Fixture;
            power = new DCPower3005(portname);
        }

        private Fixture fixture;
        private DCPower3005 power;

        private int voltage;
        public int Voltage
        {
            get { return voltage; }
        }

        private int current;
        public int Current
        {
            get { return current; }
        }

        public void Lighten()
        {
            this.Lighten(5000, 1500);
        }

        public void Lighten(int voltage, int current)
        {
            power.SetValue(voltage, true);
            power.SetValue(current, false);
            power.SetOutputStatus(true);
            this.voltage = voltage;
            this.current = current;
        }

        public void Lightoff()
        {
            Lighten(0, 0);
            power.SetOutputStatus(false);
        }

        public void MoveTestPos()
        {
            fixture.IntegratingSphereUp();
            System.Threading.Thread.Sleep(100);
            power.SetValue(5500, true);
            power.SetValue(1540, false);
            power.SetOutputStatus(true);
            voltage = 5500;
            current = 1540;
        }

        public void MoveReadyPos()
        {
            power.SetOutputStatus(false);
            power.SetValue(0, true);
            power.SetValue(0, false);
            voltage = 0;
            current = 0;
            fixture.IntegratingSphereDown();            
        }

        /// <summary>
        /// unit mA/mV
        /// </summary>
        /// <param name="voltage"></param>
        /// <param name="current"></param>
        public void ChangeTestParam(int voltage, int current)
        {
            power.SetValue(voltage, true);
            power.SetValue(current, false);
            power.SetOutputStatus(true);
            this.voltage = voltage;
            this.current = current;
        }

        protected override void ReadProfile()
        {
            throw new NotImplementedException();
        }

        protected override void WriteProfile()
        {
            throw new NotImplementedException();
        }
    }
}
