using System;
using System.Collections.Generic; 
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using NSunLite;
using System.Data;
using NSun.Data;
using NSun.Data.MsAccess;

namespace NSunLiteTest.ORMQuery
{
    [TestClass]
    public class MSACCESSQuery
    {

        [TestMethod]
        public void Save()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();//new DBQuery<UsersInfo>(new Database("db3", SqlType.MsAccess));
            //IBM.Data.DB2
            //MySql.Data.MySqlClient
            //System.Data.SqlClient
            //System.Data.OracleClient
            //System.Data.OleDb
            
            UsersInfo us = new UsersInfo()
                               {
                                   Name = "sdfsdf",
                                   Pass = "12121",
                                   LoginTime = DateTime.Now
                               };
            Console.WriteLine(db.Save(us));
           
        }

        public void A(Enum s)
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();// new DBQuery<UsersInfo>(new Database("db3", SqlType.MsAccess));

            var c= db.CreateCustomSql("Insert into users(name) values(@name)");
            //c.AddInputParameter("@name", System.Data.DbType.String, "aee");
            var cmd= c.ToDbCommand();            
            var old = new System.Data.OleDb.OleDbParameter("@name", "add");
            old.OleDbType = (System.Data.OleDb.OleDbType) s;
            old.Direction = System.Data.ParameterDirection.Input;
            cmd.Parameters.Add(old);            
            //Console.WriteLine(s);
            if (s is System.Data.SqlDbType)
            {
                Console.WriteLine(s);
            }
            else
            {
                Console.WriteLine("不是sqldb");
            }
        }

        [TestMethod]
        public void CountTest()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();// new DBQuery<UsersInfo>(new Database("db3", SqlType.MsAccess));
            var select = db.CreateQuery();
            select.SetIsDistinct(true);
            select.Where(UsersInfo.__id > 20);
            select.Select(UsersInfo.__id.Count(), UsersInfo.__name);
            select.GroupBy(UsersInfo.__name);
            Console.WriteLine(select.ToDbCommandText());
            Console.WriteLine(db.Count(select));
        }

        [TestMethod]
        public void PageTest()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>(); //new DBQuery<UsersInfo>(new Database("db3", SqlType.MsAccess));
            var select = db.CreateQuery();
            //select.SetIsDistinct(true);
            select.Where(UsersInfo.__id > 2);
            select.SortBy(UsersInfo.__name.Desc);

            select.Select(UsersInfo.__id, UsersInfo.__name);

 
            int cout;
            var list = db.ToPageList(select, 3, 2, out cout);
            foreach (var usersInfo in list)
            {
                Console.WriteLine(usersInfo.Id);
            }
            Console.WriteLine("——————————————————————————————");
            Console.WriteLine(cout); 
        }

        [TestMethod]
        public void Method()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();// new DBQuery<UsersInfo>(new Database("db3", SqlType.MsAccess));
            var select = db.CreateQuery();

            select.Where(MsAccessExtensionMethods.DateAdd(UsersInfo.__logintime,"m", 12) == DateTime.Now);
            Console.WriteLine(select.ToDbCommandText());
        }
    }
}
