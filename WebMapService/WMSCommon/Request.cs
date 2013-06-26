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
using System.Web;
using System.Collections.Specialized;
using ServiceLogicCommon;

namespace WMS.Common
{
    // This is the base class for WMS HTTP Get requests.  It provides support for manipulating the 
    // query string, and defines the common query keys for all WMS requests.
    public class Request : HttpRequest
    {
        public static readonly string serviceKey = "SERVICE";
        public static readonly string requestKey = "REQUEST";
        public static readonly string versionKey = "VERSION";

        protected static readonly string defaultService = "WMS";

        // constructs a request based on the passed string, which is expected to be a url.  
        // The url may contain a query string.
        public Request(string url) : base (url)
        {
            Service = defaultService;
        }

        public Request(string url, string version) : base (url)
        {
            Service = defaultService;
            Version = version;
        }

        // Note: changing the version may affect the key names
        public string Version
        {
            get { return getQueryString (versionKey); }
            set { setQueryString (versionKey, value); }
        }

        public string RequestType
        {
            get { return getQueryString(requestKey); }
            set { setQueryString(requestKey, value); }
        }

        public string Service
        {
            get { return getQueryString(serviceKey); }
            set { setQueryString(serviceKey, value); }
        }
    }
}
