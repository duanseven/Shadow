using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSun.Data;

namespace NSunLiteTest.Lambda
{
    [Table("USERS")]
    public class UsersEntity : BaseEntity
    {
        [PrimaryKey(true)] 
        public Int64 Id { get; set; } 

        public string Name { get; set; }

        public string Pass { get; set; }

        public DateTime? LoginTime { get; set; }

        public static QueryColumn __id = new QueryColumn("USERS.ID", System.Data.DbType.Int64); 

        public static QueryColumn __name = new QueryColumn("USERS.NAME", System.Data.DbType.String);

        public static QueryColumn __pass = new QueryColumn("USERS.PASS", System.Data.DbType.String);

        public static QueryColumn __logintime = new QueryColumn("USERS.LOGINTIME", System.Data.DbType.DateTime);
    }

    [Table("USERS")]
    public class UsersInfo : BaseEntity
    {
        [PrimaryKey(true)]        
        public Int64 Id { get; set; }

        public string Name { get; set; }

        public string Pass { get; set; }

        public DateTime? LoginTime { get; set; }
    }
}
