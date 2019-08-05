using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ConsoleApplication1.Test
{
    class Class3
    {

        private static void NewMethod4()
        {
            var fields = typeof(TestClass).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            foreach (var fieldInfo in fields)
            {
                Console.WriteLine(fieldInfo.Name);
            }
        }

        private static void NewMethod3()
        {
            int count = 100;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var db = DBFactory.Instance.CreateDBQuery<UsersEntity>();
            for (int i = 0; i < count; i++)
            {
                NewMethod1();
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
            sw.Reset();
            sw.Start();
            for (int i = 0; i < count; i++)
            {
                NewMethod11();
            }
            Console.WriteLine(sw.Elapsed);
            sw.Stop();
        }

        public static void J(object i)
        {
            Console.WriteLine("JI");
        }

        public static void J(UserInfoEntity u)
        {
            Console.WriteLine("JU");
        }

        private static void NewMethod2()
        {
            var db = DBFactory.CreateDBQuery<UsersEntity>();

            db.BeginBatch(10, IsolationLevel.ReadCommitted);

            for (int i = 0; i < 10; i++)
            {
                db.Save(new UsersEntity()
                {
                    Name = "dqqw" + i,
                    Pass = "qwr" + i,
                    Logintime = DateTime.Now
                });
            }

            db.EndBatch();

            //var u= db.Get(1);

            //foreach (BookEntity bookEntity in u.books)
            //{
            //    Console.WriteLine(bookEntity.Name);
            //    foreach (UsersEntity usersEntity in bookEntity.Users)
            //    {
            //        Console.WriteLine(usersEntity.Name);   
            //    }               
            //}
            //Console.WriteLine(u.Name);
            //Console.WriteLine(u.Usersinfo.CodeId);
        }

        private static void NewMethod1()
        {
            var db = DBFactory.CreateDBQuery<UsersEntity>();
            UsersEntity user = db.Get(1);
            int d = user.Id;
            var w = user.Useres_book;
            var b = user.Useres_book;
            var book = user.Useres_book[0, false];
            var s = book.Name;
            foreach (BookEntity bookEntity in user.Useres_book)
            {
                int i = bookEntity.Id;//Console.WriteLine("first-bookid:" + bookEntity.Id);
                int j = bookEntity.Book_useres.Id;//Console.WriteLine("first-userid:" + bookEntity.Book_useres.Id);
                foreach (BookEntity entity in bookEntity.Book_useres.Useres_book)
                {
                    int m = entity.Id;
                    int n = entity.Book_useres.Id;
                    //Console.WriteLine("second-bookid:" + entity.Id);
                    //Console.WriteLine("second-userid:" + entity.Book_useres.Id);
                }
            }
        }

        private static void NewMethod11()
        {
            var db = DBFactory2.CreateDBQuery<UsersEntity>();
            UsersEntity user = db.Get(1);
            int d = user.Id;
            var w = user.Useres_book;
            var b = user.Useres_book;
            var book = user.Useres_book[0, false];
            var s = book.Name;
            foreach (BookEntity bookEntity in user.Useres_book)
            {
                int i = bookEntity.Id;//Console.WriteLine("first-bookid:" + bookEntity.Id);
                int j = bookEntity.Book_useres.Id;//Console.WriteLine("first-userid:" + bookEntity.Book_useres.Id);
                foreach (BookEntity entity in bookEntity.Book_useres.Useres_book)
                {
                    int m = entity.Id;
                    int n = entity.Book_useres.Id;
                    //Console.WriteLine("second-bookid:" + entity.Id);
                    //Console.WriteLine("second-userid:" + entity.Book_useres.Id);
                }
            }
        }

        private static void NewMethod()
        {
            var db = DBFactory.CreateDBQuery<UsersEntity>();
            var db2 = DBFactory.CreateDBQuery<BookEntity>();
            int id = db.Save(new UsersEntity()
            {
                Logintime = DateTime.Now,
                Name = "ns7",
                Pass = "ps7"
            });
            db2.Save(new BookEntity()
            {
                Userid = id,
                Name = "da"
            });
        }
    }
}
