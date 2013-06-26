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
using System.Data;

namespace ServiceLogicCommon
{
    public interface ITabularService
    {
        // method to retrieve the available data for a service.  The result is a list of SeriesGroups, 
        // which are logical groupings of series (e.g., data collection sites).  Each group contains 
        // one or more Series which may be requested.
        IEnumerable<SeriesGroup> requestSeriesGroups(Service service);

        // method to retrieve the available data for a series group.  The resutl is a list of Series.
        // This will only be called if the call to requestSeriesGroups returns groups without populating
        // the series list.  This is used for a two-step retrieval of series
        IEnumerable<Series> requestSeries(Service service, SeriesGroup seriesGroup);

        // method to retrieve the data.  The TableRequest parameter should be popul;ated with the 
        // details of the data series, including the internal name of the series group (e.g. site code), 
        // and the internal name of the series (e.g., parameter code).  The result is a DataSet, which 
        // must have at least one column named "Time" with DateTime data type.
        DataTable requestData(Service service, TableRequest tableRequest);
    }
}
