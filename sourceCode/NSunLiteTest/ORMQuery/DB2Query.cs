using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSun.Data;

namespace NSunLiteTest.ORMQuery
{
    [TestClass]
    public class DB2Query
    {

        [TestMethod]
        public void Save()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();//new DBQuery<UsersInfo>(new Database("db6", SqlType.Db2));

            UsersInfo us = new UsersInfo()
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
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();// new DBQuery<UsersInfo>(new Database("db6", SqlType.Db2));
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
