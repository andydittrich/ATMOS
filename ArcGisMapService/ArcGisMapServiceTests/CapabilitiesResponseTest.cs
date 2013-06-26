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

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization.Json;
using System.IO;
using ArcGisMapService;

namespace ArcGisMapServiceTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class CapabilitiesResponseTest
    {
        public CapabilitiesResponseTest()
        {
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
        public void IdahoExampleSerializationTest()
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(CapabilitiesResponse));
            MemoryStream exampleCapabilitiesResponseStream = new MemoryStream(Encoding.UTF8.GetBytes(TestResources.ExampleInsideIdahoCapabilitiesResponse));
            CapabilitiesResponse capabilitiesResponse = (CapabilitiesResponse)serializer.ReadObject(exampleCapabilitiesResponseStream);

            Assert.AreEqual("10.03", capabilitiesResponse.CurrentVersion);
            Assert.AreEqual("Layers", capabilitiesResponse.MapName);
            Assert.AreEqual("INSIDE Idaho", capabilitiesResponse.CopyrightText);
            Assert.IsFalse(capabilitiesResponse.SupportsDynamicLayers);

            Assert.AreEqual(4, capabilitiesResponse.Layers.Length);
            Assert.AreEqual("Agroclimate Zones", capabilitiesResponse.Layers[2].Name);
            Assert.AreEqual(2, capabilitiesResponse.Layers[2].Id);
            Assert.IsNull(capabilitiesResponse.Layers[2].SubLayerIds);

            Assert.AreEqual(0, capabilitiesResponse.Tables.Length);

            Assert.AreEqual(102100, capabilitiesResponse.SpatialReference.Wkid);

            Assert.IsNull(capabilitiesResponse.TileInfo);

            Assert.AreEqual(-13388407.0547671, capabilitiesResponse.InitialExtent.Xmin);
            Assert.AreEqual(6933084.98916851, capabilitiesResponse.FullExtent.Ymax);
            
            Assert.AreEqual("esriMeters", capabilitiesResponse.Units);
            Assert.IsTrue(capabilitiesResponse.SupportedImageFormatTypes.Contains("TIFF"));
            Assert.AreEqual(13, capabilitiesResponse.SupportedImageFormatTypes.Split(',').Length);

            // TODO: make this work: Assert.AreEqual("INSIDE Idaho", capabilitiesResponse.DocumentInfo["Author"]);

            Assert.IsTrue(capabilitiesResponse.Capabilities.Contains("Map"));
            Assert.AreEqual(3, capabilitiesResponse.Capabilities.Split(',').Length);

            // TODO: add more thorough tests
        }

        [TestMethod]
        public void EsriExampleSerializationTest()
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(CapabilitiesResponse));
            MemoryStream exampleCapabilitiesResponseStream = new MemoryStream(Encoding.UTF8.GetBytes(TestResources.ExampleESRICapabilitiesResponse));
            CapabilitiesResponse capabilitiesResponse = (CapabilitiesResponse)serializer.ReadObject(exampleCapabilitiesResponseStream);
        }

        [TestMethod]
        public void PointRoundTripParsingTest()
        {
            Point point = new Point(154, 123124);
            string pointString = point.ToString();
            Assert.AreEqual("154,123124", pointString);
            Point parsedPoint = new Point(pointString);
            Assert.AreEqual(154, parsedPoint.X);
            Assert.AreEqual(123124, parsedPoint.Y);
        }
    }
}
