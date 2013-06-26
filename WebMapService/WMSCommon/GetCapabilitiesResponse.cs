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
using System.Xml.Serialization;
using ServiceLogicCommon;

namespace WMS.Common
{
    // the Capabilities class is a version-agnostic representation of a WMS getCapabilities response.  
    // It contains only the information needed to validate responses and construct getMap requests.
    public class GetCapabilitiesResponse
    {
        public GetCapabilitiesResponse(string version)
        {
            Version = version;
        }

        public string Version { get; protected set; }
        public Service Service { get; set; }
        public string[] MapFormats { get; set; }
        public string GetMapUrl { get; set; }
        public Layer Layer { get; set; }
    }
}
