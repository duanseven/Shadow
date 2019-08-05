using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSun.Data;
using System.Reflection;
using NSun.Data.Lambda;
using NSun.Data.SqlClient;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Data; 
namespace NSunLiteTest.Lambda
{
    [TestClass]
    public class TestLambda
    {

        [TestMethod]
        public void MyTestMethod2()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();// new DBQuery<UsersInfo>(new Database("db", SqlType.Sqlserver9));
            var query = db.CreateQuery();
            query.Where(p1 => p1.Id == 1 || p1.Id == 2);
            query.Select(p => p.Id);
            query.SortBy(p => p.Name.Desc());
            query.GroupBy(p => p.Name);
            Console.WriteLine(query.ToDbCommandText());
        }

        [TestMethod]
        public void GroupOrder()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>();// new DBQuery<UsersInfo>(new Database("db", SqlType.Sqlserver9));
            var query= db.CreateQuery();
            query.Where(p1 =>p1.Id == 1);
            query.Select(p => p.Id);
            query.SortBy(p => p.Name.Desc());
            Console.WriteLine(query.ToDbCommandText());
        }

        [TestMethod]
        public void LikeParameterMethod()
        {
            SqlConnection con = new SqlConnection("server=.;database=demo;uid=sa;pwd=tiger");
            SqlCommand cmd = new SqlCommand("select * from users where name like @a",con);
            cmd.Parameters.Add(new SqlParameter("@a", "%a%"));
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                Console.WriteLine(dr["name"]);
            }
        }

        [TestMethod]
        public void TestLambdaOne()
        {
            DBQuery<UsersEntity> query = DBFactoryNew.CreateDBQuery<UsersEntity>(); //new DBQuery<UsersEntity>(new Database("db", SqlType.Sqlserver9));

            Stopwatch s = new Stopwatch();
            s.Start();
            for (int i = 0; i < 10000; i++)
            {
                var select = query.CreateQuery();
                select.Where(p => p.Id == i);
                var c = query.ToEntity(select);
                //Console.WriteLine("1:"+c.Id);
            }
            s.Stop();
            Console.WriteLine(s.Elapsed);

            //select.Where(p => p.Id == 1 && p.Name == "1");
            //Console.WriteLine(select.ToDbCommandText());
        }

        [TestMethod]
        public void LambdaCache()
        {
            DBQuery<UsersEntity> query = DBFactoryNew.CreateDBQuery<UsersEntity>(); //new DBQuery<UsersEntity>(new Database("db", SqlType.Sqlserver9));

            //Func<SelectSqlSection<UsersEntity>, int, UsersEntity> fun
            //    = (sel, p1) => query.SelectToEntity(sel.Where(p => p.Id == p1));

            Func<SelectSqlSection<UsersEntity>, int, UsersEntity> fun
                = (sel, p1) => query.ToEntity(sel.Where(p => p.Id == p1));

            Stopwatch s = new Stopwatch();
            s.Start();
            for (int i = 0; i < 1000; i++)
            {
                var select = query.CreateQuery();
                var c = fun(select, i);
                //Console.WriteLine("2:" + c.Id);
            }
            s.Stop();
            Console.WriteLine(s.Elapsed);
        }

        [TestMethod]
        public void LambdaEntity()
        {
            DBQuery<UsersInfo> db = DBFactoryNew.CreateDBQuery<UsersInfo>(); //new DBQuery<UsersInfo>(new Database("db", SqlType.Sqlserver9));
            var query = db.CreateQuery();
            query.Where(p => p.Id == 1);
            Console.WriteLine(db.ToEntity(query).Name ?? "null name");
        }

        [TestMethod]
        public void Enum()
        {
            Console.WriteLine((int)WorkPlanType.LongYear);
            Console.WriteLine((int)WorkPlanType.Year);
            Console.WriteLine((int)WorkPlanType.Month);
        }

        public enum WorkPlanType
        {
            Year = 11,
            LongYear = 24,
            Month = 38
        }


        [TestMethod]
        public void MyTestMethod()
        {            
            DBQuery<UsersEntity> db = DBFactoryNew.CreateDBQuery<UsersEntity>();// new DBQuery<UsersEntity>(new Database("db", SqlType.Sqlserver9));            
            db.BeginBatch(10);
            db.Save(new UsersEntity() { Name = "cd" });
            db.EndBatch();
        }
    }
}
