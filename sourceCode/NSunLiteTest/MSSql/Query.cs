using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSunLiteTest.MSSql.Db;
using NSun.Data.MySql;
using NSun.Data;
using System.Diagnostics;

namespace NSunLiteTest.MSSql
{
    [TestClass]
    public class Query
    {
        [TestMethod]
        public void ColumnLink()
        {
            var db = DBFactory.Hz_qyhznr;
            var query = db.CreateQuery();
            query.Select(Hz_qyhznr._ID, Hz_qyhznr._QYMC, (Hz_qyhznr._ID.Cast("varchar(50)") + Hz_qyhznr._QYMC).As("id"));
            var table = db.ToDataTable(query);
        }

        [TestMethod]
        public void ColumnCustom()
        {
            var db = DBFactory.Hz_qyhznr;
            var query = db.CreateQuery();
            query.Select(Hz_qyhznr._ID, new ExpressionClip("id+1", System.Data.DbType.Int32));
            Console.WriteLine(query.ToDbCommandText());
        }

        [TestMethod]
        public void SimpleWhere()
        {
            var db = DBFactory.Hz_qyhznr;
            var query = db.CreateQuery();
            int w = 4;
            query.Where(Hz_qyhznr._ID > 12 || (Hz_qyhznr._ID > w && Hz_qyhznr._ID < w + 10));
            Console.WriteLine(query.ToDbCommandText());
        }

        [TestMethod]
        public void In()
        {
            var db = DBFactory.Hz_qyhznr;
            var query = db.CreateQuery();
            query.Where(Hz_qyhznr._ID.In(new[] { 12, 4, 3 }));
        }

        [TestMethod]
        public void ChildrenSearch()
        {
            var db = DBFactory.Hz_qyhznr;
            var custom = db.CreateCustomSql("select  id from users where id<@id");
            custom.AddInputParameter("@id", System.Data.DbType.Int32, "50");
            var childrensearch = custom.ToSubQuery();
            var select = db.CreateQuery();
            select.Where(Hz_qyhznr._QYMC.Contains("中国"));
            select.Where(Hz_qyhznr._ID.In(childrensearch));
            Console.WriteLine(select.ToDbCommandText());
        }

        [TestMethod]
        public void Exists()
        {
            //SELECT *  FROM [HZ_QYHZNR] WHERE [HZ_QYHZNR].[QYMC] LIKE N'%中国%' AND 
            //EXISTS (SELECT *  FROM [HZ_QYHZNR] WHERE [HZ_QYHZNR].[ID] < 50)
            var db = DBFactory.Hz_qyhznr;
            var custom = db.CreateQuery();
            custom.Where(Hz_qyhznr._ID < 50);
            var childrensearch = custom.ToSubQuery();
            var select = db.CreateQuery();
            select.Where(Hz_qyhznr._QYMC.Contains("中国"));
            select.Where(childrensearch.Exists());
            Console.WriteLine(select.ToDbCommandText());
        }

        [TestMethod]
        public void SearchPage()
        {
            var db = DBFactory.Hz_qyhznr;
            int countpage;
            db.ToPageList(10, 1, out countpage);
        }

        [TestMethod]
        public void OrderyBy()
        {
            var db = DBFactory.Hz_qyhznr;
            var query = db.CreateQuery();
            query.SortBy(Hz_qyhznr._ID.Desc);
            query.ThenSortBy(Hz_qyhznr._QYMC.Asc);
            db.ToList(query);
        }

        [TestMethod]
        public void Distinct()
        {
            var db = DBFactory.Hz_qyhznr;
            var query = db.CreateQuery();
            query.SetIsDistinct(true);
            db.ToList(query);
        }

        [TestMethod]
        public void GroupBy()
        {
            var db = DBFactory.Hz_qyhznr;
            var query = db.CreateQuery();
            query.Select(Hz_qyhznr._ID.Count(), Hz_qyhznr._QYMC);
            query.GroupBy(Hz_qyhznr._QYMC);
        }

        [TestMethod]
        public void Join()
        {
            var db = DBFactory.Hz_qyhznr;
            CustomQueryTable cq = new CustomQueryTable("SELECT * from us");
            var q = db.CreateQuery();
            q.Join(cq, "ww", Hz_qyhznr._ID == new ExpressionClip("ww.id", System.Data.DbType.Int32));
            Console.WriteLine(q.ToDbCommandText());
        }

        [TestMethod]
        public void JoinSelect()
        {
            var db = DBFactory.Hz_qyhznr;
            var q1 = db.CreateQuery("us");
            CustomQueryTable cq = new CustomQueryTable(q1.ToSubQuery());
            var q = db.CreateQuery();
            q.Join(cq, "ww", Hz_qyhznr._ID == new ExpressionClip("ww.id", System.Data.DbType.Int32));
            Console.WriteLine(q.ToDbCommandText());
        }

        [TestMethod]
        public void JoinCust()
        {
            var db = DBFactory.Hz_qyhznr;
            var q1 = db.CreateCustomSql("select * from us");
            CustomQueryTable cq = new CustomQueryTable(q1.ToSubQuery());
            var q = db.CreateQuery();
            q.Join(cq, "ww", Hz_qyhznr._ID == new ExpressionClip("ww.id", System.Data.DbType.Int32));
            Console.WriteLine(q.ToDbCommandText());
        }

        [TestMethod]
        public void JoinColumn()
        {
            var db = DBFactory.Hz_qyhznr;
            var query = db.CreateQuery();
            query.Join(new UsersInfo2(), Hz_qyhznr._ID == UsersInfo2.__id);
            query.Select(Hz_qyhznr._ID.As("hz_id"), Hz_qyhznr._QYMC,UsersInfo2.__id);
            Console.WriteLine(query.ToDbCommandText());
        }

        [TestMethod]
        public void JoinCustomColumn()
        {
            var db = DBFactory.Hz_qyhznr;
            var q1 = db.CreateCustomSql("select * from users");
            CustomQueryTable cq = new CustomQueryTable(q1.ToSubQuery());
            var query = db.CreateQuery();
            query.Join(cq, "USERS", Hz_qyhznr._ID == UsersInfo2.__id);
            //new ExpressionClip("USERS.ID", System.Data.DbType.String);
            query.Select(Hz_qyhznr._ID.As("hz_id"), Hz_qyhznr._QYMC, UsersInfo2.__id);
            Console.WriteLine(query.ToDbCommandText());
        }

        [TestMethod]
        public void CustQuery()
        {
            var db = DBFactory.Hz_qyhznr;
            var condition = new Condition();
            int i = 4;
            if (i > 0)
            {
                condition =  Hz_qyhznr._QYMC.Contains("中国");
            }
            if (i % 2 == 0)
            {
                condition = condition && Hz_qyhznr._QYMC.Contains("太原");
            }
            if (i % 4 == 0)
            {
                condition = condition && Hz_qyhznr._QYMC.Contains("山西");
            }             
            var q = db.CreateQuery();
            q.Where(condition);
            Console.WriteLine(q.ToDbCommandText()); 
        }

    }
}
