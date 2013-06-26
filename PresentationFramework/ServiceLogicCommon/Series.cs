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
    public class Series
    {
        [Serializable]
        public class DateTimeRange
        {
            public DateTimeOffset Start { get; set; }
            public DateTimeOffset End { get; set; }
        }

        public string Name { get; set; }
        public string InternalName { get; set; }
        public string Units { get; set; }
        public DateTimeRange DateRange { get; set; }
        public int SampleCount { get; set; }
    }
}
