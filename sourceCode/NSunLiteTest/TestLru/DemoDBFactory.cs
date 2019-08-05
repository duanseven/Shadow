using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSun.Data;

namespace NSunLiteTest.TestLru
{
    public class DBFactory //: DbFactoryAbst
    {
        private static readonly DBQueryFactory _instance;
        public static DBQueryFactory Instance
        {
            get { return _instance; }
        }
        private DBFactory() { }

        static DBFactory()
        {
            //DBQueryFactory.IsCachDouble = true;
            _instance = new DBQueryFactory("db3", SqlType.Sqlserver9);
            _instance.Options.IsUseRelation = true;
            _instance.Options.DynamicProxyType = ProxyType.Remoting;
        }

        public static DBQuery<T> CreateDBQuery<T>() where T : class,IBaseEntity
        {
            return Instance.CreateDBQuery<T>();
        }

        public static DBQuery CreateDBQuery()
        {
            return Instance.CreateDBQuery();
        }
        //private static readonly DBFactory _instance = new DBFactory();

        //public static DBFactory Instance
        //{
        //    get
        //    {
        //        return _instance;
        //    }
        //}

        //private DBFactory() { }

        //private static Database db;

        //static DBFactory()
        //{
        //    db = Database.Default;
        //}

        //public override Database GetDatabase()
        //{
        //    return db;
        //}
    }
}
