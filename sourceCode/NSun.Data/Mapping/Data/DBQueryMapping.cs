using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using NSun.Data.Common;

namespace NSun.Data
{
    public class DBQuery<T> : BaseDbQuery<T> where T : class,IBaseEntity
    { 
        #region Construction

        internal DBQuery(Database db)
            : base(db)
        {
            //_cache = new LruDictionary<object, T>(int.MaxValue);    
        }

        internal DBQuery(Database db, DBQueryFactory factory)
            : base(db, factory)
        {
            //_cache = new LruDictionary<object, T>(int.MaxValue); 
        }

        #endregion

        #region Exists

        public bool Exists(QueryColumn qcolumn, object value)
        {
            if (null != Transaction)
            {
                return Exists(qcolumn, value, Transaction);
            }
            else if (null != NSun.Data.Transactions.Transaction.Current)
            {
                return Exists(qcolumn, value,
                              NSun.Data.Transactions.Transaction.Current.DbTransactionWrapper.DbTransaction);
            }
            return Exists(qcolumn, value, null);
        }

        public bool Exists(QueryColumn qcolumn, object value, DbTransaction tran)
        {
            SelectSqlSection selectsql = CreateQuery();
            selectsql.Where(qcolumn == value);
            if (tran != null)
                return base.Exists(selectsql);
            return base.Exists(selectsql);
        }

        public bool ExistsKey(object value)
        {
            return ExistsKey(value, null);
        }

        public bool ExistsKey(object value, DbTransaction tran)
        {
            return Exists(Table.IdentyColumn, value, tran);
        }

        #endregion

        #region Save Update Delete

        public int Save(T tag)
        {
            return Save(tag, null);
        }

        //Cache Add
        public int Save(T tag, DbTransaction tran)
        {
            int res;
            bool isPersistence = tag.IsPersistence();
            if (!isPersistence)
            {
                InsertSqlSection insertsql = CreateInsert();
                if (tran != null)
                {
                    insertsql.SetTransaction(tran);
                }
                res = InsertQueryColumn(insertsql, tag);
            }
            else
            {
                UpdateSqlSection updatesql = CreateUpdate();
                if (tran != null)
                {
                    updatesql.SetTransaction(tran);
                }
                res = UpdateQueryColumn(updatesql, tag);
            }
            AddCache(tag);
            DBQueryFactory.AddCacheDouble(tag);
            return res;
        }

        public int Update(UpdateSqlSection updatesql, T tag)
        {
            return Update(updatesql, tag, null);
        }
        //Cache Add
        public int Update(UpdateSqlSection updatesql, T tag, DbTransaction tran)
        {
            updatesql.SetTransaction(tran);
            if (IsUnitOfWork)
            {
                UnitOfWorkExecContext.TryAdd(updatesql, new DbQueryAndArgument() { DbQuery = this, Argument = tag });
                return -1;
            }
            int res = UpdateQueryColumn(updatesql, tag);
            AddCache(tag);
            DBQueryFactory.AddCacheDouble(tag);
            return res;
        }

        public int Delete(T tag)
        {
            return Delete(tag, null);
        }
        //Cache Remove
        public int Delete(T tag, DbTransaction tran)
        {
            DeleteSqlSection deletesql = CreateDelete();
            object value = ReflectionUtils.GetFieldValue(tag, Table.IdentyFieldMemberInfo);
            if (value == null)
            {
                return -1;
            }
            if (tran != null)
            {
                deletesql.SetTransaction(tran);
            }

            #region OptimisticLocking

            foreach (var queryColumn in Table.OptimisticLocking)
            {
                PropertyInfo pi = null;
                foreach (var item in Table.ValuePropertys)
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
                        deletesql.Where(queryColumn.Value == entityFieldValue);
                }
            }

            #endregion

            deletesql.Where(Table.IdentyColumn == value);

            tag.SetIsPersistence(false);
            if (IsUnitOfWork)
            {
                UnitOfWorkExecContext.TryAdd(deletesql, new DbQueryAndArgument() { DbQuery = this, Argument = tag });
                return -1;
            }

            int res = ToExecute(deletesql);

            RemoveCache(value);
            DBQueryFactory.RemoveCacheDouble(typeof(T), value);

            if (res == 0 && Table.OptimisticLocking.Count > 0)
            {
                throw new Exception("object non-existent or delete yet");
            }

            return res;
        }

        public int DeleteKey(object key)
        {
            return DeleteKey(key, null);
        }
        //Cache Remove
        public int DeleteKey(object key, DbTransaction tran)
        {
            DeleteSqlSection deletesql = CreateDelete();
            if (Table.IdentyColumn == null)
            {
                return -1;
            }
            if (tran != null)
            {
                deletesql.SetTransaction(tran);
            }
            deletesql.Where(Table.IdentyColumn == key);
            if (IsUnitOfWork)
            {
                UnitOfWorkExecContext.TryAdd(deletesql, new DbQueryAndArgument() { DbQuery = this, Argument = key });
                return -1;
            }

            int res = ToExecute(deletesql);

            RemoveCache(key);
            DBQueryFactory.RemoveCacheDouble(typeof(T), key);

            return res;
        }

        public int Delete(object[] keys)
        {
            return Delete(keys, null);
        }
        //Cache Remove
        public int Delete(object[] keys, DbTransaction tran)
        {
            DeleteSqlSection deletesql = CreateDelete();
            if (tran != null)
            {
                deletesql.SetTransaction(tran);
            }
            if (Table.IdentyColumn == null)
            {
                return -1;
            }
            deletesql.Where(Table.IdentyColumn.In(keys));
            if (IsUnitOfWork)
            {
                UnitOfWorkExecContext.TryAdd(deletesql, new DbQueryAndArgument() { DbQuery = this, Argument = keys });
                return -1;
            }
            int res = ToExecute(deletesql);

            RemoveCache(keys);
            DBQueryFactory.RemoveCacheDouble(typeof(T), keys);

            return res;
        }

        public int Delete(DeleteSqlSection deletesql)
        {
            return Delete(deletesql, null);
        }

        public int Delete(DeleteSqlSection deletesql, DbTransaction tran)
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

        #endregion

        #region Count

        public int Count()
        {
            var select = CreateQuery();
            return Count(select);
        }

        public int Count(DbTransaction tran)
        {
            var select = CreateQuery(tran);
            return Count(select);
        }

        public long CountLong()
        {
            var select = CreateQuery();
            return CountLong(select);
        }

        public long CountLong(DbTransaction tran)
        {
            var select = CreateQuery(tran);
            return CountLong(select);
        }

        #endregion

        #region Page Function

        #region Not SelectSqlSection

        public DataTable ToPageDataTable(int pagesize, int currentpage, out int countpage)
        {
            SelectSqlSection selectsql = CreateQuery();
            return ToPageDataTable(selectsql, pagesize, currentpage, out countpage);
        }

        public DataTable ToPageDataTable(int pagesize, int currentpage, out int countpage, DbTransaction tran)
        {
            SelectSqlSection selectsql = CreateQuery(tran);
            return ToPageDataTable(selectsql, pagesize, currentpage, out countpage);
        }

        public IEnumerable<T> ToPageIEnumerable(int pagesize, int currentpage, out int countpage)
        {
            SelectSqlSection selectsql = CreateQuery();
            return ToPageIEnumerable(selectsql, pagesize, currentpage, out countpage);
        }

        public IEnumerable<T> ToPageIEnumerable(int pagesize, int currentpage, out int countpage, DbTransaction tran)
        {
            SelectSqlSection selectsql = CreateQuery(tran);
            return ToPageIEnumerable(selectsql, pagesize, currentpage, out countpage);
        }

        public IList<T> ToPageList(int pagesize, int currentpage, out int countpage)
        {
            SelectSqlSection selectsql = CreateQuery();
            return ToPageList(selectsql, pagesize, currentpage, out countpage);
        }

        public IList<T> ToPageList(int pagesize, int currentpage, out int countpage, DbTransaction tran)
        {
            SelectSqlSection selectsql = CreateQuery(tran);
            return ToPageList(selectsql, pagesize, currentpage, out countpage);
        }

        #endregion

        public DataTable ToPageDataTable(SelectSqlSection selectsql, int pagesize, int currentpage, out int countpage)
        {
            return base.ToPageDataTable<T>(selectsql, pagesize, currentpage, out countpage);
        }

        public IEnumerable<T> ToPageIEnumerable(SelectSqlSection selectsql, int pagesize, int currentpage, out int countpage)
        {
            return base.ToPageIEnumerable<T>(selectsql, pagesize, currentpage, out countpage);
        }


        public new IEnumerable<T> ToPageIEnumerable<T>(SelectSqlSection selectsql, int pagesize, int currentpage, out int countpage)
        {
            return base.ToPageIEnumerable<T>(selectsql, pagesize, currentpage, out countpage);
        }

        public IList<T> ToPageList(SelectSqlSection selectsql, int pagesize, int currentpage, out int countpage)
        {
            return base.ToPageList<T>(selectsql, pagesize, currentpage, out countpage);
        }

        public new IList<T> ToPageList<T>(SelectSqlSection selectsql, int pagesize, int currentpage, out int countpage)
        {
            return base.ToPageList<T>(selectsql, pagesize, currentpage, out countpage);
        }

        #endregion

        #region Entity

        public T Get(object key)
        {
            return Get(key, null);
        }

        public T Get(object key, DbTransaction tran)
        {
            if (object.ReferenceEquals(Table.IdentyColumn, null))
                throw new Exception("IdentyColumn Is Null");
            SelectSqlSection select = CreateQuery(tran).Where(Table.IdentyColumn == key);
            return ToEntity(select);
        }

        /// <summary>
        /// Cache Load
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Load(object key)
        {
            return Load(key, null);
        }

        public T Load(T entity)
        {
            return Load<T>(entity);
        }

        public T Load(object key, DbTransaction tran)
        {
            if (object.ReferenceEquals(Table.IdentyColumn, null))
                throw new Exception("IdentyColumn Is Null");
            var tag = GetCache(key);
            if (tag == null)
            {
                tag = DBQueryFactory.GetCacheDouble(typeof(T), key) as T;
                if (tag == null)
                {
                    SelectSqlSection select = CreateQuery(tran).Where(Table.IdentyColumn == key);
                    tag = ToEntity(select);

                    AddCache(tag);
                    DBQueryFactory.AddCacheDouble(tag);
                }
            }
            return tag;
        }

        /// <summary>
        /// Not Cache
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public T ToEntity(QueryCriteria criteria)
        {
            var t = ToEntity<T>(criteria);
            return t;
        }

        public T LoadOrDefault(object key)
        {
            return LoadOrDefault(key, null);
        }

        public T LoadOrDefault(object key, DbTransaction tran)
        {
            if (object.ReferenceEquals(Table.IdentyColumn, null))
                throw new Exception("IdentyColumn Is Null");
            var tag = GetCache(key);
            if (tag == null)
            {
                tag = DBQueryFactory.GetCacheDouble(typeof(T), key) as T;
                if (tag == null)
                {
                    SelectSqlSection select = CreateQuery(tran).Where(Table.IdentyColumn == key);
                    tag = ToEntityOrDefault(select);
                    if (tag != null)
                    {
                        AddCache(tag);
                        DBQueryFactory.AddCacheDouble(tag);
                    }
                }
            }
            return tag;
        }

        public T ToEntityOrDefault(QueryCriteria criteria)
        {
            var t = ToEntityOrDefault<T>(criteria);
            return t;
        }

        #endregion

        #region List

        public IDataReader ToDataReader()
        {
            var query = CreateQuery();
            return ToDataReader(query);
        }

        public DataSet ToDataSet()
        {
            var query = CreateQuery();
            return ToDataSet(query);
        }

        public DataTable ToDataTable()
        {
            var query = CreateQuery();
            return ToDataTable(query);
        }

        public IList<T> ToList()
        {
            var query = CreateQuery();
            return ToList(query);
        }

        public IList<T> ToList(QueryCriteria criteria)
        {
            return ToList<T>(criteria);
        }

        public IEnumerable<T> ToIEnumerable()
        {
            var query = CreateQuery();
            return ToIEnumerable(query);
        }

        public IEnumerable<T> ToIEnumerable(QueryCriteria criteria)
        {
            var list = ToIEnumerable<T>(criteria);
            return list;
        }

        #endregion
    }
}
