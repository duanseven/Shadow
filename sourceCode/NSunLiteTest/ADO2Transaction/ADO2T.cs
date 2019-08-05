using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using NSun.Data;

namespace NSunLiteTest.ADO2Transaction
{
    public delegate void A();
    /// <summary>
    /// UnitTest1 的摘要说明
    /// </summary>
    [TestClass]
    public class ADO2T
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            { 
                try
                {
                    var st = DBFactory.dbUsers;
                    UsersEntity uss = new UsersEntity()
                                    {
                                        Name = "wudi" + Guid.NewGuid().ToString(),
                                        Pass = "dc" + Guid.NewGuid().ToString()
                                    };
                    st.Save(uss);
                    Console.WriteLine(Transaction.Current);

                    //var st1= DBFactory.dbmsaccess;
                    //UsersInfo us1 = new UsersInfo()
                    //                    {
                    //                        Name = "wudi" + Guid.NewGuid().ToString(),
                    //                        Pass = "dc" + Guid.NewGuid().ToString()
                    //                    };

                    //st1.Save(us1);

                    var st2 = DBFactory.dbUs;
                    UsEntity us = new UsEntity()
                                      {
                                          Name = Guid.NewGuid().ToString(),
                                          Pass = "1111111111111111111111111111111"
                                      };
                    st2.Save(us);  

                    tran.Complete();
                    Console.WriteLine("成功");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        [TestMethod]
        public void TestMethod2()
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                try
                {
                    FileCreate();
                    SaveDB();
                    //FileRead();


                    tran.Complete();
                    Console.WriteLine("成功");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private static void SaveDB()
        {

            var st2 = DBFactory.dbUs;
            UsEntity us = new UsEntity()
            {
                Name = Guid.NewGuid().ToString(),
                Pass = "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };
            st2.Save(us);
        }

        private static void FileCreate()
        {
            string path = "E:\\b.txt";
            var file = File.CreateText(path);
            file.Write("hello ada!");
            file.Flush();
            file.Close();
        }

        private static void FileRead()
        {
            string path = "E:\\b.txt";
            using (var filestream = File.Open(path, FileMode.Open))
            {
                byte[] b = new byte[128];
                using (MemoryStream ms = new MemoryStream())
                {
                    while (filestream.Read(b, 0, b.Length) > 0)
                    {
                        ms.Write(b, 0, b.Length);
                    }
                    Console.WriteLine(Encoding.Default.GetString(ms.GetBuffer()));
                }
            }
        }

        [TestMethod]
        public void MyTestMethod9()
        {
            var db = DBFactory.dbUsers;
            var c = db.Load(1);
            Console.WriteLine(c.Id + "" + c.IsPersistence() );
            var cd= db.LoadOrDefault(222);
            Console.WriteLine(cd == null ? "null" : cd.Id + "");
        }

        [TestMethod]
        public void MyTestMethod()
        {
            Console.WriteLine(((MyEnum)Enum.Parse(typeof(MyEnum), "2")) == MyEnum.Many);
        }

        [TestMethod]
        public void MyTestMethod2()
        {
            //var titleen = DBFactory.dbExamTitle.Load(1);
            //Console.WriteLine(titleen.Id + titleen.Title);

            //foreach (var selectItemEntity in titleen.SelectItems)
            //{
            //    Console.WriteLine(selectItemEntity.Id + selectItemEntity.Selectcontext);
            //}
        }

        [TestMethod]
        public void MyTestMethod3()
        {
            var db= DBFactory.dbUsers;
            UsersEntity ue = new UsersEntity();
            ue.Name = default(string);//DefaultValue<string>.Default;
            ue.Pass = default(string);            
            ue.Logintime = DateTime.Now;
            db.Save(ue);             
        }

        [TestMethod]
        public void MyTestMethod4()
        {
            var db = DBFactory.dbUsers;
            var del= db.CreateDelete();
            del.Join(new UsEntity(), UsEntity._id == UsersEntity._id);
            del.Where(UsersEntity._id == 1);
            Console.WriteLine(del.ToDbCommandText());
        }

        [TestMethod]
        public void MyTestMethod5()
        {
            var db = DBFactory.dbUsers;
            var del = db.CreateUpdate();
            del.AddColumn(UsersEntity._name, "2");
            //del.Join(new UsEntity(), UsEntity._id == UsersEntity._id);
            del.Where(UsersEntity._id == 1);
            Console.WriteLine(del.ToDbCommandText());
        }

        [TestMethod]
        public void WriteFile()
        {
            //IO 写入已有文件
            //var filewrite = File.Open("E:\\a.txt", FileMode.OpenOrCreate);
            //filewrite.Seek(filewrite.Length, SeekOrigin.Begin);
            //var context= "hello dacey";
            //filewrite.Write(Encoding.Default.GetBytes("\r\n"), 0, "\r\n".Length);
            //filewrite.Write(Encoding.Default.GetBytes(context), 0, context.Length);
            //filewrite.Flush();
            //filewrite.Close();
        }

        [TestMethod]
        public void MyTestMethod6()
        {
            UsersEN u = new UsersEN()
                            {
                                Name="wuditeddy",
                                Pass="wudidacey",
                                Logintime=DateTime.Now
                            };            
            DBQuery<UsersEN> db = DBFactoryNew.CreateDBQuery<UsersEN>();//new DBQuery<UsersEN>(Database.Default);
            db.Save(u);
        }

        [TestMethod]
        public void MyTestMethod7()
        {
            //UsersEN u = new UsersEN()
            //{
            //    Name = "wuditeddy",
            //    Pass = "wudidacey",
            //    Logintime = DateTime.Now
            //};
            DBQuery<UsersIdEntity> db = DBFactoryNew.CreateDBQuery<UsersIdEntity>();// new DBQuery<UsersIdEntity>(DBFactory.db);
            SelectSqlSection s = db.CreateQuery();
            s.Where(UsersEN._name.Contains("d"));
            s.SetMaxResults(10);
            foreach (var VARIABLE in db.ToList(s))
            {
                Console.WriteLine(VARIABLE.Id);
            } 
        }

        [TestMethod]
        public void MyTestMethod8()
        {
             
        }
    }

    public enum MyEnum
    {
        Single = 1,
        Many = 2,
        AQ = 3
    }
}
