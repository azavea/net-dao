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

using Azavea.Open.Common;
using NUnit.Framework;

namespace Azavea.Open.DAO.Tests
{
    /// <exclude/>
    [TestFixture]
    public class PseudoJoinTests : AbstractJoinTests
    {
        /// <exclude/>
        /// Use two different connection descriptors to force FastDAO to use the PseudoJoiner.
        public PseudoJoinTests()
            : base(
                new FastDAO<JoinClass1>(new Config("..\\..\\Tests\\MemoryDao.config", "MemoryDaoConfig"), "DAO"),
                new FastDAO<JoinClass2>(new Config("..\\..\\Tests\\MemoryDao.config", "MemoryDaoConfig"), "DAO2"),
                false, true, true, true, true) { }
    }
}