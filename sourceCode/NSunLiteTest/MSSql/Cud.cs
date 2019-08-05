using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSunLiteTest.MSSql.Db;
using System.Transactions;
using System.Data.Common;
using NSunLiteTest.Sproc;

namespace NSunLiteTest.MSSql
{
    [TestClass]
    public class Cud
    {

        [TestMethod]
        public void Add()
        {
            var query = DBFactory.Hz_qyhznr;
            Hz_qyhznr hzinfo = new Hz_qyhznr()
            {
                DH = "13935186422",
                QYMC = "中国国税"
            };
            query.Save(hzinfo);
        }

        [TestMethod]
        public void Addbatchquery()
        {
            //var query = DBFactory.dbUsers;
            //var batchquery = query.BeginBatch(7);
            //batchquery.OnLog += new NSun.Data.LogHandler(batchquery_OnLog);
            //var up = batchquery.CreateUpdate();
            //up.AddColumn(UsersInfo.__name, "fefefe");
            //up.Where(UsersInfo.__id == 2);
            //batchquery.Update(up);

            //var intp = batchquery.CreateInsert();
            //intp.AddColumn(UsersInfo.__name, "wudiaaabbbccc");
            //intp.AddColumn(UsersInfo.__pass, "123wudiaaabbbccc");
            //batchquery.ToExecute(intp);

            //batchquery.Save(new UsersInfo()
            //                    {
            //                        Name = "heixiu",
            //                        Pass = "heixiu2"
            //                    });

            //var cusq = batchquery.CreateCustomSql("insert into users(name,pass) values('efewqq','ewwe')");
            //batchquery.ToExecute(cusq);

            //var sp1 = batchquery.CreateStoredProcedure("addusers");
            //sp1.AddInputParameter("@nane", System.Data.DbType.String, "ew");
            //sp1.AddInputParameter("@pass", System.Data.DbType.String, "e2121w");
            //batchquery.ToExecute(sp1);


            //NSunLiteTest.Sproc.SprocObject1 s1 = new Sproc.SprocObject1();
            //s1.Name = "232fefw";
            //s1.Pass = "f2f23-10003";
            //batchquery.ToProcedureExecute(s1);

            //NSunLiteTest.Sproc.SprocObject1 s3 = query.CreateSprocEntity<SprocObject1>();
            //s3.Name = "232fefw4";
            //s3.Pass = "f2f23-10004";
            //batchquery.ToProcedureExecute(s3);

            //batchquery.EndBatch();

        }

        [TestMethod]
        public void GuidP()
        {
            Console.WriteLine(Guid.NewGuid().ToString());
            Console.WriteLine(Guid.NewGuid().ToString().Length);
            Console.WriteLine(Guid.NewGuid().ToString("N"));
            Console.WriteLine(Guid.NewGuid().ToString("N").Length);
        }

        void batchquery_OnLog(string logMsg)
        {
            Console.WriteLine(logMsg);
        }


        [TestMethod]
        public void AddTran()
        {
            var query = DBFactory.Hz_qyhznr;

            using (DbTransaction tran = query.GetDbTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    Hz_qyhznr hzinfo2 = new Hz_qyhznr()
                    {
                        DH = "13935186427",
                        QYMC = "中国国税"
                    };
                    query.Save(hzinfo2, tran);

                    Hz_qyhznr hzinfo = new Hz_qyhznr()
                    {
                        DH = "13935186423",
                        QYMC = "中国国税",
                        JYJZRQ = "123456789011"
                    };
                    query.Save(hzinfo, tran);

                    tran.Commit();
                    Console.WriteLine("ok");
                }
                catch (Exception e)
                {
                    tran.Rollback();
                    Console.WriteLine(e.Message);
                }
            }
        }

        [TestMethod]
        public void AddTran20()
        {
            var query = DBFactory.Hz_qyhznr;
            try
            {

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    Add();

                    Hz_qyhznr hzinfo = new Hz_qyhznr()
                    {
                        DH = "13935186424",
                        QYMC = "中国国税",
                        JYJZRQ = "123456789012"
                    };
                    query.Save(hzinfo);

                    scope.Complete();
                    Console.WriteLine("ok");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public Hz_qyhznr Read(int id)
        {
            var query = DBFactory.Hz_qyhznr;
            return query.Load(id);
        }

        [TestMethod]
        public void Update()
        {
            var query = DBFactory.Hz_qyhznr;
            var hzinfo = Read(141132);
            hzinfo.QYMC = "中国国税Update";
            query.Save(hzinfo);
        }

        [TestMethod]
        public void UpdateColumn()
        {
            var query = DBFactory.Hz_qyhznr;
            var update = query.CreateUpdate();
            update.AddColumn(Hz_qyhznr._QYMC, "中国国税123");
            update.Where(Hz_qyhznr._ID == 141132);
            query.Update(update);
        }

        [TestMethod]
        public void Delete()
        {
            var hzinfo = Read(141132);
            var query = DBFactory.Hz_qyhznr;
            query.Delete(hzinfo);

            //or delete key
            //query.Delete(hzinfo.ID);
        }

        [TestMethod]
        public void DeleteWhere()
        {
            var query = DBFactory.Hz_qyhznr;
            var delete = query.CreateDelete()
                              .Where(Hz_qyhznr._ID == 141132);
            query.DeleteKey(delete);
        }
         
    }
}
