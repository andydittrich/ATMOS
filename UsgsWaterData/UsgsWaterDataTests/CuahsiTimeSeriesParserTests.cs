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
using System.Xml.Serialization;
using UsgsWaterData;
using System.IO;

namespace UsgsWaterDataTests
{
    [TestClass]
    public class CuahsiTimeSeriesParserTests
    {
        [TestMethod]
        public void TestParseGetSiteInfoMultiple ()
        {
            // parse the site info response
            StringReader reader = new StringReader(TestResources.GetSiteInfoMultiple);
            XmlSerializer serializer = new XmlSerializer(typeof(SiteInfoResponseType));
            SiteInfoResponseType siteInfo = (SiteInfoResponseType)serializer.Deserialize(reader);

            // spot check some of the info, especially the info we will use

            // verify the multiple site info
            Assert.AreEqual(2, siteInfo.site.Length);
            Assert.AreEqual("Little Bear River at Mendon Road near Mendon, Utah", siteInfo.site[0].siteInfo.siteName);
            Assert.AreEqual(23, siteInfo.site[0].seriesCatalog[0].series.Length);
            Assert.AreEqual("Little Bear River near Wellsville, Utah", siteInfo.site[1].siteInfo.siteName);
            Assert.AreEqual(17, siteInfo.site[1].seriesCatalog[0].series.Length);

            // verify the details of one series
            var temperatureSeries = (from series in siteInfo.site[0].seriesCatalog[0].series
                                     where series.variable.variableName == "Temperature" && series.variable.dataType == "Average"
                                     select series).FirstOrDefault();
            Assert.IsNotNull(temperatureSeries);
            Assert.AreEqual("degree celcius", temperatureSeries.variable.unit.unitName);
            Assert.AreEqual("minute", temperatureSeries.variable.timeScale.unit.unitName);
            Assert.AreEqual(10373, temperatureSeries.valueCount.Value);
            Assert.AreEqual("Water temperature measured using a Hydrolab MS5 Water Quality Multiprobe.", temperatureSeries.method.methodDescription);
            DateTime expectedBeginDate = new DateTime (2007, 8, 16, 16, 30, 0);
            DateTime expectedEndDate = new DateTime (2008, 03, 27, 12, 30, 0);
            Assert.AreEqual(expectedBeginDate, temperatureSeries.variableTimeInterval.beginDateTime);
            Assert.AreEqual(expectedEndDate, temperatureSeries.variableTimeInterval.endDateTime);
        }

        [TestMethod]
        public void TestParseGetValuesForSiteMendon()
        {
            // parse the example time series response
            StringReader reader = new StringReader(TestResources.GetVaulesforSite_Mendon);
            XmlSerializer serializer = new XmlSerializer(typeof(TimeSeriesResponseType));
            TimeSeriesResponseType timeSeriesResponse = (TimeSeriesResponseType)serializer.Deserialize(reader);

            // spot check some of the info, especially the info we will use

            // verify the site parameter from the query
            var siteParameter = (from parameter in timeSeriesResponse.queryInfo.criteria.parameter
                                  where parameter.name == "site"
                                  select parameter).FirstOrDefault ();
            Assert.AreEqual("LBR_TEST:USU-LBR-Mendon", siteParameter.value);

            // verify the time series details
            Assert.AreEqual(12, timeSeriesResponse.timeSeries.Length);

            // verify that there are 3 Gage Height series
            var gageHeightTimeSeries = from timeSeries in timeSeriesResponse.timeSeries
                                       where timeSeries.variable.variableName == "Gage height"
                                       select timeSeries;
            Assert.AreEqual(3, gageHeightTimeSeries.Count ());

            // verify details of the average Gage Height series
            var averageGageHeightTimeSeries = (from timeSeries in gageHeightTimeSeries
                                               where timeSeries.variable.dataType == "Average"
                                               select timeSeries).FirstOrDefault();
            Assert.AreEqual("ft", averageGageHeightTimeSeries.variable.unit.unitAbbreviation);
            Assert.AreEqual(-9999, averageGageHeightTimeSeries.variable.noDataValue);
            Assert.AreEqual("minute", averageGageHeightTimeSeries.variable.timeScale.unit.unitName);

            // verify some of the average gage height values
            var averageGageHeights = from values in averageGageHeightTimeSeries.values[0].value
                                     where values.Value >= 1.8m
                                     orderby values.Value ascending
                                     select values;
            Assert.AreEqual(19, averageGageHeights.Count());
            Assert.AreEqual(1.800414m, averageGageHeights.First().Value);
            DateTime expectedMinimumGageHeightTime = new DateTime(2005, 08, 05, 15, 0, 0);
            Assert.AreEqual(expectedMinimumGageHeightTime, averageGageHeights.First().dateTime);
            Assert.AreEqual(1.821758m, averageGageHeights.Last().Value);
            DateTime expectedMaximumGageHeightTime = new DateTime(2005, 08, 05, 22, 0, 0);
            Assert.AreEqual(expectedMaximumGageHeightTime, averageGageHeights.Last().dateTime);
        }

        [TestMethod]
        public void TestParserGetVariableInfo()
        {
            // parse the example time series response
            StringReader reader = new StringReader(TestResources.GetVariableInfo_All);
            XmlSerializer serializer = new XmlSerializer(typeof(VariablesResponseType));
            VariablesResponseType variablesResponse = (VariablesResponseType)serializer.Deserialize(reader);

            // spot check some of the info, especially the info we will use

            // verify the query info
            Assert.AreEqual("OD Web Service", variablesResponse.queryInfo.note[0].Value);

            // verify the variables data
            Assert.AreEqual(42, variablesResponse.variables.Length);

            // spot check the Precipitation variables
            var precipitationVariables = from variable in variablesResponse.variables
                                         where variable.variableName == "Precipitation"
                                         select variable;
            Assert.IsNotNull(precipitationVariables);
            Assert.AreEqual(2, precipitationVariables.Count());
            var hourlyPrecipitationVariable = (from variable in precipitationVariables
                                               where variable.timeScale.unit.unitName == "hour"
                                               select variable).First();
            Assert.IsNotNull(hourlyPrecipitationVariable);
            Assert.AreEqual("Field Observation", hourlyPrecipitationVariable.valueType);
            Assert.AreEqual("Climate", hourlyPrecipitationVariable.generalCategory);
            Assert.AreEqual("millimeter", hourlyPrecipitationVariable.unit.unitName);
            Assert.AreEqual("54", hourlyPrecipitationVariable.unit.unitCode);
        }
    }
}
