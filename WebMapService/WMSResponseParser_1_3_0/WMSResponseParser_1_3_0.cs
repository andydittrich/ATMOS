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
using WMS.Common;
using WMS.Capabilities_1_3_0;
using WMS.Exceptions_1_3_0;
using System.IO;
using System.Xml.Serialization;
using ServiceLogicCommon;

namespace WMS.ResponseParser_1_3_0
{
    public class ResponseParser_1_3_0 : IResponseParser
    {
        public GetCapabilitiesResponse parseResponse(string response)
        {
            // use the XML serializer to deserialize the xml response string into strongly typed classes based on the schema.
            StringReader responseReader = new StringReader(response);
            XmlSerializer serializer = new XmlSerializer(typeof(WMS_Capabilities));
            WMS_Capabilities capabilities_1_3_0 = (WMS_Capabilities)serializer.Deserialize(responseReader);

            GetCapabilitiesResponse getCapabilitiesResponse = new GetCapabilitiesResponse(capabilities_1_3_0.version);
            getCapabilitiesResponse.GetMapUrl = capabilities_1_3_0.Capability.Request.GetMap.DCPType[0].HTTP.Get.OnlineResource.href;
            getCapabilitiesResponse.MapFormats = capabilities_1_3_0.Capability.Request.GetMap.Format;

            // parse the service information
            getCapabilitiesResponse.Service = parseService(capabilities_1_3_0.Service);

            // parse the layers
            getCapabilitiesResponse.Layer = parseLayer(capabilities_1_3_0.Capability.Layer);

            return getCapabilitiesResponse;
        }

        protected ServiceLogicCommon.Service parseService(WMS.Capabilities_1_3_0.Service _service)
        {
            ServiceLogicCommon.Service service = new ServiceLogicCommon.Service();
            service.Name = _service.Name.ToString ();
            service.Title = _service.Title;
            service.Url = _service.OnlineResource.href;
            service.MaxWidth = Int32.Parse (_service.MaxWidth);
            service.MaxHeight = Int32.Parse (_service.MaxHeight);
            return service;
        }

        protected ServiceLogicCommon.Layer parseLayer(WMS.Capabilities_1_3_0.Layer _layer)
        {
            ServiceLogicCommon.Layer layer = new ServiceLogicCommon.Layer();
            layer.Name = _layer.Name;
            layer.Title = _layer.Title;

            if (_layer.CRS != null && _layer.CRS.Length > 0)
            {
                layer.SpatialReferenceSystem = _layer.CRS[0];
            }

            if (_layer.EX_GeographicBoundingBox != null)
            {
                layer.BoundingBox = new ServiceLogicCommon.BoundingBox(_layer.EX_GeographicBoundingBox.westBoundLongitude,
                                                                       _layer.EX_GeographicBoundingBox.southBoundLatitude,
                                                                       _layer.EX_GeographicBoundingBox.eastBoundLongitude,
                                                                       _layer.EX_GeographicBoundingBox.northBoundLatitude);
            }

            if (_layer.Layer1 != null)
            {
                foreach (WMS.Capabilities_1_3_0.Layer _sublayer in _layer.Layer1)
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
            // use the XML serializer to deserialize the xml response string into strongly typed classes based on the schema.
            StringReader responseReader = new StringReader(response);
            XmlSerializer serializer = new XmlSerializer(typeof(ServiceExceptionReport));
            ServiceExceptionReport exception_1_3_0 = (ServiceExceptionReport)serializer.Deserialize(responseReader);
            string exceptionCode = exception_1_3_0.ServiceException[0].code;
            switch (exceptionCode)
            {
                // cases in this switch statement are from table E.1 in the 1.3.0 spec.
                case "InvalidFormat": // Request contains a Format not offered by the server.
                case "InvalidCRS": // Request contains a CRS not offered by the server for one or more of the Layers in the request.
                case "LayerNotDefined": // GetMap request is for a Layer not offered by the server, or GetFeatureInfo request is for a Layer not shown on the map.
                case "StyleNotDefined": // Request is for a Layer in a Style not offered by the server.
                case "LayerNotQueryable": // GetFeatureInfo request is applied to a Layer which is not declared queryable.
                case "InvalidPoint": // GetFeatureInfo request contains invalid I or J value.
                case "CurrentUpdateSequence": // Value of (optional) UpdateSequence parameter in GetCapabilities request is equal to current value of service metadata update sequence number.
                case "InvalidUpdateSequence": // Value of (optional) UpdateSequence parameter in GetCapabilities request is greater than current value of service metadata update sequence number.
                case "MissingDimensionValue": // Request does not include a sample dimension value, and the server did not declare a default value for that dimension.
                case "InvalidDimensionValue": // Request contains an invalid sample dimension value.
                case "OperationNotSupported": // Request is for an optional operation that is not supported by the server.
                default:
                    string message = "WebMapService exception: " + exceptionCode + " " + exception_1_3_0.ServiceException[0].Value;
                    throw new ArgumentException(message);
            }
        }
    }
}
