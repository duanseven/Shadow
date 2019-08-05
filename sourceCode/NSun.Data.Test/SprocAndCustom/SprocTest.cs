using System;
using System.Data.Common;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSun.Data.Test.SprocAndCustom;

namespace NSun.Data.Test.Sproc
{
    /// <summary>
    /// SprocTest 的摘要说明
    /// </summary>
    [TestClass]
    public class SprocTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            AddTeach d = new AddTeach()
                             {
                                 Name = "dde",
                                 Pass = "ffe"
                             };
            var dd = DBFactory.CreateDBQuery();
            dd.ToProcedureExecute(d);
        }

        [TestMethod]
        public void MyTestMethod()
        {
            var db = DBFactory.CreateDBQuery();
            var add = db.CreateSprocEntity<AddTeach>();
            add.Name = "efef";
            add.Pass = "fefe";
            db.ToProcedureExecute(add);
        }

        [TestMethod]
        public void InputTestMethod()
        {
            var query = DBFactory.CreateDBQuery();
            var s = query.CreateStoredProcedure("addusers");
            s.AddInputParameter("name", System.Data.DbType.AnsiString, "da32");
            s.AddInputParameter("pass", System.Data.DbType.AnsiString, "22da32");
            query.ToExecute(s);
            Console.WriteLine(s.ToDbCommandText());
        }

        [TestMethod]
        public void OutPutTestMethod()
        {
            var query = DBFactory.CreateDBQuery();
            var s = query.CreateStoredProcedure("getusers");
            s.AddOutputParameter("@name", System.Data.DbType.String, 50);
            s.AddInputParameter("@id", System.Data.DbType.Int32, 10542);
            DbCommand sc = null;
            query.ToExecute(s, out sc);
            object o = s.GetOutputParameterValue(sc, "@name");
            Console.WriteLine(o);
        }

        [TestMethod]
        public void IntpuOutPutTestMethod()
        {
            var query = DBFactory.CreateDBQuery();
            var s = query.CreateStoredProcedure("getusers2");
            s.AddInputOutputParameter("@id", System.Data.DbType.Int32, 10, 4);
            DbCommand sc = null;
            query.ToExecute(s, out sc);
            object o = s.GetOutputParameterValue(sc, "@name");
            Console.WriteLine(o);
        }

        [TestMethod]
        public void ReturnTestMetod()
        {
            var query = DBFactory.CreateDBQuery();
            var s = query.CreateStoredProcedure("getusers3");
            s.AddInputParameter("@id", System.Data.DbType.Int32, 10);
            s.AddReturnValueParameter("@id1", System.Data.DbType.Int32, 4);
            DbCommand sc = null;
            query.ToExecute(s, out sc);
            object o = s.GetOutputParameterValue(sc, "@id1");
            Console.WriteLine(o);
        }
    }
}
