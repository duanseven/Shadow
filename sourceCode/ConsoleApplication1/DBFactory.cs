using NSun.Data;
namespace ConsoleApplication1
{
    public class DBFactory
    {
        private static readonly DBQueryFactory _instance;
        public static DBQueryFactory Instance
        {
            get { return _instance; }
        }
        private DBFactory() { }

        static DBFactory()
        {            
            _instance = new DBQueryFactory("db", SqlType.Sqlserver9);
            _instance.Options.IsUseRelation = true;
        }

        public static DBQuery<T> CreateDBQuery<T>() where T :class, IBaseEntity
        {
            return Instance.CreateDBQuery<T>();
        }

        public static DBQuery CreateDBQuery()
        {
            return Instance.CreateDBQuery();
        }
    }

    public class DBFactory2
    {
        private static readonly DBQueryFactory _instance;
        public static DBQueryFactory Instance
        {
            get { return _instance; }
        }
        private DBFactory2() { }

        static DBFactory2()
        {
            _instance = new DBQueryFactory("db", SqlType.Sqlserver9);
            _instance.Options.IsUseRelation = true;
            _instance.Options.DynamicProxyType = ProxyType.Castle;
        }

        public static DBQuery<T> CreateDBQuery<T>() where T : class, IBaseEntity
        {
            return Instance.CreateDBQuery<T>();
        }

        public static DBQuery CreateDBQuery()
        {
            return Instance.CreateDBQuery();
        }
    }
}