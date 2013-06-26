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
using System.Web;
using System.Drawing;
using System.IO;
using ServicesBusinessLogic;
using ServicesDataAccess;
using System.Net;
using ServiceLogicCommon;
using ServiceLogic;

namespace BingMaps
{
    public class BingMapTileHandler : IHttpHandler
    {
        /// <summary>
        /// You will need to configure this handler in the web.config file of your 
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        // Processes an HTTP get request for a map tile.  This is used by the Bing Maps AJAX control to request dynamic map layers.  This handler supports the following parameters:
        //  x - the x coordinate for the tile
        //  y - the y coordinate for the tile
        //  lod - the level of detail for the map
        //  layer - the name of the layer
        //  serviceID - the ID of the service to contact
        // returned images will always be 256x256 pixels
        public void ProcessRequest(HttpContext context)
        {
            string desired_reference_system = "EPSG:4326";
            string desired_content_type = "image/png";

            // Fetch URL-parameters
            int tileX = Int32.Parse (context.Request.Params.Get("x"));
            int tileY = Int32.Parse(context.Request.Params.Get("y"));
            int lod = Int32.Parse (context.Request.Params.Get("lod"));
            string layer = context.Request.Params.Get("layer");
            int serviceID = Int32.Parse(context.Request.Params.Get("serviceID"));

            // determine the pixel ranges for the requested tile
            int pixelXMin = 0;
            int pixelXMax = 0;
            int pixelYMin = 0;
            int pixelYMax = 0;
            TileSystem.TileXYToPixelXY(tileX, tileY, out pixelXMin, out pixelYMin);
            TileSystem.TileXYToPixelXY(tileX + 1, tileY + 1, out pixelXMax, out pixelYMax);
            pixelXMax -= 1;
            pixelYMax -= 1;

            // determine the latitude and longitude ranges of the requested tile
            double latMin = 0;
            double latMax = 0;
            double longMin = 0;
            double longMax = 0;
            TileSystem.PixelXYToLatLong(pixelXMin, pixelYMin, lod, out latMax, out longMin);
            TileSystem.PixelXYToLatLong(pixelXMax, pixelYMax, lod, out latMin, out longMax);
            BoundingBox boundingBox = new BoundingBox(longMin, latMin, longMax, latMax);

            // retrieve the service information
            ServicesBusinessLogic.ServicesBusinessLogic servicesLogic = new ServicesBusinessLogic.ServicesBusinessLogic();
            ServicesDataSet.ServicesDataTable servicesTable = servicesLogic.GetServiceById(serviceID);
            ServicesDataSet.ServicesRow serviceRow = servicesTable[0];
            Service service = new Service(serviceRow.Title, serviceRow.Url, serviceRow.Type, serviceRow.Version);

            // construct the request
            LayerRequest request = new LayerRequest();
            request.SpatialReferenceSystem = desired_reference_system;
            request.Layer = layer;
            request.BoundingBox = boundingBox;
            request.Format = desired_content_type;
            request.Height = TileSystem.TileSize;
            request.Width = TileSystem.TileSize;
            request.TileX = tileX;
            request.TileY = tileY;
            request.LevelOfDetail = lod;

            // execute the request
            Services services = new Services();
            var mapService = services.getService(service.Type) as IMapService;
            Bitmap tileImage = null;
            try
            {
                tileImage = mapService.requestMapImage(service, request);
            }
            catch (WebException we)
            {
                // TODO: handle this differently.  It would be nice for this module to not have to 
                // know about web exceptions or even that the request is going over the web.
                // TODO: log an error message
            }
            catch (ArgumentException ae)
            {
                // no service with the specified type is supported
                // TODO: log an error message
            }
            catch (FormatException fe)
            {
                // TODO: log an error message
            }
            catch (NullReferenceException nre)
            {
                // no map service for this type could be found.
                // TODO: log an error message
            }
            finally
            {
                if (tileImage == null)
                {
                    // something went wrong with the request.  respond with an empty image
                    tileImage = new Bitmap(TileSystem.TileSize, TileSystem.TileSize);
                }
            }

            // write the image to the response stream
            MemoryStream writeStream = new MemoryStream();
            tileImage.Save(writeStream, System.Drawing.Imaging.ImageFormat.Png);
            writeStream.WriteTo(context.Response.OutputStream);
            context.Response.ContentType = desired_content_type;
            tileImage.Dispose();
        }

        #endregion
    }
}
