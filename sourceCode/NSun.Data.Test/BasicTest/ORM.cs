using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSun.Data.Test.Domain;

namespace NSun.Data.Test.BasicTest
{
    /// <summary>
    /// ORM 的摘要说明
    /// </summary>
    [TestClass]
    public class ORM
    {
        [TestMethod]
        public void OneToMany()
        {
            int count = 10000;
            var db = DBFactory.CreateDBQuery<Teach>();
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start(); 
            foreach (var teach in db.ToList())
            {
                var c = db.Load(teach);
                Console.WriteLine(c.Classes.Count);
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }

        [TestMethod]
        public void OneToOne()
        {

        }
    }
}
