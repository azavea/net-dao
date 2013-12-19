using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azavea.Open.DAO.Criteria.Joins.MultiJoins
{
    /// <summary>
    /// A class that defines one way in which the two DAOs are related during a join.
    /// I.E. "left.field1 = right.field2".
    /// The type indicates the relationship between the fields and/or values
    /// being compared by an expression.
    /// </summary>
    public interface IMultiJoinExpression : IJoinExpression
    {
        /// <summary>
        /// The ClassMap of the other DAO in the criteria
        /// The DAO whose Join this applies to is determined by which
        /// DaoMultiJoinCriteria this object is a member of
        /// </summary>
        ClassMapping OtherDaoClassMap { get; }

        /// <summary>
        /// An alias used to determine which DAO a join criteria applies to.
        /// Not necessary unless the same DAO is joined more than once
        /// </summary>
        string OtherDaoAlias { get; }
    }
}
