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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceLogicCommon;
using System.ComponentModel.Composition;
using System.Net;
using System.Data;
using System.Xml.Serialization;

namespace UsgsWaterData
{
    [Export(typeof(IService))]
    [ExportMetadata("Type", "USGSWaterData")]
    [ExportMetadata("HelpText", UsgsWaterDataService.helpText)]
    public class UsgsWaterDataService : IService, ITabularService
    {
        public const string helpText = "USGSWaterData provides access to time-series water data from the USGS Water Data service.  Provide a URL to the site information service for USGS water data.  Use countyCd, stateCd, or site query parameters to limit results to certain sites.  If no name is provided, a name will be created based on the query parameters.  Version numbers don't apply to this service.";

        #region IService Members

        public Service requestServiceInformation(Service service)
        {
            // Since this plugin is so specific to the usgs water data, we can almost ignore the 
            // base url here.  The query information is used to differentiate between data sources.  
            // We expect it to look something like:
            // 
            //      http://waterservices.usgs.gov/nwis/site/?site=10316500
            // or   http://waterservices.usgs.gov/nwis/site/?stateCd=NV
            // or   http://waterservices.usgs.gov/nwis/site/?countyCd=32031
            // or   http://waterservices.usgs.gov/nwis/site
            // or   http://waterservices.usgs.gov/nwis/
            //
            Request request = new Request(service.Url, Request.siteRelativePath);

            // if a site number is provided, then include that in the query for site info.  If no 
            // query string is provided, then use a county code for Washoe County (32031)
            if (request.Url.Query == null)
            {
                request.CountyCode = "32031";
            }

            WebRequest webRequest = WebRequest.Create(request.Url);
            WebResponse webResponse = webRequest.GetResponse();

            // send the site response through the rdb format parser
            RdbParser rdbParser = new RdbParser();
            rdbParser.parse(webResponse.GetResponseStream());
            Rdb rdb = rdbParser.Rdb;

            // if the title was not supplied, construct a new one
            if (service.Title == null)
            {
                StringBuilder serviceTitle = new StringBuilder("USGS Water Data");
                if (request.CountyCode != null)
                {
                    serviceTitle.Append(" (");
                    serviceTitle.Append(CountyCode.getCountyName(request.CountyCode));
                    serviceTitle.Append(")");
                }
                else if (request.StateCode != null)
                {
                    serviceTitle.Append(" (");
                    serviceTitle.Append(request.StateCode);
                    serviceTitle.Append(")");
                }
                else if (request.Site != null)
                {
                    serviceTitle.Append(" (");
                    // The site name appears in the 'station_nm' column of the first row.
                    DataTableReader reader = new DataTableReader(rdb.Data);
                    // move to the first recod
                    reader.Read();
                    serviceTitle.Append(reader["station_nm"]);
                    serviceTitle.Append(")");
                }
                service.Title = serviceTitle.ToString();
            }
            service.Url = request.Url.ToString ();

            return service;
        }

        #endregion

        #region ITabularService Members

        public IEnumerable<SeriesGroup> requestSeriesGroups(Service service)
        {
            // construct a request to retrieve the variable information
            Request request = new Request(service.Url, Request.siteRelativePath);
            request.SeriesCatalogOutput = true;

            WebRequest webRequest = WebRequest.Create(request.Url);
            WebResponse webResponse = webRequest.GetResponse();

            // send the site response through the rdb format parser
            RdbParser rdbParser = new RdbParser();
            rdbParser.parse(webResponse.GetResponseStream());
            Rdb rdb = rdbParser.Rdb;

            List<SeriesGroup> seriesGroups = new List<SeriesGroup>();

            List<Series> avaialableSeries = new List<Series>();
            foreach (DataRow row in rdb.Data.Rows)
            {
                // get the site number
                string siteNumber = row.Field<string>("site_no");
                // is there a group with this number?
                SeriesGroup seriesGroup = (from g in seriesGroups
                                           where g.InternalName == siteNumber
                                           select g).FirstOrDefault();
                // if not, create a new one.
                if (seriesGroup == null)
                {
                    seriesGroup = new SeriesGroup();
                    seriesGroup.InternalName = siteNumber;
                    seriesGroup.Name = row.Field<string>("station_nm");
                    seriesGroups.Add(seriesGroup);
                }

                // get the parameter info
                string parameterCode = row.Field<string>("parm_cd");
                // if there is no code, then skip this row
                if (String.IsNullOrEmpty(parameterCode) == false)
                {
                    Series series = new Series();
                    series.InternalName = parameterCode;
                    series.Units = ParameterCode.getParameterUnits(parameterCode);
                    series.Name = ParameterCode.getParameterName(parameterCode);

                    // get the date range
                    string startDateString = row.Field<string>("begin_date");
                    string endDateString = row.Field<string>("end_date");
                    DateTime startDate;
                    DateTime endDate;
                    DateTime.TryParse(startDateString, out startDate);
                    DateTime.TryParse(endDateString, out endDate);
                    series.DateRange = new Series.DateTimeRange();
                    series.DateRange.Start = startDate;
                    series.DateRange.End = endDate;

                    // get the sample count
                    string sampleCountString = row.Field<string>("count_nu");
                    int sampleCount = 0;
                    int.TryParse(sampleCountString, out sampleCount);
                    series.SampleCount = sampleCount;

                    seriesGroup.SeriesList.Add(series);
                }
            }

            return seriesGroups;
        }

        public IEnumerable<Series> requestSeries(Service service, SeriesGroup seriesGroup)
        {
            throw new NotImplementedException();
        }

        public System.Data.DataTable requestData(Service service, TableRequest tableRequest)
        {
            Request request = new Request(service.Url, Request.dailyValuesRelativePath);
            request.StartDate = tableRequest.StartDate;
            request.EndDate = tableRequest.EndDate;
            request.ParameterCode = tableRequest.SeriesInternalName;
            request.Site = tableRequest.SeriesGroupInternalName;

            WebRequest webRequest = WebRequest.Create(request.Url);
            WebResponse webResponse = webRequest.GetResponse();

            XmlSerializer serializer = new XmlSerializer(typeof(TimeSeriesResponseType));
            TimeSeriesResponseType timeSeriesResponse = (TimeSeriesResponseType)serializer.Deserialize(webResponse.GetResponseStream());
            DataTable seriesData = null;

            if (timeSeriesResponse.timeSeries != null)
            {
                seriesData = new DataTable();

                // add a column for time and each data type in the series
                seriesData.Columns.Add("Time", typeof (DateTime));

                // TODO: add support for multiple series in one response
                var series = timeSeriesResponse.timeSeries[0];
                //foreach (var series in timeSeriesResponse.timeSeries)
                //{
                    // we could use the variable Name, but this doesn't always match the description we 
                    // used based on the parameter code.  To make sure they match, look it up from the 
                    // parameter code here too.
                    string parameterName = ParameterCode.getParameterName(series.variable.variableCode[0].Value);
                    seriesData.Columns.Add(parameterName);
                //}

                // add the data to the table
                //foreach (var series in timeSeriesResponse.timeSeries)
                //{
                if (series.values != null && series.values[0].value != null)
                {
                    foreach (var value in series.values[0].value)
                    {
                        seriesData.Rows.Add(value.dateTime, value.Value);
                    }
                }
                //}
            }

            return seriesData;
        }

        #endregion
    }
}
