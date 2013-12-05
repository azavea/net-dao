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
    /// Base class for joins that use one field from a single DAO and compares on a value.
    /// </summary>
    public class PropertyValueEqualMultiJoinExpression : AbstractPropertyValueJoinExpression, IMultiJoinExpression
    {
        /// <summary>
        /// The DAO the property applies to (this does not have to be the same as the joined DAO)
        /// </summary>
        public ClassMapping OtherDaoClassMap { get; private set; }

        /// <summary>
        /// An alias used to determine which DAO a join criteria applies to.
        /// Not necessary unless the same DAO is joined more than once
        /// </summary>
        public string OtherDaoAlias { get; private set; }

        /// <summary>
        /// Base class for joins  that use one field from a single DAO and compares on a value.
        /// </summary>
        /// <param name="propertyClassMap">The ClassMapping of the DAO that property applies to</param>
        /// <param name="property">The name of the property on the object returned by the
        ///                        DAO that we are comparing.</param>
        /// <param name="value">The value we are comparing the property to</param>
        /// <param name="trueOrNot">True means look for matches (I.E. ==),
        ///                         false means look for non-matches (I.E. !=)</param>
        /// <param name="otherDaoAlias">An alias used to determine which DAO a join criteria applies to.</param>
        public PropertyValueEqualMultiJoinExpression(ClassMapping propertyClassMap, string property, object value,
            bool trueOrNot = true, string otherDaoAlias = null)
            : base(property, value, trueOrNot)
        {
            OtherDaoClassMap = propertyClassMap;
            OtherDaoAlias = otherDaoAlias ?? FastDAOHelper.GetDaoAlias(propertyClassMap);
        }

        public override IExpression Invert()
        {
            return new PropertyValueEqualMultiJoinExpression(OtherDaoClassMap, Property, Value, !_trueOrNot, OtherDaoAlias);
        }

        /// <summary>
        /// Does nothing, as you can't "flip" a one-sided join...
        /// </summary>
        /// <returns>The same object</returns>
        public override IJoinExpression Flip()
        {
            return this;
        }
    }
}