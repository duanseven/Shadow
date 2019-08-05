using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using NSunLite;
using System.Data.Common;
using System.Data;
//using NSunLite.OracleClient;
using NSun.Data;
using NSun.Data.OracleClient;

namespace NSunLiteTest.ORMQuery
{
    [TestClass]
    public class OracleQuery
    {
        [TestMethod]
        public void Query()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();// new DBQuery<UsersInfo>(new Database("db2", SqlType.Oracle9));
            var select = db.CreateQuery();
            select.SetIsDistinct(true);
            //select.Where(UsersInfo.__id > 20);
            int cout;
            var list = db.ToPageList(select, 10, 1, out cout);
            foreach (var usersInfo in list)
            {
                Console.WriteLine(usersInfo.Id);
            }
            Console.WriteLine(cout);
        }

        [TestMethod]
        public void SaveTest()
        {
            //DBQuery<UsersInfo> db = new DBQuery<UsersInfo>(new Database("db2", SqlType.Oracle9));
            //UsersInfo info = new UsersInfo()
            //                     {
            //                         Name = "nsunlit121231231e",
            //                         Pass = "Lite",
            //                         LoginTime = DateTime.Now
            //                     };
            //Console.WriteLine(db.Save(info));
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();// new DBQuery<UsersInfo>(new Database("db2", SqlType.Oracle9));
            using (var tran = db.GetDbTransaction())
            {
                try
                {
                    UsersInfo info = new UsersInfo()
                    {
                        Name = "nsunlit121231231e",
                        Pass = "Lite",
                        LoginTime = DateTime.Now
                    };
                    Console.WriteLine(db.Save(info, tran)); tran.Commit();
                }
                catch (Exception)
                {
                    tran.Rollback();
                }
            }
          
        }

        [TestMethod]
        public void CountTest()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();//new DBQuery<UsersInfo>(new Database("db2", SqlType.Oracle9));
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
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();//new DBQuery<UsersInfo>(new Database("db2", SqlType.Oracle9));
            var select = db.CreateQuery();
            //select.SetIsDistinct(true);
            select.Where(UsersInfo.__id > 2);
            select.SortBy(UsersInfo.__name.Desc);

            select.Select(UsersInfo.__id.Count(), UsersInfo.__name);
            select.GroupBy(UsersInfo.__name);

            //select.SetSelectRange(3, 2, UsersInfo.__id);
            select.IdentyColumnName = "UsersInfo.id";
            select.IdentyColumnIsNumber = true;
            select.SetMaxResults(3);
            select.SetSkipResults(2);
             

            int cout;
            var list = db.ToPageList(select, 3, 2, out cout);
            foreach (var usersInfo in list)
            {
                Console.WriteLine(usersInfo.Id);
            }
            Console.WriteLine("——————————————————————————————");
            Console.WriteLine(cout);


            //List<UsersInfo> list = new List<UsersInfo>();
            //using (var reader = db.ToDataReader(select))
            //{
            //    while (reader.Read())
            //    {
            //        var product = new UsersInfo();
            //        product.Id = (Int32)reader["Id"];
            //        product.Name = ((string)reader["Name"]);
            //        //product.LoginTime = (reader["LoginTime"] == DBNull.Value ? DateTime.Now : (DateTime)reader["LoginTime"]);
            //        list.Add(product);
            //    }
            //} 
        }

        [TestMethod]
        public void Link()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();// new DBQuery<UsersInfo>(new Database("db2", SqlType.Oracle));
            var select = db.CreateQuery();

            select.Select((OracleExtensionMethods.Link(UsersInfo.__id, UsersInfo.__name)).As("NAME"), UsersInfo.__pass);
            var list = db.ToList(select);
            foreach (var usersInfo in list)
            {
                Console.WriteLine(usersInfo.Name);
            }
        }

        [TestMethod]
        public void Custom()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();//new DBQuery<UsersInfo>(new Database("db2", SqlType.Oracle));
            //var s = db.CreateStoredProcedure("addusers");
            var s = db.CreateCustomSql("insert into users(name,pass) values(:pname,:ppass)");
            s.AddInputParameter("pname", System.Data.DbType.AnsiString, "da32");
            s.AddInputParameter("ppass", System.Data.DbType.AnsiString, "22da32");
            // db.StoredProcedureToExecute(s);
            db.ToExecute(s);
        }

        [TestMethod]
        public void Input()
        {
            //DBQuery<UsersInfo> db = new DBQuery<UsersInfo>(new Database("db2", SqlType.Oracle));
            //var s = db.CreateStoredProcedure("addusers");
            //s.AddInputParameter("pname", System.Data.DbType.AnsiString, "da32");
            //s.AddInputParameter("ppass", System.Data.DbType.AnsiString, "22da32");
            //db.StoredProcedureToExecute(s); 
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();//new DBQuery<UsersInfo>(new Database("db2", SqlType.Oracle));
            var s = db.CreateStoredProcedure("addusers");
            s.AddInputParameter("pname", System.Data.DbType.AnsiString, "da32");
            s.AddInputParameter("ppass", System.Data.DbType.AnsiString, "22da32");
            using (var tran = db.GetDbTransaction())
            {
                try
                {
                    s.SetTransaction(tran);
                    db.ToExecute(s);
                    tran.Commit();
                }
                catch (Exception)
                {
                    tran.Rollback();
                }
            }
        }

        [TestMethod]
        public void OutPutTestMethod()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();//new DBQuery<UsersInfo>(new Database("db2", SqlType.Oracle));
            var s = db.CreateStoredProcedure("getusers");
            s.AddOutputParameter("pname", System.Data.DbType.String, 20);
            s.AddInputParameter("pid", System.Data.DbType.Decimal, 23);
            Dictionary<string, object> dic;
            db.ToExecute(s, out dic);
            Console.WriteLine(dic["pname"]);
        }


        [TestMethod]
        public void IntpuOutPutTestMethod()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();//new DBQuery<UsersInfo>(new Database("db2", SqlType.Oracle));
            var s = db.CreateStoredProcedure("getusers2");
            s.AddInputOutputParameter("pid", System.Data.DbType.Decimal, 62, 4);
            Dictionary<string, object> dic;
            db.ToExecute(s, out dic);
            Console.WriteLine(dic["pid"]);
        }


        [TestMethod]
        public void ReturnTestMetod()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();//new DBQuery<UsersInfo>(new Database("db2", SqlType.Oracle));
            var s = db.CreateStoredProcedure("getusers3");
            s.AddInputParameter("pid", System.Data.DbType.Decimal, 62);
            s.AddReturnValueParameter("id1", System.Data.DbType.Decimal, 4);
            Dictionary<string, object> dic;
            db.ToExecute(s, out dic);
            Console.WriteLine(dic["id1"]);
        }

        [TestMethod]
        public void DataTableTestMetod()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();//new DBQuery<UsersInfo>(new Database("db2", SqlType.Oracle));
            var s = db.CreateStoredProcedure("stupkg.getstudentinfo");
            s.AddOutputParameter("cur_mycursor", System.Data.DbType.Object, 16);
            var table = db.ToDataTable(s);
            Console.WriteLine(table.Rows.Count);
        }

        //[TestMethod]
        //public void TType()
        //{
        //    long i = 1;
        //    var t = i.GetType();
        //    if (t == typeof(int))
        //    {
        //        Console.WriteLine("true");
        //    }
        //    else
        //    {
        //        Console.WriteLine("false");
        //    }
        //}
    }
}
