using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NSun.Data;

namespace NSunLiteTest.ADO2Transaction
{
    [System.SerializableAttribute()]
    [NSun.Data.TableAttribute("US")]
    public class MEN
    {
        public static NSun.Data.QueryColumn _id = new NSun.Data.QueryColumn("US.ID", System.Data.DbType.Int32);

        public static NSun.Data.QueryColumn _name = new NSun.Data.QueryColumn("US.NAME", System.Data.DbType.String);

        public static NSun.Data.QueryColumn _pass = new NSun.Data.QueryColumn("US.PASS", System.Data.DbType.String);

        [NSun.Data.PrimaryKeyAttribute(true)]
        public int Id { get; set; }

        [OptimisticLocking()]
        public string Name { get; set; }

        public string Pass { get; set; }

        [HashSetMapping(typeof(MEN2), "_id", "_id")]
        public HashSet<MEN2> Userses { get; set; }


    }

    [System.SerializableAttribute()]
    [NSun.Data.TableAttribute("USERS")]
    public class MEN2
    {

        public static NSun.Data.QueryColumn _id = new NSun.Data.QueryColumn("USERS.ID", System.Data.DbType.Int64);
        public static NSun.Data.QueryColumn _name = new NSun.Data.QueryColumn("USERS.NAME", System.Data.DbType.String);
        public static NSun.Data.QueryColumn _pass = new NSun.Data.QueryColumn("USERS.PASS", System.Data.DbType.String);
        public static NSun.Data.QueryColumn _logintime = new NSun.Data.QueryColumn("USERS.LOGINTIME", System.Data.DbType.DateTime);

        [NSun.Data.PrimaryKeyAttribute(true)]
        public long Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Pass
        {
            get;
            set;
        }

        public System.Nullable<System.DateTime> Logintime
        {
            get;
            set;
        }
    }
}
