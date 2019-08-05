
using NSun.Data;

namespace StudentService
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
