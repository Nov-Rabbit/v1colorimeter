using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Colorimeter_Config_GUI
{
    public class TestNode
    {
        public string NodeName { get; set; }
        public double Upper { get; set; }
        public double Lower { get; set; }
        public string Unit { get; set; }
        public double Value { get; set; }
        public string Error { get; set; }

        public bool Run()
        {
            if (this.Value <= this.Upper && this.Value >= this.Lower) {
                return true;
            }
            else {
                return false;
            }
        }
    }

    public class TestItem
    {
        public string TestName { get; set; }
        public bool Result { get; set; }
        public float Exposure { get; set; }
        public List<TestNode> SubNodes { get; set; }

        public TestItem()
        {
            SubNodes = new List<TestNode>();
        }
    }

    public class XMLManage
    {
        public XMLManage(string scriptName)
        {
            this.scriptName = scriptName;
            xmlDoc = new XmlDocument();
            allItems = new List<TestItem>();
        }

        private string scriptName;
        private XmlDocument xmlDoc;
        private List<TestItem> allItems;
        
        public List<TestItem> Items
        {
            get { 
                return allItems; 
            }
            set {
                allItems = value;
            }
        }

        public void LoadScript()
        {
            xmlDoc.Load(this.scriptName);
            XmlNode node = xmlDoc.SelectSingleNode("Colorimeter");
            XmlNodeList nodeList = node.ChildNodes;
            allItems.Clear();

            foreach (XmlNode nd in nodeList)
            {
                TestItem testItem = new TestItem();
                XmlElement element = (XmlElement)nd;
                testItem.TestName = element.Name;

                if (!(element.Name.Equals("White") || element.Name.Equals("Black") || element.Name.Equals("Red")
                    || element.Name.Equals("Green") || element.Name.Equals("Blue")))
                {
                    continue;
                }

                foreach (XmlNode subNode in element.ChildNodes)
                {
                    XmlElement subElement = (XmlElement)subNode;

                    if (subElement.Name.Equals("ExposureTime"))
                    {
                        testItem.Exposure = float.Parse(subElement.GetAttribute("Time"));
                    }
                    else {
                        TestNode testNode = new TestNode();
                        testNode.NodeName = subElement.GetAttribute("TestName");
                        testNode.Upper = double.Parse(subElement.GetAttribute("Upper"));
                        testNode.Lower = double.Parse(subElement.GetAttribute("Lower"));
                        testNode.Unit = subElement.GetAttribute("Unit");
                        testItem.SubNodes.Add(testNode);
                    }
                }

                allItems.Add(testItem);
            }
        }

        public void SaveScript()
        {
            int index = 0;
            xmlDoc.Load(this.scriptName);
            XmlNode node = xmlDoc.SelectSingleNode("Colorimeter");
            XmlNodeList nodeList = node.ChildNodes;

            foreach (XmlNode nd in nodeList)
            {
                XmlElement element = (XmlElement)nd;

                if (!(element.Name.Equals("White") || element.Name.Equals("Black") || element.Name.Equals("Red")
                    || element.Name.Equals("Green") || element.Name.Equals("Blue")))
                {
                    continue;
                }

                int subIndex = 0;
                TestItem testItem = allItems[index++];

                foreach (XmlNode subNode in element.ChildNodes)
                {
                    XmlElement subElement = (XmlElement)subNode;

                    if (subElement.Name.Equals("ExposureTime"))
                    {
                        subElement.SetAttribute("Time", testItem.Exposure.ToString());
                    }
                    else
                    {
                        TestNode testNode = testItem.SubNodes[subIndex++];
                        subElement.SetAttribute("TestName", testNode.NodeName);
                        subElement.SetAttribute("Upper", testNode.Upper.ToString());
                        subElement.SetAttribute("Lower", testNode.Lower.ToString());
                        subElement.SetAttribute("Unit", testNode.Unit);
                    }
                }
            }
            xmlDoc.Save(this.scriptName);
        }

        public void SaveSizeCalibrationValue(double value)
        {
            bool flag = false;
            xmlDoc.Load(this.scriptName);
            XmlNode node = xmlDoc.SelectSingleNode("Colorimeter");
            XmlNodeList nodeList = node.ChildNodes;
 
            foreach (XmlNode nd in nodeList)
            {
                XmlElement element = (XmlElement)nd;

                if (element.Name == "Calibration")
                {
                    element.SetAttribute("Value", value.ToString());
                    flag = true;
                }
            }

            if (!flag) {
                XmlNode nd = xmlDoc.CreateNode(XmlNodeType.Element, "Calibration", "");
                ((XmlElement)nd).SetAttribute("Value", value.ToString());
                node.AppendChild(nd);
            }
            xmlDoc.Save(this.scriptName);
        }

        public void SaveCalibrationExposureTime(float time, string name)
        {
            xmlDoc.Load(this.scriptName);
            XmlNode node = xmlDoc.SelectSingleNode("Colorimeter");
            XmlNodeList nodeList = node.ChildNodes;

            foreach (XmlNode nd in nodeList)
            {
                XmlElement element = (XmlElement)nd;

                if (element.Name.Equals(name)) {
                    foreach (XmlNode subnd in element.ChildNodes)
                    {
                        XmlElement e = (XmlElement)subnd;

                        if (e.Name == "ExposureTime ") {
                            e.SetAttribute("Time", time.ToString());
                        }
                    }
                }
            }
            xmlDoc.Save(this.scriptName);
            //LoadScript();
        }

        public void SetWhiteExposure(float time)
        {
            xmlDoc.Load(this.scriptName);
            XmlNode node = xmlDoc.SelectSingleNode("Colorimeter");
            XmlNodeList nodeList = node.ChildNodes;

            foreach (XmlNode nd in nodeList)
            {
                XmlElement element = (XmlElement)nd;

                if (element.Name.Equals("White"))
                {
                    foreach (XmlNode sub in nd.ChildNodes)
                    {
                        XmlElement subElem = (XmlElement)sub;

                        if (subElem.Name.Equals("ExposureTime"))
                        {
                            subElem.SetAttribute("Time", time.ToString());
                        }
                    }
                }
            }

            xmlDoc.Save(this.scriptName);
        }
    }
}
