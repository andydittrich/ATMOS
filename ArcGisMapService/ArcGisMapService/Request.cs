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

namespace ArcGisMapService
{
    // this is the base class for all ArcGIS Map Service requests.

    public class Request : HttpRequest
    {
        protected static readonly string formatKey = "f";

        // define the currently supported (as of version 10) formats. This is meant for convenience, 
        // not to restrict the possibilities.  The Format property can be set to any string.
        public static readonly string HtmlFormat = "html";
        public static readonly string JsonFormat = "json";
        public static readonly string KmzFormat = "kmz";
        public static readonly string NmfFormat = "nmf";
        public static readonly string LyrFormat = "lyr";
        public static readonly string JavascriptApiFormat = "jsapi";
        public static readonly string VirtualEarthFormat = "ve";
        public static readonly string GoogleMapsFormat = "gmaps";
        public static readonly string ImageFormat = "image";

        protected Uri rootMapServiceUri;

        public Request(string mapServiceUrl)
            : base(mapServiceUrl)
        {
            // verify that the url matches: http://<catalog-url>/<serviceName>/MapServer
            uriBuilder.Path = uriBuilder.Path.TrimEnd('/');
            if (!uriBuilder.Path.EndsWith("MapServer"))
            {
                uriBuilder.Path += "/MapServer";
            }
            uriBuilder.Path += "/";
            // save off the root map service url
            rootMapServiceUri = uriBuilder.Uri;
        }

        // sets the relative path of the url after 'MapServer'.
        public string RelativePath
        {
            get 
            {
                return rootMapServiceUri.MakeRelativeUri (uriBuilder.Uri).ToString ();
            }
            set
            {
                // reset the path
                uriBuilder.Path = rootMapServiceUri.PathAndQuery.Split('?').FirstOrDefault().TrimEnd('/');
                // append the new path
                uriBuilder.Path += "/" + value.TrimStart('/');
            }
        }

        public string Format
        {
            get { return getQueryString(formatKey); }
            set { setQueryString(formatKey, value); }
        }
    }
}
