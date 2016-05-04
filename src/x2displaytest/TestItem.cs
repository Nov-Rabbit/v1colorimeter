using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

namespace X2DisplayTest
{
    public class TestNode
    {
        public string NodeName { get; set; }
        public double Upper { get; set; }
        public double Lower { get; set; }
        public string Unit { get; set; }
        public double Value { get; set; }
        public string Error { get; set; }
        public bool IsNeedTest { get; set; }
        public bool Result { get; private set; }

        public bool Run()
        {
            return (this.Result = (this.Value <= this.Upper && this.Value >= this.Lower));
        }
    }

    public class TestItem
    {
        public string TestName { get; set; }
        public System.Drawing.Color RGB { get; set; }
        public double Exposure { get; set; }
        public bool IsNeedTest { get; set; }
        public bool Result { get; set; }
        
        public List<TestNode> TestNodes { get; set; }

        public TestItem()
        { 
            TestNodes = new List<TestNode>();
        }

        public bool Run()
        {
            this.Result = true;

            foreach (TestNode node in TestNodes)
            {
                if (node.IsNeedTest) {
                    this.Result &= node.Run();
                }
            }

            return this.Result;
        }
    }

    public class Xml
    {
        public Xml(string scriptName)
        {
            this.scriptName = scriptName;
            xml = new XmlDocument();
            xml.Load(scriptName);
            this.LoadScript();
        }

        private string scriptName;
        private XmlDocument xml;
        private List<TestItem> items;

        public List<TestItem> Items
        {
            get {
                return items;
            }
        }

        private TestNode LoadTestNode(XmlNode node)
        {
            TestNode testNode = new TestNode();
            XmlElement element = (XmlElement)node;

            if (element != null)
            {
                testNode.NodeName = element.GetAttribute("TestName");
                testNode.Upper = Convert.ToDouble(element.GetAttribute("Upper"));
                testNode.Lower = Convert.ToDouble(element.GetAttribute("Lower"));
                testNode.Unit = element.GetAttribute("Unit");
                testNode.IsNeedTest = Convert.ToBoolean(element.GetAttribute("IsNeedTest"));
            }
            
            return testNode;
        }

        private void SaveTestNode(XmlNode node, TestNode testNode)
        {
            if (node != null) {
                XmlElement element = xml.CreateElement("TestNode");
                element.SetAttribute("TestName", testNode.NodeName);
                element.SetAttribute("Upper", testNode.Upper.ToString());
                element.SetAttribute("Lower", testNode.Lower.ToString());
                element.SetAttribute("Unit", testNode.Unit);
                element.SetAttribute("IsNeedTest", testNode.IsNeedTest.ToString());
                node.AppendChild(element);
            }
        }

        private TestItem LoadTestItem(XmlNode node)
        {
            TestItem item = new TestItem();
            XmlElement element = (XmlElement)node.SelectSingleNode("Array");

            foreach (XmlNode n in element.ChildNodes)
            {
                item.TestNodes.Add(this.LoadTestNode(n));
            }

            element = (XmlElement)node.SelectSingleNode("Exposure");
            item.Exposure = Convert.ToDouble(element.GetAttribute("Time"));
            element = (XmlElement)node.SelectSingleNode("Info");
            item.TestName = element.GetAttribute("ItemName");
            item.IsNeedTest = bool.Parse(element.GetAttribute("IsNeedTest"));

            MatchCollection matches = Regex.Matches(element.GetAttribute("RGB"), @"\d+");

            if (matches.Count == 3) {
                item.RGB = System.Drawing.Color.FromArgb(int.Parse(matches[0].Value),
                    int.Parse(matches[1].Value), int.Parse(matches[2].Value));
            }

            return item;
        }

        private void SaveTestItem(XmlNode node, TestItem item)
        {
            XmlElement element = (XmlElement)node.SelectSingleNode("Array");
            element.RemoveAll();

            foreach (TestNode testNode in item.TestNodes)
            {
                this.SaveTestNode(element, testNode);
            }

            element = (XmlElement)node.SelectSingleNode("Exposure");
            element.SetAttribute("Time", item.Exposure.ToString());
            element = (XmlElement)node.SelectSingleNode("Info");
            element.SetAttribute("ItemName", item.TestName);
            element.SetAttribute("IsNeedTest", item.IsNeedTest.ToString());
            element.SetAttribute("RGB", string.Format("({0},{1},{2})", item.RGB.R, item.RGB.G, item.RGB.B));
        }

        public void LoadScript()
        {
            items = new List<TestItem>();

            XmlNodeList node = xml.SelectNodes("X2/Item");

            foreach (XmlNode n in node)
            {
                items.Add(this.LoadTestItem(n));
            }
        }

        public void SaveScript()
        {
            XmlNodeList node = xml.SelectNodes("X2/Item");

            for (int i = 0; i < items.Count; i++)
            {
                this.SaveTestItem(node[i], items[i]);
            }
            xml.Save(this.scriptName);
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
                        testItem.TestNodes.Add(testNode);
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
                        TestNode testNode = testItem.TestNodes[subIndex++];
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

        public void SetWhiteExposure(double time)
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
