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

using ServiceLogicCommon;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ServiceLogicCommonTests
{
    
    
    /// <summary>
    ///This is a test class for BoundingBoxTest and is intended
    ///to contain all BoundingBoxTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BoundingBoxTest
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
        ///A test for BoundingBox Constructor
        ///</summary>
        [TestMethod()]
        public void BoundingBoxConstructorTest()
        {
            BoundingBox nevadaBox = new BoundingBox(-120.0, 34.9, -113.5, 42.1);
            Assert.IsFalse(nevadaBox.IsEmpty);
        }

        [TestMethod()]
        public void constructFromStringTest()
        {
            BoundingBox nevadaBox = new BoundingBox("-120.0,34.9,-113.5,42.1");
            Assert.AreEqual(-120.0, nevadaBox.MinX);
            Assert.AreEqual(34.9, nevadaBox.MinY);
            Assert.AreEqual(-113.5, nevadaBox.MaxX);
            Assert.AreEqual(42.1, nevadaBox.MaxY);
        }

        [TestMethod()]
        public void toStringTest()
        {
            BoundingBox northAmericaBox = new BoundingBox(-176.6, 12.2, -44.8, 83.5);
            Assert.AreEqual("-176.6,12.2,-44.8,83.5", northAmericaBox.ToString());
        }

        [TestMethod()]
        public void toStringWithFormatTest()
        {
            BoundingBox northAmericaBox = new BoundingBox(-176.6, 12.2, -44.8, 83.5);
            Assert.AreEqual("-176.600,12.200,-44.800,83.500", northAmericaBox.ToString("F3", null));
        }

        /// <summary>
        ///A test for intersect
        ///</summary>
        [TestMethod()]
        public void intersectTest()
        {
            BoundingBox nevadaBox = new BoundingBox(-120.0, 34.9, -113.5, 42.1);
            BoundingBox northAmericaBox = new BoundingBox(-176.6, 12.2, -44.8, 83.5);
            Assert.IsTrue (nevadaBox.intersect(northAmericaBox));
            Assert.IsTrue (northAmericaBox.intersect(nevadaBox));
        }

        /// <summary>
        ///A test for IsEmpty
        ///</summary>
        [TestMethod()]
        public void IsEmptyTest()
        {
            BoundingBox emptyBox = new BoundingBox(-1.0, 1.0, -10.0, 2.0);
            Assert.IsTrue(emptyBox.IsEmpty);
        }

        [TestMethod()]
        public void EqualityOperatorTest()
        {
            BoundingBox nevadaBox = new BoundingBox(-120.0, 34.9, -113.5, 42.1);
            BoundingBox referenceToNevadaBox = nevadaBox;
            BoundingBox anotherNevadaBox = new BoundingBox(-120.0, 34.9, -113.5, 42.1);
            BoundingBox northAmericaBox = new BoundingBox(-176.6, 12.2, -44.8, 83.5);
            Assert.IsTrue(nevadaBox == anotherNevadaBox);
            Assert.IsFalse(nevadaBox == northAmericaBox);
            Assert.IsFalse(nevadaBox == null);
            Assert.IsTrue(nevadaBox == referenceToNevadaBox);
        }

        [TestMethod()]
        public void InequalityOperatorTest()
        {
            BoundingBox nevadaBox = new BoundingBox(-120.0, 34.9, -113.5, 42.1);
            BoundingBox anotherNevadaBox = new BoundingBox(-120.0, 34.9, -113.5, 42.1);
            BoundingBox northAmericaBox = new BoundingBox(-176.6, 12.2, -44.8, 83.5);
            Assert.IsTrue(nevadaBox != northAmericaBox);
            Assert.IsFalse(nevadaBox != anotherNevadaBox);
        }

        [TestMethod()]
        public void EqualsTest()
        {
            BoundingBox nevadaBox = new BoundingBox(-120.0, 34.9, -113.5, 42.1);
            BoundingBox anotherNevadaBox = new BoundingBox(-120.0, 34.9, -113.5, 42.1);
            BoundingBox northAmericaBox = new BoundingBox(-176.6, 12.2, -44.8, 83.5);
            object someObject = new object();
            Assert.IsTrue(nevadaBox.Equals(nevadaBox));
            Assert.IsTrue(nevadaBox.Equals(anotherNevadaBox));
            Assert.IsTrue(anotherNevadaBox.Equals(nevadaBox));
            Assert.IsFalse(nevadaBox.Equals(northAmericaBox));
            Assert.IsFalse(nevadaBox.Equals(someObject));
            Assert.IsFalse(nevadaBox.Equals(null));
        }
    }
}
