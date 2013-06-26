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

namespace ServiceLogicCommon
{
    public class LayerRequest
    {
        public string Format { get; set; }
        public string Layer { get; set; }
        public string SpatialReferenceSystem { get; set; }
        public BoundingBox BoundingBox { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int TileX { get; set; }
        public int TileY { get; set; }
        public int LevelOfDetail { get; set; }
    }
}
