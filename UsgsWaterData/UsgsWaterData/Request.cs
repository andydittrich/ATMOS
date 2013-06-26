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

namespace UsgsWaterData
{
    public class Request : HttpRequest
    {
        public static readonly string siteKey = "site";
        public static readonly string countyCodeKey = "countyCd";
        public static readonly string stateCodeKey = "stateCd";
        public static readonly string seriesCatalogOutputKey = "seriesCatalogOutput";
        public static readonly string startDateTimeKey = "startDT";
        public static readonly string endDateTimeKey = "endDT";
        public static readonly string parameterCodeKey = "parameterCd";

        public static readonly string siteRelativePath = "site";
        public static readonly string dailyValuesRelativePath = "dv";

        // the date times in the request should be in ISO 8601 format.
        public static readonly string dateTimeFormatSpecifier = "yyyy-MM-dd";

        protected readonly List<string> relativePaths = new List<string> { siteRelativePath, 
                                                                           dailyValuesRelativePath };

        public Request(string url, string relativePath=null)
            : base(url)
        {
            // strip off all relative paths that may be included in the supplied url
            uriBuilder.Path = uriBuilder.Path.TrimEnd('/');
            foreach (string path in relativePaths)
            {
                if (uriBuilder.Path.EndsWith(path))
                {
                    uriBuilder.Path = uriBuilder.Path.Remove(uriBuilder.Path.LastIndexOf(path));
                    uriBuilder.Path = uriBuilder.Path.TrimEnd('/');
                }
            }
            uriBuilder.Path += "/";

            // store the root service uri so we can use it with various relative paths
            rootServiceUri = uriBuilder.Uri;
            RelativePath = relativePath;
        }

        protected Uri rootServiceUri { get; set; }

        public string RelativePath
        {
            get
            {
                // get the path relative to the root service uri that we stored in the constructor
                Uri relativeUri = rootServiceUri.MakeRelativeUri(uriBuilder.Uri);
                // return just the path portion, not the query.
                return relativeUri.ToString().Split('?').FirstOrDefault();
            }
            set
            {
                // reset the path
                uriBuilder.Path = rootServiceUri.PathAndQuery.Split('?').FirstOrDefault();
                // append the new path, if provided
                if (String.IsNullOrEmpty(value) == false)
                {
                    uriBuilder.Path = uriBuilder.Path.TrimEnd('/') + "/" + value.TrimStart('/');
                }
            }
        }

        public string Site
        {
            get { return getQueryString(siteKey); }
            set { setQueryString(siteKey, value); }
        }

        public string CountyCode
        {
            get { return getQueryString(countyCodeKey); }
            set { setQueryString(countyCodeKey, value); }
        }

        public bool SeriesCatalogOutput
        {
            get 
            {
                bool seriesCatalogOutput = false;
                Boolean.TryParse (getQueryString (seriesCatalogOutputKey), out seriesCatalogOutput);
                return seriesCatalogOutput;
            }
            set { setQueryString(seriesCatalogOutputKey, value.ToString ().ToLower ()); }
        }

        public string StateCode
        {
            get { return getQueryString(stateCodeKey); }
            set { setQueryString(stateCodeKey, value); }
        }

        public bool StartDateSet
        {
            get { return (String.IsNullOrEmpty(getQueryString(startDateTimeKey)) == false); }
        }

        public DateTime StartDate
        {
            get
            {
                DateTime startDate = new DateTime ();
                DateTime.TryParse (getQueryString (startDateTimeKey), out startDate);
                return startDate;
            }
            set { setQueryString (startDateTimeKey, value.ToString (dateTimeFormatSpecifier)); }
        }

        public bool EndDateSet
        {
            get { return (String.IsNullOrEmpty(getQueryString(endDateTimeKey)) == false); }
        }

        public DateTime EndDate
        {
            get
            {
                DateTime endDate = new DateTime();
                DateTime.TryParse(getQueryString(endDateTimeKey), out endDate);
                return endDate;
            }
            set { setQueryString(endDateTimeKey, value.ToString(dateTimeFormatSpecifier)); }
        }

        public string ParameterCode
        {
            get { return getQueryString(parameterCodeKey); }
            set { setQueryString(parameterCodeKey, value); }
        }
    }
}
