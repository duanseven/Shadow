using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using NSun.Data.Common;
using NSun.Data.Helper;
using NSun.Data.Log;
using System.Reflection;

namespace NSun.Data
{    
    public class DBQuery : ILogable, IDisposable
    {
        #region Protected Member

        protected internal Database Db { get; set; }

        public QueryCommandFactory CommandFactory { get; set; }

        protected internal DbTransaction Transaction { get; set; }

        protected DBQueryFactory DbQueryFactory { get; set; }

        protected internal NSun.Data.Helper.IModelHelper ModelHelp { get; set; }

        #endregion

        #region Construction

        internal DBQuery(Database db)
        {
            Db = db;
            CommandFactory = new QueryCommandFactory(db.CommandBuilder);
            IsUnitOfWork = false;
            CacheSize = 100;
            UnitOfWorkExecContext = new System.Collections.Concurrent.ConcurrentDictionary<QueryCriteria, DbQueryAndArgument>();
            isBatch = false;
            if (db.DynamicProxyType == ProxyType.Remoting)
            {
                ModelHelp = new NSun.Data.Helper.ModelRefObjectHelper(this);
            }
            else
            {
                ModelHelp = new NSun.Data.Helper.ModelHelper(this);
            }
        }

        internal DBQuery(Database db, DBQueryFactory dbqueryfactory)
            : this(db)
        {
            DbQueryFactory = dbqueryfactory;
        }

        #endregion

        #region Create Command

        public InsertSqlSection CreateInsert(string tablename)
        {
            return this.CreateInsert(tablename, GetCurrentDbTransaction());
        }

        public InsertSqlSection CreateInsert(string tablename, DbTransaction tran)
        {
            var inser = new InsertSqlSection(Db, tablename);
            if (null != tran)
                inser.SetTransaction(tran);
            return inser;
        }

        public SelectSqlSection CreateQuery<Table>()
        {
            var table = DBQueryFactory.GetTableSchema(typeof(Table));
            return CreateQuery(table.TableName, GetCurrentDbTransaction());
        }

        public SelectSqlSection CreateQuery(string tablename)
        {
            return CreateQuery(tablename, GetCurrentDbTransaction());
        }

        public SelectSqlSection CreateQuery(IQueryTable tablename)
        {
            return CreateQuery(tablename, GetCurrentDbTransaction());
        }

        public SelectSqlSection CreateQuery<Table>(DbTransaction tran)
        {
            var table = DBQueryFactory.GetTableSchema(typeof(Table));
            return CreateQuery(table.TableName, tran);
        }

        public SelectSqlSection CreateQuery(string tablename, DbTransaction tran)
        {
            var select = new SelectSqlSection(Db, tablename);
            if (null != tran)
                select.SetTransaction(tran);
            return select;
        }

        public SelectSqlSection CreateQuery(IQueryTable tablename, DbTransaction tran)
        {
            var select = new SelectSqlSection(Db, tablename);
            if (null != tran)
                select.SetTransaction(tran);
            return select;
        }

        public UpdateSqlSection CreateUpdate(string tablename)
        {
            return CreateUpdate(tablename, GetCurrentDbTransaction());
        }

        public UpdateSqlSection CreateUpdate(string tablename, DbTransaction tran)
        {
            var up = new UpdateSqlSection(Db, tablename);
            if (null != tran)
                up.SetTransaction(tran);
            return up;
        }

        public DeleteSqlSection CreateDelete(string tablename)
        {
            return CreateDelete(tablename, GetCurrentDbTransaction());
        }

        public DeleteSqlSection CreateDelete(string tablename, DbTransaction tran)
        {
            var del = new DeleteSqlSection(Db, tablename);
            if (null != tran)
                del.SetTransaction(tran);
            return del;
        }

        public StoredProcedureSection CreateStoredProcedure(string sprocname)
        {
            return CreateStoredProcedure(sprocname, GetCurrentDbTransaction());
        }

        public StoredProcedureSection CreateStoredProcedure(string sprocname, DbTransaction tran)
        {
            var sproc = new StoredProcedureSection(Db, sprocname);
            if (null != tran)
                sproc.SetTransaction(tran);
            return sproc;
        }

        public TSprocEt CreateSprocEntity<TSprocEt>() where TSprocEt : SprocEntity
        {
            return CreateSprocEntity<TSprocEt>(GetCurrentDbTransaction());
        }

        public TSprocEt CreateSprocEntity<TSprocEt>(DbTransaction tran) where TSprocEt : SprocEntity
        {
            var sporc = Activator.CreateInstance<TSprocEt>();
            sporc.Db = Db;
            if (null != tran)
                sporc.SetTransaction(tran);
            return sporc;
        }

        public CustomSqlSection CreateCustomSql(string sql)
        {
            return CreateCustomSql(sql, GetCurrentDbTransaction());
        }

        public CustomSqlSection CreateCustomSql(string sql, DbTransaction tran)
        {
            var cust = new CustomSqlSection(Db, sql);
            if (null != tran)
                cust.SetTransaction(tran);
            return cust;
        }

        #endregion

        #region Transation

        public void BeginTransaction()
        {
            Transaction = GetDbTransaction();
        }

        public void BeginTransaction(IsolationLevel il)
        {
            Transaction = GetDbTransaction(il);
        }

        public DbTransaction GetDbTransaction()
        {
            return Db.BeginTransaction();
        }

        public DbTransaction GetDbTransaction(IsolationLevel il)
        {
            return Db.BeginTransaction(il);
        }

        internal DbTransaction GetCurrentDbTransaction()
        {
            if (null != Transaction)
            {
                return Transaction;
            }
            if (null != Transactions.Transaction.Current && null != Transactions.Transaction.Current.DbTransactionWrapper.DbTransaction)
            {
                return Transactions.Transaction.Current.DbTransactionWrapper.DbTransaction;
            }
            return null;
        }

        #endregion

        #region Command Execute

        #region Private Method

        public DbCommand PrepareCommand(QueryCriteria criteria, bool isSetConnection = false)
        {
            if (criteria.Db == null)
                criteria.Db = Db;
            WriteLog(criteria);
            DbCommand cmd = CommandFactory.CreateCommand(criteria, false);
            if (isSetConnection)
            {
                cmd.Connection = Db.GetConnection();
            }
            return cmd;
        }

        private IDataReader FindDataReader(QueryCriteria criteria)
        {
            DbCommand cmd = PrepareCommand(criteria);
            if (criteria.Tran == null)
            {
                return Db.ExecuteReader(cmd);
            }
            else
            {
                return Db.ExecuteReader(cmd, criteria.Tran);
            }
        }

        private IDataReader FindDataReader(QueryCriteria criteria, out Dictionary<string, object> outValues)
        {
            DbCommand cmd = PrepareCommand(criteria);
            IDataReader dr = null;
            if (criteria.Tran == null)
            {
                dr = Db.ExecuteReader(cmd);
            }
            else
            {
                dr = Db.ExecuteReader(cmd, criteria.Tran);
            }
            var stored = criteria as StoredProcedureSection;
            if (stored == null)
            {
                outValues = new Dictionary<string, object>();
                return null;
            }
            outValues = stored.GetOutputParameterValues(cmd);
            return dr;
        }

        private DataSet FindDataSet(QueryCriteria criteria)
        {
            DbCommand cmd = PrepareCommand(criteria);
            if (criteria.Tran == null)
            {
                return Db.ExecuteDataSet(cmd);
            }
            else
            {
                return Db.ExecuteDataSet(cmd, criteria.Tran);
            }
        }

        private DataSet FindDataSet(QueryCriteria criteria, out Dictionary<string, object> outValues)
        {
            DbCommand cmd = PrepareCommand(criteria);
            DataSet ds = null;
            if (criteria.Tran == null)
            {
                ds = Db.ExecuteDataSet(cmd);
            }
            else
            {
                ds = Db.ExecuteDataSet(cmd, criteria.Tran);
            }
            var stored = criteria as StoredProcedureSection;
            if (stored == null)
                outValues = new Dictionary<string, object>();
            else
                outValues = stored.GetOutputParameterValues(cmd);

            return ds;
        }

        #endregion

        #region Public Members

        #region Common
        //得到列
        protected string GetSqlResultColumnText(SelectSqlSection selectsql)
        {
            var sql = selectsql.ResultColumns.Count == 0
                       ? string.Empty
                       : QueryCommandBuilder.AppendResultColumns(selectsql.ResultColumns);
            return sql;
        }
        //分页算法
        protected int GetPages(int pagesize, int pages)
        {
            return CommonUtils.GetSkipCount(pagesize, pages);
        }

        #endregion

        #region Delete

        public int Delete<T>(T tag)
        {
            return Delete(tag, null);
        }

        public int Delete<T>(T tag, DbTransaction tran)
        {
            var info = (tag as IBaseEntity);
            if (info == null) throw new Exception("T isn't baseEntity");

            TableSchema table = DBQueryFactory.GetTableSchema(typeof(T));

            DeleteSqlSection deletesql = CreateDelete(info.GetTableName());
            object value = ReflectionUtils.GetFieldValue(tag, table.IdentyFieldMemberInfo);

            if (value == null)
            {
                return -1;
            }
            if (null != tran)
            {
                deletesql.SetTransaction(tran);
            }

            #region Optimisticlocking

            foreach (var queryColumn in table.OptimisticLocking)
            {
                PropertyInfo pi = null;
                foreach (var item in table.ValuePropertys)
                {
                    if (item.Name.ToLower() == queryColumn.Key.ToLower())
                    {
                        pi = item;
                        break;
                    }
                }
                if (pi != null)
                {
                    object entityFieldValue = pi.GetValue(info, null);
                    if (!CommonUtils.IsDefaultValue(pi.PropertyType, entityFieldValue))
                        deletesql.Where(queryColumn.Value == entityFieldValue);
                }
            }

            #endregion

            deletesql.Where(table.IdentyColumn == value);

            info.SetIsPersistence(false);

            if (IsUnitOfWork)
            {
                UnitOfWorkExecContext.TryAdd(deletesql,
                                             new DbQueryAndArgument() { DbQuery = this, Argument = tag, WorkType = UnitOfWorkType.Delete });
                return -1;
            }

            int res = ToExecute(deletesql);

            if (res == 0 && table.OptimisticLocking.Count > 0)
            {
                throw new Exception("object non-existent or delete yet");
            }

            return res;
        }

        public int DeleteSqlSection(DeleteSqlSection deletesql)
        {
            return DeleteSqlSection(deletesql, null);
        }

        public int DeleteSqlSection(DeleteSqlSection deletesql, DbTransaction tran)
        {
            ClearCache();
            if (tran != null)
                deletesql.SetTransaction(tran);
            if (IsUnitOfWork)
            {
                UnitOfWorkExecContext.TryAdd(deletesql, new DbQueryAndArgument() { DbQuery = this, Argument = null, WorkType = UnitOfWorkType.Delete });
                return -1;
            }
            return ToExecute(deletesql);
        }

        public int DeleteKey<T>(object key)
        {
            return DeleteKey<T>(key, null);
        }

        public int DeleteKey<T>(object key, DbTransaction tran)
        {
            TableSchema table = DBQueryFactory.GetTableSchema(typeof(T));
            if (table == null) throw new Exception("T isn't register");

            DeleteSqlSection deletesql = CreateDelete(table.TableName);
            if (table.IdentyColumn == null)
            {
                return -1;
            }
            if (tran != null)
            {
                deletesql.SetTransaction(tran);
            }
            deletesql.Where(table.IdentyColumn == key);
            if (IsUnitOfWork)
            {
                UnitOfWorkExecContext.TryAdd(deletesql, new DbQueryAndArgument() { DbQuery = this, Argument = key, WorkType = UnitOfWorkType.Delete });
                return -1;
            }
            return ToExecute(deletesql);
        }

        public int Delete<T>(object[] keys)
        {
            return Delete<T>(keys, null);
        }

        public int Delete<T>(object[] keys, DbTransaction tran)
        {
            TableSchema table = DBQueryFactory.GetTableSchema(typeof(T));
            if (table == null) throw new Exception("T isn't register");

            DeleteSqlSection deletesql = CreateDelete(table.TableName);
            if (tran != null)
            {
                deletesql.SetTransaction(tran);
            }
            if (table.IdentyColumn == null)
            {
                return -1;
            }
            deletesql.Where(table.IdentyColumn.In(keys));
            if (IsUnitOfWork)
            {
                UnitOfWorkExecContext.TryAdd(deletesql, new DbQueryAndArgument() { DbQuery = this, Argument = keys, WorkType = UnitOfWorkType.Delete });
                return -1;
            }
            return ToExecute(deletesql);
        }

        #endregion

        #region Exists

        public bool Exists(SelectSqlSection select)
        {
            return Count(select) > 0;
        }

        public bool Exists<T>(QueryColumn qcolumn, object value)
        {
            return Exists<T>(qcolumn, value, null);
        }

        public bool Exists<T>(QueryColumn qcolumn, object value, DbTransaction tran)
        {
            TableSchema table = DBQueryFactory.GetTableSchema(typeof(T));
            if (table == null) throw new Exception("T isn't register");

            SelectSqlSection selectsql = CreateQuery(table.TableName);
            selectsql.Where(qcolumn == value);
            if (tran != null)
            {
                selectsql.SetTransaction(tran);
            }
            return Exists(selectsql);
        }

        public bool ExistsKey<T>(object value)
        {
            return ExistsKey<T>(value, null);
        }

        public bool ExistsKey<T>(object value, DbTransaction tran)
        {
            TableSchema table = DBQueryFactory.GetTableSchema(typeof(T));
            if (table == null) throw new Exception("T isn't register");
            return Exists<T>(table.IdentyColumn, value, tran);
        }

        #endregion

        #region Count

        public int Count<TEntity>()
        {
            TableSchema table = DBQueryFactory.GetTableSchema(typeof(TEntity));
            if (table == null) throw new Exception("TEntity isn't TableSchema");
            var query = CreateQuery(table.TableName);
            return Count(query);
        }

        public int Count(SelectSqlSection selectsql)
        {
            var selectsql2 = selectsql.Clone() as SelectSqlSection;
            DbCommand cmd = CommandFactory.CreateCommand(selectsql2, true);
            object res = null;
            if (selectsql.Tran == null)
            {
                res = Db.ExecuteScalar(cmd);
            }
            else
            {
                res = Db.ExecuteScalar(cmd, selectsql.Tran);
            }
            return Convert.ToInt32(res);
        }

        public long CountLong<TEntity>()
        {
            TableSchema table = DBQueryFactory.GetTableSchema(typeof(TEntity));
            if (table == null) throw new Exception("TEntity isn't TableSchema");
            var query = CreateQuery(table.TableName);
            return CountLong(query);
        }

        public long CountLong(SelectSqlSection selectsql)
        {
            var selectsql2 = selectsql.Clone() as SelectSqlSection;
            DbCommand cmd = CommandFactory.CreateCommand(selectsql2, true);
            object res = null;
            if (selectsql.Tran == null)
            {
                res = Db.ExecuteScalar(cmd);
            }
            else
            {
                res = Db.ExecuteScalar(cmd, selectsql.Tran);
            }
            return Convert.ToInt64(res);
        }

        #endregion

        #region Save Update

        public int Save<T>(T tag)
        {
            return Save(tag, null);
        }

        public int Save<T>(T tag, DbTransaction tran)
        {
            var info = tag as IBaseEntity;
            if (info == null)
            {
                throw new Exception("tag baseclass isn't baseentity ");
            }
            var table = DBQueryFactory.GetTableSchema(tag.GetType());
            int res;
            bool isPersistence = info.IsPersistence();
            if (!isPersistence)//不是持久化
            {

                InsertSqlSection insertsql = CreateInsert(table.TableName);
                if (tran != null)
                {
                    insertsql.SetTransaction(tran);
                }
                res = InsertQueryColumn(insertsql, tag, table);
            }
            else
            {
                UpdateSqlSection updatesql = CreateUpdate(info.GetTableName());
                if (tran != null)
                {
                    updatesql.SetTransaction(tran);
                }
                res = UpdateQueryColumn(updatesql, tag, table);
            }
            return res;
        }

        public int Update(UpdateSqlSection updatesql)
        {
            return Update(updatesql, null);
        }

        public int Update(UpdateSqlSection updatesql, DbTransaction tran)
        {
            if (null != tran)
                updatesql.SetTransaction(tran);
            if (IsUnitOfWork)
            {
                UnitOfWorkExecContext.TryAdd(updatesql, new DbQueryAndArgument() { DbQuery = this, Argument = null, WorkType = UnitOfWorkType.Update });
                return -1;
            }
            return ToExecute(updatesql);
        }

        #endregion

        #region Add Update

        protected int InsertQueryColumn<T>(InsertSqlSection insertsql, T tag, TableSchema table)
        {
            var info = (table.EntityInfo);
            if (info == null) throw new Exception("T isn't baseEntity");

            foreach (KeyValuePair<string, QueryColumn> field in table.CreateFields.Fields)
            {
                PropertyInfo pi = null;
                foreach (var item in table.ValuePropertys)
                {
                    if (item.Name.ToLower() == field.Key.ToLower())
                    {
                        pi = item;
                        break;
                    }
                }
                if (pi == null)
                {
                    throw new Exception("Not this " + field.Key);
                }
                object entityFieldValue = pi.GetValue(tag, null);

                if (!CommonUtils.IsDefaultValue(pi.PropertyType, entityFieldValue))
                    insertsql.AddColumn(field.Value, entityFieldValue);
            }

            if (table.IsIdentyFieldAutoGenerated)
            {
                var id = ToExecuteReturnAutoIncrementId(insertsql, table.IdentyColumn);
                ((PropertyInfo)table.IdentyFieldMemberInfo).SetValue(tag,
                                                            Convert.ChangeType(id,
                                                                              ((PropertyInfo)table.IdentyFieldMemberInfo).PropertyType),
                                                            null);
                info.SetIsPersistence(true);
                return id;
            }
            if (IsUnitOfWork)
            {
                UnitOfWorkExecContext.TryAdd(insertsql, new DbQueryAndArgument() { DbQuery = this, Argument = tag, WorkType = UnitOfWorkType.Insert });
                return -1;
            }
            var res = ToExecute(insertsql);
            info.SetIsPersistence(true);
            return res;
        }

        protected int UpdateQueryColumn<T>(UpdateSqlSection updatesql, T tag, TableSchema table)
        {
            var info = (table.EntityInfo);
            if (info == null) throw new Exception("T isn't baseEntity");

            foreach (KeyValuePair<string, QueryColumn> field in table.UpdateFields.Fields)
            {
                PropertyInfo pi = null;
                foreach (var item in table.ValuePropertys)
                {
                    if (item.Name.ToLower() == field.Key.ToLower())
                    {
                        pi = item;
                        break;
                    }
                }
                if (pi != null)
                {
                    object entityFieldValue = pi.GetValue(tag, null);
                    if (!CommonUtils.IsDefaultValue(pi.PropertyType, entityFieldValue))
                        updatesql.AddColumn(field.Value, entityFieldValue);
                }
            }

            foreach (var queryColumn in table.OptimisticLocking)
            {
                PropertyInfo pi = null;
                foreach (var item in table.ValuePropertys)
                {
                    if (item.Name.ToLower() == queryColumn.Key.ToLower())
                    {
                        pi = item;
                        break;
                    }
                }
                if (pi != null)
                {
                    object entityFieldValue = pi.GetValue(tag, null);
                    if (!CommonUtils.IsDefaultValue(pi.PropertyType, entityFieldValue))
                        updatesql.Where(queryColumn.Value == entityFieldValue);
                }
            }
            updatesql.Where(table.IdentyColumn == ReflectionUtils.GetFieldValue(tag, table.IdentyFieldMemberInfo));
            if (IsUnitOfWork)
            {
                UnitOfWorkExecContext.TryAdd(updatesql, new DbQueryAndArgument() { DbQuery = this, Argument = tag, WorkType = UnitOfWorkType.Update });
                return -1;
            }
            return ToExecute(updatesql);
        }

        #endregion

        #region ExecuteNonQuery

        public int ToExecute(QueryCriteria criteria)
        {
            if (IsUnitOfWork)
            {
                UnitOfWorkExecContext.TryAdd(criteria, new DbQueryAndArgument()
                {
                    DbQuery = this,
                    WorkType = UnitOfWorkType.Exec
                });
                return -1;
            }
            DbCommand cmd = PrepareCommand(criteria);
            if (isBatch)
            {
                batchCommander.Process(cmd);
                return -1;
            }
            if (criteria.Tran == null)
            {
                return Db.ExecuteNonQuery(cmd);
            }
            else
            {
                return Db.ExecuteNonQuery(cmd, criteria.Tran);
            }
        }

        public int ToExecute(QueryCriteria criteria, out Dictionary<string, object> outValues)
        {
            DbCommand cmd = PrepareCommand(criteria);
            int affactRows = -1;
            if (criteria.Tran == null)
            {
                affactRows = Db.ExecuteNonQuery(cmd);
            }
            else
            {
                affactRows = Db.ExecuteNonQuery(cmd, criteria.Tran);
            }
            var stored = criteria as StoredProcedureSection;
            if (stored == null)
            {
                outValues = new Dictionary<string, object>();
                return -1;
            }
            outValues = stored.GetOutputParameterValues(cmd);
            return affactRows;
        }

        public int ToExecute(QueryCriteria criteria, out DbCommand cmdout)
        {
            DbCommand cmd = PrepareCommand(criteria);
            if (isBatch)
            {
                batchCommander.Process(cmd);
                cmdout = null;
                return -1;
            }
            int res = Db.ExecuteNonQuery(cmd);
            cmdout = cmd;
            return res;
        }

        public int ToExecuteReturnAutoIncrementId(QueryCriteria criteria, QueryColumn autoIncrementColumn)
        {
            string filteredAutoColumn = autoIncrementColumn.ColumnName.IndexOf('.') > 0 ? autoIncrementColumn.ColumnName.Split('.')[1] : autoIncrementColumn.ColumnName;
            DbCommand cmd = PrepareCommand(criteria);
            if (isBatch)
            {
                batchCommander.Process(cmd);
                return -1;
            }
            if (criteria.Tran == null)
            {
                return Db.ExecuteInsertReturnAutoIncrementID(cmd, criteria.TableName, filteredAutoColumn);
            }
            else
            {
                return Db.ExecuteInsertReturnAutoIncrementID(cmd, criteria.Tran, criteria.TableName,
                    filteredAutoColumn);
            }
        }

        #endregion

        #region ProcedureExecute

        public int ToProcedureExecute(SprocEntity procObj)
        {
            DbCommand sc;
            int res = ToExecute(procObj, out sc);
            procObj.BindOutProperty(sc);
            return res;
        }

        public object ToProcedureScalar(SprocEntity criteria)
        {
            DbCommand cmd = PrepareCommand(criteria);
            object res = null;
            if (criteria.Tran == null)
            {
                res = Db.ExecuteScalar(cmd);
            }
            else
            {
                res = Db.ExecuteScalar(cmd, criteria.Tran);
            }
            criteria.BindOutProperty(cmd);
            return res;
        }

        public object ToProcedureScalar(Type returnType, SprocEntity criteria)
        {
            object retValue = ToProcedureScalar(criteria);

            if (retValue == null || retValue == DBNull.Value)
                return CommonUtils.DefaultValue(returnType);

            if (returnType == typeof(Guid))
            {
                return DataUtils.ToGuid(ToScalar(criteria));
            }
            return Convert.ChangeType(retValue, returnType);
        }

        public TReturnType ToProcedureScalar<TReturnType>(SprocEntity criteria)
        {
            return (TReturnType)ToProcedureScalar(typeof(TReturnType), criteria);
        }

        public object ToProcedureStoredScalar(SprocEntity criteria)
        {
            Dictionary<string, object> outValues;
            DataSet ds = FindDataSet(criteria, out outValues);
            criteria.InitialOutParameter(outValues);
            object retObj = null;
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                retObj = ds.Tables[0].Rows[0][0];
            }
            ds.Dispose();
            return retObj;
        }

        public IDataReader ToProcedureDataReader(SprocEntity criteria)
        {
            Dictionary<string, object> outValues;
            var dr = FindDataReader(criteria, out outValues);
            criteria.InitialOutParameter(outValues);
            return dr;
        }

        public DataSet ToProcedureDataSet(SprocEntity criteria)
        {
            Dictionary<string, object> outValues;
            var dr = FindDataSet(criteria, out outValues);
            criteria.InitialOutParameter(outValues);
            return dr;
        }

        public DataTable ToProcedureDataTable(SprocEntity criteria)
        {
            Dictionary<string, object> outValues;
            var dr = FindDataSet(criteria, out outValues);
            criteria.InitialOutParameter(outValues);
            return dr.Tables[0];
        }

        #endregion

        #region Scalar

        public object ToScalar(QueryCriteria criteria)
        {
            DbCommand cmd = PrepareCommand(criteria);
            if (criteria.Tran == null)
            {
                return Db.ExecuteScalar(cmd);
            }
            else
            {
                return Db.ExecuteScalar(cmd, criteria.Tran);
            }
        }

        public object ToScalar(Type returnType, QueryCriteria criteria)
        {
            object retValue = ToScalar(criteria);

            if (retValue == null || retValue == DBNull.Value)
                return CommonUtils.DefaultValue(returnType);

            if (returnType == typeof(Guid))
            {
                return DataUtils.ToGuid(ToScalar(criteria));
            }

            return Convert.ChangeType(retValue, returnType);
        }

        public TReturnType ToScalar<TReturnType>(QueryCriteria criteria)
        {
            return (TReturnType)ToScalar(typeof(TReturnType), criteria);
        }

        public object ToStoredScalar(QueryCriteria criteria)
        {
            IDataReader reader = FindDataReader(criteria);
            object retObj = null;
            if (reader.Read())
            {
                retObj = reader.GetValue(0);
            }
            reader.Close();
            reader.Dispose();
            return retObj;
        }

        public object ToStoredScalar(QueryCriteria criteria, out Dictionary<string, object> outValues)
        {
            DataSet ds = FindDataSet(criteria, out outValues);
            object retObj = null;
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                retObj = ds.Tables[0].Rows[0][0];
            }
            ds.Dispose();
            return retObj;
        }

        #endregion

        #region DataSet DataTable DataReader

        public IDataReader ToDataReader<TEntity>()
        {
            TableSchema table = DBQueryFactory.GetTableSchema(typeof(TEntity));
            if (table != null)
            {
                var query = CreateQuery(table.TableName);
                return FindDataReader(query);
            }
            return null;
        }

        public IDataReader ToDataReader(QueryCriteria criteria)
        {
            return FindDataReader(criteria);
        }

        public IDataReader ToDataReader(QueryCriteria criteria, out Dictionary<string, object> outValues)
        {
            return FindDataReader(criteria, out outValues);
        }

        public DataSet ToDataSet(QueryCriteria criteria)
        {
            return FindDataSet(criteria);
        }

        public DataTable ToDataTable<TEntity>()
        {
            TableSchema table = DBQueryFactory.GetTableSchema(typeof(TEntity));
            if (table != null)
            {
                var query = CreateQuery(table.TableName);
                return FindDataSet(query).Tables[0];
            }
            return null;
        }

        public DataTable ToDataTable(QueryCriteria criteria)
        {
            return FindDataSet(criteria).Tables[0];
        }

        public DataTable ToDataTable(QueryCriteria criteria, out Dictionary<string, object> outValues)
        {
            var ds = ToDataSet(criteria, out outValues);
            return ds.Tables[0];
        }

        public DataSet ToDataSet(QueryCriteria criteria, out Dictionary<string, object> outValues)
        {
            return FindDataSet(criteria, out outValues);
        }

        #endregion

        #region List

        public IList<TEntity> ToList<TEntity>()
        {
            TableSchema table = DBQueryFactory.GetTableSchema(typeof(TEntity));
            if (table != null)
            {
                var query = CreateQuery(table.TableName);
                return ToList<TEntity>(query);
            }
            return null;
        }

        public IList<TEntity> ToList<TEntity>(QueryCriteria criteria)
        {
            return ConvertListUtil.DataToList<TEntity>(ToDataReader(criteria), criteria.TableName);
            //return (List<TEntity>) ConvertListUtil.DataToList(typeof(TEntity), ToDataReader(criteria),this, criteria.TableName);
        }

        internal System.Collections.IList ToList(Type type, QueryCriteria criteria)
        {
            return ConvertListUtil.DataToList(type, ToDataReader(criteria), this, criteria.TableName);
        }

        public IEnumerable<TEntity> ToIEnumerable<TEntity>()
        {
            TableSchema table = DBQueryFactory.GetTableSchema(typeof(TEntity));
            if (table != null)
            {
                var query = CreateQuery(table.TableName);
                return ToIEnumerable<TEntity>(query);
            }
            return null;
        }

        public IEnumerable<TEntity> ToIEnumerable<TEntity>(QueryCriteria criteria)
        {
            return ConvertListUtil.DataToIEnumerable<TEntity>(ToDataReader(criteria), criteria.TableName);
        }

        #endregion

        #region Entity

        public TEntity Load<TEntity>(TEntity entity) where TEntity : class
        {
            return ModelHelp.CreateProxy(entity);
        }

        public TEntity Load<TEntity>(object key) where TEntity : class
        {
            TableSchema table = DBQueryFactory.GetTableSchema(typeof(TEntity));
            if (table == null) throw new Exception("TEntity isn't BaseEntity");
            if (ReferenceEquals(table.IdentyColumn, null))
                throw new Exception("IdentyColumn Is Null");
            var query =
                CreateQuery(table.TableName).Where(table.IdentyColumn == key);
            return ToEntity<TEntity>(query);
        }

        public TEntity ToEntity<TEntity>(QueryCriteria criteria) where TEntity : class
        {
            var tag = ConvertListUtil.DataToEntity<TEntity>(ToDataReader(criteria), criteria.TableName);
            return ModelHelp.CreateProxy(tag);
        }

        public object ToEntity(Type type, QueryCriteria criteria)
        {
            var tag = ConvertListUtil.DataToEntity(type, ToDataReader(criteria), criteria.TableName);
            return ModelHelp.CreateProxy(type, tag);
        }

        public object ToEntityOrDefault(Type type, QueryCriteria criteria)
        {
            var tag = ConvertListUtil.DataToEntityOrDefault(type, ToDataReader(criteria), criteria.TableName);
            return ModelHelp.CreateProxy(tag);
        }

        public TEntity ToEntityOrDefault<TEntity>(QueryCriteria criteria) where TEntity : class
        {
            var tag = ConvertListUtil.DataToEntityOrDefault<TEntity>(ToDataReader(criteria), criteria.TableName);
            return ModelHelp.CreateProxy(tag);
        }

        #endregion

        #region Page
         

       
        public IEnumerable<TEntity> ToPageIEnumerable<TEntity>(SelectSqlSection selectsql, int pagesize, int currentpage, out int countpage)
        {
            TableSchema table = DBQueryFactory.GetTableSchema(typeof(TEntity));
            if (table == null) throw new Exception("TEntity isn't BaseEntity");
            return ToPageIEnumerable<TEntity>(selectsql, pagesize, currentpage, table.IdentyColumn, out countpage);
        }

        public IEnumerable<TEntity> ToPageIEnumerable<TEntity>(SelectSqlSection selectsql, int pagesize, int currentpage, QueryColumn key, out int countpage)
        {
            countpage = Count(selectsql);
            selectsql.SetSelectRange(pagesize, GetPages(pagesize, currentpage), key);
            var sql = GetSqlResultColumnText(selectsql);
            return ConvertListUtil.DataToIEnumerable<TEntity>(ToDataReader(selectsql), sql);
        }

        public IList<TEntity> ToPageList<TEntity>(SelectSqlSection selectsql, int pagesize, int currentpage, out int countpage)
        {
            TableSchema table = DBQueryFactory.GetTableSchema(typeof(TEntity));
            if (table == null) throw new Exception("TEntity isn't BaseEntity");
            return ToPageList<TEntity>(selectsql, pagesize, currentpage, table.IdentyColumn, out countpage);
        }

        public IList<TEntity> ToPageList<TEntity>(SelectSqlSection selectsql, int pagesize, int currentpage, QueryColumn key, out int countpage)
        {
            countpage = Count(selectsql);
            selectsql.SetSelectRange(pagesize, GetPages(pagesize, currentpage), key);
            var sql = GetSqlResultColumnText(selectsql);
            return ConvertListUtil.DataToList<TEntity>(ToDataReader(selectsql), sql);
        }

        public DataTable ToPageDataTable(SelectSqlSection selectsql, int pagesize, int currentpage, QueryColumn key, out int countpage)
        {
            countpage = Count(selectsql);
            selectsql.SetSelectRange(pagesize, GetPages(pagesize, currentpage), key);
            return ToDataTable(selectsql);
        }

        public DataTable ToPageDataTable<TEntity>(SelectSqlSection selectsql, int pagesize, int currentpage, out int countpage)
        {
            TableSchema table = DBQueryFactory.GetTableSchema(typeof(TEntity));
            if (table == null) throw new Exception("TEntity isn't BaseEntity");
            return ToPageDataTable(selectsql, pagesize, currentpage, table.IdentyColumn, out countpage);
        }

        #endregion

        #endregion

        #endregion

        #region BeginBatch

        private bool isBatch;

        private BatchCommander batchCommander;

        public void BeginBatch(int size)
        {
            isBatch = true;
            var tran = GetCurrentDbTransaction();
            if (tran == null)
            {
                batchCommander = new BatchCommander(Db, size);
            }
            else
            {
                batchCommander = new BatchCommander(Db, size, tran);
            }
        }

        public void BeginBatch(int size, DbTransaction tran)
        {
            isBatch = true;
            batchCommander = new BatchCommander(Db, size, tran);
        }

        public void BeginBatch(int size, IsolationLevel il)
        {
            isBatch = true;
            batchCommander = new BatchCommander(Db, size, il);
        }

        public void EndBatch()
        {
            isBatch = false;
            batchCommander.Close();
        }

        #endregion

        #region UnitOfWork

        public void BeginUnitOfWork()
        {            
            IsUnitOfWork = true;
        }

        [Obsolete("此方法已过时，请使用BeginUnitOfWork来设置启用工作单元")]
        public bool IsUnitOfWork
        {
            get { return Db.IsShareConnection; }
            set { Db.IsShareConnection = value; }
        }

        internal System.Collections.Concurrent.ConcurrentDictionary<QueryCriteria, DbQueryAndArgument> UnitOfWorkExecContext { get; set; }

        public virtual void Commit(bool localTransaction = true, bool isUnitOfWork = true)
        {            
            //先提交自己事务
            if (localTransaction && Transaction != null)
                Transaction.Commit();

            if (isUnitOfWork)
                Commit(System.Data.IsolationLevel.ReadCommitted);
        }

        public virtual void Rollback()
        {
            if (Transaction != null)
            {
                Transaction.Rollback();
            }
        }

        public virtual void Commit(System.Data.IsolationLevel isolationLevel)
        {
            //如果不是工作计划就return
            if (!IsUnitOfWork || UnitOfWorkExecContext.Count == 0)
            {
                return;
            }
            //添加自己的事务
            using (var tran = GetDbTransaction(isolationLevel))
            {
                try
                {
                    foreach (var dbQuery in UnitOfWorkExecContext)
                    {
                        DbCommand cmd = PrepareCommand(dbQuery.Key);
                        dbQuery.Value.DbQuery.Db.ExecuteNonQuery(cmd, tran);
                    }
                    tran.Commit();
                }
                catch (Exception e)
                {
                    tran.Rollback();
                    throw e;
                }
                finally { UnitOfWorkExecContext.Clear();
                //TODO 关闭 IsUnitOfWork 2013/7/3
                    IsUnitOfWork = false; }
            }
        }

        public virtual void Commit(bool isTransaction)
        {
            if (!IsUnitOfWork || UnitOfWorkExecContext.Count == 0)
            {
                return;
            }
            if (isTransaction)
            {
                Commit(false, true);
                return;
            }
            try
            {
                using (DbConnection connection = Db.GetConnection(true))
                {
                    foreach (var dbQuery in UnitOfWorkExecContext)
                    {
                        DbCommand cmd = PrepareCommand(dbQuery.Key);
                        dbQuery.Value.DbQuery.Db.ExecuteNonQuery(cmd, connection);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                ClearUnitOfWork();
                //TODO 关闭 IsUnitOfWork 2013/7/3
                IsUnitOfWork = false;
            }
        }

        #endregion

        #region IDispose

        public void Dispose()
        {
            ClearCache();
            Close();
            ClearUnitOfWork();
            GC.Collect();
        }

        public void ClearUnitOfWork()
        {
            UnitOfWorkExecContext.Clear();
        }

        public void Close(DbTransaction tran)
        {
            Db.CloseConnection(tran);
        }

        protected void Close(DbConnection conn)
        {
            Db.CloseConnection(conn);
        }

        protected void Close(DbCommand cmd)
        {
            Db.CloseConnection(cmd);
        }

        public void Close()
        {
            if (Transaction != null)
            {
                Close(Transaction);
                Transaction = null;
            }
        }

        #endregion

        #region ILogable 成员

        public event LogHandler OnLog;

        internal void WriteLog(QueryCriteria logMsg)
        {
            if (OnLog != null)
            {
                OnLog(logMsg.ToDbCommandText());
            }
        }

        public void LogWrite(Exception error)
        {
            Log.Write(error);
        }

        public void LogWrite(string error)
        {
            Log.Write(error);
        }

        protected IDbLog Log
        {
            get
            {
                if (Db.Logtype == LogType.File)
                    return new FileLog();
                return new DataBaseLog(this);
            }
        }

        #endregion

        #region Cache

        public int CacheSize { get; set; }

        protected System.Collections.Concurrent.ConcurrentDictionary<Type, NSun.Data.Common.LruDictionary<object, object>> _cacheContext =
            new System.Collections.Concurrent.ConcurrentDictionary<Type, Common.LruDictionary<object, object>>();

        protected void AddCache(object tag)
        {
            Type type = tag.GetType();
            object value = DBQueryFactory.GetPkValue(tag);
            LruDictionary<object, object> cachetype = _cacheContext.GetOrAdd(type, new LruDictionary<object, object>(CacheSize));
            cachetype.Add(value, tag);
        }

        protected void AddCache(IEnumerable<object> tags)
        {
            foreach (var tag in tags)
            {
                AddCache(tag);
            }
        }

        protected void RemoveCache(Type type, object value)
        {
            if (_cacheContext.ContainsKey(type))
            {
                LruDictionary<object, object> cachetype = _cacheContext[type];
                cachetype.Remove(value);
            }
        }

        protected void RemoveCache(Type type, object[] value)
        {
            if (_cacheContext.ContainsKey(type))
            {
                LruDictionary<object, object> cachetype = _cacheContext[type];
                foreach (var o in cachetype)
                {
                    RemoveCache(type, o);
                }
            }
        }

        protected object GetCache(Type type, object key)
        {
            if (_cacheContext.ContainsKey(type))
            {
                LruDictionary<object, object> cachetype = _cacheContext[type];
                var obj = cachetype[key];
                if (null != obj)
                {
                    return obj;
                }
            }
            return null;
        }

        protected virtual void ClearCache(Type type)
        {
            if (_cacheContext.ContainsKey(type))
            {
                var cachetype = _cacheContext[type];
                cachetype.Clear();
            }
            if (DBQueryFactory.IsCachDouble)
            {
                DBQueryFactory.ClearCacheDouble(type);
            }
        }

        public virtual void ClearCache()
        {
            _cacheContext.Clear();
            if (DBQueryFactory.IsCachDouble)
            {
                DBQueryFactory.ClearCacheDouble();
            }
        }

        #endregion
    }
}
