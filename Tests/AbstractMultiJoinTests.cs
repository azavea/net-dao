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
using System.Collections.Generic;
using System.Linq;
using Azavea.Open.Common;
using Azavea.Open.DAO.Criteria.Joins;
using Azavea.Open.DAO.Criteria.Joins.MultiJoins;
using NUnit.Framework;

namespace Azavea.Open.DAO.Tests
{
    /// <summary>
    /// This is a test class you can extend to test joins on any set of 3 DAOs.
    /// </summary>
    public abstract class AbstractMultiJoinTests
    {
        // Copied from AbstractJoinTests
        private readonly FastDAO<JoinClass1> _dao1;
        private readonly FastDAO<JoinClass2> _dao2;
        private readonly bool _expectNullsFirst;
        private readonly bool _supportsLeftOuter;
        private readonly bool _supportsRightOuter;
        private readonly bool _supportsFullOuter;
        private readonly bool _supportsNonMergeJoinableOuter;

        private readonly FastDAO<JoinClass3> _dao3;

        /// <exclude/>
        protected AbstractMultiJoinTests(FastDAO<JoinClass1> dao1, FastDAO<JoinClass2> dao2, FastDAO<JoinClass3> dao3,
            bool expectNullsFirst, bool supportsLeftOuter, bool supportsRightOuter, bool supportsFullOuter,
            bool supportsNonMergeJoinableOuter)
        {
            // Copied from AbstractJoinTests
            _dao1 = dao1;
            _dao2 = dao2;
            _expectNullsFirst = expectNullsFirst;
            _supportsLeftOuter = supportsLeftOuter;
            _supportsRightOuter = supportsRightOuter;
            _supportsFullOuter = supportsFullOuter;
            _supportsNonMergeJoinableOuter = supportsNonMergeJoinableOuter;

            _dao3 = dao3;
        }

        /// <summary>
        /// For child classes whose data access layers implement IDaDdlLayer, this will drop
        /// and recreate all the tables used by this test.  Otherwise it will just truncate them.
        /// 
        /// It will then insert all rows needed for the tests.
        /// </summary>
        [TestFixtureSetUp]
        public void ResetAllTables()
        {
            // Copied from AbstractJoinTest
            AbstractFastDAOTests.ResetTable(_dao1.DataAccessLayer, _dao1.ClassMap);
            AbstractFastDAOTests.ResetTable(_dao2.DataAccessLayer, _dao2.ClassMap);
            var temp1 = new JoinClass1();
            temp1.JoinField = "one";
            temp1.Name = "Bob";
            _dao1.Insert(temp1);
            temp1.JoinField = "two";
            temp1.Name = "Alice";
            _dao1.Insert(temp1);
            temp1.JoinField = "three";
            temp1.Name = "five";
            _dao1.Insert(temp1);
            temp1.JoinField = "four";
            temp1.Name = "Dave";
            _dao1.Insert(temp1);
            temp1.JoinField = "five";
            temp1.Name = "Edmund";
            _dao1.Insert(temp1);

            var temp2 = new JoinClass2();
            temp2.JoinField = "one";
            _dao2.Insert(temp2);
            temp2.JoinField = "two";
            _dao2.Insert(temp2);
            temp2.JoinField = "three";
            _dao2.Insert(temp2);
            temp2.JoinField = "4";
            _dao2.Insert(temp2);
            temp2.JoinField = "5";
            _dao2.Insert(temp2);

            // New
            AbstractFastDAOTests.ResetTable(_dao3.DataAccessLayer, _dao3.ClassMap);
            var temp3 = new JoinClass3();
            temp3.JoinField = "one";
            _dao3.Insert(temp3);
            temp3.JoinField = "two";
            _dao3.Insert(temp3);
            temp3.JoinField = "three";
            _dao3.Insert(temp3);
            temp3.JoinField = "four";
            _dao3.Insert(temp3);
            temp3.JoinField = "5";
            _dao3.Insert(temp3);
        }

        private static void AssertThreewayJoinResults<A, B, C>(IFastDaoJoinReader<JoinResult<JoinResult<A, B>, C>> pseudoReader,
            string[] expectedFirst, string[] expectedSecond, string[] expectedThird, string description,
            IList<MultiJoinSortOrder> orders, uint? start =null, uint? limit = null)
            where A : JoinClass1, new() where B : JoinClass1, new() where C : JoinClass1, new()
        {
            Assert.AreEqual(expectedFirst.Length, expectedSecond.Length,
                "Test bug: Your expected first and second lists had different lengths.");

            Assert.AreEqual(expectedSecond.Length, expectedThird.Length,
                "Test bug: Your expected second and third lists had different lengths.");

            var results = pseudoReader.Get(orders, start, limit);
            Console.WriteLine("Results:\n" + StringHelper.Join(results, "\n"));
            Assert.AreEqual(expectedFirst.Length, results.Count,
                            description + ": Wrong number of results from the join, expected A: " +
                            StringHelper.Join(expectedFirst) + ", B: " + StringHelper.Join(expectedSecond) +
                            " C:" + expectedThird + " but got " + StringHelper.Join(results));
            for (int count = 0; count < expectedFirst.Length; count++)
            {
                if (expectedFirst[count] == null)
                {
                    Assert.IsNull(results[count].Left.Left, description + ": 1st object " +
                                                            count + " was not null: " + results[count]);
                }
                else
                {
                    Assert.IsNotNull(results[count].Left.Left, description + ": 1st object was null.");
                    Assert.AreEqual(expectedFirst[count], results[count].Left.Left.JoinField,
                                    description + ": 1st object " + count + " had the wrong value.");
                }
                if (expectedSecond[count] == null)
                {
                    Assert.IsNull(results[count].Left.Right, description + ": 2nd object " +
                                                             count + " was not null: " + results[count]);
                }
                else
                {
                    Assert.IsNotNull(results[count].Left.Right, description + ": 2nd object was null.");
                    Assert.AreEqual(expectedSecond[count], results[count].Left.Right.JoinField,
                                    description + ": 2nd object " + count + " had the wrong value.");
                }
                if (expectedThird[count] == null)
                {
                    Assert.IsNull(results[count].Right, description + ": 3rd object " +
                                                        count + " was not null: " + results[count]);
                }
                else
                {
                    Assert.IsNotNull(results[count].Right, description + ": 3rd object was null.");
                    Assert.AreEqual(expectedThird[count], results[count].Right.JoinField,
                                    description + ": 3rd object " + count + " had the wrong value.");
                }
            }

            // Now we know the get works, test get count if not testing start/limit
            if (start == null && limit == null)
            {
                Assert.AreEqual(expectedFirst.Length, pseudoReader.GetCount(), "GetCount was incorrect");
            }
        }
        
        /// <exclude/>
        [Test]
        public void TestInnerJoinEquals()
        {
            var crit1 = new DaoMultiJoinCriteria(firstExpr: new EqualMultiJoinExpression("JoinField", "JoinField", _dao1.ClassMap));
            var crit2 = new DaoMultiJoinCriteria(firstExpr: new EqualMultiJoinExpression("JoinField", "JoinField", _dao1.ClassMap));
            var orders = new List<MultiJoinSortOrder> {new MultiJoinSortOrder("ID", _dao1.ClassMap)};
            AssertThreewayJoinResults(_dao1.PrepareJoin().Join(_dao2, crit1).Join(_dao3, crit2),
                              new[] { "one", "two", "three" },
                              new[] { "one", "two", "three" },
                              new[] { "one", "two", "three" },
                              "Inner join, 1 = 2 = 3",
                              orders);
        }

        /// <exclude/>
        [Test]
        public void TestJoinStart()
        {
            var crit1 = new DaoMultiJoinCriteria(firstExpr: new EqualMultiJoinExpression("JoinField", "JoinField", _dao1.ClassMap));
            var crit2 = new DaoMultiJoinCriteria(firstExpr: new EqualMultiJoinExpression("JoinField", "JoinField", _dao2.ClassMap));
            var orders = new List<MultiJoinSortOrder> { new MultiJoinSortOrder("ID", _dao1.ClassMap) };
            AssertThreewayJoinResults(_dao1.PrepareJoin().Join(_dao2, crit1).Join(_dao3, crit2),
                              new[] { "two", "three" },
                              new[] { "two", "three" },
                              new[] { "two", "three" },
                              "Join start only",
                              orders, start: 1);
        }

        /// <exclude/>
        [Test]
        public void TestJoinLimit()
        {
            var crit1 = new DaoMultiJoinCriteria(firstExpr: new EqualMultiJoinExpression("JoinField", "JoinField", _dao1.ClassMap));
            var crit2 = new DaoMultiJoinCriteria(firstExpr: new EqualMultiJoinExpression("JoinField", "JoinField", _dao2.ClassMap));
            var orders = new List<MultiJoinSortOrder> { new MultiJoinSortOrder("ID", _dao1.ClassMap) };
            AssertThreewayJoinResults(_dao1.PrepareJoin().Join(_dao2, crit1).Join(_dao3, crit2),
                              new[] { "one", "two" },
                              new[] { "one", "two" },
                              new[] { "one", "two" },
                              "Join start only",
                              orders, limit: 2);
        }

        /// <exclude/>
        [Test]
        public void TestJoinStartLimit()
        {
            var crit1 = new DaoMultiJoinCriteria(firstExpr: new EqualMultiJoinExpression("JoinField", "JoinField", _dao1.ClassMap));
            var crit2 = new DaoMultiJoinCriteria(firstExpr: new EqualMultiJoinExpression("JoinField", "JoinField", _dao2.ClassMap));
            var orders = new List<MultiJoinSortOrder> { new MultiJoinSortOrder("ID", _dao1.ClassMap) };
            AssertThreewayJoinResults(_dao1.PrepareJoin().Join(_dao2, crit1).Join(_dao3, crit2),
                              new[] { "two" },
                              new[] { "two" },
                              new[] { "two" },
                              "Join start only",
                              orders, start: 1, limit: 1);
        }

        /// <exclude/>
        [Test]
        public void TestLeftOuterLeftOuterJoinEquals()
        {
            if (!_supportsLeftOuter)
            {
                Assert.Ignore("This data source does not support left outer joins.");
            }
            var crit1 = new DaoMultiJoinCriteria(JoinType.LeftOuter, new EqualMultiJoinExpression("JoinField", "JoinField", _dao1.ClassMap));
            var crit2 = new DaoMultiJoinCriteria(JoinType.LeftOuter, new EqualMultiJoinExpression("JoinField", "JoinField", _dao1.ClassMap));
            var orders = new List<MultiJoinSortOrder> { new MultiJoinSortOrder("ID", _dao1.ClassMap) };
            AssertThreewayJoinResults(_dao1.PrepareJoin().Join(_dao2, crit1).Join(_dao3, crit2),
                              new [] {"one", "two", "three", "four", "five"},
                              new [] {"one", "two", "three", null, null},
                              new[] { "one", "two", "three", "four", null },
                              "Left join, 1 = 2, 1 = 3",
                              orders);
        }

        /// <exclude/>
        [Test]
        public void TestLeftOuterLeftInnerJoinEquals()
        {
            if (!_supportsLeftOuter)
            {
                Assert.Ignore("This data source does not support left outer joins.");
            }
            var crit1 = new DaoMultiJoinCriteria(JoinType.LeftOuter, new EqualMultiJoinExpression("JoinField", "JoinField", _dao1.ClassMap));
            var crit2 = new DaoMultiJoinCriteria(JoinType.Inner, new EqualMultiJoinExpression("JoinField", "JoinField", _dao1.ClassMap));
            var orders = new List<MultiJoinSortOrder> { new MultiJoinSortOrder("ID", _dao1.ClassMap) };
            AssertThreewayJoinResults(_dao1.PrepareJoin().Join(_dao2, crit1).Join(_dao3, crit2),
                              new[] { "one", "two", "three", "four" },
                              new[] { "one", "two", "three", null },
                              new[] { "one", "two", "three", "four" },
                              "Left join, 1 = 2, inner join 1 = 3",
                              orders);
        }

        /// <exclude/>
        [Test]
        public void TestROuterJoinEquals()
        {
            if (!_supportsRightOuter)
            {
                Assert.Ignore("This data source does not support right outer joins.");
            }
            var crit1 = new DaoMultiJoinCriteria(JoinType.RightOuter, new EqualMultiJoinExpression("JoinField", "JoinField", _dao1.ClassMap));
            var crit2 = new DaoMultiJoinCriteria(JoinType.RightOuter, new EqualMultiJoinExpression("JoinField", "JoinField", _dao2.ClassMap));
            var orders = new List<MultiJoinSortOrder> {
                new MultiJoinSortOrder("ID", _dao1.ClassMap),
                new MultiJoinSortOrder("ID", _dao2.ClassMap),
                new MultiJoinSortOrder("ID", _dao3.ClassMap)
            };

            if (_expectNullsFirst)
            {
                AssertThreewayJoinResults(_dao1.PrepareJoin().Join(_dao2, crit1).Join(_dao3, crit2),
                              new[] { null, null, "one", "two", "three" },
                              new[] { null, "5", "one", "two", "three" },
                              new[] { "four", "5", "one", "two", "three" },
                              "Right outer join, 1 = 2, 2 = 3",
                              orders);
            }
            else
            {
                AssertThreewayJoinResults(_dao1.PrepareJoin().Join(_dao2, crit1).Join(_dao3, crit2),
                              new[] { "one", "two", "three", null, null },
                              new[] { "one", "two", "three", "5", null },
                              new[] { "one", "two", "three", "5", "four" },
                              "Right outer join, 1 = 2, 2 = 3",
                              orders);
            }
        }

        /// <exclude/>
        [Test]
        public void TestOuterJoinOuterJoinEquals()
        {
            if (!_supportsFullOuter)
            {
                Assert.Ignore("This data source does not support full outer joins.");
            }
            var crit1 = new DaoMultiJoinCriteria(JoinType.Outer, new EqualMultiJoinExpression("JoinField", "JoinField", _dao1.ClassMap));
            var crit2 = new DaoMultiJoinCriteria(JoinType.Outer, new EqualMultiJoinExpression("JoinField", "JoinField", _dao1.ClassMap));
            var orders = new List<MultiJoinSortOrder> {
                new MultiJoinSortOrder("ID", _dao1.ClassMap),
                new MultiJoinSortOrder("ID", _dao2.ClassMap),
                new MultiJoinSortOrder("ID", _dao3.ClassMap)
            };
            if (_expectNullsFirst)
            {
                AssertThreewayJoinResults(_dao1.PrepareJoin().Join(_dao2, crit1).Join(_dao3, crit2),
                                  new [] { null, null, null, "one", "two", "three", "four", "five" },
                                  new [] { null,  "4",  "5", "one", "two", "three",   null,  null },
                                  new [] {  "5", null, null, "one", "two", "three", "four",  null },
                                  "outer join, 1 = 2, 1 = 3",
                                  orders);
            }
            else
            {
                AssertThreewayJoinResults(_dao1.PrepareJoin().Join(_dao2, crit1).Join(_dao3, crit2),
                                  new[] { "one", "two", "three", "four", "five", null, null, null },
                                  new[] { "one", "two", "three",   null,   null,  "4",  "5", null },
                                  new[] { "one", "two", "three", "four",   null, null, null,  "5" },
                                  "outer join, 1 = 2, 1 = 3",
                                  orders);
            }
        }

        /// <exclude/>
        [Test]
        public void TestPropertyValueLeftOuterJoin()
        {
            if (!_supportsLeftOuter)
            {
                Assert.Ignore("This data source does not support left outer joins.");
            }
            var crit1 = new DaoMultiJoinCriteria(JoinType.LeftOuter, new EqualMultiJoinExpression("JoinField", "JoinField", _dao1.ClassMap));
            crit1.Expressions.Add(new PropertyValueEqualMultiJoinExpression(_dao1.ClassMap, "Name", "Bob"));
            var crit2 = new DaoMultiJoinCriteria(JoinType.LeftOuter, new EqualMultiJoinExpression("JoinField", "JoinField", _dao1.ClassMap));
            var orders = new List<MultiJoinSortOrder> { new MultiJoinSortOrder("ID", _dao1.ClassMap) };
            AssertThreewayJoinResults(_dao1.PrepareJoin().Join(_dao2, crit1).Join(_dao3, crit2),
                              new [] {"one", "two", "three", "four", "five"},
                              new [] {"one", null, null, null, null},
                              new [] { "one", "two", "three", "four", null },
                              "Left outer join, additional filter on A.Name for B's join",
                              orders);

            crit2.Expressions.Add(new PropertyValueEqualMultiJoinExpression(_dao1.ClassMap, "Name", "Bob"));
            AssertThreewayJoinResults(_dao1.PrepareJoin().Join(_dao2, crit1).Join(_dao3, crit2),
                              new[] { "one", "two", "three", "four", "five" },
                              new[] { "one", null, null, null, null },
                              new[] { "one", null, null, null, null },
                              "Left outer join, additional filter on A.Name for B and C's join",
                              orders);
        }

        /// <exclude/>
        [Test]
        public void TestROuterLInnerJoinGreater()
        {
            if (!_supportsRightOuter)
            {
                Assert.Ignore("This data source does not support right outer joins.");
            }
            var crit1 = new DaoMultiJoinCriteria(JoinType.RightOuter, new GreaterMultiJoinExpression("JoinField", "JoinField", _dao1.ClassMap));
            var crit2 = new DaoMultiJoinCriteria(JoinType.LeftOuter, new EqualMultiJoinExpression("JoinField", "JoinField", _dao2.ClassMap));
            var orders = new List<MultiJoinSortOrder> {
                new MultiJoinSortOrder("ID", _dao1.ClassMap),
                new MultiJoinSortOrder("ID", _dao2.ClassMap),
                new MultiJoinSortOrder("ID", _dao3.ClassMap)
            };
            if (_expectNullsFirst)
            {
                AssertThreewayJoinResults(_dao1.PrepareJoin().Join(_dao2, crit1).Join(_dao3, crit2),
                                  new [] { null, "one", "one", "two",  "two", "two", "two", "three", "three", "three", "four", "four", "five", "five" },
                                  new [] { "two",  "4",   "5", "one", "three",  "4",   "5",   "one",     "4",     "5",    "4",    "5",    "4",    "5" },
                                  new [] { "two", null,   "5", "one", "three", null,   "5",   "one",    null,     "5",   null,    "5",   null,    "5" },
                                  "Right outer join, 1 > 2",
                                  orders);
            }
            else
            {
                AssertThreewayJoinResults(_dao1.PrepareJoin().Join(_dao2, crit1).Join(_dao3, crit2),
                                  new [] {"one", "one", "two",  "two", "two", "two", "three", "three", "three", "four", "four", "five", "five", null },
                                  new [] {  "4",   "5", "one", "three",  "4",   "5",   "one",     "4",     "5",    "4",    "5",    "4",    "5", "two" },
                                  new [] { null,   "5", "one", "three", null,   "5",   "one",    null,     "5",   null,    "5",   null,    "5", "two" },
                                  "Right outer join, 1 > 2",
                                  orders);
            }
        }

        /// <exclude/>
        [Test]
        public void TestInnerJoinLesser()
        {
            var crit1 = new DaoMultiJoinCriteria(JoinType.Inner, new LesserMultiJoinExpression("JoinField", "JoinField", _dao1.ClassMap));
            var crit2 = new DaoMultiJoinCriteria(JoinType.LeftOuter, new EqualMultiJoinExpression("JoinField", "JoinField", _dao2.ClassMap));
            var orders = new List<MultiJoinSortOrder> {
                new MultiJoinSortOrder("ID", _dao1.ClassMap),
                new MultiJoinSortOrder("ID", _dao2.ClassMap),
                new MultiJoinSortOrder("ID", _dao3.ClassMap)
            };
            AssertThreewayJoinResults(_dao1.PrepareJoin().Join(_dao2, crit1).Join(_dao3, crit2),
                              new [] { "one", "one", "three", "four", "four", "four", "five", "five", "five" },
                              new [] { "two", "three", "two", "one", "two", "three", "one", "two", "three" },
                              new [] { "two", "three", "two", "one", "two", "three", "one", "two", "three" },
                              "Inner join, 1 < 2",
                              orders);
        }

        /// <exclude />
        [Test]
        public void TestSelfJoin()
        {
            var crit1 = new DaoMultiJoinCriteria(JoinType.Inner,
                                                 new EqualMultiJoinExpression("Name", "JoinField", _dao1.ClassMap,
                                                     otherDaoAlias: "Person"),
                                                 daoAlias: "Doppleganger");
            var crit2 = new DaoMultiJoinCriteria(JoinType.Inner,
                                                 new EqualMultiJoinExpression("JoinField", "JoinField", _dao1.ClassMap,
                                                     otherDaoAlias: "Doppleganger"),
                                                 daoAlias: "other person");
            var orders = new List<MultiJoinSortOrder> { new MultiJoinSortOrder("ID", _dao1.ClassMap, daoAlias: "Person") };
            AssertThreewayJoinResults(_dao1.PrepareJoin(daoAlias: "Person").Join(_dao1, crit1).Join(_dao1, crit2),
                              new[] { "three" },
                              new[] { "five" },
                              new[] { "five" },
                              "inner join 1 = 1 = 1, name = joinfield, joinfield = joinfield",
                              orders);
        }
    }

    /// <exclude/>
    public class JoinClass3 : JoinClass1
    {
        /// <exclude/>
        public JoinClass3()
        {
        }
        /// <exclude/>
        public JoinClass3(string joinField)
        {
            JoinField = joinField;
        }
    }
}