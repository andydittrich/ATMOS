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
    ///This is a test class for LayerTest and is intended
    ///to contain all LayerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LayerTest
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
        ///A test for Layer Constructor
        ///</summary>
        [TestMethod()]
        public void LayerConstructorTest()
        {
            Layer layer = new Layer();
            Assert.AreEqual(0, layer.Layers.Length);
            Assert.IsNull(layer.Name);
            Assert.IsNull(layer.Title);
            Assert.IsFalse(layer.Requestable);
        }

        /// <summary>
        ///A test for addLayer
        ///</summary>
        [TestMethod()]
        public void addLayerTest()
        {
            Layer layer = new Layer();
            Layer sublayer = new Layer ();
            Assert.AreEqual(0, layer.Layers.Length);
            layer.addLayer(sublayer);
            Assert.AreEqual(1, layer.Layers.Length);
        }

        /// <summary>
        ///A test for Requestable
        ///</summary>
        [TestMethod()]
        public void RequestableTest()
        {
            Layer layer = new Layer();
            Assert.IsFalse(layer.Requestable);
            layer.Name = "name";
            Assert.IsFalse(layer.Requestable);
            layer.Title = "title";
            Assert.IsTrue(layer.Requestable);
            layer.Name = null;
            Assert.IsFalse(layer.Requestable);
        }
    }
}
