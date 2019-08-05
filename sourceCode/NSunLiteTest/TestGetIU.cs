using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSun.Data;

namespace NSunLiteTest.IU
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

    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class TestGetIU
    {
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void MyTestMethod()
        {
            DBQuery<NSunLiteTest.IU.UsersEntity> db = DBFactoryNew.CreateDBQuery<UsersEntity>();////new DBQuery<NSunLiteTest.IU.UsersEntity>(new Database("dbexpress"));

            NSunLiteTest.IU.UsersEntity ue = new UsersEntity();
            ue.Name = "dc";
            ue.Pass = "ada1234";
            ue.LoginTime = DateTime.Now;
            ue.Id = 1;
            //ue.SetIsPersistence(true);
            var up = db.GetInsertQuery(ue);
            Console.WriteLine(up.ToDbCommandText());
        } 
    }
}
