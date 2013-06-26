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
using System.IO;
using System.Xml;
using System.ComponentModel.Composition;
using WMS.Common;
using System.Drawing;
using System.Net;
using ServiceLogicCommon;

namespace WMS
{
    [Export(typeof(IService))]
    [ExportMetadata("Type", "WMS")]
    [ExportMetadata("HelpText", WebMapService.helpText)]
    public class WebMapService : IService, IMapService
    {
        public const string helpText = "WebMapService provides access to data sources that implement the OGC Web Map Service.  The url should be the getCapabilities query, or the root of the web service.  If no title is provided, it will be set using the title in the getCapabilitiesResponse.  If no version is provided, it will be set to the negotiated version for the service.  A version may be specified to force using that version.";
        private static readonly string getCapabilitiesResponseContentType = "application/vnd.ogc.wms_xml";
        private static readonly string getCapabilitiesExceptionContentType = "application/vnd.ogc.se_xml";
        private static readonly List<string> genericXMLContentTypes = new List<string> () { "text/xml", "application/xml" };
        private static readonly string serviceExceptionNodeName = "ServiceExceptionReport";

        private Dictionary<string, IResponseParser> parsers;

        public WebMapService()
        {
            // A slicker way to do this is to use MEF to import parsers from a folder of .dlls.  
            // That was used in the original implementation, but I ran into a problem where the 
            // list of parsers was empty in some cases, even after the composition was successful.
            // So I reverted to this.
            parsers = new Dictionary<string, IResponseParser>()
            {
                { "1.1.1", new ResponseParser_1_1_1.ResponseParser_1_1_1 () },
                { "1.3.0", new ResponseParser_1_3_0.ResponseParser_1_3_0 () },
            };
        }

        public IEnumerable<Layer> requestLayers(Service service)
        {
            GetCapabilitiesRequest getCapabilitiesRequest = new GetCapabilitiesRequest(service.Url, service.Version);
            GetCapabilitiesResponse getCapabilitiesResponse = executeRequest(getCapabilitiesRequest);
            List <Layer> layerList = new List<Layer> ();
            layerList.Add (getCapabilitiesResponse.Layer);
            return layerList;
        }

        public Bitmap requestMapImage(Service service, LayerRequest mapRequest)
        {
            GetMapRequest getMapRequest = new GetMapRequest(service.Url, service.Version);
            getMapRequest.Format = mapRequest.Format;
            getMapRequest.addLayer(mapRequest.Layer);
            getMapRequest.SpatialReferenceSystem = mapRequest.SpatialReferenceSystem;
            getMapRequest.BoundingBox = mapRequest.BoundingBox;
            getMapRequest.Height = mapRequest.Height;
            getMapRequest.Width = mapRequest.Width;
            getMapRequest.Transparent = true;

            return executeRequest(getMapRequest);
        }

        // method to retrieve information about this service
        public Service requestServiceInformation(Service service)
        {
            GetCapabilitiesRequest getCapabilitiesRequest = new GetCapabilitiesRequest(service.Url);
            GetCapabilitiesResponse getCapabilitiesResponse = executeRequest(getCapabilitiesRequest);
            service.MaxHeight = getCapabilitiesResponse.Service.MaxHeight;
            service.MaxWidth = getCapabilitiesResponse.Service.MaxWidth;
            service.Name = getCapabilitiesResponse.Service.Name;
            if (service.Title == null)
            {
                service.Title = getCapabilitiesResponse.Service.Title;
            }
            service.Url = getCapabilitiesRequest.Url.ToString ();
            service.Version = getCapabilitiesResponse.Version;

            return service;
        }

        public GetCapabilitiesResponse executeRequest(GetCapabilitiesRequest request)
        {
            GetCapabilitiesResponse getCapabilitiesResponse = null;

            // execute the http request and get the response
            System.Net.WebRequest webRequest = System.Net.WebRequest.Create(request.Url);
            System.Net.WebResponse webResponse = webRequest.GetResponse();
            StreamReader streamReader = new StreamReader(webResponse.GetResponseStream());

            // web responses don't have seek capabilities.  So we can't check the version then 
            // rewind and process the whole response.  We store the whole response into a string 
            // here to support a multi-pass processing approach.
            // TODO: limit the amount we read to avoid potential security issues.
            string response = streamReader.ReadToEnd();
            streamReader.Close();
            webResponse.Close();

            // the contennt type may contain character set information or other details after a 
            // delimiting ; character.  Only consider the first portion of the content type when 
            // determining what the response contains.
            string contentType = webResponse.ContentType.Split(';').FirstOrDefault();

            if (contentType == getCapabilitiesExceptionContentType || isException(response))
            {
                handleServiceException(response);
            }
            else if (contentType == getCapabilitiesResponseContentType ||
                     genericXMLContentTypes.Contains(contentType))
            {
                getCapabilitiesResponse = handleGetCapabilitiesResponse(response);
            }
            else
            {
                // the response is not a content type that we recognize, and it is not an exception.
                StringBuilder message = new StringBuilder("unrecognized response from web service: ");
                message.Append(webResponse.ContentType);
                throw new FormatException(message.ToString ());
            }

            return getCapabilitiesResponse;
        }

        protected GetCapabilitiesResponse handleGetCapabilitiesResponse(string response)
        {
            GetCapabilitiesResponse getCapabilitiesResponse = null;

            // if the request had a version, try parsing the response using that version
            string version = getVersion(response);
            IResponseParser parser = null;
            if (parsers.TryGetValue (version, out parser))
            {
                getCapabilitiesResponse = parser.parseResponse(response);
            }

            if (getCapabilitiesResponse == null)
            {
                // TODO: if an appropriate handler is not available, then negotiate a different version
                // for now, throw an exception
                StringBuilder message = new StringBuilder("Unable to parse response.  Version=");
                message.Append(version);
                message.Append(". ");
                message.Append(parsers.Count);
                message.Append(" available parsers: ");
                foreach (string v in parsers.Keys)
                {
                    message.Append(v);
                    message.Append(" ");
                }
                throw new FormatException(message.ToString());
            }

            return getCapabilitiesResponse;
        }

        protected void handleServiceException(string response)
        {
            // if the request had a version, try parsing the response using that version
            string version = getVersion(response);
            IResponseParser parser = null;
            if (parsers.TryGetValue (version, out parser))
            {
                parser.parseException (response);
            }
        }

        public Bitmap executeRequest(GetMapRequest request)
        {
            WebResponse webResponse = null;

            try
            {
                WebRequest webRequest = WebRequest.Create(request.Url);
                webResponse = webRequest.GetResponse();
            }
            catch (WebException e)
            {
                throw;
            }

            // is the response the type that was requested?
            if (webResponse.ContentType.Equals(request.Format))
            {
                return new Bitmap(Image.FromStream(webResponse.GetResponseStream()));
            }
            else
            {
                StringBuilder message = new StringBuilder("unexpected response type received: ");
                message.Append(webResponse.ContentType);
                throw new FormatException(message.ToString());
            }
        }

        // determine the version of the response.  One of the top-level nodes should have a 
        // version attribute.  For example, the WMT_MS_Capabilities element will have a 
        // version="1.1.1" attribute, the ServiceException will have a version, or the 
        // WMS_Capabilities element will have a version="1.3.0" attribute.  Use this to
        // determine how to parse the response.
        protected string getVersion(string response)
        {
            try
            {
                StringReader responseReader = new StringReader(response);
                XmlDocument responseDocument = new XmlDocument();
                responseDocument.Load(responseReader);
                foreach (XmlNode node in responseDocument.ChildNodes)
                {
                    if (node.Attributes != null)
                    {
                        foreach (XmlAttribute attribute in node.Attributes)
                        {
                            if (attribute.Name == "version")
                            {
                                return attribute.Value;
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // attempts to determine if the response is an exception.  This is based on the presence 
        // of the ServiceExceptionReport element as one of the top-level elements.
        protected bool isException(string response)
        {
            bool exception = false;
            try
            {
                StringReader responseReader = new StringReader(response);
                XmlDocument responseDocument = new XmlDocument();
                responseDocument.Load(responseReader);
                foreach (XmlNode node in responseDocument.ChildNodes)
                {
                    if (node.Name == serviceExceptionNodeName)
                    {
                        exception = true;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return exception;
        }
    }
}
