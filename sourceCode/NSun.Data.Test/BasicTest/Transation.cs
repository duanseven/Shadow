using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSun.Data.Test.Domain;

namespace NSun.Data.Test.BasicTest
{
    /// <summary>
    /// Tranction 的摘要说明
    /// </summary>
    [TestClass]
    public class Transation
    {
        [TestMethod]
        public void Transation1()
        {
            //本地事务
            var dbquery = DBFactory.CreateDBQuery<Teach>();
            try
            {
                dbquery.BeginTransaction();
                dbquery.Save(new Teach()
                {
                    Name = "bf1" + new Random().Next(),
                    Pass = "fb" + new Random().Next()
                });
                dbquery.Commit(isUnitOfWork: false);
            }
            catch (Exception e)
            {
                dbquery.Rollback();
            }
            finally
            {
                dbquery.Close();
            }
        }

        [TestMethod]
        public void Transation2()
        {
            //开启全局事务
            var dbquery = DBFactory.CreateDBQuery<Teach>();
            using (NSun.Data.Transactions.TransactionScope tran = new NSun.Data.Transactions.TransactionScope(dbquery, System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    dbquery.Save(new Teach()
                    {
                        Name = "bf" + new Random().Next(),
                        Pass = "fb" + new Random().Next()
                    });
                    tran.Complete(); 
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        [TestMethod]
        public void Transation3()
        {
            //设置事务
            DBQuery dbquery = DBFactory.CreateDBQuery<Teach>();
            using (var tran = dbquery.GetDbTransaction())
            {
                try
                {
                    dbquery.Save(new Teach()
                    {
                        Name = "bf" + new Random().Next(),
                        Pass = "fb" + new Random().Next()
                    });
                    dbquery.Save(new Teach()
                    {
                        Name = "bf" + new Random().Next(),
                        Pass = "fb" + new Random().Next()
                    }, tran);
                    tran.Commit();
                }
                catch (Exception)
                {
                    tran.Rollback();
                }
            }
        }

        [TestMethod]
        public void Transation4()
        {
            DBQuery dbquery;
            using (dbquery = DBFactory.CreateDBQuery<Teach>())
            {
                dbquery.BeginUnitOfWork();
                //dbquery.IsUnitOfWork = true;
                try
                {
                    Teach ue = new Teach()
                    {
                        Name = "dafa",
                        Pass = "ewew"
                    };
                    int s = dbquery.Save(ue);
                    dbquery.DeleteSqlSection(dbquery.CreateDelete("TEACH").Where(TeachMapping._name == "dafa"));
                    dbquery.DeleteKey<Teach>(s);

                    //var query = dbquery.CreateQuery<Teach>();
                    //query.Where(TeachMapping._id == ue.Id);
                    //dbquery.ToDataSet(query);
                    dbquery.Commit(true);
                }
                catch (Exception e)
                {
                    throw;
                }
            }
        }

        [TestMethod]
        public void Transation5()
        {
            //2.0 事务 
            var dbquery = DBFactory.CreateDBQuery<Teach>();
            using (System.Transactions.TransactionScope tran = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted, Timeout = new TimeSpan(0, 0, 5) }))
            {
                try
                {
                    dbquery.Save(new Teach()
                                     {
                                         Name="ewe2222222222222222222222222222222222w",
                                         Pass="23323"
                                     });
                    tran.Complete();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        [TestMethod]
        public void Transation6()
        {
            var dbquery = DBFactory.CreateDBQuery<Teach>();
            dbquery.BeginBatch(10, System.Data.IsolationLevel.ReadCommitted);

            for (int i = 10; i < 20; i++)
            {
                dbquery.Save(new Teach()
                {
                    Name = "ee11" + i,
                    Pass = "bb11" + i
                });
            } 

            dbquery.EndBatch();
        }
    }
}
