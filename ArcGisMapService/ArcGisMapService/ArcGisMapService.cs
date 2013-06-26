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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceLogicCommon;
using System.Drawing;
using System.ComponentModel.Composition;
using System.Runtime.Serialization.Json;
using System.Net;

namespace ArcGisMapService
{
    [Export(typeof(IService))]
    [ExportMetadata("Type", "ArcGISMapService")]
    [ExportMetadata("HelpText", ArcGisMapService.helpText)]
    public class ArcGisMapService : IService, IMapService
    {
        public const string helpText = "ArcGISMapService provides access to maps served through the ArcGIS server MapService.  Provide a url to either the catalog or the map service directly.  If no title is provided, the map name will be used.  The version will be set from the service metadata, and should not be modified by the user.";
        #region IMapServiceImplementation
        // method to request a map image
        public Bitmap requestMapImage(Service service, LayerRequest layerRequest)
        {
            Bitmap tileImage = null;

            // There are two (or more) methods to request map images from a map service.
            // One method is to request a tile using the x, y, and lod parameters.
            // The other method is to request an image using a bounding box.
            // try requesting a tile first
            tileImage = requestMapImageByTile(service, layerRequest);
            if (tileImage == null)
            {
                tileImage = requestMapImageByBoundingBox(service, layerRequest);
            }

            return tileImage;
        }

        // method to get available layers from a map service
        public IEnumerable<ServiceLogicCommon.Layer> requestLayers(Service service)
        {
            // TODO: this could be converted to query the layers for detailed information.  This 
            // can be done by setting the relative path to 'layers' and creating a LayersResponse
            // class that defines the data contract for the response.  For now, just the names and
            // IDs of the layers is sufficient
            Request request = new Request(service.Url);
            request.Format = Request.JsonFormat;
            WebRequest webRequest = WebRequest.Create(request.Url);
            WebResponse webResponse = webRequest.GetResponse();
            // TODO: ensure the response is Json format (mime type application/json).  Throw an exception if not.
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(CapabilitiesResponse));
            object objResponse = jsonSerializer.ReadObject(webResponse.GetResponseStream());
            CapabilitiesResponse capabilitiesResponse = objResponse as CapabilitiesResponse;

            List<ServiceLogicCommon.Layer> layers = new List<ServiceLogicCommon.Layer>();
            foreach (Layer layer in capabilitiesResponse.Layers)
            {
                ServiceLogicCommon.Layer serviceLayer = new ServiceLogicCommon.Layer();
                serviceLayer.BoundingBox = null;
                serviceLayer.Name = layer.Id.ToString();
                serviceLayer.SpatialReferenceSystem = null;
                serviceLayer.Title = layer.Name;
                // TODO: each layer specifies a parent layer ID, which could be used to organize 
                // these layers hierarchically.  For now, just return a flat list.
                layers.Add(serviceLayer);
            }

            return layers;
        }

        // method to retrieve information about this service
        public Service requestServiceInformation(Service service)
        {
            Request request = new Request(service.Url);
            request.Format = Request.JsonFormat;
            WebRequest webRequest = WebRequest.Create(request.Url);
            WebResponse webResponse = webRequest.GetResponse();
            // TODO: ensure the response is Json format (mime type application/json).  If not, throw an exception.
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(CapabilitiesResponse));
            object objResponse = jsonSerializer.ReadObject(webResponse.GetResponseStream());
            CapabilitiesResponse capabilitiesResponse = objResponse as CapabilitiesResponse;

            service.Name = capabilitiesResponse.MapName;
            if (service.Title == null)
            {
                service.Title = capabilitiesResponse.MapName; // TODO: this could be retrieved from the DocumentInfo
            }
            service.Version = capabilitiesResponse.CurrentVersion;

            // don't include the format on the stored URL.
            request.Format = null;
            service.Url = request.Url.ToString ();
            service.MaxHeight = capabilitiesResponse.MaxImageHeight;
            service.MaxWidth = capabilitiesResponse.MaxImageWidth;

            return service;
        }
        #endregion

        protected Bitmap requestMapImageByBoundingBox(Service service, LayerRequest layerRequest)
        {
            ExportMapRequest request = new ExportMapRequest(service.Url);
            request.RelativePath = "export";
            request.Format = Request.ImageFormat;
            request.BoundingBox = layerRequest.BoundingBox;
            request.BoundingBoxSpatialReference = "4326";
            request.MapImageFormat = "png";  // TODO: make sure this is in the list of supported types
            request.Size = new Point(layerRequest.Width, layerRequest.Height);
            request.Transparent = true;
            request.addLayer(layerRequest.Layer);

            WebRequest webRequest = WebRequest.Create(request.Url);
            WebResponse webResponse = null;
            try
            {
                webResponse = webRequest.GetResponse();
            }
            catch (WebException we)
            {
                // TODO: log the error
                return null;
            }
            // TODO: ensure the response is an image format
            return new Bitmap(Image.FromStream(webResponse.GetResponseStream()));
        }

        protected Bitmap requestMapImageByTile(Service service, LayerRequest layerRequest)
        {
            // tiles are requested with: http://<mapservice-url>/tile/<level>/<row>/<column>
            Request request = new Request(service.Url);
            // TODO: determine how the layer can be specified
            request.RelativePath = "tile/" + layerRequest.LevelOfDetail + "/" + layerRequest.TileY + "/" + layerRequest.TileX;
            WebRequest webRequest = WebRequest.Create(request.Url);
            WebResponse webResponse = null;
            try
            {
                webResponse = webRequest.GetResponse();
            }
            catch (WebException we)
            {
                // TODO: log the error
                return null;
            }
            // TODO: ensure the response is an image format
            return new Bitmap(Image.FromStream(webResponse.GetResponseStream()));
        }
    }
}
