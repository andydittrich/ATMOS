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

namespace WMS.Common
{
    public class GetMapRequest : Request
    {
        public static readonly string getMapRequest = "GetMap";

        protected static readonly string heightKey = "HEIGHT";
        protected static readonly string widthKey = "WIDTH";
        protected static readonly string formatKey = "FORMAT";
        protected static readonly string layersKey = "LAYERS";
        protected static readonly string stylesKey = "STYLES";
        protected static readonly string boundingBoxKey = "BBOX";
        protected static readonly string transparentKey = "TRANSPARENT";

        // the spatial reference system is "SRS" for version 1.1.1, and "CRS" for version 1.3.0.  
        // This needs to be handled differently.  Other attributes may also need to be updated to 
        // use this method if they vary across versions of the service.
        protected readonly Dictionary<string, string> spatialReferenceSystemKeys = new Dictionary<string,string> () 
        {
            { "1.1.1", "SRS" },
            { "1.3.0", "CRS" },
        };

        public GetMapRequest(string url)
            : base(url)
        {
            RequestType = getMapRequest;
        }

        public GetMapRequest(string url, string version)
            : base(url, version)
        {
            RequestType = getMapRequest;
        }

        public string[] Layers
        {
            get
            {
                string layerString = getQueryString(layersKey);
                if (layerString != null)
                {
                    char[] delimiters = { ',', ' ' };
                    string[] layers = layerString.Split(delimiters);
                    return layers;
                }
                else
                {
                    return null;
                }
            }
        }

        public void addLayer(string layer)
        {
            string layerString = getQueryString(layersKey);
            if (layerString != null)
            {
                layerString += ",";
            }
            layerString += layer;
            setQueryString(layersKey, layerString);
        }

        // styles: comma separated list of styles for each layer
        public string[] Styles
        {
            get
            {
                string styleString = getQueryString(stylesKey);
                if (styleString != null)
                {
                    char[] delimiters = { ',', ' ' };
                    string[] styles = styleString.Split(delimiters);
                    return styles;
                }
                else
                {
                    return null;
                }
            }
        }

        public void addStyle(string style)
        {
            string styleString = getQueryString(stylesKey);
            if (styleString != null)
            {
                styleString += ",";
            }
            styleString += style;
            setQueryString(stylesKey, styleString);
        }

        public string SpatialReferenceSystem
        {
            get { return getQueryString(spatialReferenceSystemKeys[Version]); }
            set { setQueryString(spatialReferenceSystemKeys[Version], value); }
        }

        public BoundingBox BoundingBox
        {
            get
            {
                string boundingBoxString = getQueryString(boundingBoxKey);
                if (boundingBoxString != null)
                {
                    return new BoundingBox(boundingBoxString);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                string boundingBoxString = value.ToString();
                setQueryString(boundingBoxKey, boundingBoxString);
            }
        }

        public int Height
        {
            get { return Int32.Parse (getQueryString(heightKey)); }
            set { setQueryString(heightKey, value.ToString ()); }
        }

        public int Width
        {
            get { return Int32.Parse (getQueryString(widthKey)); }
            set { setQueryString(widthKey, value.ToString ()); }
        }

        public string Format
        {
            get { return getQueryString(formatKey); }
            set { setQueryString(formatKey, value); }
        }

        public bool Transparent
        {
            get 
            {
                bool transparent = false;
                try
                {
                    bool.TryParse(getQueryString(transparentKey), out transparent);
                }
                catch (Exception e)
                {
                    // TODO: improve handling of this exception
                }
                return transparent; 
            }
            set { setQueryString(transparentKey, value.ToString()); }
        }
    }
}
