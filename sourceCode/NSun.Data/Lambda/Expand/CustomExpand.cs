using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace NSun.Data
{
    public static class CustomExpand
    {
        #region DataTable DataSet

        public static DataTable ToDataTable(this CustomSqlSection customsql)
        {
            return ToDataTable(customsql, null);
        }

        public static DataTable ToDataTable(this CustomSqlSection customsql, DbTransaction tran)
        {
            return ToDataSet(customsql, tran).Tables[0];
        }

        public static DataSet ToDataSet(this CustomSqlSection customsql)
        {
            return ToDataSet(customsql, null);
        }

        public static DataSet ToDataSet(this CustomSqlSection customsql, DbTransaction tran)
        {
            var db = new DBQuery(customsql.Db);
            if (tran != null)
                customsql.SetTransaction(tran);
            return db.ToDataSet(customsql);
        }

        #endregion

        #region List

        public static IList<T> ToList<T>(this CustomSqlSection customsql)
        {
            var db = new DBQuery(customsql.Db);
            return db.ToList<T>(customsql);
        }
         
        #endregion

        #region Entity Object

        public static T ToEntity<T>(this CustomSqlSection customsql) where T :class
        {
            DBQuery db = new DBQuery(customsql.Db);
            return db.ToEntity<T>(customsql);
        }
         
        public static T ToEntityOrDefault<T>(this CustomSqlSection customsql) where T :class, IBaseEntity
        {
            T t = ToEntity<T>(customsql);
            return t.IsPersistence() ? t : default(T);
        }

        public static object ToScalar(this CustomSqlSection customsql)
        {
            DBQuery db = new DBQuery(customsql.Db); 
            return db.ToScalar(customsql);
        } 

        public static int ToExecute(this CustomSqlSection customsql)
        {
            DBQuery db = new DBQuery(customsql.Db); 
            return db.ToExecute(customsql);
        }
         
        #endregion

        #region DataReader

        public static IDataReader ToDataReader(this CustomSqlSection customsql)
        {
            return ToDataReader(customsql, null);
        }

        public static IDataReader ToDataReader(this CustomSqlSection customsql, DbTransaction tran)
        {
            DBQuery db = new DBQuery(customsql.Db);
            if (tran != null)
            {
                customsql.SetTransaction(tran);
            }
            return db.ToDataReader(customsql);
        }
        #endregion

        #region IEnumerable

        public static IEnumerable<T> ToIEnumerable<T>(this CustomSqlSection customsql)
        {
            return ToIEnumerable<T>(customsql, null);
        }

        public static IEnumerable<T> ToIEnumerable<T>(this CustomSqlSection customsql, DbTransaction tran)
        {
            DBQuery db = new DBQuery(customsql.Db);
            if (tran != null)
                customsql.SetTransaction(tran);
            return db.ToIEnumerable<T>(customsql);
        }

        #endregion

    }
}
