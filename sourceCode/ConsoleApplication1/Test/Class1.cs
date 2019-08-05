using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
   public class Class1
    {

        //private static void NewMethod()
        //{
            //var db = Database.Open("db2");
            //db.Stu.Insert(new {  name = "dc", pass = "fffaaa", age = 23});

            //NewMethod();
            //NewMethod1();

            //NSun.Data.Data.DBQueryFactory df = new NSun.Data.Data.DBQueryFactory("db", NSun.Data.SqlType.Sqlserver9);
            //var dbquery = df.CreateDBQuery<StuEntity>();

        //    SqlConnection conn =
        //        new SqlConnection(@"server=.\sqlexpress;database=demo;uid=sa;pwd=sa");
        //    Console.WriteLine(conn.State);

        //    SqlCommand cmd = new SqlCommand("select top 1999 * from stu", conn);
        //    SqlDataAdapter da = new SqlDataAdapter(cmd);
        //    DataSet ds = new DataSet();
        //    Console.WriteLine(conn.State);
        //    da.Fill(ds);
        //    Console.WriteLine(conn.State);

        //    SqlCommand cmd2 = new SqlCommand("select top 10 * from stu", conn);
        //    conn.Open();
        //    IDataReader dr = cmd2.ExecuteReader(CommandBehavior.CloseConnection);
        //    while (dr.Read())
        //    {

        //        Console.WriteLine(dr["id"]);
        //    }
        //    Console.WriteLine(conn.State);
        //    dr.Close();
        //    Console.WriteLine(conn.State);
        //    Console.WriteLine(ds.Tables[0].Rows.Count);
        //}

        //private static void NewMethod1()
        //{
        //    NSun.Data.DBQuery<StuEntity> query = new NSun.Data.DBQuery<StuEntity>(new NSun.Data.Database("db2"));

        //    using (NSun.Data.Transactions.TransactionScope scope = new NSun.Data.Transactions.TransactionScope(query))
        //    {
        //        try
        //        {
        //            query.Save(new StuEntity() { Age = 12, Name = "a", Pass = "b" });
        //            scope.Complete();
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }

        //    Console.WriteLine(query.IsCache);
        //}

        //private static void NewMethod()
        //{
        //    var db = Database.Open("db2");
        //    Stopwatch stop = new Stopwatch();
        //    stop.Start();
        //    var w = db.Stu.Query();
        //    for (int i = 0; i < 1; i++)
        //    {
        //        var o = w.ToList();
        //    }
        //    stop.Stop();
        //    Console.WriteLine(stop.Elapsed);
        //    NSun.Data.DBQuery<StuEntity> query = new NSun.Data.DBQuery<StuEntity>(new NSun.Data.Database("db2"));
        //    query.IsCache = true;
        //    var c = query.CreateQuery();//.SetMaxResults(1);
        //    stop.Restart();
        //    for (int i = 0; i < 10; i++)
        //    {
        //        var o = query.ToList(c);
        //    }
        //    stop.Stop();
        //    Console.WriteLine(stop.Elapsed);
        //}
    }
}
