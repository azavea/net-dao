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

namespace Azavea.Open.DAO.Criteria
{
    /// <summary>
    /// This base class just provides the TrueOrNot implementation.
    /// </summary>
    [Serializable]
    public abstract class AbstractExpression : IExpression
    {
        /// <summary>
        /// Holds the value indicating whether this expression is negated.
        /// </summary>
        protected bool _trueOrNot;

        /// <summary>
        /// This lets you specify whether to negate this expression.
        /// </summary>
        /// <param name="trueOrNot">True means look for matches (I.E. ==),
        ///                         false means look for non-matches (I.E. !=)</param>
        protected AbstractExpression(bool trueOrNot)
        {
            _trueOrNot = trueOrNot;
        }

        /// <summary>
        /// This tells you whether you want things that match, or things that
        /// don't match, this expression.
        /// </summary>
        /// <returns>True means look for matches (I.E. ==), false means look for non-matches (I.E. !=)</returns>
        virtual public bool TrueOrNot()
        {
            return _trueOrNot;
        }

        /// <summary>
        /// Produces an expression that is the exact opposite of this expression.
        /// The new expression should exclude everything this one includes, and
        /// include everything this one excludes.
        /// </summary>
        /// <returns>The inverse of this expression.</returns>
        public abstract IExpression Invert();

        /// <exclude/>
        public override string ToString()
        {
            return "TrueOrNot=" + _trueOrNot + ", Type=" + GetType().Name;
        }
    }
}