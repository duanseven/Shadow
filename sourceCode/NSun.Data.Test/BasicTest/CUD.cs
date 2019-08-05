using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSun.Data.Test.Domain;

namespace NSun.Data.Test.Basic
{
    /// <summary>
    /// UnitTest1 的摘要说明
    /// </summary>
    [TestClass]
    public class Insert
    {
        private DBQuery<Teach> TeachDB { get; set; }

        public Insert()
        {
            TeachDB = DBFactory.CreateDBQuery<Teach>();
        }

        [TestMethod]
        public void InsertMSSQL()
        {

            Teach t = new Teach()
                          {
                              Name = "dc",
                              Pass = "ada"
                          };
            TeachDB.Save(t);
            Console.WriteLine(t.Id);
        }

        [TestMethod]
        public void InsertMSSQL2()
        {
            var insert = TeachDB.CreateInsert();
            
            insert.AddColumn(TeachMapping._name, "ada1");
            //insert.AddColumn(TeachMapping._pass, "dc");
            int res = TeachDB.ToExecuteReturnAutoIncrementId(insert, TeachMapping._id);
            Console.WriteLine(res);
        }

        [TestMethod]
        public void Update()
        {
            var teach = TeachDB.Get(3);
            teach.Pass = "dc1";
            TeachDB.Save(teach);
            Console.WriteLine(teach.Pass);
        }

        [TestMethod]
        public void Update2()
        {
            var update = TeachDB.CreateUpdate();
            update.AddColumn(TeachMapping._name, "dacey");
            update.Where(TeachMapping._id == 3);
            int res = TeachDB.Update(update);
            Console.WriteLine(res);
        }

        [TestMethod]
        public void Delete()
        {
            Teach t = new Teach()
            {
                Name = "dc2",
                Pass = "ada2"
            };
            Console.WriteLine(TeachDB.Save(t));
            Console.WriteLine(
            TeachDB.Delete(t));
        }

        [TestMethod]
        public void Delete2()
        {
            Console.WriteLine(
            TeachDB.DeleteKey(3));
        }

        [TestMethod]
        public void Delete3()
        {
            var delete = TeachDB.CreateDelete();
            delete.Where(TeachMapping._id < 2);
            TeachDB.Delete(delete);
        }

        [TestMethod]
        public void Delete4()
        {
            TeachDB.Delete(new object[] {1, 2, 3});
        }

    }
}
