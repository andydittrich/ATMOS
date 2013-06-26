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
    class ExportMapRequest : Request
    {
        protected static readonly string boundingBoxKey = "bbox";
        protected static readonly string sizeKey = "size";
        protected static readonly string dpiKey = "dpi";
        protected static readonly string imageSpatialReferenceKey = "imageSR";
        protected static readonly string boundingBoxSpatialReferenceKey = "bboxSR";
        protected static readonly string mapImageFormatKey = "format";
        protected static readonly string layerDefsKey = "layerDefs";
        protected static readonly string layersKey = "layers";
        protected static readonly string transparentKey = "transparent";
        protected static readonly string timeKey = "time";
        protected static readonly string layerTimeOptionsKey = "layerTimeOptions";

        public ExportMapRequest(string url)
            : base(url)
        {
        }

        public void addLayer(string layer)
        {
            string layerString = getQueryString(layersKey);
            // check if this is the first layer.  If so, add 'show:' to indicate we only want to show this one.
            if (layerString == null)
            {
                layerString = "show:";
            }
            else
            {
                // TODO: this assumes that if the string is not null, it is "show:layer1".  This could be made more robust by parsing the current string with a regular expression to make sure this assumption is correct.
                layerString += ",";
            }
            layerString += layer;
            setQueryString(layersKey, layerString);
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

        public string BoundingBoxSpatialReference
        {
            get { return getQueryString(boundingBoxSpatialReferenceKey); }
            set { setQueryString(boundingBoxSpatialReferenceKey, value); }
        }

        public Point Size
        {
            get
            {
                Point size = null;
                string sizeString = getQueryString(sizeKey);
                if (sizeString != null)
                {
                    size = new Point(sizeString);
                }
                return size;
            }
            set
            {
                string sizeString = value.ToString();
                setQueryString(sizeKey, sizeString);
            }
        }

        public string MapImageFormat
        {
            get { return getQueryString(mapImageFormatKey); }
            set { setQueryString(mapImageFormatKey, value); }
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
