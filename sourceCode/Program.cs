using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Simple
{
    class Program
    {
        static void Main(string[] args)
        {
            var db1 = DBFactory.CreateDBQuery<StudentEntity>();
            Action a = new Action(() =>
                                      {
                                          Thread.Sleep(500);
                                          var db2 = DBFactory.CreateDBQuery();
                                          db2.BeginBatch(2);
                                          db2.Save(new StudentEntity() { ClassId = 1, Age = 12, Name = "11" });
                                          db2.Save(new StudentEntity() { ClassId = 1, Age = 12, Name = "22" });
                                          db2.EndBatch();
                                      });
            a.BeginInvoke(null, null);
            db1.BeginBatch(2);
            db1.Save(new StudentEntity() { ClassId = 1, Age = 12, Name = "33" });
            db1.Save(new StudentEntity() { ClassId = 1, Age = 12, Name = "44" });
            db1.Save(new StudentEntity() { ClassId = 1, Age = 12, Name = "45" });
            Console.ReadKey();
            db1.EndBatch();
            Console.ReadKey();
        }
    }
}
