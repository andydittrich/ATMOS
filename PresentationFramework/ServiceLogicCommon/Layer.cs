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

namespace ServiceLogicCommon
{
    [Serializable()]
    public class Layer
    {
        public Layer()
        {
            layers = new List<Layer>();
        }

        // the layer's internal name.  This is the name used to request layers from the service, and may be different than the title.
        public string Name { get; set; }

        // The layer's title.  This is the human readable title that is displayed to users.
        public string Title { get; set; }

        // a list of sublayers
        [NonSerialized()]
        private List <Layer> layers;
        public Layer[] Layers
        {
            get { return layers.ToArray(); }
        }

        // adds a layer to the list of sublayers
        public void addLayer(Layer layer)
        {
            layers.Add(layer);
        }

        // the spatial reference system for this layer.
        // TODO: for now we just support 1, this should be updated to support a list of SRS
        public string SpatialReferenceSystem { get; set; }

        // the latitude longitude bounding box for the layer.
        public BoundingBox BoundingBox { get; set; }

        // a flag indicating whether this layer can be requested with a getMap request
        public bool Requestable
        {
            // TODO: this logic should be in the service
            get { return Name != null && Title != null; }
        }

        // TODO: styles
        // TODO: attribution
        // TODO: metadata
    }
}
