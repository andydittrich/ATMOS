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
using WMS.Common;
using ServiceLogicCommon;

namespace WMSCommonTests
{
    [TestClass]
    public class GetMapRequestTest
    {
        protected static readonly string exampleGetMapUrl = "http://www.example.com/wms?VERSION=1.3.0&WIDTH=600&REQUEST=GetMap&HEIGHT=300&LAYERS=ozone&TIME=2000-08-03&CRS=CRS:84&ELEVATION=1000&BBOX=-180,-90,180,90&FORMAT=image/gif";
        protected static readonly string exampleGetMapUrlWithSRS = "http://www.example.com/wms?VERSION=1.1.1&WIDTH=256&REQUEST=GetMap&HEIGHT=256&LAYERS=seaSurfaceTemp&SRS=EPSG:4326&BBOX=-97.105,24.913,-78.794,36.358&FORMAT=image/png";
        protected static readonly string exampleGetMapUrlNoRequestTypeNoLayers = "http://www.example.com/wms?VERSION=1.3.0&WIDTH=600&HEIGHT=300&CRS=CRS:84&BBOX=-180,-90,180,90&FORMAT=image/png";

        [TestMethod]
        public void TestConstructor ()
        {
            GetMapRequest request = new GetMapRequest(exampleGetMapUrl);
            Assert.AreEqual("1.3.0", request.Version);
            Assert.AreEqual(GetMapRequest.getMapRequest, request.RequestType);
            Assert.AreEqual(600, request.Width);
            Assert.AreEqual(300, request.Height);
            Assert.AreEqual(1, request.Layers.Length);
            Assert.AreEqual("ozone", request.Layers[0]);
            Assert.AreEqual("CRS:84", request.SpatialReferenceSystem);
            BoundingBox expectedBoundingBox = new BoundingBox (-180,-90,180,90);
            Assert.AreEqual(expectedBoundingBox, request.BoundingBox);
            Assert.AreEqual("image/gif", request.Format);
        }

        [TestMethod]
        public void TestConstructor2()
        {
            GetMapRequest request = new GetMapRequest(exampleGetMapUrlWithSRS);
            Assert.AreEqual("1.1.1", request.Version);
            Assert.AreEqual(GetMapRequest.getMapRequest, request.RequestType);
            Assert.AreEqual(256, request.Width);
            Assert.AreEqual(256, request.Height);
            Assert.AreEqual(1, request.Layers.Length);
            Assert.AreEqual("seaSurfaceTemp", request.Layers[0]);
            Assert.AreEqual("EPSG:4326", request.SpatialReferenceSystem);
            BoundingBox expectedBoundingBox = new BoundingBox(-97.105, 24.913, -78.794, 36.358);
            Assert.AreEqual(expectedBoundingBox, request.BoundingBox);
            Assert.AreEqual("image/png", request.Format);
        }

        [TestMethod]
        public void AddFirstLayerTest()
        {
            GetMapRequest request = new GetMapRequest(exampleGetMapUrlNoRequestTypeNoLayers);
            Assert.IsNull(request.Layers);
            request.addLayer("temperature");
            Assert.AreEqual(1, request.Layers.Length);
            Assert.AreEqual("temperature", request.Layers[0]);
            Assert.IsTrue(request.Url.Query.Contains("LAYERS=temperature"));
        }

        [TestMethod]
        public void AddAnotherLayerTest()
        {
            GetMapRequest request = new GetMapRequest(exampleGetMapUrl);
            Assert.AreEqual(1, request.Layers.Length);
            Assert.AreEqual("ozone", request.Layers[0]);
            request.addLayer("temperature");
            Assert.AreEqual(2, request.Layers.Length);
            Assert.AreEqual("ozone", request.Layers[0]);
            Assert.AreEqual("temperature", request.Layers[1]);
            Assert.IsTrue(request.Url.Query.Contains("ozone,temperature"));
        }

        [TestMethod]
        public void ChangeBoundingBoxTest()
        {
            BoundingBox nevadaBox = new BoundingBox(-120.0, 34.9, -113.5, 42.1);
            GetMapRequest request = new GetMapRequest(exampleGetMapUrl);
            request.BoundingBox = nevadaBox;
            Assert.AreEqual(nevadaBox, request.BoundingBox);
            Assert.IsTrue(request.Url.Query.Contains("-120,34.9,-113.5,42.1"));
        }

        [TestMethod]
        public void transparencyTest()
        {
            string transparencyTrue1 = "http://www.example.com/wms?TRANSPARENT=true";
            string transparencyTrue2 = "http://www.example.com/wms?TRANSPARENT=TRUE";
            string transparencyFalse1 = "http://www.example.com/wms?TRANSPARENT=false";
            string transparencyFalse2 = "http://www.example.com/wms?TRANSPARENT=fAlSE";
            string transparencyMissing = "http://www.example.com/wms?";
            GetMapRequest request1 = new GetMapRequest(transparencyTrue1);
            Assert.IsTrue(request1.Transparent);
            GetMapRequest request2 = new GetMapRequest(transparencyTrue2);
            Assert.IsTrue(request2.Transparent);
            GetMapRequest request3 = new GetMapRequest(transparencyFalse1);
            Assert.IsFalse(request3.Transparent);
            GetMapRequest request4 = new GetMapRequest(transparencyFalse2);
            Assert.IsFalse(request4.Transparent);
            GetMapRequest request5 = new GetMapRequest(transparencyMissing);
            Assert.IsFalse(request5.Transparent);
            request5.Transparent = true;
            Assert.IsTrue(request5.Transparent);
            Assert.IsTrue(request5.Url.Query.ToLower().Contains("transparent=true"));
        }
    }
}
