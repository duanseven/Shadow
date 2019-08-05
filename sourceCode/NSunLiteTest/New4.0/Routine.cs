using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;
using NSun.Data;

namespace NSunLiteTest
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class Routine
    {
        DBQuery<NSunLiteTest.ADO2Transaction.UsersEntity> db = DBFactoryNew.Instance.CreateDBQuery<NSunLiteTest.ADO2Transaction.UsersEntity>();


        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void MyTestMethod1()
        {

        }



        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void MyTestMethod()
        {
            try
            {
                using (TransactionScope tran = new TransactionScope())
                {
                    db.Save(new ADO2Transaction.UsersEntity()
                    {
                        Logintime = DateTime.Now,
                        Name = "无敌撒",
                        Pass = "heihei"
                    });

                    SQLTransactionTest();

                    tran.Complete();
                    Console.WriteLine("yes");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SQLTransactionTest()
        {
            //if (Transaction.Current != null)
            //{
            //    using (var c = Transaction.Current.DependentClone(DependentCloneOption.BlockCommitUntilComplete))
            //    {
            //        db.Save(new ADO2Transaction.UsersEntity()
            //        {
            //            Logintime = DateTime.Now,
            //            Name = "无敌撒123",
            //            Pass = "heihei123heihei123heihei123heihei123heihei123heihei123heihei123heihei123heihei123heihei123heihei123heihei123heihei123heihei123heihei123"
            //        });
            //        c.Complete();
            //    }
            //}
            //else
            //{
            //    using (var tran = db.GetDbTransaction())
            //    {
            //try
            //{
            db.Save(new ADO2Transaction.UsersEntity()
            {
                Logintime = DateTime.Now,
                Name = "无敌撒123",
                Pass = "heihei123heihei123heihei123heihei123heihei123heihei123heihei123heihei123heihei123heihei123heihei123heihei123heihei123heihei123heihei123"
            });
            //        tran.Commit();
            //    }
            //    catch (Exception ex)
            //    {
            //        tran.Rollback();
            //        throw ex;
            //    }
            //}
            //}
        }

    }
}
