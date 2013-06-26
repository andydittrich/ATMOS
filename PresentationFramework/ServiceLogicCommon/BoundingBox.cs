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
    [Serializable()]
    public class BoundingBox : IFormattable
    {
        private double minX;
        private double minY;
        private double maxX;
        private double maxY;

        public BoundingBox(double _minX, double _minY, double _maxX, double _maxY)
        {
            minX = _minX;
            maxX = _maxX;
            minY = _minY;
            maxY = _maxY;
        }

        public BoundingBox(string queryString)
        {
            char[] delimiters = { ',' };
            string[] coordStrings = queryString.Split(delimiters);
            if (coordStrings.Length != 4)
            {
                throw new ArgumentException("unable to parse queryString: \"" + queryString + "\".  Expected 4 comma-delimited values, found " + coordStrings.Length);
            }
            minX = Double.Parse(coordStrings[0]);
            minY = Double.Parse(coordStrings[1]);
            maxX = Double.Parse(coordStrings[2]);
            maxY = Double.Parse(coordStrings[3]);
        }

        public static bool operator ==(BoundingBox lhs, BoundingBox rhs)
        {
            if (System.Object.ReferenceEquals(lhs, rhs))
            {
                return true;
            }

            if (((object)lhs == null) || ((object)rhs == null))
            {
                return false;
            }

            return (lhs.minX == rhs.minX &&
                    lhs.maxX == rhs.maxX &&
                    lhs.minY == rhs.minY &&
                    lhs.maxY == rhs.maxY);
        }

        public static bool operator !=(BoundingBox lhs, BoundingBox rhs)
        {
            return !(lhs == rhs);
        }

        // tests for equality with the passed object.
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                BoundingBox other = (BoundingBox)obj;
                return (this == other);
            }
        }

        // overrides getHashCode, which is required when overriding Equals.
        public override int GetHashCode()
        {
            return minX.GetHashCode() ^
                   maxX.GetHashCode() ^
                   minY.GetHashCode() ^
                   maxY.GetHashCode();
        }

        // tests if the passed bounding box intersects with this one
        public bool intersect(BoundingBox other)
        {
            return (minX < other.maxX &&
                    maxX > other.minX &&
                    minY < other.maxY &&
                    maxX > other.minX);
        }

        public double MinX
        {
            get { return minX; }
        }

        public double MinY
        {
            get { return minY; }
        }

        public double MaxX
        {
            get { return maxX; }
        }

        public double MaxY
        {
            get { return maxY; }
        }

        // tests if the bounding box is empty
        public bool IsEmpty
        {
            get { return (maxX <= minX || maxY <= minY); }
        }

        // returns a string representation of this object.  
        // overridden here to return a string that is useful in getMap queries.
        public override string ToString()
        {
            return minX.ToString() + "," + minY.ToString() + "," + maxX.ToString() + "," + maxY.ToString();
        }

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return minX.ToString(format, formatProvider) + "," +
                   minY.ToString(format, formatProvider) + "," +
                   maxX.ToString(format, formatProvider) + "," +
                   maxY.ToString(format, formatProvider);
        }

        #endregion
    }
}
