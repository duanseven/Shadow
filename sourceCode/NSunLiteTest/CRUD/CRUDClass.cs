using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSun.Data;
using System.Data;
//using NSunLite;

namespace NSunLiteTest.CRUD
{
    /// <summary>
    /// Insert 的摘要说明
    /// </summary>
    [TestClass]
    public class CRUDClass
    {
        private DBQuery query;
        public CRUDClass()
        {
            //Database db = Database.Default; 
            query = DBFactoryNew.CreateDBQuery();//new DBQuery(db);
        }

        [TestMethod]
        public void TestInsert()
        {
            var insert = query.CreateInsert("Users");
            insert.AddAssignment(Users.Name.Set("de21"));
            insert.AddAssignment(Users.Pass.Set("21dc"));
            Console.WriteLine(insert.ToDbCommandText());
            //query.ToExecuteNonQuery(insert);
        }

        [TestMethod]
        public void TestUpdate()
        {
            var update = query.CreateUpdate("Users");
            update.AddColumn(Users.Name, "hahxiix");
            update.AddColumn(Users.Pass, "s");
            update.Where(Users.Id == 10 || Users.Id == 11);
            Console.WriteLine(update.ToDbCommandText());
            //query.ToExecuteNonQuery(update);
        }

        [TestMethod]
        public void TestDelete()
        {
            var delete = query.CreateDelete("Users");
            delete.Where(Users.Id == 11 || Users.Name.Like("%23232323%"));
            Console.WriteLine(delete.ToDbCommandText());
            //query.ToExecuteNonQuery(delete);
        }

        [TestMethod]
        public void TestCustomEntity()
        {
            CustomQueryTable cu = new CustomQueryTable("(select * from users) as a");
            var q = query.CreateQuery(cu);
            q.Where(new QueryColumn("a.id", DbType.Int32) < 20);

            var dt = query.ToDataTable(q);
            foreach (DataRow dataRow in dt.Rows)
            {
                Console.WriteLine(dataRow[0]);
            }
        }

        [TestMethod]
        public void TestM()
        {
            
        }
    }
}
