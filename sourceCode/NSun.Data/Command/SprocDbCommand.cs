using System;
using System.Data.Common;
using System.Data;

namespace NSun.Data
{
    /// <summary>
    /// The DbCommand for a stored procedure call
    /// </summary>
    public sealed class SprocDbCommand : DbCommand, IDisposable
    {
        private readonly DbCommand _cmd;
        private readonly QueryCommandBuilder _builder;

        public DbCommand Command
        {
            get
            {
                return _cmd;
            }
        }

        #region Constructors

        internal SprocDbCommand(DbCommand cmd, QueryCommandBuilder builder)
        {
            _cmd = cmd;
            _builder = builder;
        }

        #endregion

        #region DbCommand Members

        /// <summary>
        /// Attempts to cancels the execution of a <see cref="T:System.Data.Common.DbCommand"/>.
        /// </summary>
        public override void Cancel()
        {
            _cmd.Cancel();
        }

        /// <summary>
        /// Gets or sets the text command to run against the data source.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The text command to execute. The default value is an empty string ("").
        /// </returns>
        public override string CommandText
        {
            get
            {
                return _cmd.CommandText;
            }
            set
            {
                _cmd.CommandText = value;
            }
        }

        /// <summary>
        /// Gets or sets the wait time before terminating the attempt to execute a command and generating an error.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The time in seconds to wait for the command to execute.
        /// </returns>
        public override int CommandTimeout
        {
            get
            {
                return _cmd.CommandTimeout;
            }
            set
            {
                _cmd.CommandTimeout = value;
            }
        }

        /// <summary>
        /// Indicates or specifies how the <see cref="P:System.Data.Common.DbCommand.CommandText"/> property is interpreted.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// One of the <see cref="T:System.Data.CommandType"/> values. The default is Text.
        /// </returns>
        public override CommandType CommandType
        {
            get
            {
                return _cmd.CommandType;
            }
            set
            {
                _cmd.CommandType = value;
            }
        }

        /// <summary>
        /// Creates a new instance of a <see cref="T:System.Data.Common.DbParameter"/> object.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Data.Common.DbParameter"/> object.
        /// </returns>
        protected override DbParameter CreateDbParameter()
        {
            return _cmd.CreateParameter();
        }

        /// <summary>
        /// Gets or sets the <see cref="T:System.Data.Common.DbConnection"/> used by this <see cref="T:System.Data.Common.DbCommand"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The connection to the data source.
        /// </returns>
        protected override DbConnection DbConnection
        {
            get
            {
                return _cmd.Connection;
            }
            set
            {
                _cmd.Connection = value;
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="T:System.Data.Common.DbParameter"/> objects.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The parameters of the SQL statement or stored procedure.
        /// </returns>
        protected override DbParameterCollection DbParameterCollection
        {
            get { return _cmd.Parameters; }
        }

        /// <summary>
        /// Gets or sets the <see cref="P:System.Data.Common.DbCommand.DbTransaction"/> within which this <see cref="T:System.Data.Common.DbCommand"/> object executes.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The transaction within which a Command object of a .NET Framework data provider executes. The default value is a null reference (Nothing in Visual Basic).
        /// </returns>
        protected override DbTransaction DbTransaction
        {
            get
            {
                return _cmd.Transaction;
            }
            set
            {
                _cmd.Transaction = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the command object should be visible in a customized interface control.
        /// </summary>
        /// <value></value>
        /// <returns>true, if the command object should be visible in a control; otherwise false. The default is true.
        /// </returns>
        public override bool DesignTimeVisible
        {
            get
            {
                return _cmd.DesignTimeVisible;
            }
            set
            {
                _cmd.DesignTimeVisible = value;
            }
        }

        /// <summary>
        /// Executes the command text against the connection.
        /// </summary>
        /// <param name="behavior">An instance of <see cref="T:System.Data.CommandBehavior"/>.</param>
        /// <returns>
        /// A <see cref="T:System.Data.Common.DbDataReader"/>.
        /// </returns>
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            var result = _cmd.ExecuteReader();
            return result;
        }

        /// <summary>
        /// Executes a SQL statement against a connection object.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
        public override int ExecuteNonQuery()
        {
            var result = _cmd.ExecuteNonQuery();
            return result;
        }

        /// <summary>
        /// Executes the query and returns the first column of the first row in the result set returned by the query. All other columns and rows are ignored.
        /// </summary>
        /// <returns>
        /// The first column of the first row in the result set.
        /// </returns>
        public override object ExecuteScalar()
        {
            var result = _cmd.ExecuteScalar();
            return result;
        }

        /// <summary>
        /// Creates a prepared (or compiled) version of the command on the data source.
        /// </summary>
        public override void Prepare()
        {
            _cmd.Prepare();
        }

        /// <summary>
        /// Gets or sets how command results are applied to the <see cref="T:System.Data.DataRow"/> when used by the Update method of a <see cref="T:System.Data.Common.DbDataAdapter"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// One of the <see cref="T:System.Data.UpdateRowSource"/> values. The default is Both unless the command is automatically generated. Then the default is None.
        /// </returns>
        public override UpdateRowSource UpdatedRowSource
        {
            get
            {
                return _cmd.UpdatedRowSource;
            }
            set
            {
                _cmd.UpdatedRowSource = value;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Releases all resources used by the <see cref="T:System.ComponentModel.Component"/>.
        /// </summary>
        void IDisposable.Dispose()
        {
            _cmd.Dispose();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a parameter.
        /// </summary>
        /// <param name="sprocParamCondition">The sproc param condition.</param>
        /// <returns></returns>
        public SprocDbCommand AddParameter(ParameterEqualsCondition sprocParamCondition)
        {
            if (ReferenceEquals(sprocParamCondition, null))
                throw new ArgumentNullException("sprocParamCondition");

            _builder.AddParameterEqualsConditionParameter(sprocParamCondition, _cmd.Parameters);

            return this;
        }


        /// <summary>
        /// Gets a output parameter value.
        /// </summary>
        /// <param name="sprocParam">The sproc param.</param>
        /// <returns></returns>
        public object GetOutputParameterValue(ParameterExpression sprocParam)
        {
            if (ReferenceEquals(sprocParam, null))
                throw new ArgumentNullException("sprocParam");

            foreach (DbParameter p in _cmd.Parameters)
            {
                if (p.Direction != ParameterDirection.Input
                    && string.Compare(
                       p.ParameterName,
                       _builder.ToParameterName(sprocParam.ID),
                       StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (p.Value == DBNull.Value)
                        return null;
                    return p.Value;
                }
            }

            return null;
        }

        public object GetOutputParameterValue(string sprocParamname)
        {
            foreach (DbParameter p in _cmd.Parameters)
            {
                if (p.Direction != ParameterDirection.Input
                    && string.Compare(
                       p.ParameterName,
                       _builder.ToParameterName(sprocParamname),
                       StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (p.Value == DBNull.Value)
                        return null;
                    return p.Value;
                }
            }

            return null;
        }
        #endregion
    }
}
