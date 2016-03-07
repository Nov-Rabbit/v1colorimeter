using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using DUTclass;

namespace Colorimeter_Config_GUI
{
    public delegate void DataDelegate(object sender);

    class Engine
    {
        public Engine(string configPath)
        {
            colorimeter = new Colorimeter();
            config = new Config(configPath);
            config.ReadProfile();
            dut = new Hodor();
            ip = new imagingpipeline();
        }

        public event DataDelegate dataChange;

        private bool flagExit;
        private bool flagAutoMode;
        private Colorimeter colorimeter;
        private Config config;

        //dut setup
        private DUT dut;
        private imagingpipeline ip;

        private Thread tdBlock;

        public void Initilazie()
        {
            if (!colorimeter.Connect())
            {
            }

            new Action(delegate() {
                while (!flagExit) {
                    if (dataChange != null)
                    {
                        dataChange(this);
                    }
                    System.Threading.Thread.Sleep(100);
                }
            }).BeginInvoke(null, null);
        }

        public void Start()
        {
            if (tdBlock != null)
            {
                tdBlock.Abort();
                tdBlock = null;
            }

            tdBlock = new Thread(RunSequence){
                IsBackground = true
            };
        }

        private void RunSequence()
        {

        }
    }
}
