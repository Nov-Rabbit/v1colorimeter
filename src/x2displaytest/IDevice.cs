using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hnwlxy.HmzSysPlatform;

namespace X2DisplayTest
{
    public abstract class IDevice// : IDisposable, ICloneable
    {
        protected abstract void ReadProfile();
        protected abstract void WriteProfile();

        protected string uuid;
        public virtual string UUID
        {
            get { 
                return uuid;
            }
        }

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
        private static DevManage instance;

        public static DevManage Instance
        {
            get {
                if (instance == null) {
                    instance = new DevManage();
                }

                return instance;
            }
        }

        private DevManage()
        {
            devices = new Dictionary<string, IDevice>();
        }

        private Dictionary<string, IDevice> devices;

        public Dictionary<string, IDevice> Devices
        {
            get {
                return devices;
            }
        }

        public void AddDevice(IDevice device)
        {
            if (!devices.ContainsKey(device.GetType().Name)) {
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
