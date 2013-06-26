﻿// Copyright 2012 Andrew Dittrich
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
using UsgsWaterData;

namespace UsgsWaterDataTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class CountyCodeTest
    {
        public CountyCodeTest()
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
        public void TestCountyCodeLookup ()
        {
            // unique county name
            string code = CountyCode.getCountyCode("Miami-Dade County");
            Assert.AreEqual("12086", code);

            // duplicate county name, we expect to find the first
            code = CountyCode.getCountyCode ("Monroe County");
            Assert.AreEqual("01099", code);
        }

        [TestMethod]
        public void TestInvalidCountyCodeLookup()
        {
            string code = CountyCode.getCountyCode("Unlikely County Name");
            Assert.IsNull(code);
        }

        [TestMethod]
        public void TestCountyNameLookup()
        {
            string name = CountyCode.getCountyName("17101");
            Assert.AreEqual("Lawrence County", name);
        }

        [TestMethod]
        public void TestInvalidCountyNameLookup()
        {
            string name = CountyCode.getCountyName("99999");
            Assert.IsNull(name);
        }
    }
}
