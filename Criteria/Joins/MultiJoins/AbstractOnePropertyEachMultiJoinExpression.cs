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

using Azavea.Open.DAO.Util;

namespace Azavea.Open.DAO.Criteria.Joins.MultiJoins
{
    /// <summary>
    /// Base class for normal joins that use one field from both DAOs.
    /// </summary>
    public abstract class AbstractOnePropertyEachMultiJoinExpression : AbstractOnePropertyEachJoinExpression, IMultiJoinExpression
    {
        /// <summary>
        /// The DAO the left property is on
        /// </summary>
        public ClassMapping OtherDaoClassMap { get; protected set; }

        /// <summary>
        /// A manually assigned alias to an already joined table.
        /// Only necessary for the case of distinguishing between multiple
        /// joins of the same DAO, this is automatically assigned otherwise
        /// 
        /// When this MultiJoinExpression is passed to IFastDaoJoiner.Join,
        /// if the criteria's alias *must* match an already provided alias, or
        /// an exception will be thrown
        /// </summary>
        public string OtherDaoAlias { get; protected set; }

        protected bool _otherDaoIsLeft;

        /// <summary>
        /// Base class for normal joins that use one field from both DAOs.
        /// </summary>
        /// <param name="otherDaoMapping">The ClassMapping of the left DAO we are comparing</param>
        /// <param name="leftProperty">The name of the property on the object returned by the
        ///                            left DAO that we are comparing.</param>
        /// <param name="rightProperty">The name of the property on the object returned by the
        ///                             right DAO that we are comparing.</param>
        /// <param name="otherDaoIsLeft">True means otherDaoMapping applies to leftProperty,
        ///                              false means it applies to rightProperty</param>
        /// <param name="trueOrNot">True means look for matches (I.E. ==),
        ///                         false means look for non-matches (I.E. !=)</param>
        /// <param name="otherDaoAlias">An alias used to distinguish between identical ClassMappings</param>
        protected AbstractOnePropertyEachMultiJoinExpression(string leftProperty, string rightProperty,
            ClassMapping otherDaoMapping, bool otherDaoIsLeft, bool trueOrNot, string otherDaoAlias)
            : base(leftProperty, rightProperty, trueOrNot)
        {
            _otherDaoIsLeft = otherDaoIsLeft;
            OtherDaoClassMap = otherDaoMapping;
            OtherDaoAlias = otherDaoAlias ?? FastDAOHelper.GetDaoAlias(otherDaoMapping);
        }
    }
}
