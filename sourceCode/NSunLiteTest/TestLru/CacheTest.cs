using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSunLiteTest.TestLru
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class CacheTest
    {
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void MyTestMethod()
        { 

            //var c = DBFactory.Instance.CreateDBQuery<UsersInfo2>();
            //c.OnLog += new NSun.Data.LogHandler(c_OnLog);
            //var info = c.ToList();
            //var info2 = c.Load(1);
            //Console.WriteLine(info2.Id + "__" + info2.Name + "__" + info2.Pass);

            NSun.Data.Common.LruDictionary<object, UsersInfo> list =
                new NSun.Data.Common.LruDictionary<object, UsersInfo>(2);
            var w = new UsersInfo() { Id = 12 };
            var w1 = new UsersInfo() { Id = 11 };
            var w2 = new UsersInfo() { Id = 10 };
            list.Add(w.Id, w);
            list.Add(w1.Id, w1);
            list.Add(w2.Id, w2);
            list.Add(9, new UsersInfo() { Id = 9 });

            Console.WriteLine(list[12].Id);

            Dictionary<object, UsersInfo> dic = new Dictionary<object, UsersInfo>();
            dic.Add(w.Id, w);
            dic.TryGetValue(w.Id, out w);
            Console.WriteLine(w.Id);
        }

        void c_OnLog(string logMsg)
        {
            Console.WriteLine(logMsg);
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void MyTestMethod1()
        {
            NSun.Data.Common.LruDependingDictionary<string, string> list =
                new NSun.Data.Common.LruDependingDictionary<string, string>(2);
            
        }
    }
}

