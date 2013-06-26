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
using System.ComponentModel.Composition;
using DataProcessorCommon;
using System.Data;

namespace TimeDomainDataCombiners
{
    [Export(typeof(ITableCombiner))]
    [ExportMetadata("Method", "Simple Time Domain")]
    public class SimpleTimeDomainCombiner : ITableCombiner
    {
        #region ITableCombiner Members

        public DataTable combine(DataTable lhs, DataTable rhs)
        {
            DataTable tempDataTable = lhs.Clone();

            // combine the columns
            foreach (DataColumn column in rhs.Columns)
            {
                if (tempDataTable.Columns.Contains(column.ColumnName) == false)
                {
                    tempDataTable.Columns.Add(column.ColumnName, column.DataType);
                }
            }

            // combine each row
            foreach (DataRow row in rhs.Rows)
            {
                // find an existing row with the same time stamp as this new row
                DataRow matchingRow = (from DataRow dataRow in lhs.Rows
                                       where dataRow.Field<DateTime>("Time") == row.Field<DateTime>("Time")
                                       select dataRow).FirstOrDefault();
                DataRow combinedRow = tempDataTable.NewRow();
                foreach (DataColumn column in tempDataTable.Columns)
                {
                    if (matchingRow != null && matchingRow.Table.Columns.Contains(column.ColumnName))
                    {
                        // if both the new row and the existing row have the same data, use the existing data
                        combinedRow[column.ColumnName] = matchingRow[column.ColumnName];
                    }
                    else if (row.Table.Columns.Contains(column.ColumnName))
                    {
                        // if there is no matching row, or the existing row doesn't have this data, use the new data
                        combinedRow[column.ColumnName] = row[column.ColumnName];
                    }
                    else
                    {
                        // TODO: print a warning that duplicate data was found
                    }
                }
                tempDataTable.Rows.Add(combinedRow);
            }

            return tempDataTable;
        }

        #endregion
    }
}
