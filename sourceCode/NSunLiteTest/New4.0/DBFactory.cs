using NSun.Data;
namespace NSunLiteTest
{
	public class DBFactoryNew //: DbFactoryAbst
	{
        //private static readonly DBFactoryNew _instance = new DBFactoryNew();

        //public static DBFactoryNew Instance
        //{
        //    get { return _instance; }
        //}

        //private DBFactoryNew() { }

        //private static Database db;

        //static DBFactoryNew()
        //{
        //    db = Database.Default;
        //}

        //public override Database GetDatabase()
        //{
        //    return db;
        //}

         private static readonly DBQueryFactory _instance;
        public static DBQueryFactory Instance
        {
            get { return _instance; }
        }
        private DBFactoryNew() { }

        static DBFactoryNew()
        {
            //DBQueryFactory.IsCachDouble = true;
            _instance = new DBQueryFactory("db3", SqlType.MySql);            
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
	}
}