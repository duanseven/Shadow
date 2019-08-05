using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
//using MicroOrm;
using System.Diagnostics;
//using NSun.Data.Transactions;
using System.Data.SqlClient;
using System.Data;
using NSun.Data;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = DBFactory.CreateDBQuery<UsersEntity>();
            UsersEntity us = db.Load(1);
            Console.WriteLine(us.books.Count);
            Console.ReadKey();
        }
    }
}
