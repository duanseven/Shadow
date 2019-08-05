using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;

namespace NSun.Data
{
    public static class StoredExpand
    {
        #region StoredProcedure

        #region Entity Object

        public static int ToExecute(this StoredProcedureSection stored)
        {
            return ToExecute(stored, null);
        }

        public static int ToExecute(this StoredProcedureSection stored, DbTransaction tran)
        {
            DBQuery db = new DBQuery(stored.Db);
            if (tran != null)
            {
                stored.SetTransaction(tran);
            }
            return db.ToExecute(stored);
        }

        public static int ToExecute(this StoredProcedureSection stored, out Dictionary<string, object> outValues)
        {
            return ToExecute(stored, null, out outValues);
        }

        public static int ToExecute(this StoredProcedureSection stored, DbTransaction tran, out Dictionary<string, object> outValues)
        {
            DBQuery db = new DBQuery(stored.Db);
            if (tran != null)
            {
                stored.SetTransaction(tran);
            }
            return db.ToExecute(stored, out outValues);
        }

        public static T ToEntity<T>(this StoredProcedureSection stored) where T :class, IBaseEntity
        {
            DBQuery db = new DBQuery(stored.Db);
            return db.ToEntity<T>(stored);
        }
         
        public static T ToEntity<T>(this StoredProcedureSection stored, out Dictionary<string, object> outValues) where T : class,IBaseEntity
        {
            return ToEntity<T>(stored, null, out outValues);
        }

        public static T ToEntity<T>(this StoredProcedureSection stored, DbTransaction tran, out Dictionary<string, object> outValues) where T : class,IBaseEntity
        {
            DBQuery db = new DBQuery(stored.Db);
            var dr = db.ToDataReader(stored, out outValues);
            return ConvertListUtil.DataToEntity<T>(dr, stored.TableName);
        }

        public static object ToScalar(this StoredProcedureSection stored)
        {
            return ToScalar(stored, null);
        }

        public static object ToScalar(this StoredProcedureSection stored, DbTransaction tran)
        {
            DBQuery db = new DBQuery(stored.Db);
            if (tran != null)
            {
                stored.SetTransaction(tran);
            }
            return db.ToStoredScalar(stored);
        }

        public static object ToScalar(this StoredProcedureSection stored, out Dictionary<string, object> outValues)
        {
            return ToScalar(stored, null, out outValues);
        }

        public static object ToScalar(this StoredProcedureSection stored, DbTransaction tran, out Dictionary<string, object> outValues)
        {
            DBQuery db = new DBQuery(stored.Db);
            if (tran != null)
            {
                stored.SetTransaction(tran);
            }
            return db.ToStoredScalar(stored, out outValues);
        }


        #endregion

        #region DataTable DataSet

        public static DataTable ToDataTable(this StoredProcedureSection stored)
        {
            return ToDataTable(stored, null);
        }

        public static DataTable ToDataTable(this StoredProcedureSection stored, DbTransaction tran)
        {
            DBQuery db = new DBQuery(stored.Db);
            if (tran != null)
            {
                stored.SetTransaction(tran);
            }
            return db.ToDataTable(stored);
        }

        public static DataTable ToDataTable(this StoredProcedureSection stored, out Dictionary<string, object> outValues)
        {
            return ToDataTable(stored, null, out outValues);
        }

        public static DataTable ToDataTable(this StoredProcedureSection stored, DbTransaction tran, out Dictionary<string, object> outValues)
        {
            DBQuery db = new DBQuery(stored.Db);
            if (tran != null)
            {
                stored.SetTransaction(tran);
            }
            return db.ToDataTable(stored, out outValues);
        }

        public static DataSet ToDataSet(this StoredProcedureSection stored)
        {
            return ToDataSet(stored, null);
        }

        public static DataSet ToDataSet(this StoredProcedureSection stored, DbTransaction tran)
        {
            DBQuery db = new DBQuery(stored.Db);
            if (tran != null)
            {
                stored.SetTransaction(tran);
            }
            return db.ToDataSet(stored);
        }

        public static DataSet ToDataSet(this StoredProcedureSection stored, out Dictionary<string, object> outValues)
        {
            return ToDataSet(stored, null, out outValues);
        }

        public static DataSet ToDataSet(this StoredProcedureSection stored, DbTransaction tran, out Dictionary<string, object> outValues)
        {
            DBQuery db = new DBQuery(stored.Db);
            if (tran != null)
            {
                stored.SetTransaction(tran);
            }
            return db.ToDataSet(stored, out outValues);
        }

        #endregion

        #region List

        public static IList<T> ToList<T>(this StoredProcedureSection stored) where T : class,IBaseEntity
        {
            DBQuery db = new DBQuery(stored.Db);
            return db.ToList<T>(stored);
        }
 
        #endregion

        #region DataReader

        public static IDataReader ToDataReader(this StoredProcedureSection stored)
        {
            return ToDataReader(stored, null);
        }

        public static IDataReader ToDataReader(this StoredProcedureSection stored, DbTransaction tran)
        {
            DBQuery db = new DBQuery(stored.Db);
            if (tran != null)
            {
                stored.SetTransaction(tran);
            }
            return db.ToDataReader(stored);
        }

        #endregion

        #region IEnumerable

        public static IEnumerable<T> ToIEnumerable<T>(this StoredProcedureSection stored) where T :class, IBaseEntity
        {
            DBQuery db = new DBQuery(stored.Db);
            return db.ToIEnumerable<T>(stored);
        } 

        #endregion

        #endregion
    }
}
