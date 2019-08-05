using System.Data.Common;
using IBM.Data.DB2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using NSun.Data;

namespace NSunLiteTest.Test
{
    [TestClass]
    public class TestOther
    {
        [TestMethod]
        public void DB2sql()
        {
            using (DB2Connection conn = new IBM.Data.DB2.DB2Connection("Server=192.168.0.12:50000;Database=demo;UID=db2admin;PWD=tiger;"))
            {
                DB2Command cmd = new DB2Command("select * from users", conn);
                conn.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Console.WriteLine(dr[0]);
                }
            }
        }

        [TestMethod]
        public void postgresql()
        {
            using (Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection("Server=192.168.0.12;Port=5432;Database=demo;User Id=postgres;Password=tiger;"))
            {
                Npgsql.NpgsqlCommand cmd = new Npgsql.NpgsqlCommand("select * from \"USERS\"", conn);
                conn.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Console.WriteLine(dr[0]);
                    Console.WriteLine(dr[1]);
                    Console.WriteLine(dr[2]);
                    Console.WriteLine(dr[3]);
                }
            }
        }

        [TestMethod]
        public void S()
        {
            
            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            //Database db = new Database("db");// Database.Default;
            var query = DBFactoryNew.CreateDBQuery<UsersInfo>();// new DBQuery<UsersInfo>(db);
            for (var i = 1; i <= 1000; ++i)
            {
                var select = query.CreateQuery();
                select.Select(UsersInfo.__id, UsersInfo.__name);
                select.SetIsOnLock(true);
                select.Where(UsersInfo.__id == i);
                var s = query.ToEntity(select);
            }
            stopWatch.Stop();

            Console.WriteLine(string.Format("{0}: {1}ms", this.GetType().Name, stopWatch.ElapsedMilliseconds));

            stopWatch.Reset();
            stopWatch.Start();
            for (var i = 1; i <= 1000; ++i)
            {
                var select = query.CreateQuery();
                select.SetIsOnLock(true);
                select.Where(UsersInfo.__id == i);
                var s = query.ToEntity(select);
            }
            stopWatch.Stop();

            Console.WriteLine(string.Format("{0}: {1}ms", this.GetType().Name, stopWatch.ElapsedMilliseconds));
        }

        [TestMethod]
        public void TestSpeed()
        {

        }
    }
}