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
using WMS.Common;
using WMS.Capabilities_1_1_1;
using WMS.Exceptions_1_1_1;
using System.Xml.Serialization;
using ServiceLogicCommon;

namespace WMS.ResponseParser_1_1_1
{
    public class ResponseParser_1_1_1 : IResponseParser
    {
        public GetCapabilitiesResponse parseResponse(string response)
        {
            // use the XML serializer to deserialize the xml response string into strongly typed classes based on the schema.
            StringReader responseReader = new StringReader(response);
            XmlSerializer serializer = new XmlSerializer(typeof(WMT_MS_Capabilities));
            WMT_MS_Capabilities capabilities_1_1_1 = (WMT_MS_Capabilities)serializer.Deserialize(responseReader);

            // convert the schema specific classes into generic, version-agnostic classes 
            GetCapabilitiesResponse getCapabilitiesResponse = new GetCapabilitiesResponse(capabilities_1_1_1.version);
            getCapabilitiesResponse.GetMapUrl = capabilities_1_1_1.Capability.Request.GetMap.DCPType[0].HTTP.Get.OnlineResource.href;
            getCapabilitiesResponse.MapFormats = capabilities_1_1_1.Capability.Request.GetMap.Format;

            // parse the service information
            getCapabilitiesResponse.Service = parseService (capabilities_1_1_1.Service);

            // parse the layers
            getCapabilitiesResponse.Layer = parseLayer(capabilities_1_1_1.Capability.Layer[0]);
            
            return getCapabilitiesResponse;
        }

        // creates a Service from the passed version-specific service
        protected Service parseService(WMT_MS_CapabilitiesService _service)
        {
            Service service = new Service();
            service.Name = _service.Name;
            service.Title = _service.Title;
            service.Url = _service.OnlineResource.href;
            return service;
        }

        // creates a Layer from the passed version-specific layer
        protected ServiceLogicCommon.Layer parseLayer(WMS.Capabilities_1_1_1.Layer _layer)
        {
            ServiceLogicCommon.Layer layer = new ServiceLogicCommon.Layer();
            layer.Name = _layer.Name;
            layer.Title = _layer.Title;

            if (_layer.SRS != null && _layer.SRS.Length > 0)
            {
                layer.SpatialReferenceSystem = _layer.SRS;
            }

            if (_layer.LatLonBoundingBox != null && _layer.LatLonBoundingBox.Length > 0)
            {
                layer.BoundingBox = new BoundingBox(Double.Parse(_layer.LatLonBoundingBox[0].minx),
                                                    Double.Parse(_layer.LatLonBoundingBox[0].miny),
                                                    Double.Parse(_layer.LatLonBoundingBox[0].maxx),
                                                    Double.Parse(_layer.LatLonBoundingBox[0].maxy));
            }

            // process the sublayers, if any.
            if (_layer.Layer1 != null)
            {
                foreach (WMS.Capabilities_1_1_1.Layer _sublayer in _layer.Layer1)
                {
                    ServiceLogicCommon.Layer sublayer = parseLayer(_sublayer);

                    // TODO: sublayers add SRS to the parent (for now, we just have one SRS per layer)
                    if (sublayer.SpatialReferenceSystem == null)
                    {
                        sublayer.SpatialReferenceSystem = layer.SpatialReferenceSystem;
                    }

                    // sublayers may have null bounding boxes
                    if (sublayer.BoundingBox == null)
                    {
                        sublayer.BoundingBox = layer.BoundingBox;
                    }

                    layer.addLayer(sublayer);
                }
            }

            return layer;
        }

        public void parseException(string response)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ServiceExceptionReport));
            StringReader stringReader = new StringReader(response);
            ServiceExceptionReport exception = (ServiceExceptionReport)serializer.Deserialize(stringReader);
            string exceptionCode = exception.ServiceException[0].code;
            switch (exceptionCode)
            {
                // the following codes are from Table A.1 in the 1.1.1 spec.
                case "InvalidFormat": // Request contains a Format not offered by the service instance.
                case "InvalidSRS": // Request contains an SRS not offered by the service instance for one or more of the Layers in the request.
                case "LayerNotDefined": // Request is for a Layer not offered by the service instance.
                case "StyleNotDefined": // Request is for a Layer in a Style not offered by the service instance.
                case "LayerNotQueryable": // GetFeatureInfo request is applied to a Layer which is not declared queryable.
                case "CurrentUpdateSequence": // Value of (optional) UpdateSequence parameter in GetCapabilities request is equal to current value of Capabilities XML update sequence number.
                case "InvalidUpdateSequence": // Value of (optional) UpdateSequence parameter in GetCapabilities request is greater than current value of Capabilities XML update sequence number.
                case "MissingDimensionValue": // Request does not include a sample dimension value, and the service instance did not declare a default value for that dimension.
                case "InvalidDimensionValue": // Request contains an invalid sample dimension value.
                default:
                    string message = "WMS Service Exception: " + exceptionCode + " " + exception.ServiceException[0].Value;
                    throw new ArgumentException(message);
            }
        }
    }
}
