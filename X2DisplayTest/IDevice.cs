using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hnwlxy.HmzSysPlatform;

namespace Colorimeter_Config_GUI
{
    public abstract class IDevice// : IDisposable, ICloneable
    {
        protected abstract void ReadProfile();
        protected abstract void WriteProfile();

        protected System.Windows.Forms.Control panel;
        public virtual System.Windows.Forms.Control DeviceConfigPanel
        {
            get; 
            protected set;
        }

        protected static string filename;
        protected static HmzIniFile fileHandle;

        static IDevice()
        {
            if (filename == null) {
                filename = @".\profile.ini";
            }
            fileHandle = new HmzIniFile(filename);
            fileHandle.Create();
        }
    }

    public class DevManage
    {
        public DevManage()
        {
            devices = new Dictionary<string, IDevice>();
        }

        private Dictionary<string, IDevice> devices;

        public void AddDevice(IDevice device)
        {
            if (!devices.ContainsValue(device)) {
                devices.Add(device.GetType().Name, device);
            }
        }

        public void RemoveDevice(IDevice device)
        {
            if (devices.ContainsKey(device.GetType().Name)) {
                devices.Remove(device.GetType().Name);
            }
        }

        public IDevice SelectDevice(string deviceName)
        {
            IDevice dev = null;

            if (devices.ContainsKey(deviceName)) {
                dev = devices[deviceName];
            }

            return dev;
        }
    }
}
