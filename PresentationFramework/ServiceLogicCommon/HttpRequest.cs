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

namespace ServiceLogicCommon
{
    // This is the base class for WMS HTTP Get requests.  It provides support for manipulating the 
    // query string, and defines the common query keys for all WMS requests.
    public class HttpRequest
    {
        protected UriBuilder uriBuilder = null;

        // constructs a request based on the passed string, which is expected to be a url.  
        // The url may contain a query string.
        public HttpRequest(string url)
        {
            uriBuilder = new UriBuilder(url);
        }

        public Uri Url
        {
            get { return uriBuilder.Uri; }
        }

        protected void setQueryString(string key, string value)
        {
            // convert key to upper before adding
            key = key.ToUpper();
            Dictionary<string, string> queryDict = QueryDict;
            if (value != null)
            {
                // trim leading and trailing space from value
                queryDict[key] = value.Trim();
            }
            else if (queryDict.ContainsKey (key))
            {
                // passing a null value removes the key from the query
                queryDict.Remove(key);
            }
            QueryDict = queryDict;
        }

        protected string getQueryString(string key)
        {
            string value = null;
            // convert key to upper before searching
            key = key.ToUpper();
            Dictionary<string, string> queryDict = QueryDict;
            if (queryDict.ContainsKey(key))
            {
                value = queryDict[key];
            }
            return value;
        }

        protected Dictionary<string, string> QueryDict
        {
            get
            {
                Dictionary<string, string> queryDict = new Dictionary<string,string>();
                try
                {
                    if (uriBuilder.Query.Length > 1)
                    {
                        string query = uriBuilder.Query.TrimStart('?');
                        string[] queryPairs = query.Split('&');
                        // note, we convert the keys to upper when creating dictionaty.
                        queryDict = queryPairs.ToDictionary(s => s.Split('=')[0].ToUpper(), s => s.Split('=')[1]);
                    }
                }
                catch (IndexOutOfRangeException e)
                {
                    // the query string is empty or doesn't contain the ? or & delimiters
                }
                catch (ArgumentNullException e)
                {
                    // the key selector produced null keys
                }
                catch (ArgumentException e)
                {
                    // the key selector produced duplicate keys
                }
                return queryDict;
            }
            set
            {
                string query = "";
                foreach (KeyValuePair<string, string> pair in value)
                {
                    if (query.Length != 0)
                    {
                        query += "&";
                    }
                    query += pair.Key + "=" + pair.Value;
                }
                uriBuilder.Query = query;
            }
        }
    }
}
