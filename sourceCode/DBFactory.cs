using NSun.Data;
namespace Simple
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
            _instance = new DBQueryFactory("db", SqlType.MySql);
            _instance.Options.IsUseRelation = true;
            _instance.Options.DynamicProxyType = ProxyType.Remoting;//Default
        }

        public static DBQuery<T> CreateDBQuery<T>(bool isClone = true) where T : class, IBaseEntity
        {
            return Instance.CreateDBQuery<T>(isClone);
        }

        public static DBQuery CreateDBQuery(bool isClone = true)
        {
            return Instance.CreateDBQuery(isClone);
        }
    }
}
