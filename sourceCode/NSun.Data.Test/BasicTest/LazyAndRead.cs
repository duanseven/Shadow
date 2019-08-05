using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NSun.Data.Test.BasicTest
{
    /// <summary>
    /// LazyAndRead 的摘要说明
    /// </summary>
    [TestClass]
    public class LazyAndRead
    {
        [TestMethod]
        public void Test1()
        {
            var db = DBFactory.CreateDBQuery<NSun.Data.Test.Domain.Teach>();
            var entity = db.Get(5);
            foreach (var classes in entity.Classes)
            {
                Console.WriteLine("班级名称:" + classes.Name);
                Console.WriteLine("班级老师:" + classes.TeachInfo.Name);
                Console.WriteLine("这个老师管几个班:" + classes.TeachInfo.Classes.Count);
            }
        }
    }
}
