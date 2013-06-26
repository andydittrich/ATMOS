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
using System.IO;
using System.Text;

namespace UsgsWaterDataTests
{
    
    
    /// <summary>
    ///This is a test class for RdbParserTest and is intended
    ///to contain all RdbParserTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RdbParserTest
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
        ///A test for RdbParser Constructor
        ///</summary>
        [TestMethod()]
        public void RdbParserConstructorTest()
        {
            RdbParser parser = new RdbParser();
            Assert.IsNotNull(parser.Rdb);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseBasicRdbTest()
        {
            RdbParser parser = new RdbParser();
            MemoryStream source = new MemoryStream(Encoding.UTF8.GetBytes(TestResources.Site1031650_rdb));
            parser.parse(source);
            // verify data exists
            Assert.IsNotNull(parser.Rdb.Data);
            Assert.IsNotNull(parser.Rdb.Data.Columns);
            Assert.IsNotNull(parser.Rdb.Data.Rows);
            // verify column count
            Assert.AreEqual(11, parser.Rdb.Data.Columns.Count);
            // verify column details
            Assert.AreEqual("agency_cd",            parser.Rdb.Data.Columns[0].ColumnName);
            Assert.AreEqual(typeof(string),         parser.Rdb.Data.Columns[0].DataType);
            Assert.AreEqual(5,                      parser.Rdb.Data.Columns[0].MaxLength);
            Assert.AreEqual("site_no",              parser.Rdb.Data.Columns[1].ColumnName);
            Assert.AreEqual(typeof(string),         parser.Rdb.Data.Columns[1].DataType);
            Assert.AreEqual(15,                     parser.Rdb.Data.Columns[1].MaxLength);
            Assert.AreEqual("station_nm",           parser.Rdb.Data.Columns[2].ColumnName);
            Assert.AreEqual(typeof(string),         parser.Rdb.Data.Columns[2].DataType);
            Assert.AreEqual(50,                     parser.Rdb.Data.Columns[2].MaxLength);
            Assert.AreEqual("site_tp_cd",           parser.Rdb.Data.Columns[3].ColumnName);
            Assert.AreEqual(typeof(string),         parser.Rdb.Data.Columns[3].DataType);
            Assert.AreEqual(7,                      parser.Rdb.Data.Columns[3].MaxLength);
            Assert.AreEqual("dec_lat_va",           parser.Rdb.Data.Columns[4].ColumnName);
            Assert.AreEqual(typeof(string),         parser.Rdb.Data.Columns[4].DataType);
            Assert.AreEqual(16,                     parser.Rdb.Data.Columns[4].MaxLength);
            Assert.AreEqual("dec_long_va",          parser.Rdb.Data.Columns[5].ColumnName);
            Assert.AreEqual(typeof(string),         parser.Rdb.Data.Columns[5].DataType);
            Assert.AreEqual(16,                     parser.Rdb.Data.Columns[5].MaxLength);
            Assert.AreEqual("coord_acy_cd",         parser.Rdb.Data.Columns[6].ColumnName);
            Assert.AreEqual(typeof(string),         parser.Rdb.Data.Columns[6].DataType);
            Assert.AreEqual(1,                      parser.Rdb.Data.Columns[6].MaxLength);
            Assert.AreEqual("dec_coord_datum_cd",   parser.Rdb.Data.Columns[7].ColumnName);
            Assert.AreEqual(typeof(string),         parser.Rdb.Data.Columns[7].DataType);
            Assert.AreEqual(10,                     parser.Rdb.Data.Columns[7].MaxLength);
            Assert.AreEqual("alt_va",               parser.Rdb.Data.Columns[8].ColumnName);
            Assert.AreEqual(typeof(string),         parser.Rdb.Data.Columns[8].DataType);
            Assert.AreEqual(8,                      parser.Rdb.Data.Columns[8].MaxLength);
            Assert.AreEqual("alt_acy_va",           parser.Rdb.Data.Columns[9].ColumnName);
            Assert.AreEqual(typeof(string),         parser.Rdb.Data.Columns[9].DataType);
            Assert.AreEqual(3,                      parser.Rdb.Data.Columns[9].MaxLength);
            Assert.AreEqual("alt_datum_cd",         parser.Rdb.Data.Columns[10].ColumnName);
            Assert.AreEqual(typeof(string),         parser.Rdb.Data.Columns[10].DataType);
            Assert.AreEqual(10,                     parser.Rdb.Data.Columns[10].MaxLength);
            // verify row count
            Assert.AreEqual(1, parser.Rdb.Data.Rows.Count);
            // verify table data
            Assert.AreEqual("USGS",                         parser.Rdb.Data.Rows[0][0] as string);
            Assert.AreEqual("10316500",                     parser.Rdb.Data.Rows[0][1] as string);
            Assert.AreEqual("LAMOILLE CK NR LAMOILLE, NV",  parser.Rdb.Data.Rows[0][2] as string);
            Assert.AreEqual("ST",                           parser.Rdb.Data.Rows[0][3] as string);
            Assert.AreEqual("40.69076057",                  parser.Rdb.Data.Rows[0][4] as string);
            Assert.AreEqual("-115.477003",                  parser.Rdb.Data.Rows[0][5] as string);
            Assert.AreEqual("S",                            parser.Rdb.Data.Rows[0][6] as string);
            Assert.AreEqual("NAD83",                        parser.Rdb.Data.Rows[0][7] as string);
            Assert.AreEqual("6240.",                        parser.Rdb.Data.Rows[0][8] as string);
            Assert.AreEqual(" 40",                          parser.Rdb.Data.Rows[0][9] as string);
            Assert.AreEqual("NGVD29",                       parser.Rdb.Data.Rows[0][10] as string);
        }
    }
}
