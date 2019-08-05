using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Text;

namespace NSun.Data
{ 
    public class BatchCommander
    {
        #region Init Member

        private Database _db;

        internal int _batchSize;

        private DbTransaction _tran;

        private List<DbCommand> _batchCommands;

        private bool isUsingOutsideTransaction = false;

        #endregion

        #region Init Method
        public DbCommand GetSqlStringCommand(string query)
        {
            return CreateCommandByCommandType(CommandType.Text, query);
        }

        private DbCommand CreateCommandByCommandType(CommandType commandType, string commandText)
        {
            DbCommand command = _db.DbProvider.CreateCommand();
            command.CommandType = commandType;
            command.CommandText = commandText;
            return command;
        }

        private DbCommand MergeCommands()
        {
            DbCommand cmd = GetSqlStringCommand("init");
            var sb = new StringBuilder();
            foreach (DbCommand item in _batchCommands)
            {
                if (item.CommandType == CommandType.Text)
                {
                    string sql = item.CommandText;
                    for (int i = 0; i < item.Parameters.Count; i++)
                    {
                        var p = (DbParameter)((ICloneable)item.Parameters[i]).Clone();
                        string newParamName = CommonUtils.MakeGuidUniqueKey(p.ParameterName);// CommonUtils.MakeUniqueKey(16, "@p");
                        sql = sql.Replace(p.ParameterName, newParamName);
                        p.ParameterName = newParamName;
                        cmd.Parameters.Add(p);
                    }
                    sb.Append(sql);
                    sb.Append(';');
                }
            }
            cmd.CommandText = sb.ToString();
            return cmd;
        }
        #endregion

        #region Public Members

        public void ExecuteBatch()
        {
            DbCommand cmd = MergeCommands();
            if (cmd.CommandText.Trim().Length > 0)
            {
                if (_tran != null)
                {
                    cmd.Transaction = _tran;
                    cmd.Connection = _tran.Connection;
                }
                else
                {
                    cmd.Connection = _db.CreateConnection(true);
                }
                cmd.ExecuteNonQuery();
                if (_tran == null)
                {
                    _db.CloseConnection(cmd);
                }
            }
            _batchCommands.Clear();
        }

        public BatchCommander(Database db, int batchSize, IsolationLevel il)
            : this(db, batchSize)
        {
            _tran = db.BeginTransaction(il);
        }

        public BatchCommander(Database db, int batchSize, DbTransaction tran)
            : this(db, batchSize)
        {
            _tran = tran;
            if (tran != null)
            {
                isUsingOutsideTransaction = true;
            }
        }

        public BatchCommander(Database db, int batchSize)
        {
            _db = db;            
            _batchSize = batchSize;
            _batchCommands = new List<DbCommand>(batchSize);
            if (!db.CommandBuilder.SupportMultiSqlStatementInOneCommand)
                _batchSize = 1;
        }

        public void Process(DbCommand cmd)
        {
            if (cmd == null) return;
            cmd.Transaction = null;
            cmd.Connection = null;
            _batchCommands.Add(cmd);
            if (_batchCommands.Count > _batchSize)
            {
                try
                {
                    ExecuteBatch();
                }
                catch
                {
                    if (_tran != null && (!isUsingOutsideTransaction))
                    {
                        _tran.Rollback();
                    }
                    throw;
                }
            }
        }

        public void Close()
        {
            try
            {
                ExecuteBatch();
                if (_tran != null && (!isUsingOutsideTransaction))
                    _tran.Commit();
            }
            catch
            {
                if (_tran != null && (!isUsingOutsideTransaction))
                    _tran.Rollback();
                throw;
            }
            finally
            {
                if (_tran != null && (!isUsingOutsideTransaction))
                    _db.CloseConnection(_tran);
            }
        }

        #endregion
    }
}
