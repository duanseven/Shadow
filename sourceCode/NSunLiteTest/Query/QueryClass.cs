using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSun.Data;

//using NSunLite;

namespace NSunLiteTest.Query
{
    /// <summary>
    /// QueryClass 的摘要说明
    /// </summary>
    [TestClass]
    public class QueryClass
    {
        private DBQuery query;

        public QueryClass()
        {
            query = DBFactoryNew.CreateDBQuery();//new DBQuery(Database.Default);
        }

        [TestMethod]
        public void SelectAnd()
        {
            var select= query.CreateQuery(new Users());
            select.Where(Users.Id == 10);
            select.Where(Users.Name == "dc");
            select.Where(Users.Name == 12);//自动转换
            select.Or(Users.Id == 10);
            Console.WriteLine(select.ToDbCommandText());
        }


        [TestMethod]
        public void SelectOr()
        {
            var select = query.CreateQuery(new Users());
            select.Where(Users.Id == 1 && (Users.Name == "dc" || Users.Id == 10));
            Console.WriteLine(select.ToDbCommandText());
        }

        [TestMethod]
        public void SelectIn()
        {
            var s1 = query.CreateQuery(new Users());
            s1.Select(Users.Id);
            s1.Where(Users.Name.Like("%2%"));
            var c1 = query.CreateCustomSql("select id from users where name like @a");
            c1.AddInputParameter("@a", System.Data.DbType.String, "2%");
            var select = query.CreateQuery(new Users());
            select.Where(Users.Id.In(new [] {1, 2, 3, 4}));
            select.Where(Users.Name.In(new [] {"21","22"}));            
            select.Where(Users.Id == 1);
            select.Where(Users.Id.In(s1.ToSubQuery()));
            select.Where(Users.Id.In(c1.ToSubQuery()));
            Console.WriteLine(select.ToDbCommandText());
        }

        [TestMethod]
        public void SelectThan()
        {
            var select = query.CreateQuery(new Users());
            select.Where(Users.Id > 1);
            select.Where(Users.Id >= 2);
            select.Where(Users.Id < 3);
            select.Where(Users.Id <= 4);
            Console.WriteLine(select.ToDbCommandText());
        }

        [TestMethod]
        public void SelectBitwise()
        {
            var select = query.CreateQuery(new Users());
            select.Where(Users.Id.BitwiseAnd(12) == 12);
            select.Where(Users.Id.BitwiseOr(13) == Users.Id);
            Console.WriteLine(select.ToDbCommandText());
        }

        [TestMethod]
        public void SelectBetween()
        {
            var select = query.CreateQuery(new Users());
            select.Where(Users.Id.Between(12, 16, false, false));
            select.Where(Users.Id.Between(12, 16, true, false));
            Console.WriteLine(select.ToDbCommandText());
        }


        [TestMethod]
        public void SelectLike()
        {
            var select = query.CreateQuery(new Users());
            select.Where(Users.Name.Like("%cc%"));
            select.Where(Users.Name.Contains("bb"));
            Console.WriteLine(select.ToDbCommandText());
        }

        [TestMethod]
        public void SelectTop()
        {
            var select = query.CreateQuery(new Users());
            select.SetMaxResults(10);
            Console.WriteLine(select.ToDbCommandText());
        }

        [TestMethod]
        public void SelectDistinct()
        {
            var select = query.CreateQuery(new Users());
            select.SetIsDistinct(true);
            select.SetMaxResults(10);
            Console.WriteLine(select.ToDbCommandText());
        }


        [TestMethod]
        public void SelectColumn()
        {
            var select = query.CreateQuery(new Users());
            //select.Select(Users.Id, Users.Name, Users.Pass);
            select.Select(Users.Id.Count(), Users.Pass.Max(), Users.Pass.Min().As("pass"));
            //select.Select(Users.Id + 1, Users.Name + "ss");
            Console.WriteLine(select.ToDbCommandText());
        }

        [TestMethod]
        public void SelectOrderBy()
        {
            var select = query.CreateQuery(new Users());
            select.SortBy(Users.Id.Desc);
            select.SortBy(Users.Pass.Asc);
            select.ThenSortBy(Users.Name.Desc);
            Console.WriteLine(select.ToDbCommandText());
        }

        [TestMethod]
        public void SelectGroupBy()
        {
            var select = query.CreateQuery(new Users());
            select.Select(Users.Id.Count(), Users.Name,Users.Pass);//默认聚合后显示列名不加表名
            select.Where(Users.Id > 30);
            select.SortBy(new ExpressionClip("id", System.Data.DbType.Int32).Asc);
            select.GroupBy(Users.Name,Users.Pass);
            select.Having(Users.Name.Like("%2%"));
            Console.WriteLine(select.ToDbCommandText());
        }

        [TestMethod]
        public void SelectGroupBy2()
        {
            var s1 = query.CreateQuery(new Users());
            s1.Select(Users.Id);
            s1.Where(Users.Id > 100);

            var select = query.CreateQuery(new Users());
            select.Select(Users.Id.Count().As("usersid"), Users.Name);
            select.Where(Users.Id.In(s1.ToSubQuery()));
            select.SortBy(new QueryColumn("usersid", System.Data.DbType.Int32).Asc);
            select.GroupBy(Users.Name);
            select.Having(Users.Name.Like("%2%"));
            Console.WriteLine(select.ToDbCommandText());
        }

    }
}
