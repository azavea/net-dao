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
using System.Collections.Generic;
using System.Text;
using Azavea.Open.DAO.Util;

namespace Azavea.Open.DAO.SQL
{
    /// <summary>
    /// Base class for SQL-based data access layers that support DDL commands.
    /// </summary>
    public abstract class SqlDaDdlLayer : SqlDaJoinableLayer, IDaDdlLayer
    {
        /// <summary>
        /// Instantiates the data access layer with the connection descriptor for the DB.
        /// </summary>
        /// <param name="connDesc">The connection descriptor that is being used by this FastDaoLayer.</param>
        /// <param name="supportsNumRecords">If true, methods that return numbers of records affected will be
        ///                                 returning accurate numbers.  If false, they will probably return
        ///                                 FastDAO.UNKNOWN_NUM_ROWS.</param>
        protected SqlDaDdlLayer(AbstractSqlConnectionDescriptor connDesc, bool supportsNumRecords)
            : base(connDesc, supportsNumRecords) { }

        #region Implementation of IDaDdlLayer

        /// <summary>
        /// Indexes the data for faster queries.  Some data sources may not support indexes
        /// (such as CSV files), in which case this should throw a NotSupportedException.
        /// 
        /// If the data source supports indexes, but support for creating them is not yet
        /// implemented, this should throw a NotImplementedException.
        /// </summary>
        /// <param name="name">Name of the index.  Some data sources require names for indexes,
        ///                    and even if not this is required so the index can be deleted if desired.</param>
        /// <param name="mapping">ClassMapping for the data that is being indexed.</param>
        /// <param name="propertyNames">Names of the data properties to include in the index (in order).</param>
        public virtual void CreateIndex(string name, ClassMapping mapping, ICollection<string> propertyNames)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes an index on the data for slower queries (but usually faster inserts/updates/deletes).
        /// Some data sources may not support indexes (such as CSV files), 
        /// in which case this method should be a no-op.
        /// 
        /// If the data source supports indexes, but support for deleting them is not yet
        /// implemented, this should throw a NotImplementedException.
        /// 
        /// If there is no index with the given name, this should be a no-op.
        /// </summary>
        /// <param name="name">Name of the index to delete.</param>
        /// <param name="mapping">ClassMapping for the data that was being indexed.</param>
        public virtual void DeleteIndex(string name, ClassMapping mapping)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns whether an index with this name exists or not.  NOTE: This does NOT
        /// verify what properties the index is on, merely whether an index with this
        /// name is already present.
        /// </summary>
        /// <param name="name">Name of the index to check for.</param>
        /// <param name="mapping">ClassMapping for the data that may be indexed.</param>
        /// <returns>Whether an index with this name exists in the data source.</returns>
        public virtual bool IndexExists(string name, ClassMapping mapping)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sequences are things that automatically generate unique, usually incrementing,
        /// numbers.  Some data sources may not support sequences, in which case this should
        /// throw a NotSupportedException.
        /// 
        /// If the data source supports sequences, but support for creating them is not yet
        /// implemented, this should throw a NotImplementedException.
        /// </summary>
        /// <param name="name">Name of the new sequence to create.</param>
        public virtual void CreateSequence(string name)
        {
            SqlConnectionUtilities.XSafeCommand(_connDesc, "CREATE SEQUENCE " + name, null);
        }

        /// <summary>
        /// Removes a sequence.  Some data sources may not support sequences, 
        /// in which case this method should be a no-op.
        /// 
        /// If the data source supports sequences, but support for deleting them is not yet
        /// implemented, this should throw a NotImplementedException.
        /// 
        /// If there is no sequence with the given name, this should be a no-op.
        /// </summary>
        /// <param name="name">Name of the sequence to delete.</param>
        public virtual void DeleteSequence(string name)
        {
            if (SequenceExists(name))
            {
                SqlConnectionUtilities.XSafeCommand(_connDesc, "DROP SEQUENCE " + name, null);
            }
        }

        /// <summary>
        /// Returns whether a sequence with this name exists or not.
        /// </summary>
        /// <param name="name">Name of the sequence to check for.</param>
        /// <returns>Whether a sequence with this name exists in the data source.</returns>
        public virtual bool SequenceExists(string name)
        {
            int count = SqlConnectionUtilities.XSafeIntQuery(_connDesc,
                "SELECT COUNT(*) FROM information_schema.sequences where sequence_name = '" +
                name.ToLower() + "'", null);
            return count == 0;
        }

        /// <summary>
        /// Creates the store house specified in the connection descriptor.  If this
        /// data source doesn't use a store house, this method should be a no-op.
        /// 
        /// If this data source DOES use store houses, but support for adding
        /// them is not implemented yet, this should throw a NotImplementedException.
        /// 
        /// Store house typically corresponds to "database".
        /// </summary>
        public virtual void CreateStoreHouse()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the store house specified in the connection descriptor.  If this
        /// data source doesn't use a store house, this method should be a no-op.
        /// 
        /// If this data source DOES use store houses, but support for dropping
        /// them is not implemented yet, this should throw a NotImplementedException.
        /// 
        /// Store house typically corresponds to "database".
        /// 
        /// If there is no store house with the given name, this should be a no-op.
        /// </summary>
        public virtual void DeleteStoreHouse()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if you need to call "CreateStoreHouse" before storing any
        /// data.  This method is "Missing" not "Exists" because implementations that
        /// do not use a store house (I.E. single-file-based data access layers) can
        /// return "false" from this method without breaking either a user's app or the
        /// spirit of the method.
        /// 
        /// Store house typically corresponds to "database".
        /// </summary>
        /// <returns>Returns true if you need to call "CreateStoreHouse"
        ///          before storing any data.</returns>
        public virtual bool StoreHouseMissing()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates the store room specified in the connection descriptor.  If this
        /// data source doesn't use a store room, this method should be a no-op.
        /// 
        /// If this data source DOES use store rooms, but support for adding
        /// them is not implemented yet, this should throw a NotImplementedException.
        /// 
        /// Store room typically corresponds to "table".
        /// </summary>
        /// <param name="mapping">ClassMapping for the data that will be stored in this room.</param>
        public virtual void CreateStoreRoom(ClassMapping mapping)
        {
            StringBuilder sb = DbCaches.StringBuilders.Get();
            sb.Append("CREATE TABLE ").Append(mapping.Table).Append("(");
            string separator = "";
            IList<string> extraStatements = new List<string>();
            foreach (string col in mapping.AllDataColsInOrder)
            {
                if (AddColDefinition(sb, col, mapping,
                    separator, extraStatements))
                {
                    separator = ",";
                }
            }
            sb.Append(")");
            SqlConnectionUtilities.XSafeCommand(_connDesc, sb.ToString(), null);
            foreach (string statement in extraStatements)
            {
                SqlConnectionUtilities.XSafeCommand(_connDesc, statement, null);
            }
            DbCaches.StringBuilders.Return(sb);
        }

        /// <summary>
        /// For a column, returns the type of generator used for it.
        /// </summary>
        /// <param name="col">Column to look up.</param>
        /// <param name="mapping">Mapping for the class we're creating a table for.</param>
        /// <returns>Type of generator, or GeneratorType.NONE.</returns>
        protected GeneratorType GetGeneratorType(string col, ClassMapping mapping)
        {
            bool isId = mapping.IdGeneratorsByDataCol.ContainsKey(col);
            GeneratorType gen = GeneratorType.NONE;
            if (isId)
            {
                gen = mapping.IdGeneratorsByDataCol[col];
            }
            return gen;
        }

        /// <summary>
        /// Add the definition for the given column to the create table statement.
        /// </summary>
        /// <param name="sb">Current create table statement to append to.</param>
        /// <param name="col">Name of the column to add a definition for.</param>
        /// <param name="mapping">Classmap for the class we're generating columns for.</param>
        /// <param name="separator">Separator to use before appending to sb.</param>
        /// <param name="extraStatements">If adding this column requires any additional
        ///                               SQL statements to be run afterwards, put them here.</param>
        /// <returns>Whether or not it appended anything to the string builder.</returns>
        protected virtual bool AddColDefinition(StringBuilder sb, string col, ClassMapping mapping,
                                                 string separator, ICollection<string> extraStatements)
        {
            Type colType = GetDataType(col, mapping);
            sb.Append(separator).Append(col);
            bool nullable = true;
            string colTypeStr;
            if (colType.Equals(typeof(string)))
            {
                colTypeStr = GetStringType();
            }
            else if (colType.Equals(typeof(int)))
            {
                nullable = false;
                colTypeStr = GetIntType();
            }
            else if (colType.Equals(typeof(long)))
            {
                nullable = false;
                colTypeStr = GetLongType();
            }
            else if (colType.Equals(typeof(double)))
            {
                nullable = false;
                colTypeStr = GetDoubleType();
            }
            else if (colType.Equals(typeof(DateTime)))
            {
                nullable = false;
                colTypeStr = GetDateTimeType();
            }
            else if (colType.IsEnum)
            {
                nullable = false;
                colTypeStr = GetIntType();
            }
            else if (colType.Equals(typeof(bool)))
            {
                nullable = false;
                colTypeStr = GetBooleanType();
            }
            else if (colType.Equals(typeof(short)))
            {
                nullable = false;
                colTypeStr = GetShortType();
            }
            else if (colType.Equals(typeof(byte)))
            {
                nullable = false;
                colTypeStr = GetByteType();
            }
            else if (colType.Equals(typeof(char)))
            {
                nullable = false;
                colTypeStr = GetCharType();
            }
            else if (colType.Equals(typeof(float)))
            {
                nullable = false;
                colTypeStr = GetFloatType();
            }
            else if (colType.Equals(typeof(DateTime?)))
            {
                colTypeStr = GetDateTimeType();
            }
            else if (colType.Equals(typeof(int?)))
            {
                colTypeStr = GetIntType();
            }
            else if (colType.Equals(typeof(long?)))
            {
                colTypeStr = GetLongType();
            }
            else if (colType.Equals(typeof(double?)))
            {
                colTypeStr = GetDoubleType();
            }
            else if (colType.Equals(typeof(float?)))
            {
                colTypeStr = GetFloatType();
            }
            else if (colType.Equals(typeof(bool?)))
            {
                colTypeStr = GetBooleanType();
            }
            else if (colType.Equals(typeof(byte[])))
            {
                colTypeStr = GetByteArrayType();
            }
            // Nullables are generics, so nullable enums are more work to check for.
            else if (colType.IsGenericType &&
                     colType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                // Note that we're only handling nullables, which have 1 generic param
                // which is why the [0] is correct.
                Type genericType = colType.GetGenericArguments()[0];
                if (genericType.IsEnum)
                {
                    colTypeStr = GetIntType();
                }
                else
                {
                    throw new NotImplementedException("Unable to map generic type " + colType +
                                                      "." + genericType + " to a database type.");
                }
            }
            else if (colType.Equals(typeof(AsciiString)))
            {
                colTypeStr = GetAsciiStringType();
            }
            else
            {
                throw new NotImplementedException("Unable to map type " + colType +
                                                  " to a database type.");
            }
            switch (GetGeneratorType(col, mapping))
            {
                case GeneratorType.AUTO:
                    // Override whatever it was mapped as and make it an auto number.
                    colTypeStr = GetAutoType(colType);
                    break;
                case GeneratorType.SEQUENCE:
                    CreateSequence(mapping.IdSequencesByDataCol[col]);
                    break;
            }
            sb.Append(" ").Append(colTypeStr);
            if (!nullable)
            {
                sb.Append(" NOT NULL");
            }
            return true;
        }

        // Below methods are implemented to return ANSI standard SQL types.

        /// <summary>
        /// Returns the DDL for the type of an automatically incrementing column.
        /// Some databases only store autonums in one col type so baseType may be
        /// ignored.
        /// </summary>
        /// <param name="baseType">The data type of the column (nominally).</param>
        /// <returns>The autonumber definition string.</returns>
        protected abstract string GetAutoType(Type baseType);

        /// <summary>
        /// Returns the SQL type used to store an ascii string in the DB.
        /// </summary>
        protected virtual string GetAsciiStringType()
        {
            return "VARCHAR(2000)";
        }

        /// <summary>
        /// Returns the SQL type used to store a byte array in the DB.
        /// </summary>
        protected abstract string GetByteArrayType();

        /// <summary>
        /// Returns the SQL type used to store a float in the DB.
        /// </summary>
        protected virtual string GetFloatType()
        {
            return "FLOAT";
        }

        /// <summary>
        /// Returns the SQL type used to store a char in the DB.
        /// </summary>
        protected virtual string GetCharType()
        {
            return "CHAR(1)";
        }

        /// <summary>
        /// Returns the SQL type used to store a byte in the DB.
        /// </summary>
        protected virtual string GetByteType()
        {
            return "SMALLINT";
        }

        /// <summary>
        /// Returns the SQL type used to store a short in the DB.
        /// </summary>
        protected virtual string GetShortType()
        {
            return "SMALLINT";
        }

        /// <summary>
        /// Returns the SQL type used to store a boolean in the DB.
        /// </summary>
        protected virtual string GetBooleanType()
        {
            return "BIT";
        }

        /// <summary>
        /// Returns the SQL type used to store a DateTime in the DB.
        /// </summary>
        protected virtual string GetDateTimeType()
        {
            return "TIMESTAMP WITH TIME ZONE";
        }

        /// <summary>
        /// Returns the SQL type used to store a double in the DB.
        /// </summary>
        protected virtual string GetDoubleType()
        {
            return "DOUBLE PRECISION";
        }

        /// <summary>
        /// Returns the SQL type used to store a long in the DB.
        /// </summary>
        protected abstract string GetLongType();

        /// <summary>
        /// Returns the SQL type used to store an integer in the DB.
        /// </summary>
        protected virtual string GetIntType()
        {
            return "INTEGER";
        }

        /// <summary>
        /// Returns the SQL type used to store a "normal" (unicode) string in the DB.
        /// </summary>
        protected virtual string GetStringType()
        {
            return "NVARCHAR(2000)";
        }

        /// <summary>
        /// Deletes the store room specified in the connection descriptor.  If this
        /// data source doesn't use a store room, this method should be a no-op.
        /// 
        /// If this data source DOES use store rooms, but support for adding
        /// them is not implemented yet, this should throw a NotImplementedException.
        /// 
        /// Store room typically corresponds to "table".
        /// 
        /// If there is no store room with the given name, this should be a no-op.
        /// </summary>
        /// <param name="mapping">ClassMapping for the data that was stored in this room.</param>
        public virtual void DeleteStoreRoom(ClassMapping mapping)
        {
            if (!StoreRoomMissing(mapping))
            {
                SqlConnectionUtilities.XSafeCommand(_connDesc, "DROP TABLE " + mapping.Table, null);
            }
            foreach (string sequence in mapping.IdSequencesByDataCol.Values)
            {
                if (sequence != null)
                {
                    DeleteSequence(sequence);
                }
            }
        }

        /// <summary>
        /// Returns true if you need to call "CreateStoreRoom" before storing any
        /// data.  This method is "Missing" not "Exists" because implementations that
        /// do not use a store room can return "false" from this method without
        /// breaking either a user's app or the spirit of the method.
        /// 
        /// Store room typically corresponds to "table".
        /// </summary>
        /// <returns>Returns true if you need to call "CreateStoreRoom"
        ///          before storing any data.</returns>
        public virtual bool StoreRoomMissing(ClassMapping mapping)
        {
            int count = SqlConnectionUtilities.XSafeIntQuery(_connDesc,
                "SELECT COUNT(*) FROM information_schema.tables where table_name = '" +
                mapping.Table.ToLower() + "'", null);
            return count == 0;
        }

        /// <summary>
        /// Uses some form of introspection to determine what data is stored in
        /// this data store, and generates a ClassMapping that can be immediately
        /// used with a DictionaryDAO.  As much data as practical will be populated
        /// on the ClassMapping, at a bare minimum the Table (typically set to
        /// the storeRoomName passed in, or the more correct or fully qualified version
        /// of that name), the TypeName (set to the storeRoomName, since we have no
        /// .NET type), and the "data cols" and "obj attrs" will be the list of 
        /// attributes / columns in the data source, mapped to themselves.
        /// </summary>
        /// <param name="storeRoomName">The name of the storeroom (I.E. table).  May be null
        ///                             if this data source does not use store rooms.</param>
        /// <param name="columnSorter">If you wish the columns / attributes to be in a particular
        ///                            order, supply this optional parameter.  May be null.</param>
        /// <returns>A ClassMapping that can be used with a DictionaryDAO.</returns>
        public virtual ClassMapping GenerateClassMappingFromStoreRoom(string storeRoomName, IComparer<ClassMapColDefinition> columnSorter)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}