using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSun.Data;
using NSunLiteTest.ADO2Transaction;

namespace NSunLiteTest.New4
{
    /// <summary>
    /// NewTest 的摘要说明
    /// </summary>
    [TestClass]
    public class NewTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var db = DBFactoryNew.Instance.CreateDBQuery<PhoneEntity>();
            
            //foreach (var info in db.ToList())
            //{
            //    Console.WriteLine(info.Id + "__" + info.Name);                
            //}       
            var s = db.CreateQuery();
            s.SetMaxResults(1);
            var p = db.ToEntity(s);            
            //            PhoneEntity p = new PhoneEntity();
            Console.WriteLine(p.IdentityKey);
        }

        [TestMethod]
        public void MyTestMethod()
        {
            var db = DBFactoryNew.Instance.CreateDBQuery<SchoolEntity>();
            SchoolEntity sc = new SchoolEntity()
                                  {
                                      Name = "晋机中学"
                                  };
            db.Save(sc);
        }


        private static void NewMethod9()
        {
            var db = DBFactoryNew.Instance.CreateDBQuery<UsersEntity>();
            var query = db.CreateQuery();
            var list = new List<ExpressionClip>(query.ResultColumns.ToArray());
            list.Add(new ExpressionClip("DENSE_RANK() OVER(ORDER BY id asc) AS seq", System.Data.DbType.Int32));
            query.Select(list.ToArray());
            query.Where(UsersEntity._id < 100);

            CustomWithQueryTable custtable = new CustomWithQueryTable(query.ToSubQuery(), "w");

            var custdb = DBFactoryNew.Instance.CreateDBQuery();
            var select = custdb.CreateQuery(custtable);

            select.Where(new QueryColumn("seq", DbType.Int32) < 10);

            Console.WriteLine(select.ToDbCommandText());
            var dt = custdb.ToDataTable(select);
            foreach (DataRow row in dt.Rows)
            {
                Console.WriteLine(row[0] + "" + row[1] + row[2]);
            }

        }

        private static void NewMethod8()
        {
            var db = DBFactoryNew.Instance.CreateDBQuery<UsersEntity>();
            var query = db.CreateQuery();
            var list = new List<ExpressionClip>(query.ResultColumns.ToArray());
            list.Add(new ExpressionClip("DENSE_RANK() OVER(ORDER BY id asc) AS seq", System.Data.DbType.Int32));
            query.Select(list.ToArray());
            query.Where(UsersEntity._id < 100);
            CustomQueryTable ct = new CustomQueryTable(query.ToSubQuery(), "w");

            var custdb = DBFactoryNew.Instance.CreateDBQuery();
            var select = custdb.CreateQuery(ct);
            select.Where(new QueryColumn("seq", System.Data.DbType.Int32) < 10);

            Console.WriteLine(select.ToDbCommandText());
            var dt = db.ToDataTable(select);
            foreach (DataRow row in dt.Rows)
            {
                Console.WriteLine(row[0] + "" + row[1] + row[2]);
            }
        }
    }
}
