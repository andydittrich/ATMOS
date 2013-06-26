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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace ArcGisMapService
{
    [DataContract]
    public class CapabilitiesResponse
    {
        //Added at 10.0 SP1
        [DataMember(Name="currentVersion")]
        public string CurrentVersion { get; set; }

        [DataMember (Name="serviceDescription")]
        public string ServiceDescription { get; set; }
        
        [DataMember (Name="mapName")]
        public string MapName { get; set; }
        
        [DataMember (Name="description")]
        public string Description { get; set; }
        
        [DataMember (Name="copyrightText")]
        public string CopyrightText { get; set; }
        
        [DataMember (Name="supportsDynamicLayers")]
        public bool SupportsDynamicLayers { get; set; }

        [DataMember (Name="layers")]
        public Layer[] Layers { get; set; }

        //the tables published by this service - from 10 onward
        [DataMember (Name="tables")]
        public Table[] Tables { get; set; }

        [DataMember (Name="spatialReference")]
        public SpatialReference SpatialReference { get; set; }

        [DataMember (Name="singleFusedMapCache")]
        public bool SingleFusedMapCache { get; set; }

        [DataMember (Name="tileInfo")]
        public TileInfo TileInfo { get; set; }

        [DataMember (Name="initialExtent")]
        public Envelope InitialExtent { get; set; }

        [DataMember(Name = "fullExtent")]
        public Envelope FullExtent { get; set; }

        // from 10 onward - if the map supports querying and exporting maps based on time
        // TODO: complete support for timeInfo
        //[DataMember(Name = "timeInfo")]
        //public TimeInfo TimeInfo { get; set; }

        [DataMember (Name="units")]
        public string Units { get; set; }

        [DataMember (Name="supportedImageFormatTypes")]
        public string SupportedImageFormatTypes { get; set; }

        [DataMember (Name="documentInfo")]
        public Dictionary<string, string> DocumentInfo { get; set; }

        [DataMember (Name="capabilities")]
        public string Capabilities { get; set; }

        [DataMember (Name="maxRecordCount")]
        public int MaxRecordCount { get; set; }

        [DataMember (Name="maxImageHeight")]
        public int MaxImageHeight { get; set; }

        [DataMember (Name="maxImageWidth")]
        public int MaxImageWidth { get; set; }

        [DataMember (Name="minScale")]
        public int MinScale { get; set; }

        [DataMember (Name="maxScale")]
        public int MaxScale { get; set; }

        //Added at 10.1
        [DataMember(Name = "tileServers")]
        public string[] TileServers { get; set; }

        [DataMember(Name = "supportedQueryFormats")]
        public string SupportedQueryFormats { get; set; }
    }

    //the spatial layers published by this service
    [DataContract]
    public class Layer
    {
        [DataMember (Name="id")]
        public int Id { get; set; }

        [DataMember (Name="name")]
        public string Name { get; set; }

        [DataMember (Name="defaultVisibility")]
        public bool DefaultVisibility { get; set; }

        [DataMember (Name="parentLayerId")]
        public int ParentLayerId { get; set; }

        [DataMember (Name="subLayerIds")]
        public int[] SubLayerIds { get; set; }

        [DataMember (Name="minScale")]
        public double MinScale { get; set; }

        [DataMember (Name="maxScale")]
        public double MaxScale { get; set; }
    }

    [DataContract]
    public class Table
    {
        [DataMember (Name="id")]
        public int Id { get; set; }

        [DataMember (Name="name")]
        public string Name { get; set; }
    }

    [DataContract]
    public class TileInfo
    {
        [DataMember (Name="rows")]
        public int Rows { get; set; }

        [DataMember (Name="cols")]
        public int Cols { get; set; }

        [DataMember (Name="dpi")]
        public int Dpi { get; set; }

        [DataMember (Name="format")]
        public string Format { get; set; }

        [DataMember (Name="compressionQuality")]
        public int CompressionQuality { get; set; }

        [DataMember (Name="origin")]
        public Point Origin { get; set; }

        [DataMember (Name="spatialReference")]
        public SpatialReference SpatialReference { get; set; }

        [DataMember (Name="lods")]
        public LevelOfDetail[] Lods { get; set; }
    }

    [DataContract]
    public class SpatialReference
    {
        [DataMember (Name="wkid")]
        public int Wkid { get; set; }
    }

    [DataContract]
    public class Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point(string pointString)
        {
            string[] components = pointString.Split(',', ' ');
            X = Int32.Parse(components.FirstOrDefault());
            Y = Int32.Parse(components.LastOrDefault());
        }

        public override string ToString()
        {
            return X.ToString() + "," + Y.ToString();
        }

        [DataMember (Name="x")]
        public int X { get; set; }

        [DataMember (Name="y")]
        public int Y { get; set; }
    }

    [DataContract]
    public class LevelOfDetail
    {
        [DataMember (Name="level")]
        public int Level { get; set; }

        [DataMember (Name="resolution")]
        public double Resolution { get; set; }

        [DataMember (Name="scale")]
        public int Scale { get; set; }
    }

    [DataContract]
    public class Envelope
    {
        [DataMember(Name = "xmin")]
        public double Xmin { get; set; }

        [DataMember(Name = "xmax")]
        public double Xmax { get; set; }

        [DataMember(Name = "ymin")]
        public double Ymin { get; set; }

        [DataMember(Name = "ymax")]
        public double Ymax { get; set; }

        [DataMember(Name = "spatialReference")]
        public SpatialReference SpatialReference { get; set; }
    }

    [DataContract]
    public class TimeInfo
    {
        // TODO: implement this class
/*              "timeExtent": [<startTime>, <endTime>],
              "timeReference": {
                "timeZone": "<timeZone>",
                "respectsDaylightSaving": <true | false>
              },
              "timeRelation": "<esriTimeRelationOverlaps | esriTimeRelationOverlapsStartWithinEnd | esriTimeRelationAfterStartOverlapsEnd>",
              "defaultTimeInterval" : <time interval>,
              "defaultTimeIntervalUnits" : "<time interval units>",
              "defaultTimeInterval" : <time interval>,
              "defaultTimeWindow" : <time window>,
              "hasLiveData" : <true | false>,
            },
            }*/
    }
}
