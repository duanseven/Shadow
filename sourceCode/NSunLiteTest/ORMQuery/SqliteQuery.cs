using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSun.Data;

namespace NSunLiteTest.ORMQuery
{
    [TestClass]
    public class SqliteQuery
    { 
        [TestMethod]
        public void Save()
        {
            DBQuery<UsersInfo2> db =DBFactoryNew.CreateDBQuery<UsersInfo2>(); 
            //DBQuery<UsersInfo2> db = new DBQuery<UsersInfo2>(new Database("db7", SqlType.Sqlite));

            UsersInfo2 us = new UsersInfo2()
                               {
                                   Name = "sdfsdf",
                                   Pass = "12121",
                                   LoginTime = DateTime.Now
            };
            Console.WriteLine(db.Save(us));
            
        }

        [TestMethod]
        public void Query()
        {
            //DBQuery<UsersInfo2> db = new DBQuery<UsersInfo2>(new Database("db7", SqlType.Sqlite));
            DBQuery<UsersInfo2> db = DBFactoryNew.CreateDBQuery<UsersInfo2>(); 
            var select = db.CreateQuery();
            select.SetIsDistinct(true);
            //select.Where(UsersInfo.__id > 20);
            int cout;
            var list = db.ToPageList(select, 3, 2, out cout);

            foreach (var usersInfo in list)
            {
                Console.WriteLine(usersInfo.Id);
            }
            Console.WriteLine(cout);
        }
    }
}
