using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCFClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceStudent.StudentServiceClient sc = new ServiceStudent.StudentServiceClient();
            NSun.Data.SelectSqlSection select = new NSun.Data.SelectSqlSection(new RemotingModel.StudentRemotingModel());
            select.Where(RemotingModel.StudentRemotingModelTable._id > 2 &&
                         RemotingModel.StudentRemotingModelTable._age > 20);
            select.SortBy(RemotingModel.StudentRemotingModelTable._birthday.Asc);

            var list = sc.GetStudent(select);
            foreach (var studentRemotingModel in list)
            {
                Console.WriteLine("姓名: " + studentRemotingModel.Name);
            }
            Console.WriteLine("新学生的编号: " + sc.Save(new RemotingModel.StudentRemotingModel()
                                                       {
                                                           Age = 21,
                                                           Birthday = new DateTime(1981, 11, 22),
                                                           Classid = 1,
                                                           Name = "王嘻嘻",
                                                           Pass = "张嘻嘻"
                                                       }));
            Console.ReadKey();
        }
    }
}
