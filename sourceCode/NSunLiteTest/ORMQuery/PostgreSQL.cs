using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSun.Data;

//using NSunLite;

namespace NSunLiteTest.ORMQuery
{
    [TestClass]
    public class PostgreSQL
    { 

        [TestMethod]
        public void Save()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();// new DBQuery<UsersInfo>(new Database("db8", SqlType.PostgreSql));

            UsersInfo us = new UsersInfo()
            {
                Name = "sdfsdf",
                Pass = "12121",
                LoginTime = DateTime.Now
            };
            Console.WriteLine(db.Save(us));             
        }

        [TestMethod]
        public void Page()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();//new DBQuery<UsersInfo>(new Database("db8", SqlType.PostgreSql));
            var select = db.CreateQuery();
            select.SetIsDistinct(true);
            //select.Where(UsersInfo.__id > 20); 
            int cout;
            var list = db.ToPageList(select, 5,2, out cout);
            foreach (var usersInfo in list)
            {
                Console.WriteLine(usersInfo.Id);
            }
            Console.WriteLine(cout);
        }
    }
}
