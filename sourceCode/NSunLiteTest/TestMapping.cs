using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NSun.Data;

//using NSunLite;

namespace NSunLiteTest
{
    [Table("USERS")]
    public class UsersInfo : BaseEntity
    {
        [PrimaryKey(true)]
        public Int32 Id { get; set; } 
        //public decimal Id { get; set; }//oracle
        //public Int64 Id { get; set; } //mssql
                
        public string Name { get; set; }

        public string Pass { get; set; }

        public DateTime? LoginTime { get; set; }

        //public static Column __id = new Column("ID", System.Data.OleDb.OleDbType.BigInt);

        //public static Column __name = new Column("NAME", System.Data.OleDb.OleDbType.VarChar);

        //public static Column __pass = new Column("PASS", System.Data.OleDb.OleDbType.VarChar);

        //public static Column __logintime = new Column("LOGINTIME", System.Data.DbType.Date);

        public static QueryColumn __id = new QueryColumn("USERS.ID", System.Data.DbType.Int32);
        //public static Column __id = new Column("USERS.ID", System.Data.DbType.Decimal);

        public static QueryColumn __name = new QueryColumn("USERS.NAME", System.Data.DbType.String);

        public static QueryColumn __pass = new QueryColumn("USERS.PASS", System.Data.DbType.String);

        public static QueryColumn __logintime = new QueryColumn("USERS.LOGINTIME", System.Data.DbType.DateTime);
    }

    [Table("USERS")]
    public class UsersInfo2 : BaseEntity
    {
        [PrimaryKey(true)]
        //public Int32 Id { get; set; }
        //public decimal Id { get; set; }//oracle
        public Int64 Id { get; set; } //mssql

        public string Name { get; set; }

        public string Pass { get; set; }

        public  DateTime? LoginTime { get; set; }

        //public static Column __id = new Column("ID", System.Data.OleDb.OleDbType.BigInt);

        //public static Column __name = new Column("NAME", System.Data.OleDb.OleDbType.VarChar);

        //public static Column __pass = new Column("PASS", System.Data.OleDb.OleDbType.VarChar);

        //public static Column __logintime = new Column("LOGINTIME", System.Data.DbType.Date);

        public static QueryColumn __id = new QueryColumn("USERS.ID", System.Data.DbType.Int32);
        //public static Column __id = new Column("USERS.ID", System.Data.DbType.Decimal);

        public static QueryColumn __name = new QueryColumn("USERS.NAME", System.Data.DbType.String);

        public static QueryColumn __pass = new QueryColumn("USERS.PASS", System.Data.DbType.String);

        public static QueryColumn __logintime = new QueryColumn("USERS.LOGINTIME", System.Data.DbType.DateTime);
    }
}
