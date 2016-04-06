using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colorimeter_Config_GUI
{
    public class IntegratingSphere
    {
        public IntegratingSphere(Fixture fixture, string portname)
        {
            this.fixture = fixture;
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

        public void MoveTestPos()
        {
            fixture.IntegratingSphereUp();
            System.Threading.Thread.Sleep(100);
            power.setValue(5500, true);
            power.setValue(1540, false);
            power.setOutputStatues(true);
            voltage = 5500;
            current = 1540;
        }

        public void MoveReadyPos()
        {
            power.setOutputStatues(false);
            power.setValue(0, true);
            power.setValue(0, false);
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
            power.setValue(voltage, true);
            power.setValue(current, false);
            power.setOutputStatues(true);
            this.voltage = voltage;
            this.current = current;
        }
    }
}
