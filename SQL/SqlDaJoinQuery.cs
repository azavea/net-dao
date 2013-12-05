// Copyright (c) 2004-2010 Azavea, Inc.
// 
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;

namespace Azavea.Open.DAO.SQL
{
    /// <summary>
    /// A SQL query that joins two tables, can be run by the SqlDaLayer.
    /// </summary>
    public class SqlDaJoinQuery : SqlDaQuery, IDaJoinQuery, IDaMultiJoinQuery
    {
        private string[] _prefixes;

        /// <summary>
        /// Populates the prefix strings.
        /// </summary>
        /// <param name="prefixes">Prefixes for columns from tables.</param>
        public void SetPrefixes(params string[] prefixes)
        {
            if (prefixes.Length < 2)
            {
                throw new ArgumentException("Must provide at least 2 table prefixes.");
            }
            _prefixes = prefixes;
        }

        /// <summary>
        /// The prefix that should be used to get the left table's columns out of the IDataReader
        /// when accessing them by name.
        /// </summary>
        /// <returns>The prefix for columns in the left table (I.E. "left_table.")</returns>
        public string GetLeftColumnPrefix()
        {
            return _prefixes[0];
        }

        /// <summary>
        /// The prefix that should be used to get the right table's columns out of the IDataReader
        /// when accessing them by name.
        /// </summary>
        /// <returns>The prefix for columns in the right table (I.E. "right_table.")</returns>
        public string GetRightColumnPrefix()
        {
            return _prefixes[1];
        }

        /// <summary>
        /// The full list of prefixes.
        /// </summary>
        /// <returns>An array of column prefixes (i.e. ["table_A.", "table_B."])</returns>
        public string[] GetPrefixes()
        {
            return _prefixes;
        }
    }
}