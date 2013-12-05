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
    /// This interface implements nothing.  It's purpose is to ensure that all
    /// of the expressions in DaoMultiJoinCriteria.Expressions inherit from
    /// either AbstractOnePropertyEachMultiJoinExpression
    /// or PropertyValueEqualMultiJoinExpression
    /// </summary>
    public interface IMultiJoinExpression : IJoinExpression {}

    /// <summary>
    /// This class defines how to join two FastDAOs together.
    /// </summary>
    public class DaoMultiJoinCriteria
    {
        /// <summary>
        /// log4net logger for logging any appropriate messages.
        /// </summary>
        protected static ILog _log = LogManager.GetLogger(
            new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().DeclaringType.Namespace);

        /// <summary>
        /// The individual expressions that make up this criteria, defaults to empty.
        /// </summary>
        public readonly List<IMultiJoinExpression> Expressions = new List<IMultiJoinExpression>();

        /// <summary>
        /// The list of properties to sort on, defaults to empty.
        /// </summary>
        public readonly List<MultiJoinSortOrder> Orders = new List<MultiJoinSortOrder>();

        /// <summary>
        /// Whether this is an "AND" criteria or an "OR" criteria.
        /// </summary>
        public BooleanOperator BoolType;
        /// <summary>
        /// Used to limit the data returned, only data rows Start to Start + Limit will be returned.
        /// A value of -1 means ignore this parameter.
        /// </summary>
        public int Start = -1;
        /// <summary>
        /// Used to limit the data returned, only data rows Start to Start + Limit will be returned.
        /// A value of -1 means ignore this parameter.
        /// </summary>
        public int Limit = -1;
        /// <summary>
        /// Constructs a blank inner join criteria, which will return all records unless you customize it.
        /// All expressions added to it will be ANDed together.
        /// </summary>
        public DaoMultiJoinCriteria()
            : this(null, BooleanOperator.And) { }
        /// <summary>
        /// Constructs a criteria with one expression.  May be handy for cases
        /// where you only need one expression.
        /// </summary>
        /// <param name="firstExpr">The first expression to add.</param>
        public DaoMultiJoinCriteria(IMultiJoinExpression firstExpr)
            : this(firstExpr, BooleanOperator.And) { }
        /// <summary>
        /// Constructs a blank criteria, which will return all records unless you customize it.
        /// </summary>
        /// <param name="howToAddExpressions">How expressions will be added together.  Determines
        ///                                   if we do exp1 AND exp2 AND exp3, or if we do
        ///                                   exp1 OR exp2 OR exp3.</param>
        public DaoMultiJoinCriteria(BooleanOperator howToAddExpressions)
            : this(null, howToAddExpressions) { }
        /// <summary>
        /// Constructs a criteria with one expression.
        /// </summary>
        /// <param name="firstExpr">The first expression to add.</param>
        /// <param name="howToAddExpressions">How expressions will be added together.  Determines
        ///                                   if we do exp1 AND exp2 AND exp3, or if we do
        ///                                   exp1 OR exp2 OR exp3.</param>
        public DaoMultiJoinCriteria(IMultiJoinExpression firstExpr, BooleanOperator howToAddExpressions)
        {
            BoolType = howToAddExpressions;
            if (firstExpr != null)
            {
                Expressions.Add(firstExpr);
            }
        }

        /// <summary>
        /// Completely clears the object so that it may be used over again.
        /// </summary>
        public void Clear()
        {
            BoolType = BooleanOperator.And;
            Expressions.Clear();
            Orders.Clear();
            Start = -1;
            Limit = -1;
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
                   BoolType + "ed), orders: " + StringHelper.Join(Orders);
        }
    }
}
