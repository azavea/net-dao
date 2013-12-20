using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;
using Azavea.Open.DAO.Criteria;
using Azavea.Open.DAO.Criteria.Joins;
using Azavea.Open.DAO.Criteria.Joins.MultiJoins;
using Azavea.Open.DAO.Exceptions;
using Azavea.Open.DAO.Util;

namespace Azavea.Open.DAO
{
    /// <summary>
    /// A utility class used to hold information on joins for FastDAOJoinReader
    /// It is only intended to be created by the constructor of FastDAOJoinReader,
    /// but it is public because it is in the method signatures for IDaMultiJoinableLayer
    /// </summary>
    public class JoinInfo : DaoMultiJoinCriteria
    {
        public new IList<IMultiJoinExpression> Expressions;
        public new JoinType? TypeOfJoin;
        public ClassMapping ClassMap;

        internal JoinInfo() {}
    }

    /// <summary>
    /// A "Pseudo" reader class used for chaining joins to other classes
    /// This class should not be instantiated directly, instead you should call IFastDaoJoiner.Join()
    /// </summary>
    /// <typeparam name="T">The type of object that will be returned by a call to Get().
    /// This type changes with successive calls to Join(), becoming a series of nested JoinResults
    /// with types ordered first to last from left to right.
    /// </typeparam>
    public class FastDAOJoinReader<T> : IFastDaoJoinReader<T> where T : class, new()
    {

        private readonly IList<JoinInfo> _joinInfos;
        private readonly IConnectionDescriptor _connDesc;
        private readonly IDaMultiJoinableLayer _joinableDataLayer;

        internal FastDAOJoinReader(DaoCriteria crit, string daoAlias, IFastDaoBase<T> firstDao)
        {
            _joinableDataLayer = firstDao.ConnDesc.CreateDataAccessLayer() as IDaMultiJoinableLayer;
            if (_joinableDataLayer  == null)
            {
                throw new NotSupportedException("DAO does not support native joins");
            }

            daoAlias = daoAlias ?? FastDAOHelper.GetDaoAlias(firstDao.ClassMap);

            _connDesc = firstDao.ConnDesc;
            _joinInfos = new List<JoinInfo>
                {
                    new JoinInfo
                        {
                            TypeOfJoin = null,
                            DaoAlias = daoAlias,
                            Criteria = crit,
                            ClassMap = firstDao.ClassMap
                        }
                };
        }

        private FastDAOJoinReader(IList<JoinInfo> joinInfos, IConnectionDescriptor connDesc, IDaMultiJoinableLayer dataLayer)
        {
            _connDesc = connDesc;
            _joinableDataLayer = dataLayer;
            _joinInfos = joinInfos;
        }

        /// <summary>
        /// Joins this DAO to another one, and returns a DAO-like object that can be joined against
        /// other DAOs or used to retrieve data
        /// </summary>
        /// <param name="nextDao">The other DAO we are joining against.</param>
        /// <param name="criteria">Contains the criteria, sort order, and join criteria for the
        ///                        DAO we are joining against</param>
        /// <typeparam name="R">The type of object returned by the other DAO.</typeparam>
        /// <returns>An intermediary type which can be joined to other DAOs or retrieve data using Get</returns>
        public IFastDaoJoinReader<JoinResult<T, R>> Join<R>(IFastDaoBase<R> nextDao, DaoMultiJoinCriteria criteria)
            where R : class, new()
        {
            if (!_connDesc.Equals(nextDao.ConnDesc))
            {
                throw new NotSupportedException("DAO does not support native joins");
            }

            var nextDaoAlias = criteria.DaoAlias ?? FastDAOHelper.GetDaoAlias(nextDao.ClassMap);
            var knownAliases = _joinInfos.Select(i => i.DaoAlias).ToList();
            if (knownAliases.Contains(nextDaoAlias))
            {
                throw new ArgumentException("You cannot use the same alias multiple times", "criteria");
            }
            knownAliases.Add(nextDaoAlias);
            if (criteria.Expressions.Any(joinExpression => !knownAliases.Contains(joinExpression.OtherDaoAlias)))
            {
                throw new ArgumentException("Join criteria aliases must match an already provided DAO alias", "criteria");
            }

            var joinInfos = new List<JoinInfo>(_joinInfos)
                {
                    new JoinInfo
                        {
                            TypeOfJoin = criteria.TypeOfJoin,
                            Criteria = criteria.Criteria,
                            Expressions = criteria.Expressions,
                            BoolType = criteria.BoolType,
                            ClassMap = nextDao.ClassMap,
                            DaoAlias = nextDaoAlias
                        }
                };

            return new FastDAOJoinReader<JoinResult<T, R>>(joinInfos, _connDesc, _joinableDataLayer);
        }

        /// <summary>
        /// Returns all objects of the given types which match the specified criteria.
        /// </summary>
        /// <param name="orders">A list of criteria to sort the results by</param>
        /// <param name="start">Which row to start returning data from (Ignored if null)</param>
        /// <param name="limit">How many records to return (Ignored if null)</param>
        /// <returns>A list of objects, or an empty list (not null).</returns>
        public IList<T> Get(IList<MultiJoinSortOrder> orders = null, uint? start = null, uint? limit = null)
        {
            return Get(null, orders, start, limit);
        }

        /// <summary>
        /// Returns all objects of the given types which match the specified criteria.
        /// </summary>
        /// <param name="transaction">The transaction to do this as part of. May be null.</param>
        /// <param name="orders">A list of criteria to sort the results by</param>
        /// <param name="start">Which row to start returning data from (Ignored if null)</param>
        /// <param name="limit">How many records to return (Ignored if null)</param>
        /// <returns>A list of objects, or an empty list (not null).</returns>
        public IList<T> Get(ITransaction transaction, IList<MultiJoinSortOrder> orders = null,
                            uint? start = null, uint? limit = null)
        {
            Hashtable parameters = DbCaches.Hashtables.Get();
            if (start != null)
            {
                parameters.Add("start", (uint)start);
            }
            if (limit != null)
            {
                parameters.Add("limit", (uint)limit);
            }
            orders = orders ?? new List<MultiJoinSortOrder>();

            IDaJoinQuery query = (_joinableDataLayer).CreateJoinQuery(_joinInfos, orders);
            var prefixes = query.GetPrefixes();
            if (prefixes.Length != _joinInfos.Count)
            {
                throw new UnexpectedResultsException("Invalid number of table prefixes returned: ", _connDesc);
            }
            parameters.Add("prefixes", query.GetPrefixes());
            _joinableDataLayer.ExecuteQuery(transaction, null, query, CreateJoinObjectsFromReader, parameters);
            _joinableDataLayer.DisposeOfQuery(query);

            var items = (List<T>)parameters["items"];

            DbCaches.Hashtables.Return(parameters);

            return items;
        }

        /// <summary>
        /// Returns the number objects of the given types which match the specified criteria.
        /// </summary>
        /// <param name="transaction">The transaction to do this as part of. May be null.</param>
        /// <returns>The number of matching objects</returns>
        public int GetCount(ITransaction transaction = null)
        {
            return _joinableDataLayer.GetCount(_joinInfos, transaction);
        }

        private void CreateJoinObjectsFromReader(Hashtable parameters, IDataReader reader)
        {
            uint start = 0;
            var limit = uint.MaxValue;
            if (parameters.ContainsKey("start"))
            {
                start = (uint)parameters["start"];
            }
            if (parameters.ContainsKey("limit"))
            {
                limit = (uint)parameters["limit"];
            }
            var prefixes = (string[])parameters["prefixes"];

            var items = new List<T>();
            parameters["items"] = items;
            if (limit == 0)
            {
                return;
            }

            uint rowNum = 1;
            var colNums = DbCaches.StringIntDicts.Get();

            // GetJoinObjectFromReader will need a genericized version of FastDAOHelper.GetDataObjectFromReader
            // in order to construct the objects
            var getDataObjectMethods = new List<MethodInfo>();
            var getDataObjectBaseMethod = typeof (FastDAOHelper).GetMethod("GetDataObjectFromReader", BindingFlags.Static | BindingFlags.NonPublic);
            for (var i = 0; i < _joinInfos.Count; i++)
            {
                // It is faster to access by index than by column name, so we make a dictionary before reading anything
                FastDAOHelper.PopulateColNums(reader, _joinInfos[i].ClassMap, colNums, prefixes[i]);
                getDataObjectMethods.Add(getDataObjectBaseMethod.MakeGenericMethod(_joinInfos[i].ClassMap.ClassType));
            }
            // GetJoinObjectFromReader will need a list of constructors in order to make all of the nested JoinResult<>s
            var joinResultType = typeof (JoinResult<,>);
            var constructorList = new List<ConstructorInfo>();
            var leftType = _joinInfos[0].ClassMap.ClassType;
            for (var i = 1; i < _joinInfos.Count; i++)
            {
                var rightType = _joinInfos[i].ClassMap.ClassType;
                var typeParams = new[] {leftType, rightType};
                leftType = joinResultType.MakeGenericType(typeParams);
                constructorList.Add(leftType.GetConstructor(typeParams));
            }
            while (reader.Read() && items.Count < limit)
            {
                if (rowNum++ > start)
                {
                    items.Add(GetJoinObjectFromReader(reader, colNums, prefixes, constructorList, getDataObjectMethods));
                }
            }
            DbCaches.StringIntDicts.Return(colNums);
        }

        private T GetJoinObjectFromReader(IDataReader reader, IDictionary<string, int> colNumsByName, string[] prefixes,
                                          IList<ConstructorInfo> constructorInfos, IList<MethodInfo> getDataFromObjectMethods)
        {
            var leftObj = GetDataObj(reader, colNumsByName, prefixes[0], _joinInfos[0], getDataFromObjectMethods[0]);
            for (var i = 1; i < _joinInfos.Count; i++)
            {
                var rightObj = GetDataObj(reader, colNumsByName, prefixes[i], _joinInfos[i], getDataFromObjectMethods[i]);
                leftObj = constructorInfos[i - 1].Invoke(new [] {leftObj, rightObj});
            }

            return (T)leftObj;
        }

        private object GetDataObj(IDataReader reader, IDictionary<string, int> colNumsByName, string prefix,
                                  JoinInfo joinInfo, MethodInfo getDataFromObjectMethod)
        {
            return IsRowNull(reader, colNumsByName, prefix, joinInfo.ClassMap)
                ? null
                : getDataFromObjectMethod.Invoke(null, new object[]
                    {
                        reader, colNumsByName, prefix, joinInfo.ClassMap, _joinableDataLayer
                    });
        }

        private bool IsRowNull(IDataReader reader,
                               IDictionary<string, int> colNumsByName, string colPrefix, ClassMapping classMap)
        {
            IEnumerable<string> colNamesToCheck;
            // If there are ID columns, we just need to check if those are null.  ID columns
            // are presumably not allowed to be null.
            if (classMap.IdDataColsByObjAttrs.Count > 0)
            {
                colNamesToCheck = classMap.IdDataColsByObjAttrs.Values;
            }
            // Otherwise, we need to check all columns.  Even that is a hacky check
            // since it is entirely possible to have a row with all nulls in it, but
            // it's the best we can do.
            else
            {
                colNamesToCheck = classMap.AllDataColsByObjAttrs.Values;
            }

            // Now check if they're all null.
            bool allNull = true;
            foreach (string colName in colNamesToCheck)
            {
                if (!reader.IsDBNull(colNumsByName[colPrefix + colName]))
                {
                    allNull = false;
                    break;
                }
            }
            return allNull;
        }
    }
}
