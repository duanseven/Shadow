using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSun.Data;

//using NSunLite;

namespace NSunLiteTest.Query
{
    /// <summary>
    /// QueryHand 的摘要说明
    /// </summary>
    [TestClass]
    public class QueryHand
    {
        private DBQuery query;

        public QueryHand()
        {
            query = DBFactoryNew.CreateDBQuery();//new DBQuery(Database.Default);
        }

        [TestMethod]
        public void TestMethodInnerJoin()
        {
            var select = query.CreateQuery(new Users());
            select.Join(new Us(), Users.Id == Us.Id);            
            Console.WriteLine(select.ToDbCommandText());
        }

        [TestMethod]
        public void TestMethodLeftJoin()
        {
            var select = query.CreateQuery(new Users());
            select.LeftJoin(new Us(), Users.Id == Us.Id);
            Console.WriteLine(select.ToDbCommandText());
        }

        [TestMethod]
        public void TestMethodRightJoin()
        {
            var select = query.CreateQuery(new Users());
            select.RightJoin(new Us(), Users.Id == Us.Id);
            Console.WriteLine(select.ToDbCommandText());
        }

        [TestMethod]
        public void TestMethodJoinPamare()
        {
            var select = query.CreateQuery(new Users());
            select.Join(new Us(), Users.Id == Us.Id + 1);
            Console.WriteLine(select.ToDbCommandText());
        }

        [TestMethod]
        public void TestMethodJoinSelectColumn()
        {
            var select = query.CreateQuery(new Users());
            select.Select(Users.Id, Users.Name, Us.Name, Us.Pass);
            select.Join(new Us(), Users.Id == Us.Id + 1);
            Console.WriteLine(select.ToDbCommandText());
        }

        [TestMethod]
        public void TestMethodJoin()
        {
            var cust= query.CreateCustomSql("select id from users where id >@how");
            cust.AddInputParameter("@how", System.Data.DbType.Int32, 30);

            var select = query.CreateQuery(new Users());
            select.Select(Users.Id, Users.Name, Us.Name, Us.Pass+"pass");
            select.Join(new Us(), Users.Id == Us.Id + 1);

            select.Where(Users.Id < 90);
            select.Where(Users.Id.In(cust.ToSubQuery()));
            Console.WriteLine(select.ToDbCommandText());
        }

        [TestMethod]
        public void TestMethodEasyPage()
        {
            var select = query.CreateQuery(new Users());
            //select.SetSelectRange(10, 10, Users.Id);
            select.IdentyColumnName = "users.id";
            select.IdentyColumnIsNumber = true;
             select.SetMaxResults(10);
             select.SetSkipResults(10);
            Console.WriteLine(select.ToDbCommandText());
        }

        [TestMethod]
        public void TestMethodEasy2Page()
        {
            var select = query.CreateQuery(new Users());
            select.SortBy(Users.Name.Asc);
            select.ThenSortBy(Users.Pass.Desc);
            //select.SetSelectRange(10, 10, Users.Id);

            select.IdentyColumnName = "Users.id";
            select.IdentyColumnIsNumber = true;
            select.SetMaxResults(10);
            select.SetSkipResults(10);

            Console.WriteLine(select.ToDbCommandText());
        }

        [TestMethod]
        public void TestMethodEasy3Page()
        {
            var cust= query.CreateCustomSql("select id from users where id >@how");
            cust.AddInputParameter("@how", System.Data.DbType.Int32, 30);
            var select = query.CreateQuery(new Users());
            //select.SetSelectRange(10, 10, Users.Id);
            select.IdentyColumnName = "Users.id";
            select.IdentyColumnIsNumber = true;
            select.SetMaxResults(10);
            select.SetSkipResults(10);

            select.Where(Users.Id > 10);
            select.Where(Users.Id.In(cust.ToSubQuery()));
            Console.WriteLine(select.ToDbCommandText());
        }

        [TestMethod]
        public void TestMethodEasyGroupPage()
        {
            var cust = query.CreateCustomSql("select id from users where id > @how");
            cust.AddInputParameter("@how", System.Data.DbType.Int32, 30);
            var select = query.CreateQuery(new Users());
            select.Select(Users.Id.Count(), Users.Name);
            select.Where(Users.Id > 10);
            select.Where(Users.Id.In(cust.ToSubQuery()));
            select.GroupBy(Users.Name);
            select.Having(Users.Name.Like("%2%"));
            //select.SetSelectRange(10, 10, Users.Id);
            select.IdentyColumnName = "Users.id";
            select.IdentyColumnIsNumber = true;
            select.SetMaxResults(10);
            select.SetSkipResults(10);
            Console.WriteLine(select.ToDbCommandText());
        }

         
    }
}
