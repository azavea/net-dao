using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Azavea.Open.DAO.Tests
{
    /// <summary>
    /// This thin DbCommand subclass avoids adding a dependency to a specific DbCommand implementation
    /// </summary>
    public class TestCommand : DbCommand
    {
        /// <summary>
        /// Create a new TestCommand with a SQL string
        /// </summary>
        /// <param name="sql">The SQL string to be set on the CommandText property.</param>
        /// <remarks>Sets the CommandType to CommandType.Text.</remarks>
        public TestCommand(string sql)
            : this()
        {
            CommandText = sql;
            CommandType = CommandType.Text;
        }

        /// <summary>
        /// Create a new TestCommand 
        /// </summary>
        public TestCommand()
        {
            _dbParameters = new TestParameterCollection();
        }

        private readonly TestParameterCollection _dbParameters;

        public override void Prepare() { throw new NotImplementedException(); }
        public override void Cancel() { throw new NotImplementedException(); }
        protected override DbParameter CreateDbParameter() { throw new NotImplementedException(); }
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) { throw new NotImplementedException(); }
        public override int ExecuteNonQuery() { throw new NotImplementedException(); }
        public override object ExecuteScalar() { throw new NotImplementedException(); }
        public override sealed string CommandText { get; set; }
        public override int CommandTimeout { get; set; }
        public override sealed CommandType CommandType { get; set; }
        public override UpdateRowSource UpdatedRowSource { get; set; }
        protected override DbConnection DbConnection { get; set; }
        protected override DbParameterCollection DbParameterCollection { get { return _dbParameters; } }
        protected override DbTransaction DbTransaction { get; set; }
        public override bool DesignTimeVisible { get; set; }
    }

    /// <summary>
    /// /// This thin DbParameterCollection subclass avoids adding a dependency to a specific DbParameterCollection implementation
    /// </summary>
    internal class TestParameterCollection : DbParameterCollection
    {
        private readonly List<DbParameter> _parameters;
 
        public TestParameterCollection()
        {
            _parameters = new List<DbParameter>();
        }

        public override int Add(object value)
        {
            _parameters.Add((DbParameter)value);
            return _parameters.Count;
        }

        public override int Count
        {
            get { return _parameters.Count; }
        }

        protected override DbParameter GetParameter(int index)
        {
            return _parameters[index];
        }

        public override bool Contains(object value)
        {
            return _parameters.Contains((DbParameter)value);
        }

        public override void Clear() { throw new NotImplementedException(); }
        public override int IndexOf(object value) { throw new NotImplementedException(); }
        public override void Insert(int index, object value) { throw new NotImplementedException(); }
        public override void Remove(object value) { throw new NotImplementedException(); }
        public override void RemoveAt(int index) { throw new NotImplementedException(); }
        public override void RemoveAt(string parameterName) { throw new NotImplementedException(); }
        protected override void SetParameter(int index, DbParameter value) { throw new NotImplementedException(); }
        protected override void SetParameter(string parameterName, DbParameter value) { throw new NotImplementedException(); }
        public override object SyncRoot { get { throw new NotImplementedException(); } }
        public override bool IsFixedSize { get { throw new NotImplementedException(); } }
        public override bool IsReadOnly { get { throw new NotImplementedException(); } }
        public override bool IsSynchronized { get { throw new NotImplementedException(); } }
        public override int IndexOf(string parameterName) { throw new NotImplementedException(); }
        public override IEnumerator GetEnumerator() { throw new NotImplementedException(); }
        protected override DbParameter GetParameter(string parameterName) { throw new NotImplementedException(); }
        public override bool Contains(string value) { throw new NotImplementedException(); }
        public override void CopyTo(Array array, int index) { throw new NotImplementedException(); }
        public override void AddRange(Array values) { throw new NotImplementedException(); }
    }
}
