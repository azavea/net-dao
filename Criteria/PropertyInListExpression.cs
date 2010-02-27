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
using System.Collections;

namespace Azavea.Open.DAO.Criteria
{
    /// <summary>
    /// Property is equal to one of the values in the given list of values.
    /// </summary>
    [Serializable]
    public class PropertyInListExpression : AbstractSinglePropertyExpression
    {
        /// <summary>
        /// The values to check for.  An empty list will always mean "false".
        /// </summary>
        public readonly IEnumerable Values;

        /// <summary>
        /// Property is equal to one of the values in the given IEnumerable of values.
        /// </summary>
        /// <param name="property">The data class' property/field being compared.
        ///                        May not be null.</param>
        /// <param name="values">The values to check for.
        ///                      May not be null.  An empty list will always mean "false".</param>
        public PropertyInListExpression(string property, IEnumerable values)
            : this(property, values, true) {}
        /// <summary>
        /// Property is equal to one of the values in the given IList of values.
        /// </summary>
        /// <param name="property">The data class' property/field being compared.
        ///                        May not be null.</param>
        /// <param name="values">The values to check for.
        ///                      May not be null.  An empty list will always mean "false".</param>
        /// <param name="trueOrNot">True means look for matches (I.E. ==),
        ///                         false means look for non-matches (I.E. !=)</param>
        public PropertyInListExpression(string property, IEnumerable values, bool trueOrNot)
            : base(property, trueOrNot)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values", "Values parameter cannot be null.");
            }
            Values = values;
        }

        /// <summary>
        /// Produces an expression that is the exact opposite of this expression.
        /// The new expression should exclude everything this one includes, and
        /// include everything this one excludes.
        /// </summary>
        /// <returns>The inverse of this expression.</returns>
        public override IExpression Invert()
        {
            return new PropertyInListExpression(Property, Values, !_trueOrNot);
        }
    }
}