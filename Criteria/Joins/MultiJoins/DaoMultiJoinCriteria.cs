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
using Azavea.Open.Common;
using log4net;

namespace Azavea.Open.DAO.Criteria.Joins.MultiJoins
{
    /// <summary>
    /// This class defines how to join a FastDAO to others.
    /// </summary>
    public class DaoMultiJoinCriteria
    {
        /// <summary>
        /// The individual expressions that make up this criteria, defaults to empty.
        /// </summary>
        public readonly List<IMultiJoinExpression> Expressions = new List<IMultiJoinExpression>();

        /// <summary>
        /// These are the criteria applied to this DAO's records by themselves.
        /// I.E. only return rows where this.field == 5.
        /// </summary>
        public DaoCriteria Criteria;

        /// <summary>
        /// Whether this is an "AND" criteria or an "OR" criteria.
        /// </summary>
        public BooleanOperator BoolType;

        /// <summary>
        /// Whether this is an inner join, left join, etc.
        /// </summary>
        public JoinType TypeOfJoin;

        /// <summary>
        /// An alias used in IMultiJoinExpression and MultiJoinSortOrder to distinguish which DAO
        /// a criteria applies to when the same DAO is joined nultiple times in the same request
        /// 
        /// You do not need to set this if there are no DAOs joined multiple times
        /// </summary>
        public string DaoAlias;

        /// <summary>
        /// Constructs a criteria with one expression.
        /// </summary>
        /// <param name="typeOfJoin">Is this an inner join, left join, etc.</param>
        /// <param name="firstExpr">The first expression to add.</param>
        /// <param name="howToAddExpressions">How expressions will be added together.  Determines
        ///                                   if we do exp1 AND exp2 AND exp3, or if we do
        ///                                   exp1 OR exp2 OR exp3.</param>
        /// <param name="daoAlias">An alias used to determine which DAO a join criteria applies to.
        ///                        Not necessary unless the same DAO will be joined more than once</param>
        public DaoMultiJoinCriteria(JoinType typeOfJoin = JoinType.Inner, IMultiJoinExpression firstExpr = null,
            BooleanOperator howToAddExpressions = BooleanOperator.And, string daoAlias = null)
        {
            TypeOfJoin = typeOfJoin;
            BoolType = howToAddExpressions;
            if (firstExpr != null)
            {
                Expressions.Add(firstExpr);
            }
            DaoAlias = daoAlias;
        }

        /// <summary>
        /// Completely clears the object so that it may be used over again.
        /// </summary>
        public void Clear()
        {
            BoolType = BooleanOperator.And;
            Expressions.Clear();
            Criteria = null;
        }

        ///<summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        ///</summary>
        ///
        ///<returns>
        /// A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public override string ToString()
        {
            return "Expressions: " + StringHelper.Join(Expressions) + " (" +
                   BoolType + "ed)";
        }
    }
}
