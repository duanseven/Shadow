using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using NSunLite;
//using NSunLite.SqlClient;
using NSun.Data;
using NSun.Data.Log;
using NSun.Data.SqlClient;
namespace NSunLiteTest.ORMQuery
{
    [TestClass]
    public class QuerySelect
    {
        [TestMethod]
        public void CountTest()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();//new DBQuery<UsersInfo>(Database.Default);
            var select = db.CreateQuery(); 
            select.SetIsDistinct(true);
            select.Where(Users.Id < 20);            
            //select.Select(Users.Id.Count(), Users.Name);
            //select.GroupBy(Users.Name);
            Console.WriteLine(select.ToDbCommandText());
            Console.WriteLine(db.Count(select));
            //int count;
            //var info = db.SelectPageToList(select, 10, 1, out count);
            //Console.WriteLine("pagecount:" + count);
            //foreach (var usersInfo in info)
            //{
            //    Console.WriteLine("id:" + usersInfo.Id);
            //}
        } 

        [TestMethod]
        public void Count2Test()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();//new DBQuery<UsersInfo>(new Database("db", SqlType.Sqlserver9));
            var select = db.CreateQuery();
            select.SetIsDistinct(true);
            select.Where(UsersInfo.__id > 20);
            select.Select(UsersInfo.__id.Count(), UsersInfo.__name);
            select.GroupBy(UsersInfo.__name);
            //Console.WriteLine(select.ToDbCommandText());
            Console.WriteLine(db.Count(select));
        }

        [TestMethod]
        public void WhereTest()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>(); //new DBQuery<UsersInfo>(Database.Default);
            var select = db.CreateQuery();
            select.SetIsDistinct(true);
            select.Where(Users.Id < 20);//问题
            select.Select(Users.Id.Count(), Users.Name, Users.Id);
            select.GroupBy(Users.Name,Users.Id);
            Console.WriteLine(select.ToDbCommandText());
        }


        [TestMethod]
        public void PageTest()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();// new DBQuery<UsersInfo>(new Database("db", SqlType.Sqlserver9));
            var select = db.CreateQuery();
            select.SetIsDistinct(true);
            select.Where(UsersInfo.__id > 20);
            //select.SortBy(UsersInfo.__name.Desc);
            //select.Select(UsersInfo.__id.Count(), UsersInfo.__name);
            //select.GroupBy(UsersInfo.__name);
            db.OnLog += new LogHandler(db_OnLog);
            int cout;
            var list = db.ToPageList(select, 10, 2, out cout);
            foreach (var usersInfo in list)
            {
                Console.WriteLine(usersInfo.Id);
            }
            Console.WriteLine(cout);
        }

        void db_OnLog(string logMsg)
        {
            Console.WriteLine(logMsg);
        }
 

        [TestMethod]
        public void ExistsTest()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>(); //new DBQuery<UsersInfo>(new Database("db", SqlType.Sqlserver9));

            var select1 = db.CreateQuery();
            select1.Where(UsersInfo.__id > 2);

            var select = db.CreateQuery();
            select.Where(select1.ToSubQuery().NotExists());
            Console.WriteLine(select.ToDbCommandText());
        }


        [TestMethod]
        public void MethodTest()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();// new DBQuery<UsersInfo>(new Database("db", SqlType.Sqlserver9));

            var select1 = db.CreateQuery();

            select1.Where(SqlExtensionMethods.Length(UsersInfo.__id) > 2);
            select1.Select(UsersInfo.__id, UsersInfo.__name, UsersInfo.__pass,
                SqlExtensionMethods.GetCurrentDate().As("logintime"));
            Console.WriteLine(select1.ToDbCommandText());
             

            foreach (var VARIABLE in db.ToList(select1))
            {
                Console.Write(VARIABLE.LoginTime);
                Console.WriteLine(VARIABLE.Id);
            }
        }

        [TestMethod]
        public void FuncTest()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>(); //new DBQuery<UsersInfo>(new Database("db", SqlType.Sqlserver9));
            var s= db.CreateStoredProcedure("aaa");
            s.AddReturnValueParameter("ref", System.Data.DbType.Int32, 4);
            Dictionary<string, object> dic;
            db.ToExecute(s, out dic);
            Console.WriteLine(dic.Count);
        }
    }
}