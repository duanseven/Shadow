
using NSun.Data;

namespace ServiceShool
{
    public class DBFactory
    {
        private static readonly DBQueryFactory _instance;

        public static DBQueryFactory Instance
        {
            get { return _instance; }
        }

        static DBFactory()
        {
            _instance = new DBQueryFactory("msdb", SqlType.Sqlserver9);
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
