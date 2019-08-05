using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NSun.Data.Test.MSSQL
{
    /// <summary>
    /// QueryRelation 的摘要说明
    /// </summary>
    [TestClass]
    public class QueryRelation
    { 
        [TestMethod]
        public void TestMethod1()
        {            
            var db= DBFactory.CreateDBQuery<UsersEntity>();
            var user = db.Get(1);
            Console.WriteLine(user.Usersinfo.CodeId);
            Console.WriteLine(user.Id);
        }
    }
}
