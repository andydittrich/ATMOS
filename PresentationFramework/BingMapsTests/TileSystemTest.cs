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

using BingMaps;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BingMapsTests
{
    
    
    /// <summary>
    ///This is a test class for TileSystemTest and is intended
    ///to contain all TileSystemTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TileSystemTest
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
        ///A test for Clip
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BingTileSystem.dll")]
        public void ClipTest()
        {
            double inRange = 15678F;
            double aboveRange = 20001F;
            double belowRange = 22F;
            double minValue = 23F;
            double maxValue = 20000F;
            Assert.AreEqual(inRange, TileSystem_Accessor.Clip(inRange, minValue, maxValue));
            Assert.AreEqual(maxValue, TileSystem_Accessor.Clip(aboveRange, minValue, maxValue));
            Assert.AreEqual(minValue, TileSystem_Accessor.Clip(belowRange, minValue, maxValue));
        }

        /// <summary>
        ///A test for GroundResolution
        ///</summary>
        [TestMethod()]
        public void GroundResolutionTest()
        {
            double latitude = 39.430392F;
            int levelOfDetail = 6;
            double expected = 1889.270827;
            double actual;
            actual = TileSystem.GroundResolution(latitude, levelOfDetail);
            Assert.AreEqual(expected, Math.Round (actual, 6));
        }

        /// <summary>
        ///A test for LatLongToPixelXY
        ///</summary>
        [TestMethod()]
        public void LatLongToPixelXYTest()
        {
            double latitude = 39.430392F;
            double longitude = -119.720142F;
            int levelOfDetail = 21;
            int pixelX = 0;
            int pixelXExpected = 89895845;
            int pixelY = 0;
            int pixelYExpected = 204352446;
            TileSystem.LatLongToPixelXY(latitude, longitude, levelOfDetail, out pixelX, out pixelY);
            Assert.AreEqual(pixelXExpected, pixelX);
            Assert.AreEqual(pixelYExpected, pixelY);
        }

        /// <summary>
        ///A test for MapScale
        ///</summary>
        [TestMethod()]
        public void MapScaleTest()
        {
            double latitude = 0F;
            int levelOfDetail = 11;
            int screenDpi = 96;
            double expected = 288895.85;
            double actual = TileSystem.MapScale(latitude, levelOfDetail, screenDpi);
            Assert.AreEqual(expected, Math.Round (actual, 2));
        }

        /// <summary>
        ///A test for MapSize
        ///</summary>
        [TestMethod()]
        public void MapSizeTest()
        {
            int levelOfDetail = 6;
            uint expected = 16384;
            uint actual = TileSystem.MapSize(levelOfDetail);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for PixelXYToLatLong
        ///</summary>
        [TestMethod()]
        public void PixelXYToLatLongTest()
        {
            int pixelX = 89895845;
            int pixelY = 204352446;
            int levelOfDetail = 21;
            double latitudeExpected = 39.430393;
            double longitudeExpected = -119.720139;
            double latitude;
            double longitude;
            TileSystem.PixelXYToLatLong(pixelX, pixelY, levelOfDetail, out latitude, out longitude);
            Assert.AreEqual(latitudeExpected, Math.Round (latitude, 6));
            Assert.AreEqual(longitudeExpected, Math.Round (longitude, 6));
        }

        /// <summary>
        ///A test for PixelXYToTileXY
        ///</summary>
        [TestMethod()]
        public void PixelXYToTileXYTest()
        {
            int pixelX = 2500;
            int pixelY = 257;
            int tileXExpected = 9;
            int tileYExpected = 1;
            int tileX;
            int tileY;
            TileSystem.PixelXYToTileXY(pixelX, pixelY, out tileX, out tileY);
            Assert.AreEqual(tileXExpected, tileX);
            Assert.AreEqual(tileYExpected, tileY);
        }

        /// <summary>
        ///A test for QuadKeyToTileXY
        ///</summary>
        [TestMethod()]
        public void QuadKeyToTileXYTest()
        {
            string quadKey = "213";
            int tileXExpected = 3;
            int tileYExpected = 5;
            int levelOfDetailExpected = 3;
            int tileX;
            int tileY;
            int levelOfDetail;
            TileSystem.QuadKeyToTileXY(quadKey, out tileX, out tileY, out levelOfDetail);
            Assert.AreEqual(tileXExpected, tileX);
            Assert.AreEqual(tileYExpected, tileY);
            Assert.AreEqual(levelOfDetailExpected, levelOfDetail);
        }

        /// <summary>
        ///A test for TileXYToPixelXY
        ///</summary>
        [TestMethod()]
        public void TileXYToPixelXYTest()
        {
            int tileX = 7;
            int tileY = 0;
            int pixelXExpected = 7 * 256;
            int pixelYExpected = 0;
            int pixelX;
            int pixelY;
            TileSystem.TileXYToPixelXY(tileX, tileY, out pixelX, out pixelY);
            Assert.AreEqual(pixelXExpected, pixelX);
            Assert.AreEqual(pixelYExpected, pixelY);
        }

        /// <summary>
        ///A test for TileXYToQuadKey
        ///</summary>
        [TestMethod()]
        public void TileXYToQuadKeyTest()
        {
            int tileX = 6;
            int tileY = 9;
            int levelOfDetail = 7;
            string expected = "0002112";
            string actual = TileSystem.TileXYToQuadKey(tileX, tileY, levelOfDetail);
            Assert.AreEqual(expected, actual);
        }
    }
}
