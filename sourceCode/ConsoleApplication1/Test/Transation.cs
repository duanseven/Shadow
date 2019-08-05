using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSun.Data;
using System.Data;

namespace ConsoleApplication1
{
    public class Transation
    {
        public void Bi()
        {
            DBQuery dbquery;
            //1、简单的执行
            dbquery = DBFactory.CreateDBQuery<UsersEntity>();
            dbquery.Save(new UsersEntity()
            {
                Logintime = DateTime.Now,
                Name = "bf" + new Random().Next(),
                Pass = "fb" + new Random().Next()
            });

            //2、开启本地事务
            dbquery = DBFactory.CreateDBQuery<UsersEntity>();
            dbquery.BeginTransaction();
            try
            {
                dbquery.Save(new UsersEntity()
                {
                    Logintime = DateTime.Now,
                    Name = "bf1" + new Random().Next(),
                    Pass = "fb" + new Random().Next()
                });
                dbquery.Commit();
            }
            catch (Exception e)
            {
                dbquery.Rollback();
            }
            finally
            {
                dbquery.Close();
            }

            //3开启全局事务
            dbquery = DBFactory.CreateDBQuery<UsersEntity>();
            using (NSun.Data.Transactions.TransactionScope tran = new NSun.Data.Transactions.TransactionScope(dbquery, IsolationLevel.ReadCommitted))
            {
                try
                {
                    dbquery.Save(new UsersEntity()
                    {
                        Logintime = DateTime.Now,
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

        public void Bi2()
        {
            DBQuery dbquery;
            //1、简单的执行
            dbquery = DBFactory.CreateDBQuery<UsersEntity>();

            using (var tran = dbquery.GetDbTransaction())
            {
                try
                {
                    dbquery.Save(new UsersEntity()
                    {
                        Logintime = DateTime.Now,
                        Name = "bf" + new Random().Next(),
                        Pass = "fb" + new Random().Next()
                    });
                    dbquery.Save(new UsersEntity()
                    {
                        Logintime = DateTime.Now,
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

        public void Bi3()
        {
            DBQuery dbquery;
            using (dbquery = DBFactory.CreateDBQuery<UsersEntity>())
            {
                //dbquery.IsUnitOfWork = true;
                dbquery.BeginUnitOfWork();
                try
                {
                    UsersEntity ue = new UsersEntity()
                    {
                        Logintime = DateTime.Now,
                        Name = "dafa",
                        Pass = "ewew"
                    };
                    dbquery.Save<UsersEntity>(ue);

                    var query = dbquery.CreateQuery<UsersEntity>();
                    query.Where(UsersEntity._id == ue.Id);
                    var ir = dbquery.ToDataSet(query);

                    dbquery.Commit(true);
                }
                catch (Exception e)
                {
                    throw;
                }
            }
        }
    }
}
