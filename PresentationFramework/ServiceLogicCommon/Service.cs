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
    public class Service
    {
        // TODO: use name and internal name instead of title

        public Service() { }

        public Service(string title, string url, string type, string version)
        {
            Title = title;
            Url = url;
            Type = type;
            Version = version;
        }

        public string Name { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
        public string Version { get; set; }
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }
    }
}
