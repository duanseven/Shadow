using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data; 

namespace NSun.Data
{
    public static class SelectExpand
    {
        #region Exists

        public static bool Exists(this SelectSqlSection select)
        {
            DBQuery db = new DBQuery(select.Db);
            return db.Exists(select);
        }

        #endregion

        #region Count

        public static long CountLong(this SelectSqlSection selectsql)
        {
            DBQuery db = new DBQuery(selectsql.Db);
            return db.CountLong(selectsql);
        }

        public static int Count(this SelectSqlSection selectsql)
        {
            DBQuery db = new DBQuery(selectsql.Db);
            return db.Count(selectsql);
        }

        #endregion

        #region Select

        #region Entity Object

        public static object ToScalar(this SelectSqlSection selectsql)
        {
            return ToScalar(selectsql, null);
        }

        public static object ToScalar(this SelectSqlSection selectsql, DbTransaction tran)
        {
            DBQuery db = new DBQuery(selectsql.Db);
            selectsql.SetTransaction(tran);
            return db.ToScalar(selectsql); 
        }

        public static T ToEntity<T>(this SelectSqlSection<T> selectsql) where T : class,IBaseEntity
        {
            return ToEntity(selectsql, null);
        }

        public static T ToEntity<T>(this SelectSqlSection<T> selectsql, DbTransaction tran) where T : class,IBaseEntity
        {
            DBQuery db = new DBQuery(selectsql.Db);
            selectsql.SetTransaction(tran);
            return db.ToEntity<T>(selectsql);            
        }

        public static T ToEntityOrNull<T>(this SelectSqlSection<T> selectsql) where T : class,IBaseEntity
        {
            return ToEntity(selectsql, null).IsPersistence() ? ToEntity(selectsql, null) : null;
        }

        public static T ToEntityOrNull<T>(this SelectSqlSection<T> selectsql, DbTransaction tran) where T : class,IBaseEntity
        {
            return ToEntity(selectsql, tran).IsPersistence() ? ToEntity(selectsql, tran) : null;
        }

        public static T ToEntity<T>(this SelectSqlSection selectsql) where T : class,IBaseEntity
        {
            return ToEntity<T>(selectsql, null);
        }

        public static T ToEntity<T>(this SelectSqlSection selectsql, DbTransaction tran) where T : class,IBaseEntity
        {
            DBQuery db = new DBQuery(selectsql.Db);
            selectsql.SetTransaction(tran);
            return db.ToEntity<T>(selectsql);
        }

        public static T ToEntityOrNull<T>(this SelectSqlSection selectsql) where T : class,IBaseEntity
        {
            return ToEntity<T>(selectsql, null).IsPersistence() ? ToEntity<T>(selectsql, null) : null;
        }

        public static T ToEntityOrNull<T>(this SelectSqlSection selectsql, DbTransaction tran) where T : class,IBaseEntity
        {
            return ToEntity<T>(selectsql, tran).IsPersistence() ? ToEntity<T>(selectsql, tran) : null;
        }

        #endregion

        #region List

        public static IList<T> ToList<T>(this SelectSqlSection selectsql)
        {
            DBQuery db = new DBQuery(selectsql.Db);
            return db.ToList<T>(selectsql);
        }

        public static IList<T> ToList<T>(this SelectSqlSection<T> selectsql) where T : class,IBaseEntity
        {
            DBQuery db = new DBQuery(selectsql.Db);
            return db.ToList<T>(selectsql);
        } 
        #endregion

        #region DataTable

        public static DataTable ToDataTable(this SelectSqlSection selectsql)
        {
            return ToDataTable(selectsql, null);
        }

        public static DataTable ToDataTable(this SelectSqlSection selectsql, DbTransaction tran)
        {
            DBQuery db = new DBQuery(selectsql.Db);
            selectsql.SetTransaction(tran);
            return db.ToDataTable(selectsql);
        }

        #endregion

        #region IEnumerable

        public static IEnumerable<T> ToIEnumerable<T>(this SelectSqlSection<T> selectsql) where T : class,IBaseEntity
        {
            return ToIEnumerable(selectsql, null);
        }

        public static IEnumerable<T> ToIEnumerable<T>(this SelectSqlSection<T> selectsql, DbTransaction tran) where T : class,IBaseEntity
        {
            DBQuery db = new DBQuery(selectsql.Db);
            selectsql.SetTransaction(tran);
            return db.ToIEnumerable<T>(selectsql);
        }

        public static IEnumerable<T> ToIEnumerable<T>(this SelectSqlSection selectsql)
        {
            return ToIEnumerable<T>(selectsql, null);
        }

        public static IEnumerable<T> ToIEnumerable<T>(this SelectSqlSection selectsql, DbTransaction tran)
        {
            DBQuery db = new DBQuery(selectsql.Db);
            selectsql.SetTransaction(tran);
            return db.ToIEnumerable<T>(selectsql);
        }

        #endregion

        #region Page Function

        public static DataTable ToPageDataTable<T>(this SelectSqlSection<T> selectsql, int pagesize, int currentpage, out int countpage) where T : class,IBaseEntity
        {
            DBQuery<T> db = new DBQuery<T>(selectsql.Db);
            return db.ToPageDataTable(selectsql, pagesize, currentpage,out countpage);
        }

        public static IList<T> ToPageList<T>(this SelectSqlSection<T> selectsql, int pagesize, int currentpage, out int countpage) where T : class,IBaseEntity
        {
            DBQuery<T> db = new DBQuery<T>(selectsql.Db);
            return db.ToPageList(selectsql, pagesize, currentpage, out countpage);
        }

        public static IEnumerable<T> ToPageIEnumerable<T>(this SelectSqlSection<T> selectsql, int pagesize, int currentpage, out int countpage) where T : class,IBaseEntity
        {
            DBQuery<T> db = new DBQuery<T>(selectsql.Db);
            return db.ToPageIEnumerable(selectsql, pagesize, currentpage, out countpage);
        }

        #endregion

        #endregion
    }
}
