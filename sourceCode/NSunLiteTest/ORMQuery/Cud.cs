using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSun.Data;

//using NSunLite;

namespace NSunLiteTest.ORMQuery
{
    [TestClass]
    public class Cud
    {
        private DBQuery db;

        public Cud()
        {

            db = DBFactoryNew.CreateDBQuery();
        }

        [TestMethod]
        public void InsertTest()
        {
            var insert = db.CreateInsert("Users");
            insert.AddColumn(Users.Name, "peee");
            Console.WriteLine(insert.ToDbCommandText());
        }

        [TestMethod]
        public void InsertReturnTest()
        {
            var insert = db.CreateInsert("Users");
            insert.AddColumn(Users.Name, "peee12322");
            Console.WriteLine(insert.ToDbCommandText());
            //Console.WriteLine(insert.ExecuteReturnAutoIncrementID(Users.Id));
        }

        [TestMethod]
        public void SaveTest()
        {
            DBQuery<UsersInfo> db =  DBFactoryNew.CreateDBQuery<UsersInfo>();
            UsersInfo info = new UsersInfo()
                                 {
                                     Id = 1,
                                     LoginTime = DateTime.Now,
                                     Name = "锡ixwhh",
                                     Pass = "无敌铁精钢"
                                 };
            db.Save(info);
            Console.WriteLine(info.Id);
        }

        [TestMethod]
        public void UpdateTest()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();//new DBQuery<UsersInfo>(Database.Default);
            var info = db.Load(10542);
            if (info.IsPersistence())
            {
                info.Name = "ixihah哈";
                db.Save(info);
            } 
        }

        [TestMethod]
        public void UpdateWhereTest()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();//new DBQuery<UsersInfo>(Database.Default);
            var update = db.CreateUpdate();
            update.Where(UsersInfo.__id < 11);
            update.AddColumn(UsersInfo.__name, "孙悟空");
            var res = db.Update(update);
            Console.WriteLine(res);
        }

        [TestMethod]
        public void DeleteKeyTest()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();//new DBQuery<UsersInfo>(Database.Default);
            UsersInfo info = db.Load(10);
            int res = db.Delete(info);
            Console.WriteLine(res);
        }


        [TestMethod]
        public void DeleteWhereTest()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();//new DBQuery<UsersInfo>(Database.Default);
            var s= db.CreateDelete();
            s.Where(Users.Name.Like("%32%"));
            Console.WriteLine(s.ToDbCommandText());
        }
    }
}
