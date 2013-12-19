using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Azavea.Open.Common;

namespace Azavea.Open.DAO.Util
{
    internal class FastDAOHelper
    {
        /// <summary>
        /// This method returns an object that has been loaded from the current
        /// row of the data reader.
        /// </summary>
        /// <param name="reader">The reader, which should already be positioned on the row to read.</param>
        /// <param name="colNums">A dictionary of column name to index mappings (faster than calling
        ///     GetOrdinal over and over again).</param>
        /// <param name="colPrefix">The prefix (if any) to use when looking for columns by name.</param>
        /// <param name="classMap"></param>
        /// <param name="dataLayer"></param>
        /// <returns>The newly loaded data object.</returns>
        internal static T GetDataObjectFromReader<T>(IDataReader reader, IDictionary<string, int> colNums,
                                                     string colPrefix, ClassMapping classMap, IDaLayer dataLayer)
            where T : class, new()
        {
            T retVal = new T();
            foreach (string colName in classMap.AllDataColsByObjAttrs.Values)
            {
                // It is possible for the object to have fields that don't exist
                // in the database (or at least in the cols returned by this query).
                if (colName != null)
                {
                    // Prefix the name with the prefix.
                    int colIndex = colNums[colPrefix + colName];
                    if (!reader.IsDBNull(colIndex))
                    {
                        SetValueOnObject(retVal, classMap, colName, reader[colIndex], dataLayer);
                    }
                    else
                    {
                        SetValueOnObject(retVal, classMap, colName, null, dataLayer);
                    }
                }
            }
            return retVal;
        }

        /// <summary>
        /// Populates the dictionary of column name to index mappings, so that
        /// we can minimize the number of times we call GetOrdinal.
        /// </summary>
        /// <param name="reader">Reader that has been generated from some query.</param>
        /// <param name="classMap">The ClassMapping for the DAO</param>
        /// <param name="colNums">Mapping dictionary to populate.</param>
        /// <param name="colNamePrefix">The prefix (if any) to use for looking up our
        ///                             columns from the data reader.  I.E. "TableName." or
        ///                             "TableAlias." or whatever.</param>
        internal static void PopulateColNums(IDataReader reader, ClassMapping classMap,
                                             IDictionary<string, int> colNums, string colNamePrefix)
        {
            foreach (string colName in classMap.AllDataColsByObjAttrs.Values)
            {
                string prefixedName = colNamePrefix + colName;
                try
                {
                    colNums[prefixedName] = reader.GetOrdinal(prefixedName);
                }
                catch (Exception e)
                {
                    throw new LoggingException("The " + classMap + " has attribute '" +
                                               classMap.AllObjAttrsByDataCol[colName] + "' mapped to column '" + colName +
                                               "', but that column was not present in the results of our query.", e);
                }
            }
        }

        /// <summary>
        /// Given an object and a (data source) column name
        /// set the given memberValue onto the object's property.
        /// </summary>
        /// <param name="dataObj">Object to set the value upon.</param>
        /// <param name="classMap">Object's mapping.</param>
        /// <param name="colName">Name of the column we got the value from.</param>
        /// <param name="memberValue">Value to set on the field or property.</param>
        internal static void SetValueOnObject<T>(T dataObj, ClassMapping classMap,
                                                string colName, object memberValue, IDaLayer dataLayer)
        {
            MemberInfo info = null;
            try
            {
                info = classMap.AllObjMemberInfosByDataCol[colName];
                SetValueOnObjectProperty(dataObj, memberValue, info, dataLayer);
            }
            catch (Exception e)
            {
                throw new LoggingException("Unable to set value (" + memberValue + ") from column " + colName +
                    " to member " + (info == null ? "<null>" : info.Name) + " on type " + classMap.TypeName, e);
            }
        }

        /// <summary>
        /// Given an object and a MemberInfo object
        /// set the given memberValue onto the object's property.
        /// </summary>
        /// <param name="dataObj">Object to set the value upon.</param>
        /// <param name="memberValue">The new value to set</param>
        /// <param name="info">The metadata pertaining to the object property (taken from the ClassMapping)</param>
        internal static void SetValueOnObjectProperty<T>(T dataObj, object memberValue, MemberInfo info, IDaLayer dataLayer)
        {
            // Don't call MemberType getter twice
            MemberTypes type = info.MemberType;
            if (type == MemberTypes.Field)
            {
                var fInfo = ((FieldInfo)info);
                object newValue = memberValue == null
                                      ? null
                                      : dataLayer.CoerceType(fInfo.FieldType, memberValue);
                fInfo.SetValue(dataObj, newValue);
            }
            else if (type == MemberTypes.Property)
            {
                var pInfo = ((PropertyInfo)info);
                object newValue = memberValue == null
                                      ? null
                                      : dataLayer.CoerceType(pInfo.PropertyType, memberValue);
                pInfo.SetValue(dataObj, newValue, null);
            }
        }

        internal static bool IsRowNull(IDataReader reader, IDictionary<string, int> colNumsByName, string colPrefix, ClassMapping classMap)
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

        internal static string GetDaoAlias(ClassMapping classMap)
        {
            return classMap.Table;
        }
    }
}
