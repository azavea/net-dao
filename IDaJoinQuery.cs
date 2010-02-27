﻿// Copyright (c) 2004-2010 Azavea, Inc.
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

namespace Azavea.Open.DAO
{
    /// <summary>
    /// This is an interface that defines a join query that can be run by an IDaJoinableLayer.
    /// Since the layer defines what (if any) table aliases are used, it needs a way to
    /// communicate what (if anything) the columns in the data reader will be prefixed with.
    /// </summary>
    public interface IDaJoinQuery : IDaQuery
    {
        /// <summary>
        /// The prefix that should be used to get the left table's columns out of the IDataReader
        /// when accessing them by name.
        /// </summary>
        /// <returns>The prefix for columns in the left table (I.E. "left_table.")</returns>
        string GetLeftColumnPrefix();
        /// <summary>
        /// The prefix that should be used to get the right table's columns out of the IDataReader
        /// when accessing them by name.
        /// </summary>
        /// <returns>The prefix for columns in the right table (I.E. "right_table.")</returns>
        string GetRightColumnPrefix();
    }
}