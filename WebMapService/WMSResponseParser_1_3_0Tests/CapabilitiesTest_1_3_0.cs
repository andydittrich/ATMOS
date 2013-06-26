// Copyright 2012 Andrew Dittrich
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.using System;

using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WMS.Capabilities_1_3_0;
using System.IO;
using System;
using System.Collections;

namespace WMSResponseParser_1_3_0Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class CapabilitiesTest_1_3_0
    {
        protected string[] ignoredNodes = { "xsi:schemaLocation" };
        protected ArrayList ignoredNodeList;
        protected string[] ignoredAttributes = { "xsi:schemaLocation" };
        protected ArrayList ignoredAttributeList;

        public CapabilitiesTest_1_3_0()
        {
            ignoredNodeList = new ArrayList(ignoredNodes);
            ignoredAttributeList = new ArrayList(ignoredAttributes);
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestDeserializeRGIS_MRCOG_GetCapabilities()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(WMS_Capabilities));
            StringReader reader = new StringReader(WMSResponseParser_1_3_0Tests.TestResources.RGIS_MRCOG_GetCapabilities_1_3_0);
            WMS_Capabilities wms_capabilities;
            wms_capabilities = (WMS_Capabilities)serializer.Deserialize(reader);
        }

        [TestMethod]
        public void TestRGIS_MRCOG_PostCode()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(WMS_Capabilities));
            StringReader reader = new StringReader(WMSResponseParser_1_3_0Tests.TestResources.RGIS_MRCOG_GetCapabilities_1_3_0);
            WMS_Capabilities wms_capabilities;
            wms_capabilities = (WMS_Capabilities)serializer.Deserialize(reader);
            Assert.AreEqual("87131", wms_capabilities.Service.ContactInformation.ContactAddress.PostCode);
        }

        [TestMethod]
        public void TestRGIS_MRCOG_GetMapFormatSupportsPNG()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(WMS_Capabilities));
            StringReader reader = new StringReader(WMSResponseParser_1_3_0Tests.TestResources.RGIS_MRCOG_GetCapabilities_1_3_0);
            WMS_Capabilities wms_capabilities;
            wms_capabilities = (WMS_Capabilities)serializer.Deserialize(reader);
            bool png_found = false;
            foreach (string format in wms_capabilities.Capability.Request.GetMap.Format)
            {
                if (format == "image/png")
                {
                    png_found = true;
                }
            }
            Assert.IsTrue(png_found);
        }

        [TestMethod]
        public void TestRGIS_MRCOG_GetMapURL()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(WMS_Capabilities));
            StringReader reader = new StringReader(WMSResponseParser_1_3_0Tests.TestResources.RGIS_MRCOG_GetCapabilities_1_3_0);
            WMS_Capabilities wms_capabilities;
            wms_capabilities = (WMS_Capabilities)serializer.Deserialize(reader);
            Assert.AreEqual("http://gstore.unm.edu/apps/rgis/datasets/118650/services/ogc/wms?",
                             wms_capabilities.Capability.Request.GetMap.DCPType[0].HTTP.Get.OnlineResource.href);
        }

        [TestMethod]
        public void TestRGIS_MRCOG_KeywordsContainsRGIS()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(WMS_Capabilities));
            StringReader reader = new StringReader(WMSResponseParser_1_3_0Tests.TestResources.RGIS_MRCOG_GetCapabilities_1_3_0);
            WMS_Capabilities wms_capabilities;
            wms_capabilities = (WMS_Capabilities)serializer.Deserialize(reader);

            bool found = false;
            foreach (Keyword keyword in wms_capabilities.Service.KeywordList)
            {
                if (keyword.Value == "RGIS")
                {
                    found = true;
                }
            }
            Assert.IsTrue(found);
        }

        [TestMethod]
        public void TestDeserializeRGIS_NAIP_GetCapabilities()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(WMS_Capabilities));
            StringReader reader = new StringReader(WMSResponseParser_1_3_0Tests.TestResources.RGIS_NAIP_GetCapabilities_1_3_0);
            WMS_Capabilities wms_capabilities;
            wms_capabilities = (WMS_Capabilities)serializer.Deserialize(reader);
        }

        [TestMethod]
        public void TestDeserializeExampleGetCapabilities()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(WMS_Capabilities));
            StringReader reader = new StringReader(WMSResponseParser_1_3_0Tests.TestResources.example_capabilities_1_3_0);
            WMS_Capabilities wms_capabilities;
            wms_capabilities = (WMS_Capabilities)serializer.Deserialize(reader);
        }

        [TestMethod]
        public void TestExampleGetCapabilitiesLayerCount()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(WMS_Capabilities));
            StringReader reader = new StringReader(WMSResponseParser_1_3_0Tests.TestResources.example_capabilities_1_3_0);
            WMS_Capabilities wms_capabilities;
            wms_capabilities = (WMS_Capabilities)serializer.Deserialize(reader);

            int layers = countLayers(wms_capabilities.Capability.Layer.Layer1);
            layers += 1; // for the root layer

            Assert.AreEqual(10, layers);
        }

        private int countLayers(Layer[] rootLayer)
        {
            int layers = 0;
            layers += rootLayer.Length;
            foreach (Layer layer in rootLayer)
            {
                if (layer.Layer1 != null)
                {
                    layers += countLayers(layer.Layer1);
                }
            }
            return layers;
        }

        [TestMethod]
        public void TestExampleGetCapabilitiesNoUnknkowns()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(WMS_Capabilities));
            serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);
            StringReader reader = new StringReader(WMSResponseParser_1_3_0Tests.TestResources.example_capabilities_1_3_0);
            WMS_Capabilities wms_capabilities;
            wms_capabilities = (WMS_Capabilities)serializer.Deserialize(reader);
        }

        private void serializer_UnknownNode (object sender, XmlNodeEventArgs e)
        {
            if (!ignoredNodeList.Contains(e.Name))
            {
                Assert.Fail("Unknown Node: " + e.Name + "\t" + e.Text);
            }
        }

        private void serializer_UnknownAttribute (object sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            if (!ignoredAttributeList.Contains(attr.Name))
            {
                Assert.Fail("Unknown attribute: " + attr.Name + "='" + attr.Value + "'");
            }
        }
    }
}
