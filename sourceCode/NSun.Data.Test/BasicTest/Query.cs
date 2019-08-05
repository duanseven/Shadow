using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSun.Data.Test.Domain;
using NSun.Data.Test.TestCommon;

namespace NSun.Data.Test.BasicTest
{
    /// <summary>
    /// Query 的摘要说明
    /// </summary>
    [TestClass]
    public class Query
    {
        private DBQuery<Teach> TeachDB;
        public Query()
        {
            TeachDB = DBFactory.CreateDBQuery<Teach>();
        }        

        [TestMethod]
        public void Query1()
        {
            var query = TeachDB.CreateQuery();
            query.Where(TeachMapping._id > 5);
            var list = TeachDB.ToList(query);
            Console.WriteLine(list.Count);
        }

        [TestMethod]
        public void Query2()
        {
            var query = TeachDB.CreateQuery(); 
            //query.Where(TeachMapping._name == 5 || TeachMapping._id == 6);
            query.Where(TeachMapping._name == "ewew" && TeachMapping._pass == "23323");
            var list = TeachDB.ToList(query);
            Console.WriteLine(list.Count);
        }

        [TestMethod]
        public void Query3()
        {
            var db = DBFactory.CreateDBQuery<Student>();
            var query= db.CreateQuery();
            query.Where(StudentMapping._name.Like("%w/_%").Space(StudentMapping._name.Escape("'/'")));
            Console.WriteLine(query.ToDbCommandText());
            //var list= db.ToList(query);
            //Console.WriteLine(list.Count);
        }

        [TestMethod]
        public void Query4()
        {
            var db= DBFactory.CreateDBQuery<Student>();
            var query = db.CreateQuery(); 
            query.Where(StudentMapping._id == 1 && StudentMapping._id == StudentInfoMapping._id);

            var db2 = DBFactory.CreateDBQuery<StudentInfo>();

            var q2 = db2.CreateQuery();
            q2.Select(StudentMapping._id.As("studentid"));
            q2.Where(query.ToSubQuery().Exists());
            //Console.WriteLine(db2.ToList(q2).Count);
            Console.WriteLine(q2.ToDbCommandText());
        }

        [TestMethod]
        public void Query5()
        {
            var db = DBFactory.CreateDBQuery<Student>();
            var query = db.CreateQuery();
            query.Where(StudentMapping._birthday.CustomMethodExtensions(System.Data.DbType.String, false,
                                                                        "GetCurrentDate") == "02/02/1987");
            Console.WriteLine(query.ToDbCommandText());
            //var list= db.ToList(query);
            //Console.WriteLine(list.Count);
        }


        //[TestMethod]
        //public void SelectAnd()
        //{            
        //    var select = query.CreateQuery(new Users());
        //    select.Where(Users.Id == 10);
        //    select.Where(Users.Name == "dc");
        //    select.Where(Users.Name == 12);//自动转换
        //    select.Or(Users.Id == 10);
        //    Console.WriteLine(select.ToDbCommandText());
        //}


        //[TestMethod]
        //public void SelectOr()
        //{
        //    var select = query.CreateQuery(new Users());
        //    select.Where(Users.Id == 1 && (Users.Name == "dc" || Users.Id == 10));
        //    Console.WriteLine(select.ToDbCommandText());
        //}

        //[TestMethod]
        //public void SelectIn()
        //{
        //    var s1 = query.CreateQuery(new Users());
        //    s1.Select(Users.Id);
        //    s1.Where(Users.Name.Like("%2%"));
        //    var c1 = query.CreateCustomSql("select id from users where name like @a");
        //    c1.AddInputParameter("@a", System.Data.DbType.String, "2%");
        //    var select = query.CreateQuery(new Users());
        //    select.Where(Users.Id.In(new[] { 1, 2, 3, 4 }));
        //    select.Where(Users.Name.In(new[] { "21", "22" }));
        //    select.Where(Users.Id == 1);
        //    select.Where(Users.Id.In(s1.ToSubQuery()));
        //    select.Where(Users.Id.In(c1.ToSubQuery()));
        //    Console.WriteLine(select.ToDbCommandText());
        //}

        //[TestMethod]
        //public void SelectThan()
        //{
        //    var select = query.CreateQuery(new Users());
        //    select.Where(Users.Id > 1);
        //    select.Where(Users.Id >= 2);
        //    select.Where(Users.Id < 3);
        //    select.Where(Users.Id <= 4);
        //    Console.WriteLine(select.ToDbCommandText());
        //}

        //[TestMethod]
        //public void SelectBitwise()
        //{
        //    var select = query.CreateQuery(new Users());
        //    select.Where(Users.Id.BitwiseAnd(12) == 12);
        //    select.Where(Users.Id.BitwiseOr(13) == Users.Id);
        //    Console.WriteLine(select.ToDbCommandText());
        //}

        //[TestMethod]
        //public void SelectBetween()
        //{
        //    var select = query.CreateQuery(new Users());
        //    select.Where(Users.Id.Between(12, 16, false, false));
        //    select.Where(Users.Id.Between(12, 16, true, false));
        //    Console.WriteLine(select.ToDbCommandText());
        //}


        //[TestMethod]
        //public void SelectLike()
        //{
        //    var select = query.CreateQuery(new Users());
        //    select.Where(Users.Name.Like("%cc%"));
        //    select.Where(Users.Name.Contains("bb"));
        //    Console.WriteLine(select.ToDbCommandText());
        //}

        //[TestMethod]
        //public void SelectTop()
        //{
        //    var select = query.CreateQuery(new Users());
        //    select.SetMaxResults(10);
        //    Console.WriteLine(select.ToDbCommandText());
        //}

        //[TestMethod]
        //public void SelectDistinct()
        //{
        //    var select = query.CreateQuery(new Users());
        //    select.SetIsDistinct(true);
        //    select.SetMaxResults(10);
        //    Console.WriteLine(select.ToDbCommandText());
        //}


        //[TestMethod]
        //public void SelectColumn()
        //{
        //    var select = query.CreateQuery(new Users());
        //    //select.Select(Users.Id, Users.Name, Users.Pass);
        //    select.Select(Users.Id.Count(), Users.Pass.Max(), Users.Pass.Min().As("pass"));
        //    //select.Select(Users.Id + 1, Users.Name + "ss");
        //    Console.WriteLine(select.ToDbCommandText());
        //}

        //[TestMethod]
        //public void SelectOrderBy()
        //{
        //    var select = query.CreateQuery(new Users());
        //    select.SortBy(Users.Id.Desc);
        //    select.SortBy(Users.Pass.Asc);
        //    select.ThenSortBy(Users.Name.Desc);
        //    Console.WriteLine(select.ToDbCommandText());
        //}

        //[TestMethod]
        //public void SelectGroupBy()
        //{
        //    var select = query.CreateQuery(new Users());
        //    select.Select(Users.Id.Count(), Users.Name, Users.Pass);//默认聚合后显示列名不加表名
        //    select.Where(Users.Id > 30);
        //    select.SortBy(new ExpressionClip("id", System.Data.DbType.Int32).Asc);
        //    select.GroupBy(Users.Name, Users.Pass);
        //    select.Having(Users.Name.Like("%2%"));
        //    Console.WriteLine(select.ToDbCommandText());
        //}

        //[TestMethod]
        //public void SelectGroupBy2()
        //{
        //    var s1 = query.CreateQuery(new Users());
        //    s1.Select(Users.Id);
        //    s1.Where(Users.Id > 100);

        //    var select = query.CreateQuery(new Users());
        //    select.Select(Users.Id.Count().As("usersid"), Users.Name);
        //    select.Where(Users.Id.In(s1.ToSubQuery()));
        //    select.SortBy(new QueryColumn("usersid", System.Data.DbType.Int32).Asc);
        //    select.GroupBy(Users.Name);
        //    select.Having(Users.Name.Like("%2%"));
        //    Console.WriteLine(select.ToDbCommandText());
        //}


        //[TestMethod]
        //public void TestMethodInnerJoin()
        //{
        //    var select = query.CreateQuery(new Users());
        //    select.Join(new Us(), Users.Id == Us.Id);
        //    Console.WriteLine(select.ToDbCommandText());
        //}

        //[TestMethod]
        //public void TestMethodLeftJoin()
        //{
        //    var select = query.CreateQuery(new Users());
        //    select.LeftJoin(new Us(), Users.Id == Us.Id);
        //    Console.WriteLine(select.ToDbCommandText());
        //}

        //[TestMethod]
        //public void TestMethodRightJoin()
        //{
        //    var select = query.CreateQuery(new Users());
        //    select.RightJoin(new Us(), Users.Id == Us.Id);
        //    Console.WriteLine(select.ToDbCommandText());
        //}

        //[TestMethod]
        //public void TestMethodJoinPamare()
        //{
        //    var select = query.CreateQuery(new Users());
        //    select.Join(new Us(), Users.Id == Us.Id + 1);
        //    Console.WriteLine(select.ToDbCommandText());
        //}

        //[TestMethod]
        //public void TestMethodJoinSelectColumn()
        //{
        //    var select = query.CreateQuery(new Users());
        //    select.Select(Users.Id, Users.Name, Us.Name, Us.Pass);
        //    select.Join(new Us(), Users.Id == Us.Id + 1);
        //    Console.WriteLine(select.ToDbCommandText());
        //}

        //[TestMethod]
        //public void TestMethodJoin()
        //{
        //    var cust = query.CreateCustomSql("select id from users where id >@how");
        //    cust.AddInputParameter("@how", System.Data.DbType.Int32, 30);

        //    var select = query.CreateQuery(new Users());
        //    select.Select(Users.Id, Users.Name, Us.Name, Us.Pass + "pass");
        //    select.Join(new Us(), Users.Id == Us.Id + 1);

        //    select.Where(Users.Id < 90);
        //    select.Where(Users.Id.In(cust.ToSubQuery()));
        //    Console.WriteLine(select.ToDbCommandText());
        //}

        //[TestMethod]
        //public void TestMethodEasyPage()
        //{
        //    var select = query.CreateQuery(new Users());
        //    //select.SetSelectRange(10, 10, Users.Id);
        //    select.IdentyColumnName = "users.id";
        //    select.IdentyColumnIsNumber = true;
        //    select.SetMaxResults(10);
        //    select.SetSkipResults(10);
        //    Console.WriteLine(select.ToDbCommandText());
        //}

        //[TestMethod]
        //public void TestMethodEasy2Page()
        //{
        //    var select = query.CreateQuery(new Users());
        //    select.SortBy(Users.Name.Asc);
        //    select.ThenSortBy(Users.Pass.Desc);
        //    //select.SetSelectRange(10, 10, Users.Id);

        //    select.IdentyColumnName = "Users.id";
        //    select.IdentyColumnIsNumber = true;
        //    select.SetMaxResults(10);
        //    select.SetSkipResults(10);

        //    Console.WriteLine(select.ToDbCommandText());
        //}

        //[TestMethod]
        //public void TestMethodEasy3Page()
        //{
        //    var cust = query.CreateCustomSql("select id from users where id >@how");
        //    cust.AddInputParameter("@how", System.Data.DbType.Int32, 30);
        //    var select = query.CreateQuery(new Users());
        //    //select.SetSelectRange(10, 10, Users.Id);
        //    select.IdentyColumnName = "Users.id";
        //    select.IdentyColumnIsNumber = true;
        //    select.SetMaxResults(10);
        //    select.SetSkipResults(10);

        //    select.Where(Users.Id > 10);
        //    select.Where(Users.Id.In(cust.ToSubQuery()));
        //    Console.WriteLine(select.ToDbCommandText());
        //}

        //[TestMethod]
        //public void TestMethodEasyGroupPage()
        //{
        //    var cust = query.CreateCustomSql("select id from users where id > @how");
        //    cust.AddInputParameter("@how", System.Data.DbType.Int32, 30);
        //    var select = query.CreateQuery(new Users());
        //    select.Select(Users.Id.Count(), Users.Name);
        //    select.Where(Users.Id > 10);
        //    select.Where(Users.Id.In(cust.ToSubQuery()));
        //    select.GroupBy(Users.Name);
        //    select.Having(Users.Name.Like("%2%"));
        //    //select.SetSelectRange(10, 10, Users.Id);
        //    select.IdentyColumnName = "Users.id";
        //    select.IdentyColumnIsNumber = true;
        //    select.SetMaxResults(10);
        //    select.SetSkipResults(10);
        //    Console.WriteLine(select.ToDbCommandText());
        //}

    }
}
