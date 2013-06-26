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
using WMS.ResponseParser_1_1_1;
using WMS.Common;
using ServiceLogicCommon;

namespace WMSResponseParser_1_1_1Tests
{
    [TestClass]
    public class ResponseParser_1_1_1Tests
    {
        [TestMethod]
        public void ParseMRCOGResponseTest ()
        {
            ResponseParser_1_1_1 parser = new ResponseParser_1_1_1();
            GetCapabilitiesResponse response = parser.parseResponse(WMSResponseParser_1_1_1Tests.TestResources.RGIS_MRCOG_GetCapabilities_1_1_1);
            Assert.AreEqual("1.1.1", response.Version);
            Assert.AreEqual("OGC:WMS", response.Service.Name);
            Assert.AreEqual("RGIS Dataset", response.Service.Title);
            Assert.AreEqual("http://gstore.unm.edu/apps/rgis/datasets/118650/services/ogc/wms?", response.Service.Url);
            Assert.AreEqual(0, response.Service.MaxHeight);
            Assert.AreEqual(0, response.Service.MaxWidth);
        }

        [TestMethod]
        public void ParseMRCOGResponseGetMapTest()
        {
            ResponseParser_1_1_1 parser = new ResponseParser_1_1_1();
            GetCapabilitiesResponse response = parser.parseResponse(WMSResponseParser_1_1_1Tests.TestResources.RGIS_MRCOG_GetCapabilities_1_1_1);
            string[] expectedFormats = new string[] { "image/jpeg", 
                                                      "image/png",
                                                      "image/gif",
                                                      "image/png; mode=24bit",
                                                      "image/vnd.wap.wbmp",
                                                      "image/tiff",
                                                      "image/svg+xml" };
            foreach (string format in response.MapFormats)
            {
                Assert.IsTrue(expectedFormats.Contains(format));
            }
            Assert.AreEqual(expectedFormats.Length, response.MapFormats.Length);
            Assert.AreEqual("http://gstore.unm.edu/apps/rgis/datasets/118650/services/ogc/wms?", response.GetMapUrl);
        }

        [TestMethod]
        public void ParseMRCOGResponseLayersTest()
        {
            ResponseParser_1_1_1 parser = new ResponseParser_1_1_1();
            GetCapabilitiesResponse response = parser.parseResponse(WMSResponseParser_1_1_1Tests.TestResources.RGIS_MRCOG_GetCapabilities_1_1_1);
            
            Assert.AreEqual("RGISMap", response.Layer.Name);
            Assert.AreEqual("RGIS Dataset", response.Layer.Title);
            Assert.AreEqual("EPSG:4326", response.Layer.SpatialReferenceSystem);
            BoundingBox expectedBoundingBox = new BoundingBox (-107.221, 34.2469, -105.972, 35.6434);
            Assert.AreEqual(expectedBoundingBox, response.Layer.BoundingBox);
            Assert.IsNotNull(response.Layer.Layers);
        }

        [TestMethod]
        public void ParseException()
        {
            bool argumentExceptionThrown = false;
            ResponseParser_1_1_1 parser = new ResponseParser_1_1_1();
            try
            {
                parser.parseException(TestResources.example_exception_1_1_1);
            }
            catch (ArgumentException ex)
            {
                Assert.IsTrue(ex.Message.Contains("Plain text message about an error."));
                argumentExceptionThrown = true;
            }
            Assert.IsTrue(argumentExceptionThrown);
        }
    }
}
