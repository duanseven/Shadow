using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Configuration;
using System.Runtime.InteropServices;

namespace NSun.Data
{ 
    [Serializable]
    public class Database : ICloneable
    {
        #region Init Method

        protected internal DbProviderFactory DbProvider { get; set; }

        protected internal QueryCommandBuilder CommandBuilder { get; internal set; }

        public ConnectionStringSettings ConnectionSetting { get; set; }

        internal bool IsUseRelation { get; set; }        
        //internal LoadType LoadRelationType { get; set; }
        public ProxyType DynamicProxyType { get; set; }

        //如果是true那么不能关闭连接需要外面进行关闭
        public bool IsShareConnection { get; set; }

        #endregion

        #region Construction

        protected Database()
        {
            IsUseRelation = false;

            ConnectionSetting = new ConnectionStringSettings();
            DynamicProxyType = ProxyType.Remoting;
        }

        protected Database(SqlType providerName)
            : this()
        {
            DbProvider = GetDbProviderFactory(providerName);
        }

        public Database(string connectionStringName, SqlType providerName)
            : this(providerName)
        {
            ConnectionSetting.ConnectionString = GetConnectionString(connectionStringName).ConnectionString;
        }

        public Database(string connectionStringName)
            : this(SqlType.Sqlserver9)
        {
            ConnectionSetting.ConnectionString = GetConnectionString(connectionStringName).ConnectionString;
        }

        public Database(SqlType providerName, string connectionString)
            : this(providerName)
        {
            ConnectionSetting.ConnectionString = connectionString;
        }

        public Database(string connectionStringName, QueryCommandBuilder providerCommandBuilder)
            : this()
        {
            ConnectionSetting.ConnectionString = GetConnectionString(connectionStringName).ConnectionString;
            DbProvider = GetDbProviderFactory(providerCommandBuilder);
        }

        public Database(QueryCommandBuilder providerCommandBuilder, string connectionString)
            : this()
        {
            ConnectionSetting.ConnectionString = connectionString;
            DbProvider = GetDbProviderFactory(providerCommandBuilder);
        }

        #endregion

        #region Non-Public Methods

        private ConnectionStringSettings GetConnectionString(string connectionStringName)
        {
            if (string.IsNullOrEmpty(connectionStringName))
            {
                return ConfigurationManager.ConnectionStrings[ConfigurationManager.ConnectionStrings.Count - 1];
            }
            return ConfigurationManager.ConnectionStrings[connectionStringName];
        }

        internal QueryCommandBuilder GetQueryCommandBuilder(SqlType providerName)
        {
            switch (providerName)
            {
                case SqlType.Sqlserver8:
                    {
                        ConnectionSetting.ProviderName = "System.Data.SqlClient";
                        return SqlClient.Sql2000QueryCommandBuilder.Instance;
                    }
                case SqlType.Sqlserver9:
                    {
                        ConnectionSetting.ProviderName = "System.Data.SqlClient";
                        return SqlClient.SqlQueryCommandBuilder.Instance;
                    }
                case SqlType.Sqlserver10:
                    {
                        ConnectionSetting.ProviderName = "System.Data.SqlClient";
                        return SqlClient.Sql2008QueryCommandBuilder.Instance;
                    }
                case SqlType.Oracle:
                    {
                        ConnectionSetting.ProviderName = "System.Data.OracleClient";
                        return OracleClient.OracleQueryCommandBuilder.Instance;
                    }
                case SqlType.Oracle9:
                    {
                        ConnectionSetting.ProviderName = "System.Data.OracleClient";
                        return OracleClient.Oracle9iQueryCommandBuilder.Instance;
                    }
                case SqlType.MsAccess:
                    {
                        ConnectionSetting.ProviderName = "access";
                        return MsAccess.MsAccessQueryCommandBuilder.Instance;
                    }
                case SqlType.MySql:
                    {
                        ConnectionSetting.ProviderName = "mysql";
                        return MySql.MySqlQueryCommandBuilder.Instance;
                    }
                case SqlType.MySql8:
                    {
                        ConnectionSetting.ProviderName = "mysql";
                        return MySql.MySqlQueryCommandBuilder.Instance;
                    }
                case SqlType.Db2:
                    {
                        ConnectionSetting.ProviderName = "db2";
                        return DB2.DB2QueryCommandBuilder.Instance;
                    }
                case SqlType.Sqlite:
                    {
                        ConnectionSetting.ProviderName = "sqlite";
                        return Sqlite.SqliteQueryCommandBuilder.Instance;
                    }
                case SqlType.PostgreSql:
                    {
                        ConnectionSetting.ProviderName = "postgresql";
                        return Npgsql.NpgsqlQueryCommandBuilder.Instance;
                    }

            }
            return SqlClient.SqlQueryCommandBuilder.Instance;
        }

        protected DbProviderFactory GetDbProviderFactory(SqlType providerName)
        {
            CommandBuilder = GetQueryCommandBuilder(providerName);
            return CommandBuilder.GetDbProviderFactory();
        }

        protected DbProviderFactory GetDbProviderFactory(QueryCommandBuilder cmdBuilder)
        {
            CommandBuilder = cmdBuilder;
            return CommandBuilder.GetDbProviderFactory();
        }

        #endregion

        #region Factory Methods

        public DbCommand GetCommandBySqlString(string query)
        {
            return GetCommandByCommandType(CommandType.Text, query);
        }

        private DbCommand GetCommandByCommandType(CommandType commandType, string commandText)
        {
            DbCommand command = DbProvider.CreateCommand();
            command.CommandType = commandType;
            command.CommandText = commandText;
            return command;
        }

        private DbCommandBuilder GetDbCommandBuilder()
        {
            return DbProvider.CreateCommandBuilder();
        }

        public DbDataAdapter GetDataAdapter()
        {
            return DbProvider.CreateDataAdapter();
        }

        public DbConnection GetConnection()
        {
            return CreateConnection();
        }

        public DbConnection GetConnection(bool tryOpen)
        {
            return CreateConnection(tryOpen);
        }

        // No Open Just Set Connectionstring
        public DbConnection CreateConnection()
        {
            DbConnection newConnection = DbProvider.CreateConnection();
            newConnection.ConnectionString = ConnectionSetting.ConnectionString;
            return newConnection;
        }

        // Try Open
        public DbConnection CreateConnection(bool tryOpenning)
        {
            if (!tryOpenning)
            {
                return CreateConnection();
            }
            DbConnection connection = null;
            try
            {
                connection = CreateConnection();
                connection.Open();
            }
            catch (DataException)
            {
                CloseConnection(connection);
                throw;
            }
            return connection;
        }

        #endregion

        #region Prepare Command

        private void PrepareCommand(DbCommand command, DbConnection connection)
        {
            Check.Require(command != null, "command could not be null.");
            Check.Require(connection != null, "connection could not be null.");

            command.Connection = connection;

            if (this.CommandBuilder.GetType() == typeof(NSun.Data.MsAccess.MsAccessQueryCommandBuilder))
            {
                command.CommandText = FilterNTextPrefix(command.CommandText);
            }
        }

        private void PrepareCommand(DbCommand command, DbTransaction transaction)
        {
            Check.Require(command != null, "command could not be null.");
            Check.Require(transaction != null, "transaction could not be null.");

            PrepareCommand(command, transaction.Connection);
            command.Transaction = transaction;

            if (this.CommandBuilder.GetType() == typeof(NSun.Data.MsAccess.MsAccessQueryCommandBuilder))
            {
                command.CommandText = FilterNTextPrefix(command.CommandText);
            }
        }

        private string FilterNTextPrefix(string sql)
        {
            if (sql == null)
            {
                return sql;
            }
            return sql.Replace(" N'", " '");
        }

        #endregion

        #region Execute Builds

        private void DoLoadDataSet(DbCommand command, DataSet dataSet, string[] tableNames)
        {
            using (DbDataAdapter adapter = GetDataAdapter())
            {
                command.CommandText = command.CommandText.Replace('[', ' ').Replace(']', ' ');
                ((IDbDataAdapter)adapter).SelectCommand = command;
                try
                {
                    string systemCreatedTableNameRoot = "Table";
                    for (int i = 0; i < tableNames.Length; i++)
                    {
                        string systemCreatedTableName = (i == 0)
                             ? systemCreatedTableNameRoot
                             : systemCreatedTableNameRoot + i;

                        adapter.TableMappings.Add(systemCreatedTableName, tableNames[i]);
                    }
                    adapter.Fill(dataSet);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        private object DoExecuteScalar(DbCommand command)
        {
            try
            {
                object returnValue = command.ExecuteScalar();
                return returnValue;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                CloseConnection(command);
            }
        }

        private int DoExecuteNonQuery(DbCommand command)
        {
            try
            {
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                CloseConnection(command);
            }
        }

        private IDataReader DoExecuteReader(DbCommand command, CommandBehavior cmdBehavior)
        {
            try
            {
                IDataReader reader = command.ExecuteReader(cmdBehavior);
                return reader;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region ReadDataSet

        private void LoadDataSet(DbCommand command, DataSet dataSet, string tableName)
        {
            LoadDataSet(command, dataSet, new string[] { tableName });
        }

        private void LoadDataSet(DbCommand command, DataSet dataSet, string tableName, DbTransaction transaction)
        {
            LoadDataSet(command, dataSet, new string[] { tableName }, transaction);
        }

        private void LoadDataSet(DbCommand command, DataSet dataSet, string[] tableNames)
        { 
            using (DbConnection connection = GetConnection())
            {
                PrepareCommand(command, connection);
                DoLoadDataSet(command, dataSet, tableNames);
            }
        }

        private void LoadDataSet(DbCommand command, DataSet dataSet, string[] tableNames, DbTransaction transaction)
        {
            //带事务的走事务
            PrepareCommand(command, transaction);
            DoLoadDataSet(command, dataSet, tableNames);
        }

        #endregion

        #region Basic Execute Methods

        public DataSet ExecuteDataSet(DbCommand command)
        {
            DataSet dataSet = new DataSet();
            dataSet.Locale = CultureInfo.InvariantCulture;
            LoadDataSet(command, dataSet, "Table");
            return dataSet;
        }

        public DataSet ExecuteDataSet(DbCommand command, DbTransaction transaction)
        {
            DataSet dataSet = new DataSet();
            dataSet.Locale = CultureInfo.InvariantCulture;
            LoadDataSet(command, dataSet, "Table", transaction);
            return dataSet;
        }

        /// <summary>
        /// 不能使用共享DbConnection
        /// </summary>
        public DataSet ExecuteDataSet(CommandType commandType, string commandText)
        {
            using (DbCommand command = GetCommandByCommandType(commandType, commandText))
            {
                return ExecuteDataSet(command);
            }
        }

        public DataSet ExecuteDataSet(DbTransaction transaction, CommandType commandType, string commandText)
        {
            using (DbCommand command = GetCommandByCommandType(commandType, commandText))
            {
                return ExecuteDataSet(command, transaction);
            }
        }

        public object ExecuteScalar(DbCommand command)
        {           
            if (command.Connection == null)
            {
                using (DbConnection connection = GetConnection(true))
                {
                    PrepareCommand(command, connection);
                    return DoExecuteScalar(command);
                }
            }
            else
            {
                return DoExecuteScalar(command);
            }
        }

        public object ExecuteScalar(DbCommand command, DbTransaction transaction)
        {
            PrepareCommand(command, transaction);
            return DoExecuteScalar(command);
        }

        /// <summary>
        /// 不能使用共享DbConnection
        /// </summary>
        public object ExecuteScalar(CommandType commandType, string commandText)
        {
            using (DbCommand command = GetCommandByCommandType(commandType, commandText))
            {
                return ExecuteScalar(command);
            }
        }

        public object ExecuteScalar(CommandType commandType, string commandText, DbTransaction transaction)
        {
            using (DbCommand command = GetCommandByCommandType(commandType, commandText))
            {
                return ExecuteScalar(command, transaction);
            }
        }

        public int ExecuteNonQuery(DbCommand command)
        {
            if (command.Connection == null)
            {
                using (DbConnection connection = GetConnection(true))
                {
                    PrepareCommand(command, connection);
                    return DoExecuteNonQuery(command);
                }
            }
            else
            {
                return DoExecuteNonQuery(command);
            }
        }

        public int ExecuteNonQuery(DbCommand command, DbConnection connection)
        {
            PrepareCommand(command, connection);
            return DoExecuteNonQuery(command);
        }

        public int ExecuteNonQuery(DbCommand command, DbTransaction transaction)
        {
            PrepareCommand(command, transaction);
            return DoExecuteNonQuery(command);
        }

        /// <summary>
        /// 不能使用共享DbConnection
        /// </summary>
        public int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            using (DbCommand command = GetCommandByCommandType(commandType, commandText))
            {
                return ExecuteNonQuery(command);
            }
        }

        public int ExecuteNonQuery(CommandType commandType, string commandText, DbTransaction transaction)
        {
            using (DbCommand command = GetCommandByCommandType(commandType, commandText))
            {
                return ExecuteNonQuery(command, transaction);
            }
        }
        
        public IDataReader ExecuteReader(DbCommand command)
        {
            if (command.Connection == null)
            {
                DbConnection connection = GetConnection(true);
                PrepareCommand(command, connection);
                try
                {
                    return DoExecuteReader(command, CommandBehavior.CloseConnection);
                }
                catch (DataException e)
                {
                    CloseConnection(connection);
                    throw e;
                }
            }
            else
            {
                try
                {
                    return DoExecuteReader(command, CommandBehavior.CloseConnection);
                }
                catch (DataException e)
                {
                    CloseConnection(command);
                    throw e;
                }
            }
        }

        public IDataReader ExecuteReader(DbCommand command, DbTransaction transaction)
        {
            PrepareCommand(command, transaction);
            return DoExecuteReader(command, CommandBehavior.Default);
        }

        /// <summary>
        /// 不能使用共享DbConnection
        /// </summary>
        public IDataReader ExecuteReader(CommandType commandType, string commandText)
        {
            using (DbCommand command = GetCommandByCommandType(commandType, commandText))
            {
                return ExecuteReader(command);
            }
        }

        public IDataReader ExecuteReader(CommandType commandType, string commandText, DbTransaction transaction)
        {
            using (DbCommand command = GetCommandByCommandType(commandType, commandText))
            {
                return ExecuteReader(command, transaction);
            }
        }

        #endregion

        #region Close Connection

        public void CloseConnection(DbCommand command)
        {
            if (command != null && command.Connection.State != ConnectionState.Closed && !IsShareConnection)
            {
                if (command.Transaction == null)
                {
                    CloseConnection(command.Connection);
                    command.Dispose();
                }
            }
        }

        public void CloseConnection(DbConnection conn)
        {
            if (conn != null && conn.State != ConnectionState.Closed)
            {
                conn.Close();
                conn.Dispose();
            }
        }

        public void CloseConnection(DbTransaction tran)
        {
            if (null != tran)
            {
                if (tran.Connection != null)
                {
                    CloseConnection(tran.Connection);
                    tran.Dispose();
                }
            }
        }

        #endregion

        #region ADO.NET 1.1 style Transactions

        public DbTransaction BeginTransaction()
        {
            return GetConnection(true).BeginTransaction();
        }

        public DbTransaction BeginTransaction(System.Data.IsolationLevel il)
        {
            return GetConnection(true).BeginTransaction(il);
        }

        #endregion

        #region Extended Execute Methods Batch

        //如果没有Tran直接处理
        public int ExecuteInsertReturnAutoIncrementID(DbCommand basicInsertCmd,
            DbTransaction tran, string tableName, string autoIncrementColumn)
        {
            string selectLastInsertIDSql = null;
            //针对Oracle做的处理
            if (!string.IsNullOrEmpty(autoIncrementColumn))
            {
                Dictionary<string, string> additionalOptions = null;
                if (ConnectionSetting.ProviderName == "System.Data.OracleClient" && (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["OracleGlobalAutoIncrementSeqeunceName"])))
                {
                    additionalOptions = new Dictionary<string, string>();
                    additionalOptions.Add("OracleGlobalAutoIncrementSeqeunceName", System.Configuration.ConfigurationManager.AppSettings["OracleGlobalAutoIncrementSeqeunceName"]);
                }
                selectLastInsertIDSql = CommandBuilder.GetSelectLastInsertAutoIncrementIDSql(tableName, autoIncrementColumn, additionalOptions);
            }
            object retVal = 0;

            if (autoIncrementColumn != null && selectLastInsertIDSql != null)
            {
                if (ConnectionSetting.ProviderName == "System.Data.SqlClient" || CommandBuilder.SupportMultiSqlStatementInOneCommand)
                {
                    basicInsertCmd.CommandText = basicInsertCmd.CommandText + ';' + selectLastInsertIDSql;
                    if (tran == null)
                    { 
                        retVal = ExecuteScalar(basicInsertCmd); 
                    }
                    else
                    {
                        retVal = ExecuteScalar(basicInsertCmd, tran);
                    }
                    if (retVal != DBNull.Value)
                    {
                        return Convert.ToInt32(retVal);
                    }
                }
                else if (ConnectionSetting.ProviderName == "System.Data.OracleClient" || selectLastInsertIDSql.Contains("SELECT SEQ_"))
                {
                    if (tran == null)
                    {
                        var command = GetCommandBySqlString(selectLastInsertIDSql);
                        retVal = ExecuteScalar(command);
                        ExecuteNonQuery(basicInsertCmd);
                    }
                    else
                    {
                        retVal = ExecuteScalar(CommandType.Text, selectLastInsertIDSql, tran);
                        ExecuteNonQuery(basicInsertCmd, tran);
                    } 
                    if (retVal != DBNull.Value)
                    {
                        return Convert.ToInt32(retVal);
                    }
                }
                else if (!CommandBuilder.SupportADO20Transaction)
                {
                    DbTransaction t = (tran ?? BeginTransaction());
                    try
                    {
                        ExecuteNonQuery(basicInsertCmd, t);
                        retVal = ExecuteScalar(CommandType.Text, selectLastInsertIDSql, t);

                        if (tran == null)
                        {
                            t.Commit();
                        }
                    }
                    catch (DbException)
                    {
                        if (tran == null)
                        {
                            t.Rollback();
                        }
                        throw;
                    }
                    finally
                    {
                        if (tran == null)
                        {
                            CloseConnection(t);
                            t.Dispose();
                        }
                    }
                    if (retVal != DBNull.Value)
                    {
                        return Convert.ToInt32(retVal);
                    }
                }
                else
                {
                    if (tran == null)
                    {                        
                        ExecuteNonQuery(basicInsertCmd); 
                    }
                    else
                    {
                        ExecuteNonQuery(basicInsertCmd, tran);
                    }
                }
            }
            else
            {
                if (tran == null)
                {  
                    ExecuteNonQuery(basicInsertCmd); 
                }
                else
                {
                    ExecuteNonQuery(basicInsertCmd, tran);
                }
            }
            return Convert.ToInt32(retVal);
        }

        public int ExecuteInsertReturnAutoIncrementID(DbCommand basicInsertCmd,
            string tableName, string autoIncrementColumn)
        {
            return ExecuteInsertReturnAutoIncrementID(basicInsertCmd, null, tableName, autoIncrementColumn);
        }

        #endregion

        #region ILogable 成员

        private LogType _logtype = LogType.File;

        public LogType Logtype
        {
            get { return _logtype; }
            set { _logtype = value; }
        }

        #endregion

        #region Clone

        public object Clone()
        {
            Database db = new Database();
            db.DbProvider = this.DbProvider;
            db.CommandBuilder = this.CommandBuilder;
            db.ConnectionSetting = this.ConnectionSetting;
            db.IsUseRelation = this.IsUseRelation;
            db.Logtype = this.Logtype;
            db.DynamicProxyType = this.DynamicProxyType; 
            return db;
        }

        #endregion                
    }
     
}
