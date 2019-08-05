using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSun.Data;
using NSunLiteTest.TestLru; 

//using NSunLite;

namespace NSunLiteTest.Custom
{
    /// <summary>
    /// CostomClass 的摘要说明
    /// </summary>
    [TestClass]
    public class CostomClass
    {
        private DBQuery query;
        public CostomClass()
        {
            query = DBFactory.CreateDBQuery(); // new DBQuery(Database.Default);
        }
 
        [TestMethod]
        public void CostomTestCRUDMethod()
        {
            var custom = query.CreateCustomSql("insert into uesrs(name) values(@name)");
            custom.AddInputParameter("@name", System.Data.DbType.AnsiString, 12);
            Console.WriteLine(custom.ToDbCommandText());            
        }

        [TestMethod]
        public void CostomTestSelectMethod()
        {
            var custom = query.CreateCustomSql("select * from users where name like @name");
            custom.AddInputParameter("@name", System.Data.DbType.AnsiString, "%12%");
            Console.WriteLine(custom.ToDbCommandText());            
            Console.WriteLine(custom.ToSubQuery().Sql);
        }


    }
}
