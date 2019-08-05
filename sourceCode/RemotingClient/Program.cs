using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using RemotingModel;

namespace RemotingClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //RemotingConfiguration.Configure("RemotingClient.exe.config");

            var b = (IServiceSchool.IBookService)Activator.GetObject(typeof(IServiceSchool.IBookService), "tcp://localhost:2133/bookservice");
            var where = BookRemotingModelTable._id < 5 && BookRemotingModelTable._name.StartsWith("c");
            NSun.Data.SelectSqlSection select = new NSun.Data.SelectSqlSection<BookRemotingModel>();
            select.Where(where);
            select.SortBy(BookRemotingModelTable._price.Asc);
            foreach (var book in b.GetBooks(select))
            {
                Console.WriteLine("名称:" + book.Name + " 价格:" + book.Price);
            }

            var stu = (IServiceSchool.IStudentService)Activator.GetObject(typeof(IServiceSchool.IStudentService), "tcp://localhost:2133/studentservice");

            var lis = stu.GetStudents(null);
            foreach (var studentRemotingModel in lis)
            {
                if (studentRemotingModel.StudentInfo == null)
                {

                }
                Console.WriteLine("学生名称:" + studentRemotingModel.Name + " 年龄: " + studentRemotingModel.Age);
            }

            var student = stu.GetStudent(1);
            Console.WriteLine("ID为1的学生名称:" + student.Name);

            StudentRemotingModel entity = new StudentRemotingModel()
                                              {
                                                  Name = "奥特曼",
                                                  Pass = "12345",
                                                  Classid = 1,
                                                  Age = 21,
                                                  Birthday = new DateTime(1991, 11, 22)
                                              };
            Console.WriteLine("新学生ID：" + stu.Save(entity));
            Console.ReadKey();
        }
    }
}
