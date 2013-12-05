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

using System.Collections.Generic;
using Azavea.Open.DAO.Criteria.Joins;
using Azavea.Open.DAO.Criteria.Joins.MultiJoins;

namespace Azavea.Open.DAO
{
    /// <summary>
    /// If the layer supports joins natively (such as SQL statements in the database)
    /// it will implement this interface.
    /// 
    /// NOTE on transactions: The methods on the interface accept transactions,
    /// but if your data source does not support transactions (or you have not
    /// yet implemented support), you may ignore that parameter as long as your
    /// equivalent IConnectionDescriptor is not an ITransactionalConnectionDescriptor.
    /// </summary>
    public interface IDaMultiJoinableLayer : IDaLayer
    {
        /// <summary>
        /// This is not guaranteed to succeed unless CanJoin(rightConn, rightMapping) returns true
        /// for each DAO which was joined.
        /// </summary>
        /// <param name="joinInfos">Objects which contain the classmapping and criteria for each
        ///                         DAO we are joining.</param>
        /// <param name="orders">The a list of sort criteria, applied in the order they are given in</param>
        IDaJoinQuery CreateJoinQuery(IList<JoinInfo> joinInfos, IList<MultiJoinSortOrder> orders);

        /// <summary>
        /// This gets a count instead of an actual data retrieval.
        /// Depending on the data access layer implementation, this may or may not be significantly
        /// faster than actually executing the normal query and seeing how many results you get back.
        /// Generally it should be faster.
        /// </summary>
        /// <param name="joinInfos">Objects which contain the classmapping and criteria for each
        ///                         DAO we are joining.</param>
        /// <param name="transaction">The transaction to run the query in, can be null</param>
        /// <returns>The number of results that you would get if you ran the actual query.</returns>
        int GetCount(IList<JoinInfo> joinInfos, ITransaction transaction = null);
    }
}