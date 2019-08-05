using System;
using System.Collections.Generic; 
using NSun.Data;

//using NSunLite;

namespace NSunLiteTest
{
    public class Users : IQueryTable
    {
        public static QueryColumn Id = new QueryColumn("Users.ID", System.Data.DbType.Int32);
        public static QueryColumn Name = new QueryColumn("Users.Name", System.Data.DbType.String);
        public static QueryColumn Pass = new QueryColumn("Users.Pass", System.Data.DbType.String);
        public static QueryColumn Data = new QueryColumn("Users.Logintime", System.Data.DbType.DateTime);

        #region IQueryTable 成员

        public string GetTableName()
        {
            return "Users";
        }

        public List<IColumn> GetPredefinedColumns()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class Us : IQueryTable
    {
        public static QueryColumn Id = new QueryColumn("Us.ID", System.Data.DbType.Int32);
        public static QueryColumn Name = new QueryColumn("Us.Name", System.Data.DbType.String);
        public static QueryColumn Pass = new QueryColumn("Us.Pass", System.Data.DbType.String);
        public static QueryColumn Data = new QueryColumn("Us.Data", System.Data.DbType.DateTime);

        #region IQueryTable 成员

        public string GetTableName()
        {
            return "Us";
        }

        public List<IColumn> GetPredefinedColumns()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}