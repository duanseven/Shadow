using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSun.Data.Test.Domain;

namespace NSun.Data.Test
{
    /// <summary>
    /// CustomTest 的摘要说明
    /// </summary>
    [TestClass]
    public class CustomTest
    {
        [TestMethod]
        public void Custom1()
        {
            var db= DBFactory.CreateDBQuery();
            var cust= db.CreateCustomSql("select * from teach");
            var list= db.ToList<Teach>(cust);
            foreach (var student in list)
            {
                Console.WriteLine(student.Id);
            }            
        }
    }
}
