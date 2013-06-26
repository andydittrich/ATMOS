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
using System.Data;
using System.Text.RegularExpressions;

namespace UsgsWaterData
{
    public class RdbParser
    {
        protected Rdb _rdb;
        public Rdb Rdb { get { return _rdb; } }
        protected bool ParsedColumnHeaders { get; set; }
        protected bool ParsedDataDefinitions { get; set; }

        public RdbParser ()
        {
            _rdb = new Rdb ();
            ParsedColumnHeaders = false;
            ParsedDataDefinitions = false;
        }

        public void parse(Stream source)
        {
            StreamReader streamReader = new StreamReader(source);
            string line = streamReader.ReadLine();
            while (line != null)
            {
                line = line.Trim();
                // if this line starts with a '#' character, it is a comment
                if (line.StartsWith("#"))
                {
                    parseComment (line);
                }
                else
                {
                    // the first non-comment line contains the column headers.  If we haven't already defined the columns, do so now.
                    if (ParsedColumnHeaders == false)
                    {
                        // this should be the column header row
                        parseColumnHeaders (line);
                        ParsedColumnHeaders = true;
                    }
                    else if (ParsedDataDefinitions == false)
                    {
                        // this should be the data definitions row
                        parseDataDefinitions(line);
                        ParsedDataDefinitions = true;
                    }
                    else
                    {
                        // this is a data row
                        parseDataRow (line);
                    }
                }

                // read the next line
                line = streamReader.ReadLine();
            }
        }

        protected void parseComment (string line)
        {
            // this is a comment line.  
            // TODO: check if this comment contains the retrieval date
            // TODO: check if this comment contains the contact info
            // TODO: check if this comment contains a column description
        }

        protected void parseColumnHeaders (string line)
        {
            // this should be the header row.
            foreach (string column in line.Split ('\t'))
            {
                _rdb.Data.Columns.Add (column);
            }
        }

        protected void parseDataDefinitions(string line)
        {
            // this should be the row describing the width and type of data in each column
            string[] dataDefinitions = line.Split('\t');
            Regex re = new Regex("(\\d+)(\\D*)");
            for (int i = 0; i < _rdb.Data.Columns.Count; ++i)
            {
                Match match = re.Match(dataDefinitions[i]);
                if (match.Success)
                {
                    // set the data type
                    string dataType = match.Groups[2].ToString();
                    switch (dataType)
                    {
                        case "S":
                        case "s":
                            _rdb.Data.Columns[i].MaxLength = Int32.Parse(match.Groups[1].ToString());
                            _rdb.Data.Columns[i].DataType = typeof(string);
                            break;

                        case "N":
                        case "n":
                            // TODO: change this to an integer type 
                            // Note, we don't set the max length here because there were many, many cases of the data exceeding this length.
                            //_rdb.Data.Columns[i].MaxLength = Int32.Parse(match.Groups[1].ToString());
                            _rdb.Data.Columns[i].DataType = typeof(string);
                            break;

                        case "D":
                        case "d":
                            // date.  This will be represented as a string
                            // TODO: consider using DateTime
                            _rdb.Data.Columns[i].MaxLength = Int32.Parse(match.Groups[1].ToString());
                            _rdb.Data.Columns[i].DataType = typeof(string);
                            break;

                        default:
                            StringBuilder message = new StringBuilder("unknown type: ");
                            message.Append(dataType);
                            throw new NotImplementedException(message.ToString());
                    }
                }
            }
        }

        protected void parseDataRow(string line)
        {
            // TODO: do some basic checking for the number of elements per row and any data type conversions necessary.  For now, just ignore any exceptions that occur while adding data.
            try
            {
                _rdb.Data.Rows.Add(line.Split('\t'));
            }
            catch (ArgumentException aex)
            {

            }
        }
    }
}
