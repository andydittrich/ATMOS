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
using System.Data;

namespace WaterOneFlow
{
    [Export(typeof(IService))]
    [ExportMetadata("Type", "WaterOneFlow")]
    [ExportMetadata("HelpText", WaterOneFlowService.helpText)]
    public class WaterOneFlowService : IService, ITabularService
    {
        public const string helpText = "WaterOneFlow provides access to services that implement the WaterOneFlow SOAP interface.  The url should be a link to the WSDL for the service.  If a Title is not provided, it will be set to a default value.  The version is ingored.";
        // the date times in the request should be in ISO 8601 format.
        public static readonly string dateTimeFormatSpecifier = "yyyy-MM-dd";

        #region IService Members

        public Service requestServiceInformation(Service service)
        {
            // TODO: allow user to provide the site code as a parameter to limit the site selection.  Site codes look like "AgencyCode:SiteNumber", for example, "NWIS:10316500".  For now, pass an empty array to get all of the sites.
            List<string> sites = new List<string> ();
            string authToken = "";
            var client = getClient(service);
            var response = client.GetSites(sites.ToArray (), authToken);

            if (service.Title == null)
            {
                // The WaterOneFlow interface doesn't have a method to query information for the service overall.  It only has site info.  So we use a generic title here.
                // TODO: if we allow the user to provide a site code, use that in the title
                service.Title = "WaterOneFlow service";
            }
            service.Name = null;
            service.Version = null;

            return service;
        }

        #endregion

        #region ITabularService Members

        public IEnumerable<SeriesGroup> requestSeriesGroups(Service service)
        {
            List<SeriesGroup> seriesGroups = new List<SeriesGroup>();

            // get a list of sites.  Passing an empty string requests all sites.
            string authToken = "";
            List<string> sites = new List<string> ();
            var client = getClient(service);
            var response = client.GetSites(sites.ToArray(), authToken);

            // extract the series information for each site in the response
            if (response.site != null)
            {
                foreach (var site in response.site)
                {
                    // Although the site info response schema allows the series catalog to be included, 
                    // it is typically not.  So instead, we call getSiteInfoObject here to retrieve 
                    // detailed information for each site.

                    SeriesGroup seriesGroup = new SeriesGroup();
                    seriesGroup.Name = site.siteInfo.siteName;
                    // the site is specified as "network:siteCode"
                    seriesGroup.InternalName = site.siteInfo.siteCode[0].network + ":" + site.siteInfo.siteCode[0].Value;
                    seriesGroups.Add(seriesGroup);
                }
            }

            return seriesGroups;
        }

        public IEnumerable<Series> requestSeries(Service service, SeriesGroup seriesGroup)
        {
            List<Series> seriesList = new List<Series>();
            string authToken = "";
            var client = getClient(service);
            var siteInfo = client.GetSiteInfoObject(seriesGroup.InternalName, authToken);

            if (siteInfo.site[0].seriesCatalog != null)
            {
                foreach (var seriesCatalog in siteInfo.site[0].seriesCatalog)
                {
                    if (seriesCatalog.series != null)
                    {
                        foreach (var dataSeries in seriesCatalog.series)
                        {
                            Series series = new Series();
                            series.Name = dataSeries.variable.variableName + " (" + dataSeries.variable.variableCode[0].Value + ")";
                            series.InternalName = siteInfo.site[0].siteInfo.siteCode[0].network + ":" + dataSeries.variable.variableCode[0].Value;
                            series.Units = dataSeries.variable.units.Value;
                            series.DateRange = new Series.DateTimeRange();
                            series.DateRange.Start = (dataSeries.variableTimeInterval as ServiceReference.TimeIntervalType).beginDateTime;
                            series.DateRange.End = (dataSeries.variableTimeInterval as ServiceReference.TimeIntervalType).endDateTime;
                            series.SampleCount = dataSeries.valueCount.Value;

                            seriesList.Add(series);
                        }
                    }
                }
            }

            return seriesList;
        }

        public System.Data.DataTable requestData(Service service, TableRequest tableRequest)
        {
            ServiceReference.WaterOneFlowSoapClient client = getClient(service);
            string authToken = "";
            var valuesResponse = client.GetValuesObject(tableRequest.SeriesGroupInternalName, 
                                                        tableRequest.SeriesInternalName,
                                                        tableRequest.StartDate.ToString(dateTimeFormatSpecifier),
                                                        tableRequest.EndDate.ToString(dateTimeFormatSpecifier), 
                                                        authToken);

            DataTable seriesData = new DataTable ();

            // add a column for time
            seriesData.Columns.Add("Time", typeof (DateTime));

            // add a column for the variable, if it is valid
            if (valuesResponse != null &&
                valuesResponse.timeSeries != null &&
                valuesResponse.timeSeries.variable != null &&
                !String.IsNullOrEmpty(valuesResponse.timeSeries.variable.variableName))
            {
                seriesData.Columns.Add(valuesResponse.timeSeries.variable.variableName);

                // add the data
                if (valuesResponse.timeSeries.values != null &&
                    valuesResponse.timeSeries.values.value != null)
                {
                    foreach (var value in valuesResponse.timeSeries.values.value)
                    {
                        seriesData.Rows.Add(value.dateTime, value.Value);
                    }

                    return seriesData;
                }
            }

            return null;
        }

        #endregion

        protected ServiceReference.WaterOneFlowSoapClient getClient(Service service)
        {
            // specify the url and binding to make this service generic.  This allows it to support any url that implements the WaterOneFlow SOAP interface.
            System.ServiceModel.EndpointAddress address = new System.ServiceModel.EndpointAddress(service.Url);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
            binding.MaxReceivedMessageSize = 512 * 1024 * 1024;
            return new ServiceReference.WaterOneFlowSoapClient(binding, address);
        }
    }
}
