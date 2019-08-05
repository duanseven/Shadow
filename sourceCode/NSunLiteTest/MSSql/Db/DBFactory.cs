using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSun.Data;

namespace NSunLiteTest.MSSql.Db
{
    public static class DBFactory
    {
        public static DBQuery<Hz_qyhznr> Hz_qyhznr;
        public static DBQuery<UsersInfo> dbUsers;

        private static Database db;

        static DBFactory()
        {
            db = new Database("db", SqlType.Sqlserver9);

            Hz_qyhznr = DBFactory.CreateQuery<Hz_qyhznr>();// new DBQuery<Hz_qyhznr>(db);
            dbUsers = DBFactory.CreateQuery<UsersInfo>();// new DBQuery<UsersInfo>(db);
        }

        public static DBQuery<T> CreateQuery<T>() where T : BaseEntity
        {
            return DBFactory.CreateQuery<T>();// new DBQuery<T>(db);
        }

        public static DBQuery<T> CreateQuery<T>(Database refdb) where T : BaseEntity
        {
            return DBFactory.CreateQuery<T>(refdb);///new DBQuery<T>(refdb);
        }
    }
}
