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
using Azavea.Open.DAO.Criteria;
using Azavea.Open.DAO.Criteria.Joins;
using Azavea.Open.DAO.Criteria.Joins.MultiJoins;

namespace Azavea.Open.DAO
{
    /// <summary>
    /// This interface defines the "joining" methods of FastDAO, used to join more than 2 DAOs.
    /// For joining only 2 DAOs, use the simpler Get methods on IFastDaoReader which support joining to a single other DAO
    /// </summary>
    /// <typeparam name="T">The type of object that can be written.</typeparam>
    public interface IFastDaoNextJoiner<T> where T : class, new()
    {
        /// <summary>
        /// Joins this DAO to another one, and returns a DAO-like object that can be joined against
        /// other DAOs or used to retrieve data
        /// </summary>
        /// <param name="nextDao">The other DAO we are joining against.</param>
        /// <param name="criteria">Contains the criteria, sort order, and join criteria for the
        ///                        DAO we are joining against</param>
        /// <typeparam name="R">The type of object returned by the other DAO.</typeparam>
        /// <returns>An intermediary type which can be joined to other DAOs or retrieve data using Get</returns>
        IFastDaoJoinReader<JoinResult<T, R>> Join<R>(IFastDaoBase<R> nextDao, DaoMultiJoinCriteria criteria)
            where R : class, new();
    }

    /// <summary>
    /// This interface defines the first set of joining methods of FastDAO, used to join more than 2 DAOs.
    /// For joining only 2 DAOs, use the simpler Get methods on IFastDaoReader which support joining to a single other DAO
    /// </summary>
    /// <typeparam name="T">The type of object that can be written.</typeparam>
    public interface IFastDaoJoiner<T> : IFastDaoBase<T> where T : class, new()
    {
        /// <summary>
        /// Prepares this DAO for being joined to others
        /// </summary>
        /// <param name="daoCrit">The criteria for the DAO</param>
        /// <param name="daoAlias">An alias used to determine which DAO a join criteria applies to.
        ///                        Not necessary unless the same DAO will be joined more than once</param>
        /// <returns>An intermediary type which can be joined to other DAOs or retrieve data using Get</returns>
        IFastDaoJoinReader<T> PrepareJoin(DaoCriteria daoCrit = null, string daoAlias = null);
    }

    /// <summary>
    /// An intermediary type returned by IFastDAOJoiner.Join, which can be joined to other
    /// DAOs or retrieve data using Get or GetCount
    /// </summary>
    /// <typeparam name="T">A tree of JoinResult objects which determines the types returned</typeparam>
    public interface IFastDaoJoinReader<T> : IFastDaoNextJoiner<T> where T : class, new()
    {
        /// <summary>
        /// Returns all objects of the given types which match the specified criteria.
        /// </summary>
        /// <param name="orders">A list of criteria to sort the results by</param>
        /// <param name="start">Which row to start returning data from (Ignored if null)</param>
        /// <param name="limit">How many records to return (Ignored if null)</param>
        /// <returns>A list of objects, or an empty list (not null).</returns>
        IList<T> Get(IList<MultiJoinSortOrder> orders = null, uint? start = null, uint? limit = null);

        /// <summary>
        /// Returns all objects of the given types which match the specified criteria.
        /// </summary>
        /// <param name="transaction">The transaction to do this as part of. May be null.</param>
        /// <param name="orders">A list of criteria to sort the results by</param>
        /// <param name="start">Which row to start returning data from (Ignored if null)</param>
        /// <param name="limit">How many records to return (Ignored if null)</param>
        /// <returns>A list of objects, or an empty list (not null).</returns>
        IList<T> Get(ITransaction transaction, IList<MultiJoinSortOrder> orders = null,
                     uint? start = null, uint? limit = null);

        /// <summary>
        /// Returns the number objects of the given types which match the specified criteria.
        /// </summary>
        /// <param name="transaction">The transaction to do this as part of. May be null.</param>
        /// <returns>The number of matching objects</returns>
        int GetCount(ITransaction transaction = null);
    }
}