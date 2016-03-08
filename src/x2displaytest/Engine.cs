using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using DUTclass;

namespace Colorimeter_Config_GUI
{
    public delegate void DataDelegate(object sender, DataChangeEventArgs args);

    public class DataChangeEventArgs
    {
        public string StatusInfo { get; set; }
        public Colorimeter Colorimeter { get; set; }
    }

    public class Engine
    {
        public Engine(string scriptName)
        {
            colorimeter = new Colorimeter();
            xml = new XMLManage(scriptName);
            dut = new Hodor();
            ip = new imagingpipeline();
            args = new DataChangeEventArgs();
            log = new testlog();
        }

        public event DataDelegate dataChange;
        private DataChangeEventArgs args;

        private bool flagExit;
        private bool flagAutoMode;
        private Colorimeter colorimeter;
        private XMLManage xml;
        private testlog log;

        //dut setup
        private DUT dut;
        private imagingpipeline ip;

        private Thread tdBlock;

        public void Initilazie()
        {
            if (!colorimeter.Connect())
            {
                args.StatusInfo = "No Camera";
            }

            new Action(delegate() {
                while (!flagExit) {
                    if (dataChange != null)
                    {
                        this.args.Colorimeter = colorimeter;
                        dataChange.Invoke(this, args);
                        //dataChange(this, args);
                    }
                    System.Threading.Thread.Sleep(100);
                }
            }).BeginInvoke(null, null);
        }

        public void Start()
        {
            if (tdBlock != null) {
                tdBlock.Abort();
                tdBlock = null;
            }

            tdBlock = new Thread(RunSequence){
                IsBackground = true
            };
        }

        private void RunSequence()
        {
            args.StatusInfo = "Wait DUT.";

            while (!dut.checkDUT()) { Thread.Sleep(100); }
            log.WriteUartLog(string.Format("DUT connected, DeviceID: {0}\r\n", dut.DeviceID));

        }
    }
}
