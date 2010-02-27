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

using GeoAPI.Geometries;

namespace Azavea.Open.DAO.Criteria.Spatial
{
    /// <summary>
    /// The feature is completely contained by the criteria shape.
    /// </summary>
    public class WithinExpression : AbstractSingleShapeExpression
    {
        /// <summary>
        /// The feature is completely within the criteria shape.
        /// </summary>
        /// <param name="property">The data class' property/field being compared.
        ///                        May not be null.</param>
        /// <param name="shape">This is what you want records' shapes to be within.</param>
        public WithinExpression(string property, IGeometry shape) : this(property, shape, true) {}

        /// <summary>
        /// The feature is completely within the criteria shape.
        /// </summary>
        /// <param name="property">The data class' property/field being compared.
        ///                        May not be null.</param>
        /// <param name="shape">This is what you want records' shapes to be within.</param>
        /// <param name="trueOrNot">True means look for matches (I.E. is within),
        ///                         False means look for non-matches (I.E. is not within)</param>
        public WithinExpression(string property, IGeometry shape, bool trueOrNot)
            : base(property, shape, trueOrNot) { }

        /// <summary>
        /// Produces an expression that is the exact opposite of this expression.
        /// The new expression should exclude everything this one includes, and
        /// include everything this one excludes.
        /// </summary>
        /// <returns>The inverse of this expression.</returns>
        public override IExpression Invert()
        {
            return new WithinExpression(Property, Shape, !_trueOrNot);
        }
    }
}
