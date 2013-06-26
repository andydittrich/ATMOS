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

using WMS.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace WMSCommonTests
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
            string url = "http://gstore.unm.edu/apps/rgis/datasets/159893/services/ogc/wms?VERSION=1.1.1&SERVICE=WMS&REQUEST=GetCapabilities";
            Request Request = new Request(url);
            Assert.AreEqual("1.1.1", Request.Version);
            Assert.AreEqual("WMS", Request.Service);
            Assert.AreEqual("GetCapabilities", Request.RequestType);
            Assert.AreEqual("http", Request.Url.Scheme);
            Assert.AreEqual("http://gstore.unm.edu/apps/rgis/datasets/159893/services/ogc/wms", Request.Url.GetLeftPart(UriPartial.Path));
        }

        [TestMethod()]
        public void RequestConstructorLowercaseTest()
        {
            string url = "http://gstore.unm.edu/apps/rgis/datasets/159893/services/ogc/wms?VERSION=1.1.1&service=WMS&request=getcapabilities";
            Request Request = new Request(url);
            Assert.AreEqual("1.1.1", Request.Version);
            Assert.AreEqual("WMS", Request.Service);
            Assert.AreEqual("GETCAPABILITIES", Request.RequestType.ToUpper());
            Assert.AreEqual("http", Request.Url.Scheme);
            Assert.AreEqual("http://gstore.unm.edu/apps/rgis/datasets/159893/services/ogc/wms", Request.Url.GetLeftPart(UriPartial.Path));
        }

        [TestMethod()]
        public void RequestConstructorMixedCaseTest()
        {
            string url = "http://gstore.unm.edu/apps/rgis/datasets/159893/services/ogc/wms?VeRsIoN=1.3.0&sErvIce=WMS&reQueSt=getcApabilities";
            Request Request = new Request(url);
            Assert.AreEqual("1.3.0", Request.Version);
            Assert.AreEqual("WMS", Request.Service);
            Assert.AreEqual("GETCAPABILITIES", Request.RequestType.ToUpper());
            Assert.AreEqual("http", Request.Url.Scheme);
            Assert.AreEqual("http://gstore.unm.edu/apps/rgis/datasets/159893/services/ogc/wms", Request.Url.GetLeftPart(UriPartial.Path));
        }

        /// <summary>
        ///A test for Request Constructor
        ///</summary>
        [TestMethod()]
        public void RequestConstructorNoQueryTest()
        {
            string url = "http://gstore.unm.edu/apps/rgis/datasets/159893/services/ogc/wms";
            Request Request = new Request(url);
            Assert.AreEqual("http", Request.Url.Scheme);
            Assert.AreEqual("WMS", Request.Service);
            Assert.IsNull(Request.Version);
            Assert.IsNull(Request.RequestType);
            Assert.AreEqual("http://gstore.unm.edu/apps/rgis/datasets/159893/services/ogc/wms", Request.Url.GetLeftPart(UriPartial.Path));
        }

        /// <summary>
        ///A test for Request Constructor
        ///</summary>
        [TestMethod()]
        public void RequestConstructorWithVersion()
        {
            string url = "http://gstore.unm.edu/apps/rgis/datasets/159893/services/ogc/wms";
            string version = "1.2.3.4";
            Request Request = new Request(url, version);
            Assert.AreEqual("http", Request.Url.Scheme);
            Assert.AreEqual("WMS", Request.Service);
            Assert.AreEqual(version, Request.Version);
            Assert.IsTrue(Request.Url.Query.Contains(version));
            Assert.IsNull(Request.RequestType);
            Assert.AreEqual("http://gstore.unm.edu/apps/rgis/datasets/159893/services/ogc/wms", Request.Url.GetLeftPart(UriPartial.Path));
        }

        /// <summary>
        ///A test for Request construvtor using a version string with trailing space
        ///</summary>
        [TestMethod()]
        public void RequestConstructorWithTrailingSpaceVersion()
        {
            string url = "http://gstore.unm.edu/apps/rgis/datasets/159893/services/ogc/wms";
            string version = "1.2.3.4    ";
            Request Request = new Request(url, version);
            Assert.AreEqual("1.2.3.4", Request.Version);
        }

        /// <summary>
        ///A test for changing versions
        ///</summary>
        [TestMethod()]
        public void RequestChangeVersionTest ()
        {
            string url = "http://gstore.unm.edu/apps/rgis/datasets/159893/services/ogc/wms?VERSION=1.1.1&SERVICE=WMS&REQUEST=GetCapabilities";
            Request Request = new Request(url);
            Assert.AreEqual("1.1.1", Request.Version);
            Request.Version = "1.3.0";
            Assert.AreEqual("1.3.0", Request.Version);
        }

        /// <summary>
        ///A test for changing versions
        ///</summary>
        [TestMethod()]
        public void RequestSetNullVersionTest()
        {
            string url = "http://gstore.unm.edu/apps/rgis/datasets/159893/services/ogc/wms?VERSION=1.3.0&SERVICE=WMS&REQUEST=GetCapabilities";
            Request Request = new Request(url);
            Assert.AreEqual("1.3.0", Request.Version);
            Request.Version = null;
            Assert.IsNull (Request.Version);
            Assert.IsFalse(Request.Url.Query.Contains(Request.versionKey));
        }

        /// <summary>
        ///A test for changing request types
        ///</summary>
        [TestMethod()]
        public void RequestChangeRequestTypeTest()
        {
            string url = "http://gstore.unm.edu/apps/rgis/datasets/159893/services/ogc/wms?VERSION=1.1.1&SERVICE=WMS&REQUEST=GetCapabilities";
            Request Request = new Request(url);
            Assert.AreEqual("GetCapabilities", Request.RequestType);
            Request.RequestType = "ADifferentType";
            Assert.AreEqual("ADifferentType", Request.RequestType);
        }

        /// <summary>
        ///A test for a poorly formatted url
        ///</summary>
        [TestMethod()]
        public void RequestNullUrlTest()
        {
            bool caughtException = false;
            string url = null;
            try
            {
                Request Request = new Request(url);
            }
            catch (ArgumentNullException)
            {
                caughtException = true;
            }
            Assert.IsTrue(caughtException);
        }

        /// <summary>
        ///A test for a poorly formatted url
        ///</summary>
        [TestMethod()]
        public void RequestEmptyUrlTest()
        {
            bool caughtException = false;
            string url = "  ";
            try
            {
                Request Request = new Request(url);
            }
            catch (UriFormatException)
            {
                caughtException = true;
            }
            Assert.IsTrue(caughtException);
        }

        /// <summary>
        ///A test for a poorly formatted url
        ///</summary>
        [TestMethod()]
        public void RequestPoorlyFormattedUrlTest()
        {
            bool caughtException = false;
            string url = "this is not a url/:4";
            try
            {
                Request Request = new Request(url);
            }
            catch (UriFormatException)
            {
                caughtException = true;
            }
            Assert.IsTrue(caughtException);
        }
    }
}
