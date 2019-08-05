using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSun.Data;

namespace GeneratorV2.Service
{
    public class DBFactory
    { 
        private DBFactory() { } 

        public static DBQuery<T> CreateDBQuery<T>(SqlType sql,string connectionString)where T : class, IBaseEntity
        {
            return new DBQueryFactory(sql, connectionString).CreateDBQuery<T>();
        }

        public static DBQuery CreateDBQuery(SqlType sql, string connectionString)
        {
            return new DBQueryFactory(sql, connectionString).CreateDBQuery();
        } 
    }
}
