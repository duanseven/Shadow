using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NSun.Data;
using NSun.Data.Lambda; 

namespace Simple
{
    class Program
    {
        static void Main(string[] args)
        {
            //var db1 = DBFactory.CreateDBQuery<StudentEntity>();
            //Action a = new Action(() =>
            //                          {
            //                              Thread.Sleep(500);
            //                              var db2 = DBFactory.CreateDBQuery();
            //                              db2.BeginBatch(2);
            //                              db2.Save(new StudentEntity() { ClassId = 1, Age = 12, Name = "11" });
            //                              db2.Save(new StudentEntity() { ClassId = 1, Age = 12, Name = "22" });
            //                              db2.EndBatch();
            //                          });
            //a.BeginInvoke(null, null);
            //db1.BeginBatch(2);
            //db1.Save(new StudentEntity() { ClassId = 1, Age = 12, Name = "33" });
            //db1.Save(new StudentEntity() { ClassId = 1, Age = 12, Name = "44" });
            //db1.Save(new StudentEntity() { ClassId = 1, Age = 12, Name = "45" });
            //Console.ReadKey();
            //db1.EndBatch();

            var db = DBFactory.CreateDBQuery<StudentEntity>();
            
            //Save(db);
            Update(db);
            Delete(db);

            var query = db.CreateQuery();
            query.Where(p1 => p1.Id == 1);
            Console.WriteLine(query.ToDbCommandText());

            query.Reset();//查询重置
            query.Where(p1 => p1.Id == 1 || p1.Id == 2);
            Console.WriteLine(query.ToDbCommandText());

            query.Reset();
            query.Where(p1 => p1.Id > 7 && p1.Name.StartsWith("W"));
            Console.WriteLine(query.ToDbCommandText());

            query.Reset();
            query.Where(p1 => p1.Name.Like("%wui%"));
            Console.WriteLine(query.ToDbCommandText());

            query.Reset();
            query.Where(p1 => p1.Name.Length() == 2);
            Console.WriteLine(query.ToDbCommandText());

            query.Reset();
            query.Where(p1 => p1.Name.ToNumber() == 5);
            Console.WriteLine(query.ToDbCommandText());

            query.Reset();
            query.Where(p1 => p1.Id.Between(1, 8));
            Console.WriteLine(query.ToDbCommandText());

            query.Reset();
            query.Where(p1 => p1.Id.In(1, 2, 3));
            Console.WriteLine(query.ToDbCommandText());

            query.Reset();
            query.Where(p1 => p1.Name.ToUpper() == "WUI");
            Console.WriteLine(query.ToDbCommandText());

            query.Reset();
            query.Where(p1 => p1.Name.ToLower() == "wui");
            Console.WriteLine(query.ToDbCommandText());

            query.Reset();
            query.Where(p2 => p2.Name.ToLower() == "wui");
            query.Where<TeacherEntity>(p2 => p2.Name.ToLower() == "name3");
            Console.WriteLine(query.ToDbCommandText());

            query.Reset();
            query.Where(p1 => p1.Id != 1);
            query.SortBy(p1 => p1.Id.Asc());
            Console.WriteLine(query.ToDbCommandText());

            query.Reset();
            query.Where(p1 => p1.Id != 1);
            query.SortBy(p1 => new[] { p1.Id.Asc(), p1.Name.Desc(), p1.Age.Asc() });
            Console.WriteLine(query.ToDbCommandText());

            query.Reset();
            query.Select(p1 => new { p1.Name, maxid = p1.Id.Max() });
            query.Where(p1 => p1.Id != 1);
            query.GroupBy(p1 => p1.Name);
            Console.WriteLine(query.ToDbCommandText());

            query.Reset();
            query.Select(p1 => new { p1.Name, maxid = p1.Id.Max(), p1.Age });
            query.Where(p1 => p1.Id != 1);
            query.GroupBy(p1 => new { p1.Name, p1.Age });
            Console.WriteLine(query.ToDbCommandText());

            //having
            query.Reset();
            query.Select(p1 => new { p1.Name, maxid = p1.Id.Max() });
            query.Where(p1 => p1.Id != 1);
            query.GroupBy(p1 => p1.Name);
            query.Having(p1 => p1.Id.Max() > 24);
            Console.WriteLine(query.ToDbCommandText());

            //join test two
            query.Reset();
            query.Join<TeacherEntity>((j1, i1) => j1.Id == i1.Id);
            Console.WriteLine(query.ToDbCommandText());

            query.Reset();
            query.Join<TeacherEntity>((j1, i1) => j1.Id == i1.Id);
            query.Where<TeacherEntity>((j1, i1) => j1.Name.Like("wui") && i1.Name == "name13");
            Console.WriteLine(query.ToDbCommandText());


            query.Reset();
            query.Select<TeacherEntity>((p1, p2) => new { p1.Name, p2name = p2.Name });
            query.Join<TeacherEntity>((j1, i1) => j1.Id == i1.Id);
            query.Where(j1 => j1.Name == "wui");
            query.Where<TeacherEntity>(i1 => i1.Name == "name13");
            query.SortBy<TeacherEntity>((p1, p2) => new[] { p1.Id.Desc(), p2.Name.Desc() });
            Console.WriteLine(query.ToDbCommandText());

            query.Reset();
            query.Select<TeacherEntity>((p1, p2) => new { p1.Name, p2name = p2.Name });
            query.Join<TeacherEntity>((j1, i1) => j1.Id == i1.Id);
            query.Join<StudentEntity, TeacherEntity>((j1, i1) => j1.Id == i1.Id);
            Console.WriteLine(query.ToDbCommandText());

            query.Reset();
            query.BindColumn(j1 => new { j1.Name, j1.ClassId })//绑定显示列
                .BindColumn<TeacherEntity>(i1 => new { teach__name = i1.Name, i1.Id })
                .BindColumn<StudentEntity>(i1 => new { i1.Name, i1.Id });
            query.Join<TeacherEntity>((j1, i1) => j1.Id == i1.Id);
            query.LeftJoin<ClassEntity, TeacherEntity>((f1, e1) => f1.Id == e1.Id);
            query.Where<StudentEntity>(s1 => s1.Id != null);
            Console.WriteLine(query.ToDbCommandText());

            query.Reset();
            query.BindColumn(j1 => new { j1.Name, cout = j1.Id.Count() });
            query.Join<TeacherEntity>((j1, i1) => j1.Id == i1.Id);
            query.GroupBy(p1 => p1.Name);
            Console.WriteLine(query.ToDbCommandText());

            query.Reset();
            query.Select(p1 => new
            {
                idadd = p1.Id.CustomMethodExtensions(
                    System.Data.DbType.AnsiString, "dbo", "getidaddone")
            });
            Console.WriteLine(query.ToDbCommandText());

            query.Reset();
            query.Where(p1 => p1.Id.CustomMethodExtensions(
                    System.Data.DbType.AnsiString, "dbo", "getidaddone") == "12");
            Console.WriteLine(query.ToDbCommandText());

            query.Reset();
            query.Where(p1 => p1.Id.CustomMethodExtensions(System.Data.DbType.AnsiString, string.Empty, "getdate", true, false) == "20101025");
            Console.WriteLine(query.ToDbCommandText());

            query.Reset(); 
            query.Where(p1 => p1.Id == 1 || p1.Id == 2);
            query.Select(p => p.Id);
            query.SortBy(p => p.Name.Desc());
            query.GroupBy(p => p.Name);
            Console.WriteLine(query.ToDbCommandText());


            query.Reset();
            //query.Where(p1 => p1.Id == 1);
            query.Select(p => p.Id).Where(p=>p.Name!=null && p.Name.Like("D%")).SortBy(p => p.Id.Asc());
            //query.SortBy(p => p.Name.Desc());
            Console.WriteLine(query.ToDbCommandText());
            Console.ReadKey();        
        }

       
        private static void Save(DBQuery<StudentEntity> db)
        {
            db.OnLog += db_OnLog;           
            db.Save(new StudentEntity() {Age = 12, Name = "dc"});
        }

        static void db_OnLog(string logMsg)
        {
            Console.WriteLine(logMsg);
        }

        private static void Update(DBQuery<StudentEntity> db)
        {
            var update = db.CreateUpdate();            
            update.Where(p1 => p1.Id == 1);
            update.AddColumn(p1 => p1.Name, "测试");
            Console.WriteLine(update.ToDbCommandText());
        }

        private static void Delete(DBQuery<StudentEntity> db)
        {
            var delete = db.CreateDelete();
            delete.Where(p1 => p1.Id == 2);
            Console.WriteLine(delete.ToDbCommandText());
        }

    }
}
