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

using ArcGisMapService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ArcGisMapServiceTests
{
    
    
    /// <summary>
    ///This is a test class for RequestTest and is intended
    ///to contain all RequestTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RequestTest
    {
        protected static readonly string idahoCatalogUrl = "http://cloud.insideidaho.org/ArcGIS/rest/services/climatologyMeteorologyAtmosphere/climatologyMeteorologyAtmosphere";
        protected static readonly string idahoCatalogUrlWithSlash = idahoCatalogUrl + "/";
        protected static readonly string idahoMapServiceUrl = idahoCatalogUrlWithSlash + "MapServer";
        protected static readonly string idahoMapServiceUrlWithFormat = idahoMapServiceUrl + "?f=json";

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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Request Constructor
        ///</summary>
        [TestMethod()]
        public void RequestConstructorTest_CatalogOnly()
        {
            Request request = new Request(idahoCatalogUrl);
            // make sure the constructor added the 'MapServer' path
            Assert.IsTrue(request.Url.PathAndQuery.Contains("MapServer"));
            // make sure the Uri ends with a slash, even though we didn't provide one
            Assert.IsTrue(request.Url.PathAndQuery.EndsWith("/"));
            // make sure there is a single slash between the catalog and 'MapServer' (that it has been canonicalized).
            Assert.IsTrue(request.Url.PathAndQuery.Contains("climatologyMeteorologyAtmosphere/MapServer"));
        }

        [TestMethod()]
        public void RequestConstructorTest_CatalogOnlyWithSlash()
        {
            Request request = new Request(idahoCatalogUrlWithSlash);
            // make sure the constructor added the 'MapServer' path
            Assert.IsTrue(request.Url.PathAndQuery.Contains("MapServer"));
            // make sure the Uri ends with a slash, even though we didn't provide one
            Assert.IsTrue(request.Url.PathAndQuery.EndsWith("/"));
            // make sure there is a single slash between the catalog and 'MapServer' (that it has been canonicalized).
            Assert.IsTrue(request.Url.PathAndQuery.Contains("climatologyMeteorologyAtmosphere/MapServer"));
        }

        /// <summary>
        ///A test for Format
        ///</summary>
        [TestMethod()]
        public void FormatTest()
        {
            Request request = new Request(idahoMapServiceUrlWithFormat);
            Assert.AreEqual(Request.JsonFormat, request.Format);
            request.Format = Request.HtmlFormat;
            Assert.AreEqual(Request.HtmlFormat, request.Format);
            Assert.IsTrue (request.Url.Query.Contains ("=" + Request.HtmlFormat));
        }

        /// <summary>
        ///A test for RelativePath
        ///</summary>
        [TestMethod()]
        public void RelativePathTest()
        {
            Request request = new Request(idahoMapServiceUrl);
            Assert.IsTrue(String.IsNullOrEmpty(request.RelativePath));
            request.RelativePath = "layers";
            Assert.AreEqual("layers", request.RelativePath);
            Assert.IsTrue(request.Url.PathAndQuery.EndsWith ("MapServer/layers"));

            request.RelativePath = "/layers";
            Assert.AreEqual("layers", request.RelativePath);
            Assert.IsTrue(request.Url.PathAndQuery.EndsWith("MapServer/layers"));
        }
    }
}
