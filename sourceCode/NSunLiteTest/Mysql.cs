using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NSunLiteTest
{
    /// <summary>
    /// Mysql 的摘要说明
    /// </summary>
    [TestClass]
    public class Mysql
    { 
        [TestMethod]
        public void TestMethod1()
        {
            using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection("Server=127.0.0.1;Database=test;Uid=root;Pwd=root;"))
            {
                conn.Open();
                MySql.Data.MySqlClient.MySqlCommand cmd =
                    new MySql.Data.MySqlClient.MySqlCommand("insert into customer(name,type) values('ss',1)", conn);
                MySql.Data.MySqlClient.MySqlCommand cmd1 =
                   new MySql.Data.MySqlClient.MySqlCommand("insert into customer(name,type) values('ss',1)", conn);
                cmd.ExecuteNonQuery();
                cmd1.ExecuteNonQuery();
            }
        }
    }
}
