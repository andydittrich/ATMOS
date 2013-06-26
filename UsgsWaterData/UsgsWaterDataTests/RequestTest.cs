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

using UsgsWaterData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UsgsWaterDataTests
{
    
    
    /// <summary>
    ///This is a test class for RequestTest and is intended
    ///to contain all RequestTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RequestTest
    {


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
        public void RequestConstructorTest()
        {
            string url = "http://waterservices.usgs.gov/nwis/site/?site=10316500";
            Request request = new Request(url, Request.siteRelativePath);
            Assert.AreEqual(Request.siteRelativePath, request.RelativePath);
            Assert.AreEqual("site=10316500", request.Url.Query.TrimStart('?'));
        }

        [TestMethod()]
        public void RequestConstructor_ChangePathsTest()
        {
            string url = "http://waterservices.usgs.gov/nwis/dv/?site=10316500";
            Request request = new Request(url, Request.siteRelativePath);
            Assert.AreEqual(Request.siteRelativePath, request.RelativePath);
            Assert.AreEqual("site=10316500", request.Url.Query.TrimStart('?'));
        }

        [TestMethod()]
        public void RequestConstructor_NullPathsTest()
        {
            string url = "http://waterservices.usgs.gov/nwis/dv/?site=10316500";
            Request request = new Request(url);
            Assert.AreEqual(String.Empty, request.RelativePath);
            Assert.AreEqual("site=10316500", request.Url.Query.TrimStart('?'));
        }

        [TestMethod()]
        public void RequestRelativePathsTest()
        {
            string url = "http://waterservices.usgs.gov/nwis/dv/?site=10316500";
            Request request = new Request(url);
            Assert.AreEqual(String.Empty, request.RelativePath);

            request.RelativePath = Request.dailyValuesRelativePath;
            Assert.AreEqual(Request.dailyValuesRelativePath, request.RelativePath);

            request.RelativePath = Request.siteRelativePath;
            Assert.AreEqual(Request.siteRelativePath, request.RelativePath);
        }

        [TestMethod ()]
        public void RequestSeriesCatalogOutputTest ()
        {
            string urlWithNoSeriesCatalogOutput = "http://waterservices.usgs.gov/nwis/site/?format=rdb&sites=01646500";
            string urlWithSeriesCatalogOutputTrue = "http://waterservices.usgs.gov/nwis/site/?format=rdb&sites=01646500&seriesCatalogOutput=true";
            string urlWithSeriesCatalogOutputFalse = "http://waterservices.usgs.gov/nwis/site/?format=rdb&sites=01646500&seriesCatalogOutput=FALSE";

            Request request = new Request (urlWithNoSeriesCatalogOutput);
            Assert.IsFalse (request.SeriesCatalogOutput);
            request.SeriesCatalogOutput = true;
            Assert.IsTrue (request.SeriesCatalogOutput);
            Assert.IsTrue (request.Url.Query.ToLower ().Contains ("seriescatalogoutput=true"));
            request.SeriesCatalogOutput = false;
            Assert.IsFalse (request.SeriesCatalogOutput);
            Assert.IsTrue (request.Url.Query.ToLower ().Contains ("seriescatalogoutput=false"));

            request = new Request (urlWithSeriesCatalogOutputFalse);
            Assert.IsFalse (request.SeriesCatalogOutput);

            request = new Request (urlWithSeriesCatalogOutputTrue);
            Assert.IsTrue (request.SeriesCatalogOutput);
        }

        [TestMethod()]
        public void RequestStartEndDateTest()
        {
            string urlWithNoDates = "http://waterservices.usgs.gov/nwis/dv/?sites=01646500";
            string urlWithStartDate = "http://waterservices.usgs.gov/nwis/dv/?sites=01646500&startDT=01/01/2012";

            Request request = new Request(urlWithNoDates);
            Assert.IsFalse(request.StartDateSet);
            Assert.IsFalse(request.EndDateSet);

            DateTime newDate = new DateTime(2011, 12, 13, 14, 15, 16);
            DateTime expectedNewDate = new DateTime(2011, 12, 13);
            request.StartDate = newDate;
            Assert.IsTrue(request.StartDateSet);
            Assert.IsFalse(request.EndDateSet);
            Assert.AreEqual(expectedNewDate, request.StartDate);
            request.EndDate = newDate;
            Assert.IsTrue(request.EndDateSet);
            Assert.AreEqual(expectedNewDate, request.EndDate);

            DateTime expectedDate = new DateTime(2012, 1, 1);
            request = new Request(urlWithStartDate);
            Assert.IsTrue(request.StartDateSet);
            Assert.IsFalse(request.EndDateSet);
            Assert.AreEqual(expectedDate, request.StartDate);
        }
    }
}
