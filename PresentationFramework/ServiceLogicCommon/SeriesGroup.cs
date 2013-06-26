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
    [Serializable]
    public class SeriesGroup
    {
        public SeriesGroup()
        {
            _series = new List<Series>();
        }

        public string Name { get; set; }
        public string InternalName { get; set; }

        [NonSerialized]
        private List<Series> _series;
        public List<Series> SeriesList
        {
            get { return _series; }
            set { _series = value; }
        }
    }
}
